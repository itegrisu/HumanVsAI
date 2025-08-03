<template>
  <div class="min-h-screen modern-gradient p-4 relative overflow-hidden">
    <!-- Enhanced Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="floating-orb orb-1 bg-red-glow"></div>
      <div class="floating-orb orb-2 bg-orange-glow"></div>
      <div class="floating-orb orb-3 bg-yellow-glow"></div>
      <div class="grid-pattern"></div>
      <div class="speed-lines"></div>
    </div>

    <div class="relative z-10 max-w-3xl mx-auto">
      <!-- Game Not Started - Enhanced -->
      <div v-if="!isActive" class="text-center scale-in">
        <div class="modern-card rounded-3xl p-8 mb-8 time-rush-intro">
          <div class="time-rush-icon mb-6">
            <div class="icon-container">
              <div class="lightning-bolt">⚡</div>
              <div class="energy-rings"></div>
            </div>
          </div>
          
          <h1 class="text-5xl font-bold text-white mb-4 gradient-text time-rush-title">
            Zamana Karşı Yarış!
          </h1>
          <p class="text-white/90 text-xl mb-8 font-medium">30 saniyede en çok soruyu cevapla ve hızlı bonusları kazan!</p>
          
          <!-- Enhanced Stats Preview -->
          <div class="grid grid-cols-3 gap-6 mb-8">
            <div class="stat-preview time-stat">
              <div class="stat-icon">⏱️</div>
              <div class="stat-value text-red-300">30s</div>
              <div class="stat-label">Süre Limiti</div>
            </div>
            <div class="stat-preview question-stat">
              <div class="stat-icon">🎯</div>
              <div class="stat-value text-blue-300">∞</div>
              <div class="stat-label">Soru Sayısı</div>
            </div>
            <div class="stat-preview bonus-stat">
              <div class="stat-icon">🔥</div>
              <div class="stat-value text-orange-300">+Bonus</div>
              <div class="stat-label">Hızlı Puan</div>
            </div>
          </div>

          <button 
            @click="startTimeRush"
            class="time-rush-btn-start w-full text-2xl px-8 py-6 mb-4 group"
          >
            🚀 Yarışı Başlat!
          </button>

          <button 
            @click="goHome"
            class="modern-btn-secondary w-full"
          >
            🏠 Ana Sayfa
          </button>
        </div>
      </div>

      <!-- Game Active - Enhanced -->
      <div v-if="isActive" class="slide-in-up">
        <!-- Enhanced Timer and Stats -->
        <div class="modern-card rounded-3xl p-6 mb-6 time-rush-header">
          <div class="flex items-center justify-between mb-4">
            <!-- Enhanced Timer -->
            <div class="flex items-center space-x-4">
              <div class="relative w-20 h-20">
                <svg class="w-20 h-20 transform -rotate-90" viewBox="0 0 64 64">
                  <circle cx="32" cy="32" r="28" stroke="rgba(255,255,255,0.1)" stroke-width="4" fill="none"/>
                  <circle 
                    cx="32" 
                    cy="32" 
                    r="28" 
                    :stroke="timeLeft <= 5 ? '#ef4444' : timeLeft <= 10 ? '#f59e0b' : '#10b981'"
                    stroke-width="4" 
                    fill="none"
                    stroke-linecap="round"
                    :stroke-dasharray="2 * Math.PI * 28"
                    :stroke-dashoffset="2 * Math.PI * 28 * (1 - timeLeft / totalTime)"
                    class="transition-all duration-1000 ease-linear timer-circle"
                  />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <span class="text-white font-bold text-xl timer-text">{{ timeLeft }}</span>
                </div>
                <div class="timer-glow" :class="{
                  'timer-danger': timeLeft <= 5,
                  'timer-warning': timeLeft <= 10 && timeLeft > 5,
                  'timer-safe': timeLeft > 10
                }"></div>
              </div>
              <div>
                <div class="text-white font-semibold text-lg">⏱️ Kalan Süre</div>
                <div class="text-white/80 text-sm">{{ Math.floor(timeLeft / 60) }}:{{ String(timeLeft % 60).padStart(2, '0') }}</div>
              </div>
            </div>

            <!-- Enhanced Score -->
            <div class="text-right score-display">
              <div class="text-4xl font-bold text-yellow-300 score-number">{{ score }}</div>
              <div class="text-white/80 text-sm font-medium">💎 Puan</div>
              <div v-if="lastPoints > 0" class="score-popup">+{{ lastPoints }}</div>
            </div>
          </div>

          <!-- Enhanced Progress Stats -->
          <div class="grid grid-cols-4 gap-4">
            <div class="stat-box">
              <div class="stat-icon">📊</div>
              <div class="stat-value text-blue-300">{{ totalAnswered }}</div>
              <div class="stat-label">Toplam</div>
            </div>
            <div class="stat-box">
              <div class="stat-icon">✅</div>
              <div class="stat-value text-green-300">{{ correctAnswers }}</div>
              <div class="stat-label">Doğru</div>
            </div>
            <div class="stat-box">
              <div class="stat-icon">📈</div>
              <div class="stat-value text-white">{{ accuracy }}%</div>
              <div class="stat-label">Başarı</div>
            </div>
            <div class="stat-box">
              <div class="stat-icon">🔥</div>
              <div class="stat-value text-orange-300">{{ streak }}</div>
              <div class="stat-label">Seri</div>
            </div>
          </div>
        </div>

        <!-- Enhanced Current Question -->
        <div class="modern-card rounded-3xl p-6 mb-6 question-container">
          <div class="text-center mb-6">
            <div class="question-header mb-4">
              <div class="question-number">Soru #{{ totalAnswered + 1 }}</div>
              <h2 class="text-3xl font-bold text-white mb-2 gradient-text">Bu görsel AI mi İnsan mı?</h2>
              <div class="difficulty-indicator">
                <span class="difficulty-badge">⚡ Hızlı Mod</span>
              </div>
            </div>
            
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
                :disabled="isAnswering || showFeedback"
                :class="{
                  'opacity-50 cursor-not-allowed': isAnswering || showFeedback,
                  'hover:scale-105': !isAnswering && !showFeedback
                }"
                class="bg-gradient-to-r from-purple-600 to-blue-600 hover:from-purple-700 hover:to-blue-700 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform flex items-center justify-center space-x-2"
              >
                <span v-if="!isAnswering">🤖</span>
                <span v-else class="animate-spin">⚡</span>
                <span>{{ isAnswering ? 'Değerlendiriliyor...' : 'AI Yapımı' }}</span>
              </button>
              
              <button 
                @click="makeGuess('human')"
                :disabled="isAnswering || showFeedback"
                :class="{
                  'opacity-50 cursor-not-allowed': isAnswering || showFeedback,
                  'hover:scale-105': !isAnswering && !showFeedback
                }"
                class="bg-gradient-to-r from-orange-500 to-red-500 hover:from-orange-600 hover:to-red-600 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform flex items-center justify-center space-x-2"
              >
                <span v-if="!isAnswering">👨</span>
                <span v-else class="animate-spin">⚡</span>
                <span>{{ isAnswering ? 'Değerlendiriliyor...' : 'İnsan Yapımı' }}</span>
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
const isAnswering = ref(false) // Spam engelleme için

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
  // Spam engelleme - eğer zaten cevap verme işlemi devam ediyorsa çık
  if (isAnswering.value || showFeedback.value) {
    return
  }
  
  // Cevap verme işlemini başlat
  isAnswering.value = true
  
  const result = timeRushStore.answerQuestion(guess)
  
  lastAnswerCorrect.value = result.isCorrect
  lastPoints.value = result.points
  showFeedback.value = true
  imageLoaded.value = false

  // Feedback gösterme süresi
  setTimeout(() => {
    showFeedback.value = false
    isAnswering.value = false // Spam engelini kaldır
  }, 800) // Reduced from 1000ms

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
/* Modern Gradient Background */
.modern-gradient {
  background: linear-gradient(135deg, 
    #ef4444 0%, 
    #f59e0b 25%, 
    #eab308 50%, 
    #f97316 75%, 
    #dc2626 100%);
  background-size: 300% 300%;
  animation: gradientFlow 12s ease infinite;
}

@keyframes gradientFlow {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

/* Floating Orbs with Glows */
.floating-orb {
  position: absolute;
  border-radius: 50%;
  opacity: 0.6;
  animation: float 15s ease-in-out infinite;
  backdrop-filter: blur(20px);
}

.bg-red-glow {
  width: 250px;
  height: 250px;
  background: radial-gradient(circle, rgba(239, 68, 68, 0.3) 0%, transparent 70%);
  top: -125px;
  right: -125px;
  animation-delay: 0s;
}

.bg-orange-glow {
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(245, 158, 11, 0.25) 0%, transparent 70%);
  bottom: -100px;
  left: -100px;
  animation-delay: -8s;
}

.bg-yellow-glow {
  width: 180px;
  height: 180px;
  background: radial-gradient(circle, rgba(234, 179, 8, 0.2) 0%, transparent 70%);
  top: 40%;
  left: 60%;
  animation-delay: -4s;
}

@keyframes float {
  0%, 100% { transform: translateY(0px) rotate(0deg); }
  25% { transform: translateY(-30px) rotate(90deg); }
  50% { transform: translateY(-60px) rotate(180deg); }
  75% { transform: translateY(-30px) rotate(270deg); }
}

/* Speed Lines */
.speed-lines {
  position: absolute;
  inset: 0;
  background-image: 
    linear-gradient(45deg, rgba(255,255,255,0.02) 1px, transparent 1px),
    linear-gradient(-45deg, rgba(255,255,255,0.02) 1px, transparent 1px);
  background-size: 30px 30px;
  animation: speedMove 2s linear infinite;
}

@keyframes speedMove {
  0% { background-position: 0 0, 0 0; }
  100% { background-position: 30px 30px, -30px 30px; }
}

/* Modern Card */
.modern-card {
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(20px);
  box-shadow: 
    0 25px 50px -12px rgba(0, 0, 0, 0.25),
    0 0 0 1px rgba(255, 255, 255, 0.1) inset,
    0 8px 32px rgba(31, 38, 135, 0.37);
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.modern-card:hover {
  transform: translateY(-2px);
  box-shadow: 
    0 35px 70px -12px rgba(0, 0, 0, 0.35),
    0 0 0 1px rgba(255, 255, 255, 0.2) inset,
    0 12px 48px rgba(31, 38, 135, 0.47);
}

/* Time Rush Intro */
.time-rush-intro {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.1) 0%, rgba(245, 158, 11, 0.1) 100%);
  border: 2px solid rgba(239, 68, 68, 0.3);
}

.time-rush-icon {
  position: relative;
  display: inline-block;
}

.icon-container {
  position: relative;
  display: inline-block;
}

.lightning-bolt {
  font-size: 4rem;
  filter: drop-shadow(0 0 20px rgba(245, 158, 11, 0.7));
  animation: lightningPulse 2s ease-in-out infinite;
}

@keyframes lightningPulse {
  0%, 100% { transform: scale(1); filter: drop-shadow(0 0 20px rgba(245, 158, 11, 0.7)); }
  50% { transform: scale(1.1); filter: drop-shadow(0 0 30px rgba(245, 158, 11, 1)); }
}

.energy-rings {
  position: absolute;
  inset: -20px;
  border: 2px solid rgba(245, 158, 11, 0.3);
  border-radius: 50%;
  animation: energyRotate 3s linear infinite;
}

.energy-rings::before,
.energy-rings::after {
  content: '';
  position: absolute;
  inset: -15px;
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: 50%;
  animation: energyRotate 4s linear infinite reverse;
}

@keyframes energyRotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Gradient Text */
.gradient-text {
  background: linear-gradient(135deg, #fff 0%, #fbbf24 50%, #ef4444 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  animation: textShimmer 3s ease-in-out infinite;
}

@keyframes textShimmer {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

/* Stat Previews */
.stat-preview {
  background: linear-gradient(135deg, rgba(255,255,255,0.12) 0%, rgba(255,255,255,0.08) 100%);
  border: 1px solid rgba(255,255,255,0.2);
  border-radius: 1rem;
  padding: 1.5rem;
  text-align: center;
  transition: all 0.3s ease;
}

.stat-preview:hover {
  transform: translateY(-5px);
  background: linear-gradient(135deg, rgba(255,255,255,0.18) 0%, rgba(255,255,255,0.12) 100%);
}

.stat-icon {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  filter: drop-shadow(0 0 10px rgba(255,255,255,0.3));
}

.stat-value {
  font-size: 2rem;
  font-weight: bold;
  line-height: 1;
  text-shadow: 0 0 20px currentColor;
}

.stat-label {
  font-size: 0.875rem;
  color: rgba(255,255,255,0.8);
  margin-top: 0.5rem;
  font-weight: 500;
}

/* Buttons */
.time-rush-btn-start {
  background: linear-gradient(135deg, #ef4444 0%, #f59e0b 100%);
  border: none;
  color: white;
  font-weight: 700;
  border-radius: 1.5rem;
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  position: relative;
  overflow: hidden;
  box-shadow: 0 15px 40px rgba(239, 68, 68, 0.4);
}

.time-rush-btn-start:hover {
  transform: translateY(-3px) scale(1.02);
  box-shadow: 0 20px 60px rgba(239, 68, 68, 0.6);
}

.modern-btn-secondary {
  background: linear-gradient(135deg, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0.05) 100%);
  border: 1px solid rgba(255,255,255,0.3);
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 1rem 2rem;
  transition: all 0.3s ease;
}

.modern-btn-secondary:hover {
  background: linear-gradient(135deg, rgba(255,255,255,0.2) 0%, rgba(255,255,255,0.1) 100%);
  transform: translateY(-2px);
}

/* Timer Styles */
.time-rush-header {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.12) 0%, rgba(245, 158, 11, 0.12) 100%);
  border: 1px solid rgba(239, 68, 68, 0.3);
}

.timer-circle {
  filter: drop-shadow(0 0 10px currentColor);
}

.timer-text {
  text-shadow: 0 0 20px currentColor;
  animation: timerPulse 1s ease-in-out infinite;
}

@keyframes timerPulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.1); }
}

.timer-glow {
  position: absolute;
  inset: -5px;
  border-radius: 50%;
  opacity: 0.5;
  animation: glowPulse 2s ease-in-out infinite;
}

.timer-danger {
  background: radial-gradient(circle, rgba(239, 68, 68, 0.4) 0%, transparent 70%);
  animation-duration: 0.5s;
}

.timer-warning {
  background: radial-gradient(circle, rgba(245, 158, 11, 0.3) 0%, transparent 70%);
  animation-duration: 1s;
}

.timer-safe {
  background: radial-gradient(circle, rgba(16, 185, 129, 0.2) 0%, transparent 70%);
}

@keyframes glowPulse {
  0%, 100% { opacity: 0.3; transform: scale(1); }
  50% { opacity: 0.7; transform: scale(1.1); }
}

/* Score Display */
.score-display {
  position: relative;
}

.score-number {
  text-shadow: 0 0 30px currentColor;
  animation: scoreGlow 2s ease-in-out infinite;
}

@keyframes scoreGlow {
  0%, 100% { text-shadow: 0 0 30px currentColor; }
  50% { text-shadow: 0 0 50px currentColor, 0 0 80px currentColor; }
}

.score-popup {
  position: absolute;
  top: -20px;
  right: 0;
  background: rgba(34, 197, 94, 0.9);
  color: white;
  padding: 0.25rem 0.5rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: bold;
  animation: scorePopup 1s ease-out;
}

@keyframes scorePopup {
  0% { opacity: 0; transform: translateY(10px) scale(0.8); }
  50% { opacity: 1; transform: translateY(-5px) scale(1.1); }
  100% { opacity: 0; transform: translateY(-20px) scale(1); }
}

/* Stat Boxes */
.stat-box {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 0.75rem;
  padding: 1rem;
  text-align: center;
  transition: all 0.3s ease;
}

.stat-box:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.15);
}

.stat-box .stat-icon {
  font-size: 1.5rem;
  margin-bottom: 0.5rem;
}

.stat-box .stat-value {
  font-size: 1.5rem;
  font-weight: bold;
  line-height: 1;
}

.stat-box .stat-label {
  font-size: 0.75rem;
  color: rgba(255,255,255,0.7);
  margin-top: 0.25rem;
}

/* Question Container */
.question-container {
  background: linear-gradient(135deg, rgba(255,255,255,0.12) 0%, rgba(255,255,255,0.08) 100%);
  border: 1px solid rgba(255,255,255,0.25);
}

.question-header {
  margin-bottom: 1rem;
}

.question-number {
  color: rgba(255,255,255,0.8);
  font-size: 0.875rem;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.difficulty-indicator {
  margin-top: 1rem;
}

.difficulty-badge {
  background: linear-gradient(135deg, #ef4444 0%, #f59e0b 100%);
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-size: 0.75rem;
  font-weight: 600;
  box-shadow: 0 4px 15px rgba(239, 68, 68, 0.3);
}

/* Animation Classes */
.scale-in {
  animation: scaleIn 0.6s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

@keyframes scaleIn {
  0% { 
    opacity: 0; 
    transform: scale(0.9) translateY(20px); 
  }
  100% { 
    opacity: 1; 
    transform: scale(1) translateY(0); 
  }
}

.slide-in-up {
  animation: slideInUp 0.6s ease-out;
}

@keyframes slideInUp {
  0% { 
    opacity: 0; 
    transform: translateY(30px); 
  }
  100% { 
    opacity: 1; 
    transform: translateY(0); 
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .time-rush-icon .lightning-bolt {
    font-size: 3rem;
  }
  
  .stat-preview {
    padding: 1rem;
  }
  
  .stat-icon {
    font-size: 1.5rem;
  }
  
  .stat-value {
    font-size: 1.5rem;
  }
  
  .timer-circle {
    width: 60px;
    height: 60px;
  }
  
  .timer-text {
    font-size: 1rem;
  }
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
