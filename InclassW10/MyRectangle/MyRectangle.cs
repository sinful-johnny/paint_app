
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Shapes;
using System.Windows.Shapes;

namespace MyRectangle
{
    public class MyRectangle : IShape
    {
        public double strokeThickness { get; set; }
        public string Name => "Rectangle";

        public Brush brushColor { get; set; }
        public DoubleCollection strokeDash { get; set; }

        public Brush fill { get; set; }

        private Point _topLeft;
        private Point _rightBottom;
        public void AddFirst(Point point)
        {
            _topLeft = point;
        }

        public void AddSecond(Point point)
        {
            _rightBottom = point;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public UIElement Convert()
        {

            var element = new Rectangle()
            {
                Width = (_rightBottom.X >= _topLeft.X) ? _rightBottom.X - _topLeft.X : _topLeft.X - _rightBottom.X,
                Height = (_rightBottom.Y >= _topLeft.Y) ? _rightBottom.Y - _topLeft.Y : _topLeft.Y - _rightBottom.Y,
                StrokeThickness = strokeThickness,
                Fill = fill,
                Stroke = brushColor,
                StrokeDashArray = strokeDash
            };
            if (_rightBottom.Y >= _topLeft.Y)
            {
                Canvas.SetTop(element, _topLeft.Y);
            }
            else
            {
                Canvas.SetTop(element, _rightBottom.Y);
            }
            if (_rightBottom.X >= _topLeft.X)
            {
                Canvas.SetLeft(element, _topLeft.X);
            }
            else
            {
                Canvas.SetLeft(element, _rightBottom.X);
            }
            return element;

        }

        public void setBrushColor(Brush color)
        {
            brushColor = color;
        }
        public void SetStrokeDash(DoubleCollection strokeDash)
        {
            this.strokeDash = strokeDash;
        }
        public void SetThickness(double Thickness)
        {
            strokeThickness = Thickness;
        }
        public void SetFill(Brush brush)
        {
            fill = brush;
        }
    }

}
