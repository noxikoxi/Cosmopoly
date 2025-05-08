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
using System.Runtime.CompilerServices;
using Game.utils;
using Engine.models;
using System.Numerics;
using System.Diagnostics;

namespace Game
{
    public partial class MainWindow : Window
    {
        Engine.Game game;
        List<IPlanetControl> galaxyEntities;
        List<Image> playerShips;

        CardVM cardVm;

        const int PLANET_WIDTH = 150;
        const int PLANET_HEIGHT = 100;

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
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                });
            }
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory;
                string configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appDir, "..", "..", "..", "..", "configs"));
                game = new Engine.Game(players, configPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;

            }
            
            // After config loading, we can fill the board
            var idx = 0;
            while (idx < game.Entities.Count)
            {
                var planetSystem = game.GetPlanetarySystem(game.Entities[idx]);
                if (planetSystem != null)
                {
                    GameContainers.containers.PlanetarySystem system = new();
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
                            Planet planet = new(PLANET_WIDTH, PLANET_HEIGHT );
                            planet.PlanetName = game.Entities[idx].Name;
                            planet.PlanetOwner = "Niezamieszkana";
                            planet.PlanetColor = systemColor;
                            //planet.Tag = game.Entities[idx];
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
                    var model = game.Entities[idx];
                    var entity = PlanetControlFactory.CreateControl(model, PLANET_WIDTH, PLANET_HEIGHT);
                    if (entity != null)
                    {
                        GalaxyInside.Children.Add((UIElement)entity);
                        galaxyEntities.Add(entity);
                    }
                    idx++;
                }
            }

            game.SetInitialCredits();
            SetInfo();

            // ships images
            for (int i = 0; i < players.Length; i++)
            {
                galaxyEntities[0].GetShipsContainer().Children.Add(playerShips[i]);
            }

            this.Loaded += (s, e) => CanvasWriter.DrawArrows(galaxyEntities, ArrowCanvas);
            this.SizeChanged += (s, e) => CanvasWriter.DrawArrows(galaxyEntities, ArrowCanvas);
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

        public void ShowPlanetSettlement(HabitablePlanet planet)
        {
            PopupBG.Visibility = Visibility.Visible;
            Card.Visibility = Visibility.Visible;

            bool canAfford = game.CanPlayerSettle();
            var cardViewModel = new CardVM
            {
                Title = $"Zasiedlenie planety",
                Description = $"Czy chcesz wysłać osadników na planetę {planet.Name}?\nKoszt {game.GetHousingCost()}",
                OnAccept = (o) => {
                    PopupBG.Visibility = Visibility.Hidden;
                    Card.Visibility = Visibility.Hidden;
                    game.SettlePlanet();
                    // Owner label change
                    Planet pl = (Planet)galaxyEntities[game.GetCurrentPlayer().position];
                    pl.PlanetOwner = game.GetCurrentPlayer().Name;
                    game.NextPlayer();
                    SetInfo();
                },
                OnDecline = (o) => {
                    PopupBG.Visibility = Visibility.Hidden;
                    Card.Visibility = Visibility.Hidden;
                    game.NextPlayer();
                    SetInfo();
                },
                CanAccept = (o) => canAfford
            };

            Card.DataContext = cardViewModel;
            SetInfo();
        }

        private async void GameButtons_Dice_Clicked(object sender, EventArgs e)
        {
            GameButtons.DisableDiceButton();
            var rolled = game.RollDice();
            GameButtons.DiceRollsCount = rolled;

            await MoveShipsWithDelay(rolled);

            var currentSpaceEntity = game.GetCurrentPlayerPositionEntity();

            if (currentSpaceEntity is Engine.models.HabitablePlanet planet && planet.Owner == null && game.CanPlayerSettle())
            {
                ShowPlanetSettlement(planet);
            }
            else
            {
                game.NextPlayer();
                SetInfo();
            }
            GameButtons.EnableDiceButton();
        }

        private void GameButtons_Upgrade_Clicked(object sender, EventArgs e)
        {
            PopupBG.Visibility = Visibility.Visible;
            EntityChooser.Visibility = Visibility.Visible;
            EntityChooserVM entityChooser = new EntityChooserVM();
            foreach(var system in game.GetPlayerSystems())
            {
                entityChooser.AddEntity(
                    name: $"System {system.Name}",
                    color: planetColors[game.PlanetarySystems.IndexOf(system)],
                    action: (o) =>
                    {
                        PopupBG.Visibility = Visibility.Hidden;
                        EntityChooser.Visibility = Visibility.Hidden;
                        ShowUpgrades(system);
                    });

            }
            foreach(var planet in game.GetPlayerPlanets())
            {
                entityChooser.AddEntity(
                    name: $"Planet {planet.Name}",
                    color: planetColors[game.PlanetarySystems.IndexOf(game.GetPlanetarySystem(planet))],
                    action: (o) =>
                    {
                        EntityChooser.Visibility = Visibility.Hidden;
                        ShowUpgrades(planet);
                    });
            }
            EntityChooser.DataContext = entityChooser;
        }

        private void ShowUpgrades(HabitablePlanet planet)
        {
            UpgradeChooser.Visibility = Visibility.Visible;
            UpgradeChooserVM upgradeChooser = new UpgradeChooserVM();
            // Dictionary<string, int>
            foreach (KeyValuePair<string, int> pair in game.GetPossiblePlanetUpgrades(planet))
            {
                upgradeChooser.AddUpgrade(
                    name: UpgradesNames.ConvertName(pair.Key),
                    level: planet.GetBuildingLevel(pair.Key),
                    price: pair.Value,
                    effect: game.GetUpgradeEffect(pair.Key),
                    action: (o) =>
                    {
                        PopupBG.Visibility = Visibility.Hidden;
                        UpgradeChooser.Visibility = Visibility.Hidden;

                        game.UpgradePlanet(planet, pair.Key);
                    },
                    canUpgrade: (o) => game.GetCurrentPlayer().credits >= pair.Value
                    );
                
            }
            UpgradeChooser.DataContext = upgradeChooser;
        }

        private void ShowUpgrades(Engine.models.PlanetarySystem system)
        {
            PopupBG.Visibility = Visibility.Visible;
            UpgradeChooser.Visibility = Visibility.Visible;
            UpgradeChooserVM upgradeChooser = new UpgradeChooserVM();
            foreach (KeyValuePair<string, int> pair in game.GetPossibleSystemUpgrades(system))
            {
                upgradeChooser.AddUpgrade(
                    name: UpgradesNames.ConvertName(pair.Key),
                    level: system.GetBuildingLevel(pair.Key),
                    price: pair.Value,
                    effect: game.GetUpgradeEffect(pair.Key),
                    action: (o) =>
                    {
                        PopupBG.Visibility = Visibility.Hidden;
                        UpgradeChooser.Visibility = Visibility.Hidden;

                        game.UpgradeSystem(system, pair.Key);
                    },
                    canUpgrade: (o) => game.GetCurrentPlayer().credits >= pair.Value
                    );
            }
            UpgradeChooser.DataContext = upgradeChooser;
        }

        private void GameButtons_SkipTurn_Clicked(object sender, EventArgs e)
        {
            if (game.CanSkipTurn())
            {
                game.SkipPlayerTurn();
                SetInfo();
            }
            else
            {
                MessageBox.Show("Nie możesz teraz pominąć tury.");
            }
        }

        private void EntityChooser_Exit_Clicked(object sender, EventArgs e)
        {
            PopupBG.Visibility = Visibility.Hidden;
            EntityChooser.Visibility = Visibility.Hidden;
        }

        private void UpgradeChooser_Exit_Clicked(object sender, EventArgs e)
        {
            PopupBG.Visibility = Visibility.Hidden;
            UpgradeChooser.Visibility = Visibility.Hidden;

        }
    }
}