using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.IO;

namespace StreamDeckIconResizer
{
    public class IconResizer
    {
        /// <summary>
        /// Resizes an image
        /// Original image not modified and the returned image
        /// is a copy resized.
        /// </summary>
        /// <param name="icon">Icon original to resize</param>
        /// <param name="resizeWidth">The width in pixels to resize the icon to</param>
        /// <param name="resizeHeight">The height in pixels to resize the icon to</param>
        /// <param name="canvasWidth">The canvas width in pixels to add the resized icon to</param>
        /// <param name="canvasHeight">The canvas height in pixels to add the resized icon to</param>
        /// <returns></returns>
        public static Image<Rgba32> ResizeImage(Image<Rgba32> icon, int resizeWidth, int resizeHeight, int canvasWidth, int canvasHeight)
        {
            using (var resizedIcon = icon.Clone(i => i.Resize(resizeWidth, resizeHeight, KnownResamplers.CatmullRom)))
            {
                var canvasCenter = new Point(canvasWidth / 2, canvasHeight / 2);
                var centerIconOffset = -1 * new Point(resizeWidth / 2, resizeHeight / 2);
                
                var resizedImage = new Image<Rgba32>(canvasWidth, canvasHeight);
                resizedImage.Mutate(i => i.DrawImage(resizedIcon, canvasCenter.TranslatePoint(centerIconOffset), 1.0f));

                return resizedImage;
            }

        }

        /// <summary>
        /// Saves the image to a given file in PNG format
        /// </summary>
        /// <param name="image">Image to save</param>
        /// <param name="filePath">File to save to, please include .png in path</param>
        public static void SaveImagePng(Image<Rgba32> image, string filePath)
        {
            var filePathAsPng = filePath;
            var fileStream = new FileStream(filePathAsPng, FileMode.Create);
            try
            {
                var encodingOptions = new PngEncoder();
                encodingOptions.ColorType = PngColorType.RgbWithAlpha;
                image.SaveAsPng(fileStream, encodingOptions);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                fileStream.Close();
            }
        }
    }

    public static class PointExtensions
    {
        /// <summary>
        /// Translates point p1 by some offset
        /// </summary>
        /// <param name="p1">Original point</param>
        /// <param name="offset">Translation offset</param>
        /// <returns>A new point with the translation offset applied</returns>
        public static Point TranslatePoint(this Point p1, Point offset)
        {
            return new Point(p1.X + offset.X, p1.Y + offset.Y);
        }
    }
}
