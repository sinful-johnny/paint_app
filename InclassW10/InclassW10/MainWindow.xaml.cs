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
using Shapes;

namespace InclassW10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(myCanvas);
            if (_mode == 1)
            {
                _isDrawing = true;
                _start = mousePosition;
            }
            else if(_mode == 0) {
                var result = myCanvas.InputHitTest(mousePosition) as UIElement;
                if (result != null)
                {
                    //var selectedPainter = _painters.FirstOrDefault(x => x.Convert() == result);
                    myCanvas.Children.Remove(result);
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mode == 1)
            {
                _isDrawing = false;
                myCanvas.Children.Add(_painter.Convert());
                //_painters.Add((IShape)_painter.Clone());
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

                    //foreach (var item in _painters)
                    //{
                    //    myCanvas.Children.Add(item.Convert());
                    //}
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
            _mode = 0;
        }
    }
}