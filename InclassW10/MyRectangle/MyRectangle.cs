
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Shapes;
using System.Windows.Shapes;

namespace MyRectangle
{
    public class MyRectangle : IShape
    {
        public double strokeThickness { get => strokeThickness; set => strokeThickness = value; }
        public string Name => "Rectangle";

        public Color brushColor { get => brushColor; set => brushColor = value; }
        public DoubleCollection strokeDash { get => strokeDash; set => strokeDash = value; }

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

            var element = new Rectangle()
            {
                Width = (_rightBottom.X >= _topLeft.X) ? _rightBottom.X - _topLeft.X : _topLeft.X - _rightBottom.X,
                Height = (_rightBottom.Y >= _topLeft.Y) ? _rightBottom.Y - _topLeft.Y : _topLeft.Y - _rightBottom.Y,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Green)
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
    }

}
