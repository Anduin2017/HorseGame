using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gtk;
using HorseGame.Shared;
using HorseGame.Unified.Services;

namespace HorseGame.Unified.Windows
{
    public class GameWindow : Window
    {
        private readonly GameSession session;
        private readonly GameRepository repository;
        private readonly HorseEvaluator horseEvaluator;
        private readonly OvertakeEvaluator overtakeEvaluator;
        private readonly ProgressMadeMeasurer measurer;

        // UI Controls - Left Panel (Player Management)
        private Entry playerNameEntry;
        private TreeView playersTreeView;
        private ListStore playersListStore;

        // Betting status table
        private TreeView bettingStatusTreeView;
        private ListStore bettingStatusListStore;

        // Betting controls  
        private ComboBoxText betPlayerCombo;
        private ComboBoxText betHorseCombo;
        private Entry betAmountEntry;
        private Label currentRoundLabel;

        // UI Controls - Right Panel (Game Playback)
        private ComboBoxText roundsCombo;
        private Button playButton;
        private ProgressBar gryffindorProgress;
        private ProgressBar hufflepuffProgress;
        private ProgressBar ravenclawProgress;
        private ProgressBar slytherinProgress;
        private Label gTime;
        private Label hTime;
        private Label rTime;
        private Label sTime;
        private Label gScore;
        private Label hScore;
        private Label rScore;
        private Label sScore;
        private Label resultLabel;

        public GameWindow(GameSession session, GameRepository repository) : base($"HorseGame - {session.Game.GameName}")
        {
            this.session = session;
            this.repository = repository;
            this.horseEvaluator = new HorseEvaluator();
            this.overtakeEvaluator = new OvertakeEvaluator();
            this.measurer = new ProgressMadeMeasurer();

            SetDefaultSize(1800, 1000);
            SetPosition(WindowPosition.Center);
            DeleteEvent += OnWindowDelete;

            var mainHBox = new HBox(false, 10);
            mainHBox.BorderWidth = 10;

            // Left Panel - Player Management 
            var leftVBox = new VBox(false, 10);
            leftVBox.SetSizeRequest(650, -1);

            var playerLabel = new Label();
            playerLabel.Markup = "<b>Player Management</b>";
            leftVBox.PackStart(playerLabel, false, false, 0);

            // Player entry and buttons
            var playerEntryBox = new HBox(false, 5);
            playerNameEntry = new Entry();
            playerNameEntry.PlaceholderText = "Player name...";
            playerEntryBox.PackStart(playerNameEntry, true, true, 0);

            var addPlayerButton = new Button("Add Player");
            addPlayerButton.Clicked += OnAddPlayer;
            playerEntryBox.PackStart(addPlayerButton, false, false, 0);

            var deletePlayerButton = new Button("Delete Selected");
            deletePlayerButton.Clicked += OnDeletePlayer;
            playerEntryBox.PackStart(deletePlayerButton, false, false, 0);

            leftVBox.PackStart(playerEntryBox, false, false, 0);

            // Players TreeView (showing player, balance, purchased clues count)
            var scrolled = new ScrolledWindow();
            scrolled.SetSizeRequest(-1, 180);
            playersListStore = new ListStore(typeof(string), typeof(string), typeof(string)); // Name, Balance, Clues
            playersTreeView = new TreeView(playersListStore);

            var nameCol = new TreeViewColumn();
            nameCol.Title = "Player";
            var nameCell = new CellRendererText();
            nameCol.PackStart(nameCell, true);
            nameCol.AddAttribute(nameCell, "text", 0);
            playersTreeView.AppendColumn(nameCol);

            var balanceCol = new TreeViewColumn();
            balanceCol.Title = "Balance";
            var balanceCell = new CellRendererText();
            balanceCol.PackStart(balanceCell, true);
            balanceCol.AddAttribute(balanceCell, "text", 1);
            playersTreeView.AppendColumn(balanceCol);

            var cluesCol = new TreeViewColumn();
            cluesCol.Title = "Clues Bought";
            var cluesCell = new CellRendererText();
            cluesCol.PackStart(cluesCell, true);
            cluesCol.AddAttribute(cluesCell, "text", 2);
            playersTreeView.AppendColumn(cluesCol);

            scrolled.Add(playersTreeView);
            leftVBox.PackStart(scrolled, false, false, 0);

            // Buy Clue Button
            var buyClueButton = new Button("Buy Clue for Selected Player (2元)");
            buyClueButton.Clicked += OnBuyClue;
            leftVBox.PackStart(buyClueButton, false, false, 0);

            // Betting Status Table
            var bettingStatusLabel = new Label();
            bettingStatusLabel.Markup = "<b>Betting Status (10 Rounds)</b>";
            leftVBox.PackStart(bettingStatusLabel, false, false, 10);

            var bettingScrolled = new ScrolledWindow();
            bettingScrolled.SetSizeRequest(-1, 200);
            // ListStore: Player name + 10 round bet info (string for each round)
            bettingStatusListStore = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string),
                typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
            bettingStatusTreeView = new TreeView(bettingStatusListStore);

            // Player name column
            var playerCol = new TreeViewColumn();
            playerCol.Title = "Player";
            var playerCell = new CellRendererText();
            playerCol.PackStart(playerCell, true);
            playerCol.AddAttribute(playerCell, "text", 0);
            bettingStatusTreeView.AppendColumn(playerCol);

            // 10 round columns
            for (int i = 0; i < 10; i++)
            {
                var roundCol = new TreeViewColumn();
                roundCol.Title = $"R{i + 1}";
                var roundCell = new CellRendererText();
                roundCell.WrapWidth = 80;
                roundCell.WrapMode = Pango.WrapMode.Word;
                roundCol.PackStart(roundCell, true);
                roundCol.AddAttribute(roundCell, "text", i + 1);
                bettingStatusTreeView.AppendColumn(roundCol);
            }

            bettingScrolled.Add(bettingStatusTreeView);
            leftVBox.PackStart(bettingScrolled, false, false, 0);

            // Betting Section
            var bettingLabel = new Label();
            bettingLabel.Markup = "<b>Place New Bet</b>";
            leftVBox.PackStart(bettingLabel, false, false, 10);

            var betGrid = new Table(3, 2, false);
            betGrid.RowSpacing = 5;
            betGrid.ColumnSpacing = 5;

            betGrid.Attach(new Label("Player:") { Xalign = 0 }, 0, 1, 0, 1);
            betPlayerCombo = new ComboBoxText();
            betGrid.Attach(betPlayerCombo, 1, 2, 0, 1);

            betGrid.Attach(new Label("Horse:") { Xalign = 0 }, 0, 1, 1, 2);
            betHorseCombo = new ComboBoxText();
            betHorseCombo.AppendText("Gryffindor");
            betHorseCombo.AppendText("Hufflepuff");
            betHorseCombo.AppendText("Ravenclaw");
            betHorseCombo.AppendText("Slytherin");
            betGrid.Attach(betHorseCombo, 1, 2, 1, 2);

            betGrid.Attach(new Label("Amount:") { Xalign = 0 }, 0, 1, 2, 3);
            betAmountEntry = new Entry();
            betAmountEntry.PlaceholderText = "Bet amount...";
            betGrid.Attach(betAmountEntry, 1, 2, 2, 3);

            leftVBox.PackStart(betGrid, false, false, 0);

            currentRoundLabel = new Label();
            currentRoundLabel.Markup = "<i>Betting for current round</i>";
            leftVBox.PackStart(currentRoundLabel, false, false, 0);

            var placeBetButton = new Button("Place Bet for Current Round");
            placeBetButton.Clicked += OnPlaceBet;
            leftVBox.PackStart(placeBetButton, false, false, 0);

            // View Clues Button (opens separate window)
            var viewCluesButton = new Button("📋 View All Clues");
            viewCluesButton.Clicked += OnViewClues;
            leftVBox.PackStart(viewCluesButton, false, false, 10);

            mainHBox.PackStart(leftVBox, false, false, 0);

            // Right Panel - Game Playback 
            var rightVBox = new VBox(false, 10);

            var gameLabel = new Label();
            gameLabel.Markup = $"<b>{session.Game.GameName}</b>";
            rightVBox.PackStart(gameLabel, false, false, 0);

            // Round selection and play button
            var controlBox = new HBox(false, 10);
            roundsCombo = new ComboBoxText();
            for (int i = 0; i < session.Game.Levels.Count; i++)
            {
                roundsCombo.AppendText($"Round {i + 1}");
            }
            roundsCombo.Active = session.CurrentRound;
            roundsCombo.Sensitive = false; // DISABLE - prevent user from switching rounds manually
            controlBox.PackStart(new Label("Current Round:"), false, false, 0);
            controlBox.PackStart(roundsCombo, true, true, 0);

            playButton = new Button("Play Race");
            playButton.Clicked += OnPlayRound;
            controlBox.PackStart(playButton, false, false, 0);

            rightVBox.PackStart(controlBox, false, false, 0);

            // Race tracks
            var tracksBox = new VBox(false, 15);
            tracksBox.BorderWidth = 20;

            AddHorseTrack(tracksBox, "Gryffindor", out gryffindorProgress, out gTime, out gScore);
            AddHorseTrack(tracksBox, "Hufflepuff", out hufflepuffProgress, out hTime, out hScore);
            AddHorseTrack(tracksBox, "Ravenclaw", out ravenclawProgress, out rTime, out rScore);
            AddHorseTrack(tracksBox, "Slytherin", out slytherinProgress, out sTime, out sScore);

            rightVBox.PackStart(tracksBox, true, true, 0);

            // Result label
            resultLabel = new Label("");
            resultLabel.SetSizeRequest(-1, 100);
            rightVBox.PackStart(resultLabel, false, false, 0);

            mainHBox.PackStart(rightVBox, true, true, 0);

            Add(mainHBox);

            RefreshPlayersList();
            RefreshPlayerCombo();
            RefreshBettingStatusTable();
            UpdateCurrentRoundLabel();
            LoadClues();
        }

        private void AddHorseTrack(VBox container, string name, out ProgressBar progress, out Label timeLabel, out Label scoreLabel)
        {
            var hbox = new HBox(false, 10);

            var nameLabel = new Label(name);
            nameLabel.SetSizeRequest(100, -1);
            hbox.PackStart(nameLabel, false, false, 0);

            progress = new ProgressBar();
            progress.SetSizeRequest(500, 35);
            hbox.PackStart(progress, true, true, 0);

            timeLabel = new Label("");
            timeLabel.SetSizeRequest(60, -1);
            hbox.PackStart(timeLabel, false, false, 0);

            scoreLabel = new Label("0");
            scoreLabel.SetSizeRequest(60, -1);
            scoreLabel.Markup = "<span size='18000' weight='bold'>0</span>";
            hbox.PackStart(scoreLabel, false, false, 0);

            container.PackStart(hbox, false, false, 0);
        }

        private void LoadClues()
        {
            // Clues stored in session.Game.Clues, accessed via View Clues button
        }

        private void OnViewClues(object? sender, EventArgs e)
        {
            var cluesWindow = new Window("All Clues");
            cluesWindow.SetDefaultSize(800, 600);
            cluesWindow.SetPosition(WindowPosition.Center);

            var vbox = new VBox(false, 10);
            vbox.BorderWidth = 15;

            var titleLabel = new Label();
            titleLabel.Markup = "<span size='16000' weight='bold'>📋 All Generated Clues</span>";
            vbox.PackStart(titleLabel, false, false, 0);

            var scrolled = new ScrolledWindow();
            var textView = new TextView();
            textView.Editable = false;
            textView.WrapMode = WrapMode.Word;
            textView.Buffer.Text = string.Join("\n\n", session.Game.Clues.Select((c, i) => $"{i + 1}. {c}"));
            scrolled.Add(textView);
            vbox.PackStart(scrolled, true, true, 0);

            var closeButton = new Button("Close");
            closeButton.Clicked += (s, ev) => cluesWindow.Destroy();
            vbox.PackStart(closeButton, false, false, 0);

            cluesWindow.Add(vbox);
            cluesWindow.ShowAll();
        }

        private decimal CalculatePlayerCurrentBalance(Player player)
        {
            decimal balance = player.InitialBalance;

            // Add income for each completed round (1元 for round 1, 2元 for round 2, etc.)
            for (int r = 1; r <= session.CurrentRound + 1; r++)
            {
                balance += r;
            }

            // Deduct clue purchases 
            balance -= player.PurchasedClueIndices.Count * 2;

            // Calculate betting results
            if (session.PlayerBets.ContainsKey(player.PlayerName))
            {
                foreach (var bet in session.PlayerBets[player.PlayerName])
                {
                    balance -= bet.BetAmount;
                    balance += bet.Payout;
                }
            }

            return balance;
        }

        private void RefreshPlayersList()
        {
            playersListStore.Clear();
            foreach (var player in session.Players)
            {
                var balance = CalculatePlayerCurrentBalance(player);
                playersListStore.AppendValues(
                    player.PlayerName,
                    $"{balance:F1}元",
                    player.PurchasedClueIndices.Count.ToString()
                );
            }
        }

        private void RefreshPlayerCombo()
        {
            betPlayerCombo.RemoveAll();
            foreach (var player in session.Players)
            {
                betPlayerCombo.AppendText(player.PlayerName);
            }
        }

        private void RefreshBettingStatusTable()
        {
            bettingStatusListStore.Clear();

            foreach (var player in session.Players)
            {
                var rowData = new List<string> { player.PlayerName };

                // For each of the 10 rounds
                for (int round = 1; round <= 10; round++)
                {
                    var bet = session.PlayerBets.ContainsKey(player.PlayerName)
                        ? session.PlayerBets[player.PlayerName].FirstOrDefault(b => b.RoundNumber == round)
                        : null;

                    if (bet != null)
                    {
                        // Show horse name and bet amount
                        rowData.Add($"{bet.HorseName.Substring(0, 1)}\n{bet.BetAmount}元");
                    }
                    else
                    {
                        rowData.Add("-");
                    }
                }

                bettingStatusListStore.AppendValues(rowData.ToArray());
            }
        }

        private void OnAddPlayer(object? sender, EventArgs e)
        {
            var playerName = playerNameEntry.Text.Trim();
            if (string.IsNullOrWhiteSpace(playerName))
                return;

            if (session.Players.Any(p => p.PlayerName == playerName))
            {
                var md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Warning,
                    ButtonsType.Ok,
                    "Player already exists!");
                md.Run();
                md.Destroy();
                return;
            }

            session.Players.Add(new Player { PlayerName = playerName });
            session.PlayerBets[playerName] = new List<Bet>();
            RefreshPlayersList();
            RefreshPlayerCombo();
            RefreshBettingStatusTable();
            playerNameEntry.Text = "";

            repository.SaveGameSession(session);
        }

        private void OnDeletePlayer(object? sender, EventArgs e)
        {
            var selection = playersTreeView.Selection;
            if (selection.GetSelected(out var model, out var iter))
            {
                var playerName = (string)model.GetValue(iter, 0);
                var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);

                if (player != null)
                {
                    session.Players.Remove(player);
                    session.PlayerBets.Remove(playerName);
                    RefreshPlayersList();
                    RefreshPlayerCombo();
                    RefreshBettingStatusTable();
                    repository.SaveGameSession(session);
                }
            }
        }

        private void OnBuyClue(object? sender, EventArgs e)
        {
            var selection = playersTreeView.Selection;
            if (!selection.GetSelected(out var model, out var iter))
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    "Please select a player first!");
                md.Run();
                md.Destroy();
                return;
            }

            var playerName = (string)model.GetValue(iter, 0);
            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);

            if (player == null) return;

            // Check balance
            var currentBalance = CalculatePlayerCurrentBalance(player);
            if (currentBalance < 2)
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    $"{playerName} doesn't have enough balance! Current: {currentBalance:F1}元");
                md.Run();
                md.Destroy();
                return;
            }

            // Find an unpurchased clue
            var availableClues = Enumerable.Range(0, session.Game.Clues.Count)
                .Where(i => !player.PurchasedClueIndices.Contains(i))
                .ToList();

            if (availableClues.Count == 0)
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                    "All clues have been purchased by this player!");
                md.Run();
                md.Destroy();
                return;
            }

            // Pick a random clue
            var random = new Random();
            var clueIndex = availableClues[random.Next(availableClues.Count)];
            player.PurchasedClueIndices.Add(clueIndex);

            // Show the clue
            var clue = session.Game.Clues[clueIndex];
            var clueDialog = new MessageDialog(
                this,
                DialogFlags.Modal,
                MessageType.Info,
                ButtonsType.Ok,
                false,
                $"Clue purchased for {playerName} (2元 deducted):\n\n{clue}\n\n(You can copy this clue)");
            clueDialog.Run();
            clueDialog.Destroy();

            RefreshPlayersList();
            repository.SaveGameSession(session);
        }

        private void UpdateCurrentRoundLabel()
        {
            currentRoundLabel.Markup = $"<i>Betting for Round {session.CurrentRound + 1}</i>";
        }

        private void OnPlaceBet(object? sender, EventArgs e)
        {
            if (betPlayerCombo.Active == -1 ||
                betHorseCombo.Active == -1 || string.IsNullOrWhiteSpace(betAmountEntry.Text))
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    "Please fill in all betting fields!");
                md.Run();
                md.Destroy();
                return;
            }

            var playerName = betPlayerCombo.ActiveText;
            var roundNumber = session.CurrentRound + 1; // Use current round
            var horseName = betHorseCombo.ActiveText;

            if (!decimal.TryParse(betAmountEntry.Text, out var betAmount) || betAmount <= 0)
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    "Invalid bet amount!");
                md.Run();
                md.Destroy();
                return;
            }

            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null) return;

            // Check if already bet on this round
            if (session.PlayerBets[playerName].Any(b => b.RoundNumber == roundNumber))
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    $"Player has already placed a bet for round {roundNumber}!");
                md.Run();
                md.Destroy();
                return;
            }

            // Check balance
            var currentBalance = CalculatePlayerCurrentBalance(player);
            if (currentBalance < betAmount)
            {
                var md = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Ok,
                    $"Insufficient balance! Current: {currentBalance:F1}元, Needed: {betAmount}元");
                md.Run();
                md.Destroy();
                return;
            }

            var bet = new Bet
            {
                RoundNumber = roundNumber,
                HorseName = horseName,
                BetAmount = betAmount,
                Payout = 0 // Will be calculated when round is played
            };

            session.PlayerBets[playerName].Add(bet);

            var successDialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
                $"Bet placed!\n{playerName} bets {betAmount}元 on {horseName} for Round {roundNumber}");
            successDialog.Run();
            successDialog.Destroy();

            betAmountEntry.Text = "";
            RefreshPlayersList();
            RefreshBettingStatusTable();
            repository.SaveGameSession(session);
        }

        private async void OnPlayRound(object? sender, EventArgs e)
        {
            if (roundsCombo.Active == -1) return;

            var selectedIndex = roundsCombo.Active;
            var level = session.Game.Levels[selectedIndex];
            var scores = new List<string>();
            var stopWatch = new Stopwatch();

            gryffindorProgress.Fraction = 0;
            hufflepuffProgress.Fraction = 0;
            ravenclawProgress.Fraction = 0;
            slytherinProgress.Fraction = 0;
            gTime.Text = string.Empty;
            hTime.Text = string.Empty;
            rTime.Text = string.Empty;
            sTime.Text = string.Empty;

            stopWatch.Start();
            while (
                gryffindorProgress.Fraction < 1 ||
                hufflepuffProgress.Fraction < 1 ||
                ravenclawProgress.Fraction < 1 ||
                slytherinProgress.Fraction < 1)
            {
                await Task.Delay(1);
                var elapsed = stopWatch.Elapsed.TotalSeconds;

                gryffindorProgress.Fraction = Math.Min(1.0, measurer.GetProgress(level.GryffindorSpeeds, elapsed) / 100.0);
                hufflepuffProgress.Fraction = Math.Min(1.0, measurer.GetProgress(level.HufflepuffSpeeds, elapsed) / 100.0);
                ravenclawProgress.Fraction = Math.Min(1.0, measurer.GetProgress(level.RavenclawSpeeds, elapsed) / 100.0);
                slytherinProgress.Fraction = Math.Min(1.0, measurer.GetProgress(level.SlytherinSpeeds, elapsed) / 100.0);

                if (gryffindorProgress.Fraction >= 1 && !scores.Any(t => t.Contains("Gryffindor")))
                {
                    scores.Add("Gryffindor");
                }
                if (hufflepuffProgress.Fraction >= 1 && !scores.Any(t => t.Contains("Hufflepuff")))
                {
                    scores.Add("Hufflepuff");
                }
                if (ravenclawProgress.Fraction >= 1 && !scores.Any(t => t.Contains("Ravenclaw")))
                {
                    scores.Add("Ravenclaw");
                }
                if (slytherinProgress.Fraction >= 1 && !scores.Any(t => t.Contains("Slytherin")))
                {
                    scores.Add("Slytherin");
                }
                // Simple text update during animation
                resultLabel.Text = string.Join('\n', scores);
            }
            stopWatch.Stop();

            // Beautify result display AFTER race completes
            var medals = new[] { "🏆", "🥈", "🥉", "4th" };
            var formattedResults = new List<string>();
            for (int i = 0; i < scores.Count; i++)
            {
                formattedResults.Add($"{medals[i]} {scores[i]}");
            }
            resultLabel.Markup = $"<span size='14000' weight='bold'>\n{string.Join("\n", formattedResults)}</span>";

            gTime.Text = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds).ToString("F2");
            hTime.Text = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds).ToString("F2");
            rTime.Text = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds).ToString("F2");
            sTime.Text = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds).ToString("F2");

            // Calculate cumulative scores FIRST (needed for round 10 logic)
            int gryffindorScore = 0;
            int ravenclawScore = 0;
            int hufflepuffScore = 0;
            int slytherinScore = 0;

            for (int i = 0; i <= selectedIndex; i++)
            {
                var previousLevel = session.Game.Levels[i];
                var gryffindorTime = horseEvaluator.EvaluatorTime(previousLevel.GryffindorSpeeds);
                var ravenclawTime = horseEvaluator.EvaluatorTime(previousLevel.RavenclawSpeeds);
                var hufflepuffTime = horseEvaluator.EvaluatorTime(previousLevel.HufflepuffSpeeds);
                var slytherinTime = horseEvaluator.EvaluatorTime(previousLevel.SlytherinSpeeds);

                var scoresList = new List<double>
                {
                    gryffindorTime,
                    ravenclawTime,
                    hufflepuffTime,
                    slytherinTime
                }.OrderBy(t => t).ToArray();

                gryffindorScore += overtakeEvaluator.GetScoreBasedOnTimeChart(scoresList, gryffindorTime);
                ravenclawScore += overtakeEvaluator.GetScoreBasedOnTimeChart(scoresList, ravenclawTime);
                hufflepuffScore += overtakeEvaluator.GetScoreBasedOnTimeChart(scoresList, hufflepuffTime);
                slytherinScore += overtakeEvaluator.GetScoreBasedOnTimeChart(scoresList, slytherinTime);
            }

            gScore.Markup = $"<span size='18000' weight='bold'>{gryffindorScore}</span>";
            rScore.Markup = $"<span size='18000' weight='bold'>{ravenclawScore}</span>";
            hScore.Markup = $"<span size='18000' weight='bold'>{hufflepuffScore}</span>";
            sScore.Markup = $"<span size='18000' weight='bold'>{slytherinScore}</span>";

            // Calculate betting payouts - DIFFERENT logic for round 10
            var roundNumber = selectedIndex + 1;
            Dictionary<string, int> horsePositions;

            if (roundNumber == 10)
            {
                // Round 10: Use TOTAL SCORE ranking instead of race position
                var scoreRanking = new List<(string Horse, int Score)>
                {
                    ("Gryffindor", gryffindorScore),
                    ("Hufflepuff", hufflepuffScore),
                    ("Ravenclaw", ravenclawScore),
                    ("Slytherin", slytherinScore)
                }.OrderByDescending(x => x.Score).ToList();

                horsePositions = new Dictionary<string, int>();
                for (int i = 0; i < scoreRanking.Count; i++)
                {
                    horsePositions[scoreRanking[i].Horse] = i + 1;
                }
            }
            else
            {
                // Rounds 1-9: Use race position from this round
                horsePositions = new Dictionary<string, int>
                {
                    ["Gryffindor"] = scores.IndexOf("Gryffindor") + 1,
                    ["Hufflepuff"] = scores.IndexOf("Hufflepuff") + 1,
                    ["Ravenclaw"] = scores.IndexOf("Ravenclaw") + 1,
                    ["Slytherin"] = scores.IndexOf("Slytherin") + 1
                };
            }

            foreach (var playerBets in session.PlayerBets.Values)
            {
                var bet = playerBets.FirstOrDefault(b => b.RoundNumber == roundNumber && b.Payout == 0);
                if (bet != null && horsePositions.ContainsKey(bet.HorseName))
                {
                    var position = horsePositions[bet.HorseName];
                    bet.Payout = session.CalculatePayout(bet.BetAmount, position, roundNumber);
                }
            }

            // Update current round
            var playedRound = selectedIndex;
            if (playedRound == session.CurrentRound && session.CurrentRound < session.Game.Levels.Count - 1)
            {
                session.CurrentRound++;
                roundsCombo.Active = session.CurrentRound;
                UpdateCurrentRoundLabel();
            }

            RefreshPlayersList();
            RefreshBettingStatusTable();
            repository.SaveGameSession(session);
        }

        private void OnWindowDelete(object sender, DeleteEventArgs args)
        {
            repository.SaveGameSession(session);
        }
    }
}
