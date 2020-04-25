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

        public ImageResizerPreview()
        {
            InitializeComponent();
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

        }

        public BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

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
