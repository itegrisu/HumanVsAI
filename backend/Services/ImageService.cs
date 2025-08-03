using AivsHuman.Api.Models;

namespace AivsHuman.Api.Services;

public interface IImageService
{
    List<GameImage> GetRandomImages(int count = 10);
    GameImage? GetImageById(string id);
}

public class ImageService : IImageService
{
    private readonly List<GameImage> _images;

    public ImageService()
    {
        _images = InitializeMockImages();
    }

    public List<GameImage> GetRandomImages(int count = 10)
    {
        var random = new Random();
        return _images.OrderBy(x => random.Next()).Take(count).ToList();
    }

    public GameImage? GetImageById(string id)
    {
        return _images.FirstOrDefault(img => img.Id == id);
    }

    private static List<GameImage> InitializeMockImages()
    {
        var images = new List<GameImage>();

        // AI Generated Images (Mock data with Unsplash for MVP)
        for (int i = 1; i <= 50; i++)
        {
            images.Add(new GameImage
            {
                Id = $"ai_{i:D3}",
                Url = $"https://picsum.photos/600/400?random={i}",
                Label = "ai",
                Category = "general"
            });
        }

        // Human Created Images (Mock data with Unsplash for MVP)
        for (int i = 51; i <= 100; i++)
        {
            images.Add(new GameImage
            {
                Id = $"human_{i:D3}",
                Url = $"https://picsum.photos/600/400?random={i}",
                Label = "human",
                Category = "general"
            });
        }

        return images;
    }
}
