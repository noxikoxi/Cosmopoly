using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    class MoveCard(string title, string description) : Card(title, description)
    {
        public Station? station { get; set; }

        public override void Apply(Game game)
        {
            if (station!= null)
            {
                game.MovePlayerToPostion(game.Entities.FindIndex((s) => s == station));
            }
        }
    }
}
