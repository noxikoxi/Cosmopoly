using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Game.utils;

namespace Game.ViewModels
{
    public class CardVM : INotifyPropertyChanged
    {
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

        public Action<object> OnAccept { get; set; }
        public Action<object> OnDecline { get; set; }

        public Predicate<object> CanAccept { get; set; }

        public CardVM()
        {
            Title = "Tytuł";
            Description = "Opis";
            AcceptCommand = new RelayCommand(ExecuteAccept, CanAcceptExecute);
            DeclineCommand = new RelayCommand(ExecuteDecline);
        }

        private void ExecuteAccept(object parameter)
        {
            OnAccept?.Invoke(parameter); // Wywołaj przekazaną akcję
        }

        private void ExecuteDecline(object parameter)
        {
            OnDecline?.Invoke(parameter); // Wywołaj przekazaną akcję
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanAcceptExecute(object parameter)
        {
            return CanAccept == null || CanAccept(parameter);
        }
    }
}
