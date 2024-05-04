using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using Shapes;

namespace MyLine
{
    public class MyLine : IShape
    {
        public double strokeThickness { get; set; }
        public string Name => "Line";

        public Color brushColor { get; set; }

        public DoubleCollection strokeDash { get; set; }

        public Brush fill { get; set; }

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
                StrokeThickness = strokeThickness,
                Stroke = new SolidColorBrush(brushColor),
                StrokeDashArray = strokeDash
            };
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

        public void SetFill(Brush brush)
        {
            fill = brush;
        }
    }
}
