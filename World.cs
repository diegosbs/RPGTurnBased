using Microsoft.Xna.Framework;
using RPGTurnBased.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGTurnBased.World
{
    public enum EnvironmentType
    {
        Village,    // Vila - seguro, com NPCs e quests
        Forest,     // Floresta - goblins, animais
        Cave,       // Caverna - esqueletos, morcegos
        Mountains,  // Montanhas - orcs, dragões
        Swamp,      // Pântano - slimes, venenos
        Plains,     // Planícies - lobos, bandidos
        Desert,     // Deserto - serpentes, escorpiões
        Ruins,      // Ruínas antigas - mortos-vivos
        Coast,      // Costa - caranguejos, piratas
        Volcano     // Vulcão - elementais, dragões
    }

    // Classe para passar dados entre estados
    public class CombatData
    {
        public EnvironmentType Environment { get; set; }
        public int PlayerLevel { get; set; }
        public Player Player { get; set; }
    }

    public enum LocationType
    {
        Safe,       // Local seguro (vila, taverna)
        Combat,     // Local de combate
        Quest,      // Local com quest
        Treasure,   // Baú de tesouro
        Shop,       // Loja
        Exit        // Saída para outro mapa
    }

    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public LocationType Type { get; set; }
        public bool IsVisited { get; set; }
        public bool HasCombat { get; set; }
        public List<string> AvailableActions { get; set; }

        public Location(string name, string description, LocationType type)
        {
            Name = name;
            Description = description;
            Type = type;
            IsVisited = false;
            HasCombat = type == LocationType.Combat;
            AvailableActions = new List<string>();

            GenerateActions();
        }

        private void GenerateActions()
        {
            AvailableActions.Clear();

            switch (Type)
            {
                case LocationType.Safe:
                    AvailableActions.Add("Rest");
                    AvailableActions.Add("Look around");
                    break;
                case LocationType.Combat:
                    AvailableActions.Add("Enter cautiously");
                    AvailableActions.Add("Charge in");
                    break;
                case LocationType.Quest:
                    AvailableActions.Add("Investigate");
                    AvailableActions.Add("Talk to NPC");
                    break;
                case LocationType.Treasure:
                    AvailableActions.Add("Search for treasure");
                    AvailableActions.Add("Examine carefully");
                    break;
                case LocationType.Shop:
                    AvailableActions.Add("Browse items");
                    AvailableActions.Add("Talk to merchant");
                    break;
                case LocationType.Exit:
                    AvailableActions.Add("Leave area");
                    AvailableActions.Add("Explore more");
                    break;
            }
        }
    }

    public class GameMap
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public EnvironmentType Environment { get; set; }
        public int RecommendedLevel { get; set; }
        public List<Location> Locations { get; set; }
        public int CurrentLocationIndex { get; set; }
        public bool IsDiscovered { get; set; }

        public Location CurrentLocation => Locations[CurrentLocationIndex];

        public GameMap(string id, string name, EnvironmentType environment, int recommendedLevel)
        {
            Id = id;
            Name = name;
            Environment = environment;
            RecommendedLevel = recommendedLevel;
            Locations = new List<Location>();
            CurrentLocationIndex = 0;
            IsDiscovered = false;

            GenerateLocations();
        }

        private void GenerateLocations()
        {
            var random = new Random();

            // Gerar locais baseados no tipo de ambiente
            switch (Environment)
            {
                case EnvironmentType.Village:
                    GenerateVillageLocations();
                    break;
                case EnvironmentType.Forest:
                    GenerateForestLocations();
                    break;
                case EnvironmentType.Cave:
                    GenerateCaveLocations();
                    break;
                case EnvironmentType.Mountains:
                    GenerateMountainLocations();
                    break;
                case EnvironmentType.Swamp:
                    GenerateSwampLocations();
                    break;
                case EnvironmentType.Plains:
                    GeneratePlainsLocations();
                    break;
                case EnvironmentType.Desert:
                    GenerateDesertLocations();
                    break;
                case EnvironmentType.Ruins:
                    GenerateRuinsLocations();
                    break;
                case EnvironmentType.Coast:
                    GenerateCoastLocations();
                    break;
                case EnvironmentType.Volcano:
                    GenerateVolcanoLocations();
                    break;
            }

            // Sempre adicionar uma saída no final
            Locations.Add(new Location("Exit", "Path leading to other areas", LocationType.Exit));
        }

        private void GenerateVillageLocations()
        {
            Locations.Add(new Location("Village Center", "A peaceful town square with a well", LocationType.Safe));
            Locations.Add(new Location("Tavern", "The local inn, warm and welcoming", LocationType.Safe));
            Locations.Add(new Location("Blacksmith", "Tools and weapons for sale", LocationType.Shop));
            Locations.Add(new Location("Village Elder", "An old man with a quest", LocationType.Quest));
        }

        private void GenerateForestLocations()
        {
            Locations.Add(new Location("Forest Entrance", "Dense trees block much sunlight", LocationType.Safe));
            Locations.Add(new Location("Goblin Camp", "Signs of recent goblin activity", LocationType.Combat));
            Locations.Add(new Location("Ancient Tree", "A massive tree with something hidden", LocationType.Treasure));
            Locations.Add(new Location("Forest Clearing", "Open area with wildflowers", LocationType.Safe));
        }

        private void GenerateCaveLocations()
        {
            Locations.Add(new Location("Cave Entrance", "Dark opening in the rockface", LocationType.Safe));
            Locations.Add(new Location("Bone Chamber", "Old bones scattered everywhere", LocationType.Combat));
            Locations.Add(new Location("Underground Pool", "Crystal clear water", LocationType.Safe));
            Locations.Add(new Location("Deep Cavern", "Echoes suggest danger ahead", LocationType.Combat));
        }

        private void GenerateMountainLocations()
        {
            Locations.Add(new Location("Mountain Path", "Rocky trail leading upward", LocationType.Safe));
            Locations.Add(new Location("Orc Stronghold", "Fortified position on a cliff", LocationType.Combat));
            Locations.Add(new Location("Mountain Peak", "Breathtaking view from the top", LocationType.Treasure));
        }

        private void GenerateSwampLocations()
        {
            Locations.Add(new Location("Swamp Edge", "Murky water begins here", LocationType.Safe));
            Locations.Add(new Location("Poisonous Bog", "Dangerous creatures lurk here", LocationType.Combat));
            Locations.Add(new Location("Witch's Hut", "Abandoned hut on stilts", LocationType.Quest));
        }

        private void GeneratePlainsLocations()
        {
            Locations.Add(new Location("Open Field", "Wide grasslands stretch ahead", LocationType.Safe));
            Locations.Add(new Location("Bandit Camp", "Smoke rises from a hidden camp", LocationType.Combat));
            Locations.Add(new Location("Abandoned Wagon", "Merchant wagon left behind", LocationType.Treasure));
        }

        private void GenerateDesertLocations()
        {
            Locations.Add(new Location("Desert Oasis", "Refreshing water in the sand", LocationType.Safe));
            Locations.Add(new Location("Sandstorm Area", "Visibility is nearly zero", LocationType.Combat));
            Locations.Add(new Location("Buried Ruins", "Ancient structure half-buried", LocationType.Treasure));
        }

        private void GenerateRuinsLocations()
        {
            Locations.Add(new Location("Ruined Gate", "Crumbling entrance to ancient city", LocationType.Safe));
            Locations.Add(new Location("Undead Halls", "Skeletal warriors patrol here", LocationType.Combat));
            Locations.Add(new Location("Treasure Vault", "Sealed chamber with riches", LocationType.Treasure));
            Locations.Add(new Location("Throne Room", "Ancient seat of power", LocationType.Combat));
        }

        private void GenerateCoastLocations()
        {
            Locations.Add(new Location("Sandy Beach", "Waves crash gently on shore", LocationType.Safe));
            Locations.Add(new Location("Pirate Cove", "Hidden base of sea thieves", LocationType.Combat));
            Locations.Add(new Location("Tide Pools", "Interesting creatures in the water", LocationType.Treasure));
        }

        private void GenerateVolcanoLocations()
        {
            Locations.Add(new Location("Volcanic Slope", "Heat rises from the ground", LocationType.Safe));
            Locations.Add(new Location("Lava Chamber", "Molten rock and fire elementals", LocationType.Combat));
            Locations.Add(new Location("Dragon's Lair", "The most dangerous place", LocationType.Combat));
        }

        public bool CanMoveToNext()
        {
            return CurrentLocationIndex < Locations.Count - 1;
        }

        public void MoveToNext()
        {
            if (CanMoveToNext())
            {
                CurrentLocationIndex++;
                CurrentLocation.IsVisited = true;
            }
        }

        public bool CanMoveToPrevious()
        {
            return CurrentLocationIndex > 0;
        }

        public void MoveToPrevious()
        {
            if (CanMoveToPrevious())
            {
                CurrentLocationIndex--;
            }
        }
    }

    public class WorldManager
    {
        private Dictionary<string, GameMap> _mapCache;
        private GameMap _currentMap;
        private Random _random;

        public GameMap CurrentMap => _currentMap;
        public Location CurrentLocation => _currentMap?.CurrentLocation;

        public WorldManager()
        {
            _mapCache = new Dictionary<string, GameMap>();
            _random = new Random();

            // Começar na vila inicial
            _currentMap = CreateStartingVillage();
            _mapCache[_currentMap.Id] = _currentMap;
        }

        private GameMap CreateStartingVillage()
        {
            return new GameMap("village_start", "Peaceful Village", EnvironmentType.Village, 1);
        }

        public List<EnvironmentType> GetAvailableDestinations(int playerLevel)
        {
            var available = new List<EnvironmentType>();

            foreach (EnvironmentType env in Enum.GetValues<EnvironmentType>())
            {
                if (env != _currentMap.Environment && IsEnvironmentUnlocked(env, playerLevel))
                {
                    available.Add(env);
                }
            }

            return available;
        }

        private bool IsEnvironmentUnlocked(EnvironmentType environment, int playerLevel)
        {
            return environment switch
            {
                EnvironmentType.Village => true,
                EnvironmentType.Forest => playerLevel >= 1,
                EnvironmentType.Plains => playerLevel >= 1,
                EnvironmentType.Coast => playerLevel >= 2,
                EnvironmentType.Cave => playerLevel >= 3,
                EnvironmentType.Swamp => playerLevel >= 4,
                EnvironmentType.Desert => playerLevel >= 4,
                EnvironmentType.Mountains => playerLevel >= 5,
                EnvironmentType.Ruins => playerLevel >= 7,
                EnvironmentType.Volcano => playerLevel >= 9,
                _ => false
            };
        }

        public void TravelToEnvironment(EnvironmentType environment, int playerLevel)
        {
            string mapId = GenerateMapId(environment);

            // Verificar se já visitamos este mapa
            if (_mapCache.ContainsKey(mapId))
            {
                _currentMap = _mapCache[mapId];
            }
            else
            {
                // Criar novo mapa
                _currentMap = GenerateNewMap(environment, playerLevel);
                _mapCache[mapId] = _currentMap;
            }

            _currentMap.IsDiscovered = true;
        }

        private string GenerateMapId(EnvironmentType environment)
        {
            // Por enquanto, um mapa por tipo de ambiente
            // Futuramente pode ter múltiplos mapas do mesmo tipo
            return $"{environment.ToString().ToLower()}_001";
        }

        private GameMap GenerateNewMap(EnvironmentType environment, int playerLevel)
        {
            string name = GetEnvironmentName(environment);
            return new GameMap(GenerateMapId(environment), name, environment, playerLevel);
        }

        private string GetEnvironmentName(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => "Peaceful Village",
                EnvironmentType.Forest => "Dark Forest",
                EnvironmentType.Cave => "Mysterious Cave",
                EnvironmentType.Mountains => "Rocky Mountains",
                EnvironmentType.Swamp => "Treacherous Swamp",
                EnvironmentType.Plains => "Open Plains",
                EnvironmentType.Desert => "Scorching Desert",
                EnvironmentType.Ruins => "Ancient Ruins",
                EnvironmentType.Coast => "Windswept Coast",
                EnvironmentType.Volcano => "Active Volcano",
                _ => "Unknown Area"
            };
        }

        public void MoveToNextLocation()
        {
            _currentMap?.MoveToNext();
        }

        public void MoveToPreviousLocation()
        {
            _currentMap?.MoveToPrevious();
        }

        public bool HasCombatAtCurrentLocation()
        {
            return CurrentLocation?.HasCombat ?? false;
        }

        public List<string> GetVisitedMapNames()
        {
            return _mapCache.Values.Where(m => m.IsDiscovered).Select(m => m.Name).ToList();
        }

        public Color GetEnvironmentColor(EnvironmentType environment)
        {
            return environment switch
            {
                EnvironmentType.Village => new Color(139, 120, 93),    // Marrom claro
                EnvironmentType.Forest => new Color(34, 85, 34),       // Verde escuro
                EnvironmentType.Cave => new Color(64, 64, 64),         // Cinza escuro
                EnvironmentType.Mountains => new Color(105, 105, 105), // Cinza
                EnvironmentType.Swamp => new Color(85, 107, 47),       // Verde oliva
                EnvironmentType.Plains => new Color(154, 205, 50),     // Verde lima
                EnvironmentType.Desert => new Color(238, 203, 173),    // Bege
                EnvironmentType.Ruins => new Color(112, 128, 144),     // Cinza aço
                EnvironmentType.Coast => new Color(70, 130, 180),      // Azul aço
                EnvironmentType.Volcano => new Color(178, 34, 34),     // Vermelho escuro
                _ => Color.Gray
            };
        }
    }
}