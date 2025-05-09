using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.strategies;

namespace Engine.utils
{
    public static class StrategyNameConverter
    {
        public static string ConvertName(strategyType type)
        {
            switch (type)
            {
                case strategyType.Move:
                    return "Zmień pozycję";
                case strategyType.GiveCredits:
                    return "Otrzymaj kredyty";
                case strategyType.TakeCredits:
                    return "Zapłać";
                case strategyType.BlockTurn:
                    return "Blokada tur";
                case strategyType.Shield:
                    return "Akceptuj";
                case strategyType.Destroy:
                    return "Zniszcz";
                case strategyType.UseShield:
                    return "Użyj karty ochrony";
                case strategyType.Cancel:
                    return "Anuluj";
                default:
                    throw new ArgumentException("Invalid strategy type");
            }
        }
    }
}
