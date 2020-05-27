using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;

namespace StreamDeckIconResizer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string CurrentWorkingImagePath { get; set; } = "Drag and drop and image to start";
        public double ImageScalePercent { get; set; } = 100;
        public bool ShowTransparencyBackground { get; set; } = true;
    }
}
