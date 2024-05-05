using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace InclassW10
{
    internal class SelectionAdorner : Adorner
    {
        int angle;
        VisualCollection AdornerVisual;
        Thumb thumb1, thumb2, thumbCenter, lineThumb1, lineThumb2;
        Rectangle Rec;
        TextBlock angleText;
        TextBox angleInput;
        public SelectionAdorner(UIElement adornedElement) : base(adornedElement)
        {
            AdornerVisual = new VisualCollection(this);
            thumb1 = new Thumb() { Background= Brushes.OrangeRed, Height=10,Width=10};
            thumb2 = new Thumb() { Background = Brushes.OrangeRed, Height = 10, Width = 10 };
            thumbCenter = new Thumb() { Background = Brushes.OrangeRed, Height = 10, Width = 10 };
            lineThumb1 = new Thumb() { Background = Brushes.OrangeRed, Height = 10, Width = 10 };
            lineThumb2 = new Thumb() { Background = Brushes.OrangeRed, Height = 10, Width = 10 };
            //thumbBottom = new Thumb() { Background = Brushes.OrangeRed, Height = 10, Width = 10 };
            Rec = new Rectangle() { Stroke = Brushes.OrangeRed, StrokeThickness = 2, StrokeDashArray = { 3.0, 2.0 } };
            angleText = new TextBlock() { Width=50,Height=30 };
            angleInput = new TextBox() { Width=50,Height=30, HorizontalContentAlignment= HorizontalAlignment.Center, VerticalContentAlignment= VerticalAlignment.Center };
            
            thumb1.DragDelta += Thumb1_DragDelta;
            thumb2.DragDelta += Thumb2_DragDelta;
            //thumbBottom.DragDelta += ThumbBottom_DragDelta;
            thumbCenter.DragDelta += ThumbCenter_DragDelta;
            angleInput.KeyDown += AngleInput_EnterPressed;
            lineThumb1.DragDelta += LineThumb1_DragDelta;
            lineThumb2.DragDelta += LineThumb2_DragDelta;

            AdornerVisual.Add(angleText);
            AdornerVisual.Add(thumbCenter);
            AdornerVisual.Add(Rec);
            AdornerVisual.Add(thumb1);
            AdornerVisual.Add(thumb2);
            AdornerVisual.Add(angleInput);
            AdornerVisual.Add(lineThumb1);
            AdornerVisual.Add(lineThumb2);
        }

        private void LineThumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = AdornedElement as Line;

            if(element != null)
            {
                element.Y2 = element.Y2 + e.VerticalChange < 0 ? 0 : element.Y2 + e.VerticalChange;
                element.X2 = element.X2 + e.HorizontalChange < 0 ? 0 : element.X2 + e.HorizontalChange;
            }
        }

        private void LineThumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = AdornedElement as Line;

            if (element != null)
            {
                element.Y1 = element.Y1 + e.VerticalChange < 0 ? 0 : element.Y1 + e.VerticalChange;
                element.X1 = element.X1 + e.HorizontalChange < 0 ? 0 : element.X1 + e.HorizontalChange;
            }
        }

        private void AngleInput_EnterPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    angle = int.Parse(angleInput.Text);
                    var element = AdornedElement as FrameworkElement;

                    RotateTransform myRotateTransform = new RotateTransform();
                    myRotateTransform.CenterX = element.Width / 2;
                    myRotateTransform.CenterY = element.Height / 2;
                    myRotateTransform.Angle = angle;
                    element.RenderTransform = myRotateTransform;
                }
                catch { }
            }
        }

        private void ThumbBottom_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = AdornedElement as FrameworkElement;

            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.CenterX = element.Width / 2;
            myRotateTransform.CenterY = element.Height / 2;

            double radius = element.Width/2 + 20;
            //double MovementX = Math.Abs(e.HorizontalChange) >= radius ? e.HorizontalChange >0 ? radius : -radius : e.HorizontalChange;
            //double MovementY = e.VerticalChange > radius ? radius : e.VerticalChange;

            double MovementX = e.HorizontalChange;
            double MovementY = e.VerticalChange;

            if(Math.Abs(e.HorizontalChange) <= radius)
            {
                double radians = Math.Asin((MovementX / radius));
                double angle = radians * (180 / Math.PI);

                myRotateTransform.Angle = angle;
                element.RenderTransform = myRotateTransform;
            }
            

            //if (MovementX != 0 && MovementY != 0)
            //{
            //    //double longEdge = Math.Sqrt(Math.Pow(MovementX,2) + Math.Pow(MovementY,2));
            //    if (MovementX > 0 && MovementY > 0 && radius != 0)
            //    {
            //        double radians = Math.Acos((MovementY / radius));
            //        double angle = radians * (180 / Math.PI);

            //        myRotateTransform.Angle = angle;

            //        element.RenderTransform = myRotateTransform;
            //        angleText.Text = angle.ToString();
            //    }
            //else if (MovementX > 0 && MovementY < 0 && longEdge != 0)
            //{
            //    double radians = Math.Asin((MovementX / longEdge));
            //    double angle = radians * (180 / Math.PI);

            //    myRotateTransform.Angle = 90 - angle;
            //    myRotateTransform.CenterX = element.Width / 2;
            //    myRotateTransform.CenterY = element.Height / 2;
            //    element.RenderTransform = myRotateTransform;
            //    angleText.Text = angle.ToString();
            //}

            //}
            angleText.Text = e.HorizontalChange.ToString();

        }

        private void ThumbCenter_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //var element = AdornedElement as FrameworkElement;

            //var currentPoint = element.TranslatePoint(new Point(0.0, 0.0), (UIElement)element.Parent);

            //element.SetValue(Canvas.LeftProperty, currentPoint.X + e.HorizontalChange);
            //element.SetValue(Canvas.TopProperty, currentPoint.Y + e.VerticalChange);
            var thumb = AdornedElement as UIElement;
            var transform = thumb.RenderTransform as RotateTransform;
            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (transform != null)
            {
                dragDelta = transform.Transform(dragDelta);
            }

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + dragDelta.X);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + dragDelta.Y);
        }

        private void Thumb2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = AdornedElement as FrameworkElement;

            element.Height = element.Height + e.VerticalChange < 0 ? 0 : element.Height + e.VerticalChange;
            element.Width = element.Width + e.HorizontalChange < 0 ? 0 : element.Width + e.HorizontalChange;
        }

        private void Thumb1_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var element = AdornedElement as FrameworkElement;

            //var currentPoint = element.TranslatePoint(new Point(0.0, 0.0), (UIElement)element.Parent);

            //element.SetValue(Canvas.LeftProperty, currentPoint.X + e.HorizontalChange);
            //element.SetValue(Canvas.TopProperty, currentPoint.Y + e.VerticalChange);

            var thumb = AdornedElement as UIElement;
            var transform = thumb.RenderTransform as RotateTransform;
            Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (transform != null)
            {
                dragDelta = transform.Transform(dragDelta);
            }

            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + dragDelta.X);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + dragDelta.Y);

            //element.Height = element.Height - e.VerticalChange < 0 ? 0 : element.Height - e.VerticalChange;
            //element.Width = element.Width - e.HorizontalChange < 0 ? 0 : element.Width - e.HorizontalChange;
        }

        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisual[index];
        }

        protected override int VisualChildrenCount => AdornerVisual.Count;

        
        protected override Size ArrangeOverride(Size finalSize)
        {
            if(typeof(Line).IsAssignableTo(AdornedElement.GetType()))
            {
                var  selectedLine = AdornedElement as Line;
                if(selectedLine != null)
                {
                    double left = Math.Min(selectedLine.X1, selectedLine.X2);
                    double top = Math.Min(selectedLine.Y1, selectedLine.Y2);

                    var startRect = new Rect(selectedLine.X1 - (lineThumb1.Width / 2), selectedLine.Y1 - (lineThumb1.Width / 2), lineThumb1.Width, lineThumb1.Height);
                    lineThumb1.Arrange(startRect);

                    var endRect = new Rect(selectedLine.X2 - (lineThumb2.Width / 2), selectedLine.Y2 - (lineThumb2.Height / 2), lineThumb2.Width, lineThumb2.Height);
                    lineThumb2.Arrange(endRect);
                    return base.ArrangeOverride(finalSize);
                }
                else
                {
                    return base.ArrangeOverride(finalSize);
                }
            }
            else
            {
                Rec.Arrange(new Rect(-2.5, -2.5, AdornedElement.DesiredSize.Width + 5, AdornedElement.DesiredSize.Height + 5));
                thumb1.Arrange(new Rect(-5, -5, 10, 10));
                thumbCenter.Arrange(new Rect(AdornedElement.DesiredSize.Width / 2 - 5, AdornedElement.DesiredSize.Height / 2 - 5, 10, 10));
                thumb2.Arrange(new Rect(AdornedElement.DesiredSize.Width - 5, AdornedElement.DesiredSize.Height - 5, 10, 10));
                //thumbBottom.Arrange(new Rect(AdornedElement.DesiredSize.Width / 2 - 5, -20, 10, 10));
                angleText.Arrange(new Rect(AdornedElement.DesiredSize.Width / 2 - 5, AdornedElement.DesiredSize.Height + 30, 50, 30));
                angleInput.Arrange(new Rect(AdornedElement.DesiredSize.Width / 2 - 25, -35, 50, 30));
                return base.ArrangeOverride(finalSize);
            }
            
        }
    }
}
