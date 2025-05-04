using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    class PiratePlanet(byte blockedTurns) : SpaceEntity("Planeta Piratów", false)
    {
        public byte BlockedTurns { set; get; } = blockedTurns;

        protected override string ToStringInternal()
        {
            return "Planeta Piratów";
        }
    }
}
