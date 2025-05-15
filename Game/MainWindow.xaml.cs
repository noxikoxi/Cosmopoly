using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Game.ViewModels;
using GameContainers;
using GameContainers.controlls;
using GameContainers.containers;
using GameContainers.converters;
using System.Globalization;
using Game.utils;
using Engine.models;
using Engine.strategies;
using System.Diagnostics;
using System.Numerics;
using System;
using System.Windows.Data;


namespace Game
{
    public partial class MainWindow : Window
    {
        Engine.Game game;
        List<IPlanetControl> galaxyEntities;
        List<Image> playerShips;
        List<GameContainers.containers.PlanetarySystem> UISystems;

        const int PLANET_WIDTH = 150;
        const int PLANET_HEIGHT = 110;

        public MainWindow()
        {
            InitializeComponent();
            galaxyEntities = new();
            playerShips = new();
            UISystems = new();
            PlayersList.DataContext = ((App)Application.Current).Players;
            // Initialize the game with players from the application context
            var appPlayers = ((App)Application.Current).Players;
            Engine.models.Player[] players = new Engine.models.Player[appPlayers.Count];
            ShipNameToImageConverter shipName = new();
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
                string configPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(appDir, "configs"));
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
                    var index = game.PlanetarySystems.IndexOf(planetSystem);
                    var systemPlanetSource = new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/planet{index}.png"));
                    system.RadiusX = 1;
                    system.RadiusY = 1;
                    system.Width = 180;
                    system.Height = 90;
                    system.Upgradeable = true;
                    system.SystemName = planetSystem.Name;

                    Image Shipyard = new Image();
                    Shipyard.Source = new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/Shipyard.png"));
                    Shipyard.Width = 30;
                    Shipyard.Height = 30;
                    Shipyard.Visibility = Visibility.Hidden;
                    system.Children.Add(Shipyard);

                    Image Mine = new Image();
                    Mine.Source = new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/Mine.png"));
                    Mine.Width = 30;
                    Mine.Height = 30;

                    Label label = new();
                    label.Foreground = Brushes.White;
                    Binding binding = new Binding("MineLevel")
                    {
                        Source = system,
                        Mode = BindingMode.OneWay
                    };
                    label.SetBinding(ContentProperty, binding);
                    label.HorizontalAlignment = HorizontalAlignment.Center;


                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Vertical;
                    panel.Children.Add(Mine);
                    panel.Children.Add(label);
                    system.Children.Add(panel);

                    UISystems.Add(system);

                    var nextSystem = game.GetPlanetarySystem(game.Entities[idx]);
                    while (nextSystem != null && nextSystem == planetSystem)
                    {
                        if (game.Entities[idx] is HabitablePlanet)
                        {
                            Planet planet = new(PLANET_WIDTH, PLANET_HEIGHT);
                            planet.PlanetName = game.Entities[idx].Name;
                            planet.PlanetOwner = "Niezamieszkana";
                            planet.PlanetImageSource = systemPlanetSource;
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

                        if(entity is GameContainers.controlls.Station station)
                        {
                            station.StationName = model.Name;
                        }
                    }
                    idx++;
                }
            }

            game.SetInitialCredits();
            // Bindowanie Danych Gracza
            PlayerInfo.DataContext = game.CurrentPlayer;

            // ships images
            for (int i = 0; i < players.Length; i++)
            {
                galaxyEntities[0].GetShipsContainer().Children.Add(playerShips[i]);
            }

            this.Loaded += (s, e) => CanvasWriter.DrawArrows(galaxyEntities, ArrowCanvas);
            this.SizeChanged += (s, e) => CanvasWriter.DrawArrows(galaxyEntities, ArrowCanvas);
        }

        private void SetInfoAndNextPlayer()
        {
            game.NextPlayer();
            PlayerInfo.DataContext = game.CurrentPlayer;
        }

        private async Task MoveShipWithDelay(int rolled)
        {
            var currentPlayerIndex = game.GetCurrentPlayerIndex();
            for (int i = 0; i < rolled; i++)
            {
                var currentPlanetContainer = galaxyEntities[game.CurrentPlayer.position].GetShipsContainer();
                var shipToMove = playerShips[currentPlayerIndex];

                // Usuń statek z bieżącej planety
                currentPlanetContainer.Children.Remove(shipToMove);

                // Przesuń gracza (logika)
                game.MovePlayerByPoint();

                // Dodaj statek do nowej planety
                galaxyEntities[game.CurrentPlayer.position].GetShipsContainer().Children.Add(shipToMove);

                // Czekaj bez blokowania UI
                await Task.Delay(200);
            }
        }

        private async Task MoveShipWithDelayUntil(int position)
        {
            var currentPlayerIndex = game.GetCurrentPlayerIndex();
            while (game.CurrentPlayer.position != position)
            {
                var currentPlanetContainer = galaxyEntities[game.CurrentPlayer.position].GetShipsContainer();
                var shipToMove = playerShips[currentPlayerIndex];
                currentPlanetContainer.Children.Remove(shipToMove);
                game.MovePlayerByPoint();
                galaxyEntities[game.CurrentPlayer.position].GetShipsContainer().Children.Add(shipToMove);
                await Task.Delay(50);
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
                Description = $"Czy chcesz wysłać osadników na planetę {planet.Name} i wybudować port kosmiczny?\nKoszt {game.GetHousingCost()}",
            };

            cardViewModel.AddOption(
                label: "Zasiedl planetę",
                action: (o) =>
                {
                    PopupBG.Visibility = Visibility.Hidden;
                    Card.Visibility = Visibility.Hidden;
                    game.SettlePlanet();
                    // Owner label change
                    Planet pl = (Planet)galaxyEntities[game.CurrentPlayer.position];
                    pl.PlanetOwner = game.CurrentPlayer.Name;
                    pl.Upgrade("Hotel");
                    SetInfoAndNextPlayer();
                },
                canExecute: (o) => canAfford
            );
            cardViewModel.AddOption(
                label: "Anuluj",
                action: (o) =>
                {
                    PopupBG.Visibility = Visibility.Hidden;
                    Card.Visibility = Visibility.Hidden;
                    SetInfoAndNextPlayer();
                },
                canExecute: (o) => true
            );
            Card.DataContext = cardViewModel;
        }

        private async void GameButtons_Dice_Clicked(object sender, EventArgs e)
        {
            GameButtons.DisableDiceButton();
            var rolled = game.RollDice();
            GameButtons.DiceRollsCount = rolled;

            await MoveShipWithDelay(rolled);

            var currentSpaceEntity = game.GetCurrentPlayerPositionEntity();

            if (currentSpaceEntity is HabitablePlanet planet && planet.Owner == null && game.CanPlayerSettle())
            {
                ShowPlanetSettlement(planet);
            }
            else if (currentSpaceEntity is HabitablePlanet planet2 && planet2.Owner != null && planet2.Owner != game.CurrentPlayer)
            {
                PopupBG.Visibility = Visibility.Visible;
                Card.Visibility = Visibility.Visible;
                var cost = game.GetCurrentPositionHousingCost();
                var cardViewModel = new CardVM
                {
                    Title = $"Pobyt w hotelu",
                    Description = $"Musisz nocować na planecie {planet2.Name}.\nKoszt hotelu wynosi {cost}."
                };
                cardViewModel.AddOption(
                    label: "Zapłać", 
                    action: (o) => HandleHousing(cost, planet2.Owner),
                    canExecute: (o) => true
                );
                Card.DataContext = cardViewModel;
            }else if (currentSpaceEntity is Engine.models.Singularity || currentSpaceEntity is PiratePlanet)
            {
                Engine.models.Card card;
                if (currentSpaceEntity is Engine.models.Singularity)
                {
                    card = game.GetCardFromSingularity();
                }
                else
                {
                    card = game.GetPirateCard();
                }
                PopupBG.Visibility = Visibility.Visible;
                Card.Visibility = Visibility.Visible;

                string desc = card.Description;
                if (card.Title == "Podatek od nieruchomości")
                {
                    desc += $"\nKoszt {game.GetCurrentPlayerPropertyTax(card.strategies[0].Value)}";

                }
                var cardViewModel = new CardVM
                {
                    Title = card.Title,
                    Description = desc,
                };
                var len = card.strategies.Count();
                if ( card.ApplyTogether)
                {
                    len = 1; // I tak nie ma opcji do wyboru
                }
                for (int i = 0; i < len; i++)
                {
                    var strategy = card.strategies[i];
                    AddCardStrategyOption(strategy, cardViewModel, i, card);
                }
                Card.DataContext = cardViewModel;
            }
            else
            {
                SetInfoAndNextPlayer();
            }
            GameButtons.EnableDiceButton();
        }

        private void AddCardStrategyOption(CardStrategy strategy, CardVM viewModel, int i, Engine.models.Card card)
        {
            viewModel.AddOption(
            label: Engine.utils.StrategyNameConverter.ConvertName(strategy.Type),
            action: (o) =>
            {
                PopupBG.Visibility = Visibility.Hidden;
                Card.Visibility = Visibility.Hidden;
                if (strategy.Type == strategyType.Move)
                {
                    HandleMoveCard(card);
                }else if (strategy.Type == strategyType.Cancel)
                {
                    PopupBG.Visibility = Visibility.Hidden;
                    Card.Visibility = Visibility.Hidden;
                    SetInfoAndNextPlayer();
                }
                else
                {
                    var idx = game.ApplyCard(card, i);
                    if (strategy.Type == strategyType.TakeCredits && game.IsPlayerInBankruptcy(game.CurrentPlayer))
                    {
                        ShowBancruptyMessage();
                    }
                    else if (strategy.Type == strategyType.Destroy)
                    {
                        UISystems[idx].DestroyShipyard();
                    } else
                    {
                        SetInfoAndNextPlayer();
                    }
                }
               
            },
                canExecute: (o) =>
                {
                    if (strategy.Type == strategyType.TakeCredits)
                    {
                        return game.CurrentPlayer.Credits >= card.strategies[0].Value;
                    }
                    else if (strategy.Type == strategyType.UseShield)
                    {
                        return game.CurrentPlayer.ShieldCards > 0;
                    }
                    else
                    {
                        return true;
                    }
                }
            );

        }

        private void HandleMoveCard(Engine.models.Card card)
        { 
            PopupBG.Visibility = Visibility.Visible;
            StationChooser.Visibility = Visibility.Visible;
            StationChooserVM stationChooser = new StationChooserVM();
            foreach (KeyValuePair<Engine.models.Station, int> station in game.GetStationWithIdx())
            {

                stationChooser.AddStation(
                    name: station.Key.Name,
                    action: async (o) =>
                    {
                        StationChooser.Visibility = Visibility.Hidden;
                        PopupBG.Visibility = Visibility.Hidden;
                        await MoveShipWithDelayUntil(station.Value);
                        SetInfoAndNextPlayer();
                    });
            }
            stationChooser.AddStation(
                name: "Pozostań na swoim miejscu",
                action: (o) =>
                {
                    StationChooser.Visibility = Visibility.Hidden;
                    PopupBG.Visibility = Visibility.Hidden;
                    SetInfoAndNextPlayer();
                });
            StationChooser.DataContext = stationChooser;
            
        }

        private void HandleHousing(long cost, Player owner)
        {
            PopupBG.Visibility = Visibility.Hidden;
            Card.Visibility = Visibility.Hidden;
            game.TransferCredits(game.CurrentPlayer, owner, cost);
            if (game.IsPlayerInBankruptcy(game.CurrentPlayer))
            {
                ShowBancruptyMessage();
            }
            else
            {
                SetInfoAndNextPlayer();
            }
        }

        private void HandleAccepteBancruptyMessage()
        {
            PopupBG.Visibility = Visibility.Hidden;
            Card.Visibility = Visibility.Hidden;
            game.RemoveBankruptPlayer(game.CurrentPlayer);
            foreach (var entity in galaxyEntities)
            {
                if (entity is Planet planet && planet.PlanetOwner == game.CurrentPlayer.Name)
                {
                    planet.PlanetOwner = "Niezamieszkana";
                    planet.ResetBuildings();
                }

            }
            SetInfoAndNextPlayer();
        }

        private void ShowBancruptyMessage()
        {
            PopupBG.Visibility = Visibility.Visible;
            Card.Visibility = Visibility.Visible;
            var cardViewModel = new CardVM
            {
                Title = $"Bankructwo",
                Description = $"Nie masz wystarczającej ilości kredytów, aby opłacić hotel.\nZbankrutowałeś."
            };
            cardViewModel.AddOption(
                label: "Akceptuj",
                canExecute: (o) => true,
                action: (o) => HandleAccepteBancruptyMessage()
                );
            Card.DataContext = cardViewModel;
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
                    img: new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/planet{game.PlanetarySystems.IndexOf(system)}.png")),
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
                    img: new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/planet{game.PlanetarySystems.IndexOf(game.GetPlanetarySystem(planet))}.png")),
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
            foreach (KeyValuePair<string, int> pair in game.GetPossiblePlanetUpgrades(planet))
            {
                upgradeChooser.AddUpgrade(
                    name: UpgradesNames.ConvertName(pair.Key, planet.GetBuildingLevel(pair.Key)),
                    level: planet.GetBuildingLevel(pair.Key),
                    price: pair.Value,
                    effect: game.GetUpgradeEffect(pair.Key),
                    action: (o) =>
                    {
                        PopupBG.Visibility = Visibility.Hidden;
                        UpgradeChooser.Visibility = Visibility.Hidden;

                        game.UpgradePlanet(planet, pair.Key);
                        var UIPlanet = galaxyEntities[game.GetEntityIndex(planet)] as Planet;
                        UIPlanet.Upgrade(pair.Key);
                    },
                    img: new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/{pair.Key}.png")),
                    canUpgrade: (o) => game.CurrentPlayer.Credits >= pair.Value
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
                    name: UpgradesNames.ConvertName(pair.Key, system.GetBuildingLevel(pair.Key)),
                    level: system.GetBuildingLevel(pair.Key),
                    price: pair.Value,
                    effect: game.GetUpgradeEffect(pair.Key),
                    action: (o) =>
                    {
                        PopupBG.Visibility = Visibility.Hidden;
                        UpgradeChooser.Visibility = Visibility.Hidden;

                        game.UpgradeSystem(system, pair.Key);
                        UISystems[game.GetPlanetarySystemIndex(system)].Upgrade(pair.Key);
                    },
                    img: new BitmapImage(new Uri($"pack://application:,,,/GameContainers;component/assets/{pair.Key}.png")),
                    canUpgrade: (o) => game.CurrentPlayer.Credits >= pair.Value
                    );
            }
            UpgradeChooser.DataContext = upgradeChooser;
        }

        private void GameButtons_SkipTurn_Clicked(object sender, EventArgs e)
        {
            if (game.CanSkipTurn())
            {
                game.SkipPlayerTurn();
                PlayerInfo.DataContext = game.CurrentPlayer;
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
            UpgradeChooser.Visibility = Visibility.Hidden;
            EntityChooser.Visibility = Visibility.Visible;

        }
    }
}