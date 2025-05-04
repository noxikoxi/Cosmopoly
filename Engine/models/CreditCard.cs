using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    class CreditCard : Card 
    {
        public int Amount { get; set; }

        public CreditCard(string title, string description, int amount) : base(title, description)
        {
            Amount = amount;
        }

        public override void Apply(Game game)
        {
            game.AddCredits(game.GetCurrentPlayer(), Amount);
        }
    }
}
