using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator
{
    internal class Dungeon
    {
        public List<Room> Rooms { get; set; }

        public Dungeon()
        {
            Rooms = new List<Room>();
        }

        public void GenerateDungeon(int numberOfRooms)
        {
            Random rand = new Random();
            for (int i = 0; i < numberOfRooms; i++)
            {
                int width = rand.Next(50, 100);
                int height = rand.Next(50, 100);
                int x = rand.Next(0, 800 - width);
                int y = rand.Next(0, 600 - height);

                Rooms.Add(new Room(x, y, width, height));
            }
        }
    }
}
