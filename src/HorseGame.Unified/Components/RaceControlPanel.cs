#pragma warning disable IDE0060,CS0612
#pragma warning disable
using Gtk;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for race control (round selection and actions)
    /// </summary>
    public class RaceControlPanel : HBox
    {
        private ComboBoxText roundCombo;
        private Button playButton;

        // Events
        public event System.Action? PlayRequested;
        public event System.Action? ViewCluesRequested;

        public RaceControlPanel(int totalRounds) : base(false, 10)
        {
            // Round selector (disabled - sequential only)
            roundCombo = new ComboBoxText();
            for (int i = 0; i < totalRounds; i++)
            {
                roundCombo.AppendText($"Round {i + 1}");
            }

            roundCombo.Active = 0;
            roundCombo.Sensitive = false; // Prevent manual switching

            PackStart(new Label("Current Round:"), false, false, 0);
            PackStart(roundCombo, false, false, 0);

            // Play button
            playButton = new Button("▶ Play Race");
            playButton.Clicked += (_,__) => PlayRequested?.Invoke();
            PackStart(playButton, false, false, 0);

            // View Clues button
            var viewCluesButton = new Button("📋 View Clues");
            viewCluesButton.Clicked += (_ ,__) => ViewCluesRequested?.Invoke();
            PackEnd(viewCluesButton, false, false, 0);
        }

        public void UpdateCurrentRound(int roundNumber)
        {
            roundCombo.Active = roundNumber;
        }
    }
}
