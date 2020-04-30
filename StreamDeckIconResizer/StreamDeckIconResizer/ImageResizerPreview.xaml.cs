using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Svg;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = SixLabors.ImageSharp.Image;

namespace StreamDeckIconResizer
{
    /// <summary>
    /// Interaction logic for ImageResizerPreview.xaml
    /// </summary>
    public partial class ImageResizerPreview : UserControl
    {
        private ImageSource _imageSource;
        private Uri _iconPath;
        private const int CanvasWidth = 1000;
        private const int CanvasHeight = 1000;

        public ImageResizerPreview()
        {
            InitializeComponent();
        }

        public void LoadImageFile(Uri fileUri, double scale = 1.0)
        {
            BitmapImage originalImage;
            if (Path.GetExtension(fileUri.AbsolutePath) == ".svg")
            {
                SvgDocument svgDocument = SvgDocument.Open(fileUri.AbsolutePath);
                var svgImage = svgDocument.Draw(CanvasWidth, CanvasHeight);
                originalImage = Bitmap2BitmapImage(svgImage);
            }
            else
            {
                originalImage = new BitmapImage(fileUri);

            }

            _imageSource = originalImage;
            _iconPath = fileUri;
            ScaleImage(scale);
        }

        public bool TransparencyBackgroundVisible {
            get
            {
                return TransparencyImage.Visibility == Visibility.Visible;
            } 
            set 
            {
                TransparencyImage.Visibility = value ? Visibility.Visible : Visibility.Hidden;
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
            Scale = scale;
            var resizeWidth = (int) Math.Max(BackgroundRect.ActualWidth * scale, 1.0);
            var resizeHeight = (int) Math.Max(BackgroundRect.ActualHeight * scale, 1.0);
            var resizedImage = CreateResizedImage(_imageSource, resizeWidth, resizeHeight);
            ResizeImage.Source = resizedImage;
        }

        public void SaveDisplayedIconResized()
        {
            if (_iconPath == null)
            {
                return;
            }

            var imageExtension = Path.GetExtension(_iconPath.AbsolutePath);

            Image<Rgba32> icon;
            if (imageExtension == ".svg")
            {
                SvgDocument svgDocument = SvgDocument.Open(_iconPath.AbsolutePath);
                var iconBitmap = svgDocument.Draw(CanvasWidth, CanvasHeight);
                ImageConverter converter = new ImageConverter();
                var rawIconBytes = (byte[]) converter.ConvertTo(iconBitmap, typeof(byte[]));
                icon = Image.Load<Rgba32>(rawIconBytes);
            }
            else
            {
                icon = Image.Load<Rgba32>(_iconPath.AbsolutePath);
            }

            //Saves the image as png
            using (icon)
            {
                var resizeWidth = (int)Math.Max(CanvasWidth * Scale, 1.0);
                var resizeHeight = (int)Math.Max(CanvasHeight * Scale, 1.0);


                using (var resizedImage = IconResizer.ResizeImage(icon, resizeWidth, resizeHeight, CanvasWidth, CanvasHeight))
                {
                    var fullPath = _iconPath.AbsolutePath;
                    var originalFilePath = Path.GetFullPath(fullPath);
                    var resizedFileName = Path.GetFileNameWithoutExtension(fullPath);
                    var resizedFilePath = originalFilePath + "_resized.png";
                    IconResizer.SaveImagePng(resizedImage, resizedFilePath);
                }
            }
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

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
