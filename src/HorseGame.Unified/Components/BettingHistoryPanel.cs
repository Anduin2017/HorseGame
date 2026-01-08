
#pragma warning disable
using Gtk;
using HorseGame.Shared;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for displaying betting history across all rounds
    /// </summary>
    public class BettingHistoryPanel : VBox
    {
        private readonly ListStore bettingListStore;

        public BettingHistoryPanel() : base(false, 10)
        {
            // Title
            var titleLabel = new Label();
            titleLabel.Markup = "<b>📊 Betting History (All Rounds)</b>";
            PackStart(titleLabel, false, false, 0);

            // TreeView with scrolling
            var scrolled = new ScrolledWindow();

            // ListStore: Player name + 10 round columns
            bettingListStore = new ListStore(
                typeof(string), typeof(string), typeof(string), typeof(string), typeof(string),
                typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string)
            );

            var bettingTreeView1 = new TreeView(bettingListStore);

            // Player column
            var playerCol = new TreeViewColumn { Title = "Player" };
            var playerCell = new CellRendererText();
            playerCol.PackStart(playerCell, true);
            playerCol.AddAttribute(playerCell, "text", 0);
            bettingTreeView1.AppendColumn(playerCol);

            // 10 round columns
            for (int i = 0; i < 10; i++)
            {
                var roundCol = new TreeViewColumn { Title = $"R{i + 1}" };
                var roundCell = new CellRendererText { WrapWidth = 70, WrapMode = Pango.WrapMode.Word };
                roundCol.PackStart(roundCell, true);
                roundCol.AddAttribute(roundCell, "text", i + 1);
                bettingTreeView1.AppendColumn(roundCol);
            }

            scrolled.Add(bettingTreeView1);
            PackStart(scrolled, true, true, 0); // Expand with window height
        }

        public void UpdateBets(List<Player> players, Dictionary<string, List<Bet>> playerBets)
        {
            bettingListStore.Clear();

            foreach (var player in players)
            {
                var values = new List<string> { player.PlayerName };

                for (int round = 1; round <= 10; round++)
                {
                    var bet = playerBets.ContainsKey(player.PlayerName)
                        ? playerBets[player.PlayerName].FirstOrDefault(b => b.RoundNumber == round)
                        : null;

                    if (bet != null)
                    {
                        var horseInitial = bet.HorseName[0];
                        values.Add($"{horseInitial}\n{bet.BetAmount:F0}元");
                    }
                    else
                    {
                        values.Add("-");
                    }
                }

                bettingListStore.AppendValues(values.Cast<object>().ToArray());
            }
        }
    }
}
