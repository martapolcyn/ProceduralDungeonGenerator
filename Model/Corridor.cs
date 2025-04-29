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

        public Corridor(Room startRoom, Room endRoom)
        {
            StartRoom = startRoom;
            EndRoom = endRoom;
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

        public override string ToString()
        {
            return $"Corridor: Start Room No. {StartRoom.RoomID}, End Room No. {EndRoom.RoomID}";
        }
    }

}
