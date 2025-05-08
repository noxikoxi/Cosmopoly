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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameContainers.controlls
{
    /// <summary>
    /// Logika interakcji dla klasy GameButtons.xaml
    /// </summary>
    public partial class GameButtons : UserControl
    {
        public event EventHandler Dice_Clicked;
        public event EventHandler Upgrade_Clicked;
        public event EventHandler SkipTurn_Clicked;
        public GameButtons()
        {
            InitializeComponent();
        }

        public int DiceRollsCount
        {
            get => int.Parse(DiceRolls.Content.ToString());
            set => DiceRolls.Content = value.ToString();
        }

        public void DisableDiceButton()
        {
            RollDiceButton.IsEnabled = false;
        }

        public void EnableDiceButton()
        {
            RollDiceButton.IsEnabled = true;
        }

        private void Dice_Click(object sender, RoutedEventArgs e)
        {
            Dice_Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void Upgrade_Click(object sender, RoutedEventArgs e)
        {
            Upgrade_Clicked?.Invoke(this, EventArgs.Empty);
        }

        private void SkipTurn_Click(object sender, RoutedEventArgs e)
        {
            SkipTurn_Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
