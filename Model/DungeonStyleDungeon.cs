using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleDungeon : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Dungeon";

        public Pen GetCorridorPen()
        {
            return new Pen(Color.DarkGray, 10);
        }

        public Brush GetRoomBrush()
        {
            return Brushes.SaddleBrown;
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
                < 5 => RoomShape.Rectangle,
                _ => RoomShape.LShape
            };
        }

        public List<Point> GetCorridorPath(Corridor corridor)
        {
            var start = corridor.StartRoom.Center();
            var end = corridor.EndRoom.Center();

            var path = new List<Point> { start };
            var random = new Random();

            int segments = random.Next(3, 6);
            Point current = start;

            for (int i = 0; i < segments - 1; i++)
            {
                int dx = end.X - current.X;
                int dy = end.Y - current.Y;
                bool horizontal = (i % 2 == 0);

                int stepX = dx / (segments - i);
                int stepY = dy / (segments - i);

                current = new Point(
                    current.X + (horizontal ? stepX : random.Next(-30, 30)),
                    current.Y + (!horizontal ? stepY : random.Next(-30, 30))
                );

                path.Add(current);
            }

            path.Add(end);
            return path;
        }
    }
}
