using Gtk;
using HorseGame.Generator;
using HorseGame.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HorseGame.Player
{
    public class MainWindow : Window
    {
        private readonly Game? game;
        private ComboBoxText levelsCombo;
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
        private Label helpLabel;
        private Button playButton;

        public MainWindow() : base("Horse Game Player")
        {
            SetDefaultSize(840, 550);
            SetPosition(WindowPosition.Center);

            DeleteEvent += (o, e) => Application.Quit();

            var fixedContainer = new Fixed();

            levelsCombo = new ComboBoxText();
            fixedContainer.Put(levelsCombo, 37, 34);
            levelsCombo.SetSizeRequest(326, -1);

            playButton = new Button("Play");
            playButton.Clicked += Button_Click;
            fixedContainer.Put(playButton, 550, 30);
            playButton.SetSizeRequest(176, 26);

            // Labels and ProgressBars
            AddHorseRow(fixedContainer, "Gryffindor", 94, out gryffindorProgress, out gTime, out gScore);
            AddHorseRow(fixedContainer, "Hufflepuff", 160, out hufflepuffProgress, out hTime, out hScore);
            AddHorseRow(fixedContainer, "Ravenclaw", 229, out ravenclawProgress, out rTime, out rScore);
            AddHorseRow(fixedContainer, "Slytherin", 303, out slytherinProgress, out sTime, out sScore);

            resultLabel = new Label("Result");
            fixedContainer.Put(resultLabel, 250, 348);
            resultLabel.SetSizeRequest(213, -1);

            helpLabel = new Label("");
            fixedContainer.Put(helpLabel, 80, 348);
            helpLabel.SetSizeRequest(240, -1);

            Add(fixedContainer);

            helpLabel.Text = "Horse +3 You Get 3x\nHorse +2 You Get 1x\nHorse +1 You Get half\nHorse +0 You Get Nothing";

            if (File.Exists("game.json"))
            {
                var gameJson = File.ReadAllText("game.json");
                this.game = System.Text.Json.JsonSerializer.Deserialize<Game>(gameJson);
            }
            else
            {
                var generator = new SuitableGameGenerator();
                this.game = generator.BuildSuitable();
                var gameJson = System.Text.Json.JsonSerializer.Serialize(this.game);
                File.WriteAllText("game.json", gameJson);
            }

            if (this.game != null)
            {
                for (int i = 0; i < this.game.Levels.Count; i++)
                {
                    levelsCombo.AppendText("Round " + (i + 1));
                }
                levelsCombo.Active = 0;
            }
        }

        private void AddHorseRow(Fixed container, string name, int y, out ProgressBar progress, out Label timeLabel, out Label scoreLabel)
        {
            var nameLabel = new Label(name);
            container.Put(nameLabel, 37, y + 2);

            progress = new ProgressBar();
            container.Put(progress, 132, y);
            progress.SetSizeRequest(558, 28);

            timeLabel = new Label("");
            container.Put(timeLabel, 726, y + 2);

            scoreLabel = new Label("0");
            container.Put(scoreLabel, 37, y + 28);
        }

        private async void Button_Click(object? sender, EventArgs e)
        {
            if (this.game == null || levelsCombo.Active == -1) return;

            var scores = new List<string>();
            var selectedIndex = levelsCombo.Active;
            var level = this.game.Levels[selectedIndex];
            var stopWatch = new Stopwatch();
            var measurer = new ProgressMadeMeasurer();
            var horseEvaluator = new HorseEvaluator();
            var overtake = new OvertakeEvaluator();

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
                resultLabel.Text = string.Join('\n', scores);
            }
            stopWatch.Stop();

            gTime.Text = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds).ToString("F2");
            hTime.Text = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds).ToString("F2");
            rTime.Text = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds).ToString("F2");
            sTime.Text = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds).ToString("F2");


            int gryffindorScore = 0;
            int ravenclawScore = 0;
            int hufflepuffScore = 0;
            int slytherinScore = 0;
            foreach (var previouslevel in game.Levels)
            {
                var gryffindorTime = horseEvaluator.EvaluatorTime(previouslevel.GryffindorSpeeds);
                var ravenclawTime = horseEvaluator.EvaluatorTime(previouslevel.RavenclawSpeeds);
                var hufflepuffTime = horseEvaluator.EvaluatorTime(previouslevel.HufflepuffSpeeds);
                var slytherinsTime = horseEvaluator.EvaluatorTime(previouslevel.SlytherinSpeeds);

                var scoresList = new List<double>
                {
                    gryffindorTime,
                    ravenclawTime,
                    hufflepuffTime,
                    slytherinsTime
                }.OrderBy(t => t).ToArray();

                gryffindorScore += overtake.GetScoreBasedOnTimeChart(scoresList, gryffindorTime);
                ravenclawScore += overtake.GetScoreBasedOnTimeChart(scoresList, ravenclawTime);
                hufflepuffScore += overtake.GetScoreBasedOnTimeChart(scoresList, hufflepuffTime);
                slytherinScore += overtake.GetScoreBasedOnTimeChart(scoresList, slytherinsTime);

                gScore.Text = gryffindorScore.ToString();
                rScore.Text = ravenclawScore.ToString();
                hScore.Text = hufflepuffScore.ToString();
                sScore.Text = slytherinScore.ToString();

                if (previouslevel == level)
                {
                    return;
                }
            }
        }
    }
}
