using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Styles
{
    public interface IDungeonStyle
    {
        string Name { get; }

        Brush GetCorridorBrush();

        Brush GetRoomBrush();

        RoomShape DetermineRoomShape(Room room);

        List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked);

    }
}
