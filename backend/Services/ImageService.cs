using AivsHuman.Api.Models;

namespace AivsHuman.Api.Services;

public interface IImageService
{
    Task<List<GameImage>> GetRandomImagesAsync(int count = 10, string? category = null, string? difficulty = null);
    Task<GameImage?> GetImageByIdAsync(string id);
    Task<List<GameImage>> GetImagesByCategoryAsync(string category, int count = 20);
    Task<List<string>> GetCategoriesAsync();
    Task<ImageStats> GetImageStatsAsync();
    Task<List<GameImage>> GetDuelImagesAsync(int count = 5);
}

public class ImageService : IImageService
{
    private readonly List<GameImage> _images;
    private readonly Random _random = new();

    public ImageService()
    {
        _images = InitializeImageDatabase();
        Console.WriteLine($"ImageService initialized with {_images.Count} images");
    }

    public Task<List<GameImage>> GetRandomImagesAsync(int count = 10, string? category = null, string? difficulty = null)
    {
        var query = _images.AsQueryable();

        if (!string.IsNullOrEmpty(category) && !category.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(difficulty) && !difficulty.Equals("mixed", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => x.Difficulty.Equals(difficulty, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.OrderBy(x => _random.Next()).Take(count).ToList();
        return Task.FromResult(result);
    }

    public Task<GameImage?> GetImageByIdAsync(string id)
    {
        var image = _images.FirstOrDefault(img => img.Id == id);
        return Task.FromResult(image);
    }

    public Task<List<GameImage>> GetImagesByCategoryAsync(string category, int count = 20)
    {
        var result = _images
            .Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => _random.Next())
            .Take(count)
            .ToList();
        
        return Task.FromResult(result);
    }

    public Task<List<string>> GetCategoriesAsync()
    {
        var categories = _images.Select(x => x.Category).Distinct().ToList();
        return Task.FromResult(categories);
    }

    public Task<ImageStats> GetImageStatsAsync()
    {
        var stats = new ImageStats
        {
            TotalImages = _images.Count,
            AiImages = _images.Count(x => x.Label == "ai"),
            HumanImages = _images.Count(x => x.Label == "human"),
            CategoriesCount = _images.Select(x => x.Category).Distinct().Count(),
            CategoryBreakdown = _images
                .GroupBy(x => x.Category)
                .ToDictionary(g => g.Key, g => g.Count()),
            DifficultyBreakdown = _images
                .GroupBy(x => x.Difficulty)
                .ToDictionary(g => g.Key, g => g.Count())
        };
        
        return Task.FromResult(stats);
    }

    public Task<List<GameImage>> GetDuelImagesAsync(int count = 5)
    {
        // Düello için özel olarak seçilmiş, dengeli resimler
        var balancedImages = _images
            .GroupBy(x => x.Category)
            .SelectMany(g => g.OrderBy(x => _random.Next()).Take(2)) // Her kategoriden 2 resim
            .OrderBy(x => _random.Next())
            .Take(count)
            .ToList();
            
        return Task.FromResult(balancedImages);
    }

    private static List<GameImage> InitializeImageDatabase()
    {
        var images = new List<GameImage>();
        var categories = new[] { "portrait", "landscape", "object", "abstract", "architecture" };
        var difficulties = new[] { "easy", "medium", "hard" };
        var random = new Random(); // Local random for static method

        int imageId = 1;

        // AI Generated Images
        foreach (var category in categories)
        {
            foreach (var difficulty in difficulties)
            {
                for (int i = 1; i <= 20; i++) // Her kategori-zorluk kombinasyonu için 20 resim
                {
                    images.Add(new GameImage
                    {
                        Id = $"ai_{category}_{difficulty}_{i:D2}",
                        Url = $"https://picsum.photos/600/450?random={imageId}",
                        Label = "ai",
                        Category = category,
                        Difficulty = difficulty,
                        Tags = GenerateTags(category, "ai", difficulty),
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                        Source = GetAiSource(difficulty),
                        Description = $"AI-generated {category} image with {difficulty} difficulty level"
                    });
                    imageId++;
                }
            }
        }

        // Human Created Images
        foreach (var category in categories)
        {
            foreach (var difficulty in difficulties)
            {
                for (int i = 1; i <= 20; i++)
                {
                    images.Add(new GameImage
                    {
                        Id = $"human_{category}_{difficulty}_{i:D2}",
                        Url = $"https://picsum.photos/600/450?random={imageId}",
                        Label = "human",
                        Category = category,
                        Difficulty = difficulty,
                        Tags = GenerateTags(category, "human", difficulty),
                        CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                        Source = GetHumanSource(category),
                        Description = $"Human-created {category} image with {difficulty} difficulty level"
                    });
                    imageId++;
                }
            }
        }

        return images;
    }

    private static List<string> GenerateTags(string category, string type, string difficulty)
    {
        var baseTags = new List<string> { category, type, difficulty };
        
        var categoryTags = category switch
        {
            "portrait" => new[] { "face", "person", "photography", "studio" },
            "landscape" => new[] { "nature", "outdoor", "scenery", "mountains" },
            "object" => new[] { "still-life", "product", "close-up", "detail" },
            "abstract" => new[] { "art", "creative", "patterns", "digital" },
            "architecture" => new[] { "building", "structure", "urban", "design" },
            _ => new[] { "general", "misc" }
        };

        baseTags.AddRange(categoryTags);
        return baseTags;
    }

    private static string GetAiSource(string difficulty)
    {
        return difficulty switch
        {
            "easy" => "DALL-E 2",
            "medium" => "Midjourney",
            "hard" => "Stable Diffusion",
            _ => "AI Generator"
        };
    }

    private static string GetHumanSource(string category)
    {
        return category switch
        {
            "portrait" => "Professional Photography",
            "landscape" => "Nature Photography",
            "object" => "Product Photography",
            "abstract" => "Digital Art",
            "architecture" => "Architectural Photography",
            _ => "Photography"
        };
    }
}
