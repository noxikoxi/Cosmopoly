using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;

namespace Engine.models
{
    public class Player(string name, long initialCredits = 0) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; private set; } = name;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private long _credits = initialCredits;
        public long Credits
        {
            get => _credits;
            set
            {
                if (_credits != value)
                {
                    _credits = value;
                    OnPropertyChanged(nameof(Credits));
                }
            }
        }

        public int BlockedTurns { get; set; }

        public byte SkippedTurns { get; private set; } = 0;

        public int position;

        private int _shield_cards = 0;
        public int ShieldCards
        {
            get => _shield_cards;
            set
            {
                if (_shield_cards != value)
                {
                    _shield_cards = value;
                    OnPropertyChanged(nameof(ShieldCards));
                }
            }
        }

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
