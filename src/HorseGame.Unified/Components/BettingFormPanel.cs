#pragma warning disable CS0612
using Gtk;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for placing new bets
    /// </summary>
    public class BettingFormPanel : VBox
    {
        private ComboBoxText playerCombo;
        private ComboBoxText horseCombo;
        private Entry amountEntry;
        private Label roundLabel;

        // Events
        public event Action<string, string, decimal>? BetPlaced;

        public BettingFormPanel() : base(false, 10)
        {
            // Title
            var titleLabel = new Label();
            titleLabel.Markup = "<b>💵 Place Bet</b>";
            PackStart(titleLabel, false, false, 0);

            // Bet form grid
            var grid = new Table(3, 2, false);
            grid.RowSpacing = 5;
            grid.ColumnSpacing = 5;

            // Player
            grid.Attach(new Label("Player:") { Xalign = 0 }, 0, 1, 0, 1);
            playerCombo = new ComboBoxText();
            grid.Attach(playerCombo, 1, 2, 0, 1);

            // Horse
            grid.Attach(new Label("Horse:") { Xalign = 0 }, 0, 1, 1, 2);
            horseCombo = new ComboBoxText();
            horseCombo.AppendText("Gryffindor");
            horseCombo.AppendText("Hufflepuff");
            horseCombo.AppendText("Ravenclaw");
            horseCombo.AppendText("Slytherin");
            grid.Attach(horseCombo, 1, 2, 1, 2);

            // Amount
            grid.Attach(new Label("Amount:") { Xalign = 0 }, 0, 1, 2, 3);
            amountEntry = new Entry();
            amountEntry.PlaceholderText = "Bet amount...";
            amountEntry.Activated += OnPlaceBetClicked;
            grid.Attach(amountEntry, 1, 2, 2, 3);

            PackStart(grid, false, false, 0);

            // Current round label
            roundLabel = new Label();
            roundLabel.Markup = "<i>Betting for Round 1</i>";
            PackStart(roundLabel, false, false, 0);

            // Place Bet button
            var placeBetButton = new Button("Place Bet");
            placeBetButton.Clicked += OnPlaceBetClicked;
            PackStart(placeBetButton, false, false, 0);
        }

        public void UpdatePlayers(List<string> playerNames)
        {
            playerCombo.RemoveAll();
            foreach (var name in playerNames)
            {
                playerCombo.AppendText(name);
            }
        }

        public void UpdateCurrentRound(int roundNumber)
        {
            roundLabel.Markup = $"<i>Betting for Round {roundNumber}</i>";
        }

        private void OnPlaceBetClicked(object? sender, EventArgs e)
        {
            if (playerCombo.Active == -1 || horseCombo.Active == -1 || string.IsNullOrWhiteSpace(amountEntry.Text))
                return;

            if (decimal.TryParse(amountEntry.Text, out var amount) && amount > 0)
            {
                BetPlaced?.Invoke(playerCombo.ActiveText, horseCombo.ActiveText, amount);
                amountEntry.Text = "";
                horseCombo.Active = -1;
            }
        }
    }
}
