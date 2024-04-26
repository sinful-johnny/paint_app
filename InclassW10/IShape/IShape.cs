

using System.Windows;
using System.Windows.Media;

namespace Shapes
{
    public interface IShape : ICloneable
    {
        void AddFirst(Point point);
        void AddSecond(Point point);
        UIElement Convert();

        string Name { get; }
        double strokeThickness { get; set; }
        Color brushColor { get; set; }
        public void setBrushColor(Color color);
        DoubleCollection strokeDash { get; set; }
        public void SetStrokeDash(DoubleCollection strokeDash);
        public void SetThickness(Double Thickness);
        Brush fill {  get; set; }
    }

}
