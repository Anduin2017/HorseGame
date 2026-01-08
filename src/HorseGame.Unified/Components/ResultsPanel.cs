using Gtk;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for displaying race results with trophy medals
    /// </summary>
    public class ResultsPanel : Label
    {
        private static readonly string[] Medals = { "🏆", "🥈", "🥉", "4th" };

        public ResultsPanel() : base("")
        {
            SetSizeRequest(-1, 100);
            Xalign = 0.5f;
            Yalign = 0;
        }

        public void ShowResults(List<string> rankings)
        {
            if (rankings.Count == 0)
            {
                Markup = "";
                return;
            }

            var formattedResults = new List<string>();
            for (int i = 0; i < rankings.Count && i < 4; i++)
            {
                formattedResults.Add($"{Medals[i]} {rankings[i]}");
            }

            Markup = $"<span size='14000' weight='bold'>\n{string.Join("\n", formattedResults)}</span>";
        }

        public void Clear()
        {
            Markup = "";
        }
    }
}
