<template>
  <div class="min-h-screen gradient-bg flex items-center justify-center p-4 relative overflow-hidden">
    <!-- Animated Background Elements -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="absolute -top-40 -right-40 w-80 h-80 bg-white/10 rounded-full blur-3xl animate-pulse-slow"></div>
      <div class="absolute -bottom-40 -left-40 w-80 h-80 bg-blue-300/20 rounded-full blur-3xl animate-bounce-slow"></div>
      <div class="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-purple-300/10 rounded-full blur-3xl"></div>
    </div>

    <!-- Name Input Modal -->
    <div v-if="!userStore.isNameSet" class="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm animate-fade-in">
      <div class="glass-card rounded-3xl p-8 max-w-md w-full mx-4 animate-slide-up">
        <div class="text-center mb-6">
          <div class="text-6xl mb-4">👋</div>
          <h2 class="text-3xl font-bold text-white mb-2">Hoş Geldin!</h2>
          <p class="text-white/80">Seni nasıl çağırayım?</p>
        </div>
        
        <div class="mb-6">
          <input 
            v-model="tempUserName"
            @keyup.enter="saveName"
            type="text" 
            placeholder="İsmin nedir?"
            class="w-full px-4 py-3 bg-white/10 border border-white/30 rounded-2xl text-white placeholder-white/50 text-center text-lg focus:outline-none focus:ring-2 focus:ring-blue-400"
            maxlength="20"
            autofocus
          />
        </div>
        
        <button 
          @click="saveName"
          :disabled="!tempUserName.trim()"
          class="w-full btn-primary text-lg py-3 px-6 rounded-2xl disabled:opacity-50 disabled:cursor-not-allowed"
        >
          🚀 Oyuna Başla
        </button>
      </div>
    </div>

    <div v-else class="relative z-10 w-full max-w-md animate-fade-in">
      <!-- Main Card -->
      <div class="glass-card rounded-3xl p-8 text-center backdrop-blur-xl border border-white/30">
        <!-- Welcome Section -->
        <div class="mb-6">
          <div class="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl mb-4 shadow-neon">
            <svg class="w-10 h-10 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
            </svg>
          </div>
          <h1 class="text-3xl font-bold text-white mb-2 tracking-tight">Merhaba {{ userStore.userName }}! 👋</h1>
          <p class="text-white/80 text-lg">AI mi İnsan mı? Zekânı test et!</p>
        </div>

        <!-- User Stats Preview -->
        <div v-if="userStore.userStats.totalGames > 0" class="mb-6 p-4 bg-white/10 rounded-2xl border border-white/20">
          <div class="grid grid-cols-3 gap-3 text-center">
            <div>
              <div class="text-xl font-bold text-yellow-300">{{ userStore.userStats.totalGames }}</div>
              <div class="text-white/60 text-xs">Oyun</div>
            </div>
            <div>
              <div class="text-xl font-bold text-green-300">{{ userStore.userStats.bestScore }}</div>
              <div class="text-white/60 text-xs">En İyi</div>
            </div>
            <div>
              <div class="text-xl font-bold text-blue-300">{{ Math.round(userStore.userStats.averageAccuracy) }}%</div>
              <div class="text-white/60 text-xs">Ortalama</div>
            </div>
          </div>
        </div>
        
        <!-- Game Modes -->
        <div class="space-y-3 mb-8">
          <!-- Active Mode -->
          <div class="p-4 bg-gradient-to-r from-blue-500/20 to-purple-500/20 rounded-2xl border-2 border-blue-400/50 card-hover backdrop-blur-sm animate-slide-up">
            <div class="flex items-center space-x-3">
              <div class="w-3 h-3 bg-green-400 rounded-full animate-pulse"></div>
              <div class="text-left flex-1">
                <h3 class="font-semibold text-white text-lg">🖼️ Görsel Mod</h3>
                <p class="text-white/70 text-sm mt-1">Görsellerin AI ya da insan yapımı olduğunu tahmin et</p>
              </div>
            </div>
          </div>
          
          <!-- Coming Soon Modes -->
          <div class="p-4 bg-white/5 rounded-2xl border border-white/20 opacity-60">
            <div class="flex items-center space-x-3">
              <div class="w-3 h-3 bg-gray-400 rounded-full"></div>
              <div class="text-left flex-1">
                <h3 class="font-semibold text-white/60">📝 Metin Mod</h3>
                <p class="text-white/40 text-sm mt-1">Yakında geliyor...</p>
              </div>
            </div>
          </div>
          
          <div class="p-4 bg-white/5 rounded-2xl border border-white/20 opacity-60">
            <div class="flex items-center space-x-3">
              <div class="w-3 h-3 bg-gray-400 rounded-full"></div>
              <div class="text-left flex-1">
                <h3 class="font-semibold text-white/60">🌐 Website Mod</h3>
                <p class="text-white/40 text-sm mt-1">Yakında geliyor...</p>
              </div>
            </div>
          </div>
          
          <div class="p-4 bg-white/5 rounded-2xl border border-white/20 opacity-60">
            <div class="flex items-center space-x-3">
              <div class="w-3 h-3 bg-gray-400 rounded-full"></div>
              <div class="text-left flex-1">
                <h3 class="font-semibold text-white/60">🔀 Karışık Mod</h3>
                <p class="text-white/40 text-sm mt-1">Yakında geliyor...</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Start Button -->
        <button 
          @click="startGame"
          class="w-full btn-primary text-xl py-4 px-8 rounded-2xl relative overflow-hidden group mb-4"
        >
          <span class="relative z-10">🚀 Oyuna Başla</span>
          <div class="absolute inset-0 bg-gradient-to-r from-blue-600 to-purple-600 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
        </button>

        <!-- Leaderboard Button -->
        <button 
          @click="goToLeaderboard"
          class="w-full bg-gradient-to-r from-yellow-500 to-orange-500 hover:from-yellow-600 hover:to-orange-600 text-white font-semibold py-4 px-8 rounded-2xl transition-all duration-300 transform hover:scale-105 text-xl mb-4"
        >
          🏆 Liderlik Tablosu
        </button>

        <!-- Time Rush Button -->
        <button 
          @click="goToTimeRush"
          class="w-full bg-gradient-to-r from-red-500 to-orange-500 hover:from-red-600 hover:to-orange-600 text-white font-semibold py-4 px-8 rounded-2xl transition-all duration-300 transform hover:scale-105 text-xl"
        >
          ⚡ Zamana Karşı Yarış
        </button>

        <!-- Change Name Button -->
        <div class="mt-6 pt-6 border-t border-white/20">
          <button 
            @click="changeName"
            class="text-white/60 hover:text-white text-sm transition-colors duration-300"
          >
            👤 İsim Değiştir
          </button>
        </div>
      </div>

      <!-- Footer -->
      <div class="text-center mt-6">
        <p class="text-white/60 text-sm">Yapay zeka çağında insanlığını koru! 🤖</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '../stores/user'

const router = useRouter()
const userStore = useUserStore()
const tempUserName = ref('')

onMounted(() => {
  userStore.loadUserFromStorage()
})

const saveName = () => {
  if (tempUserName.value.trim()) {
    userStore.setUserName(tempUserName.value)
  }
}

const changeName = () => {
  userStore.resetUser()
  tempUserName.value = ''
}

const startGame = () => {
  router.push('/game')
}

const goToLeaderboard = () => {
  router.push('/leaderboard')
}

const goToTimeRush = () => {
  router.push('/time-rush')
}
</script>

<style scoped>
.gradient-bg {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  position: relative;
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

.animate-slide-up {
  animation: slideUp 0.6s ease-out 0.2s both;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
