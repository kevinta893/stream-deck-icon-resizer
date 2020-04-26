using System;
using System.Collections.Generic;
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

namespace StreamDeckIconResizer
{
    /// <summary>
    /// Interaction logic for ImageResizerPreview.xaml
    /// </summary>
    public partial class ImageResizerPreview : UserControl
    {
        private double _scale;
        private ImageSource _imageSource;
        private readonly double _originalWidth;
        private readonly double _originalHeight;

        public ImageResizerPreview()
        {
            InitializeComponent();
            _originalWidth = BackgroundRect.ActualWidth;
            _originalHeight = BackgroundRect.ActualHeight;
        }

        public ImageSource Source { 
            get 
            {
                return _imageSource;
            } 
            set 
            {
                ResizeImage.Source = value;
                _imageSource = value;
                ScaleImage(_scale);
            }
        }

        /// <summary>
        /// Current scale of the image. 1.0 is the regular
        /// size of the image
        /// </summary>
        public double Scale { get; private set; }

        /// <summary>
        /// Scales the foreground image by X percent
        /// </summary>
        /// <param name="percent">Expects a value between [0.0,1.0]. Beyond 1.0 will scale the image larger.</param>
        public void ScaleImage (double scale)
        {
            _scale = scale;
            var resizeWidth = (int) Math.Max(BackgroundRect.ActualWidth * scale, 1.0);
            var resizeHeight = (int) Math.Max(BackgroundRect.ActualHeight * scale, 1.0);
            var resizedImage = CreateResizedImage(_imageSource, resizeWidth, resizeHeight);
            ResizeImage.Source = resizedImage;
        }


        /// <summary>
        /// Resizes an image to fit inside a given width and height
        /// </summary>
        /// <param name="source">Source image</param>
        /// <param name="width">Destination width</param>
        /// <param name="height">Destination height</param>
        /// <returns></returns>
        public static BitmapFrame CreateResizedImage(ImageSource source, int width, int height)
        {
            var rect = new Rect(0, 0, width, height);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }
    }
}
