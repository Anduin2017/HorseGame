using System.Diagnostics;
using HorseGame.Shared;
using HorseGame.Unified.Services;

namespace HorseGame.Unified.Presenters
{
    /// <summary>
    /// Presenter for GameWindow - handles all business logic
    /// Coordinates between UI components and data/services
    /// </summary>
    public class GamePresenter
    {
        private readonly GameSession session;
        private readonly GameRepository repository;
        private readonly HorseEvaluator horseEvaluator;
        private readonly OvertakeEvaluator overtakeEvaluator;
        private readonly ProgressMadeMeasurer measurer;
        private readonly BettingService bettingService;

        // Events for UI updates
        public event Action? PlayersChanged;
        public event Action? BetsChanged;
        public event Action<int>? CurrentRoundChanged;
        public event Action<Dictionary<string, double>>? ProgressUpdated;
        public event Action<Dictionary<string, double>>? TimesUpdated;
        public event Action<Dictionary<string, int>>? ScoresUpdated;
        public event Action<List<string>>? RaceCompleted;

        public GamePresenter(GameSession session, GameRepository repository)
        {
            this.session = session;
            this.repository = repository;
            this.horseEvaluator = new HorseEvaluator();
            this.overtakeEvaluator = new OvertakeEvaluator();
            this.measurer = new ProgressMadeMeasurer();
            this.bettingService = new BettingService();
        }

        public GameSession Session => session;

        #region Player Management

        public void AddPlayer(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
                throw new ArgumentException("Player name cannot be empty");

            if (session.Players.Any(p => p.PlayerName == playerName))
                throw new InvalidOperationException("Player already exists");

            session.Players.Add(new Player { PlayerName = playerName });
            session.PlayerBets[playerName] = new List<Bet>();

            repository.SaveGameSession(session);
            PlayersChanged?.Invoke();
            BetsChanged?.Invoke();
        }

        public void DeletePlayer(string playerName)
        {
            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null)
                throw new InvalidOperationException("Player not found");

            session.Players.Remove(player);
            session.PlayerBets.Remove(playerName);

            repository.SaveGameSession(session);
            PlayersChanged?.Invoke();
            BetsChanged?.Invoke();
        }

        public void PurchaseClue(string playerName)
        {
            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null)
                throw new InvalidOperationException("Player not found");

            var balance = CalculatePlayerBalance(playerName);
            if (balance < 2)
                throw new InvalidOperationException(
                    $"{playerName} doesn't have enough balance! Current: {balance:F1}元");

            var availableClues = Enumerable.Range(0, session.Game.Clues.Count)
                .Where(i => !player.PurchasedClueIndices.Contains(i))
                .ToList();

            if (availableClues.Count == 0)
                throw new InvalidOperationException("All clues have been purchased by this player!");

            var random = new Random();
            var clueIndex = availableClues[random.Next(availableClues.Count)];
            player.PurchasedClueIndices.Add(clueIndex);

            repository.SaveGameSession(session);
            PlayersChanged?.Invoke();

            // Return the purchased clue text for display
            // UI will handle showing it
        }

        public string GetPurchasedClue(int clueIndex)
        {
            return session.Game.Clues[clueIndex];
        }

        public decimal CalculatePlayerBalance(string playerName)
        {
            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null) return 0;

            decimal balance = player.InitialBalance;

            // Add income for each completed round
            balance += bettingService.CalculateTotalIncome(session.CurrentRound + 1);

            // Deduct clue purchases
            balance -= player.PurchasedClueIndices.Count * 2;

            // Calculate betting results
            if (session.PlayerBets.ContainsKey(playerName))
            {
                foreach (var bet in session.PlayerBets[playerName])
                {
                    balance -= bet.BetAmount;
                    balance += bet.Payout;
                }
            }

            return balance;
        }

        #endregion

        #region Betting

        public void PlaceBet(string playerName, string horseName, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Bet amount must be positive");

            var player = session.Players.FirstOrDefault(p => p.PlayerName == playerName);
            if (player == null)
                throw new InvalidOperationException("Player not found");

            var roundNumber = session.CurrentRound + 1;

            // Check if bet already placed for this round
            if (session.PlayerBets.ContainsKey(playerName))
            {
                var existingBet = session.PlayerBets[playerName].FirstOrDefault(b => b.RoundNumber == roundNumber);
                if (existingBet != null)
                    throw new InvalidOperationException(
                        $"{playerName} has already placed a bet for Round {roundNumber}!");
            }

            // Check balance
            var balance = CalculatePlayerBalance(playerName);
            if (balance < amount)
                throw new InvalidOperationException(
                    $"{playerName} doesn't have enough balance! Current: {balance:F1}元, Bet: {amount}元");

            // Create bet
            var bet = new Bet
            {
                PlayerName = playerName,
                RoundNumber = roundNumber,
                HorseName = horseName,
                BetAmount = amount,
                Payout = 0
            };

            session.PlayerBets[playerName].Add(bet);
            repository.SaveGameSession(session);

            BetsChanged?.Invoke();
            PlayersChanged?.Invoke();
        }

        #endregion

        #region Race Playback

        public async Task PlayRace(int roundIndex)
        {
            var level = session.Game.Levels[roundIndex];
            var scores = new List<string>();
            var stopWatch = new Stopwatch();

            // Reset progress
            ProgressUpdated?.Invoke(new Dictionary<string, double>
            {
                ["Gryffindor"] = 0,
                ["Hufflepuff"] = 0,
                ["Ravenclaw"] = 0,
                ["Slytherin"] = 0
            });

            TimesUpdated?.Invoke(new Dictionary<string, double>());

            stopWatch.Start();
            while (scores.Count < 4)
            {
                await Task.Delay(1);
                var elapsed = stopWatch.Elapsed.TotalSeconds;

                var progress = new Dictionary<string, double>
                {
                    ["Gryffindor"] = Math.Min(1.0, measurer.GetProgress(level.GryffindorSpeeds, elapsed) / 100.0),
                    ["Hufflepuff"] = Math.Min(1.0, measurer.GetProgress(level.HufflepuffSpeeds, elapsed) / 100.0),
                    ["Ravenclaw"] = Math.Min(1.0, measurer.GetProgress(level.RavenclawSpeeds, elapsed) / 100.0),
                    ["Slytherin"] = Math.Min(1.0, measurer.GetProgress(level.SlytherinSpeeds, elapsed) / 100.0)
                };

                ProgressUpdated?.Invoke(progress);

                // Track finishers
                if (progress["Gryffindor"] >= 1 && !scores.Contains("Gryffindor")) scores.Add("Gryffindor");
                if (progress["Hufflepuff"] >= 1 && !scores.Contains("Hufflepuff")) scores.Add("Hufflepuff");
                if (progress["Ravenclaw"] >= 1 && !scores.Contains("Ravenclaw")) scores.Add("Ravenclaw");
                if (progress["Slytherin"] >= 1 && !scores.Contains("Slytherin")) scores.Add("Slytherin");
            }

            stopWatch.Stop();

            // Calculate times
            var times = new Dictionary<string, double>
            {
                ["Gryffindor"] = horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                ["Hufflepuff"] = horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                ["Ravenclaw"] = horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                ["Slytherin"] = horseEvaluator.EvaluatorTime(level.SlytherinSpeeds)
            };
            TimesUpdated?.Invoke(times);

            // Calculate cumulative scores
            var cumulativeScores = CalculateCumulativeScores(roundIndex);
            ScoresUpdated?.Invoke(cumulativeScores);

            // Calculate payouts
            ProcessBettingPayouts(roundIndex, scores, cumulativeScores);

            // Advance round
            if (roundIndex == session.CurrentRound && session.CurrentRound < session.Game.Levels.Count - 1)
            {
                session.CurrentRound++;
                CurrentRoundChanged?.Invoke(session.CurrentRound);
            }

            repository.SaveGameSession(session);
            PlayersChanged?.Invoke();
            BetsChanged?.Invoke();
            RaceCompleted?.Invoke(scores);
        }

        private Dictionary<string, int> CalculateCumulativeScores(int upToRoundIndex)
        {
            var scores = new Dictionary<string, int>
            {
                ["Gryffindor"] = 0,
                ["Hufflepuff"] = 0,
                ["Ravenclaw"] = 0,
                ["Slytherin"] = 0
            };

            for (int i = 0; i <= upToRoundIndex; i++)
            {
                var level = session.Game.Levels[i];
                var times = new[]
                {
                    horseEvaluator.EvaluatorTime(level.GryffindorSpeeds),
                    horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds),
                    horseEvaluator.EvaluatorTime(level.RavenclawSpeeds),
                    horseEvaluator.EvaluatorTime(level.SlytherinSpeeds)
                }.OrderBy(t => t).ToArray();

                scores["Gryffindor"] +=
                    overtakeEvaluator.GetScoreBasedOnTimeChart(times,
                        horseEvaluator.EvaluatorTime(level.GryffindorSpeeds));
                scores["Hufflepuff"] +=
                    overtakeEvaluator.GetScoreBasedOnTimeChart(times,
                        horseEvaluator.EvaluatorTime(level.HufflepuffSpeeds));
                scores["Ravenclaw"] +=
                    overtakeEvaluator.GetScoreBasedOnTimeChart(times,
                        horseEvaluator.EvaluatorTime(level.RavenclawSpeeds));
                scores["Slytherin"] +=
                    overtakeEvaluator.GetScoreBasedOnTimeChart(times,
                        horseEvaluator.EvaluatorTime(level.SlytherinSpeeds));
            }

            return scores;
        }

        private void ProcessBettingPayouts(int roundIndex, List<string> racePositions,
            Dictionary<string, int> cumulativeScores)
        {
            var roundNumber = roundIndex + 1;
            Dictionary<string, int> horsePositions;

            if (roundNumber == 10)
            {
                // Round 10: Use total score ranking
                var scoreRanking = cumulativeScores.OrderByDescending(kv => kv.Value).ToList();
                horsePositions = new Dictionary<string, int>();
                for (int i = 0; i < scoreRanking.Count; i++)
                {
                    horsePositions[scoreRanking[i].Key] = i + 1;
                }
            }
            else
            {
                // Rounds 1-9: Use race position
                horsePositions = new Dictionary<string, int>
                {
                    ["Gryffindor"] = racePositions.IndexOf("Gryffindor") + 1,
                    ["Hufflepuff"] = racePositions.IndexOf("Hufflepuff") + 1,
                    ["Ravenclaw"] = racePositions.IndexOf("Ravenclaw") + 1,
                    ["Slytherin"] = racePositions.IndexOf("Slytherin") + 1
                };
            }

            // Calculate payouts
            foreach (var playerBets in session.PlayerBets.Values)
            {
                var bet = playerBets.FirstOrDefault(b => b.RoundNumber == roundNumber && b.Payout == 0);
                if (bet != null && horsePositions.ContainsKey(bet.HorseName))
                {
                    var position = horsePositions[bet.HorseName];
                    bet.Payout = bettingService.CalculatePayout(bet.BetAmount, position, roundNumber);
                }
            }
        }

        #endregion
    }
}
