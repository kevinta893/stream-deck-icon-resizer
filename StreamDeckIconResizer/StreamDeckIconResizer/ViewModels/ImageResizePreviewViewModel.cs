using StreamDeckIconResizer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeckIconResizer.ViewModels
{
    public class ImageResizePreviewViewModel : ViewModelBase
    {
        public string BaseColor { get; set; }
        public string TransparencyImage { get; set; }
        public bool IsTransparencyVisible { get; set; }
        public string BackgroundImage { get; set; }
        public string Icon { get; set; }
        public double IconScale { get; set; }
    }
}
