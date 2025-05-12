namespace ProceduralDungeonGenerator.Model
{
    public enum EnemyType
    {
        Zombie,
        Spider,
        Vampire,
        Mummy,
        Ufo,
        Alien
    }

    public class Enemy
    {
        public EnemyType Type { get; private set; }

        public Enemy(EnemyType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Enemy: Typ={Type}";
        }
    }
}