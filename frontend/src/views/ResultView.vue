<template>
  <div class="min-h-screen gradient-bg flex items-center justify-center p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-10 right-10 w-72 h-72 bg-yellow-300/10 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-10 left-10 w-72 h-72 bg-green-300/10 rounded-full blur-3xl animate-bounce-slow"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-purple-300/10 rounded-full blur-3xl"></div>
    </div>

    <div class="relative z-10 max-w-lg w-full animate-fade-in">
      <!-- Results Card -->
      <div class="glass-card rounded-3xl p-8 text-center border border-white/30">
        
        <!-- Celebration Header -->
        <div class="mb-8">
          <div class="text-6xl mb-4 animate-bounce">
            {{ getEmoji() }}
          </div>
          <h1 class="text-3xl font-bold text-white mb-2">{{ getTitle() }}</h1>
          <p class="text-white/80 text-lg">{{ getSubtitle() }}</p>
        </div>
        
        <!-- Score Circle -->
        <div class="mb-8 relative">
          <div class="w-40 h-40 mx-auto relative">
            <!-- Background Circle -->
            <svg class="w-40 h-40 transform -rotate-90" viewBox="0 0 144 144">
              <circle cx="72" cy="72" r="60" stroke="rgba(255,255,255,0.2)" stroke-width="8" fill="none" />
              <circle 
                cx="72" 
                cy="72" 
                r="60" 
                :stroke="getScoreColor()" 
                stroke-width="8" 
                fill="none"
                stroke-linecap="round"
                :stroke-dasharray="circumference"
                :stroke-dashoffset="strokeOffset"
                class="transition-all duration-2000 ease-out"
              />
            </svg>
            
            <!-- Score Text -->
            <div class="absolute inset-0 flex items-center justify-center">
              <div>
                <div class="text-4xl font-bold text-white mb-1">{{ successRate }}%</div>
                <div class="text-white/60 text-sm">Başarı</div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Detailed Stats -->
        <div class="grid grid-cols-3 gap-4 mb-8">
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-4 border border-white/20">
            <div class="text-2xl font-bold text-green-400">{{ results.correct }}</div>
            <div class="text-white/70 text-sm">Doğru</div>
          </div>
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-4 border border-white/20">
            <div class="text-2xl font-bold text-red-400">{{ results.wrong }}</div>
            <div class="text-white/70 text-sm">Yanlış</div>
          </div>
          <div class="bg-white/10 backdrop-blur-sm rounded-2xl p-4 border border-white/20">
            <div class="text-2xl font-bold text-blue-400">{{ results.total }}</div>
            <div class="text-white/70 text-sm">Toplam</div>
          </div>
        </div>

        <!-- Performance Badge -->
        <div class="mb-8">
          <div :class="getBadgeClass()" class="inline-flex items-center px-6 py-3 rounded-2xl text-sm font-semibold">
            <span class="mr-2">{{ getBadgeIcon() }}</span>
            {{ getBadgeText() }}
          </div>
        </div>
        
        <!-- Action Buttons -->
        <div class="space-y-4 mb-8">
          <button 
            @click="playAgain"
            class="w-full btn-primary text-lg py-4 px-6 relative overflow-hidden group"
          >
            <span class="relative z-10 flex items-center justify-center space-x-2">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"></path>
              </svg>
              <span>Tekrar Oyna</span>
            </span>
          </button>
          
          <button 
            @click="goToLeaderboard"
            class="w-full bg-gradient-to-r from-yellow-500 to-orange-500 hover:from-yellow-600 hover:to-orange-600 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
          >
            <span>🏆</span>
            <span>Liderlik Tablosu</span>
          </button>
          
          <button 
            @click="goHome"
            class="w-full bg-white/10 hover:bg-white/20 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 border border-white/30"
          >
            🏠 Ana Sayfa
          </button>
        </div>
        
        <!-- Social Share -->
        <div class="pt-6 border-t border-white/20">
          <p class="text-white/70 text-sm mb-4">Başarını paylaş:</p>
          <div class="flex gap-3 justify-center">
            <button 
              @click="shareToTwitter"
              class="bg-blue-500 hover:bg-blue-600 text-white px-6 py-2 rounded-xl text-sm transition-all duration-300 transform hover:scale-105 flex items-center space-x-2"
            >
              <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
                <path d="M23.953 4.57a10 10 0 01-2.825.775 4.958 4.958 0 002.163-2.723c-.951.555-2.005.959-3.127 1.184a4.92 4.92 0 00-8.384 4.482C7.69 8.095 4.067 6.13 1.64 3.162a4.822 4.822 0 00-.666 2.475c0 1.71.87 3.213 2.188 4.096a4.904 4.904 0 01-2.228-.616v.06a4.923 4.923 0 003.946 4.827 4.996 4.996 0 01-2.212.085 4.936 4.936 0 004.604 3.417 9.867 9.867 0 01-6.102 2.105c-.39 0-.779-.023-1.17-.067a13.995 13.995 0 007.557 2.209c9.053 0 13.998-7.496 13.998-13.985 0-.21 0-.42-.015-.63A9.935 9.935 0 0024 4.59z"/>
              </svg>
              <span>Twitter</span>
            </button>
            
            <button 
              @click="copyResults"
              class="bg-gray-600 hover:bg-gray-700 text-white px-6 py-2 rounded-xl text-sm transition-all duration-300 transform hover:scale-105 flex items-center space-x-2"
            >
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z"></path>
              </svg>
              <span>Kopyala</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useGameStore } from '../stores/game'
import { useUserStore } from '../stores/user'

const router = useRouter()
const gameStore = useGameStore()
const userStore = useUserStore()

const results = computed(() => gameStore.results)
const successRate = computed(() => 
  Math.round((results.value.correct / results.value.total) * 100)
)

// Circle animation
const circumference = 2 * Math.PI * 60
const strokeOffset = ref(circumference)

onMounted(() => {
  // Animate the circle
  setTimeout(() => {
    strokeOffset.value = circumference - (successRate.value / 100) * circumference
  }, 500)
})

const getScoreColor = () => {
  const rate = successRate.value
  if (rate >= 80) return '#10b981' // green
  if (rate >= 60) return '#f59e0b' // yellow
  return '#ef4444' // red
}

const getEmoji = () => {
  const rate = successRate.value
  if (rate >= 90) return '🏆'
  if (rate >= 80) return '🎉'
  if (rate >= 70) return '👏'
  if (rate >= 60) return '👍'
  if (rate >= 50) return '😊'
  return '💪'
}

const getTitle = () => {
  const rate = successRate.value
  const userName = userStore.userName || 'Dostum'
  if (rate >= 90) return `Mükemmel ${userName}!`
  if (rate >= 80) return `Harika ${userName}!`
  if (rate >= 70) return `Çok İyi ${userName}!`
  if (rate >= 60) return `İyi ${userName}!`
  if (rate >= 50) return `Fena Değil ${userName}!`
  return `Daha İyisini Yapabilirsin ${userName}!`
}

const getSubtitle = () => {
  const rate = successRate.value
  if (rate >= 90) return 'AI uzmanı seviyesindesin!'
  if (rate >= 80) return 'Gerçekten yeteneklisin!'
  if (rate >= 70) return 'İyi bir performans sergledin!'
  if (rate >= 60) return 'Ortalama üstü bir sonuç!'
  if (rate >= 50) return 'Yarı yarıya doğru tahmin ettin!'
  return 'Tekrar deneyerek gelişebilirsin!'
}

const getBadgeClass = () => {
  const rate = successRate.value
  if (rate >= 80) return 'bg-green-500/20 text-green-300 border border-green-400/30'
  if (rate >= 60) return 'bg-yellow-500/20 text-yellow-300 border border-yellow-400/30'
  return 'bg-red-500/20 text-red-300 border border-red-400/30'
}

const getBadgeIcon = () => {
  const rate = successRate.value
  if (rate >= 80) return '🌟'
  if (rate >= 60) return '⭐'
  return '🎯'
}

const getBadgeText = () => {
  const rate = successRate.value
  if (rate >= 90) return 'AI Dedektifi'
  if (rate >= 80) return 'Süper Tahmineci'
  if (rate >= 70) return 'İyi Gözlemci'
  if (rate >= 60) return 'Orta Seviye'
  if (rate >= 50) return 'Başlangıç Seviyesi'
  return 'Gelişim Aşamasında'
}

const playAgain = () => {
  gameStore.resetGame()
  router.push('/game')
}

const goToLeaderboard = () => {
  router.push('/leaderboard')
}

const goHome = () => {
  gameStore.resetGame()
  router.push('/')
}

const shareToTwitter = () => {
  const text = `🎯 AivsHuman'da ${successRate.value}% başarı oranı elde ettim! ${getEmoji()} AI vs İnsan görselleri ayırt etmede sen ne kadar iyisin? #AivsHuman #AI #Challenge`
  const url = `https://twitter.com/intent/tweet?text=${encodeURIComponent(text)}`
  window.open(url, '_blank')
}

const copyResults = () => {
  const text = `🎯 AivsHuman sonuçlarım: ${results.value.correct}/${results.value.total} doğru (%${successRate.value} başarı) ${getEmoji()}`
  navigator.clipboard.writeText(text).then(() => {
    // Burada bir toast mesajı gösterilebilir
    console.log('Sonuçlar kopyalandı!')
  })
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

.animate-fade-in {
  animation: fadeIn 1s ease-out;
}

.animate-bounce-slow {
  animation: bounce 2s infinite;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px) scale(0.95); }
  to { opacity: 1; transform: translateY(0) scale(1); }
}
</style>
