using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StreamDeckIconResizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IEnumerable<string> SupportedImageFormats = new List<string>()
        {
            ".jpg", ".bmp", ".gif", ".svg", ".png",
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateScaleText(double percent)
        {
            var formattedPercent = (percent * 100).ToString("0.0");
            ResizePropertiesLabel.Text = $"Scale: {formattedPercent}";
        }

        #region UI Events

        private void ResizeImage_Drop(object sender, DragEventArgs e)
        {
            // Get file path of the dropped item
            var filePath = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            }

            // Load image
            var isSupportedFileExtension = SupportedImageFormats.Any(ext => ext == System.IO.Path.GetExtension(filePath));
            if (File.Exists(filePath) && isSupportedFileExtension)
            {
                var absoluteUri = new Uri(filePath);
                var percentScale = ImageScaleSlider.Value / ImageScaleSlider.Maximum;
                UpdateScaleText(percentScale);
                ResizeImage.LoadImageFile(absoluteUri, percentScale);
                CurrentFileLabel.Content = filePath;
            }

            e.Handled = true;
        }

        private void IconScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsLoaded) { return; }
            var percentScale = e.NewValue / ImageScaleSlider.Maximum;
            UpdateScaleText(percentScale);
            ResizeImage.ScaleImage(percentScale);
        }

        private void ShowTransparencyBackgroundCheckbox_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)e.Source;
            ResizeImage.TransparencyBackgroundVisible = checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false;
        }

        #endregion

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            ResizeImage.SaveDisplayedIconResized();
        }
    }
}
