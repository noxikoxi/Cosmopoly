using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    class ShieldCard(string title, string description) : Card(title, description)
    {
        public override void Apply(Game game)
        {
        }
    }
}
