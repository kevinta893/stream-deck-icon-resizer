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
    /// Interaction logic for ScaleGrid.xaml
    /// </summary>
    public partial class ScaleGridOverlay : UserControl
    {
        public ScaleGridOverlay()
        {
            InitializeComponent();
        }

        public void AddScaleGrids(double[] percentGrid)
        {
            ClearGrid();
            foreach (var percent in percentGrid)
            {
                DrawGrid(percent);
            }
        }

        /// <summary>a
        /// 
        /// </summary>
        /// <param name="scale"></param>
        private void DrawGrid(double scale)
        {

        }

        private void ClearGrid()
        {
            var image = GridOverlay.Source;
            GridOverlay.Source = null;
        }
    }
}
