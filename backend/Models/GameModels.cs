namespace AivsHuman.Api.Models;

public class GameImage
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty; // "ai" or "human"
    public string Category { get; set; } = string.Empty; // portrait, landscape, object, abstract, architecture
    public string Difficulty { get; set; } = "medium"; // easy, medium, hard
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty; // AI model name or human source
    public string Description { get; set; } = string.Empty;
    public int TimesUsed { get; set; } = 0;
    public double AverageGuessTime { get; set; } = 0;
    public double AccuracyRate { get; set; } = 0; // Percentage of correct guesses
}

public class ImageStats
{
    public int TotalImages { get; set; }
    public int AiImages { get; set; }
    public int HumanImages { get; set; }
    public int CategoriesCount { get; set; }
    public Dictionary<string, int> CategoryBreakdown { get; set; } = new();
    public Dictionary<string, int> DifficultyBreakdown { get; set; } = new();
}

public class GameSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PlayerName { get; set; } = string.Empty;
    public string GameMode { get; set; } = "normal"; // normal, time-rush, duel
    public List<GameResult> Results { get; set; } = new();
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public int TotalScore { get; set; }
    public double Accuracy { get; set; }
    public double AverageResponseTime { get; set; }
    public List<GameImage> Images { get; set; } = new(); // For backward compatibility
    public int CurrentQuestion { get; set; } = 1;
    public int TotalQuestions { get; set; } = 10;
}

public class GuessRequest
{
    public string ImageId { get; set; } = string.Empty;
    public string Guess { get; set; } = string.Empty; // "ai" or "human"
    public double ResponseTimeSeconds { get; set; } = 0;
    public string SessionId { get; set; } = string.Empty;
}

public class GuessResponse
{
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public int PointsEarned { get; set; }
    public int TotalScore { get; set; }
    public GameImage? NextImage { get; set; }
    public bool IsGameComplete { get; set; }
}

public class GameResult
{
    public string ImageId { get; set; } = string.Empty;
    public string PlayerGuess { get; set; } = string.Empty; // "ai" or "human"
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public double ResponseTimeSeconds { get; set; }
    public int PointsEarned { get; set; }
    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    public int Correct { get; set; } // For backward compatibility
    public int Wrong { get; set; } // For backward compatibility
    public int Total { get; set; } // For backward compatibility
    public double SuccessRate { get; set; } // For backward compatibility
}

public class PlayerStats
{
    public string PlayerName { get; set; } = string.Empty;
    public int TotalGames { get; set; }
    public int TotalCorrect { get; set; }
    public int TotalWrong { get; set; }
    public double OverallAccuracy { get; set; }
    public double AverageResponseTime { get; set; }
    public int HighestScore { get; set; }
    public int TotalScore { get; set; }
    public Dictionary<string, double> CategoryAccuracy { get; set; } = new();
    public List<GameSession> RecentGames { get; set; } = new();
    public DateTime FirstPlayed { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayed { get; set; } = DateTime.UtcNow;
}

public class LeaderboardEntry
{
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public double Accuracy { get; set; }
    public int GamesPlayed { get; set; }
    public DateTime LastPlayed { get; set; }
    public string GameMode { get; set; } = "normal";
}

public class StartGameRequest
{
    public string PlayerName { get; set; } = string.Empty;
    public string GameMode { get; set; } = "normal";
    public string? Category { get; set; }
    public string? Difficulty { get; set; }
    public int ImageCount { get; set; } = 10;
}

public class StartGameResponse
{
    public string SessionId { get; set; } = string.Empty;
    public GameImage FirstImage { get; set; } = new();
    public int TotalQuestions { get; set; }
    public string GameMode { get; set; } = string.Empty;
}
