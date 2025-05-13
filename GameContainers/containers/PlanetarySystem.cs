using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace GameContainers.containers
{
    public class PlanetarySystem : Panel, INotifyPropertyChanged
    {
        public static readonly DependencyProperty SystemNameProperty =
           DependencyProperty.Register(
               nameof(SystemName),
               typeof(string),
               typeof(PlanetarySystem),
               new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnSystemNameChanged)));

        private static void OnSystemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlanetarySystem ps && ps.systemLabel != null)
            {
                ps.systemLabel.Content = e.NewValue?.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string SystemName
        {
            get => (string)GetValue(SystemNameProperty);
            set => SetValue(SystemNameProperty,value);
        }

        public static readonly DependencyProperty RadiusXProperty =
        DependencyProperty.Register(
            nameof(RadiusX),
            typeof(double),
            typeof(PlanetarySystem),
            new FrameworkPropertyMetadata(0.4, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double RadiusX
        {
            get => (double)GetValue(RadiusXProperty);
            set => SetValue(RadiusXProperty, value);
        }

        public static readonly DependencyProperty RadiusYProperty =
        DependencyProperty.Register(
            nameof(RadiusY), 
            typeof(double), 
            typeof(PlanetarySystem),
            new FrameworkPropertyMetadata(0.4, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }

        public static readonly DependencyProperty PercentageSizeProperty =
        DependencyProperty.Register(
            nameof(PercentageSize),
            typeof(double),
            typeof(PlanetarySystem),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double PercentageSize
        {
            get => (double)GetValue(PercentageSizeProperty);
            set => SetValue(PercentageSizeProperty, value);
        }

        public static readonly DependencyProperty UpgradeableProperty =
        DependencyProperty.Register(
            nameof(Upgradeable),
            typeof(bool),
            typeof(PlanetarySystem),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange));

        public bool Upgradeable
        {
            get => (bool)GetValue(UpgradeableProperty);
            set => SetValue(UpgradeableProperty, value);
        }

        private readonly Label systemLabel;
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

        public PlanetarySystem() : base()
        {
            systemLabel = new Label
            {
                Content = SystemName,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false,
            };
            Children.Add(systemLabel);
            MineLevel = 0;
        }

        public void ShowShipyard()
        {
            if(Upgradeable)
            {
                InternalChildren[1].Visibility = Visibility.Visible;
            }
        }

        public void DestroyShipyard()
        {
            if (Upgradeable)
            {
                InternalChildren[1].Visibility = Visibility.Hidden;
            }
        }

        public void Upgrade(string upgrade)
        {
            if (upgrade == "Mine")
            {
                MineLevel++;
            }
            else if (upgrade == "Shipyard")
            {
                InternalChildren[1].Visibility = Visibility.Visible;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Tutaj ustawimy pozycje planet w kontenerze
            int n_children = InternalChildren.Count;

            double centerX = finalSize.Width / 2;
            double centerY = finalSize.Height / 2;

            double radiusX = PercentageSize * finalSize.Width * RadiusX;
            double radiusY = PercentageSize * finalSize.Height * RadiusY;

            InternalChildren[0].Arrange(new Rect(new System.Windows.Point(centerX - InternalChildren[0].DesiredSize.Width/2, centerY - InternalChildren[0].DesiredSize.Height /2), InternalChildren[0].DesiredSize));
            if (Upgradeable)
            {
                InternalChildren[1].Arrange(new Rect(new System.Windows.Point(centerX - InternalChildren[0].DesiredSize.Width / 2 - InternalChildren[1].DesiredSize.Width, centerY - InternalChildren[1].DesiredSize.Height / 2) , InternalChildren[1].DesiredSize));
                InternalChildren[2].Arrange(new Rect(new System.Windows.Point(centerX + InternalChildren[0].DesiredSize.Width / 2 + 3, centerY - InternalChildren[2].DesiredSize.Height / 2), InternalChildren[2].DesiredSize));
            }
            for (int i = Upgradeable ? 3 : 1; i < n_children; i++)
            {
                double baseAngle = 360.0 / (n_children - (Upgradeable ? 3 : 1)) * i;
                double angleInRadians = baseAngle * Math.PI / 180;

                double x = centerX + radiusX * Math.Cos(angleInRadians) - InternalChildren[i].DesiredSize.Width / 2;
                double y = centerY + radiusY * Math.Sin(angleInRadians) - InternalChildren[i].DesiredSize.Height / 2;

                InternalChildren[i].Arrange(new Rect(new System.Windows.Point(x, y), InternalChildren[i].DesiredSize));
            }

            return finalSize;
        }
    }
}
