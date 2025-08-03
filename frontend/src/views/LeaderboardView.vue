<template>
  <div class="min-h-screen gradient-bg p-4 relative overflow-hidden">
    <!-- Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute top-20 right-20 w-64 h-64 bg-yellow-300/10 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-20 left-20 w-64 h-64 bg-blue-300/10 rounded-full blur-3xl"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-80 h-80 bg-purple-300/10 rounded-full blur-3xl animate-pulse-slow"></div>
    </div>

    <div class="relative z-10 max-w-4xl mx-auto animate-fade-in">
      <!-- Header -->
      <div class="text-center mb-8">
        <div class="inline-flex items-center space-x-3 bg-white/10 backdrop-blur-md rounded-2xl px-6 py-3 border border-white/20 mb-4">
          <div class="text-2xl">🏆</div>
          <h1 class="text-3xl font-bold text-white">Liderlik Tablosu</h1>
        </div>
        <p class="text-white/80 text-lg">En iyi AI vs İnsan dedektifleri</p>

        <!-- Stats Overview -->
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mt-8">
          <div class="glass-card rounded-2xl p-4 text-center">
            <div class="text-3xl font-bold text-yellow-300">{{ totalGames }}</div>
            <div class="text-white/70 text-sm">Toplam Oyun</div>
          </div>
          <div class="glass-card rounded-2xl p-4 text-center">
            <div class="text-3xl font-bold text-blue-300">{{ averageScore }}%</div>
            <div class="text-white/70 text-sm">Ortalama Skor</div>
          </div>
          <div class="glass-card rounded-2xl p-4 text-center">
            <div class="text-3xl font-bold text-green-300">{{ topPlayers.length }}</div>
            <div class="text-white/70 text-sm">Aktif Oyuncu</div>
          </div>
        </div>
      </div>

      <!-- Current Player Highlight -->
      <div v-if="currentPlayerEntry" class="glass-card rounded-2xl p-6 mb-8 border-2 border-yellow-400/50 animate-slide-up">
        <div class="text-center">
          <div class="text-2xl mb-2">🎉</div>
          <h3 class="text-xl font-bold text-white mb-2">Son Oyununuz!</h3>
          <div class="flex items-center justify-center space-x-4">
            <div class="text-4xl">{{ currentPlayerEntry.avatar }}</div>
            <div>
              <div class="text-lg font-semibold text-white">{{ currentPlayerEntry.playerName }}</div>
              <div class="text-2xl font-bold text-yellow-300">{{ currentPlayerEntry.successRate }}%</div>
              <div class="text-sm text-white/70">{{ currentPlayerEntry.badge }}</div>
            </div>
            <div class="text-center">
              <div class="text-sm text-white/70">Sıralama</div>
              <div class="text-2xl font-bold text-blue-300">#{{ getPlayerRank(currentPlayerEntry.id) }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Leaderboard Table -->
      <div class="glass-card rounded-3xl p-6">
        <div class="space-y-4">
          <div 
            v-for="(player, index) in topPlayers" 
            :key="player.id"
            class="flex items-center space-x-4 p-4 rounded-2xl transition-all duration-300 hover:bg-white/10"
            :class="[
              index === 0 ? 'bg-gradient-to-r from-yellow-500/20 to-orange-500/20 border border-yellow-400/30' :
              index === 1 ? 'bg-gradient-to-r from-gray-400/20 to-gray-500/20 border border-gray-400/30' :
              index === 2 ? 'bg-gradient-to-r from-orange-600/20 to-orange-700/20 border border-orange-400/30' :
              'bg-white/5 border border-white/10',
              player.id === currentPlayerEntry?.id ? 'ring-2 ring-blue-400' : ''
            ]"
          >
            <!-- Rank -->
            <div class="flex-shrink-0 w-12 text-center">
              <div v-if="index === 0" class="text-2xl">🥇</div>
              <div v-else-if="index === 1" class="text-2xl">🥈</div>
              <div v-else-if="index === 2" class="text-2xl">🥉</div>
              <div v-else class="text-xl font-bold text-white/70">#{{ index + 1 }}</div>
            </div>

            <!-- Avatar -->
            <div class="flex-shrink-0 w-12 h-12 bg-white/10 rounded-full flex items-center justify-center text-2xl backdrop-blur-sm">
              {{ player.avatar }}
            </div>

            <!-- Player Info -->
            <div class="flex-1 min-w-0">
              <div class="flex items-center space-x-2 mb-1">
                <h3 class="text-lg font-semibold text-white truncate">{{ player.playerName }}</h3>
                <div v-if="player.id === currentPlayerEntry?.id" class="px-2 py-1 bg-blue-500 text-white text-xs rounded-full">
                  Sen
                </div>
              </div>
              <div class="text-sm text-white/70">{{ player.badge }}</div>
            </div>

            <!-- Score -->
            <div class="flex-shrink-0 text-right">
              <div class="text-2xl font-bold text-white">{{ player.successRate }}%</div>
              <div class="text-sm text-white/70">{{ player.score }}/{{ player.totalQuestions }}</div>
            </div>

            <!-- Date -->
            <div class="flex-shrink-0 text-right text-sm text-white/50 w-20">
              {{ formatDate(player.timestamp) }}
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="topPlayers.length === 0" class="text-center py-12">
          <div class="text-6xl mb-4">🎮</div>
          <h3 class="text-xl font-semibold text-white mb-2">Henüz kimse oynamamış</h3>
          <p class="text-white/70">İlk oyuncu sen ol!</p>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex gap-4 mt-8 justify-center">
        <button 
          @click="goToGame"
          class="btn-primary text-lg px-8 py-3 flex items-center space-x-2"
        >
          <span>🎮</span>
          <span>Oyuna Katıl</span>
        </button>
        
        <button 
          @click="goHome"
          class="bg-white/10 hover:bg-white/20 text-white font-semibold py-3 px-6 rounded-xl transition-all duration-300 border border-white/30"
        >
          🏠 Ana Sayfa
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useLeaderboardStore } from '../stores/leaderboard'

const router = useRouter()
const leaderboardStore = useLeaderboardStore()

const topPlayers = computed(() => leaderboardStore.topPlayers)
const currentPlayerEntry = computed(() => leaderboardStore.currentPlayerEntry)
const totalGames = computed(() => leaderboardStore.totalGames)
const averageScore = computed(() => leaderboardStore.averageScore)

const getPlayerRank = (playerId: string) => {
  return leaderboardStore.getPlayerRank(playerId)
}

const formatDate = (date: Date) => {
  const now = new Date()
  const diffInDays = Math.floor((now.getTime() - new Date(date).getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffInDays === 0) return 'Bugün'
  if (diffInDays === 1) return 'Dün'
  if (diffInDays < 7) return `${diffInDays}g önce`
  if (diffInDays < 30) return `${Math.floor(diffInDays / 7)}h önce`
  return new Date(date).toLocaleDateString('tr-TR', { 
    month: 'short', 
    day: 'numeric' 
  })
}

const goToGame = () => {
  router.push('/game')
}

const goHome = () => {
  router.push('/')
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
  animation: slideUp 0.6s ease-out 0.2s both;
}

.animate-pulse-slow {
  animation: pulse 3s infinite;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse {
  0%, 100% { opacity: 0.7; }
  50% { opacity: 0.3; }
}

/* Responsive grid */
@media (max-width: 768px) {
  .grid-cols-1 {
    grid-template-columns: repeat(1, 1fr);
  }
}

@media (min-width: 768px) {
  .md\\:grid-cols-3 {
    grid-template-columns: repeat(3, 1fr);
  }
}
</style>
