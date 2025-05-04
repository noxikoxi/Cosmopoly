using Engine.managers;
using Engine.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Engine.utils
{
    internal class CardsConfig
    {
        [JsonPropertyName("cards")]
        public List<Card> Cards { get; set; } = [];
    }

    internal enum EntityType
    {
        Station,
        Pirates,
        Planet,
        Singularity
    }

    internal class EntityConfig
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required EntityType Type { get; set; }

        [JsonPropertyName("system")]
        public string? System { get; set; }
    }

    internal class EntitiesConfig
    {
        [JsonPropertyName("entities")]
        public List<EntityConfig> Entities { get; set; } = [];
    }

    internal class FinanceConfig
    {
        [JsonPropertyName("finances")]
        public required FinanceManager Finances { get; set; }
    }



    internal static class ConfigLoader
    {
        const byte PIRATES_BLOCKED_TURNS = 2;

        public static List<Card> LoadCardConfig(string filepath)
        {
            try
            {
                string jsonString = File.ReadAllText(filepath);
                
                if (jsonString != null)
                {
                    CardsConfig? config = JsonSerializer.Deserialize<CardsConfig>(jsonString);

                    if (config != null)
                    {
                        return config.Cards;
                    }
                    else
                    {
                        Console.WriteLine("Deserializacja configu nie powiodła się");
                        return new List<Card>();

                    }
                }

                Console.WriteLine("Config jest pusty");
                return new List<Card>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania konfiguracji: {ex.Message}");
                return new List<Card>();
            }

        }

        public static FinanceManager? LoadFinanceManagerConfig(string filepth)
        {
            try
            {
                string jsonString = File.ReadAllText(filepth);
                if (jsonString != null)
                {
                    FinanceConfig? config = JsonSerializer.Deserialize<FinanceConfig>(jsonString);
                    if (config != null)
                    {
                        return config.Finances;
                    }
                    else
                    {
                        Console.WriteLine("Deserializacja configu nie powiodła się");
                        return null;
                    }
                }
                Console.WriteLine("Config jest pusty");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania konfiguracji: {ex.Message}");
                return null;
            }
        }

        public static (List<SpaceEntity>, List<PlanetarySystem>) LoadGalaxyConfig(string filepath)
        {
            try
            {
                string jsonString = File.ReadAllText(filepath);
                List<SpaceEntity> entities = new();
                List<PlanetarySystem> systems = new();

                if (jsonString != null)
                {
                    EntitiesConfig? config = JsonSerializer.Deserialize<EntitiesConfig>(jsonString);
                    if (config != null)
                    {
                        HashSet<string> systemsNames = new();
                        int curr_system_id = -1;

                        for(byte i = 0; i < config.Entities.Count; ++i)
                        {
                            
                            var entity = config.Entities[i];
                            
                            if (entity.Type==EntityType.Station)
                            {
                                entities.Add(new Station(entity.Name));
                            }else if (entity.Type == EntityType.Pirates)
                            {
                                entities.Add(new PiratePlanet(PIRATES_BLOCKED_TURNS));
                            }
                            else if (entity.Type == EntityType.Singularity)
                            {
                                entities.Add(new Singularity());
                            } else
                            {
                                if (!systemsNames.Contains(entity.System))
                                {
                                    ++curr_system_id;
                                    systemsNames.Add(entity.System);
                                    systems.Add(new PlanetarySystem(entity.System));
                                }

                                systems[curr_system_id].AddPlanet(i);
                                entities.Add(new HabitablePlanet(entity.Name));
                            }
                        }


                        return (entities, systems);
                    }
                    Console.WriteLine("Config jest pusty");
                    return (entities, systems);
                }
                else
                {
                    Console.WriteLine("Deserializacja configu nie powiodła się");
                    return (entities, systems);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wczytywania konfiguracji: {ex.Message}");
                return (new List<SpaceEntity>(), new List<PlanetarySystem>());
            }

        }
    }
}
