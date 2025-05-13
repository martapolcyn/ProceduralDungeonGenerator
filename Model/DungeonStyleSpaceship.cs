using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleSpaceship : IDungeonStyle
    {
        public string Name => "Spaceship";

        public Pen GetCorridorPen()
        {
            return new Pen(Color.Cyan, 20);
        }

        public Brush GetRoomBrush()
        {
            return Brushes.GreenYellow;
        }

        public RoomShape GetRoomShape(Room room)
        {
            throw new NotImplementedException();
        }

    }
}
