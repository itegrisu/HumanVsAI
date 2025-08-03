using Microsoft.AspNetCore.Mvc;
using AivsHuman.Api.Models;
using AivsHuman.Api.Services;

namespace AivsHuman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly IGameService _gameService;

    public GameController(IImageService imageService, IGameService gameService)
    {
        _imageService = imageService;
        _gameService = gameService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<StartGameResponse>> StartGame([FromBody] StartGameRequest request)
    {
        try
        {
            var response = await _gameService.StartGameAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("guess")]
    public async Task<ActionResult<GuessResponse>> MakeGuess([FromBody] GuessRequest request)
    {
        try
        {
            var response = await _gameService.SubmitGuessAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<GameSession>> GetGameSession(string sessionId)
    {
        var session = await _gameService.GetGameSessionAsync(sessionId);
        
        if (session == null)
        {
            return NotFound("Game session not found");
        }

        return Ok(session);
    }

    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<LeaderboardEntry>>> GetLeaderboard(
        [FromQuery] string gameMode = "normal", 
        [FromQuery] int limit = 10)
    {
        var leaderboard = await _gameService.GetLeaderboardAsync(gameMode, limit);
        return Ok(leaderboard);
    }

    [HttpGet("stats/{playerName}")]
    public async Task<ActionResult<PlayerStats>> GetPlayerStats(string playerName)
    {
        var stats = await _gameService.GetPlayerStatsAsync(playerName);
        
        if (stats == null)
        {
            return NotFound("Player not found");
        }

        return Ok(stats);
    }

    [HttpGet("images")]
    public async Task<ActionResult<List<GameImage>>> GetImages(
        [FromQuery] string? category = null,
        [FromQuery] string? difficulty = null,
        [FromQuery] int count = 20)
    {
        var images = await _imageService.GetRandomImagesAsync(count, category, difficulty);
        return Ok(images);
    }

    [HttpGet("images/categories")]
    public async Task<ActionResult<List<string>>> GetCategories()
    {
        var categories = await _imageService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("images/stats")]
    public async Task<ActionResult<ImageStats>> GetImageStats()
    {
        var stats = await _imageService.GetImageStatsAsync();
        return Ok(stats);
    }

    [HttpGet("images/category/{category}")]
    public async Task<ActionResult<List<GameImage>>> GetImagesByCategory(
        string category, 
        [FromQuery] int count = 20)
    {
        var images = await _imageService.GetImagesByCategoryAsync(category, count);
        return Ok(images);
    }

    [HttpGet("images/duel")]
    public async Task<ActionResult<List<GameImage>>> GetDuelImages([FromQuery] int count = 5)
    {
        var images = await _imageService.GetDuelImagesAsync(count);
        return Ok(images);
    }

    [HttpGet("image/{id}")]
    public async Task<ActionResult<GameImage>> GetImage(string id)
    {
        var image = await _imageService.GetImageByIdAsync(id);
        
        if (image == null)
        {
            return NotFound("Image not found");
        }

        return Ok(image);
    }

    // Legacy endpoints for backward compatibility
    [HttpGet("start")]
    public async Task<ActionResult<GameSession>> StartGameLegacy()
    {
        var request = new StartGameRequest
        {
            PlayerName = "Anonymous",
            GameMode = "normal",
            ImageCount = 10
        };

        var response = await _gameService.StartGameAsync(request);
        
        var session = new GameSession
        {
            Id = response.SessionId,
            Images = new List<GameImage> { response.FirstImage },
            CurrentQuestion = 1,
            TotalQuestions = response.TotalQuestions
        };

        return Ok(session);
    }
}
