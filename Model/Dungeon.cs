using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Drawing;
using ProceduralDungeonGenerator.Configuration;

namespace ProceduralDungeonGenerator.Model
{
    public class Dungeon
    {
        public List<Room> rooms { get; private set; }
        public List<Corridor> corridors { get; private set; }
        private List<EnemyType> weightedEnemyTypes = new();
        private List<ArtifactName> weightedArtifactNames = new();
        private IDungeonStyle style;

        private Random rand = new Random();

        public Dungeon(IDungeonStyle dungeonStyle)
        {
            style = dungeonStyle;
            rooms = new List<Room>();
            corridors = new List<Corridor>();
        }

        public void GenerateDungeon()
        {
            Logger.Log($"Using dungeon style: {style.GetName()}");
            InitializeWeightedEnemyList();
            InitializeWeightedArtifactList();
            GenerateRooms();
            GenerateCorridors();
        }

        private void GenerateRooms()
        {
            rooms.Clear();

            // for each type of the room
            foreach (var config in Config.RoomConfigs)
            {
                // pick random count of rooms within min and max and create them
                int roomCount = rand.Next(config.MinCount, config.MaxCount + 1);

                for (int i = 0; i < roomCount; i++)
                {
                    int attempts = 0;
                    const int maxAttempts = 100;

                    while (attempts < maxAttempts)
                    {
                        var newRoom = CreateRoom(config);
                        attempts++;

                        // Add the room no matter what if this is the very last attempt
                        if (attempts == maxAttempts)
                        {
                            rooms.Add(newRoom);
                            Logger.Log($"[WARNING] Created despite collision: {newRoom}");
                            break;
                        }

                        // Check if the room fits the dungeon
                        if (!newRoom.IsWithinBounds())
                            continue;

                        // Check if the room does not collide with other rooms
                        bool collision = rooms.Any(existing => newRoom.Intersects(existing));
                        if (!collision)
                        {
                            rooms.Add(newRoom);
                            Logger.Log($"Created: {newRoom}");
                            break;
                        }
                    }
                }
            }
        }

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
                (c.StartRoom == a && c.EndRoom == b) ||
                (c.StartRoom == b && c.EndRoom == a));
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
            // Position the room randomly
            int x = rand.Next(0, Config.dungeonWidth);
            int y = rand.Next(0, Config.dungeonHeight);

            var room = new Room(x, y, rConfig.Size, rConfig.Shape, rConfig.Type);

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

            foreach (var config in Config.EnemyConfigs)
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

            foreach (var config in Config.ArtifactConfigs)
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

        // Draw dungeon
        public void Draw(Graphics g)
        {
            foreach (var corridor in corridors)
            {
                corridor.Draw(g);
            }
            foreach (var room in rooms)
            {
                room.Draw(g);
            }
        }

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