import { defineStore } from 'pinia'

export interface LeaderboardEntry {
  id: string
  playerName: string
  score: number
  totalQuestions: number
  successRate: number
  timestamp: Date
  badge: string
  avatar?: string
  gameMode?: 'normal' | 'timeRush'
}

export const useLeaderboardStore = defineStore('leaderboard', {
  state: () => ({
    players: [
      // Mock data for demo
      {
        id: '1',
        playerName: 'AI Master 🤖',
        score: 10,
        totalQuestions: 10,
        successRate: 100,
        timestamp: new Date('2024-01-15'),
        badge: '🏆 Perfect Score',
        avatar: '🧠'
      },
      {
        id: '2',
        playerName: 'Vision Expert',
        score: 9,
        totalQuestions: 10,
        successRate: 90,
        timestamp: new Date('2024-01-14'),
        badge: '🌟 AI Detective',
        avatar: '👁️'
      },
      {
        id: '3',
        playerName: 'Tech Guru',
        score: 8,
        totalQuestions: 10,
        successRate: 80,
        timestamp: new Date('2024-01-13'),
        badge: '⭐ Super Guesser',
        avatar: '🚀'
      },
      {
        id: '4',
        playerName: 'Smart Player',
        score: 8,
        totalQuestions: 10,
        successRate: 80,
        timestamp: new Date('2024-01-12'),
        badge: '⭐ Super Guesser',
        avatar: '🎯'
      },
      {
        id: '5',
        playerName: 'Visual Analyst',
        score: 7,
        totalQuestions: 10,
        successRate: 70,
        timestamp: new Date('2024-01-11'),
        badge: '👍 Good Observer',
        avatar: '🔍'
      },
      {
        id: '6',
        playerName: 'Rookie Hunter',
        score: 6,
        totalQuestions: 10,
        successRate: 60,
        timestamp: new Date('2024-01-10'),
        badge: '🎮 Getting Better',
        avatar: '🎲'
      },
      {
        id: '7',
        playerName: 'Beginner Pro',
        score: 5,
        totalQuestions: 10,
        successRate: 50,
        timestamp: new Date('2024-01-09'),
        badge: '📈 Learning',
        avatar: '🌱'
      },
      {
        id: '8',
        playerName: 'First Timer',
        score: 4,
        totalQuestions: 10,
        successRate: 40,
        timestamp: new Date('2024-01-08'),
        badge: '🆕 Newbie',
        avatar: '👶'
      }
    ] as LeaderboardEntry[],
    currentPlayerId: null as string | null
  }),

  getters: {
    topPlayers: (state) => {
      return state.players
        .sort((a: LeaderboardEntry, b: LeaderboardEntry) => {
          // İlk olarak success rate'e göre sırala
          if (b.successRate !== a.successRate) {
            return b.successRate - a.successRate
          }
          // Eğer success rate aynıysa, skora göre sırala
          if (b.score !== a.score) {
            return b.score - a.score
          }
          // Eğer hepsi aynıysa, tarihe göre sırala (yeni olanlar üstte)
          return new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
        })
        .slice(0, 10) // Top 10
    },

    getPlayerRank: (state) => (playerId: string) => {
      const sortedEntries = state.players.sort((a: LeaderboardEntry, b: LeaderboardEntry) => {
        if (b.successRate !== a.successRate) {
          return b.successRate - a.successRate
        }
        if (b.score !== a.score) {
          return b.score - a.score
        }
        return new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()
      })
      
      return sortedEntries.findIndex((entry: LeaderboardEntry) => entry.id === playerId) + 1
    },

    averageScore: (state) => {
      if (state.players.length === 0) return 0
      const total = state.players.reduce((sum: number, entry: LeaderboardEntry) => sum + entry.successRate, 0)
      return Math.round(total / state.players.length)
    },

    totalGames: (state) => state.players.length,

    currentPlayerEntry: (state) => {
      return state.currentPlayerId 
        ? state.players.find((player: LeaderboardEntry) => player.id === state.currentPlayerId)
        : null
    }
  },

  actions: {
    addScore(scoreData: { playerName: string; score: number; totalQuestions: number }) {
      const { playerName, score, totalQuestions } = scoreData
      const successRate = Math.round((score / totalQuestions) * 100)
      const badge = this.getBadgeForScore(successRate)
      const avatar = this.getAvatarForScore(successRate)
      
      const newEntry: LeaderboardEntry = {
        id: Date.now().toString(),
        playerName: playerName || `Player${Math.floor(Math.random() * 1000)}`,
        score,
        totalQuestions,
        successRate,
        timestamp: new Date(),
        badge,
        avatar
      }

      this.players.unshift(newEntry) // Yeni gelen en üste
      this.currentPlayerId = newEntry.id

      // Sadece son 50 oyunu sakla
      if (this.players.length > 50) {
        this.players = this.players.slice(0, 50)
      }
    },

    getBadgeForScore(successRate: number): string {
      if (successRate >= 100) return '🏆 Perfect Score'
      if (successRate >= 90) return '🌟 AI Detective'
      if (successRate >= 80) return '⭐ Super Guesser'
      if (successRate >= 70) return '👍 Good Observer'
      if (successRate >= 60) return '🎮 Getting Better'
      if (successRate >= 50) return '📈 Learning'
      if (successRate >= 40) return '🆕 Newbie'
      return '💪 Keep Trying'
    },

    getAvatarForScore(successRate: number): string {
      if (successRate >= 100) return '🧠'
      if (successRate >= 90) return '👁️'
      if (successRate >= 80) return '🚀'
      if (successRate >= 70) return '🔍'
      if (successRate >= 60) return '🎯'
      if (successRate >= 50) return '🎲'
      if (successRate >= 40) return '🌱'
      return '👶'
    },

    clearCurrentPlayerEntry() {
      this.currentPlayerId = null
    }
  }
})
