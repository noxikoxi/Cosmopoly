namespace CosmopolyEngine.models
{
    public class Player
    {
        private string _name;

        public long credits;

        public int BlockedTurns { get; set; }

        public byte SkippedTurns { get; private set; }

        public int position;

        public List<Card> cards;

        public bool IsBankrupt { get; set; }

        public Player(string name, long initialCredits=0)
        {
            _name = name;
            credits = initialCredits;
            cards = new List<Card>();
            IsBankrupt = false;
            SkippedTurns = 0;
        }

        public void SkipTurn()
        {
            SkippedTurns += 1;
        }

        public string GetName() 
        { 
            return _name;
        }

    }
}
