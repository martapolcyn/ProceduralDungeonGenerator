using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleDungeon : IDungeonStyle
    {
        public string Name => "Dungeon";

        public Pen GetCorridorPen()
        {
            return new Pen(Color.DarkGray, 4);
        }

        public Brush GetRoomBrush()
        {
            return Brushes.SaddleBrown;
        }

        public RoomShape GetRoomShape(Room room)
        {
            throw new NotImplementedException();
        }

        
    }
}
