using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace trello
{
    public class CollectionChangedWeakEventSource : WeakEventSourceBase<INotifyCollectionChanged>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override WeakEventListenerBase CreateWeakEventListener(INotifyCollectionChanged eventObject)
        {
            var weakListener = new WeakEventListener<CollectionChangedWeakEventSource,
                INotifyCollectionChanged,
                NotifyCollectionChangedEventArgs>(this, eventObject);

            weakListener.OnDetachAction = (listener, source) => source.CollectionChanged -= listener.OnEvent;
            weakListener.OnEventAction = (instance, source, e) =>
            {
                if (instance.CollectionChanged != null)
                    instance.CollectionChanged(source, e);
            };
            eventObject.CollectionChanged += weakListener.OnEvent;

            return weakListener;
        }
    }

    public class PropertyChangedWeakEventSource : WeakEventSourceBase<INotifyPropertyChanged>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected override WeakEventListenerBase CreateWeakEventListener(INotifyPropertyChanged eventObject)
        {
            var weakListener = new WeakEventListener<PropertyChangedWeakEventSource,
                INotifyPropertyChanged,
                PropertyChangedEventArgs>(this, eventObject);

            weakListener.OnDetachAction = (listener, source) => source.PropertyChanged -= listener.OnEvent;
            weakListener.OnEventAction = (instance, source, e) =>
            {
                if (instance.PropertyChanged != null)
                    instance.PropertyChanged(source, e);
            };
            eventObject.PropertyChanged += weakListener.OnEvent;

            return weakListener;
        }
    }

    public class WeakEventListener<TInstance, TSource, TEventArgs> : WeakEventListenerBase
        where TInstance : class 
        where TSource : class
    {
        /// <summary>
        /// WeakReference to the rootInstance listening for the event.
        /// </summary>
        private readonly WeakReference _weakInstance;

        /// <summary>
        /// To hold a reference to source object. With this instance the WeakEventListener 
        /// can guarantee that the handler gets unregistered when listener is released.
        /// </summary>
        private readonly WeakReference _weakSource;

        /// <summary>
        /// Delegate to the method to call when the event fires.
        /// </summary>
        private Action<TInstance, object, TEventArgs> _onEventAction;

        /// <summary>
        /// Delegate to the method to call when detaching from the event.
        /// </summary>
        private Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> _onDetachAction;

        public WeakEventListener(TInstance instance, TSource source)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (source == null)
                throw new ArgumentNullException("source");

            _weakInstance = new WeakReference(instance);
            _weakSource = new WeakReference(source);
        }

        /// <summary>
        /// Gets or sets the method to call when the event fires.
        /// </summary>
        public Action<TInstance, object, TEventArgs> OnEventAction
        {
            get { return _onEventAction; }
            set
            {
                // CAUTION: Don't remove this check, as it can cause a memory leak
                if (value != null && !value.Method.IsStatic)
                    throw new ArgumentException("OnEventAction method must be static " +
                                                "otherwise the event WeakEventListner class " +
                                                "does not prevent memory leaks.");

                _onEventAction = value;
            }
        }

        /// <summary>
        /// Gets or sets the method to call when detaching from the event.
        /// </summary>
        internal Action<WeakEventListener<TInstance, TSource, TEventArgs>, TSource> OnDetachAction
        {
            get { return _onDetachAction; }
            set
            {

                // CAUTION: Don't remove this check, as it can cause a memory leak
                if (value != null && !value.Method.IsStatic)
                    throw new ArgumentException("OnDetachAction method must be static otherwise the " +
                                                "event WeakEventListner cannot guarantee to unregister " +
                                                "the handler.");

                _onDetachAction = value;
            }
        }

        /// <summary>
        /// Handler for the subscribed event calls OnEventAction to handle it.
        /// </summary>
        /// <param name="source">Event source.</param>
        /// <param name="eventArgs">Event arguments.</param>
        public void OnEvent(object source, TEventArgs eventArgs)
        {
            var target = (TInstance)_weakInstance.Target;
            if (null != target)
            {
                // Call registered action
                if (null != OnEventAction)
                {
                    OnEventAction(target, source, eventArgs);
                }
            }
            else
            {
                // Detach from event
                Detach();
            }
        }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        public override void Detach()
        {
            var source = (TSource)_weakSource.Target;
            if (null == OnDetachAction || source == null) 
                return;

            // Passing the source instance also, because of static event handlers
            OnDetachAction(this, source);
            OnDetachAction = null;
        }

    }

    public abstract class WeakEventSourceBase<TSource> 
        where TSource : class
    {
        private WeakReference _weakEventSource;
        private WeakReference _weakListener;

        /// <summary>
        /// Gets the event source instance which this listener is using.
        /// </summary>
        /// <remarks>
        /// The reference to the event source is weak.
        /// </remarks>
        public object EventSource
        {
            get
            {
                if (_weakEventSource == null)
                    return null;

                return _weakEventSource.Target;
            }
        }

        /// <summary>
        /// Set the event source for this instance. 
        /// When passing a new event source it replaces the event source the 
        /// listener is listen for an event. When passing null/nothing is detaches 
        /// the previous event source from this event listener. 
        /// </summary>
        /// <param name="eventSource">The event source instance.</param>
        public void SetEventSource(object eventSource)
        {
            // The listener can just listen for one event source.
            // Detach the previous one.
            Detach();

            _weakEventSource = new WeakReference(eventSource);

            var eventObject = eventSource as TSource;
            if (eventObject == null)
                return;

            var weakListener = CreateWeakEventListenerInternal(eventObject);
            if (weakListener == null)
                throw new InvalidOperationException("The method CreateWeakEventListener must return a value.");

            // Store the weak listener as a weak reference (used in Detach)
            _weakListener = new WeakReference(weakListener);
        }

        private WeakEventListenerBase CreateWeakEventListenerInternal(TSource eventObject)
        {
            // todo: sanity checks
            return CreateWeakEventListener(eventObject);
        }

        /// <summary>
        /// When overridden in a derived class, it creates the weak event listener for the given event source.
        /// </summary>
        /// <param name="eventObject">The event source instance to listen for an event</param>
        /// <returns>Return the weak event listener instance</returns>
        protected abstract WeakEventListenerBase CreateWeakEventListener(TSource eventObject);

        /// <summary>
        /// Detaches the event from the event source.
        /// </summary>
        public void Detach()
        {
            if (_weakListener != null)
            {
                // Do it the GC safe way, since an object could potentially be reclaimed
                // for garbage collection immediately after the IsAlive property returns true
                var target = _weakListener.Target as WeakEventListenerBase;
                if (target != null)
                    target.Detach();
            }

            _weakEventSource = null;
            _weakListener = null;
        }
    }

    public abstract class WeakEventListenerBase
    {
        public abstract void Detach();
    }

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
