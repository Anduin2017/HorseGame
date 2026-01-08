using HorseGame.Shared;
using HorseGame.Unified.Generators;

namespace HorseGame.Unified.Services
{
    public class ClueService
    {
        public List<string> GenerateClues(Game game)
        {
            var generators = new List<IClueGenerator>();

            // ===== DEDUCTIVE CLUES (组合推理线索) =====

            // NotFirst (排除法) - 频率：中等
            // 每轮为3个非冠军马生成排除线索
            generators.Add(new NotFirstGenerator());

            // TopTwo (范围限定 - 强) - 频率：低
            // 每轮为前2名生成线索，信息强，保持少量
            generators.Add(new TopTwoGenerator());

            // TopThree (范围限定 - 弱) - 频率：中高
            // 每轮为前3名生成线索，信息较弱，可以多一些
            generators.Add(new TopThreeGenerator());
            generators.Add(new TopThreeGenerator()); // 双倍频率

            // ===== LEGACY CLUES (原有线索，减少数量) =====

            // OverTake: 累计分数反超
            generators.Add(new OverTakeGenerator());

            // Faster: 单轮速度对比（减少到1个）
            generators.Add(new FasterGenerator());

            // TotalCount: 全局统计
            generators.Add(new TotalCountGenerator());

            // 移除：Positioning（太直接，降低推理难度）

            var allClues = generators
                .SelectMany(t => t.GetClues(game))
                .Select(t => t.Print())
                .ToList();

            // Randomize clues
            var random = new Random();
            return allClues.OrderBy(_ => random.Next()).ToList();
        }
    }
}
