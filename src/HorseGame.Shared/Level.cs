namespace HorseGame.Shared
{
    public class Level
    {
        public List<int> GryffindorSpeeds { get; set; } = new List<int>();
        public List<int> HufflepuffSpeeds { get; set; } = new List<int>();
        public List<int> RavenclawSpeeds { get; set; } = new List<int>();
        public List<int> SlytherinSpeeds { get; set; } = new List<int>();
    }

    public class ProgressMadeMeasurer
    {
        public double GetProgress(List<int> speeds, double elapsed) // 20,20,10,7,5, 1-5
        {
            if (elapsed >= 0 && elapsed < 1)
            {
                return elapsed * speeds[0];
            }

            if (elapsed >= 1 && elapsed < 2)
            {
                return speeds[0] + (elapsed - 1) * speeds[1];
            }

            if (elapsed >= 2 && elapsed < 3)
            {
                return speeds[0] + speeds[1] + (elapsed - 2) * speeds[2];
            }

            if (elapsed >= 3 && elapsed < 4)
            {
                return speeds[0] + speeds[1] + speeds[2] + (elapsed - 3) * speeds[3];
            }
            else
            {
                return speeds[0] + speeds[1] + speeds[2] + speeds[3] + (elapsed - 4) * speeds[4];
            }
        }
    }
}
