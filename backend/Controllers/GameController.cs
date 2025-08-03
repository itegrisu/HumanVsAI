using Microsoft.AspNetCore.Mvc;
using AivsHuman.Api.Models;
using AivsHuman.Api.Services;

namespace AivsHuman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IImageService _imageService;

    public GameController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpGet("start")]
    public ActionResult<GameSession> StartGame()
    {
        var images = _imageService.GetRandomImages(10);
        var session = new GameSession
        {
            Images = images,
            CurrentQuestion = 1,
            TotalQuestions = 10
        };

        return Ok(session);
    }

    [HttpPost("guess")]
    public ActionResult<GuessResponse> MakeGuess([FromBody] GuessRequest request)
    {
        var image = _imageService.GetImageById(request.ImageId);
        
        if (image == null)
        {
            return NotFound("Image not found");
        }

        var isCorrect = request.Guess.ToLower() == image.Label.ToLower();
        
        var response = new GuessResponse
        {
            IsCorrect = isCorrect,
            CorrectAnswer = image.Label,
            Message = isCorrect ? "Doğru!" : "Yanlış!"
        };

        return Ok(response);
    }

    [HttpGet("images")]
    public ActionResult<List<GameImage>> GetAllImages()
    {
        var images = _imageService.GetRandomImages(100);
        return Ok(images);
    }

    [HttpGet("image/{id}")]
    public ActionResult<GameImage> GetImage(string id)
    {
        var image = _imageService.GetImageById(id);
        
        if (image == null)
        {
            return NotFound("Image not found");
        }

        return Ok(image);
    }
}
