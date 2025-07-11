﻿using System;
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
        LShape,
        Cave
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
        Lair,
        CrystalChamber,
        Tunnel,

        Special,
        Any
    }

    public enum RoomSize
    {
        Small,
        Medium,
        Big,
        Cave
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
        private HashSet<Point> OccupiedTiles { get; set; } = new();

        public List<Enemy> Enemies { get; private set; } = new();
        public List<Artifact> Artifacts { get; private set; } = new();
        public List<Item> Items { get; private set; } = new();

        public Room(RoomSize size, RoomType type)
        {
            ID = ++_idCounter;

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

        // Position the room
        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;
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

        // Assign enemies position
        public void AssignEnemyPositions()
        {
            var occupiedTiles = new HashSet<Point>();

            foreach (var enemy in Enemies)
            {
                if (enemy.Position == null)
                {
                    var freeTiles = new List<Point>();
                    if (RoomInteriorTiles != null && OccupiedTiles != null)
                    {
                        foreach (var p in RoomInteriorTiles)
                        {
                            if (!occupiedTiles.Contains(p))
                                freeTiles.Add(p);
                        }
                    }
                    if (freeTiles.Count > 0)
                    {
                        Random rand = new Random(enemy.GetHashCode() + ID);
                        enemy.Position = freeTiles[rand.Next(freeTiles.Count)];
                        occupiedTiles.Add(enemy.Position.Value);
                    }
                    else
                    {
                        // fallback: środek pokoju
                        enemy.Position = new Point(X + Width / 2, Y + Height / 2);
                    }
                }
                else
                {
                    occupiedTiles.Add(enemy.Position.Value);
                }
            }
        }

        // Assign artifacts position
        public void AssignArtifactPositions()
        {
            var occupiedTiles = new HashSet<Point>();
            // blocked by enemies
            foreach (var enemy in Enemies)
            {
                if (enemy.Position != null)
                    occupiedTiles.Add(enemy.Position.Value);
            }

            foreach (var artifact in Artifacts)
            {
                if (artifact.Position == null)
                {
                    var freeTiles = new List<Point>();
                    if (RoomInteriorTiles != null)
                    {
                        foreach (var p in RoomInteriorTiles)
                        {
                            if (!occupiedTiles.Contains(p))
                                freeTiles.Add(p);
                        }
                    }
                    if (freeTiles.Count > 0)
                    {
                        Random rand = new Random(artifact.GetHashCode() + ID);
                        artifact.Position = freeTiles[rand.Next(freeTiles.Count)];
                        occupiedTiles.Add(artifact.Position.Value);
                    }
                    else
                    {
                        // fallback
                        artifact.Position = new Point(X + Width / 2, Y + Height / 2);
                    }
                }
                else
                {
                    occupiedTiles.Add(artifact.Position.Value);
                }
            }
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

            // avoid occupied tiles
            var occupiedTiles = new HashSet<Point>();

            foreach (var enemy in Enemies)
                if (enemy.Position != null)
                    occupiedTiles.Add(enemy.Position.Value);

            foreach (var artifact in Artifacts)
                if (artifact.Position != null)
                    occupiedTiles.Add(artifact.Position.Value);

            foreach (var existingItem in Items)
                if (existingItem.Position != null)
                    occupiedTiles.Add(existingItem.Position.Value);

            List<Point> candidateTiles = item.Placement switch
            {
                PlacementType.Wall => RoomBoundaryTiles.Where(p => !occupiedTiles.Contains(p)).ToList(),
                PlacementType.Any or _ => RoomInteriorTiles.Where(p => !occupiedTiles.Contains(p)).ToList(),
            };

            if (candidateTiles.Count == 0)
            {
                // fallback
                item.Position = new Point(X + Width / 2, Y + Height / 2);
            }
            else
            {
                item.Position = candidateTiles[rand.Next(candidateTiles.Count)];
            }
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

                case RoomSize.Cave:
                    return (
                        rand.Next(gridW / 8, gridW / 4),
                        rand.Next(gridH / 6, gridH / 3)
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

        // Calculate distance between two points
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

                case RoomShape.Cave:
                    GenerateCaveTiles();
                    break;
            }
        }

        // Cave shape - cellural automata
        private void GenerateCaveTiles()
        {
            int width = Width;
            int height = Height;
            int[,] map = new int[width, height];
            Random rand = new Random(GetHashCode()); // Seeded for room variation

            // Random fill map
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                        map[x, y] = 1; // Wall boundary
                    else
                        map[x, y] = rand.NextDouble() < 0.45 ? 1 : 0;
                }
            }

            // Smooth map using cellular automata
            for (int i = 0; i < 4; i++)
                map = SmoothMap(map);

            RoomInteriorTiles = new List<Point>();
            RoomBoundaryTiles = new List<Point>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int globalX = X + x;
                    int globalY = Y + y;
                    Point tile = new(globalX, globalY);

                    if (map[x, y] == 0)
                    {
                        // Floor
                        RoomInteriorTiles.Add(tile);

                        // Floor next to wall
                        if (IsAdjacentToWall(map, x, y))
                        {
                            RoomBoundaryTiles.Add(tile);
                        }
                    }
                }
            }
        }

        private int[,] SmoothMap(int[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);
            int[,] newMap = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int wallCount = GetSurroundingWallCount(map, x, y);
                    newMap[x, y] = wallCount > 4 ? 1 : 0;
                }
            }

            return newMap;
        }

        private int GetSurroundingWallCount(int[,] map, int gridX, int gridY)
        {
            int wallCount = 0;
            for (int x = gridX - 1; x <= gridX + 1; x++)
            {
                for (int y = gridY - 1; y <= gridY + 1; y++)
                {
                    if (x == gridX && y == gridY) continue;
                    if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1))
                        wallCount++;
                    else
                        wallCount += map[x, y];
                }
            }
            return wallCount;
        }

        private bool IsAdjacentToWall(int[,] map, int x, int y)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            int[,] directions = new int[,] { { 0, -1 }, { 0, 1 }, { -1, 0 }, { 1, 0 } };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + directions[i, 0];
                int ny = y + directions[i, 1];

                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    if (map[nx, ny] == 1)
                        return true;
                }
            }
            return false;
        }

        // Rectangular room interior and boundary tiles
        private void GenerateRectangularTiles()
        {
            int height = Shape == RoomShape.Square ? Width : Height;

            for (int x = X; x < X + Width; x++)
            {
                for (int y = Y; y < Y + height; y++)
                {
                    Point tile = new(x, y);
                    RoomInteriorTiles.Add(tile);

                    // Boundaries if the neighbour is no room
                    if (x == X || x == X + Width - 1 || y == Y || y == Y + height - 1)
                        RoomBoundaryTiles.Add(tile);
                }
            }
        }

        // Eliptical room interior and boundary tiles
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
                    // Elipse
                    float dx = (x + 0.5f - centerX) / radiusX;
                    float dy = (y + 0.5f - centerY) / radiusY;

                    if (dx * dx + dy * dy <= 1f)
                    {
                        Point tile = new(x, y);
                        RoomInteriorTiles.Add(tile);

                        // Check if boundary
                        if (IsBoundaryTile(x, y))
                            RoomBoundaryTiles.Add(tile);
                    }
                }
            }
        }

        // Chec if tile is a boundary tile
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
                    return true; // Boundary
            }

            return false;
        }

        // L Shaped room interior and boundary tiles
        private void GenerateLShapedTiles()
        {
            RoomInteriorTiles = new List<Point>();
            RoomBoundaryTiles = new List<Point>();

            int armWidth = Width / 2;
            int armHeight = Height / 2;

            int verticalX = X;
            int verticalY = Y;
            int verticalWidth = armWidth;
            int verticalHeight = Height;

            int horizontalX = X;
            int horizontalY = Y + armHeight;
            int horizontalWidth = Width;
            int horizontalHeight = Height - armHeight;

            AddTilesFromRect(verticalX, verticalY, verticalWidth, verticalHeight);

            AddTilesFromRect(horizontalX, horizontalY, horizontalWidth, horizontalHeight);
        }

        // Helper function for L shape
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
                    // Random tile from RoomInteriorTiles
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
                    continue;

                Point tilePos = enemy.Position.Value;
                int tileSize = ConfigManager.tileSize;
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

            foreach (var artifact in Artifacts)
            {
                if (artifact.Position == null)
                    continue;

                Point tilePos = artifact.Position.Value;
                int tileSize = ConfigManager.tileSize;
                int centerXPixel = tilePos.X * tileSize + tileSize / 2;
                int centerYPixel = tilePos.Y * tileSize + tileSize / 2;
                int size = 6;

                g.FillRectangle(artifactBrush, centerXPixel - size / 2, centerYPixel - size / 2, size, size);
                g.DrawString(artifact.Name.ToString(), font, Brushes.Black, centerXPixel + 4, centerYPixel + 2);
            }

        }

        // Check intersection with other rooms
        public bool Intersects(Room other)
        {
            return !(this.X + this.Width + 1 <= other.X ||
                     other.X + other.Width + 1 <= this.X ||
                     this.Y + this.Height + 1 <= other.Y ||
                     other.Y + other.Height + 1 <= this.Y);
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
