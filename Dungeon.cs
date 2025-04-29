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

            foreach (var config in Config.RoomConfigs)
            {
                int roomCount = rand.Next(config.MinCount, config.MaxCount + 1);

                for (int i = 0; i < roomCount; i++)
                {
                    Room newRoom;
                    bool collision;

                    do
                    {
                        collision = false;
                        newRoom = CreateRoom(config);

                        foreach (var existingRoom in rooms)
                        {
                            if (newRoom.Intersects(existingRoom))
                            {
                                collision = true;
                                break;
                            }
                        }
                    }
                    while (collision);

                    rooms.Add(newRoom);
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