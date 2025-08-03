<template>
  <div class="min-h-screen modern-gradient p-4 relative overflow-hidden">
    <!-- Enhanced Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="floating-orb orb-1"></div>
      <div class="floating-orb orb-2"></div>
      <div class="floating-orb orb-3"></div>
      <div class="grid-pattern"></div>
    </div>

    <div class="relative z-10 max-w-5xl mx-auto">
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

      <!-- Enhanced Header with Score Display -->
      <div class="text-center mb-8 scale-in">
        <!-- Game Mode Badge -->
        <div class="inline-flex items-center space-x-3 modern-card rounded-2xl px-6 py-3 mb-4">
          <div class="status-indicator active"></div>
          <h1 class="text-2xl font-bold text-white gradient-text">🖼️ Görsel Mod</h1>
        </div>
        
        <!-- Enhanced Score Board -->
        <div class="modern-card rounded-2xl p-6 mb-6 score-board">
          <div class="grid grid-cols-3 gap-6">
            <!-- Current Score -->
            <div class="score-item">
              <div class="score-icon">🎯</div>
              <div class="score-number text-green-300">{{ score }}</div>
              <div class="score-label">Puan</div>
            </div>
            
            <!-- Streak -->
            <div class="score-item">
              <div class="score-icon">🔥</div>
              <div class="score-number text-orange-300">{{ getStreak() }}</div>
              <div class="score-label">Seri</div>
            </div>
            
            <!-- Accuracy -->
            <div class="score-item">
              <div class="score-icon">📊</div>
              <div class="score-number text-blue-300">{{ getAccuracy() }}%</div>
              <div class="score-label">Başarı</div>
            </div>
          </div>
        </div>
        
        <!-- Enhanced Progress Bar -->
        <div class="modern-card rounded-2xl p-4">
          <div class="flex justify-between text-white/80 text-sm mb-4">
            <span class="flex items-center gap-2">
              <span class="animate-pulse">⚡</span>
              Soru {{ currentQuestion }} / {{ totalQuestions }}
            </span>
            <span class="gradient-text font-semibold">
              {{ Math.round((currentQuestion / totalQuestions) * 100) }}% Tamamlandı
            </span>
          </div>
          
          <!-- Progress Bar Container -->
          <div class="relative w-full h-4 bg-white/10 rounded-full overflow-hidden backdrop-blur-sm">
            <!-- Background Glow -->
            <div class="absolute inset-0 bg-gradient-to-r from-blue-500/20 to-purple-500/20 rounded-full"></div>
            
            <!-- Progress Fill -->
            <div 
              class="progress-fill h-full rounded-full transition-all duration-700 ease-out relative overflow-hidden"
              :style="{ width: `${(currentQuestion / totalQuestions) * 100}%` }"
            >
              <!-- Shimmer Effect -->
              <div class="absolute inset-0 shimmer-effect"></div>
            </div>
            
            <!-- Progress Segments -->
            <div class="absolute inset-0 flex">
              <div 
                v-for="i in totalQuestions" 
                :key="i" 
                class="flex-1 border-r border-white/20 last:border-r-0"
              ></div>
            </div>
          </div>
          
          <!-- Mini Progress Indicators -->
          <div class="flex justify-between mt-3">
            <div 
              v-for="i in totalQuestions" 
              :key="i"
              :class="{
                'bg-green-400 shadow-green-glow': i < currentQuestion && getAnswerResult(i-1),
                'bg-red-400 shadow-red-glow': i < currentQuestion && !getAnswerResult(i-1),
                'bg-blue-400 shadow-blue-glow animate-pulse': i === currentQuestion,
                'bg-white/20': i > currentQuestion
              }"
              class="w-3 h-3 rounded-full transition-all duration-500 mini-indicator"
            ></div>
          </div>
        </div>
      </div>
      
      <!-- Enhanced Image Card -->
      <div class="modern-card rounded-3xl p-6 mb-8 slide-in-up image-container">
        <div v-if="currentImage" class="relative overflow-hidden rounded-2xl">
          <img 
            :src="currentImage.url" 
            :alt="`Soru ${currentQuestion}`"
            class="w-full max-h-96 object-cover transition-all duration-500 hover:scale-110 image-hover"
            @load="imageLoaded = true"
            loading="eager"
          />
          
          <!-- Enhanced Loading Overlay -->
          <div v-if="!imageLoaded" class="absolute inset-0 bg-gradient-to-br from-gray-800 to-gray-900 rounded-2xl flex items-center justify-center loading-shimmer">
            <div class="text-center">
              <div class="animate-spin text-4xl mb-2">⚡</div>
              <div class="text-white/80 font-medium">Görsel yükleniyor...</div>
            </div>
          </div>
          
          <!-- Enhanced Image Badge -->
          <div class="absolute top-4 right-4 modern-badge">
            <span class="text-sm font-bold">{{ currentQuestion }}/{{ totalQuestions }}</span>
          </div>
          
          <!-- Image Info Overlay -->
          <div class="absolute bottom-4 left-4 right-4 bg-black/30 backdrop-blur-sm rounded-xl p-3 text-white/90 transform translate-y-full group-hover:translate-y-0 transition-transform duration-300">
            <p class="text-sm font-medium">Bu görsel AI tarafından mı üretildi, yoksa insan tarafından mı çekildi?</p>
          </div>
        </div>
      </div>
      
      <!-- Enhanced Action Buttons -->
      <div v-if="!answered" class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8 slide-in-up">
        <button 
          @click="makeGuess('human')"
          :disabled="isAnswering"
          class="choice-btn human-btn group"
        >
          <div class="btn-content">
            <div class="btn-icon">
              <svg v-if="!isAnswering" class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
              </svg>
              <div v-else class="animate-spin text-2xl">⚡</div>
            </div>
            <div class="btn-text">
              <div class="text-2xl font-bold">{{ isAnswering ? 'Değerlendiriliyor...' : '👤 İnsan' }}</div>
              <div class="text-sm opacity-80 mt-1">İnsan tarafından oluşturuldu</div>
            </div>
          </div>
          <div class="btn-glow human-glow"></div>
        </button>
        
        <button 
          @click="makeGuess('ai')"
          :disabled="isAnswering"
          class="choice-btn ai-btn group"
        >
          <div class="btn-content">
            <div class="btn-icon">
              <svg v-if="!isAnswering" class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.75 17L9 20l-1 1h8l-1-1-.75-3M3 13h18M5 17h14a2 2 0 002-2V5a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"></path>
              </svg>
              <div v-else class="animate-spin text-2xl">⚡</div>
            </div>
            <div class="btn-text">
              <div class="text-2xl font-bold">{{ isAnswering ? 'Değerlendiriliyor...' : '🤖 AI' }}</div>
              <div class="text-sm opacity-80 mt-1">Yapay zeka tarafından üretildi</div>
            </div>
          </div>
          <div class="btn-glow ai-glow"></div>
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
              <div class="text-xl font-bold text-red-500 bg-red-500/20 px-3 py-1 rounded-lg border border-red-500/30">{{ wrongAnswers }}</div>
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
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '../stores/game'
import { useLeaderboardStore } from '../stores/leaderboard'
import { useUserStore } from '../stores/user'
import { useSound } from '../composables/useSound'

const router = useRouter()
const gameStore = useGameStore()
const leaderboardStore = useLeaderboardStore()
const userStore = useUserStore()
const { sounds } = useSound()

const currentQuestion = ref(1)
const totalQuestions = ref(10)
const answered = ref(false)
const isCorrect = ref(false)
const currentImage = ref<any>(null)
const imageLoaded = ref(false)
const isAnswering = ref(false) // Spam engelleme için

const correctAnswers = ref(0)
const wrongAnswers = ref(0)
const answerHistory = ref<boolean[]>([]) // Cevap geçmişi tracking

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

// Enhanced Stats Methods
const score = computed(() => correctAnswers.value * 10) // Her doğru cevap 10 puan

const getStreak = () => {
  let streak = 0
  for (let i = answerHistory.value.length - 1; i >= 0; i--) {
    if (answerHistory.value[i]) {
      streak++
    } else {
      break
    }
  }
  return streak
}

const getAccuracy = () => {
  if (answerHistory.value.length === 0) return 100
  const correct = answerHistory.value.filter(answer => answer).length
  return Math.round((correct / answerHistory.value.length) * 100)
}

const getAnswerResult = (index: number) => {
  return answerHistory.value[index] || false
}

onMounted(() => {
  loadQuestion()
  preloadNextImages() // İlk soru yüklendiğinde bir sonraki soruları preload et
  sounds.gameStart() // Oyun başlama sesi
})

const loadQuestion = () => {
  if (currentQuestion.value <= totalQuestions.value) {
    currentImage.value = mockImages[currentQuestion.value - 1]
    answered.value = false
    isAnswering.value = false // Spam engelini sıfırla
    imageLoaded.value = false
    
    // Her yeni soru için ses efekti (ilk soru hariç)
    if (currentQuestion.value > 1) {
      sounds.nextQuestion()
    }
  }
}

const makeGuess = (guess: 'ai' | 'human') => {
  // Spam engelleme - eğer zaten cevap verilmişse çık
  if (answered.value || isAnswering.value) {
    return
  }
  
  // Tıklama sesi
  sounds.click()
  
  isAnswering.value = true
  answered.value = true
  isCorrect.value = guess === currentImage.value.label
  
  // Cevabı history'ye ekle
  answerHistory.value.push(isCorrect.value)
  
  if (isCorrect.value) {
    correctAnswers.value++
    sounds.correct() // Doğru cevap sesi
    
    // Streak kontrolü
    const currentStreak = getStreak()
    if (currentStreak >= 3) {
      setTimeout(() => sounds.streak(currentStreak), 300)
    }
  } else {
    wrongAnswers.value++
    sounds.wrong() // Yanlış cevap sesi
  }
  
  // Kısa süre sonra isAnswering'i false yap
  setTimeout(() => {
    isAnswering.value = false
  }, 500)
}

const nextQuestion = () => {
  if (currentQuestion.value < totalQuestions.value) {
    currentQuestion.value++
    loadQuestion()
    preloadNextImages() // Bir sonraki soruları önceden yükle
  } else {
    // Game finished
    sounds.gameComplete() // Oyun bitirme sesi
    
    const accuracy = (correctAnswers.value / totalQuestions.value) * 100
    
    leaderboardStore.addScore({
      playerName: userStore.userName || 'Anonim',
      score: score.value, // Computed score kullan
      totalQuestions: totalQuestions.value
    })
    
    // Update user stats
    userStore.updateStats({
      score: score.value, // Computed score kullan
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
/* Modern Gradient Background */
.modern-gradient {
  background: linear-gradient(135deg, 
    #667eea 0%, 
    #764ba2 25%, 
    #f093fb 50%, 
    #f5576c 75%, 
    #4facfe 100%);
  background-size: 300% 300%;
  animation: gradientFlow 15s ease infinite;
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

@keyframes gradientFlow {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

/* Floating Orbs - Homeview ile aynı */
.floating-orb {
  position: absolute;
  border-radius: 50%;
  opacity: 0.7;
  animation: float 20s ease-in-out infinite;
  backdrop-filter: blur(20px);
}

.orb-1 {
  width: 300px;
  height: 300px;
  background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%);
  top: -150px;
  right: -150px;
  animation-delay: 0s;
}

.orb-2 {
  width: 200px;
  height: 200px;
  background: radial-gradient(circle, rgba(147,51,234,0.2) 0%, transparent 70%);
  bottom: -100px;
  left: -100px;
  animation-delay: -10s;
}

.orb-3 {
  width: 150px;
  height: 150px;
  background: radial-gradient(circle, rgba(59,130,246,0.15) 0%, transparent 70%);
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  animation-delay: -5s;
}

@keyframes float {
  0%, 100% { transform: translateY(0px) rotate(0deg); }
  25% { transform: translateY(-20px) rotate(90deg); }
  50% { transform: translateY(-40px) rotate(180deg); }
  75% { transform: translateY(-20px) rotate(270deg); }
}

/* Grid Pattern */
.grid-pattern {
  position: absolute;
  inset: 0;
  background-image: 
    linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px);
  background-size: 50px 50px;
  animation: gridMove 30s linear infinite;
}

@keyframes gridMove {
  0% { transform: translateX(0) translateY(0); }
  100% { transform: translateX(50px) translateY(50px); }
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

/* Status Indicator */
.status-indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
}

.status-indicator.active {
  background: #10b981;
  box-shadow: 0 0 10px #10b981;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

/* Gradient Text */
.gradient-text {
  background: linear-gradient(135deg, #fff 0%, #ff6b6b 50%, #4ecdc4 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  animation: textShimmer 3s ease-in-out infinite;
}

@keyframes textShimmer {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

/* Enhanced Score Board */
.score-board {
  background: linear-gradient(135deg, rgba(255,255,255,0.12) 0%, rgba(255,255,255,0.08) 100%);
  border: 1px solid rgba(255,255,255,0.3);
}

.score-item {
  text-align: center;
  transition: all 0.3s ease;
  padding: 1rem;
  border-radius: 1rem;
}

.score-item:hover {
  transform: translateY(-2px);
  background: rgba(255,255,255,0.1);
}

.score-icon {
  font-size: 2rem;
  margin-bottom: 0.5rem;
  filter: drop-shadow(0 0 10px rgba(255,255,255,0.3));
}

.score-number {
  font-size: 2rem;
  font-weight: bold;
  line-height: 1;
  text-shadow: 0 0 20px currentColor;
}

.score-label {
  font-size: 0.875rem;
  color: rgba(255,255,255,0.8);
  margin-top: 0.5rem;
  font-weight: 500;
}

/* Enhanced Progress Bar */
.progress-fill {
  background: linear-gradient(90deg, #3b82f6 0%, #8b5cf6 50%, #ec4899 100%);
  box-shadow: 0 0 20px rgba(59, 130, 246, 0.5);
  position: relative;
}

.shimmer-effect {
  background: linear-gradient(
    90deg,
    transparent 0%,
    rgba(255, 255, 255, 0.4) 50%,
    transparent 100%
  );
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(100%); }
}

/* Mini Progress Indicators */
.mini-indicator {
  transition: all 0.5s ease;
}

.shadow-green-glow {
  box-shadow: 0 0 10px #10b981;
}

.shadow-red-glow {
  box-shadow: 0 0 10px #ef4444;
}

.shadow-blue-glow {
  box-shadow: 0 0 10px #3b82f6;
}

/* Enhanced Image Container */
.image-container {
  position: relative;
  overflow: hidden;
  group: true;
}

.image-hover {
  filter: brightness(1.05) contrast(1.1);
}

.loading-shimmer {
  background: linear-gradient(
    45deg,
    #1f2937 25%,
    #374151 25%,
    #374151 50%,
    #1f2937 50%,
    #1f2937 75%,
    #374151 75%
  );
  background-size: 40px 40px;
  animation: shimmerMove 2s linear infinite;
}

@keyframes shimmerMove {
  0% { background-position: 0 0; }
  100% { background-position: 40px 40px; }
}

.modern-badge {
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(10px);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 1rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

/* Enhanced Choice Buttons */
.choice-btn {
  position: relative;
  padding: 2rem;
  border-radius: 1.5rem;
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  overflow: hidden;
  border: 2px solid transparent;
  backdrop-filter: blur(20px);
}

.choice-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none !important;
}

.choice-btn:not(:disabled):hover {
  transform: translateY(-5px) scale(1.02);
}

.choice-btn:not(:disabled):active {
  transform: translateY(-2px) scale(0.98);
}

.human-btn {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.2) 0%, rgba(22, 163, 74, 0.2) 100%);
  border-color: rgba(34, 197, 94, 0.4);
}

.human-btn:not(:disabled):hover {
  border-color: rgba(34, 197, 94, 0.8);
  box-shadow: 0 20px 60px rgba(34, 197, 94, 0.3);
}

.ai-btn {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.2) 0%, rgba(220, 38, 38, 0.2) 100%);
  border-color: rgba(239, 68, 68, 0.4);
}

.ai-btn:not(:disabled):hover {
  border-color: rgba(239, 68, 68, 0.8);
  box-shadow: 0 20px 60px rgba(239, 68, 68, 0.3);
}

.btn-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  position: relative;
  z-index: 2;
}

.btn-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 4rem;
  height: 4rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  transition: all 0.3s ease;
}

.choice-btn:not(:disabled):hover .btn-icon {
  background: rgba(255, 255, 255, 0.2);
  transform: scale(1.1);
}

.btn-text {
  flex: 1;
  text-align: left;
  color: white;
}

.btn-glow {
  position: absolute;
  inset: -2px;
  border-radius: 1.5rem;
  opacity: 0;
  transition: opacity 0.3s ease;
  z-index: 1;
}

.human-glow {
  background: linear-gradient(45deg, #22c55e, #16a34a, #15803d, #22c55e);
  background-size: 300% 300%;
  animation: glowRotate 3s linear infinite;
}

.ai-glow {
  background: linear-gradient(45deg, #ef4444, #dc2626, #b91c1c, #ef4444);
  background-size: 300% 300%;
  animation: glowRotate 3s linear infinite;
}

.choice-btn:not(:disabled):hover .btn-glow {
  opacity: 1;
}

@keyframes glowRotate {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
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
  .score-board {
    padding: 1rem;
  }
  
  .score-item {
    padding: 0.75rem;
  }
  
  .score-icon {
    font-size: 1.5rem;
  }
  
  .score-number {
    font-size: 1.5rem;
  }
  
  .choice-btn {
    padding: 1.5rem;
  }
  
  .btn-content {
    gap: 1rem;
  }
  
  .btn-icon {
    width: 3rem;
    height: 3rem;
  }
}

/* Additional Button Styles */
.btn-primary {
  background: linear-gradient(to right, #3b82f6, #1d4ed8);
  color: white;
  font-weight: 600;
  padding: 1rem 2rem;
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  transform: scale(1);
  border: none;
  cursor: pointer;
}

.btn-primary:hover {
  background: linear-gradient(to right, #1d4ed8, #1e40af);
  transform: scale(1.05);
}

.btn-primary:active {
  transform: scale(0.95);
}

/* Glass Card */
.glass-card {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  backdrop-filter: blur(20px);
}

/* Animation Classes */
.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}

.animate-slide-up {
  animation: slideUp 0.6s ease-out;
}

@keyframes fadeIn {
  from { 
    opacity: 0; 
    transform: translateY(20px); 
  }
  to { 
    opacity: 1; 
    transform: translateY(0); 
  }
}

@keyframes slideUp {
  from { 
    opacity: 0; 
    transform: translateY(30px); 
  }
  to { 
    opacity: 1; 
    transform: translateY(0); 
  }
}
</style>
