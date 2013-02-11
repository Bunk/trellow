using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace trello
{
    public class PropertyChangedEventManager
    {
        private static readonly Lazy<PropertyChangedEventManager> _instance
            = new Lazy<PropertyChangedEventManager>(() => new PropertyChangedEventManager());

        private static readonly object SyncLock = new object();
        
        private Dictionary<string, List<WeakReference>> _list;

        private static PropertyChangedEventManager Instance
        {
            get { return _instance.Value; }
        }

        public static void AddListener(INotifyPropertyChanged source, IWeakEventListener listener, string propertyName)
        {
            Instance.PrivateAddListener(source, listener, propertyName);
        }

        public static void RemoveListener(INotifyPropertyChanged source, IWeakEventListener listener, string propertyName)
        {
            Instance.PrivateRemoveListener(source, listener, propertyName);
        }

        private void PrivateAddListener(INotifyPropertyChanged source, IWeakEventListener listener, string propertyName)
        {
            if (_list == null)
                _list = new Dictionary<string, List<WeakReference>>();

            lock(SyncLock)
            {
                var reference = new WeakReference(listener);
                if (_list.ContainsKey(propertyName))
                {
                    _list[propertyName].Add(reference);
                }
                else
                {
                    var list = new List<WeakReference> {reference};
                    _list[propertyName] = list;
                }
                StartListening(source);
            }
        }

        private void PrivateRemoveListener(INotifyPropertyChanged source, IWeakEventListener listener, string propertyName)
        {
            if (_list == null)
                return;

            lock(SyncLock)
            {
                if (!_list.ContainsKey(propertyName))
                    return;

                StopListening(source);

                var reference = _list[propertyName].FirstOrDefault(item => item.Target.Equals(listener));
                if (reference != null)
                {
                    _list[propertyName].Remove(reference);
                }
            }
        }

        private void StartListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged += PropertyChanged;
        }

        private void StopListening(INotifyPropertyChanged source)
        {
            source.PropertyChanged -= PropertyChanged;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var list = _list[args.PropertyName];
            if (list == null)
                return;

            foreach (var item in list)
            {
                var eventItem = item.Target as IWeakEventListener;
                if (eventItem != null && item.IsAlive)
                    eventItem.ReceiveWeakEvent(GetType(), sender, args);
            }
        }
    }

    public interface IWeakEventListener
    {
        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">The type of the WeakEventManager calling this method</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>true if the listener handled the event.  It is considered an error by the
        /// WeakEventManager handling to register a listener for an event that the listener does not
        /// handle.  Regardless, the method should return false if it receives an event that it does
        /// not recognize or handle.</returns>
        bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e);
    }

    public class PropertyObserver<TPropertySource> : IWeakEventListener
        where TPropertySource : INotifyPropertyChanged
    {
        readonly Dictionary<string, Action<TPropertySource>> _propertyNameToHandlerMap;
        readonly WeakReference _propertySourceRef;

        public PropertyObserver(TPropertySource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            _propertySourceRef = new WeakReference(source);
            _propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
        }

        /// <summary>
        /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
        /// </summary>
        /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
        /// <param name="handler">The callback to invoke when the property has changed.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<TPropertySource> RegisterHandler(
            Expression<Func<TPropertySource, object>> expression,
            Action<TPropertySource> handler)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            if (handler == null)
                throw new ArgumentNullException("handler");

            TPropertySource propertySource = this.GetPropertySource();
            if (propertySource != null)
            {
                _propertyNameToHandlerMap[propertyName] = handler;
                PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
            }

            return this;
        }

        /// <summary>
        /// Removes the callback associated with the specified property.
        /// </summary>
        /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<TPropertySource> UnregisterHandler(Expression<Func<TPropertySource, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            TPropertySource propertySource = this.GetPropertySource();
            if (propertySource != null)
            {
                if (_propertyNameToHandlerMap.ContainsKey(propertyName))
                {
                    _propertyNameToHandlerMap.Remove(propertyName);
                    PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                }
            }

            return this;
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType != typeof(PropertyChangedEventManager))
                return false;

            var propertyName = ((PropertyChangedEventArgs) e).PropertyName;
            var propertySource = (TPropertySource) sender;

            if (string.IsNullOrEmpty(propertyName))
            {
                // when the property name is empty, all properties are considered to be invalidated.
                // Iterate over a copy of the list of handlers, in case a handler is registered by a
                // callback.
                foreach (var handler in _propertyNameToHandlerMap.Values.ToArray())
                    handler(propertySource);

                return true;
            }
            else
            {
                Action<TPropertySource> handler;
                if (_propertyNameToHandlerMap.TryGetValue(propertyName, out handler))
                {
                    handler(propertySource);
                    return true;
                }
            }

            return false;
        }

        static string GetPropertyName(Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression = null;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) lambda.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;
                if (propertyInfo != null)
                    return propertyInfo.Name;
            }

            return null;
        }

        TPropertySource GetPropertySource()
        {
            try
            {
                return (TPropertySource)_propertySourceRef.Target;
            }
            catch
            {
                return default(TPropertySource);
            }
        }
    }
}
