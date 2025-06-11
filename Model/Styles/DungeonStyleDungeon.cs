using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Styles
{
    public class DungeonStyleDungeon : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Dungeon";

        public Brush GetCorridorBrush()
        {
            return Brushes.DarkGray;
        }

        public Brush GetRoomBrush()
        {
            return Brushes.SaddleBrown;
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
                < 5 => RoomShape.Rectangle,
                _ => RoomShape.LShape
            };
        }

        public List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked)
        {
            var startTile = corridor.StartRoom.GetClosestBoundaryTileTo(corridor.EndRoom.Center());
            var endTile = corridor.EndRoom.GetClosestBoundaryTileTo(startTile);

            var pathfinder = new AStarPathfinder(ConfigManager.gridWidth, ConfigManager.gridHeight, blocked);
            return pathfinder.FindPath(startTile, endTile);
        }



        public class AStarPathfinder
        {
            private int width, height;
            private HashSet<Point> blocked;

            public AStarPathfinder(int width, int height, HashSet<Point> blocked)
            {
                this.width = width;
                this.height = height;
                this.blocked = blocked;
            }

            public List<Point> FindPath(Point start, Point end)
            {
                var open = new PriorityQueue<Point, int>();
                var cameFrom = new Dictionary<Point, Point>();
                var gScore = new Dictionary<Point, int> { [start] = 0 };
                var fScore = new Dictionary<Point, int> { [start] = Heuristic(start, end) };

                open.Enqueue(start, fScore[start]);

                while (open.Count > 0)
                {
                    var current = open.Dequeue();

                    if (current == end)
                        return ReconstructPath(cameFrom, current);

                    foreach (var neighbor in GetNeighbors(current))
                    {
                        if (blocked.Contains(neighbor)) continue;

                        int tentativeG = gScore[current] + 1;
                        if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                        {
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeG;
                            fScore[neighbor] = tentativeG + Heuristic(neighbor, end);
                            if (!open.UnorderedItems.Any(i => i.Element == neighbor))
                                open.Enqueue(neighbor, fScore[neighbor]);
                        }
                    }
                }

                return new List<Point>(); // brak ścieżki
            }

            private int Heuristic(Point a, Point b)
                => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan

            private IEnumerable<Point> GetNeighbors(Point p)
            {
                var deltas = new[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1) };
                foreach (var d in deltas)
                {
                    var np = new Point(p.X + d.X, p.Y + d.Y);
                    if (np.X >= 0 && np.X < width && np.Y >= 0 && np.Y < height)
                        yield return np;
                }
            }

            private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
            {
                var path = new List<Point> { current };
                while (cameFrom.TryGetValue(current, out var prev))
                {
                    current = prev;
                    path.Add(current);
                }
                path.Reverse();
                return path;
            }
        }

    }
}
