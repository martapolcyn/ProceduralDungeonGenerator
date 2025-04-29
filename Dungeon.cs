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
        private Random rand = new Random();

        public Dungeon()
        {
            rooms = new List<Room>();
        }


        public void GenerateDungeon()
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



        private Room CreateRoom(RoomConfig config)
        {
            // Position the room randomly
            int x = rand.Next(0, Config.dungeonWidth);
            int y = rand.Next(0, Config.dungeonHeight);

            // Create the room based on configuration
            return new Room(x, y, config.Size, config.Shape, config.Type);
        }

        // Draw room
        public void Draw(Graphics g)
        {
            foreach (var room in rooms)
            {
                room.Draw(g);
            }
        }
    }
}