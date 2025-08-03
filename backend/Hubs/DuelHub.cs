using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using AivsHuman.Api.Services;

namespace AivsHuman.Api.Hubs
{
    public class DuelHub : Hub
    {
        private static readonly ConcurrentDictionary<string, DuelRoom> _rooms = new();
        private readonly IImageService _imageService;
        private readonly IGameService _gameService;

        public DuelHub(IImageService imageService, IGameService gameService)
        {
            _imageService = imageService;
            _gameService = gameService;
        }

        public async Task CreateRoom(string roomName, string playerName)
        {
            Console.WriteLine($"CreateRoom called - RoomName: {roomName}, PlayerName: {playerName}");
            
            var roomId = GenerateRoomId();
            
            var room = new DuelRoom
            {
                Id = roomId,
                Name = roomName,
                Player1 = new DuelPlayer { Id = Context.ConnectionId, Name = playerName, IsHost = true },
                CreatedAt = DateTime.UtcNow,
                Status = DuelRoomStatus.WaitingForPlayer,
                TotalRounds = 10,
                TimeLimit = 30
            };

            _rooms[roomId] = room;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            
            Console.WriteLine($"Room created: {roomId}");
            await Clients.Caller.SendAsync("RoomJoined", new { room, playerName });
        }

        public async Task JoinRoom(string roomId, string playerName)
        {
            Console.WriteLine($"JoinRoom called - RoomId: {roomId}, PlayerName: {playerName}");
            
            if (!_rooms.TryGetValue(roomId, out var room))
            {
                Console.WriteLine($"Room not found: {roomId}");
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            if (room.Player2 != null)
            {
                Console.WriteLine($"Room full: {roomId}");
                await Clients.Caller.SendAsync("Error", "Room is full");
                return;
            }

            room.Player2 = new DuelPlayer { Id = Context.ConnectionId, Name = playerName, IsHost = false };
            room.Status = DuelRoomStatus.BothPlayersJoined;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            
            Console.WriteLine($"Player joined room: {playerName} -> {roomId}");
            
            await Clients.Caller.SendAsync("RoomJoined", new { room, playerName });
            await Clients.GroupExcept(roomId, Context.ConnectionId).SendAsync("PlayerJoined", new { room });
        }

        public async Task SetReady(string roomId)
        {
            Console.WriteLine($"SetReady called - RoomId: {roomId}");
            
            if (!_rooms.TryGetValue(roomId, out var room))
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            var player = GetPlayerByConnectionId(room, Context.ConnectionId);
            if (player != null)
            {
                player.IsReady = !player.IsReady; // Toggle ready state
                
                Console.WriteLine($"Player {(player.IsReady ? "ready" : "not ready")}: {player.Name}");
                await Clients.Group(roomId).SendAsync("PlayerReady", new { room });

                if (room.Player1?.IsReady == true && room.Player2?.IsReady == true)
                {
                    Console.WriteLine($"Both players ready - starting game in room: {roomId}");
                    await StartGame(roomId);
                }
            }
        }

        public async Task StartGame(string roomId)
        {
            Console.WriteLine($"StartGame called - RoomId: {roomId}");
            
            if (!_rooms.TryGetValue(roomId, out var room)) return;

            room.Status = DuelRoomStatus.InProgress;
            room.CurrentRound = 1;
            
            // İlk soruyu gönder
            await SendNextQuestion(roomId);
            
            await Clients.Group(roomId).SendAsync("GameStarted", new { room });
        }

        public async Task SubmitAnswer(string roomId, string answer)
        {
            Console.WriteLine($"SubmitAnswer called - RoomId: {roomId}, Answer: {answer}");
            
            if (!_rooms.TryGetValue(roomId, out var room)) 
            {
                await Clients.Caller.SendAsync("Error", "Room not found");
                return;
            }

            var player = GetPlayerByConnectionId(room, Context.ConnectionId);
            if (player == null)
            {
                await Clients.Caller.SendAsync("Error", "Player not found");
                return;
            }

            if (room.CurrentQuestion == null)
            {
                await Clients.Caller.SendAsync("Error", "No active question");
                return;
            }

            if (player.HasAnsweredCurrentRound)
            {
                await Clients.Caller.SendAsync("Error", "Already answered this round");
                return;
            }

            // Süre kontrolü - 30 saniye geçtiyse cevap kabul edilmez
            var elapsedTime = DateTime.UtcNow.Subtract(room.RoundStartTime).TotalSeconds;
            if (elapsedTime > room.TimeLimit)
            {
                Console.WriteLine($"Time's up! Player {player.Name} tried to answer after {elapsedTime} seconds");
                await Clients.Caller.SendAsync("Error", "Time's up! No more answers accepted for this round.");
                return;
            }

            // Cevabı kaydet
            player.HasAnsweredCurrentRound = true;
            player.LastAnswer = answer;
            player.LastAnswerTime = DateTime.UtcNow.Subtract(room.RoundStartTime).TotalSeconds;

            // Cevabı kontrol et
            bool isCorrect = (answer.ToLower() == "ai" && room.CurrentQuestion.IsAI) || 
                           (answer.ToLower() == "human" && !room.CurrentQuestion.IsAI);

            if (isCorrect)
            {
                player.Score += 10;
                player.CorrectAnswers++;
            }
            else
            {
                player.WrongAnswers++;
            }

            Console.WriteLine($"Player {player.Name} answered {answer}, correct: {isCorrect}, score: {player.Score}");

            // Cevabı tüm oyunculara bildir
            await Clients.Group(roomId).SendAsync("PlayerAnswered", new { 
                playerId = player.Id, 
                playerName = player.Name,
                answer, 
                isCorrect, 
                score = player.Score,
                room 
            });

            // Her iki oyuncu da cevapladı mı kontrol et
            if (room.Player1?.HasAnsweredCurrentRound == true && room.Player2?.HasAnsweredCurrentRound == true)
            {
                Console.WriteLine($"Both players answered - moving to next question");
                
                // Kısa bir bekleme süresi ver (sonuçları görmek için)
                await Task.Delay(2000);
                
                // Sonraki soruya geç
                room.CurrentRound++;
                if (room.CurrentRound <= room.TotalRounds)
                {
                    await SendNextQuestion(roomId);
                }
                else
                {
                    // Oyun bitti
                    room.Status = DuelRoomStatus.Finished;
                    Console.WriteLine($"Game finished - Room: {roomId}");
                    await Clients.Group(roomId).SendAsync("GameFinished", new { room });
                }
            }
        }

        private async Task SendNextQuestion(string roomId)
        {
            Console.WriteLine($"SendNextQuestion started - RoomId: {roomId}");
            
            if (!_rooms.TryGetValue(roomId, out var room)) 
            {
                Console.WriteLine($"Room not found in SendNextQuestion: {roomId}");
                return;
            }

            try
            {
                Console.WriteLine($"Getting random images...");
                // Rastgele bir soru al
                var images = await _imageService.GetRandomImagesAsync(1, "all", "mixed");
                Console.WriteLine($"Images received: {images?.Count ?? 0}");
                
                if (images == null || !images.Any()) 
                {
                    Console.WriteLine("No images available!");
                    return;
                }

                var image = images.First();
                Console.WriteLine($"Selected image: {image.Url}, Label: {image.Label}");
                
                var question = new DuelQuestion
                {
                    Id = Guid.NewGuid().ToString(),
                    ImageUrl = image.Url,
                    IsAI = image.Label.ToLower() == "ai",
                    Category = image.Category,
                    Difficulty = image.Difficulty
                };

                room.CurrentQuestion = question;
                room.RoundStartTime = DateTime.UtcNow; // Round başlangıç zamanını kaydet
                
                // Reset player answers for new round
                if (room.Player1 != null) room.Player1.HasAnsweredCurrentRound = false;
                if (room.Player2 != null) room.Player2.HasAnsweredCurrentRound = false;
                
                Console.WriteLine($"Sending question - Room: {roomId}, Round: {room.CurrentRound}, Image: {question.ImageUrl}, IsAI: {question.IsAI}");
                
                // Soruyu tüm oyunculara gönder
                await Clients.Group(roomId).SendAsync("QuestionReceived", new { 
                    question, 
                    round = room.CurrentRound,
                    timeLimit = room.TimeLimit 
                });
                Console.WriteLine($"Question sent successfully to room: {roomId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending question to room {roomId}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public async Task LeaveRoom(string roomId)
        {
            Console.WriteLine($"LeaveRoom called - RoomId: {roomId}");
            
            if (!_rooms.TryGetValue(roomId, out var room)) return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

            var player = GetPlayerByConnectionId(room, Context.ConnectionId);
            if (player != null)
            {
                if (player.Id == room.Player1?.Id)
                {
                    room.Player1 = null;
                }
                else if (player.Id == room.Player2?.Id)
                {
                    room.Player2 = null;
                }

                if (room.Player1 == null && room.Player2 == null)
                {
                    _rooms.TryRemove(roomId, out _);
                    await Clients.Group(roomId).SendAsync("RoomClosed");
                }
                else
                {
                    await Clients.Group(roomId).SendAsync("PlayerLeft", new { room });
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var room in _rooms.Values.ToList())
            {
                if (room.Player1?.Id == Context.ConnectionId || room.Player2?.Id == Context.ConnectionId)
                {
                    await LeaveRoom(room.Id);
                    break;
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private DuelPlayer? GetPlayerByConnectionId(DuelRoom room, string connectionId)
        {
            if (room.Player1?.Id == connectionId) return room.Player1;
            if (room.Player2?.Id == connectionId) return room.Player2;
            return null;
        }

        private string GenerateRoomId()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }
    }

    public class DuelRoom
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DuelPlayer? Player1 { get; set; }
        public DuelPlayer? Player2 { get; set; }
        public DuelRoomStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RoundStartTime { get; set; }
        public int CurrentRound { get; set; }
        public int TotalRounds { get; set; } = 10;
        public int TimeLimit { get; set; } = 30; // seconds
        public List<DuelQuestion> Questions { get; set; } = new();
        public DuelQuestion? CurrentQuestion { get; set; }
    }

    public class DuelPlayer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsHost { get; set; }
        public bool IsReady { get; set; }
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public bool HasAnsweredCurrentRound { get; set; }
        public string? LastAnswer { get; set; }
        public double LastAnswerTime { get; set; }
    }

    public class DuelQuestion
    {
        public string Id { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsAI { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
    }

    public enum DuelRoomStatus
    {
        WaitingForPlayer,
        BothPlayersJoined,
        InProgress,
        Finished
    }
}