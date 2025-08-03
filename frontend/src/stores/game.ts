import { ref, computed } from 'vue'
import { defineStore } from 'pinia'

export interface Category {
  id: string
  name: string
  description: string
  imageCount: number
}

export interface GameStatistics {
  totalGames: number
  totalCorrectAnswers: number
  totalWrongAnswers: number
  averageScore: number
  averageTime: number
  categoryStats: CategoryStats[]
}

export interface CategoryStats {
  category: string
  totalAnswers: number
  correctAnswers: number
  accuracy: number
  averageTime: number
}

export interface LeaderboardEntry {
  playerName: string
  score: number
  accuracy: number
  averageTime: number
  gamesPlayed: number
  lastPlayed: string
}

export interface GameResults {
  correct: number
  wrong: number
  total: number
}

export const useGameStore = defineStore('game', () => {
  // Existing state
  const results = ref<GameResults>({
    correct: 0,
    wrong: 0,
    total: 10
  })

  // New state for dynamic backend
  const categories = ref<Category[]>([])
  const selectedCategories = ref<string[]>([])
  const selectedDifficulty = ref<string>('mixed')
  const gameStatistics = ref<GameStatistics | null>(null)
  const leaderboard = ref<LeaderboardEntry[]>([])
  const isLoading = ref(false)

  // API Base URL
  const API_BASE = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

  // Getters
  const isAllCategoriesSelected = computed(() => 
    selectedCategories.value.length === categories.value.length
  )

  const hasSelectedCategories = computed(() => 
    selectedCategories.value.length > 0
  )

  // Existing actions
  const setGameResults = (newResults: GameResults) => {
    results.value = newResults
  }

  const resetGame = () => {
    results.value = {
      correct: 0,
      wrong: 0,
      total: 10
    }
  }

  // New actions for dynamic backend
  const fetchCategories = async () => {
    try {
      isLoading.value = true
      const response = await fetch(`${API_BASE}/game/categories`)
      if (response.ok) {
        categories.value = await response.json()
      }
    } catch (error) {
      console.error('Error fetching categories:', error)
    } finally {
      isLoading.value = false
    }
  }

  const fetchGameStatistics = async () => {
    try {
      isLoading.value = true
      const response = await fetch(`${API_BASE}/game/statistics`)
      if (response.ok) {
        gameStatistics.value = await response.json()
      }
    } catch (error) {
      console.error('Error fetching statistics:', error)
    } finally {
      isLoading.value = false
    }
  }

  const fetchLeaderboard = async () => {
    try {
      isLoading.value = true
      const response = await fetch(`${API_BASE}/game/leaderboard`)
      if (response.ok) {
        leaderboard.value = await response.json()
      }
    } catch (error) {
      console.error('Error fetching leaderboard:', error)
    } finally {
      isLoading.value = false
    }
  }

  const toggleCategory = (categoryId: string) => {
    const index = selectedCategories.value.indexOf(categoryId)
    if (index > -1) {
      selectedCategories.value.splice(index, 1)
    } else {
      selectedCategories.value.push(categoryId)
    }
  }

  const selectAllCategories = () => {
    selectedCategories.value = categories.value.map(c => c.id)
  }

  const clearCategorySelection = () => {
    selectedCategories.value = []
  }

  const setDifficulty = (difficulty: string) => {
    selectedDifficulty.value = difficulty
  }

  return {
    // Existing state
    results,
    
    // New state
    categories,
    selectedCategories,
    selectedDifficulty,
    gameStatistics,
    leaderboard,
    isLoading,

    // Getters
    isAllCategoriesSelected,
    hasSelectedCategories,

    // Existing actions
    setGameResults,
    resetGame,

    // New actions
    fetchCategories,
    fetchGameStatistics,
    fetchLeaderboard,
    toggleCategory,
    selectAllCategories,
    clearCategorySelection,
    setDifficulty
  }
})
