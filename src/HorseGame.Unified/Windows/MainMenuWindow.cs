
#pragma warning disable
using Gtk;
using HorseGame.Unified.Services;

namespace HorseGame.Unified.Windows
{
    public class MainMenuWindow : Window
    {
        private readonly GameRepository repository;
        private TreeView gamesTreeView;
        private ListStore gamesListStore;

        public MainMenuWindow() : base("HorseGame - Main Menu")
        {
            repository = new GameRepository();

            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);
            DeleteEvent += (_,_) => Application.Quit();

            var vbox = new VBox(false, 10);
            vbox.BorderWidth = 10;

            // Title
            var titleLabel = new Label();
            titleLabel.Markup = "<span size='x-large' weight='bold'>Horse Game</span>";
            vbox.PackStart(titleLabel, false, false, 10);

            // Games list
            var scrolledWindow = new ScrolledWindow();
            scrolledWindow.SetSizeRequest(760, 400);

            // Create TreeView with columns: Game Name, Created At
            gamesListStore = new ListStore(typeof(string), typeof(string), typeof(string)); // GameId, Name, Created
            gamesTreeView = new TreeView(gamesListStore);

            var nameColumn = new TreeViewColumn();
            nameColumn.Title = "Game Name";
            var nameCell = new CellRendererText();
            nameColumn.PackStart(nameCell, true);
            nameColumn.AddAttribute(nameCell, "text", 1);
            gamesTreeView.AppendColumn(nameColumn);

            var dateColumn = new TreeViewColumn();
            dateColumn.Title = "Created At";
            var dateCell = new CellRendererText();
            dateColumn.PackStart(dateCell, true);
            dateColumn.AddAttribute(dateCell, "text", 2);
            gamesTreeView.AppendColumn(dateColumn);

            scrolledWindow.Add(gamesTreeView);
            vbox.PackStart(scrolledWindow, true, true, 0);

            // Buttons
            var buttonBox = new HBox(true, 10);

            var createButton = new Button("Create New Game");
            createButton.Clicked += OnCreateNewGame;
            buttonBox.PackStart(createButton, true, true, 0);

            var replayButton = new Button("Replay Selected Game");
            replayButton.Clicked += OnReplayGame;
            buttonBox.PackStart(replayButton, true, true, 0);

            var deleteButton = new Button("Delete Selected Game");
            deleteButton.Clicked += OnDeleteGame;
            buttonBox.PackStart(deleteButton, true, true, 0);

            vbox.PackStart(buttonBox, false, false, 0);

            Add(vbox);

            RefreshGamesList();
        }

        private void RefreshGamesList()
        {
            gamesListStore.Clear();
            var games = repository.ListAllGames();

            foreach (var game in games)
            {
                gamesListStore.AppendValues(
                    game.GameId,
                    game.GameName,
                    game.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                );
            }
        }

        private void OnCreateNewGame(object? sender, EventArgs e)
        {
            var dialog = new NewGameDialog(this, repository);
            dialog.ShowAll();
            dialog.Modal = true;
            var response = dialog.Run();

            if (response == (int)ResponseType.Ok)
            {
                RefreshGamesList();
            }

            dialog.Destroy();
        }

        private void OnReplayGame(object? sender, EventArgs e)
        {
            var selection = gamesTreeView.Selection;
            if (selection.GetSelected(out var model, out var iter))
            {
                var gameId = (string)model.GetValue(iter, 0);
                var session = repository.LoadGameSession(gameId);

                if (session != null)
                {
                    var gameWindow = new GameWindow(session, repository);
                    gameWindow.ShowAll();
                }
            }
            else
            {
                var md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Warning,
                    ButtonsType.Ok,
                    "Please select a game to replay.");
                md.Run();
                md.Destroy();
            }
        }

        private void OnDeleteGame(object? sender, EventArgs e)
        {
            var selection = gamesTreeView.Selection;
            if (selection.GetSelected(out var model, out var iter))
            {
                var gameId = (string)model.GetValue(iter, 0);
                var gameName = (string)model.GetValue(iter, 1);

                var md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Question,
                    ButtonsType.YesNo,
                    $"Are you sure you want to delete '{gameName}'?");

                var response = md.Run();
                md.Destroy();

                if (response == (int)ResponseType.Yes)
                {
                    repository.DeleteGame(gameId);
                    RefreshGamesList();
                }
            }
            else
            {
                var md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Warning,
                    ButtonsType.Ok,
                    "Please select a game to delete.");
                md.Run();
                md.Destroy();
            }
        }
    }
}
