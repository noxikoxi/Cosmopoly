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
        public static readonly DependencyProperty RadiusXProperty =
        DependencyProperty.Register(
            nameof(RadiusX),
            typeof(double),
            typeof(PlanetarySystem),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsArrange));

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
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double RadiusY
        {
            get => (double)GetValue(RadiusYProperty);
            set => SetValue(RadiusYProperty, value);
        }


        public PlanetarySystem() : base()
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxChildWidth = 0;
            double maxChildHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                if (child.DesiredSize.Width > maxChildWidth)
                    maxChildWidth = child.DesiredSize.Width;

                if (child.DesiredSize.Height > maxChildHeight)
                    maxChildHeight = child.DesiredSize.Height;
            }


            double totalWidth = RadiusX * 2 + maxChildWidth;
            double totalHeight = RadiusY * 2 + maxChildHeight;

            return new Size(totalWidth, totalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Tutaj ustawimy pozycje planet w kontenerze
            int n_children = InternalChildren.Count;

            double centerX = finalSize.Width / 2;
            double centerY = finalSize.Height / 2;

            for (int i = 0; i < n_children; i++)
            {
                double angleInDegrees = 360.0 / n_children * i;
                double angleInRadians = angleInDegrees * Math.PI / 180;

                double x = centerX + RadiusX * Math.Cos(angleInRadians) - InternalChildren[i].DesiredSize.Width / 2;
                double y = centerY + RadiusY * Math.Sin(angleInRadians) - InternalChildren[i].DesiredSize.Height / 2;

                InternalChildren[i].Arrange(new Rect(new System.Windows.Point(x, y), InternalChildren[i].DesiredSize));
            }

            return finalSize;
        }
    }
}
