<template>
  <div class="min-h-screen modern-gradient p-4 relative overflow-hidden">
    <!-- Enhanced Animated Background -->
    <div class="absolute inset-0 overflow-hidden">
      <div class="floating-orb orb-1 bg-purple-glow"></div>
      <div class="floating-orb orb-2 bg-pink-glow"></div>
      <div class="floating-orb orb-3 bg-blue-glow"></div>
      <div class="grid-pattern duel-grid"></div>
      <div class="battle-particles"></div>
    </div>

    <div class="relative z-10 max-w-6xl mx-auto">
      <!-- Enhanced Header -->
      <header class="text-center mb-8 scale-in">
        <div class="duel-header-container">
          <div class="duel-icon-container mb-6">
            <div class="crossed-swords">⚔️</div>
            <div class="duel-glow-ring"></div>
          </div>
          <h1 class="text-5xl md:text-7xl font-bold gradient-text duel-title mb-4">
            Düello Modu
          </h1>
          <p class="text-white/90 text-xl font-medium">Gerçek zamanlı çok oyunculu deneyim</p>
          <div class="battle-status-line mt-4">
            <div class="status-dot"></div>
            <span class="text-white/80 text-sm font-medium">Savaş Arenası Aktif</span>
          </div>
        </div>
      </header>

      <!-- Enhanced Connection Error -->
      <div v-if="connectionError" class="modern-card p-6 mb-6 border-2 border-red-400/50 error-card scale-in">
        <div class="text-center">
          <div class="error-icon mb-4">
            <span class="text-4xl">⚠️</span>
            <div class="error-pulse"></div>
          </div>
          <h3 class="text-xl font-bold text-red-300 mb-2">Bağlantı Hatası</h3>
          <p class="text-white/80 mb-4">{{ connectionError }}</p>
          <button @click="retryConnection" class="modern-btn-warning">
            🔄 Yeniden Dene
          </button>
        </div>
      </div>

      <!-- Name Input Modal for Duel - REMOVED -->
      <!-- Artık global user store kullanıyoruz, modal gerekli değil -->

      <!-- Enhanced Room Creation/Join -->
      <div v-if="!isInRoom" class="modern-card p-8 mb-8 duel-lobby scale-in">
        <div class="text-center mb-8">
          <div class="lobby-icon mb-4">
            <span class="text-4xl">⚔️</span>
            <div class="sword-clash"></div>
          </div>
          <h2 class="text-3xl font-bold text-white mb-4 gradient-text">Düello Oluştur veya Katıl</h2>
          <p class="text-white/90 text-lg">Arkadaşınla gerçek zamanlı düello yap!</p>
        </div>

        <div class="grid md:grid-cols-2 gap-8">
          <!-- Enhanced Create Room -->
          <div class="room-option create-room">
            <div class="option-header">
              <div class="option-icon">🆕</div>
              <h3 class="text-xl font-semibold text-white mb-4">Yeni Düello Oluştur</h3>
            </div>
            <div class="option-content">
              <div class="player-info mb-4">
                <div class="player-avatar">👤</div>
                <div class="player-details">
                  <div class="player-name">{{ userStore.userName }}</div>
                  <div class="player-role">Düello Kurucusu</div>
                </div>
              </div>
              <button
                @click="createNewRoom"
                :disabled="isConnecting"
                class="duel-btn-create w-full"
              >
                <span v-if="isConnecting" class="loading-spinner">⚡</span>
                <span class="btn-text">{{ isConnecting ? 'Oluşturuluyor...' : '⚔️ Düello Oluştur' }}</span>
              </button>
            </div>
          </div>

          <!-- Enhanced Join Room -->
          <div class="room-option join-room">
            <div class="option-header">
              <div class="option-icon">🎯</div>
              <h3 class="text-xl font-semibold text-white mb-4">
                {{ route.query.join ? 'Düelloya Katıl' : 'Mevcut Düelloya Katıl' }}
              </h3>
            </div>
            <div class="option-content">
              <div v-if="route.query.join" class="invite-info mb-4">
                <div class="invite-badge">
                  <span class="invite-icon">🔗</span>
                  <div class="invite-details">
                    <div class="invite-label">Oda ID</div>
                    <div class="invite-code">{{ route.query.join }}</div>
                  </div>
                </div>
              </div>
              
              <div class="player-info mb-4">
                <div class="player-avatar">👤</div>
                <div class="player-details">
                  <div class="player-name">{{ userStore.userName }}</div>
                  <div class="player-role">Düello Katılımcısı</div>
                </div>
              </div>
              
              <input
                v-if="!route.query.join"
                v-model="manualRoomIdInput"
                type="text"
                placeholder="Oda ID'si girin"
                class="duel-input w-full mb-4"
                @keyup.enter="joinExistingRoom"
              />
              
              <button
                @click="route.query.join ? joinViaLink() : joinExistingRoom()"
                :disabled="(!route.query.join && !manualRoomIdInput.trim()) || isConnecting"
                class="duel-btn-join w-full"
              >
                <span v-if="isConnecting" class="loading-spinner">⚡</span>
                <span class="btn-text">{{ isConnecting ? 'Katılınıyor...' : '🚀 Düelloya Katıl' }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Waiting Room -->
      <div v-else-if="currentRoom?.status === 'WaitingForPlayer'" class="glassmorphism p-8 mb-8">
        <div class="text-center">
          <h2 class="text-2xl font-bold text-white mb-4">
            <i class="fas fa-users mr-2"></i>
            Düello Odası
          </h2>
          <p class="text-purple-200 mb-6">İkinci oyuncuyu bekliyoruz...</p>

          <!-- Share Link -->
          <div class="mb-6">
            <p class="text-white font-semibold mb-2">Arkadaşınızı davet edin:</p>
            <div class="flex items-center gap-2">
              <input
                :value="shareLink"
                readonly
                class="flex-1 px-4 py-2 bg-white/10 border border-white/20 rounded-lg text-white text-sm"
              />
              <button
                @click="copyLink"
                class="px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded-lg transition-colors"
                :class="{ 'bg-green-600': linkCopied }"
              >
                <i class="fas" :class="linkCopied ? 'fa-check' : 'fa-copy'"></i>
                {{ linkCopied ? 'Kopyalandı!' : 'Kopyala' }}
              </button>
            </div>
          </div>

          <!-- Room Info -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
            <div class="text-center">
              <p class="text-purple-200">Oda ID</p>
              <p class="text-white font-mono text-lg">{{ roomId }}</p>
            </div>
            <div class="text-center">
              <p class="text-purple-200">Oyuncu</p>
              <p class="text-white font-semibold">{{ playerName }}</p>
            </div>
          </div>

          <button
            @click="leaveRoom"
            class="px-6 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg transition-colors"
          >
            <i class="fas fa-door-open mr-2"></i>
            Odadan Çık
          </button>
        </div>
      </div>

      <!-- Ready Room -->
      <div v-else-if="currentRoom?.status === 'BothPlayersJoined'" class="glassmorphism p-8 mb-8">
        <div class="text-center mb-6">
          <h2 class="text-2xl font-bold text-white mb-4">
            <i class="fas fa-swords mr-2"></i>
            Düello Hazır!
          </h2>
          <p class="text-purple-200">Her iki oyuncu da hazır olduğunda oyun başlayacak</p>
        </div>

        <!-- Players -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <!-- Player 1 -->
          <div class="glassmorphism p-6">
            <div class="text-center">
              <div class="w-16 h-16 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full flex items-center justify-center mx-auto mb-4">
                <i class="fas fa-user text-white text-xl"></i>
              </div>
              <h3 class="text-xl font-semibold text-white mb-2">{{ currentRoom.player1?.name }}</h3>
              <div class="flex items-center justify-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="currentRoom.player1?.isReady ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm" :class="currentRoom.player1?.isReady ? 'text-green-400' : 'text-red-400'">
                  {{ currentRoom.player1?.isReady ? 'Hazır' : 'Hazır Değil' }}
                </span>
              </div>
              <div v-if="currentRoom.player1?.isHost" class="mt-2">
                <span class="px-2 py-1 bg-yellow-500 text-black text-xs rounded-full">Host</span>
              </div>
            </div>
          </div>

          <!-- Player 2 -->
          <div class="glassmorphism p-6">
            <div class="text-center">
              <div class="w-16 h-16 bg-gradient-to-r from-blue-500 to-cyan-500 rounded-full flex items-center justify-center mx-auto mb-4">
                <i class="fas fa-user text-white text-xl"></i>
              </div>
              <h3 class="text-xl font-semibold text-white mb-2">{{ currentRoom.player2?.name }}</h3>
              <div class="flex items-center justify-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="currentRoom.player2?.isReady ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm" :class="currentRoom.player2?.isReady ? 'text-green-400' : 'text-red-400'">
                  {{ currentRoom.player2?.isReady ? 'Hazır' : 'Hazır Değil' }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Ready Button -->
        <div class="text-center">
          <button
            @click="toggleReady"
            :disabled="isSettingReady"
            class="px-8 py-3 font-semibold rounded-lg transition-all duration-300"
            :class="currentPlayer?.isReady ? 
              'bg-red-500 hover:bg-red-600 text-white' : 
              'bg-green-500 hover:bg-green-600 text-white'"
          >
            <i v-if="isSettingReady" class="fas fa-spinner fa-spin mr-2"></i>
            <i v-else class="fas mr-2" :class="currentPlayer?.isReady ? 'fa-times' : 'fa-check'"></i>
            {{ isSettingReady ? 'Güncelleniyor...' : (currentPlayer?.isReady ? 'Hazır Değilim' : 'Hazırım') }}
          </button>
        </div>

        <!-- Auto Start Info -->
        <div v-if="bothPlayersReady" class="text-center mt-6">
          <div class="glassmorphism p-4 border border-green-400/30">
            <i class="fas fa-rocket text-green-400 text-2xl mb-2"></i>
            <p class="text-green-400 font-semibold">Her iki oyuncu hazır! Oyun başlıyor...</p>
          </div>
        </div>
      </div>

      <!-- Game Screen -->
      <div v-else-if="isGameInProgress" class="space-y-6">
        <!-- Game Header -->
        <div class="glassmorphism p-6">
          <div class="flex flex-col md:flex-row justify-between items-center gap-4">
            <!-- Round Info -->
            <div class="text-center md:text-left">
              <h2 class="text-2xl font-bold text-white">
                Round {{ currentRoom?.currentRound }} / {{ currentRoom?.totalRounds }}
              </h2>
              <p class="text-purple-200">AI mi? İnsan mı?</p>
            </div>

            <!-- Timer -->
            <div class="text-center">
              <div class="w-20 h-20 rounded-full border-4 border-purple-500 flex items-center justify-center mx-auto mb-2">
                <span class="text-2xl font-bold text-white">{{ currentRoom?.currentTime }}</span>
              </div>
              <p class="text-purple-200 text-sm">Kalan süre</p>
            </div>

            <!-- Scores -->
            <div class="flex gap-6">
              <div class="text-center">
                <p class="text-purple-200 text-sm">{{ currentRoom?.player1?.name }}</p>
                <p class="text-2xl font-bold text-white">{{ currentRoom?.player1?.score }}</p>
              </div>
              <div class="text-center">
                <p class="text-purple-200 text-sm">{{ currentRoom?.player2?.name }}</p>
                <p class="text-2xl font-bold text-white">{{ currentRoom?.player2?.score }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Question Image -->
        <div v-if="currentRoom?.currentQuestion" class="glassmorphism p-8">
          <div class="max-w-2xl mx-auto">
            <img
              :src="currentRoom.currentQuestion.imageUrl"
              :alt="`Soru ${currentRoom.currentRound}`"
              class="w-full rounded-lg shadow-2xl mb-6"
              @error="handleImageError"
            />

            <!-- Answer Buttons -->
            <div class="grid grid-cols-2 gap-4">
              <button
                @click="submitAnswer('ai')"
                :disabled="hasAnswered || isAnswering || (currentRoom?.currentTime ?? 0) <= 0"
                class="py-4 px-6 bg-gradient-to-r from-red-500 to-pink-500 text-white font-semibold rounded-lg hover:from-red-600 hover:to-pink-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-300 transform hover:scale-105"
              >
                <i class="fas fa-robot text-2xl mb-2"></i>
                <div>AI Üretimi</div>
              </button>

              <button
                @click="submitAnswer('human')"
                :disabled="hasAnswered || isAnswering || (currentRoom?.currentTime ?? 0) <= 0"
                class="py-4 px-6 bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold rounded-lg hover:from-blue-600 hover:to-cyan-600 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-300 transform hover:scale-105"
              >
                <i class="fas fa-user text-2xl mb-2"></i>
                <div>İnsan Yapımı</div>
              </button>
            </div>

            <!-- Answer Status -->
            <div v-if="hasAnswered || opponent?.hasAnswered || (currentRoom?.currentTime ?? 0) <= 0" class="mt-6 space-y-3">
              <!-- Time Up Message -->
              <div v-if="(currentRoom?.currentTime ?? 0) <= 0 && !hasAnswered" class="glassmorphism p-4 border border-red-400/30">
                <p class="text-red-400 text-center">
                  <i class="fas fa-clock mr-2"></i>
                  Süre Doldu! Artık cevap veremezsiniz.
                </p>
              </div>
              
              <div v-if="hasAnswered" class="glassmorphism p-4 border border-green-400/30">
                <p class="text-green-400 text-center">
                  <i class="fas fa-check mr-2"></i>
                  Cevabınız alındı: {{ currentPlayer?.lastAnswer === 'ai' ? 'AI Üretimi' : 'İnsan Yapımı' }}
                </p>
              </div>
              
              <div v-if="opponent?.hasAnswered" class="glassmorphism p-4 border border-blue-400/30">
                <p class="text-blue-400 text-center">
                  <i class="fas fa-user mr-2"></i>
                  {{ opponent.name }} cevabını verdi
                </p>
              </div>

              <div v-if="!opponent?.hasAnswered && (currentRoom?.currentTime ?? 0) > 0" class="glassmorphism p-4 border border-yellow-400/30">
                <p class="text-yellow-400 text-center">
                  <i class="fas fa-hourglass-half mr-2"></i>
                  {{ opponent?.name }} cevap veriyor...
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Game Results -->
      <div v-else-if="currentRoom?.status === 'Finished'" class="glassmorphism p-8">
        <div class="text-center">
          <h2 class="text-3xl font-bold text-white mb-6">
            <i class="fas fa-trophy text-yellow-400 mr-2"></i>
            Düello Sonuçları
          </h2>

          <!-- Winner -->
          <div class="mb-8">
            <div v-if="winner" class="glassmorphism p-6 border border-yellow-400/30">
              <i class="fas fa-crown text-yellow-400 text-4xl mb-4"></i>
              <h3 class="text-2xl font-bold text-yellow-400 mb-2">Kazanan</h3>
              <p class="text-3xl font-bold text-white">{{ winner.name }}</p>
              <p class="text-yellow-200">{{ winner.score }} puan</p>
            </div>
            <div v-else class="glassmorphism p-6 border border-purple-400/30">
              <i class="fas fa-handshake text-purple-400 text-4xl mb-4"></i>
              <h3 class="text-2xl font-bold text-purple-400">Beraberlik!</h3>
              <p class="text-purple-200">İki oyuncu da aynı puanı aldı</p>
            </div>
          </div>

          <!-- Detailed Results -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
            <!-- Player 1 Stats -->
            <div class="glassmorphism p-6">
              <h4 class="text-xl font-semibold text-white mb-4">{{ currentRoom.player1?.name }}</h4>
              <div class="space-y-2 text-left">
                <div class="flex justify-between">
                  <span class="text-purple-200">Toplam Puan:</span>
                  <span class="text-white font-semibold">{{ currentRoom.player1?.score }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Doğru:</span>
                  <span class="text-green-400">{{ currentRoom.player1?.correctAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Yanlış:</span>
                  <span class="text-red-400">{{ currentRoom.player1?.wrongAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Ort. Süre:</span>
                  <span class="text-white">{{ (currentRoom.player1?.averageTime || 0).toFixed(1) }}s</span>
                </div>
              </div>
            </div>

            <!-- Player 2 Stats -->
            <div class="glassmorphism p-6">
              <h4 class="text-xl font-semibold text-white mb-4">{{ currentRoom.player2?.name }}</h4>
              <div class="space-y-2 text-left">
                <div class="flex justify-between">
                  <span class="text-purple-200">Toplam Puan:</span>
                  <span class="text-white font-semibold">{{ currentRoom.player2?.score }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Doğru:</span>
                  <span class="text-green-400">{{ currentRoom.player2?.correctAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Yanlış:</span>
                  <span class="text-red-400">{{ currentRoom.player2?.wrongAnswers }}</span>
                </div>
                <div class="flex justify-between">
                  <span class="text-purple-200">Ort. Süre:</span>
                  <span class="text-white">{{ (currentRoom.player2?.averageTime || 0).toFixed(1) }}s</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <button
              @click="playAgain"
              class="px-8 py-3 bg-gradient-to-r from-green-500 to-emerald-500 text-white font-semibold rounded-lg hover:from-green-600 hover:to-emerald-600 transition-all duration-300"
            >
              <i class="fas fa-redo mr-2"></i>
              Tekrar Oyna
            </button>
            
            <router-link
              to="/"
              class="px-8 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white font-semibold rounded-lg hover:from-purple-600 hover:to-pink-600 transition-all duration-300 text-center"
            >
              <i class="fas fa-home mr-2"></i>
              Ana Sayfa
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useDuelStore } from '@/stores/duel'
import { useUserStore } from '@/stores/user'

const route = useRoute()
const router = useRouter()
const duelStore = useDuelStore()
const userStore = useUserStore()

// Random name generator for invite users (only if no user exists)
const generateRandomName = () => {
  const adjectives = ['Hızlı', 'Akıllı', 'Güçlü', 'Cesur', 'Zeki', 'Parlak', 'Seri', 'Usta', 'Pro', 'Elite']
  const nouns = ['Oyuncu', 'Savaşçı', 'Kahraman', 'Pilot', 'Ninja', 'Avcı', 'Master', 'Şampiyon', 'Düellocu', 'Rakip']
  const randomAdj = adjectives[Math.floor(Math.random() * adjectives.length)]
  const randomNoun = nouns[Math.floor(Math.random() * nouns.length)]
  const randomNum = Math.floor(Math.random() * 999) + 1
  return `${randomAdj}${randomNoun}${randomNum}`
}

// Local state
const newRoomPlayerName = ref('')
const joinPlayerName = ref('')
const joinRoomId = ref('') // This will be populated from URL (path or query)
const manualRoomIdInput = ref('') // For the manual room ID input field
const isConnecting = ref(false)
const isSettingReady = ref(false)
const isAnswering = ref(false)
const hasAnswered = ref(false)
const questionStartTime = ref(0)
const linkCopied = ref(false)

// Computed
const isInRoom = computed(() => duelStore.isInRoom)
const currentRoom = computed(() => duelStore.currentRoom)
const playerName = computed(() => duelStore.playerName)
const roomId = computed(() => duelStore.roomId)
const shareLink = computed(() => duelStore.shareLink)
const connectionError = computed(() => duelStore.connectionError)
const bothPlayersReady = computed(() => duelStore.bothPlayersReady)
const isGameInProgress = computed(() => duelStore.isGameInProgress)
const currentPlayer = computed(() => duelStore.currentPlayer)
const opponent = computed(() => duelStore.opponent)

const winner = computed(() => {
  if (!currentRoom.value || currentRoom.value.status !== 'Finished') return null
  
  const p1Score = currentRoom.value.player1?.score || 0
  const p2Score = currentRoom.value.player2?.score || 0
  
  if (p1Score > p2Score) return currentRoom.value.player1
  if (p2Score > p1Score) return currentRoom.value.player2
  return null
})

// Methods
const createNewRoom = async () => {
  // Use existing user name from user store
  let playerName = userStore.userName || newRoomPlayerName.value.trim()
  if (!playerName) {
    playerName = generateRandomName()
    userStore.setUserName(playerName)
    newRoomPlayerName.value = playerName
    console.log('Generated random name for new user:', playerName)
  }
  
  if (isConnecting.value) return
  isConnecting.value = true
  try {
    // Create room with player name as room name too
    await duelStore.createRoom(playerName, playerName)
  } finally {
    isConnecting.value = false
  }
}

const saveNameAndJoin = async () => {
  // Bu fonksiyon artık kullanılmıyor - modal kaldırıldı
}

const joinRoomHandler = async () => {
  const name = userStore.userName
  const id = (joinRoomId.value || manualRoomIdInput.value).trim()

  if (!name || !id || isConnecting.value) return
  
  isConnecting.value = true
  try {
    await duelStore.joinRoom(id, name)
  } finally {
    isConnecting.value = false
  }
}

const joinExistingRoom = async () => {
  if (!manualRoomIdInput.value.trim() || isConnecting.value) return
  
  isConnecting.value = true
  try {
    await duelStore.joinRoom(manualRoomIdInput.value.trim(), userStore.userName)
  } finally {
    isConnecting.value = false
  }
}

const joinViaLink = async () => {
  if (!joinRoomId.value || isConnecting.value) return
  
  isConnecting.value = true
  try {
    await duelStore.joinRoom(joinRoomId.value, userStore.userName)
  } finally {
    isConnecting.value = false
  }
}

const toggleReady = async () => {
  if (!currentPlayer.value || isSettingReady.value) return
  
  isSettingReady.value = true
  try {
    await duelStore.setReady()
  } finally {
    isSettingReady.value = false
  }
}

const submitAnswer = async (answer: 'ai' | 'human') => {
  // Süre bittiyse cevap kabul edilmez
  if (hasAnswered.value || isAnswering.value || !currentRoom.value?.currentQuestion || (currentRoom.value?.currentTime ?? 0) <= 0) {
    if ((currentRoom.value?.currentTime ?? 0) <= 0) {
      console.log('Time is up! Cannot submit answer.')
    }
    return
  }
  
  isAnswering.value = true
  hasAnswered.value = true
  
  try {
    await duelStore.submitAnswer(answer)
  } finally {
    isAnswering.value = false
  }
}

const copyLink = async () => {
  const success = await duelStore.copyShareLink()
  if (success) {
    linkCopied.value = true
    setTimeout(() => {
      linkCopied.value = false
    }, 2000)
  }
}

const leaveRoom = async () => {
  await duelStore.leaveRoom()
  router.push('/')
}

const retryConnection = async () => {
  // Reset and try to reconnect
  if (roomId.value && playerName.value) {
    await duelStore.joinRoom(roomId.value, playerName.value)
  }
}

const playAgain = async () => {
  // Reset game state and return to ready room
  hasAnswered.value = false
  questionStartTime.value = 0
  
  // Set players ready for new game
  if (currentPlayer.value) {
    await duelStore.setReady(true)
  }
}

const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement
  img.src = 'https://via.placeholder.com/600x450/6366f1/ffffff?text=Resim+Yüklenemedi'
}

// Watch for new questions to reset answer state
watch(() => currentRoom.value?.currentQuestion?.id, (newId, oldId) => {
  if (newId && newId !== oldId) {
    hasAnswered.value = false
    isAnswering.value = false
    questionStartTime.value = Date.now()
  }
})

// Watch for round changes to reset answer state
const watchRoundChanges = () => {
  if (currentRoom.value?.status === 'InProgress' && !hasAnswered.value) {
    questionStartTime.value = Date.now()
    hasAnswered.value = false
  }
}

// Check for URL parameters (join via link)
onMounted(async () => {
  // Load user from storage first
  userStore.loadUserFromStorage()
  
  // Check if joining via URL query parameter FIRST
  console.log('DuelView - Route info:', route)
  console.log('DuelView - Query params:', route.query)
  console.log('DuelView - Full URL:', window.location.href)
  
  let roomIdFromQuery = route.query.join as string
  
  // Alternatif URL parsing - hash'den sonra join parametresi
  if (!roomIdFromQuery) {
    const hash = window.location.hash
    const queryStart = hash.indexOf('?')
    if (queryStart !== -1) {
      const queryString = hash.substring(queryStart + 1)
      const urlParams = new URLSearchParams(queryString)
      roomIdFromQuery = urlParams.get('join') || ''
    }
  }
  
  if (roomIdFromQuery) {
    joinRoomId.value = roomIdFromQuery
    console.log('Found room ID to join:', joinRoomId.value)
  }

  // Use existing user name or generate one if user doesn't exist
  let currentUserName = userStore.userName
  if (!currentUserName) {
    currentUserName = generateRandomName()
    userStore.setUserName(currentUserName)
    console.log('Generated name for new user:', currentUserName)
  } else {
    console.log('Using existing user name:', currentUserName)
  }
  
  // Set the name in form fields (for display purposes only)
  newRoomPlayerName.value = currentUserName
  joinPlayerName.value = currentUserName

  // Initialize SignalR connection
  try {
    await duelStore.initializeConnection()
    duelStore.clearConnectionError()
  } catch (error) {
    console.error('Failed to initialize connection:', error)
    duelStore.setConnectionError('Bağlantı kurulamadı. Lütfen tekrar deneyin.')
  }
  
  // Handle invite link logic
  if (joinRoomId.value) {
    console.log('Auto-joining room:', joinRoomId.value)
    // Wait a bit for connection to establish
    setTimeout(async () => {
      await joinRoomHandler()
    }, 500)
  }
  
  // Watch for round changes
  setInterval(watchRoundChanges, 1000)
})

onUnmounted(async () => {
  // Clean up connection when leaving
  if (duelStore.isConnected) {
    await duelStore.leaveRoom()
  }
})
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
  min-height: 100vh;
  animation: gradientShift 15s ease infinite;
}

@keyframes gradientShift {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

/* Enhanced Floating Orbs */
.floating-orb {
  position: absolute;
  border-radius: 50%;
  opacity: 0.4;
  animation: float 20s infinite linear;
  background: radial-gradient(circle, rgba(255,255,255,0.8), rgba(255,255,255,0.1));
}

.orb-1 {
  width: 200px;
  height: 200px;
  top: 10%;
  left: -5%;
  background: radial-gradient(circle, rgba(168, 85, 247, 0.4), transparent);
  animation-duration: 25s;
}

.orb-2 {
  width: 150px;
  height: 150px;
  top: 60%;
  right: -5%;
  background: radial-gradient(circle, rgba(236, 72, 153, 0.4), transparent);
  animation-duration: 30s;
  animation-direction: reverse;
}

.orb-3 {
  width: 180px;
  height: 180px;
  bottom: 20%;
  left: 60%;
  background: radial-gradient(circle, rgba(59, 130, 246, 0.4), transparent);
  animation-duration: 35s;
}

@keyframes float {
  from { transform: rotate(0deg) translateX(50px) rotate(0deg); }
  to { transform: rotate(360deg) translateX(50px) rotate(-360deg); }
}

/* Grid Pattern */
.grid-pattern {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    linear-gradient(rgba(255,255,255,0.1) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.1) 1px, transparent 1px);
  background-size: 60px 60px;
  opacity: 0.2;
  animation: gridMove 30s linear infinite;
}

.duel-grid {
  background: 
    radial-gradient(circle at 25% 25%, rgba(168, 85, 247, 0.3) 0%, transparent 50%),
    radial-gradient(circle at 75% 75%, rgba(236, 72, 153, 0.3) 0%, transparent 50%);
}

@keyframes gridMove {
  from { transform: translate(0, 0); }
  to { transform: translate(60px, 60px); }
}

/* Battle Particles */
.battle-particles {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    radial-gradient(circle at 20% 20%, rgba(255, 215, 0, 0.1) 0%, transparent 1%),
    radial-gradient(circle at 80% 40%, rgba(255, 69, 0, 0.1) 0%, transparent 1%),
    radial-gradient(circle at 40% 80%, rgba(138, 43, 226, 0.1) 0%, transparent 1%);
  background-size: 200px 200px, 300px 300px, 150px 150px;
  animation: particleDrift 25s linear infinite;
}

@keyframes particleDrift {
  from { background-position: 0% 0%, 0% 0%, 0% 0%; }
  to { background-position: 100% 100%, -100% 100%, 100% -100%; }
}

/* Enhanced Modern Card */
.modern-card {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 24px;
  box-shadow: 
    0 8px 32px rgba(0, 0, 0, 0.3),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
}

.modern-card:hover {
  transform: translateY(-2px);
  box-shadow: 
    0 12px 40px rgba(0, 0, 0, 0.4),
    inset 0 1px 0 rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.3);
}

/* Glassmorphism Enhanced */
.glassmorphism {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 20px;
  box-shadow: 
    0 8px 32px rgba(0, 0, 0, 0.3),
    inset 0 1px 0 rgba(255, 255, 255, 0.1);
}

/* Duel Header Styling */
.duel-header-container {
  position: relative;
  animation: slideInFromTop 1s ease-out;
}

.duel-icon-container {
  position: relative;
  display: inline-block;
}

.crossed-swords {
  font-size: 4rem;
  animation: swordClash 2s ease-in-out infinite;
  filter: drop-shadow(0 0 20px rgba(255, 215, 0, 0.6));
}

@keyframes swordClash {
  0%, 100% { transform: rotate(0deg) scale(1); }
  25% { transform: rotate(-5deg) scale(1.1); }
  75% { transform: rotate(5deg) scale(1.1); }
}

.duel-glow-ring {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 120px;
  height: 120px;
  border: 2px solid rgba(255, 215, 0, 0.5);
  border-radius: 50%;
  transform: translate(-50%, -50%);
  animation: pulseRing 3s ease-in-out infinite;
}

@keyframes pulseRing {
  0%, 100% { 
    transform: translate(-50%, -50%) scale(1);
    opacity: 0.5;
  }
  50% { 
    transform: translate(-50%, -50%) scale(1.2);
    opacity: 0.8;
  }
}

/* Gradient Text */
.gradient-text {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 50%, #4facfe 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: textShimmer 3s ease-in-out infinite;
}

.duel-title {
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3));
}

@keyframes textShimmer {
  0%, 100% { filter: hue-rotate(0deg) brightness(1); }
  50% { filter: hue-rotate(30deg) brightness(1.2); }
}

/* Battle Status Line */
.battle-status-line {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.status-dot {
  width: 8px;
  height: 8px;
  background: #10b981;
  border-radius: 50%;
  animation: statusPulse 2s ease-in-out infinite;
}

@keyframes statusPulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.5; transform: scale(1.2); }
}

/* Error Card Styling */
.error-card {
  background: rgba(239, 68, 68, 0.1);
  backdrop-filter: blur(20px);
  animation: shake 0.5s ease-in-out;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}

.error-icon {
  position: relative;
  display: inline-block;
}

.error-pulse {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border: 2px solid rgba(239, 68, 68, 0.5);
  border-radius: 50%;
  animation: errorPulse 2s infinite;
}

@keyframes errorPulse {
  0% { transform: scale(1); opacity: 1; }
  100% { transform: scale(2); opacity: 0; }
}

/* Modern Button Variants */
.modern-btn-warning {
  background: linear-gradient(135deg, #f59e0b, #d97706);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(245, 158, 11, 0.3);
}

.modern-btn-warning:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(245, 158, 11, 0.4);
}

/* Lobby Styling */
.duel-lobby {
  animation: slideInFromBottom 0.8s ease-out;
}

@keyframes slideInFromBottom {
  from { transform: translateY(50px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.lobby-icon {
  position: relative;
  display: inline-block;
}

.sword-clash {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle, rgba(255, 215, 0, 0.3), transparent);
  border-radius: 50%;
  animation: clashGlow 3s ease-in-out infinite;
}

@keyframes clashGlow {
  0%, 100% { transform: scale(1); opacity: 0.3; }
  50% { transform: scale(1.5); opacity: 0.8; }
}

/* Room Option Cards */
.room-option {
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 20px;
  padding: 32px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.room-option::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.1), transparent);
  transition: left 0.5s ease;
}

.room-option:hover::before {
  left: 100%;
}

.room-option:hover {
  transform: translateY(-5px);
  border-color: rgba(255, 255, 255, 0.3);
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.create-room {
  background: linear-gradient(135deg, rgba(168, 85, 247, 0.1), rgba(236, 72, 153, 0.1));
  border-color: rgba(168, 85, 247, 0.3);
}

.join-room {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1), rgba(16, 185, 129, 0.1));
  border-color: rgba(59, 130, 246, 0.3);
}

.option-header {
  text-align: center;
  margin-bottom: 24px;
}

.option-icon {
  font-size: 2.5rem;
  margin-bottom: 16px;
  display: inline-block;
  animation: bounce 2s infinite;
}

@keyframes bounce {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-5px); }
}

.option-content {
  position: relative;
  z-index: 1;
}

/* Player Info Cards */
.player-info {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 12px;
}

.player-avatar {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
}

.player-details {
  flex: 1;
}

.player-name {
  color: white;
  font-weight: 600;
  font-size: 1.1rem;
}

.player-role {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.9rem;
}

/* Duel Buttons */
.duel-btn-create {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
  border: none;
  padding: 16px 24px;
  border-radius: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(16, 185, 129, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.duel-btn-create:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(16, 185, 129, 0.4);
}

.duel-btn-join {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  border: none;
  padding: 16px 24px;
  border-radius: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.duel-btn-join:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(59, 130, 246, 0.4);
}

.duel-btn-create:disabled,
.duel-btn-join:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* Loading Spinner */
.loading-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Invite Elements */
.invite-info {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 16px;
}

.invite-badge {
  display: flex;
  align-items: center;
  gap: 12px;
}

.invite-icon {
  font-size: 1.5rem;
  color: #10b981;
}

.invite-details {
  flex: 1;
}

.invite-label {
  color: rgba(255, 255, 255, 0.7);
  font-size: 0.8rem;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.invite-code {
  color: white;
  font-weight: 600;
  font-family: 'Courier New', monospace;
  font-size: 1.1rem;
}

/* Duel Input */
.duel-input {
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 12px;
  padding: 12px 16px;
  color: white;
  transition: all 0.3s ease;
}

.duel-input:focus {
  outline: none;
  border-color: rgba(59, 130, 246, 0.5);
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.duel-input::placeholder {
  color: rgba(255, 255, 255, 0.5);
}

/* Animation Classes */
.scale-in {
  animation: scaleIn 0.6s ease-out;
}

@keyframes scaleIn {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}

.slide-in {
  animation: slideInUp 0.8s ease-out;
}

@keyframes slideInUp {
  from { transform: translateY(30px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

@keyframes slideInFromTop {
  from { transform: translateY(-30px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

/* Responsive Design */
@media (max-width: 768px) {
  .modern-gradient {
    padding: 16px;
  }
  
  .duel-header-container {
    margin-bottom: 24px;
  }
  
  .crossed-swords {
    font-size: 3rem;
  }
  
  .room-option {
    padding: 24px;
  }
  
  .floating-orb {
    display: none;
  }
}

/* Utility Classes */
.bg-gradient-radial {
  background: radial-gradient(circle, var(--tw-gradient-from), var(--tw-gradient-to));
}

@keyframes pulse {
  0%, 100% { opacity: 0.8; }
  50% { opacity: 0.4; }
}
</style>
