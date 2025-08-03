<template>
  <div class="min-h-screen modern-gradient flex items-center justify-center p-4 relative overflow-hidden">
    <!-- Enhanced Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="floating-orb orb-1"></div>
      <div class="floating-orb orb-2"></div>
      <div class="floating-orb orb-3"></div>
      <div class="grid-pattern"></div>
    </div>

    <!-- Name Input Modal -->
    <div v-if="showNameModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm">
      <div class="modern-card rounded-3xl p-8 max-w-md w-full mx-4 scale-in">
        <div class="text-center mb-6">
          <div class="text-6xl mb-4 animate-bounce">👤</div>
          <h2 class="text-3xl font-bold text-white mb-2 gradient-text">İsim Değiştir</h2>
          <p class="text-white/80">Seni nasıl çağırayım?</p>
        </div>
        
        <div class="mb-6">
          <input 
            v-model="newUserName"
            @keyup.enter="saveName"
            type="text" 
            placeholder="Yeni ismin nedir?"
            class="modern-input w-full px-4 py-3 text-center text-lg"
            maxlength="20"
            autofocus
          />
        </div>
        
        <div class="flex gap-3">
          <button 
            @click="generateRandomName"
            class="modern-btn-secondary flex-1"
          >
            🎲 Rastgele
          </button>
          <button 
            @click="saveName"
            :disabled="!newUserName.trim()"
            class="modern-btn-primary flex-1"
          >
            ✅ Kaydet
          </button>
        </div>
        
        <button 
          @click="cancelNameChange"
          class="w-full mt-3 text-white/60 hover:text-white text-sm transition-colors duration-300"
        >
          İptal
        </button>
      </div>
    </div>

    <div v-if="!$route.query.join" class="relative z-10 w-full max-w-md">
      <!-- Main Card -->
      <div class="modern-card rounded-3xl p-8 text-center scale-in">
        <!-- Welcome Section -->
        <div class="mb-8">
          <div class="hero-icon mb-6">
            <div class="icon-glow">
              <svg class="w-16 h-16 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
              </svg>
            </div>
          </div>
          <h1 class="text-4xl font-bold text-white mb-3 gradient-text">
            Merhaba {{ userStore.userName }}! 
            <span class="wave">👋</span>
          </h1>
          <p class="text-white/80 text-lg">AI mi İnsan mı? Zekânı test et!</p>
        </div>

        <!-- Enhanced User Stats -->
        <div v-if="userStore.userStats.totalGames > 0" class="mb-8 p-6 stats-card rounded-2xl">
          <h3 class="text-white font-semibold mb-4">📊 İstatistiklerin</h3>
          <div class="grid grid-cols-3 gap-4">
            <div class="stat-item">
              <div class="stat-number text-yellow-300">{{ userStore.userStats.totalGames }}</div>
              <div class="stat-label">Oyun</div>
            </div>
            <div class="stat-item">
              <div class="stat-number text-green-300">{{ userStore.userStats.bestScore }}</div>
              <div class="stat-label">En İyi</div>
            </div>
            <div class="stat-item">
              <div class="stat-number text-blue-300">{{ Math.round(userStore.userStats.averageAccuracy) }}%</div>
              <div class="stat-label">Başarı</div>
            </div>
          </div>
        </div>
        
        <!-- Enhanced Game Modes -->
        <div class="space-y-4 mb-8">
          <!-- Active Mode -->
          <div class="game-mode-card active slide-in-left">
            <div class="flex items-center space-x-4">
              <div class="mode-icon">🖼️</div>
              <div class="text-left flex-1">
                <h3 class="font-semibold text-white text-lg">Görsel Mod</h3>
                <p class="text-white/70 text-sm mt-1">Görsellerin AI ya da insan yapımı olduğunu tahmin et</p>
              </div>
              <div class="status-indicator active"></div>
            </div>
          </div>
          
        </div>
        
        <!-- Enhanced Action Buttons -->
        <div class="space-y-4">
          <button 
            @click="startGame"
            class="modern-btn-primary w-full text-xl py-4 px-8 group"
          >
            <span class="flex items-center justify-center">
              🚀 <span class="ml-2">Oyuna Başla</span>
            </span>
          </button>

          <div class="grid grid-cols-1 gap-3">
            <button 
              @click="goToLeaderboard"
              class="modern-btn-accent w-full"
            >
              🏆 Liderlik Tablosu
            </button>

            <button 
              @click="goToTimeRush"
              class="modern-btn-warning w-full"
            >
              ⚡ Zamana Karşı Yarış
            </button>

            <button 
              @click="goToDuel"
              class="modern-btn-purple w-full"
            >
              ⚔️ Düello Modu
            </button>
          </div>
        </div>

        <!-- Enhanced Change Name Button -->
        <div class="mt-8 pt-6 border-t border-white/20">
          <button 
            @click="changeName"
            class="change-name-btn"
          >
            👤 İsim Değiştir
          </button>
        </div>
      </div>

      <!-- Enhanced Footer -->
      <div class="text-center mt-6 fade-in-up">
        <p class="text-white/60 text-sm">Yapay zeka çağında insanlığını koru! 🤖✨</p>
      </div>
    </div>

    <!-- Enhanced Loading for duel invite -->
    <div v-if="$route.query.join" class="relative z-10 w-full max-w-md">
      <div class="modern-card rounded-3xl p-8 text-center scale-in">
        <div class="text-6xl mb-4 animate-spin">⚔️</div>
        <h2 class="text-2xl font-bold text-white mb-4 gradient-text">Düelloya yönlendiriliyor...</h2>
        <div class="loading-dots">
          <div class="dot"></div>
          <div class="dot"></div>
          <div class="dot"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useUserStore } from '../stores/user'
import { useSound } from '../composables/useSound'

const router = useRouter()
const route = useRoute()
const userStore = useUserStore()
const { sounds } = useSound()

// Name change modal
const showNameModal = ref(false)
const newUserName = ref('')

// Otomatik isim üretme fonksiyonu (sadece ilk kez için)
const generateRandomNameForNewUser = () => {
  const adjectives = ['Hızlı', 'Zeki', 'Güçlü', 'Cesur', 'Parlak', 'Seri', 'Usta', 'Pro', 'Elite', 'Süper']
  const nouns = ['Oyuncu', 'Dedektif', 'Kahraman', 'Ninja', 'Avcı', 'Master', 'Şampiyon', 'Pilot', 'Savaşçı', 'Rakip']
  const randomAdj = adjectives[Math.floor(Math.random() * adjectives.length)]
  const randomNoun = nouns[Math.floor(Math.random() * nouns.length)]
  const randomNum = Math.floor(Math.random() * 999) + 1
  return `${randomAdj}${randomNoun}${randomNum}`
}

onMounted(() => {
  // Önce storage'dan yükle
  userStore.loadUserFromStorage()
  
  // Eğer kullanıcı adı yoksa otomatik isim ver
  if (!userStore.isNameSet) {
    const randomName = generateRandomNameForNewUser()
    userStore.setUserName(randomName)
    console.log('Generated initial random name:', randomName)
  } else {
    console.log('Using existing user name:', userStore.userName)
  }

  // Handle duel invite from URL - multiple ways to check
  console.log('Current route:', route)
  console.log('Route query:', route.query)
  console.log('Route params:', route.params)
  console.log('Full URL:', window.location.href)
  
  const joinRoomId = route.query.join || route.params.roomId
  
  if (joinRoomId) {
    console.log('Redirecting to duel with room ID:', joinRoomId)
    // Use nextTick to ensure proper navigation
    setTimeout(() => {
      router.push(`/duel?join=${joinRoomId}`)
    }, 100)
  } else {
    // Alternatif: URL'den manuel parsing
    const urlParams = new URLSearchParams(window.location.search)
    const joinFromUrl = urlParams.get('join')
    
    if (joinFromUrl) {
      console.log('Found join parameter in URL:', joinFromUrl)
      setTimeout(() => {
        router.push(`/duel?join=${joinFromUrl}`)
      }, 100)
    }
  }
})

const changeName = () => {
  newUserName.value = userStore.userName
  showNameModal.value = true
}

const generateRandomName = () => {
  const adjectives = ['Hızlı', 'Zeki', 'Güçlü', 'Cesur', 'Parlak', 'Seri', 'Usta', 'Pro', 'Elite', 'Süper']
  const nouns = ['Oyuncu', 'Dedektif', 'Kahraman', 'Ninja', 'Avcı', 'Master', 'Şampiyon', 'Pilot', 'Savaşçı', 'Rakip']
  const randomAdj = adjectives[Math.floor(Math.random() * adjectives.length)]
  const randomNoun = nouns[Math.floor(Math.random() * nouns.length)]
  const randomNum = Math.floor(Math.random() * 999) + 1
  newUserName.value = `${randomAdj}${randomNoun}${randomNum}`
}

const saveName = () => {
  if (newUserName.value.trim()) {
    userStore.setUserName(newUserName.value.trim())
    showNameModal.value = false
    newUserName.value = ''
  }
}

const cancelNameChange = () => {
  showNameModal.value = false
  newUserName.value = ''
}

const startGame = () => {
  sounds.click()
  router.push('/game')
}

const goToLeaderboard = () => {
  sounds.click()
  router.push('/leaderboard')
}

const goToTimeRush = () => {
  sounds.click()
  router.push('/time-rush')
}

const goToDuel = () => {
  sounds.click()
  router.push('/duel')
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
  animation: gradientFlow 15s ease infinite;
}

@keyframes gradientFlow {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

/* Enhanced Floating Orbs */
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

/* Modern Card Styles */
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
  transform: translateY(-5px);
  box-shadow: 
    0 35px 70px -12px rgba(0, 0, 0, 0.35),
    0 0 0 1px rgba(255, 255, 255, 0.2) inset,
    0 12px 48px rgba(31, 38, 135, 0.47);
}

/* Enhanced Hero Icon */
.hero-icon {
  position: relative;
  display: inline-block;
}

.icon-glow {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 24px;
  box-shadow: 
    0 0 30px rgba(102, 126, 234, 0.5),
    0 0 60px rgba(118, 75, 162, 0.3);
  animation: iconGlow 3s ease-in-out infinite alternate;
  position: relative;
}

.icon-glow::before {
  content: '';
  position: absolute;
  inset: -2px;
  background: linear-gradient(45deg, #ff6b6b, #4ecdc4, #45b7d1, #96ceb4, #feca57);
  border-radius: 26px;
  z-index: -1;
  animation: borderRotate 4s linear infinite;
}

@keyframes iconGlow {
  0% { box-shadow: 0 0 30px rgba(102, 126, 234, 0.5), 0 0 60px rgba(118, 75, 162, 0.3); }
  100% { box-shadow: 0 0 50px rgba(102, 126, 234, 0.8), 0 0 80px rgba(118, 75, 162, 0.5); }
}

@keyframes borderRotate {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
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

/* Wave Animation */
.wave {
  animation: wave 2s ease-in-out infinite;
  transform-origin: 70% 70%;
}

@keyframes wave {
  0%, 100% { transform: rotate(0deg); }
  10%, 30%, 50%, 70%, 90% { transform: rotate(-10deg); }
  20%, 40%, 60%, 80% { transform: rotate(10deg); }
}

/* Enhanced Stats Card */
.stats-card {
  background: linear-gradient(135deg, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0.05) 100%);
  border: 1px solid rgba(255,255,255,0.2);
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.stat-item {
  text-align: center;
  transition: transform 0.3s ease;
}

.stat-item:hover {
  transform: scale(1.1);
}

.stat-number {
  font-size: 1.5rem;
  font-weight: bold;
  line-height: 1;
}

.stat-label {
  font-size: 0.75rem;
  color: rgba(255,255,255,0.7);
  margin-top: 0.25rem;
}

/* Game Mode Cards */
.game-mode-card {
  padding: 1.5rem;
  border-radius: 1rem;
  transition: all 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  position: relative;
  overflow: hidden;
}

.game-mode-card.active {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.2) 0%, rgba(147, 51, 234, 0.2) 100%);
  border: 2px solid rgba(59, 130, 246, 0.5);
  box-shadow: 0 0 30px rgba(59, 130, 246, 0.3);
}

.game-mode-card.disabled {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.game-mode-card.active:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 40px rgba(59, 130, 246, 0.4);
}

.mode-icon {
  font-size: 2rem;
  filter: drop-shadow(0 0 10px rgba(255,255,255,0.3));
}

.status-indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  position: relative;
}

.status-indicator.active {
  background: #10b981;
  box-shadow: 0 0 10px #10b981;
  animation: pulse 2s ease-in-out infinite;
}

.status-indicator.disabled {
  background: #6b7280;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

/* Modern Button Styles */
.modern-btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 1rem 2rem;
  transition: all 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  position: relative;
  overflow: hidden;
  box-shadow: 0 10px 30px rgba(102, 126, 234, 0.3);
}

.modern-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 15px 40px rgba(102, 126, 234, 0.5);
}

.modern-btn-primary:active {
  transform: translateY(0);
}

.modern-btn-secondary {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border: none;
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 0.75rem 1.5rem;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px rgba(251, 191, 36, 0.3);
}

.modern-btn-secondary:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 35px rgba(251, 191, 36, 0.5);
}

.modern-btn-accent {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  border: none;
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 1rem 2rem;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px rgba(251, 191, 36, 0.3);
}

.modern-btn-accent:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 35px rgba(251, 191, 36, 0.5);
}

.modern-btn-warning {
  background: linear-gradient(135deg, #ef4444 0%, #f97316 100%);
  border: none;
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 1rem 2rem;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px rgba(239, 68, 68, 0.3);
}

.modern-btn-warning:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 35px rgba(239, 68, 68, 0.5);
}

.modern-btn-purple {
  background: linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%);
  border: none;
  color: white;
  font-weight: 600;
  border-radius: 1rem;
  padding: 1rem 2rem;
  transition: all 0.3s ease;
  box-shadow: 0 8px 25px rgba(139, 92, 246, 0.3);
}

.modern-btn-purple:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 35px rgba(139, 92, 246, 0.5);
}

/* Modern Input */
.modern-input {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 1rem;
  color: white;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.modern-input::placeholder {
  color: rgba(255, 255, 255, 0.5);
}

.modern-input:focus {
  outline: none;
  border-color: rgba(102, 126, 234, 0.7);
  box-shadow: 0 0 20px rgba(102, 126, 234, 0.3);
  background: rgba(255, 255, 255, 0.15);
}

/* Change Name Button */
.change-name-btn {
  color: rgba(255, 255, 255, 0.6);
  font-size: 0.875rem;
  transition: all 0.3s ease;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  border: 1px solid transparent;
}

.change-name-btn:hover {
  color: white;
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(255, 255, 255, 0.2);
}

/* Loading Dots */
.loading-dots {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  margin-top: 1rem;
}

.dot {
  width: 8px;
  height: 8px;
  background: white;
  border-radius: 50%;
  animation: dotBounce 1.4s ease-in-out infinite both;
}

.dot:nth-child(1) { animation-delay: -0.32s; }
.dot:nth-child(2) { animation-delay: -0.16s; }

@keyframes dotBounce {
  0%, 80%, 100% { transform: scale(0); }
  40% { transform: scale(1); }
}

/* Animation Classes */
.scale-in {
  animation: scaleIn 0.5s cubic-bezier(0.25, 0.46, 0.45, 0.94);
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

.slide-in-left {
  animation: slideInLeft 0.6s ease-out;
}

@keyframes slideInLeft {
  0% { 
    opacity: 0; 
    transform: translateX(-30px); 
  }
  100% { 
    opacity: 1; 
    transform: translateX(0); 
  }
}

.slide-in-right {
  animation: slideInRight 0.6s ease-out;
}

@keyframes slideInRight {
  0% { 
    opacity: 0; 
    transform: translateX(30px); 
  }
  100% { 
    opacity: 1; 
    transform: translateX(0); 
  }
}

.fade-in-up {
  animation: fadeInUp 0.8s ease-out;
}

@keyframes fadeInUp {
  0% { 
    opacity: 0; 
    transform: translateY(20px); 
  }
  100% { 
    opacity: 1; 
    transform: translateY(0); 
  }
}

/* Responsive Design */
@media (max-width: 640px) {
  .modern-card {
    margin: 1rem;
    padding: 1.5rem;
  }
  
  .hero-icon .icon-glow {
    width: 60px;
    height: 60px;
  }
  
  .mode-icon {
    font-size: 1.5rem;
  }
  
  .stat-number {
    font-size: 1.25rem;
  }
}
</style>
