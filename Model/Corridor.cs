using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class Corridor
    {
        public Point Start { get; }
        public Point End { get; }

        public Corridor(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public void Draw(Graphics g)
        {
            using var pen = new Pen(Color.Gray, 4);
            g.DrawLine(pen, Start, End);
        }

        public override string ToString()
        {
            return $"Corridor: Start=({Start.X},{Start.Y}), End=({End.X},{End.Y})";
        }
    }

}
