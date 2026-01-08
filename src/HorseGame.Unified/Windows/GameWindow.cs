using Gtk;
using HorseGame.Shared;
using HorseGame.Unified.Components;
using HorseGame.Unified.Presenters;
using HorseGame.Unified.Services;

namespace HorseGame.Unified.Windows
{
    /// <summary>
    /// Main game window - clean layout orchestrator using component-based architecture
    /// </summary>
    public class GameWindow : Window
    {
        private readonly GamePresenter presenter;
        private readonly GameRepository repository;

        // UI Components
        private PlayerManagementPanel playerPanel = null!;
        private BettingHistoryPanel historyPanel = null!;
        private BettingFormPanel bettingPanel = null!;
        private RaceControlPanel controlPanel = null!;
        private RaceTracksPanel tracksPanel = null!;
        private ResultsPanel resultsPanel = null!;

        public GameWindow(GameSession session, GameRepository repository)
            : base($"HorseGame - {session.Game.GameName}")
        {
            this.repository = repository;
            this.presenter = new GamePresenter(session, repository);

            SetDefaultSize(1800, 1000);
            SetPosition(WindowPosition.Center);
            DeleteEvent += OnWindowDelete;

            InitializeComponents();
            WireUpEvents();
            BuildLayout();
            RefreshAllUI();

            ShowAll();
        }

        private void InitializeComponents()
        {
            // Create all UI components
            playerPanel = new PlayerManagementPanel();
            historyPanel = new BettingHistoryPanel();
            bettingPanel = new BettingFormPanel();
            controlPanel = new RaceControlPanel(presenter.Session.Game.Levels.Count);
            tracksPanel = new RaceTracksPanel();
            resultsPanel = new ResultsPanel();
        }

        private void WireUpEvents()
        {
            // Component -> Presenter
            playerPanel.PlayerAdded += OnPlayerAdded;
            playerPanel.PlayerDeleted += OnPlayerDeleted;
            playerPanel.ClueRequested += OnClueRequested;
            bettingPanel.BetPlaced += OnBetPlaced;
            controlPanel.PlayRequested += OnPlayRequested;
            controlPanel.ViewCluesRequested += OnViewCluesRequested;

            // Presenter -> UI
            presenter.PlayersChanged += RefreshPlayerData;
            presenter.BetsChanged += RefreshBettingData;
            presenter.CurrentRoundChanged += OnCurrentRoundChanged;
            presenter.ProgressUpdated += tracksPanel.UpdateProgress;
            presenter.TimesUpdated += tracksPanel.UpdateTimes;
            presenter.ScoresUpdated += tracksPanel.UpdateScores;
            presenter.RaceCompleted += resultsPanel.ShowResults;
        }

        private void BuildLayout()
        {
            var mainHBox = new HBox(false, 10);
            mainHBox.BorderWidth = 10;

            // LEFT COLUMN (500px)
            var leftVBox = new VBox(false, 10);
            leftVBox.SetSizeRequest(500, -1);

            leftVBox.PackStart(playerPanel, false, false, 0);
            leftVBox.PackStart(historyPanel, true, true, 0); // Expands

            mainHBox.PackStart(leftVBox, false, false, 0);

            // RIGHT COLUMN (expands)
            var rightVBox = new VBox(false, 10);

            // Game title
            var titleLabel = new Label();
            titleLabel.Markup = $"<b>{presenter.Session.Game.GameName}</b>";
            rightVBox.PackStart(titleLabel, false, false, 0);

            // Control panel + betting form in horizontal box
            var topRightBox = new HBox(false, 10);
            topRightBox.PackStart(controlPanel, false, false, 0);
            topRightBox.PackStart(bettingPanel, false, false, 0);
            rightVBox.PackStart(topRightBox, false, false, 0);

            // Race tracks
            rightVBox.PackStart(tracksPanel, false, false, 10);

            // Results (expands)
            rightVBox.PackStart(resultsPanel, true, true, 0);

            mainHBox.PackStart(rightVBox, true, true, 0);

            Add(mainHBox);
        }

        #region Event Handlers - Component to Presenter

        private void OnPlayerAdded(string playerName)
        {
            try
            {
                presenter.AddPlayer(playerName);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void OnPlayerDeleted(string playerName)
        {
            try
            {
                presenter.DeletePlayer(playerName);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void OnClueRequested(string playerName)
        {
            try
            {
                var player = presenter.Session.Players.FirstOrDefault(p => p.PlayerName == playerName);
                if (player == null) return;

                presenter.PurchaseClue(playerName);

                // Get the last purchased clue
                var lastClueIndex = player.PurchasedClueIndices.Last();
                var clueText = presenter.GetPurchasedClue(lastClueIndex);

                // Show clue dialog
                var dialog = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Info,
                    ButtonsType.Ok,
                    $"Clue purchased for {playerName} (2元):\n\n{clueText}\n\n(You can copy this)"
                );
                dialog.Run();
                dialog.Destroy();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void OnBetPlaced(string playerName, string horseName, decimal amount)
        {
            try
            {
                presenter.PlaceBet(playerName, horseName, amount);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private async void OnPlayRequested()
        {
            try
            {
                tracksPanel.ResetProgress();
                resultsPanel.Clear();
                await presenter.PlayRace(presenter.Session.CurrentRound);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void OnViewCluesRequested()
        {
            new CluesWindow(presenter.Session);
        }

        #endregion

        #region Event Handlers - Presenter to UI

        private void OnCurrentRoundChanged(int roundNumber)
        {
            controlPanel.UpdateCurrentRound(roundNumber);
            bettingPanel.UpdateCurrentRound(roundNumber + 1);
        }

        private void RefreshPlayerData()
        {
            var balances = new Dictionary<string, decimal>();
            foreach (var player in presenter.Session.Players)
            {
                balances[player.PlayerName] = presenter.CalculatePlayerBalance(player.PlayerName);
            }

            playerPanel.UpdatePlayers(presenter.Session.Players, balances);
            bettingPanel.UpdatePlayers(presenter.Session.Players.Select(p => p.PlayerName).ToList());
        }

        private void RefreshBettingData()
        {
            historyPanel.UpdateBets(presenter.Session.Players, presenter.Session.PlayerBets);
        }

        private void RefreshAllUI()
        {
            RefreshPlayerData();
            RefreshBettingData();
            OnCurrentRoundChanged(presenter.Session.CurrentRound);
        }

        #endregion

        #region Utilities

        private void ShowError(string message)
        {
            var dialog = new MessageDialog(
                this,
                DialogFlags.Modal,
                MessageType.Warning,
                ButtonsType.Ok,
                message
            );
            dialog.Run();
            dialog.Destroy();
        }

        private void OnWindowDelete(object sender, DeleteEventArgs args)
        {
            // Save on close
            repository.SaveGameSession(presenter.Session);
        }

        #endregion
    }
}
