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
            var gameJson = File.ReadAllText("game.json");
            this.game = System.Text.Json.JsonSerializer.Deserialize<Game>(gameJson);
            foreach (var level in this.game.Levels)
            {
                this.Levels.Items.Add("Round " + (this.game.Levels.IndexOf(level) + 1));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.Levels.SelectedIndex;
            var level = this.game.Levels[selectedIndex];
            var stopWatch = new Stopwatch();
            var measurer = new ProgressMadeMeasurer();
            var horseEvaluator = new HorseEvaluator();
            stopWatch.Start();
            this.GryffindorProgress.Value = 0;
            this.HufflepuffProgress.Value = 0;
            this.RavenclawProgress.Value = 0;
            this.SlytherinProgress.Value = 0;
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
            }

            this.GTime.Content = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds);
            this.HTime.Content = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds);
            this.RTime.Content = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds);
            this.STime.Content = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds);
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
