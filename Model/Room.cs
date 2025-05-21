using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using ProceduralDungeonGenerator.Configuration;

namespace ProceduralDungeonGenerator.Model
{
    public enum RoomShape
    {
        Rectangle,
        Circle,
        Custom,
        Square,
        LShape
    }

    public enum RoomType
    {
        Entrance,
        KingChamber,
        Treasury,
        Normal,
        Exit,
        Laboratory,
        Engine,
        ControlRoom,

        Special,
        Any
    }

    public enum RoomSize
    {
        Small,
        Medium,
        Big
    }

    public class Room
    {
        private static int _idCounter = 0;
        public int ID { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public RoomShape? Shape { get; private set; }
        public RoomType Type { get; private set; }
        public RoomSize Size { get; private set; }

        private Point[]? _cachedLShapePoints;
        private Point[]? _cachedCustomShapePoints;

        public List<Enemy> Enemies { get; private set; } = new();
        public List<Artifact> Artifacts { get; private set; } = new();
        public List<Item> Items { get; private set; } = new();

        public Room(int x, int y, RoomSize size, RoomType type)
        {
            ID = ++_idCounter;
            X = x;
            Y = y;
            Size = size;
            Type = type;
            if (Type == RoomType.Entrance || Type == RoomType.Exit)
            {
                (Width, Height) = (20, 20);
            } else
            {
                (Width, Height) = GetRoomSize(size);
            }
        }

        // Assign enememy
        public void AssignEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        // Assign artifact
        public void AssignArtifact(Artifact artifact)
        {
            Artifacts.Add(artifact);
        }

        // Assign item
        public void AssignItem(Item item)
        {
            Items.Add(item);
        }

        // Assign item position
        public void AssignItemPosition(Item item)
        {
            Point position;

            switch (item.Placement)
            {
                case PlacementType.Wall:
                    position = GetRandomWallPosition();
                    break;
                case PlacementType.CorridorStart:
                    position = GetCorridorStartPosition();
                    break;
                case PlacementType.Any:
                default:
                    position = GetRandomInnerPosition();
                    break;
            }

            item.Position = position;
        }

        // Placement: Any (anywhere in the room)
        private Point GetRandomInnerPosition()
        {
            Random rand = new();
            int x = rand.Next(X + 1, X + Width - 1);
            int y = rand.Next(Y + 1, Y + Height - 1);
            return new Point(x, y);
        }

        // Placement: Wall (in the room at the wall)
        private Point GetRandomWallPosition()
        {
            Random rand = new();
            int side = rand.Next(4);

            return side switch
            {
                0 => new Point(rand.Next(X + 1, X + Width - 1), Y), // top
                1 => new Point(rand.Next(X + 1, X + Width - 1), Y + Height - 1), // bottom
                2 => new Point(X, rand.Next(Y + 1, Y + Height - 1)), // left
                3 => new Point(X + Width - 1, rand.Next(Y + 1, Y + Height - 1)), // right
                _ => GetRandomInnerPosition()
            };
        }

        // Placement: CorridorStart (e.g. door)
        private Point GetCorridorStartPosition()
        {
            return Center(); // TODO: implement finding start of corridor
        }

        // Room width and height based on room size and dungeon size
        private (int, int) GetRoomSize(RoomSize size)
        {
            Random rand = new Random();

            int w = ConfigManager.dungeonWidth;
            int h = ConfigManager.dungeonHeight;

            switch (size)
            {
                case RoomSize.Small:
                    return (rand.Next(w/20, w/16), rand.Next(h/20, h/12));
                case RoomSize.Medium:
                    return (rand.Next(w/16, w/8), rand.Next(h/12, h/6));
                case RoomSize.Big:
                    return (rand.Next(w/8, w/5), rand.Next(h/6, h/4));
                default:
                    return (w/16, w/12);
            }
        }

        // Get center of the room
        public Point Center()
        {
            int centerX = X + Width / 2;
            int centerY = Y + Height / 2;
            return new Point(centerX, centerY);
        }

        // Draw Room based on shape, with items, enemies and artifacts
        public void Draw(Graphics g, IDungeonStyle style)
        {
            DrawRoomShape(g, style);
            DrawRoomId(g);
            DrawItems(g);
            DrawEnemies(g);
            DrawArtifacts(g);
        }

        // Draw shape of the room
        private void DrawRoomShape(Graphics g, IDungeonStyle style)
        {
            Brush brush = style.GetRoomBrush();

            if (Shape == null)
                Shape = style.GetRoomShape(this);

            switch (Shape)
            {
                case RoomShape.Rectangle:
                    g.FillRectangle(brush, X, Y, Width, Height);
                    break;

                case RoomShape.Circle:
                    g.FillEllipse(brush, X, Y, Width, Height);
                    break;

                case RoomShape.Square:
                    g.FillRectangle(brush, X, Y, Width, Width);
                    break;

                case RoomShape.LShape:
                    using (GraphicsPath path = new())
                    {
                        _cachedLShapePoints ??= GetLShapePoints(X, Y, Width, Height);
                        path.AddPolygon(_cachedLShapePoints);
                        g.FillPath(brush, path);
                    }
                    break;

                case RoomShape.Custom:
                    using (GraphicsPath path = new())
                    {
                        _cachedCustomShapePoints ??= GetLShapePoints(X, Y, Width, Height);
                        path.AddPolygon(_cachedCustomShapePoints);
                        g.FillPath(brush, path);
                    }
                    break;
            }
        }

        // Sign room with id
        private void DrawRoomId(Graphics g)
        {
            using var font = new Font("Arial", 12);
            using var textBrush = new SolidBrush(Color.Black);

            string id = ID.ToString();
            float textWidth = g.MeasureString(id, font).Width;
            float textHeight = g.MeasureString(id, font).Height;

            float textX = X + (Width / 2) - (textWidth / 2);
            float textY = Y + (Height / 2) - (textHeight / 2);

            g.DrawString(id, font, textBrush, textX, textY);
        }

        // Draw items on positions
        private void DrawItems(Graphics g)
        {
            using var font = new Font("Arial", 6);
            Brush itemBrush = Brushes.Green;

            foreach (var item in Items)
            {
                if (item.Position == null)
                {
                    Random rand = new(item.GetHashCode() + ID);
                    int px = rand.Next(X + 4, X + Width - 4);
                    int py = rand.Next(Y + 4, Y + Height - 4);
                    item.Position = new Point(px, py);
                }

                Point pos = item.Position.Value;
                int size = 5;

                g.FillEllipse(itemBrush, pos.X - size / 2, pos.Y - size / 2, size, size);
                g.DrawString(item.Name, font, Brushes.Black, pos.X + 4, pos.Y + 2);
            }
        }

        // Draw enemies on positions
        private void DrawEnemies(Graphics g)
        {
            using var font = new Font("Arial", 6);
            Brush enemyBrush = Brushes.Red;

            foreach (var enemy in Enemies)
            {
                if (enemy.Position == null)
                {
                    Random rand = new(enemy.GetHashCode() + ID);
                    int px = rand.Next(X + 4, X + Width - 4);
                    int py = rand.Next(Y + 4, Y + Height - 4);
                    enemy.Position = new Point(px, py);
                }

                Point pos = enemy.Position.Value;
                int size = 6;

                g.FillRectangle(enemyBrush, pos.X - size / 2, pos.Y - size / 2, size, size);
                g.DrawString(enemy.Type.ToString(), font, Brushes.Black, pos.X + 4, pos.Y + 2);
            }
        }

        // Draw artifacts on positions
        private void DrawArtifacts(Graphics g)
        {
            using var font = new Font("Arial", 6);
            Brush artifactBrush = Brushes.Blue;

            foreach (var artifact in Artifacts)
            {
                if (artifact.Position == null)
                {
                    Random rand = new(artifact.GetHashCode() + ID);
                    int px = rand.Next(X + 4, X + Width - 4);
                    int py = rand.Next(Y + 4, Y + Height - 4);
                    artifact.Position = new Point(px, py);
                }

                Point pos = artifact.Position.Value;
                int size = 6;

                // Rysuj trójkąt (np. skierowany w górę)
                Point[] trianglePoints = new Point[]
                {
                    new Point(pos.X, pos.Y - size / 2),
                    new Point(pos.X - size / 2, pos.Y + size / 2),
                    new Point(pos.X + size / 2, pos.Y + size / 2)
                };

                g.FillPolygon(artifactBrush, trianglePoints);
                g.DrawString(artifact.Name.ToString(), font, Brushes.Black, pos.X + 4, pos.Y + 2);
            }
        }


        // L shape room
        private Point[] GetLShapePoints(int x, int y, int width, int height)
        {
            var random = new Random();
            int orientation = random.Next(0, 4); // 0–3, cztery warianty L

            int armWidth = width / 2;
            int armHeight = height / 2;

            return orientation switch
            {
                // └ L
                0 => new Point[]
                {
                    new(x, y),
                    new(x + armWidth, y),
                    new(x + armWidth, y + height - armHeight),
                    new(x + width, y + height - armHeight),
                    new(x + width, y + height),
                    new(x, y + height)
                },

                // ┌ L
                1 => new Point[]
                {
                    new(x + width - armWidth, y),
                    new(x + width, y),
                    new(x + width, y + height),
                    new(x, y + height),
                    new(x, y + armHeight),
                    new(x + width - armWidth, y + armHeight)
                },

                // ┘ L
                2 => new Point[]
                {
                    new(x, y),
                    new(x + width, y),
                    new(x + width, y + height),
                    new(x + width - armWidth, y + height),
                    new(x + width - armWidth, y + armHeight),
                    new(x, y + armHeight)
                },

                // ┐ L
                _ => new Point[]
                {
                    new(x, y),
                    new(x + armWidth, y),
                    new(x + armWidth, y + height - armHeight),
                    new(x + width, y + height - armHeight),
                    new(x + width, y + height),
                    new(x, y + height)
                },
            };
        }

        // Custom room shape
        private Point[] GenerateIrregularPolygon(int x, int y, int width, int height)
        {
            Random rand = new Random(ID);
            int pointCount = rand.Next(5, 9); // vertices

            List<Point> points = new();
            double angleStep = 2 * Math.PI / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                double angle = i * angleStep;
                double radiusX = width / 2 * (0.7 + rand.NextDouble() * 0.6);  // 70% - 130% radius
                double radiusY = height / 2 * (0.7 + rand.NextDouble() * 0.6);

                int px = x + width / 2 + (int)(Math.Cos(angle) * radiusX);
                int py = y + height / 2 + (int)(Math.Sin(angle) * radiusY);

                points.Add(new Point(px, py));
            }

            return points.ToArray();
        }

        // Check intersection with other rooms
        public bool Intersects(Room other)
        {
            return X < other.X + other.Width &&
                   X + Width > other.X &&
                   Y < other.Y + other.Height &&
                   Y + Height > other.Y;
        }

        // Check if the room fits the dungeon size
        public bool IsWithinBounds(int margin = 10)
        {
            return X >= margin &&
                   Y >= margin &&
                   X + Width + margin <= ConfigManager.dungeonWidth &&
                   Y + Height + margin <= ConfigManager.dungeonHeight;
        }

        public override string ToString()
        {
            string enemySummary = Enemies.Count == 0
                ? "None"
                : string.Join(", ", Enemies.Select(e => e.Type.ToString()));

            string artifactSummary = Artifacts.Count == 0
                ? "None"
                : string.Join(", ", Artifacts.Select(e => e.Name.ToString()));

            string itemSummary = Items.Count == 0
                ? "None"
                : string.Join(", ", Items.Select(e => e.Name.ToString()));

            return $"Room {ID}: Type={Type}, Shape={Shape}, Size={Size}, X={X}, Y={Y}, Width={Width}, Height={Height}, " +
                $"Enemies({Enemies.Count}): [{enemySummary}], " +
                $"Artrifacts({Artifacts.Count}): [{artifactSummary}], " +
                $"Items({Items.Count}): [{itemSummary}]";
        }
    }
}
