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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameContainers.controlls
{
    /// <summary>
    /// Logika interakcji dla klasy Station.xaml
    /// </summary>
    public partial class Station : UserControl, IPlanetControl
    {
        public static readonly DependencyProperty StationNameProperty =
            DependencyProperty.Register(
            nameof(StationName),
            typeof(string),
            typeof(Planet),
        new PropertyMetadata(string.Empty));


        public string StationName
        {
            get => (string)GetValue(StationNameProperty);
            set => SetValue(StationNameProperty, value);
        }
        public Station()
        {
            InitializeComponent();
        }

        public Station(int width, int height)
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
