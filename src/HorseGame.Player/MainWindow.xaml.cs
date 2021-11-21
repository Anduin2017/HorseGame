using HorseGame.Generator;
using HorseGame.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace HorseGame.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Game? game;

        public MainWindow()
        {
            InitializeComponent();
            this.HelpLabel.Content = "Horse +3 You Get 3x\nHorse +2 You Get 1x\nHorse +1 You Get half\nHorse +0 You Get Nothing";
            var gameJson = File.ReadAllText("game.json");
            this.game = System.Text.Json.JsonSerializer.Deserialize<Game>(gameJson);
            foreach (var level in this.game.Levels)
            {
                this.Levels.Items.Add("Round " + (this.game.Levels.IndexOf(level) + 1));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var scores = new List<string>();
            var selectedIndex = this.Levels.SelectedIndex;
            var level = this.game.Levels[selectedIndex];
            var stopWatch = new Stopwatch();
            var measurer = new ProgressMadeMeasurer();
            var horseEvaluator = new HorseEvaluator();
            var overtake = new OvertakeEvaluator();

            this.GryffindorProgress.Value = 0;
            this.HufflepuffProgress.Value = 0;
            this.RavenclawProgress.Value = 0;
            this.SlytherinProgress.Value = 0;
            this.GTime.Content = string.Empty;
            this.HTime.Content = string.Empty;
            this.RTime.Content = string.Empty;
            this.STime.Content = string.Empty;

            stopWatch.Start();
            while (
                this.GryffindorProgress.Value < 100 ||
                this.HufflepuffProgress.Value < 100 ||
                this.RavenclawProgress.Value < 100 ||
                this.SlytherinProgress.Value < 100)
            {
                await Task.Delay(1);
                var elapsed = stopWatch.Elapsed.TotalSeconds;
                this.GryffindorProgress.Value = measurer.GetProgress(level.GryffindorSpeeds, elapsed);
                this.HufflepuffProgress.Value = measurer.GetProgress(level.HufflepuffSpeeds, elapsed);
                this.RavenclawProgress.Value = measurer.GetProgress(level.RavenclawSpeeds, elapsed);
                this.SlytherinProgress.Value = measurer.GetProgress(level.SlytherinSpeeds, elapsed);

                if (this.GryffindorProgress.Value >= 100 && !scores.Any(t => t.Contains("Gryffindor")))
                {
                    scores.Add("Gryffindor");
                }
                if (this.HufflepuffProgress.Value >= 100 && !scores.Any(t => t.Contains("Hufflepuff")))
                {
                    scores.Add("Hufflepuff");
                }
                if (this.RavenclawProgress.Value >= 100 && !scores.Any(t => t.Contains("Ravenclaw")))
                {
                    scores.Add("Ravenclaw");
                }
                if (this.SlytherinProgress.Value >= 100 && !scores.Any(t => t.Contains("Slytherin")))
                {
                    scores.Add("Slytherin");
                }
                ResultLabel.Content = string.Join('\n', scores);
            }
            stopWatch.Stop();

            this.GTime.Content = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds);
            this.HTime.Content = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds);
            this.RTime.Content = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds);
            this.STime.Content = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds);


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

                this.GScore.Content = gryffindorScore;
                this.RScore.Content = ravenclawScore;
                this.HScore.Content = hufflepuffScore;
                this.SScore.Content = slytherinScore;

                if (previouslevel == level)
                {
                    return;
                }
            }
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
