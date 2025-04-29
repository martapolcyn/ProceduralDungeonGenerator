using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Drawing;
using ProceduralDungeonGenerator.Configuration;

namespace ProceduralDungeonGenerator
{
    public class Dungeon
    {
        public List<Room> rooms { get; private set; }
        public List<Corridor> corridors { get; private set; }
        private List<EnemyType> weightedEnemyTypes = new();


        private Random rand = new Random();

        public Dungeon()
        {
            rooms = new List<Room>();
            corridors = new List<Corridor>();
        }

        public void GenerateDungeon()
        {
            InitializeWeightedEnemyList();
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
                            System.Diagnostics.Debug.WriteLine($"[WARNING] Created despite collision: {newRoom}");
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
                            System.Diagnostics.Debug.WriteLine($"Created: {newRoom}");
                            break;
                        }
                    }
                }
            }
        }

        private void GenerateCorridors()
        {
            corridors.Clear();

            // Visited rooms list
            var visitedRooms = new HashSet<int>();

            // First room
            visitedRooms.Add(0);

            // While there are unconnected rooms
            while (visitedRooms.Count < rooms.Count)
            {
                double minDistance = double.MaxValue;
                int roomAIndex = -1, roomBIndex = -1;

                // Search for two nearest unconnected rooms
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (!visitedRooms.Contains(i))
                        continue;

                    for (int j = 0; j < rooms.Count; j++)
                    {
                        if (i == j || visitedRooms.Contains(j))
                            continue;

                        var roomA = rooms[i];
                        var roomB = rooms[j];

                        // Calculate distance between rooms
                        var distance = CalculateDistance(roomA, roomB);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            roomAIndex = i;
                            roomBIndex = j;
                        }
                    }
                }

                // Create corridor
                var a = rooms[roomAIndex];
                var b = rooms[roomBIndex];

                var start = a.Center();
                var end = b.Center();

                var mid = new Point(end.X, start.Y);

                corridors.Add(new Corridor(start, mid));
                corridors.Add(new Corridor(mid, end));

                System.Diagnostics.Debug.WriteLine($"Created corridor: {start} -> {mid} -> {end}");

                // Mark rooms as connected
                visitedRooms.Add(roomBIndex);
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

            return room;
        }

        // Choose random enemy type based on weighted list
        private EnemyType GetRandomEnemyType()
        {
            int index = rand.Next(weightedEnemyTypes.Count);
            return weightedEnemyTypes[index];
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
                System.Diagnostics.Debug.WriteLine("[WARNING] Lista weightedEnemyTypes jest pusta – brak dostępnych konfiguracji wrogów?");
            }
        }

        // Draw dungeon
        public void Draw(Graphics g)
        {
            foreach (var room in rooms)
            {
                room.Draw(g);
            }

            foreach (var corridor in corridors)
            {
                corridor.Draw(g);
            }
        }
    }
}