<template>
  <div class="min-h-screen gradient-bg p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-20 right-20 w-64 h-64 bg-red-300/10 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-20 left-20 w-64 h-64 bg-blue-300/10 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-purple-300/10 rounded-full blur-3xl"></div>
    </div>

    <div class="relative z-10 max-w-6xl mx-auto">
      <!-- Home Button -->
      <div class="mb-4 scale-in">
        <button 
          @click="router.push('/')"
          class="home-btn group"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"></path>
          </svg>
          <span>Ana Sayfa</span>
        </button>
      </div>

      <!-- Game Setup -->
      <div v-if="!gameStarted" class="text-center animate-fade-in">
        <div class="glass-card rounded-3xl p-8 mb-8">
          <div class="text-6xl mb-4">⚔️</div>
          <h1 class="text-4xl font-bold text-white mb-4">Düello Modu</h1>
          <p class="text-white/80 text-lg mb-8">Kim daha hızlı ve doğru tahmin eder?</p>
          
          <!-- Player Names Input -->
          <div class="grid grid-cols-2 gap-6 mb-8">
            <div class="text-center">
              <div class="text-blue-300 text-xl font-bold mb-2">👤 Oyuncu 1</div>
              <input 
                v-model="player1Name"
                placeholder="İsim girin"
                class="w-full px-4 py-3 bg-white/10 border border-blue-400/30 rounded-2xl text-white placeholder-white/50 text-center focus:outline-none focus:ring-2 focus:ring-blue-400"
                maxlength="15"
              />
            </div>
            
            <div class="text-center">
              <div class="text-red-300 text-xl font-bold mb-2">👤 Oyuncu 2</div>
              <input 
                v-model="player2Name"
                placeholder="İsim girin"
                class="w-full px-4 py-3 bg-white/10 border border-red-400/30 rounded-2xl text-white placeholder-white/50 text-center focus:outline-none focus:ring-2 focus:ring-red-400"
                maxlength="15"
              />
            </div>
          </div>
          
          <!-- Game Mode Selection -->
          <div class="mb-8">
            <h3 class="text-white text-xl font-bold mb-4">Oyun Modu Seçin</h3>
            <div class="grid grid-cols-3 gap-4">
              <button 
                @click="selectedMode = 'best-of-5'"
                :class="selectedMode === 'best-of-5' ? 'bg-blue-500/50 border-blue-400' : 'bg-white/10 border-white/20'"
                class="p-4 rounded-2xl border-2 transition-all duration-300 hover:scale-105"
              >
                <div class="text-2xl mb-2">🏃‍♂️</div>
                <div class="text-white font-bold">5 Soru</div>
                <div class="text-white/70 text-sm">Hızlı Düello</div>
              </button>
              
              <button 
                @click="selectedMode = 'best-of-10'"
                :class="selectedMode === 'best-of-10' ? 'bg-purple-500/50 border-purple-400' : 'bg-white/10 border-white/20'"
                class="p-4 rounded-2xl border-2 transition-all duration-300 hover:scale-105"
              >
                <div class="text-2xl mb-2">🎯</div>
                <div class="text-white font-bold">10 Soru</div>
                <div class="text-white/70 text-sm">Klasik Düello</div>
              </button>
              
              <button 
                @click="selectedMode = 'time-based'"
                :class="selectedMode === 'time-based' ? 'bg-red-500/50 border-red-400' : 'bg-white/10 border-white/20'"
                class="p-4 rounded-2xl border-2 transition-all duration-300 hover:scale-105"
              >
                <div class="text-2xl mb-2">⏰</div>
                <div class="text-white font-bold">15 Soru</div>
                <div class="text-white/70 text-sm">Maraton Düello</div>
              </button>
            </div>
          </div>
          
          <button 
            @click="startDuel"
            :disabled="!canStartGame"
            class="btn-primary text-xl py-4 px-8 rounded-2xl disabled:opacity-50 disabled:cursor-not-allowed"
          >
            ⚔️ Düelloyu Başlat
          </button>
        </div>
      </div>

      <!-- Game Screen -->
      <div v-else-if="!duelStore.isGameFinished" class="animate-fade-in">
        <!-- Round Header -->
        <div class="glass-card rounded-3xl p-6 mb-6">
          <div class="flex justify-between items-center">
            <div class="text-center">
              <div class="text-3xl font-bold text-white">Round {{ duelStore.currentRound }}</div>
              <div class="text-white/70">/ {{ duelStore.totalQuestions }}</div>
            </div>
            
            <!-- Timer -->
            <div class="text-center">
              <div class="w-20 h-20 mx-auto relative mb-2">
                <svg class="w-20 h-20 transform -rotate-90" viewBox="0 0 80 80">
                  <circle cx="40" cy="40" r="32" stroke="rgba(255,255,255,0.2)" stroke-width="4" fill="none" />
                  <circle 
                    cx="40" 
                    cy="40" 
                    r="32" 
                    :stroke="timeColor" 
                    stroke-width="4" 
                    fill="none"
                    stroke-linecap="round"
                    :stroke-dasharray="timeCircumference"
                    :stroke-dashoffset="timeOffset"
                    class="transition-all duration-1000"
                  />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <span class="text-2xl font-bold text-white">{{ duelStore.currentQuestionTime }}</span>
                </div>
              </div>
              <div class="text-white/70 text-sm">Kalan Süre</div>
            </div>
            
            <div class="text-center">
              <div class="text-white text-lg">{{ duelStore.gameMode.toUpperCase() }}</div>
              <div class="text-white/70 text-sm">Mod</div>
            </div>
          </div>
        </div>

        <!-- Players Status -->
        <div class="grid grid-cols-2 gap-6 mb-6">
          <!-- Player 1 -->
          <div class="glass-card rounded-3xl p-6 border-l-4 border-blue-400">
            <div class="flex justify-between items-center mb-4">
              <div>
                <div class="text-blue-300 text-xl font-bold">{{ duelStore.player1.name }}</div>
                <div class="text-white/70 text-sm flex items-center space-x-2">
                  <span>Skor: {{ duelStore.player1.score }}</span>
                  <span v-if="duelStore.player1.hasAnswered" class="text-green-400">✓</span>
                  <span v-else-if="duelStore.isActive" class="animate-pulse text-yellow-400">⏳</span>
                </div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-white">{{ duelStore.player1.correctAnswers }}</div>
                <div class="text-white/60 text-xs">Doğru</div>
              </div>
            </div>
            
            <!-- Player 1 Buttons -->
            <div v-if="!duelStore.player1.hasAnswered && duelStore.isActive" class="grid grid-cols-2 gap-3">
              <button 
                @click="makeGuess('player1', 'human')"
                :disabled="isAnswering"
                class="bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                👨 İnsan
              </button>
              <button 
                @click="makeGuess('player1', 'ai')"
                :disabled="isAnswering"
                class="bg-purple-600 hover:bg-purple-700 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                🤖 AI
              </button>
            </div>
          </div>

          <!-- Player 2 -->
          <div class="glass-card rounded-3xl p-6 border-l-4 border-red-400">
            <div class="flex justify-between items-center mb-4">
              <div>
                <div class="text-red-300 text-xl font-bold">{{ duelStore.player2.name }}</div>
                <div class="text-white/70 text-sm flex items-center space-x-2">
                  <span>Skor: {{ duelStore.player2.score }}</span>
                  <span v-if="duelStore.player2.hasAnswered" class="text-green-400">✓</span>
                  <span v-else-if="duelStore.isActive" class="animate-pulse text-yellow-400">⏳</span>
                </div>
              </div>
              <div class="text-center">
                <div class="text-2xl font-bold text-white">{{ duelStore.player2.correctAnswers }}</div>
                <div class="text-white/60 text-xs">Doğru</div>
              </div>
            </div>
            
            <!-- Player 2 Buttons -->
            <div v-if="!duelStore.player2.hasAnswered && duelStore.isActive" class="grid grid-cols-2 gap-3">
              <button 
                @click="makeGuess('player2', 'human')"
                :disabled="isAnswering"
                class="bg-red-600 hover:bg-red-700 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                👨 İnsan
              </button>
              <button 
                @click="makeGuess('player2', 'ai')"
                :disabled="isAnswering"
                class="bg-orange-600 hover:bg-orange-700 text-white font-semibold py-3 px-4 rounded-xl transition-all duration-300 transform hover:scale-105 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                🤖 AI
              </button>
            </div>
          </div>
        </div>

        <!-- Current Question Image -->
        <div v-if="duelStore.currentQuestion" class="glass-card rounded-3xl p-6 text-center">
          <div class="text-white/70 text-sm mb-4">Soru {{ duelStore.currentRound }}</div>
          <h2 class="text-2xl font-bold text-white mb-6">Bu görsel AI mi İnsan mı?</h2>
          
          <div class="relative mx-auto w-80 h-64 rounded-2xl overflow-hidden mb-6 shadow-2xl">
            <img 
              :src="duelStore.currentQuestion.imageUrl" 
              alt="Düello sorusu"
              class="w-full h-full object-cover"
              @load="imageLoaded = true"
              loading="eager"
            />
            <div v-if="!imageLoaded" class="absolute inset-0 bg-white/10 animate-pulse flex items-center justify-center">
              <div class="text-white/50">Yükleniyor...</div>
            </div>
            
            <!-- Difficulty Badge -->
            <div class="absolute top-3 right-3 px-3 py-1 rounded-lg text-xs font-semibold"
                 :class="{
                   'bg-green-500 text-white': duelStore.currentQuestion.difficulty === 'easy',
                   'bg-yellow-500 text-white': duelStore.currentQuestion.difficulty === 'medium', 
                   'bg-red-500 text-white': duelStore.currentQuestion.difficulty === 'hard'
                 }">
              {{ duelStore.currentQuestion.difficulty === 'easy' ? 'KOLAY' : 
                 duelStore.currentQuestion.difficulty === 'medium' ? 'ORTA' : 'ZOR' }}
            </div>
          </div>
          
          <!-- Round Results -->
          <div v-if="duelStore.bothPlayersAnswered && duelStore.isActive" class="mt-6 grid grid-cols-2 gap-6">
            <div class="p-4 rounded-2xl" :class="getResultClass(duelStore.player1)">
              <div class="font-bold text-lg">{{ duelStore.player1.name }}</div>
              <div class="text-sm">
                {{ duelStore.player1.lastAnswer === 'ai' ? 'AI' : 'İnsan' }} 
                ({{ duelStore.player1.lastAnswerTime }}s)
              </div>
              <div class="text-xs mt-1">
                {{ isCorrectAnswer(duelStore.player1) ? '✅ Doğru!' : '❌ Yanlış!' }}
              </div>
            </div>
            
            <div class="p-4 rounded-2xl" :class="getResultClass(duelStore.player2)">
              <div class="font-bold text-lg">{{ duelStore.player2.name }}</div>
              <div class="text-sm">
                {{ duelStore.player2.lastAnswer === 'ai' ? 'AI' : 'İnsan' }} 
                ({{ duelStore.player2.lastAnswerTime }}s)
              </div>
              <div class="text-xs mt-1">
                {{ isCorrectAnswer(duelStore.player2) ? '✅ Doğru!' : '❌ Yanlış!' }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Game Results -->
      <div v-else class="text-center animate-fade-in">
        <div class="glass-card rounded-3xl p-8">
          <div class="text-6xl mb-4">
            {{ duelStore.winner ? '🏆' : '🤝' }}
          </div>
          
          <h1 class="text-4xl font-bold text-white mb-4">
            {{ duelStore.winner ? `${duelStore.winner.name} Kazandı!` : 'Beraberlik!' }}
          </h1>
          
          <div class="grid grid-cols-2 gap-8 mb-8">
            <!-- Player 1 Results -->
            <div class="p-6 rounded-2xl" :class="duelStore.winner?.id === 'player1' ? 'bg-green-500/20 border border-green-400' : 'bg-white/10'">
              <div class="text-2xl font-bold text-blue-300 mb-4">{{ duelStore.player1.name }}</div>
              <div class="space-y-2">
                <div class="flex justify-between">
                  <span class="text-white/70">Final Skor:</span>
                  <span class="text-white font-bold">{{ duelStore.player1.score }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Doğru Cevap:</span>
                  <span class="text-green-400 font-bold">{{ duelStore.player1.correctAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Yanlış Cevap:</span>
                  <span class="text-red-400 font-bold">{{ duelStore.player1.wrongAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Ortalama Süre:</span>
                  <span class="text-white font-bold">{{ Math.round(duelStore.player1.averageTime) }}s</span>
                </div>
              </div>
            </div>
            
            <!-- Player 2 Results -->
            <div class="p-6 rounded-2xl" :class="duelStore.winner?.id === 'player2' ? 'bg-green-500/20 border border-green-400' : 'bg-white/10'">
              <div class="text-2xl font-bold text-red-300 mb-4">{{ duelStore.player2.name }}</div>
              <div class="space-y-2">
                <div class="flex justify-between">
                  <span class="text-white/70">Final Skor:</span>
                  <span class="text-white font-bold">{{ duelStore.player2.score }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Doğru Cevap:</span>
                  <span class="text-green-400 font-bold">{{ duelStore.player2.correctAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Yanlış Cevap:</span>
                  <span class="text-red-400 font-bold">{{ duelStore.player2.wrongAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-white/70">Ortalama Süre:</span>
                  <span class="text-white font-bold">{{ Math.round(duelStore.player2.averageTime) }}s</span>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Action Buttons -->
          <div class="grid grid-cols-2 gap-4">
            <button 
              @click="startNewDuel"
              class="btn-primary py-3 px-6 rounded-2xl"
            >
              🔄 Tekrar Oyna
            </button>
            
            <button 
              @click="goHome"
              class="bg-white/10 hover:bg-white/20 text-white font-semibold py-3 px-6 rounded-2xl transition-all duration-300"
            >
              🏠 Ana Sayfa
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useDuelStore } from '../stores/duel'

const router = useRouter()
const duelStore = useDuelStore()

// Local state
const gameStarted = ref(false)
const imageLoaded = ref(false)
const isAnswering = ref(false)
const selectedMode = ref<'best-of-5' | 'best-of-10' | 'time-based'>('best-of-5')
const player1Name = ref('')
const player2Name = ref('')

// Computed
const canStartGame = computed(() => {
  return player1Name.value.trim().length > 0 && 
         player2Name.value.trim().length > 0 && 
         selectedMode.value
})

const timeCircumference = computed(() => 2 * Math.PI * 32)
const timeProgress = computed(() => duelStore.currentQuestionTime / duelStore.timeLimit)
const timeOffset = computed(() => timeCircumference.value * (1 - timeProgress.value))
const timeColor = computed(() => {
  const progress = timeProgress.value
  if (progress > 0.5) return '#10b981' // green
  if (progress > 0.2) return '#f59e0b' // yellow
  return '#ef4444' // red
})

// Methods
const startDuel = () => {
  duelStore.initializeDuel(
    selectedMode.value, 
    player1Name.value.trim() || 'Oyuncu 1', 
    player2Name.value.trim() || 'Oyuncu 2'
  )
  
  duelStore.setPlayerReady('player1', true)
  duelStore.setPlayerReady('player2', true)
  
  gameStarted.value = true
  
  // Start first round
  setTimeout(() => {
    duelStore.startDuelRound()
  }, 1000)
}

const makeGuess = (playerId: string, answer: 'ai' | 'human') => {
  if (isAnswering.value) return
  
  isAnswering.value = true
  const timeSpent = duelStore.timeLimit - duelStore.currentQuestionTime
  
  duelStore.submitAnswer(playerId, answer, timeSpent)
  
  setTimeout(() => {
    isAnswering.value = false
  }, 500)
}

const isCorrectAnswer = (player: any) => {
  if (!duelStore.currentQuestion || !player.lastAnswer) return false
  
  return (player.lastAnswer === 'ai' && duelStore.currentQuestion.isAI) || 
         (player.lastAnswer === 'human' && !duelStore.currentQuestion.isAI)
}

const getResultClass = (player: any) => {
  const isCorrect = isCorrectAnswer(player)
  return isCorrect ? 'bg-green-500/20 border border-green-400' : 'bg-red-500/20 border border-red-400'
}

const startNewDuel = () => {
  duelStore.resetDuel()
  gameStarted.value = false
  imageLoaded.value = false
  isAnswering.value = false
}

const goHome = () => {
  duelStore.resetDuel()
  router.push('/')
}

// Cleanup
onUnmounted(() => {
  duelStore.resetDuel()
})
</script>

<style scoped>
.gradient-bg {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

/* Home Button */
.home-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(20px);
  color: white;
  padding: 0.75rem 1.5rem;
  border-radius: 1rem;
  transition: all 0.3s ease;
  font-weight: 500;
  cursor: pointer;
}

.home-btn:hover {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
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

.animate-fade-in {
  animation: fadeIn 1s ease-out;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px) scale(0.95); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}
</style>
