using Engine.models;
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
using System.Windows.Shapes;

namespace Game
{
    /// <summary>
    /// Logika interakcji dla klasy WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
            PlayersList.DataContext = ((App)Application.Current).Players;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if( ((App)Application.Current).Players.Count < 2)
            {
                MessageBox.Show("Dodaj przynajmniej 2 graczy aby rozpocząć grę.");
                return;
            }
            MainWindow gameWindow = new MainWindow();
            gameWindow.Show();

            this.Close();
        }

        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (((App)Application.Current).Players.Count >= 4)
            {
                MessageBox.Show("Nie można dodać więcej niż 4 graczy.");
                return;
            }
            var name = PlayerName.Text;
            if (name != ""){ 
                ((App)Application.Current).Players.Add(new Tuple<string, string>(name, $"ship{((App)Application.Current).Players.Count}"));
            }
            PlayerName.Text = "";
        }
    }
}
