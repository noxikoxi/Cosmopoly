using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.models
{
    internal class Singularity : SpaceEntity
    {
        private List<Card> possibleCards;

        public Singularity() : base("Osobliwość", false)
        {
            possibleCards = [];
        }

        public void SetPossibleCards(List<Card> possibleCards)
        {
            this.possibleCards = possibleCards;
        }

        public Card GetRandomCard(List<PlanetarySystem> playerSystems, Random random)
        {
            var hasGalacticShipyard = playerSystems.Count > 0 ? true : false;
            // Jeżeli gracz ma jakikolwiek port to można go uszkodzić, ta karta zawsze będzie na końcu
            var randomLength = hasGalacticShipyard ? possibleCards.Count : possibleCards.Count - 1;

            return possibleCards[random.Next(randomLength)];

        }

        protected override string ToStringInternal()
        {
            return "Osobliwość";
        }
    }
}
