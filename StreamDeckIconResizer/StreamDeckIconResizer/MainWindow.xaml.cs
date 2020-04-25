using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly IEnumerable<string> SupportedImageFormats = new List<string>()
        {
            ".jpg", ".bmp", ".gif", ".svg", ".png",
        };

        private WriteableBitmap _workingImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void LoadImageFile(Uri fileUri)
        {
            var originalImage = new BitmapImage(fileUri);
            _workingImage = new WriteableBitmap(originalImage);
            ResizeImage.Source = _workingImage;
            CurrentFileLabel.Content = fileUri.AbsolutePath.ToString();
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
                LoadImageFile(absoluteUri);
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

        #endregion
    }
}
