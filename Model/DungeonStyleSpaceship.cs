using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleSpaceship : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Spaceship";

        public Pen GetCorridorPen()
        {
            return new Pen(Color.Cyan, 20);
        }

        public Brush GetRoomBrush()
        {
            return Brushes.GreenYellow;
        }

        public RoomShape GetRoomShape(Room room)
        {
            if (room.Type == RoomType.Entrance || room.Type == RoomType.Exit)
            {
                return RoomShape.Square;
            }

            int roll = random.Next(0, 10);
            return roll switch
            {
                < 5 => RoomShape.Circle,
                _ => RoomShape.Square
            };
        }

        public List<Point> GetCorridorPath(Corridor corridor)
        {
            var start = corridor.StartRoom.Center();
            var end = corridor.EndRoom.Center();
            var mid = new Point(end.X, start.Y);

            return new List<Point> { start, mid, end };
        }
    }
}
