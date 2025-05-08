using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using GameContainers.controlls;

namespace Game.utils
{
    internal static class CanvasWriter
    {
        public static void DrawArrows(List<IPlanetControl> galaxyEntities, System.Windows.Controls.Canvas ArrowCanvas)
        {
            ArrowCanvas.Children.Clear();

            int count = galaxyEntities.Count;
            if (count < 2) return;

            for (int i = 0; i < count; i++)
            {
                var fromControl = galaxyEntities[i] as UIElement;
                var toControl = galaxyEntities[(i + 1) % count] as UIElement; // wrap to first

                if (fromControl == null || toControl == null)
                    continue;

                Point start = fromControl.TranslatePoint(
                    new Point(fromControl.RenderSize.Width / 2, fromControl.RenderSize.Height / 2),
                    ArrowCanvas);
                Point end = toControl.TranslatePoint(
                    new Point(toControl.RenderSize.Width / 2, toControl.RenderSize.Height / 2),
                    ArrowCanvas);

                DrawArrow(start, end, ArrowCanvas);
            }
        }

        private static void DrawArrow(Point start, Point end, System.Windows.Controls.Canvas ArrowCanvas)
        {
            var line = new Line
            {
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y,
                Stroke = Brushes.LightSkyBlue,
                StrokeThickness = 1.5,
                Opacity = 0.7,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            ArrowCanvas.Children.Add(line);
        }
    }
}
