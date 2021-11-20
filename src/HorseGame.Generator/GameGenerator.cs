using HorseGame.Shared;

namespace HorseGame.Generator
{
    public class GameGenerator
    {
        public Game Build()
        {
            var game = new Game();
            for (int i = 0; i < Consts.LevelsCountInAGame; i++)
            {
                var level = this.BuildASuitableLevel();
                game.Levels.Add(level);
            }
            return game;
        }

        public Level BuildASuitableLevel()
        {
            while (true)
            {
                var candidateLevel = this.BuildALevel();
                if (IsLevelSuitable(candidateLevel))
                {
                    Console.WriteLine("Level suitable. Will continue!");
                    return candidateLevel;
                }
                else
                {
                    Console.WriteLine("Level not suitable. Will retry!");
                }
            }
        }

        public Level BuildALevel()
        {
            var level = new Level();
            var randomSpeeds = GetRandomSpeeds();
            level.GryffindorSpeeds = randomSpeeds.Skip(Consts.SpeedStagesEachLevel * 0).Take(Consts.SpeedStagesEachLevel).ToList();
            level.HufflepuffSpeeds = randomSpeeds.Skip(Consts.SpeedStagesEachLevel * 1).Take(Consts.SpeedStagesEachLevel).ToList();
            level.RavenclawSpeeds = randomSpeeds.Skip(Consts.SpeedStagesEachLevel * 2).Take(Consts.SpeedStagesEachLevel).ToList();
            level.SlytherinSpeeds = randomSpeeds.Skip(Consts.SpeedStagesEachLevel * 3).Take(Consts.SpeedStagesEachLevel).ToList();
            return level;
        }

        public List<int> GetRandomSpeeds()
        {
            var totalSpeedsCount = Consts.SpeedStagesEachLevel * Consts.HorseCount;
            var eachSpeedsCount = totalSpeedsCount / Consts.PossibleSpeeds.Count();
            var speeds = new List<int>();
            foreach (var possibleSpeed in Consts.PossibleSpeeds)
            {
                for (int i = 0; i < eachSpeedsCount; i++)
                {
                    speeds.Add(possibleSpeed);
                }
            }

            var random = new Random();

            return speeds.OrderBy(t => random.Next()).ToList();
        }

        public bool IsLevelSuitable(Level level)
        {
            var evaluator = new HorseEvaluator();
            var gryffindorTime = evaluator.TimeEvaluator(level.GryffindorSpeeds);
            var hufflepuffTime = evaluator.TimeEvaluator(level.HufflepuffSpeeds);
            var ravenclawTime = evaluator.TimeEvaluator(level.RavenclawSpeeds);
            var slytherinTime = evaluator.TimeEvaluator(level.SlytherinSpeeds);

            var speedUnique = gryffindorTime != hufflepuffTime &&
                hufflepuffTime != ravenclawTime &&
                ravenclawTime != slytherinTime;

            var speedsList = new double[]
            {
                gryffindorTime,
                hufflepuffTime,
                ravenclawTime,
                slytherinTime
            };
            var speedNear = speedsList.Max() - speedsList.Min() < 5;
            var notSoLong = speedsList.Max() < 16;
            var notSoFast = speedsList.Min() > 6;

            return speedUnique && speedNear && notSoLong && notSoFast;
        }
    }
}
