using System.Drawing.Drawing2D;
using ProceduralDungeonGenerator.Configuration;
using ProceduralDungeonGenerator.Model;

public class Corridor
{
    public Room StartRoom { get; }
    public Room EndRoom { get; }
    public double Distance { get; }

    internal List<Point> Path { get; set; } = new();

    public Corridor(Room startRoom, Room endRoom)
    {
        StartRoom = startRoom;
        EndRoom = endRoom;
        Distance = CalculateRoomDistance(startRoom, endRoom);
    }

    private double CalculateRoomDistance(Room a, Room b)
    {
        int dx = a.Center().X - b.Center().X;
        int dy = a.Center().Y - b.Center().Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private int CalculatePointDistance(Point a, Point b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);

    public void Draw(Graphics g, IDungeonStyle style)
    {
        Brush brush = style.GetCorridorBrush();
        int tileSize = ConfigManager.tileSize;

        foreach (Point tile in Path)
        {
            int x = tile.X * tileSize;
            int y = tile.Y * tileSize;
            g.FillRectangle(brush, x, y, tileSize, tileSize);
        }
    }

    public bool Connects(Room a, Room b)
    {
        return (StartRoom == a && EndRoom == b) || (StartRoom == b && EndRoom == a);
    }


    public override string ToString()
    {
        return $"Corridor: Start Room No. {StartRoom.ID}, End Room No. {EndRoom.ID}";
    }
}
