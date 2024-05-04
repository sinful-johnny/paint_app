using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;
using Shapes;

namespace InclassW10
{
    static class ExtMethods
    {
        public static T GetCopy<T>(this T element) where T : UIElement
        {
            using (var ms = new MemoryStream())
            {
                XamlWriter.Save(element, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)XamlReader.Load(ms);
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal class Mode
        {
            static public int Drawing { get => 1; }
            static public int Selecting { get => 0; }
            static public int EmbedImage { get => 3; }
            static public int Rotating { get => 2; }
        }
        // Adorners must subclass the abstract base class Adorner.
        public MainWindow()
        {
            
            InitializeComponent();
        }
        bool _isDrawing = false;
        Point _start;
        Point _end;
        Image? current_image;
        Point lastMousePosition;
        IShape _painter;
        bool resizeMode;
        List<IShape> _prototypes = new List<IShape>();
        int _mode = Mode.Drawing;

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

            //foreach (var item in _prototypes)
            //{
            //    var control = new Button()
            //    {
            //        Width = 80,
            //        Height = 35,
            //        Content = item.Name,
            //        Tag = item
            //    };
            //    control.Click += Control_Click;
            //    actions.Children.Add(control);
            //}
            GeometrySelect.ItemsSource = _prototypes;
            _painter = _prototypes[0];
            _painter.SetThickness(1);
            _painter.setBrushColor(Colors.Black);
            _painter.SetStrokeDash(null);
        }

        private void Control_Click(object sender, RoutedEventArgs e)
        {
            IShape item = (IShape)(sender as Button)!.Tag;
            _painter = item;
            _mode = 1;
        }

        UIElement? _selectedElement;

        private void SelectColor(object  sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle rectangle && _painter != null)
            {
                if (rectangle.Fill is SolidColorBrush brush)
                {
                    _painter.setBrushColor(brush.Color);
                }
                else
                {
                    _painter.setBrushColor(Colors.Transparent);
                }
            }
        }

        private void SelectThickness(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is Grid grid && grid.Children[0] is Line line && _painter != null)
            {
                Double Thickness = line.StrokeThickness;
                _painter?.SetThickness(Thickness);
            }
        }

        private void SelectDashStyle(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content is Grid grid && grid.Children[0] is Line line)
            {
                DoubleCollection dashStyles; 

                if (line.StrokeDashArray != null && line.StrokeDashArray.Count > 0)
                {
                    dashStyles = line.StrokeDashArray;
                } 

                else
                {
                    dashStyles = [];
                }

                _painter?.SetStrokeDash(dashStyles);
            }
        }

        Point _previewStart = new Point(0,0);
        Point _previewEnd = new Point(0, 0);
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(DiagramDesignerCanvasContainer);
            if (_mode == Mode.Drawing)
            {
                _isDrawing = true;

                _start = mousePosition;
                _previewStart = e.GetPosition(previewCanvas);
                _painter.AddFirst(_previewStart);

                RemoveAllAdorners();

            }
            else if(_mode == Mode.Selecting)
            {
                SelectAndAdorn(mousePosition);
            }
            e.Handled = true;
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

        private void ImageImport(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    if (_painter != null)
                    {
                        string imagePath = openFileDialog.FileName;
                        ImageBrush newImage = new ImageBrush();
                        newImage.ImageSource = new BitmapImage(new Uri(imagePath));
                        newImage.Stretch = Stretch.Fill;

                        _painter.SetFill(newImage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void reWrap()
        {
            if(_selectedElement != null)
            {
                var myAdornerLayer = AdornerLayer.GetAdornerLayer(myCanvas);
                RemoveAllAdorners();
                myAdornerLayer.Add(new SelectionAdorner(_selectedElement));
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mode == Mode.Drawing)
            {
                _isDrawing = false;

                _end = e.GetPosition(DiagramDesignerCanvasContainer);

                _painter.AddFirst(_start);
                _painter.AddSecond(_end);
                myCanvas.Children.Add(_painter.Convert());

                //_observers.Clear();
                previewCanvas.Children.Clear();
                e.Handled = true;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_isDrawing)
                {
                    _previewEnd = e.GetPosition(previewCanvas);

                    previewCanvas.Children.Clear();

                    _painter.AddSecond(_previewEnd);

                    //_observers.Add(_painter.Convert());
                    previewCanvas.Children.Add(_painter.Convert());

                    e.Handled = true;
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

        private double _scaleValue = 1.0;
        private double zoomScaleFactor = 1.1;
        private Point? mousePos;

        private void ZoomAtMousePos(MouseWheelEventArgs e, UIElement element)
        {
            var pos = e.GetPosition(element);
            var scale = e.Delta > 0 ? zoomScaleFactor : 1 / zoomScaleFactor;
            var transform = (MatrixTransform)element.RenderTransform;
            var matrix = transform.Matrix;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            transform.Matrix = matrix;

            e.Handled = true;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "XAML files (*.xaml)|*.xaml";
            saveFileDialog.Filter = "Binary File (*.bin)|*.bin";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                var BFM = new BinaryFileManagement(filename, myCanvas);
                BFM.SaveFile();
            }
        }

        private void saveasButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.png)|*.png";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                string filename = saveFileDialog.FileName;
                var BFM = new BinaryFileManagement(filename, myCanvas);
                BFM.SavePngFile();
            }
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();

            if (screen.ShowDialog() == true)
            {
                string filename = screen.FileName;
                var BFM = new BinaryFileManagement(filename, myCanvas);
                BFM.LoadFile();
            }
        }

        private void SelectShape(object sender, SelectionChangedEventArgs e)
        {
            if (GeometrySelect.SelectedItem != null)
            {
                _painter = (IShape)GeometrySelect.SelectedItem;
            }
        }

    }
}