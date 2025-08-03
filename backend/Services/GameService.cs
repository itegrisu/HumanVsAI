using AivsHuman.Api.Models;

namespace AivsHuman.Api.Services;

public interface IGameService
{
    Task<StartGameResponse> StartGameAsync(StartGameRequest request);
    Task<GuessResponse> SubmitGuessAsync(GuessRequest request);
    Task<GameSession?> GetGameSessionAsync(string sessionId);
    Task<List<LeaderboardEntry>> GetLeaderboardAsync(string gameMode = "normal", int limit = 10);
    Task<PlayerStats?> GetPlayerStatsAsync(string playerName);
    Task SaveGameSessionAsync(GameSession session);
}

public class GameService : IGameService
{
    private readonly IImageService _imageService;
    private readonly Dictionary<string, GameSession> _activeSessions = new();
    private readonly Dictionary<string, PlayerStats> _playerStats = new();
    private readonly List<LeaderboardEntry> _leaderboard = new();

    public GameService(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<StartGameResponse> StartGameAsync(StartGameRequest request)
    {
        var images = await _imageService.GetRandomImagesAsync(
            request.ImageCount, 
            request.Category, 
            request.Difficulty
        );

        var session = new GameSession
        {
            Id = Guid.NewGuid().ToString(),
            PlayerName = request.PlayerName,
            GameMode = request.GameMode,
            Images = images,
            TotalQuestions = request.ImageCount,
            CurrentQuestion = 1,
            StartTime = DateTime.UtcNow
        };

        _activeSessions[session.Id] = session;

        return new StartGameResponse
        {
            SessionId = session.Id,
            FirstImage = images.First(),
            TotalQuestions = request.ImageCount,
            GameMode = request.GameMode
        };
    }

    public async Task<GuessResponse> SubmitGuessAsync(GuessRequest request)
    {
        if (!_activeSessions.TryGetValue(request.SessionId, out var session))
        {
            throw new ArgumentException("Invalid session ID");
        }

        var image = await _imageService.GetImageByIdAsync(request.ImageId);
        if (image == null)
        {
            throw new ArgumentException("Invalid image ID");
        }

        var isCorrect = request.Guess.Equals(image.Label, StringComparison.OrdinalIgnoreCase);
        var pointsEarned = CalculatePoints(isCorrect, request.ResponseTimeSeconds, image.Difficulty);

        var result = new GameResult
        {
            ImageId = request.ImageId,
            PlayerGuess = request.Guess,
            CorrectAnswer = image.Label,
            IsCorrect = isCorrect,
            ResponseTimeSeconds = request.ResponseTimeSeconds,
            PointsEarned = pointsEarned,
            AnsweredAt = DateTime.UtcNow
        };

        session.Results.Add(result);
        session.TotalScore += pointsEarned;
        session.CurrentQuestion++;

        // Update image statistics
        image.TimesUsed++;
        image.AverageGuessTime = (image.AverageGuessTime * (image.TimesUsed - 1) + request.ResponseTimeSeconds) / image.TimesUsed;
        
        var correctGuesses = session.Results.Count(r => r.ImageId == image.Id && r.IsCorrect);
        image.AccuracyRate = (double)correctGuesses / image.TimesUsed * 100;

        var response = new GuessResponse
        {
            IsCorrect = isCorrect,
            CorrectAnswer = image.Label,
            Message = GenerateResponseMessage(isCorrect, image),
            PointsEarned = pointsEarned,
            TotalScore = session.TotalScore,
            IsGameComplete = session.CurrentQuestion > session.TotalQuestions
        };

        if (!response.IsGameComplete && session.CurrentQuestion <= session.Images.Count)
        {
            response.NextImage = session.Images[session.CurrentQuestion - 1];
        }
        else if (response.IsGameComplete)
        {
            await CompleteGameAsync(session);
        }

        return response;
    }

    public Task<GameSession?> GetGameSessionAsync(string sessionId)
    {
        _activeSessions.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    public Task<List<LeaderboardEntry>> GetLeaderboardAsync(string gameMode = "normal", int limit = 10)
    {
        var entries = _leaderboard
            .Where(x => x.GameMode.Equals(gameMode, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Accuracy)
            .Take(limit)
            .ToList();

        return Task.FromResult(entries);
    }

    public Task<PlayerStats?> GetPlayerStatsAsync(string playerName)
    {
        _playerStats.TryGetValue(playerName.ToLower(), out var stats);
        return Task.FromResult(stats);
    }

    public Task SaveGameSessionAsync(GameSession session)
    {
        // In a real implementation, this would save to database
        // For now, we'll just update in-memory stats
        UpdatePlayerStats(session);
        UpdateLeaderboard(session);
        
        return Task.CompletedTask;
    }

    private async Task CompleteGameAsync(GameSession session)
    {
        session.EndTime = DateTime.UtcNow;
        session.Accuracy = (double)session.Results.Count(r => r.IsCorrect) / session.Results.Count * 100;
        session.AverageResponseTime = session.Results.Average(r => r.ResponseTimeSeconds);

        await SaveGameSessionAsync(session);
        
        // Remove from active sessions
        _activeSessions.Remove(session.Id);
    }

    private void UpdatePlayerStats(GameSession session)
    {
        var key = session.PlayerName.ToLower();
        
        if (!_playerStats.TryGetValue(key, out var stats))
        {
            stats = new PlayerStats
            {
                PlayerName = session.PlayerName,
                FirstPlayed = session.StartTime
            };
            _playerStats[key] = stats;
        }

        stats.TotalGames++;
        stats.TotalCorrect += session.Results.Count(r => r.IsCorrect);
        stats.TotalWrong += session.Results.Count(r => !r.IsCorrect);
        stats.TotalScore += session.TotalScore;
        stats.OverallAccuracy = (double)stats.TotalCorrect / (stats.TotalCorrect + stats.TotalWrong) * 100;
        stats.AverageResponseTime = (stats.AverageResponseTime * (stats.TotalGames - 1) + session.AverageResponseTime) / stats.TotalGames;
        stats.LastPlayed = session.EndTime ?? DateTime.UtcNow;
        
        if (session.TotalScore > stats.HighestScore)
        {
            stats.HighestScore = session.TotalScore;
        }

        // Update category accuracy
        foreach (var result in session.Results)
        {
            var image = session.Images.FirstOrDefault(i => i.Id == result.ImageId);
            if (image != null)
            {
                if (!stats.CategoryAccuracy.ContainsKey(image.Category))
                {
                    stats.CategoryAccuracy[image.Category] = 0;
                }
                
                var categoryResults = session.Results.Where(r => 
                    session.Images.FirstOrDefault(i => i.Id == r.ImageId)?.Category == image.Category).ToList();
                
                var categoryAccuracy = (double)categoryResults.Count(r => r.IsCorrect) / categoryResults.Count * 100;
                stats.CategoryAccuracy[image.Category] = categoryAccuracy;
            }
        }

        stats.RecentGames.Add(session);
        if (stats.RecentGames.Count > 10)
        {
            stats.RecentGames = stats.RecentGames.OrderByDescending(g => g.StartTime).Take(10).ToList();
        }
    }

    private void UpdateLeaderboard(GameSession session)
    {
        var existingEntry = _leaderboard.FirstOrDefault(x => 
            x.PlayerName.Equals(session.PlayerName, StringComparison.OrdinalIgnoreCase) && 
            x.GameMode.Equals(session.GameMode, StringComparison.OrdinalIgnoreCase));

        if (existingEntry != null)
        {
            if (session.TotalScore > existingEntry.Score)
            {
                existingEntry.Score = session.TotalScore;
                existingEntry.Accuracy = session.Accuracy;
                existingEntry.LastPlayed = session.EndTime ?? DateTime.UtcNow;
            }
            existingEntry.GamesPlayed++;
        }
        else
        {
            _leaderboard.Add(new LeaderboardEntry
            {
                PlayerName = session.PlayerName,
                Score = session.TotalScore,
                Accuracy = session.Accuracy,
                GamesPlayed = 1,
                LastPlayed = session.EndTime ?? DateTime.UtcNow,
                GameMode = session.GameMode
            });
        }
    }

    private static int CalculatePoints(bool isCorrect, double responseTime, string difficulty)
    {
        if (!isCorrect) return 0;

        var basePoints = 10;
        var timeBonus = Math.Max(0, (int)(20 - responseTime)); // Up to 20 bonus points for speed
        var difficultyMultiplier = difficulty switch
        {
            "easy" => 1.0,
            "medium" => 1.5,
            "hard" => 2.0,
            _ => 1.0
        };

        return (int)((basePoints + timeBonus) * difficultyMultiplier);
    }

    private static string GenerateResponseMessage(bool isCorrect, GameImage image)
    {
        if (isCorrect)
        {
            return image.Label == "ai" 
                ? $"Doğru! Bu görsel {image.Source} tarafından oluşturuldu."
                : $"Doğru! Bu {image.Category} kategorisinde gerçek bir fotoğraf.";
        }
        else
        {
            return image.Label == "ai"
                ? $"Yanlış! Bu görsel aslında {image.Source} tarafından AI ile oluşturuldu."
                : $"Yanlış! Bu aslında gerçek bir {image.Category} fotoğrafı.";
        }
    }
}
