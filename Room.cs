using System;
using System.Drawing;

namespace ProceduralDungeonGenerator
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
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public RoomShape Shape { get; private set; }
        public RoomType Type { get; private set; }
        public RoomSize Size { get; private set; }

        public Room(int x, int y, RoomSize size, RoomShape shape, RoomType type)
        {
            X = x;
            Y = y;
            Size = size;
            Shape = shape;
            Type = type;
            (Width, Height) = GetRoomSize(size);
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

        public Point Center()
        {
            int centerX = X + Width / 2;
            int centerY = Y + Height / 2;
            return new Point(centerX, centerY);
        }

        // Draw room based on shape
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
                    // TODO: implement irregularly shaped room
                    g.FillRectangle(brush, X, Y, Width, Height);
                    break;
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
                   (X + Width + margin) <= Config.dungeonWidth &&
                   (Y + Height + margin) <= Config.dungeonHeight;
        }

        public override string ToString()
        {
            return $"Room: Type={Type}, Shape={Shape}, Size={Size}, X={X}, Y={Y}, Width={Width}, Height={Height}";
        }

    }
}
