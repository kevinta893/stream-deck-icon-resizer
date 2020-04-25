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
        private Brush _brush;
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
