using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.models
{
    public abstract  class Card
    {
        public string Description { get; set; }

        public string Title { get; set; }

        // Być może grafika w przyszłości

        public Card(string title, string description) 
        {
            Title = title;
            Description = description;
        }

        public abstract void Apply(Game game);

        public override string ToString()
        {
            return $"Tytuł: {Title}\n\tOpis: {Description}";
        }
    }
}
