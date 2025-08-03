<template>
  <div class="min-h-screen gradient-bg p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-20 right-20 w-64 h-64 bg-yellow-300/10 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-20 left-20 w-64 h-64 bg-orange-300/10 rounded-full blur-3xl"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-red-300/10 rounded-full blur-3xl animate-pulse-slow"></div>
    </div>

    <!-- Home Button -->
    <div class="absolute top-4 left-4 z-20">
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

    <div class="relative z-10 max-w-2xl mx-auto animate-fade-in">
      <!-- Header -->
      <div class="text-center mb-8">
        <div class="text-8xl mb-4">⚡</div>
        <h1 class="text-4xl font-bold text-white mb-2">Yarış Tamamlandı!</h1>
        <p class="text-white/80 text-lg">Sürat performansın</p>
      </div>

      <!-- Main Score Card -->
      <div class="glass-card rounded-3xl p-8 mb-8 border-2 border-yellow-400/30">
        <div class="text-center mb-8">
          <!-- Score Circle -->
          <div class="relative w-32 h-32 mx-auto mb-6">
            <svg class="w-32 h-32 transform -rotate-90" viewBox="0 0 120 120">
              <circle cx="60" cy="60" r="50" stroke="rgba(255,255,255,0.1)" stroke-width="8" fill="none"/>
              <circle 
                cx="60" 
                cy="60" 
                r="50" 
                stroke="#fbbf24"
                stroke-width="8" 
                fill="none"
                stroke-linecap="round"
                :stroke-dasharray="2 * Math.PI * 50"
                :stroke-dashoffset="2 * Math.PI * 50 * (1 - accuracy / 100)"
                class="transition-all duration-2000 ease-out"
              />
            </svg>
            <div class="absolute inset-0 flex flex-col items-center justify-center">
              <div class="text-3xl font-bold text-yellow-300">{{ score }}</div>
              <div class="text-white/70 text-sm">PUAN</div>
            </div>
          </div>

          <!-- Performance Badge -->
          <div class="inline-flex items-center space-x-2 bg-gradient-to-r from-yellow-500/20 to-orange-500/20 border border-yellow-400/30 rounded-2xl px-6 py-3 mb-6">
            <span class="text-2xl">{{ getPerformanceBadge().icon }}</span>
            <span class="text-xl font-bold text-white">{{ getPerformanceBadge().title }}</span>
          </div>

          <p class="text-white/80 text-lg">{{ getPerformanceBadge().description }}</p>
        </div>

        <!-- Detailed Stats -->
        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
          <div class="bg-white/10 rounded-2xl p-4 text-center">
            <div class="text-2xl font-bold text-blue-300">{{ answered }}</div>
            <div class="text-white/70 text-sm">Cevaplanan</div>
          </div>
          
          <div class="bg-white/10 rounded-2xl p-4 text-center">
            <div class="text-2xl font-bold text-green-300">{{ correct }}</div>
            <div class="text-white/70 text-sm">Doğru</div>
          </div>
          
          <div class="bg-white/10 rounded-2xl p-4 text-center">
            <div class="text-2xl font-bold text-yellow-300">{{ accuracy }}%</div>
            <div class="text-white/70 text-sm">Başarı</div>
          </div>
          
          <div class="bg-white/10 rounded-2xl p-4 text-center">
            <div class="text-2xl font-bold text-purple-300">{{ qps }}</div>
            <div class="text-white/70 text-sm">Soru/Saniye</div>
          </div>
        </div>

        <!-- Speed Analysis -->
        <div class="bg-white/5 rounded-2xl p-6 mb-6">
          <h3 class="text-xl font-bold text-white mb-4">🏃‍♂️ Hız Analizi</h3>
          <div class="space-y-3">
            <div class="flex justify-between items-center">
              <span class="text-white/80">Ortalama Cevap Süresi:</span>
              <span class="text-yellow-300 font-semibold">{{ averageTime }}s</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-white/80">En Hızlı Kategori:</span>
              <span class="text-green-300 font-semibold">{{ getBestCategory() }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-white/80">Hız Sıralaması:</span>
              <span class="text-blue-300 font-semibold">{{ getSpeedRank() }}</span>
            </div>
          </div>
        </div>

        <!-- Achievement Unlocked -->
        <div v-if="getAchievement()" class="bg-gradient-to-r from-purple-500/20 to-pink-500/20 border border-purple-400/30 rounded-2xl p-6 mb-6 animate-pulse">
          <div class="text-center">
            <div class="text-4xl mb-2">🏆</div>
            <h3 class="text-xl font-bold text-white mb-2">Başarı Kazandın!</h3>
            <p class="text-purple-300 font-semibold">{{ getAchievement() }}</p>
          </div>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="space-y-4">
        <button 
          @click="playTimeRushAgain"
          class="w-full bg-gradient-to-r from-red-500 to-orange-500 hover:from-red-600 hover:to-orange-600 text-white font-semibold py-4 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
        >
          <span>⚡</span>
          <span>Yeniden Yarış!</span>
        </button>
        
        <div class="grid grid-cols-2 gap-4">
          <button 
            @click="goToLeaderboard"
            class="bg-gradient-to-r from-yellow-500 to-orange-500 hover:from-yellow-600 hover:to-orange-600 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
          >
            <span>🏆</span>
            <span>Liderlik</span>
          </button>
          
          <button 
            @click="goToNormalGame"
            class="bg-gradient-to-r from-blue-500 to-purple-500 hover:from-blue-600 hover:to-purple-600 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 transform hover:scale-105 flex items-center justify-center space-x-2"
          >
            <span>🎮</span>
            <span>Normal Oyun</span>
          </button>
        </div>
        
        <button 
          @click="goHome"
          class="w-full bg-white/10 hover:bg-white/20 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 border border-white/30"
        >
          🏠 Ana Sayfa
        </button>
      </div>

      <!-- Share Section -->
      <div class="glass-card rounded-2xl p-6 mt-8">
        <h3 class="text-xl font-bold text-white mb-4 text-center">📱 Paylaş</h3>
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
            @click="copyScore"
            class="bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded-xl text-sm transition-all duration-300 transform hover:scale-105 flex items-center space-x-2"
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
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()

// Get results from route params
const score = computed(() => parseInt(route.params.score as string) || 0)
const answered = computed(() => parseInt(route.params.answered as string) || 0)
const correct = computed(() => parseInt(route.params.correct as string) || 0)
const accuracy = computed(() => parseInt(route.params.accuracy as string) || 0)
const qps = computed(() => parseFloat(route.params.qps as string) || 0)

const averageTime = computed(() => {
  if (answered.value === 0) return 0
  return (30 / answered.value).toFixed(1)
})

const getPerformanceBadge = () => {
  const totalScore = score.value
  
  if (totalScore >= 500) {
    return {
      icon: '🔥',
      title: 'SÜPER SONIC!',
      description: 'İnanılmaz hızlısın! Rekor performans!'
    }
  } else if (totalScore >= 300) {
    return {
      icon: '⚡',
      title: 'HIZ CANAVARISI',
      description: 'Çok hızlısın! Harika refleksler!'
    }
  } else if (totalScore >= 200) {
    return {
      icon: '🚀',
      title: 'HIZ DEMONİ',
      description: 'İyi tempo! Güzel performans!'
    }
  } else if (totalScore >= 100) {
    return {
      icon: '🏃',
      title: 'HIZ ÖĞRENCİSİ',
      description: 'Güzel başlangıç! Pratik yaparak gelişebilirsin!'
    }
  } else {
    return {
      icon: '🐌',
      title: 'YAVAŞ VE İSTİKRARLI',
      description: 'Acele etme! Doğruluk hızdan önemli!'
    }
  }
}

const getBestCategory = () => {
  // Mock implementation - could be enhanced with real category tracking
  const categories = ['Portreler', 'Manzara', 'Objeler']
  return categories[Math.floor(Math.random() * categories.length)]
}

const getSpeedRank = () => {
  if (qps.value >= 1.0) return 'Top %5'
  if (qps.value >= 0.7) return 'Top %15'
  if (qps.value >= 0.5) return 'Top %30'
  if (qps.value >= 0.3) return 'Top %50'
  return 'Gelişim Aşaması'
}

const getAchievement = () => {
  if (answered.value >= 20) return 'Hızlı Makine - 20+ soru cevapladın!'
  if (accuracy.value >= 90) return 'Mükemmel Göz - %90+ doğruluk!'
  if (qps.value >= 1.0) return 'Sürat Kralı - 1+ soru/saniye!'
  if (score.value >= 300) return 'Puan Avcısı - 300+ puan!'
  return null
}

const playTimeRushAgain = () => {
  router.push('/time-rush')
}

const goToLeaderboard = () => {
  router.push('/leaderboard')
}

const goToNormalGame = () => {
  router.push('/game')
}

const goHome = () => {
  router.push('/')
}

const shareToTwitter = () => {
  const text = `🔥 AI vs İnsan oyununda ${score.value} puan aldım! ${answered.value} soruyu ${accuracy.value}% doğrulukla cevapladım! Sen de dene: `
  const url = encodeURIComponent(window.location.origin)
  window.open(`https://twitter.com/intent/tweet?text=${encodeURIComponent(text)}&url=${url}`, '_blank')
}

const copyScore = async () => {
  const text = `🔥 AI vs İnsan Zamana Karşı Yarış Sonucum:
⚡ Puan: ${score.value}
📊 Cevaplanan: ${answered.value}
✅ Doğru: ${correct.value}
🎯 Başarı: ${accuracy.value}%
⏱️ Hız: ${qps.value} soru/saniye

Sen de dene! ${window.location.origin}`

  try {
    await navigator.clipboard.writeText(text)
    // Show success message (could add a toast notification)
  } catch (err) {
    console.error('Failed to copy: ', err)
  }
}

onMounted(() => {
  // Add entrance animation delay
  setTimeout(() => {
    document.querySelector('.animate-fade-in')?.classList.add('animation-complete')
  }, 500)
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

.animate-fade-in {
  animation: fadeIn 0.8s ease-out;
}

.animate-pulse-slow {
  animation: pulse 3s infinite;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse {
  0%, 100% { opacity: 0.7; }
  50% { opacity: 0.3; }
}

/* Grid responsive */
@media (max-width: 768px) {
  .md\\:grid-cols-4 {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
