using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleDungeon : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Dungeon";

        public Pen GetCorridorPen()
        {
            return new Pen(Color.DarkGray, 10);
        }

        public Brush GetRoomBrush()
        {
            return Brushes.SaddleBrown;
        }

        public RoomShape GetRoomShape(Room room)
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

        
    }
}
