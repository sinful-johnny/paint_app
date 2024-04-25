using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using MyEllipse;
using Shapes;

namespace InclassW10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal class Mode
        {
            static public int Drawing { get => 1; }
            static public int Selecting { get => 0; }
            static public int Rotating { get => 2; }
        }
        // Adorners must subclass the abstract base class Adorner.
        public class SimpleCircleAdorner : Adorner
        {
            // Be sure to call the base class constructor.
            public SimpleCircleAdorner(UIElement adornedElement)
              : base(adornedElement)
            {
            }

            // A common way to implement an adorner's rendering behavior is to override the OnRender
            // method, which is called by the layout system as part of a rendering pass.
            protected override void OnRender(DrawingContext drawingContext)
            {
                Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

                // Some arbitrary drawing implements.
                SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
                renderBrush.Opacity = 0.2;
                Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
                double renderRadius = 5.0;

                // Draw a circle at each corner.
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
                drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
            }
        }
        public MainWindow()
        {
            
            InitializeComponent();
        }
        bool _isDrawing = false;
        Point _start;
        Point _end;
        //List<IShape> _painters = new List<IShape>();
        IShape _painter;
        List<IShape> _prototypes = new List<IShape>();
        int _mode = 0;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = new DirectoryInfo(folder).GetFiles("*.dll");

            foreach ( var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach(var type in  types)
                {
                    if((type.IsClass) && typeof(IShape).IsAssignableFrom(type)) { 
                        _prototypes.Add((IShape)Activator.CreateInstance(type)!);
                    }
                }
            }

            foreach (var item in _prototypes)
            {
                var control = new Button()
                {
                    Width = 80,
                    Height = 35,
                    Content = item.Name,
                    Tag = item
                };
                control.Click += Control_Click;
                actions.Children.Add(control);
            }
            _painter = _prototypes[0];
        }

        private void Control_Click(object sender, RoutedEventArgs e)
        {
            IShape item = (IShape)(sender as Button)!.Tag;
            _painter = item;
            _mode = 1;
        }

        UIElement _selectedElement;

        private void SelectColor(object  sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rectangle && _painter != null)
            {
                var brush = rectangle.Fill as SolidColorBrush;
                if (brush != null)
                {
                    _painter.setBrushColor(brush.Color);
                }
                else
                {
                    _painter.setBrushColor(Colors.Transparent);
                }
                string colorInfo = $"Selected Color: #{brush.Color.A:X2}{brush.Color.R:X2}{brush.Color.G:X2}{brush.Color.B:X2}";

                MessageBox.Show(colorInfo, "Selected Color", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(myCanvas);
            if (_mode == Mode.Drawing)
            {
                _isDrawing = true;

                _start = mousePosition;
                _painter.AddFirst(_start);

                RemoveAllAdorners();

            }
            else if(_mode == Mode.Selecting)
            {
                SelectAndAdorn(mousePosition);
            }
            //else if(_mode != Mode.Rotating && rotateSlider != null)
            //{
            //    myCanvas.Children.Remove(rotateSlider);
            //}
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(myCanvas);
            SelectAndAdorn(mousePosition);
        }

        private void SelectAndAdorn(Point mousePosition)
        {
            var result = myCanvas.InputHitTest(mousePosition) as UIElement;
            if (result != null)
            {
                _selectedElement = result;
                var myAdornerLayer = AdornerLayer.GetAdornerLayer(myCanvas);
                RemoveAllAdorners();
                myAdornerLayer.Add(new SelectionAdorner(result));
            }
            else
            {
                RemoveAllAdorners();
            }
        }

        private void RemoveAllAdorners()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(myCanvas);
            foreach (UIElement element in myCanvas.Children)
            {
                Adorner[] toRemoveArray = myAdornerLayer.GetAdorners(element);
                if (toRemoveArray != null)
                {
                    for (int x = 0; x < toRemoveArray.Length; x++)
                    {
                        myAdornerLayer.Remove(toRemoveArray[x]);
                    }
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mode == Mode.Drawing)
            {
                _isDrawing = false;

                _end = e.GetPosition(myCanvas);
                _painter.AddSecond(_end);
                myCanvas.Children.Add(_painter.Convert());

                previewCanvas.Children.Clear();
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isDrawing)
                {
                    _end = e.GetPosition(myCanvas);

                    previewCanvas.Children.Clear();

                    _painter.AddFirst(_start);
                    _painter.AddSecond(_end);

                    previewCanvas.Children.Add(_painter.Convert());
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void NoneButton_Click(object sender, RoutedEventArgs e)
        {
            _mode = Mode.Selecting;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedElement != null)
            {
                myCanvas.Children.Remove(_selectedElement);
                _selectedElement = null;
            }
            else
            {
                MessageBox.Show("Select something to delete!","Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DashStyleSelected(object sender, RoutedEventArgs e)
        {

        }

        //UIElement rotateSlider;
        //private void RotateMenuItem_Click(object sender, RoutedEventArgs e)
        //{
        //    Slider slider = new Slider()
        //    {
        //        Value = 0,
        //        Maximum = 180,
        //        Minimum = -180,
        //        Height = 30,
        //        Width = 150,
        //        IsEnabled = true,
        //    };
        //    var currentPoint = _selectedElement.TranslatePoint(new Point(0.0, 0.0),null);
        //    Canvas.SetLeft(slider, currentPoint.X);
        //    Canvas.SetTop(slider, currentPoint.Y - 20);
        //    slider.ValueChanged += Slider_ValueChanged;

        //    rotateSlider = slider;
        //    previewCanvas.Children.Add(rotateSlider);
        //    _mode = Mode.Rotating;
        //}

        //private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    var element = (FrameworkElement)_selectedElement;
        //    RotateTransform myRotateTransform = new RotateTransform();
        //    myRotateTransform.CenterX = element.Width / 2;
        //    myRotateTransform.CenterY = element.Height / 2;
        //    element.RenderTransform = myRotateTransform;
        //}
    }
}