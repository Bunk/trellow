﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace trello.Extensions
{
    public static class FrameworkElementExtensions
    {
        public struct TransformOffset
        {
            public double Value { get; set; }
            public CompositeTransform Transform { get; set; }
        }

        public static void Animate(this DependencyObject target, double? from, double? to, object propertyPath,
                                   int duration, int startTime,
                                   IEasingFunction easing = null, Action completed = null)
        
        {
            if (easing == null)
                easing = new SineEase();

            var animation = new DoubleAnimation
            {
                To = to,
                From = @from,
                EasingFunction = easing,
                Duration = TimeSpan.FromMilliseconds(duration)
            };
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));

            var storyBoard = new Storyboard {BeginTime = TimeSpan.FromMilliseconds(startTime)};

            if (completed != null)
                storyBoard.Completed += (sender, args) => completed();

            storyBoard.Children.Add(animation);
            storyBoard.Begin();
        }

        public static TransformOffset GetVerticalOffset(this FrameworkElement fe)
        {
            var trans = fe.RenderTransform as CompositeTransform;
            if (trans == null)
            {
                trans = new CompositeTransform { TranslateY = 0 };
                fe.RenderTransform = trans;
            }
            return new TransformOffset
            {
                Transform = trans,
                Value = trans.TranslateY
            };
        }

        public static TransformOffset GetHorizontalOffset(this FrameworkElement fe)
        {
            var trans = fe.RenderTransform as CompositeTransform;
            if (trans == null)
            {
                trans = new CompositeTransform { TranslateX = 0 };
                fe.RenderTransform = trans;
            }
            return new TransformOffset
            {
                Transform = trans,
                Value = trans.TranslateX
            };
        }

        public static void SetVerticalOffset(this FrameworkElement fe, double offset)
        {
            var composite = fe.RenderTransform as CompositeTransform;
            if (composite == null)
            {
                fe.RenderTransform = new CompositeTransform {TranslateY = offset};
            }
            else
            {
                composite.TranslateY = offset;
            }
        }

        public static void SetHorizontalOffset(this FrameworkElement fe, double offset)
        {
            var composite = fe.RenderTransform as CompositeTransform;
            if (composite == null)
            {
                fe.RenderTransform = new CompositeTransform { TranslateX = offset };
            }
            else
            {
                composite.TranslateX = offset;
            }
        }

        public static void SetRotation(this FrameworkElement fe, double degrees)
        {
            var composite = fe.RenderTransform as CompositeTransform;
            if (composite == null)
            {
                fe.RenderTransform = new CompositeTransform {Rotation = degrees};
            }
            else
            {
                composite.Rotation = degrees;
            }
        }
    }
}