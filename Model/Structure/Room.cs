using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Objects;
using ProceduralDungeonGenerator.Model.Styles;

namespace ProceduralDungeonGenerator.Model.Structure
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

        internal List<Point> RoomInteriorTiles { get; private set; }
        internal List<Point> RoomBoundaryTiles { get; private set; }


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
                (Width, Height) = (1, 1);
            } 
            else
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
            Random rand = new(item.GetHashCode() + ID);

            if (RoomInteriorTiles == null || RoomInteriorTiles.Count == 0 ||
                RoomBoundaryTiles == null || RoomBoundaryTiles.Count == 0)
            {
                throw new InvalidOperationException("RoomInteriorTiles or RoomBoundaryTiles not initialized. Ensure room tiles were generated before assigning positions.");
            }

            Point position = item.Placement switch
            {
                PlacementType.Wall => RoomBoundaryTiles[rand.Next(RoomBoundaryTiles.Count)],
                // PlacementType.CorridorStart => GetCorridorStartPosition(),
                PlacementType.Any or _ => RoomInteriorTiles[rand.Next(RoomInteriorTiles.Count)],
            };

            item.Position = position;
        }

        // Room width and height in tiles based on room size and dungeon size
        private (int, int) GetRoomSize(RoomSize size)
        {
            Random rand = new Random();

            int gridW = ConfigManager.gridWidth;
            int gridH = ConfigManager.gridHeight;

            switch (size)
            {
                case RoomSize.Small:
                    return (
                        rand.Next(gridW / 20, gridW / 12),
                        rand.Next(gridH / 20, gridH / 12)
                    );

                case RoomSize.Medium:
                    return (
                        rand.Next(gridW / 12, gridW / 8),
                        rand.Next(gridH / 12, gridH / 8)
                    );

                case RoomSize.Big:
                    return (
                        rand.Next(gridW / 8, gridW / 6),
                        rand.Next(gridH / 8, gridH / 6)
                    );

                default:
                    return (gridW / 20, gridH / 20);
            }
        }

        // Get center of the room
        public Point Center()
        {
            int centerX = X + Width / 2;
            int centerY = Y + Height / 2;
            return new Point(centerX, centerY);
        }

        // Get closest boundary tile to a point
        public Point GetClosestBoundaryTileTo(Point target)
        {
            return RoomBoundaryTiles
                .OrderBy(p => Distance(p, target))
                .First();
        }

        private double Distance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Room shapes definition
        public void InitializeGeometry(IDungeonStyle style)
        {
            if (Shape == null)
                Shape = style.DetermineRoomShape(this);

            RoomInteriorTiles = new List<Point>();
            RoomBoundaryTiles = new List<Point>();

            switch (Shape)
            {
                case RoomShape.Rectangle:
                case RoomShape.Square:
                    GenerateRectangularTiles();
                    break;

                case RoomShape.Circle:
                    GenerateCircularTiles();
                    break;

                case RoomShape.LShape:
                    GenerateLShapedTiles();
                    break;

                case RoomShape.Custom:
                    GenerateRectangularTiles();
                    break;
            }
        }

        private void GenerateRectangularTiles()
        {
            int height = Shape == RoomShape.Square ? Width : Height;

            for (int x = X; x < X + Width; x++)
            {
                for (int y = Y; y < Y + height; y++)
                {
                    Point tile = new(x, y);
                    RoomInteriorTiles.Add(tile);

                    // Jeśli sąsiaduje z zewnętrzem → brzeg
                    if (x == X || x == X + Width - 1 || y == Y || y == Y + height - 1)
                        RoomBoundaryTiles.Add(tile);
                }
            }
        }

        private void GenerateCircularTiles()
        {
            float centerX = X + Width / 2f;
            float centerY = Y + Height / 2f;
            float radiusX = Width / 2f;
            float radiusY = Height / 2f;

            for (int x = X; x < X + Width; x++)
            {
                for (int y = Y; y < Y + Height; y++)
                {
                    // Normalizujemy współrzędne do układu elipsy
                    float dx = (x + 0.5f - centerX) / radiusX;
                    float dy = (y + 0.5f - centerY) / radiusY;

                    if (dx * dx + dy * dy <= 1f) // punkt jest wewnątrz elipsy
                    {
                        Point tile = new(x, y);
                        RoomInteriorTiles.Add(tile);

                        // Sprawdź, czy to brzeg – sąsiad ma być na zewnątrz
                        if (IsBoundaryTile(x, y))
                            RoomBoundaryTiles.Add(tile);
                    }
                }
            }
        }

        private bool IsBoundaryTile(int x, int y)
        {
            foreach ((int dx, int dy) in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                int nx = x + dx;
                int ny = y + dy;

                float centerX = X + Width / 2f;
                float centerY = Y + Height / 2f;
                float radiusX = Width / 2f;
                float radiusY = Height / 2f;

                float ndx = (nx + 0.5f - centerX) / radiusX;
                float ndy = (ny + 0.5f - centerY) / radiusY;

                if (ndx * ndx + ndy * ndy > 1f)
                    return true; // sąsiad leży poza elipsą → to brzeg
            }

            return false;
        }

        private void GenerateLShapedTiles()
        {
            RoomInteriorTiles = new List<Point>();
            RoomBoundaryTiles = new List<Point>();

            // Wymiary prostokątów składowych
            int armWidth = Width / 2;
            int armHeight = Height / 2;

            // Pozycje
            int verticalX = X;
            int verticalY = Y;
            int verticalWidth = armWidth;
            int verticalHeight = Height;

            int horizontalX = X;
            int horizontalY = Y + armHeight;
            int horizontalWidth = Width;
            int horizontalHeight = Height - armHeight;

            // Dodajemy wszystkie tile z pionowego ramienia
            AddTilesFromRect(verticalX, verticalY, verticalWidth, verticalHeight);

            // Dodajemy wszystkie tile z poziomego ramienia (części wspólne nie będą dodane drugi raz)
            AddTilesFromRect(horizontalX, horizontalY, horizontalWidth, horizontalHeight);
        }

        private void AddTilesFromRect(int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    Point tile = new(x, y);

                    if (!RoomInteriorTiles.Contains(tile))
                        RoomInteriorTiles.Add(tile);

                    if (IsBoundaryTile(x, y))
                        RoomBoundaryTiles.Add(tile);
                }
            }
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
            int tileSize = ConfigManager.tileSize;

            if (RoomInteriorTiles == null || RoomInteriorTiles.Count == 0)
                return;

            foreach (Point tile in RoomInteriorTiles)
            {
                int x = tile.X * tileSize;
                int y = tile.Y * tileSize;
                g.FillRectangle(brush, x, y, tileSize, tileSize);
            }
        }

        // Sign room with id
        private void DrawRoomId(Graphics g)
        {
            int tileSize = ConfigManager.tileSize;
            using var font = new Font("Arial", 10);
            using var textBrush = new SolidBrush(Color.White);

            string id = ID.ToString();

            // Lewy górny róg pokoju w pikselach
            float textX = X * tileSize;
            float textY = Y * tileSize;

            g.DrawString(id, font, textBrush, textX, textY);
        }

        // Draw items on positions
        private void DrawItems(Graphics g)
        {
            int tileSize = ConfigManager.tileSize;
            using var font = new Font("Arial", 6);
            Brush itemBrush = Brushes.Green;

            foreach (var item in Items)
            {
                if (item.Position == null)
                {
                    // Wybierz losową kratkę z RoomInteriorTiles
                    if (RoomInteriorTiles != null && RoomInteriorTiles.Count > 0)
                    {
                        Random rand = new(item.GetHashCode() + ID);
                        int index = rand.Next(RoomInteriorTiles.Count);
                        item.Position = RoomInteriorTiles[index];
                    }
                }

                if (item.Position != null)
                {
                    Point pos = item.Position.Value;
                    int px = pos.X * tileSize;
                    int py = pos.Y * tileSize;

                    int size = tileSize / 2;
                    int offset = (tileSize - size) / 2;

                    g.FillEllipse(itemBrush, px + offset, py + offset, size, size);
                    g.DrawString(item.Name, font, Brushes.Black, px + size, py);
                }
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

                    if (RoomInteriorTiles != null && RoomInteriorTiles.Count > 0)
                    {
                        int index = rand.Next(RoomInteriorTiles.Count);
                        enemy.Position = RoomInteriorTiles[index];
                    }
                    else
                    {
                        // fallback – tile środkowy
                        int centerX = X + Width / 2;
                        int centerY = Y + Height / 2;
                        enemy.Position = new Point(centerX, centerY);
                    }
                }

                Point tilePos = enemy.Position.Value;
                int tileSize = ConfigManager.tileSize; // <- dodaj, jeśli masz skalowanie
                int centerXPixel = tilePos.X * tileSize + tileSize / 2;
                int centerYPixel = tilePos.Y * tileSize + tileSize / 2;
                int size = 6;

                g.FillRectangle(enemyBrush, centerXPixel - size / 2, centerYPixel - size / 2, size, size);
                g.DrawString(enemy.Type.ToString(), font, Brushes.Black, centerXPixel + 4, centerYPixel + 2);
            }
        }

        // Draw artifacts on positions
        private void DrawArtifacts(Graphics g)
        {
            using var font = new Font("Arial", 6);
            Brush artifactBrush = Brushes.Blue;
            int tileSize = ConfigManager.tileSize;

            foreach (var artifact in Artifacts)
            {
                if (artifact.Position == null)
                {
                    Random rand = new(artifact.GetHashCode() + ID);

                    if (RoomInteriorTiles != null && RoomInteriorTiles.Count > 0)
                    {
                        Point tile = RoomInteriorTiles[rand.Next(RoomInteriorTiles.Count)];
                        artifact.Position = tile;
                    }
                    else
                    {
                        // fallback: środek pokoju w tile’ach
                        artifact.Position = new Point(X + Width / 2, Y + Height / 2);
                    }
                }

                Point tilePos = artifact.Position.Value;

                // Przeliczenie środka tile'a na piksele
                int centerX = tilePos.X * tileSize + tileSize / 2;
                int centerY = tilePos.Y * tileSize + tileSize / 2;
                int size = 6;

                Point[] trianglePoints = new Point[]
                {
            new Point(centerX, centerY - size / 2),
            new Point(centerX - size / 2, centerY + size / 2),
            new Point(centerX + size / 2, centerY + size / 2)
                };

                g.FillPolygon(artifactBrush, trianglePoints);
                g.DrawString(artifact.Name.ToString(), font, Brushes.Black, centerX + 4, centerY + 2);
            }
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
        public bool IsWithinBounds()
        {
            var tileSize = ConfigManager.tileSize;
            return X >= 1 &&
                   Y >= 1 &&
                   X + Width + 1 <= ConfigManager.gridWidth &&
                   Y + Height + 1 <= ConfigManager.gridHeight;
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
