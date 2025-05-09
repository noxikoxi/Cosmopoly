using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Game.utils;

namespace Game.ViewModels
{
    public class CardOption
    {
        public string Label { get; set; }
        public ICommand Command { get; set; }
    }

    public class CardVM : INotifyPropertyChanged
    {

        public ObservableCollection<CardOption> CardOptions { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public ICommand AcceptCommand { get; }
        public ICommand DeclineCommand { get; }

        public void AddOption(string label, Action<object> action, Predicate<object> canExecute)
        {
            CardOptions.Add(new CardOption
            {
                Label = label,
                Command = new RelayCommand(action, canExecute)
            });
        }

        public CardVM()
        {
            CardOptions = new();
            Title = "Tytuł";
            Description = "Opis";
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
