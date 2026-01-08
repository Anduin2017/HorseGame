#pragma warning disable IDE0060,CS0612
#pragma warning disable
using Gtk;
using HorseGame.Shared;

namespace HorseGame.Unified.Windows
{
    /// <summary>
    /// Dedicated window for displaying all game clues
    /// Separated from main window to avoid clue exposure in screenshots
    /// </summary>
    public class CluesWindow : Window
    {
        public CluesWindow(GameSession session) : base("All Clues")
        {
            SetDefaultSize(800, 600);
            SetPosition(WindowPosition.Center);

            var vbox = new VBox(false, 10);
            vbox.BorderWidth = 15;

            // Title
            var titleLabel = new Label();
            titleLabel.Markup = "<span size='16000' weight='bold'>📋 All Generated Clues</span>";
            vbox.PackStart(titleLabel, false, false, 0);

            // Clues text view
            var scrolled = new ScrolledWindow();
            var textView = new TextView();
            textView.Editable = false;
            textView.WrapMode = WrapMode.Word;
            textView.Buffer.Text = string.Join("\n\n", session.Game.Clues.Select((c, i) => $"{i + 1}. {c}"));
            scrolled.Add(textView);
            vbox.PackStart(scrolled, true, true, 0);

            // Close button
            var closeButton = new Button("Close");
            closeButton.Clicked += (_,_) => this.Destroy();
            vbox.PackStart(closeButton, false, false, 0);

            Add(vbox);
            ShowAll();
        }
    }
}
