namespace Bean
{
    public static class Random
    {
        private static System.Random _seededRandom;

        public static int RandomInt(int min, int max)
        {
            System.Random random = new System.Random();

            return random.Next(min, max);
        }

        public static int RandomSeededInt(int min, int max)
        {
            return _seededRandom.Next(min, max);
        }

        public static float RandomFloat(float min, float max)
        {
            System.Random random = new System.Random();
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static void SetSeededRandom(int seed)
        {
            _seededRandom = new System.Random(seed);
        }
    }
}
