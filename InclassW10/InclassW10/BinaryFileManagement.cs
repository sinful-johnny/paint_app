using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InclassW10
{
    public class BinaryFileManagement
    {
        private string _path;
        private Canvas _canvas;
        public BinaryFileManagement(string path, Canvas canvas)
        {
            _path = path;
            _canvas = canvas;
        }

        public void SavePngFile()
        {
            Canvas canvasPNG = _canvas;
            canvasPNG.Background = System.Windows.Media.Brushes.Blue;
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvasPNG);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, 
                (int)bounds.Height, dpi, dpi, PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvasPNG);
                dc.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                MemoryStream ms = new MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                File.WriteAllBytes(_path, ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SaveFile()
        {
            FileStream fs = File.Open(_path, FileMode.Create);
            XamlWriter.Save(_canvas, fs);
            //BinaryWriter binaryWriter = new BinaryWriter(fs);
            //binaryWriter.Write(File.ReadAllBytes(fs));
            //binaryWriter.Close();
            //XamlBinaryWriter.Save(_canvas, fs);
            fs.Close();
        }

        public void LoadFile()
        {
            FileStream fs = File.Open(_path, FileMode.Open, FileAccess.Read);
            //var shapes = XamlReader.Load(fs);
            Canvas savedCanvas = XamlReader.Load(fs) as Canvas;
            //UIElementCollection paths = ((Canvas)savedCanvas.Children[0]).Children;

            List<UIElement> shapes = new List<UIElement>();
            foreach (var savedCanvasChild in savedCanvas.Children)
                shapes.Add((UIElement)savedCanvasChild);

            //Canvas copyCanvas = savedCanvas.GetCopy();
            //List<UIElement> shapes = new List<UIElement>();
            //foreach (UIElement shape in shapes)
            //{

            //}
            //savedCanvas.Children;

            fs.Close();

            foreach (var canvasChild in shapes)
            {
                //UIElement x = canvasChild as UIElement;
                var x = canvasChild.GetCopy();
                _canvas.Children.Add((UIElement)x);
            }
        }
    }
}
