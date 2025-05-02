using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmopolyEngine.models
{
    class PiratePlanet : SpaceEntity
    {
        public byte BlockedTurns { set ; get; }
        public PiratePlanet(byte blockedTurns) : base("Planeta Piratów", false)
        {
            BlockedTurns = blockedTurns;
        }

        protected override string ToStringInternal()
        {
            return "Planeta Piratów";
        }
    }
}
