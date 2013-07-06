using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using trello.Extensions;

namespace trello.Interactions
{
    public class PointIndex
    {
        private readonly List<Value> _points = new List<Value>();

        /// <summary>
        /// Returns the index location for a given point in the index.
        /// </summary>
        /// <param name="point">The point to do a hit test against.</param>
        /// <returns>Positive integer if a hit test matches, or -1 if not.</returns>
        public int IndexOf(Point point)
        {
            var points = _points.ToList();
            for (var i = 0; i < points.Count; i++)
            {
                if (points[i].Position.ContainsInclusive(point))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index location of a given value in the index.
        /// </summary>
        public int IndexOf(Value point)
        {
            return _points.IndexOf(point);
        }

        /// <summary>
        /// Returns the item if the target point has a hit test match in the index.
        /// </summary>
        /// <param name="targetPoint">Point to do a hit test against</param>
        public Value GetPotentialItem(Point targetPoint)
        {
            var index = IndexOf(targetPoint);
            return index < 0 ? null : _points[index];
        }

        /// <summary>
        /// Returns the index value at the given index location.
        /// </summary>
        public Value Get(int index)
        {
            Contract.Assert(index < _points.Count);
            return _points[index];
        }

        /// <summary>
        /// Adds the specified value into the point index.  The location in the index is determined by
        /// its y-coordinate relative to other items already in the list.  The resulting index will
        /// be sorted from lowest y-coordinate to highest.
        /// </summary>
        public void Add(Value item)
        {
            var insertionIndex = FindSuitableIndex(item.Position);

            _points.Insert(insertionIndex, item);

            // adjust the values below the newly inserted one in the list accordingly when inserting a new value
            for (var i = (insertionIndex + 1); i < _points.Count; i++)
            {
                var current = _points[i].Position;
                _points[i].Reposition(new Point(0, current.Y + item.Position.Height));
            }
        }

        /// <summary>
        /// Swaps the indexed items beginning with the old index until the new index.
        /// </summary>
        public void ShuffleItems(int oldIndex, int newIndex)
        {
            // NOTE: moving up / down in the list is not a transitive operation on more than one index change
            if (oldIndex > newIndex)
            {
                // Moving up in the list
                for (var i = oldIndex; i > newIndex; i--)
                    SwapIndex(i, i - 1);
            }
            else
            {
                // Moving down in the list
                for (var i = oldIndex; i < newIndex; i++)
                    SwapIndex(i, i + 1);
            }
        }

        private int FindSuitableIndex(Rect position)
        {
            // optimize for appending to the tail as the common case.
            for (var i = _points.Count - 1; i >= 0; i--)
            {
                if (position.Top >= _points[i].Position.Bottom)
                    return i + 1;
            }
            return 0;
        }

        private void SwapIndex(int indexFrom, int indexTo)
        {
            if (indexFrom == indexTo) return;

            if (indexFrom < 0)
                indexFrom = 0;

            if (indexTo < 0)
                indexTo = 0;

            if (indexFrom > indexTo)
                IntExtensions.Swap(ref indexFrom, ref indexTo);

            var itemA = _points[indexFrom];
            var itemB = _points[indexTo];

            // note: these are mutable, so if we store the values in local scope
            var posA = itemA.Position.Top + itemB.Position.Height;
            var posB = itemA.Position.Top;

            itemA.Reposition(new Point(0, posA));
            itemB.Reposition(new Point(0, posB));

            _points[indexFrom] = itemB;
            _points[indexTo] = itemA;
        }

        public class Value
        {
            public Rect Position { get; private set; }

            public void Reposition(Point position)
            {
                Position = new Rect(position.X,
                                    position.Y,
                                    Position.Width,
                                    Position.Height);
            }

            public static Value Create(FrameworkElement element, FrameworkElement relativeTo)
            {
                var position = element.GetRelativePositionIn(relativeTo);
                var rect = new Rect(position, element.RenderSize);
                return new Value {Position = rect};
            }

            public override string ToString()
            {
                return string.Format("({0}) :: (Bottom={1})", Position, Position.Bottom);
            }
        }
    }
}