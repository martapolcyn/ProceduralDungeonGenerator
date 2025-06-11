using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Styles
{
    internal class DungeonStyleCave : IDungeonStyle
    {
        public string Name => "Cave";

        public List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked)
        {
            // TODO: implement cave corridors
            var points = new List<Point>
            {
                new Point(corridor.StartRoom.X, corridor.StartRoom.Y),
                new Point(corridor.EndRoom.X, corridor.EndRoom.Y)
            };
            return points;
        }

        public Brush GetCorridorBrush()
        {
            return new SolidBrush(Color.FromArgb(120, 72, 48));
        }

        public Brush GetRoomBrush()
        {
            return new SolidBrush(Color.FromArgb(120, 72, 48));
        }

        public RoomShape DetermineRoomShape(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
