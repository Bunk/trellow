﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LinqToVisualTree;

namespace trello.Extensions
{
    public static class ItemsControlExtensions
    {
        public static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl)
        {
            var itemsHostPanel = itemsControl.Tag as Panel;
            if (itemsHostPanel == null)
            {
                itemsHostPanel = itemsControl.Descendants<Panel>().Cast<Panel>().FirstOrDefault(p => p.IsItemsHost);
                itemsControl.Tag = itemsHostPanel;
            }

            var vsp = itemsHostPanel as VirtualizingStackPanel;
            if (vsp != null)
            {
                return GetItemsInView(itemsControl, vsp);
            }
            
            return Enumerable.Range(0, itemsControl.Items.Count)
                             .Select(index => itemsControl.ItemContainerGenerator.ContainerFromIndex(index))
                             .Cast<FrameworkElement>()
                             .Where(c => c.GetRelativePositionIn(itemsControl).Y + c.ActualHeight > 0)
                             .Where(c => c.GetRelativePositionIn(itemsControl).Y - c.ActualHeight <
                                         itemsControl.ActualHeight);
        }

        private static IEnumerable<FrameworkElement> GetItemsInView(this ItemsControl itemsControl,
                                                                   VirtualizingStackPanel vsp)
        {
            var firstVisibleItem = (int) vsp.VerticalOffset;
            var visibleItemCount = (int) vsp.ViewportHeight;
            for (var index = firstVisibleItem; index <= firstVisibleItem + visibleItemCount + 3; index++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
                if (item == null)
                    continue;

                yield return item;
            }
        }
    }
}
