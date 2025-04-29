using System;
using System.Collections.Generic;
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

        public Point Start => StartRoom.Center();
        public Point End => EndRoom.Center();

        public void Draw(Graphics g)
        {
            var start = Start;
            var end = End;
            var mid = new Point(end.X, start.Y);

            using var pen = new Pen(Color.Gray, 4);

            g.DrawLine(pen, start, mid);
            g.DrawLine(pen, mid, end);
        }

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
