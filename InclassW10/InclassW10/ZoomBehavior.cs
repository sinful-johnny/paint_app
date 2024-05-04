using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace InclassW10
{
    public class ZoomBehavior : DependencyObject
    {
        #region IsEnabled attached property

        // Required
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
          "IsEnabled", typeof(bool), typeof(ZoomBehavior), new PropertyMetadata(default(bool), ZoomBehavior.OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject attachingElement, bool value) => attachingElement.SetValue(ZoomBehavior.IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject attachingElement) => (bool)attachingElement.GetValue(ZoomBehavior.IsEnabledProperty);

        #endregion

        #region ZoomFactor attached property

        // Optional
        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.RegisterAttached(
          "ZoomFactor", typeof(double), typeof(ZoomBehavior), new PropertyMetadata(0.1));

        public static void SetZoomFactor(DependencyObject attachingElement, double value) => attachingElement.SetValue(ZoomBehavior.ZoomFactorProperty, value);

        public static double GetZoomFactor(DependencyObject attachingElement) => (double)attachingElement.GetValue(ZoomBehavior.ZoomFactorProperty);

        #endregion

        #region ScrollViewer attached property

        // Optional
        public static readonly DependencyProperty ScrollViewerProperty = DependencyProperty.RegisterAttached(
          "ScrollViewer", typeof(ScrollViewer), typeof(ZoomBehavior), new PropertyMetadata(default(ScrollViewer)));

        public static void SetScrollViewer(DependencyObject attachingElement, ScrollViewer value) => attachingElement.SetValue(ZoomBehavior.ScrollViewerProperty, value);

        public static ScrollViewer GetScrollViewer(DependencyObject attachingElement) => (ScrollViewer)attachingElement.GetValue(ZoomBehavior.ScrollViewerProperty);

        #endregion
        private static void OnIsEnabledChanged(DependencyObject attachingElement, DependencyPropertyChangedEventArgs e)
        {
            if (!(attachingElement is FrameworkElement frameworkElement))
            {
                throw new ArgumentException("Attaching element must be of type FrameworkElement");
            }

            bool isEnabled = (bool)e.NewValue;
            if (isEnabled)
            {
                frameworkElement.PreviewMouseWheel += ZoomBehavior.Zoom_OnMouseWheel;
                if (ZoomBehavior.TryGetScaleTransform(frameworkElement, out _))
                {
                    return;
                }

                if (frameworkElement.LayoutTransform is TransformGroup transformGroup)
                {
                    transformGroup.Children.Add(new ScaleTransform());
                }
                else
                {
                    frameworkElement.LayoutTransform = new ScaleTransform();
                }
            }
            else
            {
                frameworkElement.PreviewMouseWheel -= ZoomBehavior.Zoom_OnMouseWheel;
            }
        }

        private static void Zoom_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var zoomTargetElement = sender as FrameworkElement;

            Point mouseCanvasPosition = e.GetPosition(zoomTargetElement);
            double scaleFactor = e.Delta > 0
              ? ZoomBehavior.GetZoomFactor(zoomTargetElement)
              : -1 * ZoomBehavior.GetZoomFactor(zoomTargetElement);

            ZoomBehavior.ApplyZoomToAttachedElement(mouseCanvasPosition, scaleFactor, zoomTargetElement);

            ZoomBehavior.AdjustScrollViewer(mouseCanvasPosition, scaleFactor, zoomTargetElement);
        }

        private static void ApplyZoomToAttachedElement(Point mouseCanvasPosition, double scaleFactor, FrameworkElement zoomTargetElement)
        {
            if (!ZoomBehavior.TryGetScaleTransform(zoomTargetElement, out ScaleTransform scaleTransform))
            {
                throw new InvalidOperationException("No ScaleTransform found");
            }

            scaleTransform.CenterX = mouseCanvasPosition.X;
            scaleTransform.CenterY = mouseCanvasPosition.Y;

            scaleTransform.ScaleX = Math.Max(0.1, scaleTransform.ScaleX + scaleFactor);
            scaleTransform.ScaleY = Math.Max(0.1, scaleTransform.ScaleY + scaleFactor);
        }

        private static void AdjustScrollViewer(Point mouseCanvasPosition, double scaleFactor, FrameworkElement zoomTargetElement)
        {
            ScrollViewer scrollViewer = ZoomBehavior.GetScrollViewer(zoomTargetElement);
            if (scrollViewer == null)
            {
                return;
            }

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + mouseCanvasPosition.X * scaleFactor);
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + mouseCanvasPosition.Y * scaleFactor);
        }

        private static bool TryGetScaleTransform(FrameworkElement frameworkElement, out ScaleTransform scaleTransform)
        {
            // C# 8.0 Switch Expression
            scaleTransform = frameworkElement.LayoutTransform switch
            {
                TransformGroup transformGroup => transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault(),
                ScaleTransform transform => transform,
                _ => null
            };

            return scaleTransform != null;
        }
    }
}
