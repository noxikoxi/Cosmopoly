using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Size = System.Windows.Size;

namespace GameContainers.containers
{
    public class PlanetarySystem : Panel
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

        private readonly Label systemLabel;

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

            for (int i = 1; i < n_children; i++)
            {
                double baseAngle = 360.0 / (n_children-1) * i;
                double angleInRadians = baseAngle * Math.PI / 180;

                double x = centerX + radiusX * Math.Cos(angleInRadians) - InternalChildren[i].DesiredSize.Width / 2;
                double y = centerY + radiusY * Math.Sin(angleInRadians) - InternalChildren[i].DesiredSize.Height / 2;

                InternalChildren[i].Arrange(new Rect(new System.Windows.Point(x, y), InternalChildren[i].DesiredSize));
            }

            return finalSize;
        }
    }
}
