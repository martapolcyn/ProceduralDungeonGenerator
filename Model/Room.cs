﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ProceduralDungeonGenerator.Model
{
    public enum RoomShape
    {
        Rectangle,
        Circle,
        Custom
    }

    public enum RoomType
    {
        Entrance,
        KingChamber,
        Treasury,
        Normal,
        Exit
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
        public RoomShape Shape { get; private set; }
        public RoomType Type { get; private set; }
        public RoomSize Size { get; private set; }

        public List<Enemy> Enemies { get; private set; } = new();
        public List<Artifact> Artifacts { get; private set; } = new();

        public Room(int x, int y, RoomSize size, RoomShape shape, RoomType type)
        {
            ID = ++_idCounter;
            X = x;
            Y = y;
            Size = size;
            Shape = shape;
            Type = type;
            (Width, Height) = GetRoomSize(size);
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

        // Room width and height based on room size and dungeon size
        private (int, int) GetRoomSize(RoomSize size)
        {
            Random rand = new Random();

            int w = Config.dungeonWidth;
            int h = Config.dungeonHeight;

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

        // Draw room based on shape
        // TODO: implement irregularly shaped room
        public void Draw(Graphics g)
        {
            Brush brush = GetBrushForRoomType();

            switch (Shape)
            {
                case RoomShape.Rectangle:
                    g.FillRectangle(brush, X, Y, Width, Height);
                    break;
                case RoomShape.Circle:
                    g.FillEllipse(brush, X, Y, Width, Height);
                    break;
                case RoomShape.Custom:
                    {
                        using GraphicsPath path = new();
                        var points = GenerateIrregularPolygon(X, Y, Width, Height);
                        path.AddPolygon(points);
                        g.FillPath(brush, path);
                        break;
                    }
            }

            using (var font = new Font("Arial", 12))
            using (var textBrush = new SolidBrush(Color.Black))
            {
                string id = ID.ToString();
                
                float textWidth = g.MeasureString(id, font).Width;
                float textHeight = g.MeasureString(id, font).Height;
                float textX = X + (Width / 2) - (textWidth / 2);
                float textY = Y + (Height / 2) - (textHeight / 2);

                g.DrawString(id, font, textBrush, textX, textY);
            }
        }

        // Room colour based on type
        // TODO: define tiles with patterns instead
        private Brush GetBrushForRoomType()
        {
            switch (Type)
            {
                case RoomType.Entrance:
                    return Brushes.Green;
                case RoomType.KingChamber:
                    return Brushes.Red;
                case RoomType.Treasury:
                    return Brushes.Gold;
                case RoomType.Normal:
                    return Brushes.Gray;
                case RoomType.Exit:
                    return Brushes.Blue;
                default:
                    return Brushes.White;
            }
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
                   X + Width + margin <= Config.dungeonWidth &&
                   Y + Height + margin <= Config.dungeonHeight;
        }

        public override string ToString()
        {
            string enemySummary = Enemies.Count == 0
                ? "None"
                : string.Join(", ", Enemies.Select(e => e.Type.ToString()));

            string artifactSummary = Artifacts.Count == 0
                ? "None"
                : string.Join(", ", Artifacts.Select(e => e.Name.ToString()));

            return $"Room {ID}: Type={Type}, Shape={Shape}, Size={Size}, X={X}, Y={Y}, Width={Width}, Height={Height}, " +
                $"Enemies({Enemies.Count}): [{enemySummary}], " +
                $"Artrifacts({Artifacts.Count}): [{artifactSummary}]";
        }
    }
}
