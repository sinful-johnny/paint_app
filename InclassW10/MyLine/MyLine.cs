using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Shapes;

namespace MyLine
{
    public class MyLine : IShape
    {
        public double strokeThickness { get => strokeThickness; set => strokeThickness = value; }
        public string Name => "Line";

        public Color brushColor { get => brushColor; set => brushColor=value; }

        public DoubleCollection strokeDash { get => strokeDash; set => strokeDash = value; }

        private Point _start;
        private Point _end;
        public void AddFirst(Point point)
        {
            _start = point;
        }

        public void AddSecond(Point point)
        {
            _end = point;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public UIElement Convert()
        {
            return new Line()
            {
                X1 = _start.X,
                Y1 = _start.Y,
                Y2 = _end.Y,
                X2 = _end.X,
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red)
            };
        }
    }

}
