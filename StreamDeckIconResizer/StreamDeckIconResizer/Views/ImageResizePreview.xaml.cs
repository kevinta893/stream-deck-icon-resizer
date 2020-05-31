using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Image = Avalonia.Controls.Image;

namespace StreamDeckIconResizer.Views
{
    public class ImageResizePreview : UserControl
    {

        private readonly IEnumerable<string> SupportedImageFormats = new List<string>()
        {
            ".jpg", ".bmp", ".gif", ".svg", ".png",
        };

        public Rectangle BackgroundRect { get; }
        public Image TransparencyImage { get; }
        public Image BackgroundImage { get; }
        public Image ResizeImage { get; }
        public Rectangle ForegroundDragDropRect { get; }

        private string _workingImagePath;
        private Image<Rgba32> _workingImage;
        private const int UpscaledCanvasWidth = 1000;
        private const int UpscaledCanvasHeight = 1000;
        private const int SvgCanvasWidth = 1000;
        private const int SvgCanvasHeight = 1000;
        
        public ImageResizePreview()
        {
            this.InitializeComponent();
            BackgroundRect = this.Find<Rectangle>(nameof(BackgroundRect));
            TransparencyImage = this.Find<Image>(nameof(TransparencyImage));
            BackgroundImage = this.Find<Image>(nameof(BackgroundImage));
            ResizeImage = this.Find<Image>(nameof(ResizeImage));
            ForegroundDragDropRect = this.Find<Rectangle>(nameof(ForegroundDragDropRect));

            AddHandler(DragDrop.DragOverEvent, DragOver);
            AddHandler(DragDrop.DropEvent, DragDropped);

            var transparencyImagePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\transparency_background.png";
            TransparencyImage.Source = new Bitmap(transparencyImagePath);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// An event triggered when a valid image file is dropped
        /// onto the image preview user control
        /// File path is provided in the event as a string
        /// </summary>
        public event EventHandler<string> ImageFileDropped;

        /// <summary>
        /// Show or hide the transparency background
        /// </summary>
        public bool TransparencyBackgroundVisible
        {
            get 
            {
                return TransparencyImage.IsVisible;
            } 
            set
            {
                TransparencyImage.IsVisible = value;
            }
        }

        private bool _invertColor;
        public bool InvertColor {
            get
            {
                return _invertColor;
            }
            set
            {
                _invertColor = value;
                DisplayImage();
            }
        }


        private double _imageScale = 1.0;
        /// <summary>
        /// The scale of the image to display at
        /// </summary>
        public double ImageScale
        {
            get
            {
                return _imageScale;
            }
            set
            {
                _imageScale = value;
                DisplayImage();
            }
        }

        /// <summary>
        /// Sets the image from a file given the file path
        /// </summary>
        /// <param name="filePath"></param>
        public void SetImage(string filePath)
        {
            if (_workingImage != null)
            {
                _workingImage.Dispose();
            }

            _workingImagePath = filePath;

            var isSvg = System.IO.Path.GetExtension(filePath).ToLower() == ".svg";
            _workingImage = isSvg ? 
                ConvertSvgToImage(filePath) :
                SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);

            DisplayImage();
        }

        /// <summary>
        /// Displays the image on the resizer preview
        /// Requires a source image set and the scale
        /// </summary>
        private void DisplayImage()
        {
            if (_workingImage == null)
            {
                return;
            }

            // Resize before display
            using (var scaledImage = ScaleImage(_workingImage, _imageScale))
            using (var invertedImage = _invertColor ? InvertImage(scaledImage) : scaledImage)
            {
                var convertedBitmap = ConvertToAvaloniaBitmap(invertedImage);
                ResizeImage.Source = convertedBitmap;
            }
        }

        /// <summary>
        /// Exports the currently set image to a file
        /// Saves image as png and applies all effects to the image before saving
        /// </summary>
        public void ExportImage()
        {
            if (_workingImage == null || string.IsNullOrEmpty(_workingImagePath))
            {
                return;
            }

            const string ResizedImageSuffix = "_resized";
            const string PngExtension = ".png";

            using (var resizedImage = ScaleImage(_workingImage, _imageScale))
            using (var invertedImage = _invertColor ? InvertImage(resizedImage) : resizedImage)
            {
                var resizedImageName = System.IO.Path.GetFileNameWithoutExtension(_workingImagePath) + ResizedImageSuffix + PngExtension;
                var resizedImagePath = System.IO.Path.Join(System.IO.Path.GetDirectoryName(_workingImagePath), resizedImageName);

                IconResizer.SaveImagePng(invertedImage, resizedImagePath);
            }
        }

        #region Utility Functions

        /// <summary>
        /// Converts an SVG file into an image
        /// </summary>
        /// <param name="svgFilePath"></param>
        /// <returns></returns>
        private static Image<Rgba32> ConvertSvgToImage(string svgFilePath)
        {
            var canvasWidth = SvgCanvasWidth;
            var canvasHeight = SvgCanvasHeight;

            SvgDocument svgDocument = SvgDocument.Open(svgFilePath);
            var svgImage = svgDocument.Draw(canvasWidth, canvasHeight);

            using (var memStream = new MemoryStream())
            {
                svgImage.Save(memStream, ImageFormat.Png);
                memStream.Seek(0, SeekOrigin.Begin);
                return SixLabors.ImageSharp.Image.Load<Rgba32>(memStream);
            }
        }

        /// <summary>
        /// Scales the foreground image by X percent
        /// </summary>
        /// <param name="scale">Expects a value between [0.0,1.0]. Beyond 1.0 will scale the image larger.</param>
        private static Image<Rgba32> ScaleImage(Image<Rgba32> sourceImage, double scale)
        {
            var canvasWidth = UpscaledCanvasWidth;
            var canvasHeight = UpscaledCanvasHeight;
            var resizeWidth = (int) Math.Max(canvasWidth * scale, 1.0);
            var resizeHeight = (int) Math.Max(canvasHeight * scale, 1.0);

            return IconResizer.ResizeImage(sourceImage, resizeWidth, resizeHeight, canvasWidth, canvasHeight);
        }

        /// <summary>
        /// Inverts the colors of the image
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <returns></returns>
        private static Image<Rgba32> InvertImage(Image<Rgba32> sourceImage)
        {
            return IconResizer.InvertImage(sourceImage);
        }

        /// <summary>
        /// Converts a <see cref="SixLabors.ImageSharp.Image"/>
        /// to a <see cref="Avalonia.Media.Imaging.Bitmap"/>
        /// Intended for displaying images
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <returns></returns>
        private static Bitmap ConvertToAvaloniaBitmap(Image<Rgba32> sourceImage)
        {
            using (var resizedImageStream = new MemoryStream())
            {
                sourceImage.SaveAsPng(resizedImageStream);
                resizedImageStream.Seek(0, SeekOrigin.Begin);

                return new Bitmap(resizedImageStream);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event when something is dragged on top of the user control
        /// </summary>
        private void DragOver(object sender, DragEventArgs e)
        {
            // Only allow Copy or Link as Drop Operations.
            e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);

            // Only allow if the dragged data is a supported image format
            var isFiles = e.Data.Contains(DataFormats.FileNames);
            var fileExtension = isFiles ? System.IO.Path.GetExtension(e.Data.GetFileNames().FirstOrDefault()) : string.Empty;
            if (!isFiles || !SupportedImageFormats.Contains(fileExtension))
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Event when something is dropped on the user control
        /// </summary>
        private void DragDropped(object sender, DragEventArgs e)
        {
            // Get file path of the dropped item
            var filePath = "";
            if (e.Data.Contains(DataFormats.FileNames))
            {
                filePath = e.Data.GetFileNames().FirstOrDefault();
            }

            // Load image
            var isSupportedFileExtension = SupportedImageFormats.Any(ext => ext == System.IO.Path.GetExtension(filePath));
            if (File.Exists(filePath) && isSupportedFileExtension)
            {
                SetImage(filePath);
                ImageFileDropped.Invoke(sender, filePath);
            }
        }

        #endregion
    }
}
