
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Shapes;

namespace MyEllipse
{
    public class MyEllipse : IShape
    {
        public string Name => "Ellipse";

        public double strokeThickness { get => strokeThickness; set => strokeThickness = value; }
        public Color brushColor { get; set; }
        public DoubleCollection strokeDash { get; set; }
        public Brush fill { get => fill; set => fill = value; }

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
            var element = new Ellipse()
            {
                Width = (_rightBottom.X >= _topLeft.X) ? _rightBottom.X - _topLeft.X : _topLeft.X - _rightBottom.X,
                Height = (_rightBottom.Y >= _topLeft.Y) ? _rightBottom.Y - _topLeft.Y : _topLeft.Y - _rightBottom.Y,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(Colors.Transparent),
                Stroke = new SolidColorBrush(Colors.Blue)
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

        public void setBrushColor(Color color)
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
    }

}
