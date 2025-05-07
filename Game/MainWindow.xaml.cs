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
using GameContainers.converters;
using System.Globalization;

namespace Game
{
    public partial class MainWindow : Window
    {
        Engine.Game game;
        List<IPlanetControl> galaxyEntities;
        List<Image> playerShips;

        const int PLANET_WIDTH = 150;
        const int PLANET_HEIGHT = 100;

        public MainWindow()
        {
            InitializeComponent();
            galaxyEntities = new List<IPlanetControl>();
            playerShips = new List<Image>();
            PlayersList.DataContext = ((App)Application.Current).Players;
            // Initialize the game with players from the application context
            var appPlayers = ((App)Application.Current).Players;
            Engine.models.Player[] players = new Engine.models.Player[appPlayers.Count];
            ShipNameToImageConverter shipName = new ShipNameToImageConverter();
            foreach (var player in appPlayers)
            {
                var imageSource = shipName.Convert(player.Item2, typeof(BitmapImage), null, CultureInfo.InvariantCulture);
                players[appPlayers.IndexOf(player)] = new Engine.models.Player(player.Item1, 0);
                playerShips.Add(new Image()
                {
                    Source = imageSource as BitmapImage,
                    Width = 36,
                    Height = 45,
                    Tag = player.Item1,
                    HorizontalAlignment=HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,   
                });
            }
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appDir, "..", "..", "..", "..", "configs"));
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
                        if (game.Entities[idx] is Engine.models.HabitablePlanet)
                        {
                            Planet planet = new();
                            planet.PlanetName = game.Entities[idx].Name;
                            planet.Width = PLANET_WIDTH;
                            planet.Height = PLANET_HEIGHT;
                            planet.PlanetOwner = "Niezamieszkana";
                            planet.PlanetColor = systemColor;
                            planet.Tag = game.Entities[idx]; // możesz przypiąć oryginalny obiekt
                            system.Children.Add(planet);

                            galaxyEntities.Add(planet);
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
                        singularity.Width = PLANET_WIDTH;
                        singularity.Height = PLANET_HEIGHT;
                        singularity.Tag = game.Entities[idx];
                        GalaxInside.Children.Add(singularity);

                        galaxyEntities.Add(singularity);
                    }
                    else if (game.Entities[idx] is Engine.models.Station)
                    {
                        Station station = new Station();
                        station.Width = PLANET_WIDTH;
                        station.Height = PLANET_HEIGHT;
                        station.StationName = game.Entities[idx].Name;
                        station.Tag = game.Entities[idx];
                        GalaxInside.Children.Add(station);

                        galaxyEntities.Add(station);
                    }
                    else
                    {
                        Pirates pirates = new Pirates();
                        pirates.Width = PLANET_WIDTH;
                        pirates.Height = PLANET_HEIGHT;
                        pirates.Tag = game.Entities[idx];
                        GalaxInside.Children.Add(pirates);

                        galaxyEntities.Add(pirates);
                    }
                    idx++;
                }


            }

            game.SetInitialCredits();
            SetInfo();

            // ships images
            for (int i=0; i < players.Length; i++)
            {
                galaxyEntities[0].GetShipsContainer().Children.Add(playerShips[i]);
            }
        }

        private void SetInfo()
        {
            PlayerInfo.PlayerNameText = game.GetCurrentPlayer().Name;
            PlayerInfo.CreditsAmount = game.GetCurrentPlayer().credits;
            PlayerInfo.PirateCardCount = game.GetCurrentPlayerPirateCards();
        }

        private async Task MoveShipsWithDelay(int rolled)
        {
            for (int i = 0; i < rolled; i++)
            {
                var currentPlayerPosition = game.GetCurrentPlayer().position;
                var currentPlayerIndex = game.GetCurrentPlayerIndex();
                var currentPlanetContainer = galaxyEntities[currentPlayerPosition].GetShipsContainer();
                var shipToMove = playerShips[currentPlayerIndex];

                // Usuń statek z bieżącej planety
                currentPlanetContainer.Children.Remove(shipToMove);

                // Przesuń gracza (logika)
                game.MovePlayerByPoints(1);

                // Dodaj statek do nowej planety
                galaxyEntities[game.GetCurrentPlayer().position].GetShipsContainer().Children.Add(shipToMove);

                // Czekaj bez blokowania UI
                await Task.Delay(200);
            }
        }

        private async void GameButtons_Dice_Clicked(object sender, EventArgs e)
        {
            var rolled = game.RollDice();
            GameButtons.DiceRollsCount = rolled;

            await MoveShipsWithDelay(rolled);



            game.NextPlayer();
            SetInfo();

            
        }

        private void GameButtons_Upgrade_Clicked(object sender, EventArgs e)
        {

        }
    }
}