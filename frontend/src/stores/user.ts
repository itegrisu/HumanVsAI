import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useUserStore = defineStore('user', () => {
  // State
  const userName = ref<string>('')
  const isNameSet = ref<boolean>(false)
  const userStats = ref({
    totalGames: 0,
    totalScore: 0,
    bestScore: 0,
    averageAccuracy: 0,
    timeRushBest: 0
  })

  // Actions
  const setUserName = (name: string) => {
    userName.value = name.trim()
    isNameSet.value = true
    // Local storage'a kaydet
    localStorage.setItem('aivshumanUserName', name.trim())
    localStorage.setItem('aivshumanNameSet', 'true')
  }

  const loadUserFromStorage = () => {
    const savedName = localStorage.getItem('aivshumanUserName')
    const nameSet = localStorage.getItem('aivshumanNameSet')
    
    if (savedName && nameSet === 'true') {
      userName.value = savedName
      isNameSet.value = true
    }

    // İstatistikleri yükle
    const savedStats = localStorage.getItem('aivshumanUserStats')
    if (savedStats) {
      userStats.value = JSON.parse(savedStats)
    }
  }

  const updateStats = (gameData: {
    score: number
    accuracy: number
    gameType: 'normal' | 'timeRush'
    timeRushScore?: number
  }) => {
    userStats.value.totalGames++
    userStats.value.totalScore += gameData.score
    
    if (gameData.score > userStats.value.bestScore) {
      userStats.value.bestScore = gameData.score
    }

    if (gameData.gameType === 'timeRush' && gameData.timeRushScore) {
      if (gameData.timeRushScore > userStats.value.timeRushBest) {
        userStats.value.timeRushBest = gameData.timeRushScore
      }
    }

    // Ortalama accuracy hesapla
    userStats.value.averageAccuracy = 
      ((userStats.value.averageAccuracy * (userStats.value.totalGames - 1)) + gameData.accuracy) / 
      userStats.value.totalGames

    // Local storage'a kaydet
    localStorage.setItem('aivshumanUserStats', JSON.stringify(userStats.value))
  }

  const resetUser = () => {
    userName.value = ''
    isNameSet.value = false
    userStats.value = {
      totalGames: 0,
      totalScore: 0,
      bestScore: 0,
      averageAccuracy: 0,
      timeRushBest: 0
    }
    localStorage.removeItem('aivshumanUserName')
    localStorage.removeItem('aivshumanNameSet')
    localStorage.removeItem('aivshumanUserStats')
  }

  return {
    userName,
    isNameSet,
    userStats,
    setUserName,
    loadUserFromStorage,
    updateStats,
    resetUser
  }
})
