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
            ".jpg",
            ".bmp",
            ".gif",
            ".svg",
            ".png"
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResizeImage_Drop(object sender, DragEventArgs e)
        {
            // Get file path of the dropped item
            var filePath = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                filePath = ((string[]) e.Data.GetData(DataFormats.FileDrop))[0];
            }

            // Load image
            var isSupportedFileExtension = SupportedImageFormats.Any(ext => ext == System.IO.Path.GetExtension(filePath));
            if (File.Exists(filePath) && isSupportedFileExtension)
            {
                LoadImageFile(filePath);
            }

            e.Handled = true;
        }

        public void LoadImageFile(string imageFilePath)
        {
            ResizeImage.Source = new BitmapImage(new Uri(imageFilePath));
        }

        private void ResizeImage_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
