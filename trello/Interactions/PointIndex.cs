using System;
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

        public int IndexOf(Value point)
        {
            return _points.IndexOf(point);
        }

        public Value GetPotentialItem(double targetPoint)
        {
            var index = IndexOf(new Point(0, targetPoint));
            return index < 0 ? null : _points[index];
        }

        public Value Get(int index)
        {
            Contract.Assert(index < _points.Count);
            return _points[index];
        }

        public void Add(Value item)
        {
            _points.Add(item);
        }

        public void Sort(Comparison<Value> func)
        {
            _points.Sort(func);
        }

        public void Reindex(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex)
                throw new InvalidOperationException("You should only shift when indexes change.");

            if (oldIndex > newIndex)
                IntExtensions.Swap(ref oldIndex, ref newIndex);

            for (var i = oldIndex; i < newIndex; i++)
                SwapIndex(i, i + 1);

            Sort((lhs, rhs) => lhs.Position.Top.CompareTo(rhs.Position.Top));
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

            var aTop = itemA.Position.Top + itemB.Position.Height;
            var bTop = itemA.Position.Top;

            itemA.Reposition(new Point(0, aTop));
            itemB.Reposition(new Point(0, bTop));
        }

        public class Value
        {
            public Rect Position { get; private set; }

            public void Reposition(Point offset)
            {
                Position = new Rect(offset.X,
                                    offset.Y,
                                    Position.Width,
                                    Position.Height);
            }

            public static Value Create(FrameworkElement element, FrameworkElement relativeTo)
            {
                var position = element.GetRelativePositionIn(relativeTo);
                var rect = new Rect(position, element.RenderSize);
                return new Value { Position = rect };
            }
        }
    }
}