namespace ProceduralDungeonGenerator.Model.Objects
{
    public enum EnemyType
    {
        // Dungeon enemies
        Zombie,
        Spider,
        Vampire,
        Mummy,

        // Spaceship enemies
        Ufo,
        Alien,

        // Cave enemies
        Bat,
        CaveSpider,
        Troll,
        Lurker,
        GiantRat,
        Goblin
    }


    public class Enemy
    {
        public EnemyType Type { get; private set; }
        public Point? Position { get; set; }

        public Enemy(EnemyType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Enemy: Typ={Type}, Position={Position}";
        }
    }
}