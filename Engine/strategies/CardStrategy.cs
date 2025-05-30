﻿using System.Text.Json.Serialization;
using Engine.managers;
using Engine.models;

namespace Engine.strategies
{
    public enum strategyType
    {
        GiveCredits,
        TakeCredits,
        Move,
        BlockTurn,
        Shield,
        Destroy,
        UseShield,
        Cancel
    }

    public class CardStrategy
    {

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public strategyType Type { get; set; }

        [JsonPropertyName("value")]
        public double Value { get; set; }

        public CardStrategy() { }

        public CardStrategy(string type, double value = 0.0)
        {
            if (value < 0.0)
            {
                throw new ArgumentException("Value cannot be negative");
            }

            this.Value = value;

            this.Type = type switch
            {
                "GiveCredits" => strategyType.GiveCredits,
                "TakeCredits" => strategyType.TakeCredits,
                "Move" => strategyType.Move,
                "UseShield" => strategyType.UseShield,
                "BlockTurn" => strategyType.BlockTurn,
                "Shield" => strategyType.Shield,
                "Destroy" => strategyType.Destroy,
                _ => throw new ArgumentException("Invalid card type"),
            };
        }

        public override string ToString()
        {
            return $"Typ Strategii: {this.Type}; Wartość: {this.Value}";
        }

        public void Apply(Game game)
        {
            switch (this.Type)
            {
                case strategyType.GiveCredits:
                    game.AddCredits(game.CurrentPlayer, (long)this.Value);
                    break;
                case strategyType.TakeCredits:
                    Player pl = game.CurrentPlayer;
                    if (this.Value < 1)
                    {
                        game.RemoveCredits(pl, game.GetCurrentPlayerPropertyTax(this.Value));
                    }
                    else
                    {
                        game.RemoveCredits(pl, (long)this.Value);
                    }
                    break;
                case strategyType.BlockTurn:
                    game.BlockPlayer((int)this.Value);
                    break;
                case strategyType.UseShield:
                    game.CurrentPlayer.ShieldCards -= 1;
                    break;
                case strategyType.Cancel:
                    break;
                default:
                    break;
            }
        }
    }
}
