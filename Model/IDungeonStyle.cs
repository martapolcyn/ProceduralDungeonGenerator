using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProceduralDungeonGenerator.Model
{
    public interface IDungeonStyle
    {
        string Name { get; }

        Pen GetCorridorPen();

        Brush GetRoomBrush();

        RoomShape GetRoomShape(Room room);

        //void GenerateCorridors(List<Room> rooms, List<Corridor> corridors, Func<Room, Room, double> distanceFunc);

    }
}
