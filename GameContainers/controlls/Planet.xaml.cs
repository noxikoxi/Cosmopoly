using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Planet : UserControl, IPlanetControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty PlanetNameProperty =
            DependencyProperty.Register(
            nameof(PlanetName),
            typeof(string),
            typeof(Planet),
        new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PlanetImageSourceProperty =
            DependencyProperty.Register(
            nameof(PlanetImageSource),
            typeof(ImageSource),
            typeof(Planet),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PlanetOwnerProperty =
        DependencyProperty.Register(
            nameof(PlanetOwner),
            typeof(string),
            typeof(Planet),
            new PropertyMetadata(string.Empty));

        public ImageSource PlanetImageSource
        {
            get => (ImageSource)GetValue(PlanetImageSourceProperty);
            set => SetValue(PlanetImageSourceProperty, value);
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

        private int _hotelLevel;
        public int HotelLevel
        {
            get => _hotelLevel;
            set
            {
                if (_hotelLevel != value)
                {
                    _hotelLevel = value;
                    OnPropertyChanged(nameof(HotelLevel));
                }
            }
        }

        private int _farmLevel;
        public int FarmLevel
        {
            get => _farmLevel;
            set
            {
                if (_farmLevel != value)
                {
                    _farmLevel = value;
                    OnPropertyChanged(nameof(FarmLevel));
                }
            }
        }

        private int _mineLevel;
        public int MineLevel
        {
            get => _mineLevel;
            set
            {
                if (_mineLevel != value)
                {
                    _mineLevel = value;
                    OnPropertyChanged(nameof(MineLevel));
                }
            }
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
            ResetBuildings();
            DataContext = this;
        }

        public Panel GetShipsContainer()
        {
            return ShipsContainer;
        }

        public void ResetBuildings()
        {
            HotelLevel = 0;
            FarmLevel = 0;
            MineLevel = 0;
        }

        public void Upgrade(string building)
        {
            switch (building)
            {
                case "Hotel":
                    HotelLevel++;
                    break;
                case "Farm":
                    FarmLevel++;
                    break;
                case "Mine":
                    MineLevel++;
                    break;
                default:
                    throw new ArgumentException("Invalid building type");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
