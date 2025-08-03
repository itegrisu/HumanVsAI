namespace AivsHuman.Api.Models;

public class GameImage
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty; // "ai" or "human"
    public string Category { get; set; } = "general";
}

public class GameSession
{
    public List<GameImage> Images { get; set; } = new();
    public int CurrentQuestion { get; set; } = 1;
    public int TotalQuestions { get; set; } = 10;
}

public class GuessRequest
{
    public string ImageId { get; set; } = string.Empty;
    public string Guess { get; set; } = string.Empty; // "ai" or "human"
}

public class GuessResponse
{
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class GameResult
{
    public int Correct { get; set; }
    public int Wrong { get; set; }
    public int Total { get; set; }
    public double SuccessRate { get; set; }
}
