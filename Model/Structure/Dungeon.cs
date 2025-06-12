using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Objects;
using ProceduralDungeonGenerator.Model.Styles;

namespace ProceduralDungeonGenerator.Model.Structure
{
    public class Dungeon
    {
        public List<Room> rooms { get; private set; }
        public List<Corridor> corridors { get; private set; }
        private List<EnemyType> weightedEnemyTypes = new();
        private List<ArtifactName> weightedArtifactNames = new();
        private IDungeonStyle _style;

        private Random rand = new Random();

        public Dungeon(IDungeonStyle dungeonStyle)
        {
            _style = dungeonStyle;
            rooms = new List<Room>();
            corridors = new List<Corridor>();
        }

        // Draw dungeon
        public void Draw(Graphics g)
        {
            foreach (var corridor in corridors)
            {
                corridor.Draw(g, _style);
            }
            foreach (var room in rooms)
            {
                room.Draw(g, _style);
            }
        }

        public void GenerateDungeon()
        {
            Logger.Log($"Using dungeon style: {_style.Name}");
            InitializeWeightedEnemyList();
            InitializeWeightedArtifactList();

            GenerateRooms();
            _style.ArrangeRooms(rooms);

            GenerateCorridors();

            foreach (var room in rooms)
            {
                Furnish(room);
                Logger.Log($"Furnished: {room}");
            }

            GenerateCorridorsPaths();
        }

        private void GenerateCorridorsPaths()
        {
            var blockedTiles = new HashSet<Point>();

            foreach (var corridor in corridors)
            {
                foreach (var room in rooms)
                {
                    foreach (var tile in room.RoomInteriorTiles)
                        blockedTiles.Add(tile);
                    foreach (var tile in room.RoomBoundaryTiles)
                        blockedTiles.Remove(tile);
                }

                corridor.Path = _style.DetermineCorridorPath(corridor, blockedTiles);

                if (corridor.Path != null && corridor.Path.Count > 0)
                {
                    foreach (var tile in corridor.Path)
                        blockedTiles.Add(tile);
                }
                else
                {
                    Logger.Log($"Nie znaleziono ścieżki dla korytarza {corridor.StartRoom.ID} -> {corridor.EndRoom.ID}");
                }
            }

        }



        // returns neighboring tiles
        private IEnumerable<Point> GetNeighbors(Point p)
        {
            var deltas = new[]
            {
        new Point(1, 0), new Point(-1, 0),
        new Point(0, 1), new Point(0, -1)
    };

            foreach (var delta in deltas)
            {
                var np = new Point(p.X + delta.X, p.Y + delta.Y);

                if (np.X >= 0 && np.X < ConfigManager.gridWidth &&
                    np.Y >= 0 && np.Y < ConfigManager.gridHeight)
                {
                    yield return np;
                }
            }
        }


        // Generates list of room objects
        private void GenerateRooms()
        {
            rooms.Clear();

            foreach (var config in ConfigManager.RoomConfigs)
            {
                int roomCount = rand.Next(config.MinCount, config.MaxCount + 1);

                for (int i = 0; i < roomCount; i++)
                {
                    // Create the room, no collision/boundary logic here
                    var room = CreateRoom(config);

                    rooms.Add(room);
                    Logger.Log($"[Unplaced] Created room: {room}");
                }
            }
        }


        // Generates list of corridor objects
        private void GenerateCorridors()
        {
            var allCorridors = new List<Corridor>();

            // Pick rooms that are neither entrance nor exit
            var nonEndCapRooms = rooms
                .Where(r => r.Type != RoomType.Entrance && r.Type != RoomType.Exit)
                .ToList();

            // Create all possible corridors
            for (int i = 0; i < nonEndCapRooms.Count; i++)
            {
                for (int j = i + 1; j < nonEndCapRooms.Count; j++)
                {
                    allCorridors.Add(new Corridor(nonEndCapRooms[i], nonEndCapRooms[j]));
                }
            }

            // Shortest paths
            allCorridors.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            // Connect Endcap rooms
            ConnectEndCapRoomsFirst(allCorridors);

            // MinumumSpanningTree
            RunMST(allCorridors);
        }

        // Fill room with items
        private void Furnish(Room room)
        {
            if (room.Type == RoomType.Entrance || room.Type == RoomType.Exit)
            {
                // No items in exits and entrances
                return;
            }

            // pick only possible items
            var candidates = ConfigManager.ItemConfigs
                .Where(iConfig => iConfig.RoomType == RoomType.Any || iConfig.RoomType == room.Type)
                .ToList();

            // weighted list of items candidates for this room
            List<Item> weightedItemList = new List<Item>();

            foreach (var config in ConfigManager.ItemConfigs)
            {
                for (int i = 0; i < config.Weight; i++)
                {
                    Item newItem = config.Category switch
                    {
                        ItemCategory.Furniture => new Furniture(config.ItemID, config.Style, config.Name, config.RoomType, config.Placement, config.Weight),
                        ItemCategory.Architecture => new ArchitecturalItem(config.ItemID, config.Style, config.Name, config.RoomType, config.Placement, config.Weight),
                        ItemCategory.Decoration => new Decoration(config.ItemID, config.Style, config.Name, config.RoomType, config.Placement, config.Weight),
                        _ => throw new ArgumentOutOfRangeException($"Unknown category: {config.Category}")
                    };

                    weightedItemList.Add(newItem);
                }
            }

            // random count of items for this room
            int itemCount = Random.Shared.Next(2, 7);

            for (int i = 0; i < itemCount; i++)
            {
                int index = rand.Next(weightedItemList.Count);
                Item item = weightedItemList[index];
                room.AssignItem(item);
                room.AssignItemPosition(item);
                Logger.Log($"Room {room.ID} has item {item}");
            }
            
        }

        // Connects entrances and exits to nearest normal room
        private void ConnectEndCapRoomsFirst(List<Corridor> allCorridors)
        {
            var endCapRooms = rooms.Where(r => r.Type is RoomType.Entrance or RoomType.Exit).ToList();
            var normalRooms = rooms.Where(r => r.Type == RoomType.Normal).ToList();

            foreach (var endRoom in endCapRooms)
            {
                Room? closest = FindClosestRoom(endRoom, normalRooms);
                if (closest != null && !IsCorridorExists(endRoom, closest))
                {
                    var corridor = new Corridor(endRoom, closest);
                    corridors.Add(corridor);
                    // Remove the corridors for the MST algorithm
                    allCorridors.RemoveAll(c => c.Connects(endRoom, closest)); 
                    Logger.Log($"[EndCap] Room No. {endRoom.ID} ({endRoom.Type}) -> {closest.ID} ({closest.Type})");
                }
            }
        }

        // find the closest room to another room
        private Room? FindClosestRoom(Room from, IEnumerable<Room> candidates)
        {
            Room? closest = null;
            double minDistance = double.MaxValue;

            foreach (var room in candidates)
            {
                double distance = CalculateDistance(from, room);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = room;
                }
            }

            return closest;
        }

        // check if corridor exists already
        private bool IsCorridorExists(Room a, Room b)
        {
            return corridors.Any(c =>
                c.StartRoom == a && c.EndRoom == b ||
                c.StartRoom == b && c.EndRoom == a);
        }

        // Minimum Spanning Tree algorithm
        private void RunMST(List<Corridor> candidateCorridors)
        {
            var disjointSet = new DisjointSet();
            disjointSet.MakeSet(rooms);

            foreach (var corridor in corridors) // already existing
            {
                disjointSet.Union(corridor.StartRoom, corridor.EndRoom);
            }

            foreach (var corridor in candidateCorridors)
            {
                var a = corridor.StartRoom;
                var b = corridor.EndRoom;

                if (!disjointSet.Connected(a, b))
                {
                    disjointSet.Union(a, b);
                    corridors.Add(corridor);
                    Logger.Log($"[Inner] Room No. {a.ID} ({a.Type}) <--> {b.ID} ({b.Type})");
                }
            }
        }

        // Calculate distance between two rooms 
        private double CalculateDistance(Room roomA, Room roomB)
        {
            var centerA = roomA.Center();
            var centerB = roomB.Center();

            return Math.Sqrt(Math.Pow(centerB.X - centerA.X, 2) + Math.Pow(centerB.Y - centerA.Y, 2));
        }

        // Create room
        private Room CreateRoom(RoomConfig rConfig)
        {

            var room = new Room(rConfig.Size, rConfig.Type);

            // Random count of enemies for this room
            int enemyCount = rand.Next(rConfig.MinEnemies, rConfig.MaxEnemies + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                var enemyType = GetRandomEnemyType();
                var enemy = new Enemy(enemyType);
                room.AssignEnemy(enemy);
            }

            // Random count of artifacts for this room
            int artifactCount = rand.Next(rConfig.MinArtifacts, rConfig.MaxArtifacts + 1);
            for (int i = 0; i < artifactCount; i++)
            {
                var artifactName = GetRandomArtifactName();
                var artifact = new Artifact(artifactName);
                room.AssignArtifact(artifact);
            }

            return room;
        }

        // Choose random enemy type based on weighted list
        private EnemyType GetRandomEnemyType()
        {
            int index = rand.Next(weightedEnemyTypes.Count);
            return weightedEnemyTypes[index];
        }

        // Choose random artifact name based on weighted list
        private ArtifactName GetRandomArtifactName()
        {
            int index = rand.Next(weightedArtifactNames.Count);
            return weightedArtifactNames[index];
        }

        // Initialize weighted enemy list
        private void InitializeWeightedEnemyList()
        {
            weightedEnemyTypes = new List<EnemyType>();

            foreach (var config in ConfigManager.EnemyConfigs)
            {
                for (int i = 0; i < config.Weight; i++)
                {
                    weightedEnemyTypes.Add(config.Type);
                }
            }

            if (weightedEnemyTypes.Count == 0)
            {
                Logger.Log("[WARNING] weightedEnemyTypes list is empty – missing enemy configuration?");
            }
        }

        // Initialize weighted artifact list
        private void InitializeWeightedArtifactList()
        {
            weightedArtifactNames = new List<ArtifactName>();

            foreach (var config in ConfigManager.ArtifactConfigs)
            {
                for (int i = 0; i < config.Weight; i++)
                {
                    weightedArtifactNames.Add(config.Name);
                }
            }

            if (weightedArtifactNames.Count == 0)
            {
                Logger.Log("[WARNING] weightedArtifactNames list is empty – missing artifact configuration?");
            }
        }

        // For MST
        private class DisjointSet
        {
            private Dictionary<Room, Room> parent = new();

            public void MakeSet(IEnumerable<Room> rooms)
            {
                foreach (var room in rooms)
                    parent[room] = room;
            }

            public Room Find(Room r)
            {
                if (parent[r] != r)
                    parent[r] = Find(parent[r]);
                return parent[r];
            }

            public void Union(Room a, Room b)
            {
                var rootA = Find(a);
                var rootB = Find(b);
                if (rootA != rootB)
                    parent[rootB] = rootA;
            }

            public bool Connected(Room a, Room b)
            {
                return Find(a) == Find(b);
            }
        }

    }
}