﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.IO;

namespace StreamDeckIconResizer.Views
{
    public class MainWindow : Window
    {
        private Button ResizeButton { get; }
        private Slider ImageScaleSlider { get; }
        private CheckBox ShowTransparencyCheckbox { get; }
        private CheckBox InvertColorsCheckbox { get; }
        private ImageResizePreview ImageResizePreview { get; }
        private TextBlock WorkingImagePath { get; }

        private double _lastImageScaleUpdate = double.MinValue;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            ResizeButton = this.Find<Button>(nameof(ResizeButton));
            ResizeButton.Click += ResizeButton_Click;

            ImageScaleSlider = this.Find<Slider>(nameof(ImageScaleSlider));
            ImageScaleSlider.PropertyChanged += ImageScaleSlider_PropertyChanged;

            ShowTransparencyCheckbox = this.Find<CheckBox>(nameof(ShowTransparencyCheckbox));
            ShowTransparencyCheckbox.PropertyChanged += ShowTransparencyCheckbox_PropertyChanged;

            InvertColorsCheckbox = this.Find<CheckBox>(nameof(InvertColorsCheckbox));
            InvertColorsCheckbox.PropertyChanged += InvertColorsCheckbox_PropertyChanged;

            ImageResizePreview = this.Find<ImageResizePreview>(nameof(ImageResizePreview));
            ImageResizePreview.ImageFileDropped += ImageResizePreview_ImageFileDropped;

            WorkingImagePath = this.Find<TextBlock>(nameof(WorkingImagePath));

            ToolTip.SetShowDelay(ImageScaleSlider, 200);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #region Event handlers

        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            ImageResizePreview.ExportImage();
        }

        private void ShowTransparencyCheckbox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            ImageResizePreview.TransparencyBackgroundVisible = checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false;
        }

        private void InvertColorsCheckbox_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            ImageResizePreview.InvertColor = checkbox.IsChecked.HasValue ? checkbox.IsChecked.Value : false;
        }

        private void ImageScaleSlider_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            const double UpdateScaleDelta = 0.005;    //The amount the value must change by before updating the image scale

            Slider slider = sender as Slider;
            var scale = slider.Value / slider.Maximum;

            if (Math.Abs(_lastImageScaleUpdate - scale) > UpdateScaleDelta)
            {
                _lastImageScaleUpdate = scale;
                ImageResizePreview.ImageScale = scale;
            }

            var scalePercent = (scale).ToString("P1");
            ToolTip.SetTip(slider, scalePercent);
        }

        private void ImageResizePreview_ImageFileDropped(object sender, string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            WorkingImagePath.Text = fileName;
        }

        #endregion
    }
}
