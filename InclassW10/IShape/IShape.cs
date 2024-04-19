
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
        DoubleCollection strokeDash {  get; set; }
    }

}
