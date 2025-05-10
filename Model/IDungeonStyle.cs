using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public interface IDungeonStyle
    {
        RoomShape GetRoomShape(Room room);
        Color GetRoomColor(Room room);
        Pen GetCorridorPen();

        string GetName();
    }
}
