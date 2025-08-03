<template>
  <div class="min-h-screen gradient-bg p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-20 right-20 w-64 h-64 bg-red-300/10 rounconst makeGuess = (guess: 'ai' | 'human') => {
  const result = timeRushStore.answerQuestion(guess)
  
  lastAnswerCorrect.value = result.isCorrect
  lastPoints.value = result.points
  showFeedback.value = true

  // Immediately load next question (no wait for image)
  setTimeout(() => {
    showFeedback.value = false
    imageLoaded.value = false
  }, 800) // Reduced from 1000ms

  // Check if game ended due to time
  if (timeLeft.value === 0) {
    handleGameEnd()
  }
}animate-pulse"></div>
      <div class="absolute bottom-20 left-20 w-64 h-64 bg-orange-300/10 rounded-full blur-3xl"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-yellow-300/10 rounded-full blur-3xl animate-pulse-slow"></div>
    </div>

    <div class="relative z-10 max-w-2xl mx-auto">
      <!-- Game Not Started -->
      <div v-if="!isActive" class="text-center animate-fade-in">
        <div class="glass-card rounded-3xl p-8 mb-8">
          <div class="text-6xl mb-4">⚡</div>
          <h1 class="text-4xl font-bold text-white mb-4">Zamana Karşı Yarış!</h1>
          <p class="text-white/80 text-lg mb-6">30 saniyede en çok soruyu cevapla!</p>
          
          <div class="grid grid-cols-3 gap-4 mb-8">
            <div class="bg-white/10 rounded-2xl p-4 text-center">
              <div class="text-2xl font-bold text-yellow-300">30s</div>
              <div class="text-white/70 text-sm">Süre</div>
            </div>
            <div class="bg-white/10 rounded-2xl p-4 text-center">
              <div class="text-2xl font-bold text-blue-300">∞</div>
              <div class="text-white/70 text-sm">Soru</div>
            </div>
            <div class="bg-white/10 rounded-2xl p-4 text-center">
              <div class="text-2xl font-bold text-green-300">+Bonus</div>
              <div class="text-white/70 text-sm">Hızlı Puan</div>
            </div>
          </div>

          <button 
            @click="startTimeRush"
            class="btn-primary text-xl px-8 py-4 w-full mb-4"
          >
            🚀 Yarışı Başlat!
          </button>

          <button 
            @click="goHome"
            class="bg-white/10 hover:bg-white/20 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 border border-white/30 w-full"
          >
            🏠 Ana Sayfa
          </button>
        </div>
      </div>

      <!-- Game Active -->
      <div v-if="isActive" class="animate-fade-in">
        <!-- Timer and Stats -->
        <div class="glass-card rounded-3xl p-6 mb-6">
          <div class="flex items-center justify-between mb-4">
            <!-- Timer -->
            <div class="flex items-center space-x-3">
              <div class="relative w-16 h-16">
                <svg class="w-16 h-16 transform -rotate-90" viewBox="0 0 64 64">
                  <circle cx="32" cy="32" r="28" stroke="rgba(255,255,255,0.1)" stroke-width="6" fill="none"/>
                  <circle 
                    cx="32" 
                    cy="32" 
                    r="28" 
                    :stroke="timeLeft <= 5 ? '#ef4444' : timeLeft <= 10 ? '#f59e0b' : '#10b981'"
                    stroke-width="6" 
                    fill="none"
                    stroke-linecap="round"
                    :stroke-dasharray="2 * Math.PI * 28"
                    :stroke-dashoffset="2 * Math.PI * 28 * (1 - timeLeft / totalTime)"
                    class="transition-all duration-1000 ease-linear"
                  />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <span class="text-white font-bold text-lg">{{ timeLeft }}</span>
                </div>
              </div>
              <div>
                <div class="text-white font-semibold">Kalan Süre</div>
                <div class="text-white/70 text-sm">{{ Math.floor(timeLeft / 60) }}:{{ String(timeLeft % 60).padStart(2, '0') }}</div>
              </div>
            </div>

            <!-- Score -->
            <div class="text-right">
              <div class="text-3xl font-bold text-yellow-300">{{ score }}</div>
              <div class="text-white/70 text-sm">Puan</div>
            </div>
          </div>

          <!-- Progress Stats -->
          <div class="grid grid-cols-3 gap-4">
            <div class="text-center">
              <div class="text-xl font-bold text-blue-300">{{ totalAnswered }}</div>
              <div class="text-white/70 text-xs">Toplam</div>
            </div>
            <div class="text-center">
              <div class="text-xl font-bold text-green-300">{{ correctAnswers }}</div>
              <div class="text-white/70 text-xs">Doğru</div>
            </div>
            <div class="text-center">
              <div class="text-xl font-bold text-white">{{ accuracy }}%</div>
              <div class="text-white/70 text-xs">Başarı</div>
            </div>
          </div>
        </div>

        <!-- Current Question -->
        <div class="glass-card rounded-3xl p-6 mb-6">
          <div class="text-center mb-6">
            <div class="text-white/70 text-sm mb-2">Soru #{{ totalAnswered + 1 }}</div>
            <h2 class="text-2xl font-bold text-white mb-4">Bu görsel AI mi İnsan mı?</h2>
            
            <!-- Question Image -->
            <div class="relative mx-auto w-64 h-64 rounded-2xl overflow-hidden mb-6 shadow-2xl">
              <img 
                v-if="currentQuestion"
                :src="currentQuestion.imageUrl" 
                alt="Analiz edilecek görsel"
                class="w-full h-full object-cover transition-all duration-300"
                @load="imageLoaded = true"
                loading="eager"
              />
              <div v-if="!imageLoaded" class="absolute inset-0 bg-white/10 animate-pulse flex items-center justify-center">
                <div class="text-white/50">⚡ Yükleniyor...</div>
              </div>
              
              <!-- Difficulty indicator -->
              <div v-if="currentQuestion" class="absolute top-3 right-3 px-2 py-1 rounded-lg text-xs font-semibold"
                   :class="{
                     'bg-green-500 text-white': currentQuestion.difficulty === 'easy',
                     'bg-yellow-500 text-white': currentQuestion.difficulty === 'medium', 
                     'bg-red-500 text-white': currentQuestion.difficulty === 'hard'
                   }">
                {{ currentQuestion.difficulty === 'easy' ? 'KOLAY' : 
                   currentQuestion.difficulty === 'medium' ? 'ORTA' : 'ZOR' }}
              </div>
            </div>
            
            <!-- Answer Buttons -->
            <div class="grid grid-cols-2 gap-4">
              <button 
                @click="makeGuess('ai')"
                class="bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
              >
                <span>🤖</span>
                <span>AI Yapımı</span>
              </button>
              
              <button 
                @click="makeGuess('human')"
                class="bg-gradient-to-r from-orange-500 to-red-500 hover:from-orange-600 hover:to-red-600 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
              >
                <span>👨</span>
                <span>İnsan Yapımı</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Feedback -->
        <div v-if="showFeedback" class="glass-card rounded-2xl p-3 mb-4 animate-slide-up">
          <div class="text-center">
            <div :class="lastAnswerCorrect ? 'text-green-400' : 'text-red-400'" class="text-xl font-bold mb-1">
              {{ lastAnswerCorrect ? '✅ +' + lastPoints + ' Puan!' : '❌ Yanlış!' }}
            </div>
          </div>
        </div>

        <!-- Pause Button -->
        <div class="text-center">
          <button 
            @click="pauseGame"
            class="bg-white/10 hover:bg-white/20 text-white font-semibold py-2 px-6 rounded-xl transition-all duration-300 border border-white/30"
          >
            ⏸️ Duraklat
          </button>
        </div>
      </div>

      <!-- Game Paused -->
      <!-- <div v-else-if="!isActive && timeLeft < totalTime && timeLeft > 0" class="text-center animate-fade-in">
        <div class="glass-card rounded-3xl p-8">
          <div class="text-6xl mb-4">⏸️</div>
          <h2 class="text-3xl font-bold text-white mb-4">Oyun Duraklatıldı</h2>
          <p class="text-white/80 mb-6">Hazır olduğunda devam et!</p>
          
          <div class="grid grid-cols-2 gap-4">
            <button 
              @click="resumeGame"
              class="btn-primary py-3 px-6"
            >
              ▶️ Devam Et
            </button>
            
            <button 
              @click="endGameEarly"
              class="bg-red-500 hover:bg-red-600 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300"
            >
              🛑 Bitir
            </button>
          </div>
        </div>
      </div> -->
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTimeRushStore } from '../stores/timeRush'
import { useLeaderboardStore } from '../stores/leaderboard'
import { useUserStore } from '../stores/user'

const router = useRouter()
const timeRushStore = useTimeRushStore()
const leaderboardStore = useLeaderboardStore()
const userStore = useUserStore()

// Local state
const imageLoaded = ref(false)
const showFeedback = ref(false)
const lastAnswerCorrect = ref(false)
const lastPoints = ref(0)

// Computed properties from store
const isActive = computed(() => timeRushStore.isActive)
const timeLeft = computed(() => timeRushStore.timeLeft)
const totalTime = computed(() => timeRushStore.totalTime)
const score = computed(() => timeRushStore.score)
const totalAnswered = computed(() => timeRushStore.totalAnswered)
const correctAnswers = computed(() => timeRushStore.correctAnswers)
const accuracy = computed(() => timeRushStore.accuracy)
const currentQuestion = computed(() => timeRushStore.currentQuestion)

const startTimeRush = () => {
  imageLoaded.value = false
  timeRushStore.startGame(30) // 30 saniye
}

const makeGuess = (guess: 'ai' | 'human') => {
  const result = timeRushStore.answerQuestion(guess)
  
  lastAnswerCorrect.value = result.isCorrect
  lastPoints.value = result.points
  showFeedback.value = true
  imageLoaded.value = false

  // Hide feedback after 1 second
  setTimeout(() => {
    showFeedback.value = false
  }, 1000)

  // Check if game ended due to time
  if (timeLeft.value === 0) {
    handleGameEnd()
  }
}

const pauseGame = () => {
  timeRushStore.pauseGame()
}

const handleGameEnd = (results?: any) => {
  const finalResults = results || timeRushStore.endGame()
  
  leaderboardStore.addScore({
    playerName: userStore.userName ? `⚡ ${userStore.userName}` : 'Anonim',
    score: finalResults.score,
    totalQuestions: finalResults.totalAnswered
  })
  
  // Update user stats
  userStore.updateStats({
    score: finalResults.correctAnswers,
    accuracy: finalResults.accuracy,
    gameType: 'timeRush',
    timeRushScore: finalResults.score
  })
  
  // Navigate to time rush results
  router.push({
    name: 'timeRushResult',
    params: {
      score: finalResults.score.toString(),
      answered: finalResults.totalAnswered.toString(),
      correct: finalResults.correctAnswers.toString(),
      accuracy: finalResults.accuracy.toString(),
      qps: finalResults.questionsPerSecond.toString()
    }
  })
}

const goHome = () => {
  router.push('/')
}

// Watch for time end
let gameWatcher: any = null
onMounted(() => {
  gameWatcher = setInterval(() => {
    if (isActive.value && timeLeft.value === 0) {
      handleGameEnd()
    }
  }, 100)
})

onUnmounted(() => {
  timeRushStore.resetGame()
  if (gameWatcher) {
    clearInterval(gameWatcher)
  }
})
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
  border: none;
  cursor: pointer;
}

.btn-primary:hover {
  background: linear-gradient(to right, #2563eb, #1d4ed8);
  transform: scale(1.05);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}

.animate-slide-up {
  animation: slideUp 0.3s ease-out;
}

.animate-pulse-slow {
  animation: pulse 3s infinite;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse {
  0%, 100% { opacity: 0.7; }
  50% { opacity: 0.3; }
}
</style>
