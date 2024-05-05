

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

        Transform transform { get; set; }
        Brush brushColor { get; set; }
        public void setBrushColor(Brush color);
        DoubleCollection strokeDash { get; set; }
        public void SetStrokeDash(DoubleCollection strokeDash);
        public void SetThickness(Double Thickness);
        public void SetFill(Brush brush);
        public void SetTransform(Transform transform);
        Brush fill {  get; set; }
    }

}
