#pragma warning disable CS0612
using Gtk;
using HorseGame.Shared;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for player management (CRUD operations)
    /// </summary>
    public class PlayerManagementPanel : VBox
    {
        private Entry playerNameEntry;
        private TreeView playersTreeView;
        private ListStore playersListStore;

        // Events
        public event Action<string>? PlayerAdded;
        public event Action<string>? PlayerDeleted;
        public event Action<string>? ClueRequested;

        public string? SelectedPlayerName { get; private set; }

        public PlayerManagementPanel() : base(false, 10)
        {
            // Title
            var titleLabel = new Label();
            titleLabel.Markup = "<b>💰 Players & Balance</b>";
            PackStart(titleLabel, false, false, 0);

            // Entry and buttons
            var entryBox = new HBox(false, 5);
            playerNameEntry = new Entry();
            playerNameEntry.PlaceholderText = "Player name...";
            playerNameEntry.Activated += OnAddPlayerClicked;
            entryBox.PackStart(playerNameEntry, true, true, 0);

            var addButton = new Button("Add");
            addButton.Clicked += OnAddPlayerClicked;
            entryBox.PackStart(addButton, false, false, 0);

            var deleteButton = new Button("Delete");
            deleteButton.Clicked += OnDeletePlayerClicked;
            entryBox.PackStart(deleteButton, false, false, 0);

            PackStart(entryBox, false, false, 0);

            // TreeView
            var scrolled = new ScrolledWindow();
            scrolled.SetSizeRequest(-1, 200);

            playersListStore = new ListStore(typeof(string), typeof(string), typeof(string));
            playersTreeView = new TreeView(playersListStore);
            playersTreeView.Selection.Changed += OnSelectionChanged;

            var nameCol = new TreeViewColumn { Title = "Player" };
            var nameCell = new CellRendererText();
            nameCol.PackStart(nameCell, true);
            nameCol.AddAttribute(nameCell, "text", 0);
            playersTreeView.AppendColumn(nameCol);

            var balanceCol = new TreeViewColumn { Title = "Balance" };
            var balanceCell = new CellRendererText();
            balanceCol.PackStart(balanceCell, true);
            balanceCol.AddAttribute(balanceCell, "text", 1);
            playersTreeView.AppendColumn(balanceCol);

            var cluesCol = new TreeViewColumn { Title = "Clues" };
            var cluesCell = new CellRendererText();
            cluesCol.PackStart(cluesCell, true);
            cluesCol.AddAttribute(cluesCell, "text", 2);
            playersTreeView.AppendColumn(cluesCol);

            scrolled.Add(playersTreeView);
            PackStart(scrolled, false, false, 0);

            // Buy Clue button
            var buyClueButton = new Button("🎫 Buy Clue (2元)");
            buyClueButton.Clicked += OnBuyClueClicked;
            PackStart(buyClueButton, false, false, 5);
        }

        public void UpdatePlayers(List<Player> players, Dictionary<string, decimal> balances)
        {
            playersListStore.Clear();
            foreach (var player in players)
            {
                var balance = balances.ContainsKey(player.PlayerName) ? balances[player.PlayerName] : 0;
                playersListStore.AppendValues(
                    player.PlayerName,
                    $"{balance:F1}元",
                    player.PurchasedClueIndices.Count.ToString()
                );
            }
        }

        private void OnSelectionChanged(object? sender, EventArgs e)
        {
            if (playersTreeView.Selection.GetSelected(out var model, out var iter))
            {
                SelectedPlayerName = (string)model.GetValue(iter, 0);
            }
            else
            {
                SelectedPlayerName = null;
            }
        }

        private void OnAddPlayerClicked(object? sender, EventArgs e)
        {
            var name = playerNameEntry.Text.Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                PlayerAdded?.Invoke(name);
                playerNameEntry.Text = "";
            }
        }

        private void OnDeletePlayerClicked(object? sender, EventArgs e)
        {
            if (SelectedPlayerName != null)
            {
                PlayerDeleted?.Invoke(SelectedPlayerName);
            }
        }

        private void OnBuyClueClicked(object? sender, EventArgs e)
        {
            if (SelectedPlayerName != null)
            {
                ClueRequested?.Invoke(SelectedPlayerName);
            }
        }
    }
}
