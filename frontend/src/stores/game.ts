import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface GameResults {
  correct: number
  wrong: number
  total: number
}

export const useGameStore = defineStore('game', () => {
  const results = ref<GameResults>({
    correct: 0,
    wrong: 0,
    total: 10
  })

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

  return {
    results,
    setGameResults,
    resetGame
  }
})
