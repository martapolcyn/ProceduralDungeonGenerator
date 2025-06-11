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

        public Brush GetCorridorBrush()
        {
            return Brushes.Cyan;
        }

        public Brush GetRoomBrush()
        {
            return Brushes.GreenYellow;
        }

        public RoomShape DetermineRoomShape(Room room)
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

        public List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked)
        {
            var path = new List<Point>();

            // Znajdź najbliższe brzegi obu pokoi
            var startTile = corridor.StartRoom.GetClosestBoundaryTileTo(corridor.EndRoom.Center());
            var endTile = corridor.EndRoom.GetClosestBoundaryTileTo(startTile);

            // Najpierw X, potem Y
            int x = startTile.X;
            int y = startTile.Y;

            path.Add(new Point(x, y));

            while (x != endTile.X)
            {
                x += x < endTile.X ? 1 : -1;
                path.Add(new Point(x, y));
            }

            while (y != endTile.Y)
            {
                y += y < endTile.Y ? 1 : -1;
                path.Add(new Point(x, y));
            }

            return path;
        }
    }
}
