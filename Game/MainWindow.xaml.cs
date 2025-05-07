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
using Game.ViewModels;
using GameContainers;
using System.IO;
using GameContainers.controlls;
using GameContainers.containers;

namespace Game
{
    public partial class MainWindow : Window
    {
        Engine.Game game;

        const int PLANET_WIDTH = 80;
        const int PLANET_HEIGHT = 100;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize the game with players from the application context
            Engine.models.Player[] players = [.. ((App)Application.Current).Players];
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appDir,"..", "..", "..", "..", "configs"));
                //MessageBox.Show($"Podana ścieżka configu: {configPath}");
                game = new Engine.Game(players, configPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
            SolidColorBrush[] planetColors = new SolidColorBrush[]
            {
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.PaleGreen),
                new SolidColorBrush(Colors.DarkGray),
                new SolidColorBrush(Colors.DarkGoldenrod),
                new SolidColorBrush(Colors.Cyan),
                new SolidColorBrush(Colors.Magenta),
                new SolidColorBrush(Colors.LightGray),
                new SolidColorBrush(Colors.DarkKhaki)
            };

            // After config loading, we can fill the board
            var idx = 0;
            while (idx < game.Entities.Count)
            {
                var planetSystem = game.GetPlanetarySystem(game.Entities[idx]);
                if (planetSystem != null)
                {
                    PlanetarySystem system = new PlanetarySystem();
                    var systemColor = planetColors[game.PlanetarySystems.IndexOf(planetSystem)];
                    system.RadiusX = 1;
                    system.RadiusY = 1;
                    system.Width = 180;
                    system.Height = 90;
                    system.SystemName = planetSystem.Name;
                    var nextSystem = game.GetPlanetarySystem(game.Entities[idx]);
                    while (nextSystem != null && nextSystem == planetSystem)
                    {
                        if (game.Entities[idx] is Engine.models.HabitablePlanet) {
                            Planet planet = new();
                            planet.PlanetName = game.Entities[idx].Name;
                            planet.Width = PLANET_WIDTH;
                            planet.Height = PLANET_HEIGHT;
                            planet.PlanetOwner = "Niezamieszkana";
                            planet.PlanetColor = systemColor;
                            planet.Tag = game.Entities[idx]; // możesz przypiąć oryginalny obiekt
                            system.Children.Add(planet);
                        }
                        idx++;
                        if (idx == game.Entities.Count)
                        {
                            break;
                        }
                        nextSystem = game.GetPlanetarySystem(game.Entities[idx]);
                    }

                    Galaxy.Children.Add(system);
                }
                else
                {
                    if (game.Entities[idx] is Engine.models.Singularity)
                    {
                        Singularity singularity = new Singularity();
                        singularity.Width = PLANET_WIDTH + 20;
                        singularity.Height = PLANET_HEIGHT;
                        GalaxInside.Children.Add(singularity);
                    }
                    else if (game.Entities[idx] is Engine.models.Station)
                    {
                        Station station = new Station();
                        station.Width = PLANET_WIDTH + 20;
                        station.Height = PLANET_HEIGHT;
                        station.StationName = game.Entities[idx].Name;
                        GalaxInside.Children.Add(station);
                    } else {
                        Pirates pirates = new Pirates();
                        pirates.Width = PLANET_WIDTH + 20;
                        pirates.Height = PLANET_HEIGHT;
                        GalaxInside.Children.Add(pirates);  
                    }
                    idx++;
                }
            }
        }
    }
}