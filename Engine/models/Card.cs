using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Engine.strategies;

namespace Engine.models
{
    public class Card
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("strategies")]
        public CardStrategy[] strategies { get; set; }

        [JsonPropertyName("applyTogether")]
        public bool ApplyTogether { get; set; }

        public Card(string title, string description, CardStrategy[] strategies, bool applyTogether = false)
        {
            Title = title;
            Description = description;
            this.strategies = strategies;
            this.ApplyTogether = applyTogether;
        }

        public override string ToString()
        {
            return $"Tytuł: {Title}\n\tOpis: {Description}";
        }
    }
}
