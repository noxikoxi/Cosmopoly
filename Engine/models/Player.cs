namespace Engine.models
{
    public class Player(string name, long initialCredits = 0)
    {
        public string Name { get; private set; } = name;

        public long credits = initialCredits;

        public int BlockedTurns { get; set; }

        public byte SkippedTurns { get; private set; } = 0;

        public int position;

        public List<Card> cards = new List<Card>();

        public bool IsBankrupt { get; set; } = false;

        public void SkipTurn()
        {
            SkippedTurns += 1;
        }

        public void ResetSkippedTurns()
        {
            SkippedTurns = 0;
        }   

    }
}
