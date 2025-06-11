using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model.Structure;

namespace ProceduralDungeonGenerator.Model.Styles
{
    public class DungeonStyleSpaceship : IDungeonStyle
    {
        private static readonly Random random = new();

        public string Name => "Spaceship";

        public Brush GetCorridorBrush()
        {
            return Brushes.Cyan;
        }

        public Brush GetRoomBrush()
        {
            return Brushes.GreenYellow;
        }

        public RoomShape DetermineRoomShape(Room room)
        {
            if (room.Type == RoomType.Entrance || room.Type == RoomType.Exit)
            {
                return RoomShape.Square;
            }

            int roll = random.Next(0, 10);
            return roll switch
            {
                < 5 => RoomShape.Circle,
                _ => RoomShape.Square
            };
        }

        public List<Point> DetermineCorridorPath(Corridor corridor, HashSet<Point> blocked)
        {
            var path = new List<Point>();

            // Znajdź najbliższe brzegi obu pokoi
            var startTile = corridor.StartRoom.GetClosestBoundaryTileTo(corridor.EndRoom.Center());
            var endTile = corridor.EndRoom.GetClosestBoundaryTileTo(startTile);

            // Najpierw X, potem Y
            int x = startTile.X;
            int y = startTile.Y;

            path.Add(new Point(x, y));

            while (x != endTile.X)
            {
                x += x < endTile.X ? 1 : -1;
                path.Add(new Point(x, y));
            }

            while (y != endTile.Y)
            {
                y += y < endTile.Y ? 1 : -1;
                path.Add(new Point(x, y));
            }

            return path;
        }

        public void ArrangeRooms(List<Room> rooms)
        {
            const int spacing = 2;
            int centerY = ConfigManager.gridHeight / 2;

            var entranceRooms = rooms.Where(r => r.Type == RoomType.Entrance).ToList();
            var exitRooms = rooms.Where(r => r.Type == RoomType.Exit).ToList();
            var spineRooms = rooms
                .Where(r => r.Type != RoomType.Entrance && r.Type != RoomType.Exit)
                .ToList();

            var rand = new Random();
            spineRooms = spineRooms.OrderBy(_ => rand.Next()).ToList();

            var arrangedRooms = new List<Room>();

            // Place entrance
            PlaceEntrance(entranceRooms, arrangedRooms, centerY, spacing);

            // Place exit
            PlaceExit(exitRooms, arrangedRooms, centerY, spacing);

            // Place spine rooms between entrance and exit
            PlaceSpineRooms(spineRooms, arrangedRooms, spacing);

            var wingRooms = rooms
                .Except(arrangedRooms)
                .ToList();

            // Place wings above and below the middle spine room
            PlaceWings(wingRooms, arrangedRooms, spacing);

            // Update original rooms list
            rooms.Clear();
            rooms.AddRange(arrangedRooms);
        }

        private void PlaceEntrance(List<Room> entranceRooms, List<Room> arrangedRooms, int centerY, int spacing)
        {
            if (!entranceRooms.Any()) return;

            var entrance = entranceRooms.First();
            entrance.SetPosition(spacing, centerY);
            entrance.InitializeGeometry(this);
            arrangedRooms.Add(entrance);
        }

        private void PlaceExit(List<Room> exitRooms, List<Room> arrangedRooms, int centerY, int spacing)
        {
            if (!exitRooms.Any()) return;

            var exit = exitRooms.First();
            exit.SetPosition(ConfigManager.gridWidth - exit.Width - spacing, centerY);
            exit.InitializeGeometry(this);
            arrangedRooms.Add(exit);
        }

        private void PlaceSpineRooms(List<Room> spineRooms, List<Room> arrangedRooms, int spacing)
        {
            if (!spineRooms.Any())
                return;

            var entrance = arrangedRooms.FirstOrDefault(r => r.Type == RoomType.Entrance);
            var exit = arrangedRooms.FirstOrDefault(r => r.Type == RoomType.Exit);

            int startX = entrance != null ? entrance.X + entrance.Width + spacing : spacing;
            int endX = exit != null ? exit.X - spacing : ConfigManager.gridWidth - spacing;

            int centerY = ConfigManager.gridHeight / 2;  // The vertical axis to align with
            int currentX = startX;

            foreach (var room in spineRooms)
            {
                if (currentX + room.Width > endX)
                {
                    Logger.Log($"[WARNING] Not enough space to place spine room: {room}");
                    break;
                }

                int posY = centerY - room.Height / 2;  // Align vertically on center axis

                room.SetPosition(currentX, posY);
                room.InitializeGeometry(this);
                arrangedRooms.Add(room);

                currentX += room.Width + spacing;
            }
        }

        private void PlaceWings(List<Room> wingRooms, List<Room> arrangedRooms, int spacing)
        {
            if (wingRooms.Count < 2 || arrangedRooms.Count < 3)
            {
                Logger.Log("[DEBUG] Not enough rooms to place wings.");
                return;
            }

            // Sort spine rooms left to right (by X)
            var orderedSpine = arrangedRooms.OrderBy(r => r.X).ToList();

            // Get the third spine room (index 2)
            var spineRoom = orderedSpine[3];
            Logger.Log($"[DEBUG] Placing wings at spine room: {spineRoom}");

            // Get center X from spine room to align horizontally
            int centerX = spineRoom.X + spineRoom.Width / 2;

            // Split wings into top and bottom
            var topWing = wingRooms[0];
            var bottomWing = wingRooms[1];

            // Align horizontally with centerX
            int posXTop = centerX - topWing.Width / 2;
            int posXBottom = centerX - bottomWing.Width / 2;

            // Offset vertically from the spine room
            int posYTop = spineRoom.Y - topWing.Height - spacing;
            int posYBottom = spineRoom.Y + spineRoom.Height + spacing;

            // Set positions and initialize geometry
            topWing.SetPosition(posXTop, posYTop);
            topWing.InitializeGeometry(this);
            arrangedRooms.Add(topWing);

            bottomWing.SetPosition(posXBottom, posYBottom);
            bottomWing.InitializeGeometry(this);
            arrangedRooms.Add(bottomWing);

            Logger.Log($"[DEBUG] Wing rooms placed at ({posXTop}, {posYTop}) and ({posXBottom}, {posYBottom})");
        }

    }
}
