using System;
using Gtk;
using HorseGame.Shared;
using HorseGame.Unified.Services;

namespace HorseGame.Unified.Windows
{
    public class NewGameDialog : Dialog
    {
        private readonly GameRepository repository;
        private Entry gameNameEntry;

        public NewGameDialog(Window parent, GameRepository repository) : base("Create New Game", parent, DialogFlags.Modal)
        {
            this.repository = repository;
            
            SetDefaultSize(400, 150);
            SetPosition(WindowPosition.CenterOnParent);

            var contentArea = ContentArea;
            contentArea.Spacing = 10;
            contentArea.BorderWidth = 10;

            var label = new Label("Game Name:");
            contentArea.PackStart(label, false, false, 0);

            gameNameEntry = new Entry();
            gameNameEntry.Text = $"Game {DateTime.Now:yyyy-MM-dd HH:mm}";
            gameNameEntry.Activated += (s, e) => OnGenerate();
            contentArea.PackStart(gameNameEntry, false, false, 0);

            AddButton("Cancel", ResponseType.Cancel);
            AddButton("Generate", ResponseType.Ok);

            Response += OnResponse;
        }

        private void OnResponse(object? sender, ResponseArgs args)
        {
            if (args.ResponseId == ResponseType.Ok)
            {
                OnGenerate();
            }
        }

        private void OnGenerate()
        {
            var gameName = gameNameEntry.Text.Trim();
            if (string.IsNullOrWhiteSpace(gameName))
            {
                var md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Warning,
                    ButtonsType.Ok,
                    "Please enter a game name.");
                md.Run();
                md.Destroy();
                return;
            }

            // Show generating dialog
            var progressDialog = new MessageDialog(
                this,
                DialogFlags.Modal,
                MessageType.Info,
                ButtonsType.None,
                "Generating game and clues...\nThis may take a moment.");
            progressDialog.ShowAll();

            // Process GTK events to show the dialog
            while (Application.EventsPending())
                Application.RunIteration();

            // Generate game
            var generator = new SuitableGameGenerator();
            var game = generator.BuildSuitable();
            game.GameName = gameName;
            game.CreatedAt = DateTime.Now;

            // Generate clues
            var clueService = new ClueService();
            game.Clues = clueService.GenerateClues(game);

            // Create session
            var session = new GameSession
            {
                Game = game
            };

            // Save
            repository.SaveGameSession(session);

            progressDialog.Destroy();

            // Open game window
            var gameWindow = new GameWindow(session, repository);
            gameWindow.ShowAll();
        }
    }
}
