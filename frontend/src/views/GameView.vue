<template>
  <div class="min-h-screen gradient-bg p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-20 right-20 w-64 h-64 bg-white/5 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-20 left-20 w-64 h-64 bg-blue-300/10 rounded-full blur-3xl"></div>
    </div>

    <div class="relative z-10 max-w-4xl mx-auto">
      <!-- Header -->
      <div class="text-center mb-8 animate-fade-in">
        <div class="inline-flex items-center space-x-3 bg-white/10 backdrop-blur-md rounded-2xl px-6 py-3 border border-white/20">
          <div class="w-3 h-3 bg-green-400 rounded-full animate-pulse"></div>
          <h1 class="text-2xl font-bold text-white">Görsel Mod</h1>
        </div>
        <p class="text-white/80 mt-4 text-lg">Bu görsel AI tarafından mı üretildi, yoksa insan tarafından mı çekildi?</p>
        
        <!-- Progress Bar -->
        <div class="mt-6 max-w-md mx-auto">
          <div class="flex justify-between text-white/80 text-sm mb-2">
            <span>İlerleme</span>
            <span>{{ currentQuestion }} / {{ totalQuestions }}</span>
          </div>
          <div class="w-full bg-white/20 rounded-full h-3 backdrop-blur-sm">
            <div 
              class="bg-gradient-to-r from-blue-400 to-purple-500 h-3 rounded-full transition-all duration-500 ease-out"
              :style="{ width: `${(currentQuestion / totalQuestions) * 100}%` }"
            ></div>
          </div>
        </div>
      </div>
      
      <!-- Image Card -->
      <div class="glass-card rounded-3xl p-6 mb-8 animate-slide-up">
        <div v-if="currentImage" class="relative">
          <img 
            :src="currentImage.url" 
            :alt="`Soru ${currentQuestion}`"
            class="w-full max-h-96 object-cover rounded-2xl shadow-2xl transition-transform duration-300 hover:scale-105"
            @load="imageLoaded = true"
            loading="eager"
          />
          
          <!-- Loading Overlay -->
          <div v-if="!imageLoaded" class="absolute inset-0 bg-gray-200 rounded-2xl animate-pulse flex items-center justify-center">
            <div class="text-gray-400">⚡ Yükleniyor...</div>
          </div>
          
          <!-- Image Number Badge -->
          <div class="absolute top-4 right-4 bg-black/50 backdrop-blur-sm text-white px-3 py-1 rounded-full text-sm font-medium">
            {{ currentQuestion }}/{{ totalQuestions }}
          </div>
        </div>
      </div>
      
      <!-- Action Buttons -->
      <div v-if="!answered" class="flex gap-6 mb-8 animate-slide-up">
        <button 
          @click="makeGuess('human')"
          class="flex-1 btn-human relative overflow-hidden group"
        >
          <span class="relative z-10 flex items-center justify-center space-x-3">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
            </svg>
            <span class="text-lg font-bold">İnsan</span>
          </span>
        </button>
        
        <button 
          @click="makeGuess('ai')"
          class="flex-1 btn-ai relative overflow-hidden group"
        >
          <span class="relative z-10 flex items-center justify-center space-x-3">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
            </svg>
            <span class="text-lg font-bold">AI</span>
          </span>
        </button>
      </div>
      
      <!-- Answer Feedback -->
      <div v-if="answered" class="text-center mb-8 animate-fade-in">
        <div class="glass-card rounded-2xl p-4">
          <div :class="isCorrect ? 'text-green-400' : 'text-red-400'" class="text-2xl font-bold mb-3 animate-bounce">
            {{ isCorrect ? '🎉 Doğru!' : '😅 Yanlış!' }}
          </div>
          
          <div class="text-white/90 mb-4">
            Bu <strong class="text-yellow-300">{{ currentImage?.label === 'ai' ? 'AI' : 'İnsan' }}</strong> yapımıydı.
          </div>
          
          <!-- Score Display -->
          <div class="flex justify-center space-x-6 mb-4">
            <div class="text-center">
              <div class="text-xl font-bold text-green-400">{{ correctAnswers }}</div>
              <div class="text-white/60 text-xs">Doğru</div>
            </div>
            <div class="text-center">
              <div class="text-xl font-bold text-red-400">{{ wrongAnswers }}</div>
              <div class="text-white/60 text-xs">Yanlış</div>
            </div>
          </div>
          
          <button 
            @click="nextQuestion"
            class="btn-primary px-6 py-2"
          >
            {{ currentQuestion < totalQuestions ? '⏭️ Sonraki Soru' : '🎯 Sonuçları Gör' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '../stores/game'
import { useLeaderboardStore } from '../stores/leaderboard'
import { useUserStore } from '../stores/user'

const router = useRouter()
const gameStore = useGameStore()
const leaderboardStore = useLeaderboardStore()
const userStore = useUserStore()

const currentQuestion = ref(1)
const totalQuestions = ref(10)
const answered = ref(false)
const isCorrect = ref(false)
const currentImage = ref<any>(null)
const imageLoaded = ref(false)

const correctAnswers = ref(0)
const wrongAnswers = ref(0)

// Mock data for MVP
const mockImages = [
  { id: 1, url: 'https://picsum.photos/800/600?random=1', label: 'human' },
  { id: 2, url: 'https://picsum.photos/800/600?random=2', label: 'ai' },
  { id: 3, url: 'https://picsum.photos/800/600?random=3', label: 'human' },
  { id: 4, url: 'https://picsum.photos/800/600?random=4', label: 'ai' },
  { id: 5, url: 'https://picsum.photos/800/600?random=5', label: 'human' },
  { id: 6, url: 'https://picsum.photos/800/600?random=6', label: 'ai' },
  { id: 7, url: 'https://picsum.photos/800/600?random=7', label: 'human' },
  { id: 8, url: 'https://picsum.photos/800/600?random=8', label: 'ai' },
  { id: 9, url: 'https://picsum.photos/800/600?random=9', label: 'human' },
  { id: 10, url: 'https://picsum.photos/800/600?random=10', label: 'ai' },
]

onMounted(() => {
  loadQuestion()
  preloadNextImages() // İlk soru yüklendiğinde bir sonraki soruları preload et
})

const loadQuestion = () => {
  if (currentQuestion.value <= totalQuestions.value) {
    currentImage.value = mockImages[currentQuestion.value - 1]
    answered.value = false
    imageLoaded.value = false
  }
}

const makeGuess = (guess: 'ai' | 'human') => {
  answered.value = true
  isCorrect.value = guess === currentImage.value.label
  
  if (isCorrect.value) {
    correctAnswers.value++
  } else {
    wrongAnswers.value++
  }
}

const nextQuestion = () => {
  setTimeout(() => {
    if (currentQuestion.value < totalQuestions.value) {
      currentQuestion.value++
      loadQuestion()
      preloadNextImages() // Bir sonraki soruları önceden yükle
    } else {
      // Game finished - add score to leaderboard with user name
      const accuracy = (correctAnswers.value / totalQuestions.value) * 100
      
      leaderboardStore.addScore({
        playerName: userStore.userName || 'Anonim',
        score: correctAnswers.value,
        totalQuestions: totalQuestions.value
      })
      
      // Update user stats
      userStore.updateStats({
        score: correctAnswers.value,
        accuracy: accuracy,
        gameType: 'normal'
      })
      
      // Set game results and go to results page
      gameStore.setGameResults({
        correct: correctAnswers.value,
        wrong: wrongAnswers.value,
        total: totalQuestions.value
      })
      router.push('/result')
    }
  }, 800) // TimeRushView ile aynı süre
}

// Bir sonraki soruları önceden yükle
const preloadNextImages = () => {
  const preloadCount = 3 // 3 resmi önceden yükle
  
  for (let i = 1; i <= preloadCount; i++) {
    const nextIndex = currentQuestion.value + i - 1
    if (nextIndex < mockImages.length) {
      const img = new Image()
      img.src = mockImages[nextIndex].url
    }
  }
}
</script>

<style scoped>
.gradient-bg {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.glass-card {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 0 8px 32px 0 rgba(31, 38, 135, 0.37);
}

.btn-primary {
  background: linear-gradient(to right, #3b82f6, #2563eb);
  color: white;
  font-weight: 600;
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  transform: scale(1);
}

.btn-primary:hover {
  background: linear-gradient(to right, #2563eb, #1d4ed8);
  transform: scale(1.05);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.btn-primary:active {
  transform: scale(0.95);
}

.btn-ai {
  background: linear-gradient(to right, #ef4444, #dc2626);
  color: white;
  font-weight: 600;
  padding: 1rem 2rem;
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  transform: scale(1);
}

.btn-ai:hover {
  background: linear-gradient(to right, #dc2626, #b91c1c);
  transform: scale(1.05);
}

.btn-ai:active {
  transform: scale(0.95);
}

.btn-human {
  background: linear-gradient(to right, #10b981, #059669);
  color: white;
  font-weight: 600;
  padding: 1rem 2rem;
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  transform: scale(1);
}

.btn-human:hover {
  background: linear-gradient(to right, #059669, #047857);
  transform: scale(1.05);
}

.btn-human:active {
  transform: scale(0.95);
}

.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}

.animate-slide-up {
  animation: slideUp 0.6s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
