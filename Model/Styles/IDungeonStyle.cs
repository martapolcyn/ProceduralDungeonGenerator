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

        // For defining colour of the corridor - in the future tile style
        Brush GetCorridorBrush();

        // For defining colour of the room - in the future tile style
        Brush GetRoomBrush();

        // For defining shape of the room
        RoomShape DetermineRoomShape(Room room);

        // For defining room arrangement (more random, more determined)
        void ArrangeRooms(List<Room> rooms);

        // For defining corridor path (random walk, A*, straight)
        List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked);

    }
}
