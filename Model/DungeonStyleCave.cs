using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    internal class DungeonStyleCave : IDungeonStyle
    {
        public string Name => "Cave";

        public List<Point> GetCorridorPath(Corridor corridor)
        {
            // TODO: implement cave corridors
            var points = new List<Point>
            {
                new Point(corridor.Start.X, corridor.Start.Y),
                new Point(corridor.End.X, corridor.End.Y)
            };
            return points;
        }

        public Pen GetCorridorPen()
        {
            return new Pen(Color.SaddleBrown, 6) { DashStyle = System.Drawing.Drawing2D.DashStyle.Solid };
        }

        public Brush GetRoomBrush()
        {
            return new SolidBrush(Color.FromArgb(120, 72, 48));
        }

        public RoomShape GetRoomShape(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
