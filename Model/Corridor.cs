using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class Corridor
    {
        public Room StartRoom { get; }
        public Room EndRoom { get; }
        public double Distance { get; }
        private List<Point>? _path; 

        public Corridor(Room startRoom, Room endRoom)
        {
            StartRoom = startRoom;
            EndRoom = endRoom;
            Distance = CalculateDistance(startRoom, endRoom);
        }

        private double CalculateDistance(Room a, Room b)
        {
            int dx = a.Center().X - b.Center().X;
            int dy = a.Center().Y - b.Center().Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void Draw(Graphics g, IDungeonStyle style)
        {
            var pen = style.GetCorridorPen();

            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            pen.LineJoin = LineJoin.Round;

            if (_path == null)
            {
                _path = style.GetCorridorPath(this);
            }

            for (int i = 0; i < _path.Count - 1; i++)
            {
                g.DrawLine(pen, _path[i], _path[i + 1]);
            }
        }

        public Point Start => StartRoom.Center();
        public Point End => EndRoom.Center();

        public bool Connects(Room a, Room b)
        {
            return (StartRoom == a && EndRoom == b) || (StartRoom == b && EndRoom == a);
        }

        public override string ToString()
        {
            return $"Corridor: Start Room No. {StartRoom.ID}, End Room No. {EndRoom.ID}";
        }
    }

}
