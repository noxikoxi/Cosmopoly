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
    /// Logika interakcji dla klasy Planet.xaml
    /// </summary>
    public partial class Planet : UserControl, IPlanetControl
    {
        public static readonly DependencyProperty PlanetNameProperty =
            DependencyProperty.Register(
            nameof(PlanetName),
            typeof(string),
            typeof(Planet),
        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PlanetColorProperty =
            DependencyProperty.Register(
            nameof(PlanetColor),
            typeof(SolidColorBrush),
            typeof(Planet),
            new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty PlanetOwnerProperty =
        DependencyProperty.Register(
            nameof(PlanetOwner),
            typeof(string),
            typeof(Planet),
            new PropertyMetadata(string.Empty));

        public SolidColorBrush PlanetColor
        {
            get => (SolidColorBrush)GetValue(PlanetColorProperty);
            set => SetValue(PlanetColorProperty, value);
        }

        public string PlanetName
        {
            get => (string)GetValue(PlanetNameProperty);
            set => SetValue(PlanetNameProperty, value);
        }

        public string PlanetOwner
        {
            get => (string)GetValue(PlanetOwnerProperty);
            set => SetValue(PlanetOwnerProperty, value);
        }
        public Planet()
        {
            InitializeComponent();

        }

        public Planet(int width, int height)
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
