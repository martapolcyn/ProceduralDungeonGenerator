using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Styles
{
    internal class DungeonStyleCave : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Cave";


        public List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked)
        {

            var path = new List<Point>();
            var rand = new Random();

            Point start = corridor.StartRoom.Center();
            Point end = corridor.EndRoom.Center();

            Point current = start;
            path.Add(current);

            int maxSteps = 2000;

            while (current != end && path.Count < maxSteps)
            {
                // Get all 4 neighbors
                var neighbors = new List<Point>
                {
                    new Point(current.X + 1, current.Y),
                    new Point(current.X - 1, current.Y),
                    new Point(current.X, current.Y + 1),
                    new Point(current.X, current.Y - 1)
                };

                // Filter neighbors within grid bounds
                neighbors = neighbors.Where(p =>
                    p.X >= 0 && p.X < ConfigManager.gridWidth &&
                    p.Y >= 0 && p.Y < ConfigManager.gridHeight).ToList();

                // Calculate distance to end for each neighbor
                var neighborDistances = neighbors.Select(p => new { Point = p, Dist = Distance(p, end) }).ToList();

                // Find minimal distance
                double minDist = neighborDistances.Min(nd => nd.Dist);

                // Collect neighbors that reduce distance (get closer)
                var closerNeighbors = neighborDistances.Where(nd => nd.Dist <= Distance(current, end)).Select(nd => nd.Point).ToList();

                // Bias: with 70% chance pick from closer neighbors, else from all neighbors
                List<Point> candidates;
                if (closerNeighbors.Count > 0 && rand.NextDouble() < 0.7)
                    candidates = closerNeighbors;
                else
                    candidates = neighbors;

                // Pick random next step
                current = candidates[rand.Next(candidates.Count)];
                path.Add(current);
            }

            corridor.Path = path;
            return path;

        }


        private double Distance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Brush GetCorridorBrush()
        {
            return new SolidBrush(Color.FromArgb(120, 72, 48));
        }

        public Brush GetRoomBrush()
        {
            return new SolidBrush(Color.FromArgb(120, 72, 48));
        }

        public void ArrangeRooms(List<Room> rooms)
        {
            var rand = new Random();
            var placedRooms = new List<Room>();
            const int maxAttempts = 100;

            foreach (var room in rooms)
            {
                bool placed = false;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    int x = rand.Next(1, ConfigManager.gridWidth - room.Width - 1);
                    int y = rand.Next(1, ConfigManager.gridHeight - room.Height - 1);

                    room.SetPosition(x, y);
                    room.InitializeGeometry(this);

                    if (!room.IsWithinBounds())
                        continue;

                    bool intersects = placedRooms.Any(other => room.Intersects(other));
                    if (intersects)
                        continue;

                    placedRooms.Add(room);
                    placed = true;
                    break;
                }

                if (!placed)
                {
                    Logger.Log($"[WARNING] Could not place room after {maxAttempts} attempts: {room}");
                }
            }

            // Overwrite original list
            rooms.Clear();
            rooms.AddRange(placedRooms);
        }


        public RoomShape DetermineRoomShape(Room room)
        {
            if (room.Type == RoomType.Entrance || room.Type == RoomType.Exit)
            {
                return RoomShape.Square;
            }

            int roll = random.Next(0, 10);
            return RoomShape.Cave;
            
        }
    }
}
