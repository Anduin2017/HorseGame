#pragma warning disable CS0612 // Type or member is obsolete

using Gtk;

namespace HorseGame.Unified.Components
{
    /// <summary>
    /// Panel for displaying race tracks with progress bars
    /// </summary>
    public class RaceTracksPanel : VBox
    {
        private Dictionary<string, ProgressBar> progressBars;
        private Dictionary<string, Label> timeLabels;
        private Dictionary<string, Label> scoreLabels;

        public RaceTracksPanel() : base(false, 15)
        {
            BorderWidth = 20;

            progressBars = new Dictionary<string, ProgressBar>();
            timeLabels = new Dictionary<string, Label>();
            scoreLabels = new Dictionary<string, Label>();

            AddHorseTrack("Gryffindor");
            AddHorseTrack("Hufflepuff");
            AddHorseTrack("Ravenclaw");
            AddHorseTrack("Slytherin");
        }

        private void AddHorseTrack(string horseName)
        {
            var hbox = new HBox(false, 10);

            // Horse name
            var nameLabel = new Label(horseName) { Xalign = 0 };
            nameLabel.SetSizeRequest(120, -1);
            hbox.PackStart(nameLabel, false, false, 0);

            // Progress bar
            var progress = new ProgressBar();
            progress.SetSizeRequest(500, 35);
            progressBars[horseName] = progress;
            hbox.PackStart(progress, true, true, 0);

            // Time label
            var timeLabel = new Label("") { Xalign = 0 };
            timeLabel.SetSizeRequest(70, -1);
            timeLabels[horseName] = timeLabel;
            hbox.PackStart(timeLabel, false, false, 0);

            // Score label (large and bold)
            var scoreLabel = new Label();
            scoreLabel.Markup = "<span size='18000' weight='bold'>0</span>";
            scoreLabel.SetSizeRequest(60, -1);
            scoreLabels[horseName] = scoreLabel;
            hbox.PackStart(scoreLabel, false, false, 0);

            PackStart(hbox, false, false, 0);
        }

        public void ResetProgress()
        {
            foreach (var progress in progressBars.Values)
            {
                progress.Fraction = 0;
            }

            foreach (var timeLabel in timeLabels.Values)
            {
                timeLabel.Text = "";
            }
        }

        public void UpdateProgress(Dictionary<string, double> progress)
        {
            foreach (var kvp in progress)
            {
                if (progressBars.ContainsKey(kvp.Key))
                {
                    progressBars[kvp.Key].Fraction = kvp.Value;
                }
            }
        }

        public void UpdateTimes(Dictionary<string, double> times)
        {
            foreach (var kvp in times)
            {
                if (timeLabels.ContainsKey(kvp.Key))
                {
                    timeLabels[kvp.Key].Text = kvp.Value.ToString("F2");
                }
            }
        }

        public void UpdateScores(Dictionary<string, int> scores)
        {
            foreach (var kvp in scores)
            {
                if (scoreLabels.ContainsKey(kvp.Key))
                {
                    scoreLabels[kvp.Key].Markup = $"<span size='18000' weight='bold'>{kvp.Value}</span>";
                }
            }
        }
    }
}
