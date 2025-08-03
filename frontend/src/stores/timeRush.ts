import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface TimeRushQuestion {
  id: string
  imageUrl: string
  isAI: boolean
  category: string
  difficulty: 'easy' | 'medium' | 'hard'
}

export interface TimeRushResult {
  score: number
  totalAnswered: number
  correctAnswers: number
  wrongAnswers: number
  timeUsed: number
  accuracy: number
  questionsPerSecond: number
}

export const useTimeRushStore = defineStore('timeRush', () => {
  // State
  const isActive = ref(false)
  const timeLeft = ref(30) // 30 saniye
  const totalTime = ref(30)
  const currentQuestionIndex = ref(0)
  const score = ref(0)
  const totalAnswered = ref(0)
  const correctAnswers = ref(0)
  const wrongAnswers = ref(0)
  const questions = ref<TimeRushQuestion[]>([])
  const currentQuestion = ref<TimeRushQuestion | null>(null)
  const gameInterval = ref<number | null>(null)

  // Computed
  const accuracy = computed(() => {
    if (totalAnswered.value === 0) return 0
    return Math.round((correctAnswers.value / totalAnswered.value) * 100)
  })

  const questionsPerSecond = computed(() => {
    const timeUsed = totalTime.value - timeLeft.value
    if (timeUsed === 0) return 0
    return Number((totalAnswered.value / timeUsed).toFixed(2))
  })

  const progress = computed(() => {
    return ((totalTime.value - timeLeft.value) / totalTime.value) * 100
  })

  // Mock questions for demo - Higher quality images
  const mockQuestions: TimeRushQuestion[] = [
    {
      id: '1',
      imageUrl: 'https://picsum.photos/600/450?random=1',
      isAI: true,
      category: 'portrait',
      difficulty: 'easy'
    },
    {
      id: '2',
      imageUrl: 'https://picsum.photos/600/450?random=2',
      isAI: false,
      category: 'landscape',
      difficulty: 'medium'
    },
    {
      id: '3',
      imageUrl: 'https://picsum.photos/600/450?random=3',
      isAI: true,
      category: 'object',
      difficulty: 'hard'
    },
    {
      id: '4',
      imageUrl: 'https://picsum.photos/600/450?random=4',
      isAI: false,
      category: 'portrait',
      difficulty: 'easy'
    },
    {
      id: '5',
      imageUrl: 'https://picsum.photos/600/450?random=5',
      isAI: true,
      category: 'landscape',
      difficulty: 'medium'
    },
    {
      id: '6',
      imageUrl: 'https://picsum.photos/600/450?random=6',
      isAI: false,
      category: 'object',
      difficulty: 'hard'
    },
    {
      id: '7',
      imageUrl: 'https://picsum.photos/600/450?random=7',
      isAI: true,
      category: 'portrait',
      difficulty: 'easy'
    },
    {
      id: '8',
      imageUrl: 'https://picsum.photos/600/450?random=8',
      isAI: false,
      category: 'landscape',
      difficulty: 'medium'
    },
    {
      id: '9',
      imageUrl: 'https://picsum.photos/600/450?random=9',
      isAI: true,
      category: 'object',
      difficulty: 'hard'
    },
    {
      id: '10',
      imageUrl: 'https://picsum.photos/600/450?random=10',
      isAI: false,
      category: 'portrait',
      difficulty: 'easy'
    }
  ]

  // Actions
  const startGame = (duration: number = 30) => {
    // Reset game state
    timeLeft.value = duration
    totalTime.value = duration
    currentQuestionIndex.value = 0
    score.value = 0
    totalAnswered.value = 0
    correctAnswers.value = 0
    wrongAnswers.value = 0
    isActive.value = true

    // Shuffle and prepare questions
    questions.value = [...mockQuestions].sort(() => Math.random() - 0.5)
    currentQuestion.value = questions.value[0]

    // Preload next few images for faster loading
    preloadImages()

    // Start timer
    gameInterval.value = setInterval(() => {
      if (timeLeft.value > 0) {
        timeLeft.value--
      } else {
        endGame()
      }
    }, 1000)
  }

  const preloadImages = () => {
    // Preload next 5 images
    for (let i = 1; i < Math.min(6, questions.value.length); i++) {
      const img = new Image()
      img.src = questions.value[i].imageUrl
    }
  }

  const answerQuestion = (guess: 'ai' | 'human'): { isCorrect: boolean; points: number } => {
    if (!isActive.value || !currentQuestion.value) {
      return { isCorrect: false, points: 0 }
    }

    const isCorrect = (guess === 'ai' && currentQuestion.value.isAI) || 
                     (guess === 'human' && !currentQuestion.value.isAI)

    totalAnswered.value++
    
    let points = 0
    if (isCorrect) {
      correctAnswers.value++
      // Difficulty based scoring
      switch (currentQuestion.value.difficulty) {
        case 'easy': points = 10; break
        case 'medium': points = 15; break
        case 'hard': points = 25; break
      }
      
      // Time bonus (more points for faster answers)
      const timeBonus = Math.floor(timeLeft.value / 5)
      points += timeBonus
      
      score.value += points
    } else {
      wrongAnswers.value++
    }

    // Load next question
    nextQuestion()

    return { isCorrect, points }
  }

  const nextQuestion = () => {
    if (currentQuestionIndex.value < questions.value.length - 1) {
      currentQuestionIndex.value++
      currentQuestion.value = questions.value[currentQuestionIndex.value]
      
      // Preload next image if available
      const nextIndex = currentQuestionIndex.value + 1
      if (nextIndex < questions.value.length) {
        const img = new Image()
        img.src = questions.value[nextIndex].imageUrl
      }
    } else {
      // Shuffle for more questions and continue
      const newQuestions = [...mockQuestions].sort(() => Math.random() - 0.5)
      questions.value = [...questions.value, ...newQuestions]
      currentQuestionIndex.value++
      currentQuestion.value = questions.value[currentQuestionIndex.value]
      
      // Preload a few more
      for (let i = 1; i < 4; i++) {
        const nextIndex = currentQuestionIndex.value + i
        if (nextIndex < questions.value.length) {
          const img = new Image()
          img.src = questions.value[nextIndex].imageUrl
        }
      }
    }
  }

  const endGame = (): TimeRushResult => {
    isActive.value = false
    
    if (gameInterval.value) {
      clearInterval(gameInterval.value)
      gameInterval.value = null
    }

    return {
      score: score.value,
      totalAnswered: totalAnswered.value,
      correctAnswers: correctAnswers.value,
      wrongAnswers: wrongAnswers.value,
      timeUsed: totalTime.value - timeLeft.value,
      accuracy: accuracy.value,
      questionsPerSecond: questionsPerSecond.value
    }
  }

  const pauseGame = () => {
    if (gameInterval.value) {
      clearInterval(gameInterval.value)
      gameInterval.value = null
    }
    isActive.value = false
  }

  const resumeGame = () => {
    if (!isActive.value && timeLeft.value > 0) {
      isActive.value = true
      gameInterval.value = setInterval(() => {
        if (timeLeft.value > 0) {
          timeLeft.value--
        } else {
          endGame()
        }
      }, 1000)
    }
  }

  const resetGame = () => {
    if (gameInterval.value) {
      clearInterval(gameInterval.value)
      gameInterval.value = null
    }
    
    isActive.value = false
    timeLeft.value = 30
    totalTime.value = 30
    currentQuestionIndex.value = 0
    score.value = 0
    totalAnswered.value = 0
    correctAnswers.value = 0
    wrongAnswers.value = 0
    currentQuestion.value = null
    questions.value = []
  }

  return {
    // State
    isActive,
    timeLeft,
    totalTime,
    currentQuestionIndex,
    score,
    totalAnswered,
    correctAnswers,
    wrongAnswers,
    currentQuestion,
    
    // Computed
    accuracy,
    questionsPerSecond,
    progress,
    
    // Actions
    startGame,
    answerQuestion,
    nextQuestion,
    endGame,
    pauseGame,
    resumeGame,
    resetGame
  }
})
