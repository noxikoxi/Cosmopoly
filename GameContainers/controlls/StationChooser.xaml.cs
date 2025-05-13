using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

namespace GameContainers.controlls
{
    /// <summary>
    /// Logika interakcji dla klasy StationChooser.xaml
    /// </summary>
    public partial class StationChooser : UserControl
    {
        private bool seeThrough = false;
        SolidColorBrush originalBrush;

        public StationChooser()
        {
            InitializeComponent();
        }

    
        private void SeeThrough()
        {
            originalBrush = (SolidColorBrush)ParentBorder.Background;
            if (originalBrush != null)
            {
                ParentBorder.Background = new SolidColorBrush(originalBrush.Color) { Opacity = 0.1 };
            }

            StationsContainer.Opacity = 0.1;
            ParentBorder.Background.Opacity = 0.1;
            StationsContainer.IsHitTestVisible = false;
        }

        private void HideThrough()
        {
            StationsContainer.Opacity = 1;
            ParentBorder.Background = originalBrush;
            StationsContainer.IsHitTestVisible = true;
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            if (seeThrough)
            {
                HideThrough();
                seeThrough = false;
                Hide.Content = "Ukryj";
            }
            else
            {
                SeeThrough();
                seeThrough = true;
                Hide.Content = "Pokaż";
            }
        }
    }
}
