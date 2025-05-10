using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public class DungeonStyleSpaceship : IDungeonStyle
    {
        public Pen GetCorridorPen()
        {
            throw new NotImplementedException();
        }

        public Color GetRoomColor(Room room)
        {
            throw new NotImplementedException();
        }

        public RoomShape GetRoomShape(Room room)
        {
            throw new NotImplementedException();
        }

        public string GetName() => "Spaceship";
    }
}
