using System;
using System.Collections.Generic;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameContainers.controlls
{
    /// <summary>
    /// Logika interakcji dla klasy Pirates.xaml
    /// </summary>
    public partial class Pirates : UserControl, IPlanetControl
    {
        public Pirates()
        {
            InitializeComponent();
        }

        public Pirates(int width, int height)
        {
            InitializeComponent();
            this.Width = width;
            this.Height = height;
        }

        public StackPanel GetShipsContainer()
        {
            return ShipsContainer;
        }
    }
}
