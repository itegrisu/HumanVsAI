import { ref, computed } from 'vue'
import { defineStore } from 'pinia'
import * as signalR from '@microsoft/signalr'

export interface Player {
  name: string
  isReady: boolean
  hasAnswered: boolean
  lastAnswer: string | null
  lastAnswerTime: number
  score: number
  correctAnswers: number
  wrongAnswers: number
  averageTime: number
}

export interface Question {
  id: string
  imageUrl: string
  difficulty: string
  category: string
}

export interface Room {
  id: string
  name: string
  status: 'WaitingForPlayer' | 'BothPlayersJoined' | 'InProgress' | 'Finished'
  player1: Player | null
  player2: Player | null
  currentRound: number
  totalRounds: number
  currentQuestion: Question | null
  timeLimit: number
  currentTime: number
}

export const useDuelStore = defineStore('duel', () => {
  // State
  const connection = ref<signalR.HubConnection | null>(null)
  const currentRoom = ref<Room | null>(null)
  const playerName = ref('')
  const roundTimer = ref<number | null>(null)

  // Getters
  const isConnected = computed(() => connection.value?.state === signalR.HubConnectionState.Connected)
  const isPlayerReady = computed(() => {
    if (!currentRoom.value || !playerName.value) return false
    const player = currentRoom.value.player1?.name === playerName.value ? 
                   currentRoom.value.player1 : currentRoom.value.player2
    return player?.isReady || false
  })
  const hasPlayerAnswered = computed(() => {
    if (!currentRoom.value || !playerName.value) return false
    const player = currentRoom.value.player1?.name === playerName.value ? 
                   currentRoom.value.player1 : currentRoom.value.player2
    return player?.hasAnswered || false
  })
  const currentPlayer = computed(() => {
    if (!currentRoom.value || !playerName.value) return null
    return currentRoom.value.player1?.name === playerName.value ? 
           currentRoom.value.player1 : currentRoom.value.player2
  })
  const opponent = computed(() => {
    if (!currentRoom.value || !playerName.value) return null
    return currentRoom.value.player1?.name === playerName.value ? 
           currentRoom.value.player2 : currentRoom.value.player1
  })
  const canStartGame = computed(() => {
    return currentRoom.value?.player1?.isReady && currentRoom.value?.player2?.isReady
  })

  // Additional getters for compatibility
  const isInRoom = computed(() => currentRoom.value !== null)
  const bothPlayersReady = computed(() => 
    currentRoom.value?.player1?.isReady && currentRoom.value?.player2?.isReady
  )
  const isGameInProgress = computed(() => currentRoom.value?.status === 'InProgress')
  const roomId = computed(() => currentRoom.value?.id || '')
  const shareLink = computed(() => `${window.location.origin}/#/duel?join=${roomId.value}`)
  const connectionError = ref('')

  // Actions
  const initializeConnection = async () => {
    if (connection.value) {
      await connection.value.stop()
    }

    // SignalR Hub URL
    const hubUrl = import.meta.env.VITE_API_BASE_URL?.replace('/api', '') || 'http://localhost:5000'
    
    connection.value = new signalR.HubConnectionBuilder()
      .withUrl(`${hubUrl}/duelhub`)
      .build()

    setupEventHandlers()

    try {
      await connection.value.start()
      console.log('SignalR Connected')
      connectionError.value = ''
    } catch (err) {
      console.error('SignalR Connection Error: ', err)
      connectionError.value = 'Sunucuya bağlanılamadı. Lütfen daha sonra tekrar deneyin.'
      throw err
    }
  }

  const setupEventHandlers = () => {
    if (!connection.value) return

    connection.value.on('RoomJoined', (data: { room: Room, playerName: string }) => {
      currentRoom.value = data.room
      playerName.value = data.playerName
    })

    connection.value.on('PlayerJoined', (data: { room: Room }) => {
      currentRoom.value = data.room
    })

    connection.value.on('PlayerReady', (data: { room: Room }) => {
      currentRoom.value = data.room
    })

    connection.value.on('GameStarted', (data: { room: Room }) => {
      currentRoom.value = data.room
    })

    connection.value.on('QuestionReceived', (data: { question: Question, round: number, timeLimit: number }) => {
      console.log('QuestionReceived:', data)
      console.log('TimeLimit received:', data.timeLimit)
      if (currentRoom.value) {
        currentRoom.value.currentRound = data.round
        currentRoom.value.currentQuestion = data.question
        currentRoom.value.timeLimit = data.timeLimit
        currentRoom.value.currentTime = data.timeLimit
        console.log('Current time set to:', currentRoom.value.currentTime)
        console.log('Room state before timer start:', JSON.stringify({
          currentTime: currentRoom.value.currentTime,
          timeLimit: currentRoom.value.timeLimit,
          round: currentRoom.value.currentRound
        }))

        // Reset round state for all players
        if (currentRoom.value.player1) {
          currentRoom.value.player1.hasAnswered = false
          currentRoom.value.player1.lastAnswer = null
        }
        if (currentRoom.value.player2) {
          currentRoom.value.player2.hasAnswered = false
          currentRoom.value.player2.lastAnswer = null
        }
        
        // Timer'ı başlat
        console.log('Starting timer...')
        startRoundTimer()
      }
    })

    connection.value.on('RoundStarted', (data: { 
      round: number, 
      totalRounds: number, 
      question: Question, 
      timeLimit: number 
    }) => {
      if (currentRoom.value) {
        currentRoom.value.status = 'InProgress'
        currentRoom.value.currentRound = data.round
        currentRoom.value.totalRounds = data.totalRounds
        currentRoom.value.currentQuestion = data.question
        currentRoom.value.timeLimit = data.timeLimit
        currentRoom.value.currentTime = data.timeLimit

        // Reset round state for all players
        if (currentRoom.value.player1) {
          currentRoom.value.player1.hasAnswered = false
          currentRoom.value.player1.lastAnswer = null
          currentRoom.value.player1.lastAnswerTime = 0
        }
        if (currentRoom.value.player2) {
          currentRoom.value.player2.hasAnswered = false
          currentRoom.value.player2.lastAnswer = null
          currentRoom.value.player2.lastAnswerTime = 0
        }

        // Start countdown timer
        startRoundTimer()
      }
    })

    connection.value.on('PlayerAnswered', (data: {
      playerId: string,
      playerName: string,
      answer: string,
      isCorrect: boolean,
      score: number,
      room: Room
    }) => {
      console.log('PlayerAnswered:', data)
      if (currentRoom.value) {
        // Sadece player bilgilerini güncelle, timer'ı etkilemeyelim
        if (currentRoom.value.player1?.name === data.playerName) {
          if (currentRoom.value.player1) {
            currentRoom.value.player1.hasAnswered = true
            currentRoom.value.player1.lastAnswer = data.answer
            currentRoom.value.player1.score = data.score
          }
        } else if (currentRoom.value.player2?.name === data.playerName) {
          if (currentRoom.value.player2) {
            currentRoom.value.player2.hasAnswered = true
            currentRoom.value.player2.lastAnswer = data.answer
            currentRoom.value.player2.score = data.score
          }
        }
        
        // UI feedback
        console.log(`${data.playerName} answered ${data.answer} - ${data.isCorrect ? 'Correct' : 'Wrong'}`)
      }
    })

    connection.value.on('RoundEnded', (data: {
      player1: { score: number },
      player2: { score: number }
    }) => {
      if (currentRoom.value) {
        // Update player stats
        if (currentRoom.value.player1) {
          currentRoom.value.player1.score = data.player1.score
        }
        if (currentRoom.value.player2) {
          currentRoom.value.player2.score = data.player2.score
        }
        
        stopRoundTimer()
      }
    })

    connection.value.on('GameFinished', (data: { room: Room }) => {
      console.log('GameFinished event received:', data)
      console.log('Room status:', data.room.status)
      if (currentRoom.value) {
        console.log('Updating room to finished state')
        currentRoom.value = data.room
        stopRoundTimer()
        console.log('Timer stopped, room status now:', currentRoom.value.status)
      }
    })

    connection.value.on('PlayerLeft', (data: { room: Room }) => {
      currentRoom.value = data.room
    })

    connection.value.on('RoomClosed', () => {
      currentRoom.value = null
      stopRoundTimer()
    })

    connection.value.on('Error', (error: string) => {
      console.error('Hub Error:', error)
    })
  }

  const startRoundTimer = () => {
    console.log('startRoundTimer called')
    if (roundTimer.value) {
      console.log('Clearing existing timer')
      clearInterval(roundTimer.value)
      roundTimer.value = null
    }

    if (!currentRoom.value) {
      console.log('No current room, cannot start timer')
      return
    }
    
    // Use a local variable for the countdown to avoid reactivity issues inside the interval.
    let timeLeft = currentRoom.value.currentTime
    console.log('Starting timer with local timeLeft:', timeLeft)

    roundTimer.value = setInterval(() => {
      if (timeLeft > 0) {
        timeLeft--
        // Update the reactive property
        if(currentRoom.value) {
            currentRoom.value.currentTime = timeLeft
        }
        console.log('Timer tick:', timeLeft)
      } else {
        console.log('Timer finished.')
        stopRoundTimer()
      }
    }, 1000)
  }

  const stopRoundTimer = () => {
    if (roundTimer.value) {
      clearInterval(roundTimer.value)
      roundTimer.value = null
    }
  }

  const createRoom = async (roomName: string, playerName: string) => {
    if (connection.value) {
      try {
        console.log('Creating room:', { roomName, playerName })
        await connection.value.invoke('CreateRoom', roomName, playerName)
        console.log('Room creation request sent')
      } catch (err) {
        console.error('Error creating room:', err)
      }
    } else {
      console.error('No connection available')
    }
  }

  const joinRoom = async (roomCode: string, playerName: string) => {
    if (connection.value) {
      try {
        await connection.value.invoke('JoinRoom', roomCode, playerName)
      } catch (err) {
        console.error('Error joining room:', err)
      }
    }
  }

  const setReady = async () => {
    if (connection.value && currentRoom.value) {
      try {
        await connection.value.invoke('SetReady', currentRoom.value.id)
      } catch (err) {
        console.error('Error setting ready:', err)
      }
    }
  }

  const startGame = async () => {
    if (connection.value && currentRoom.value) {
      try {
        await connection.value.invoke('StartGame', currentRoom.value.id)
      } catch (err) {
        console.error('Error starting game:', err)
      }
    }
  }

  const submitAnswer = async (answer: string) => {
    if (connection.value && currentRoom.value) {
      try {
        await connection.value.invoke('SubmitAnswer', currentRoom.value.id, answer)
      } catch (err) {
        console.error('Error submitting answer:', err)
      }
    }
  }

  const leaveRoom = async () => {
    if (connection.value && currentRoom.value) {
      try {
        await connection.value.invoke('LeaveRoom', currentRoom.value.id)
        currentRoom.value = null
        playerName.value = ''
        stopRoundTimer()
      } catch (err) {
        console.error('Error leaving room:', err)
      }
    }
  }

  const disconnect = async () => {
    if (connection.value) {
      try {
        await connection.value.stop()
        currentRoom.value = null
        playerName.value = ''
        stopRoundTimer()
      } catch (err) {
        console.error('Error disconnecting:', err)
      }
    }
  }

  const setConnectionError = (error: string) => {
    connectionError.value = error
  }

  const clearConnectionError = () => {
    connectionError.value = ''
  }

  const copyShareLink = async (): Promise<boolean> => {
    try {
      if (!currentRoom.value) return false
      
      const shareUrl = `${window.location.origin}/#/duel?join=${currentRoom.value.id}`
      await navigator.clipboard.writeText(shareUrl)
      return true
    } catch (err) {
      console.error('Error copying link:', err)
      return false
    }
  }

  return {
    // State
    connection,
    currentRoom,
    playerName,
    connectionError,
    
    // Getters
    isConnected,
    isPlayerReady,
    hasPlayerAnswered,
    currentPlayer,
    opponent,
    canStartGame,
    isInRoom,
    bothPlayersReady,
    isGameInProgress,
    roomId,
    shareLink,
    
    // Actions
    initializeConnection,
    createRoom,
    joinRoom,
    setReady,
    startGame,
    submitAnswer,
    leaveRoom,
    disconnect,
    setConnectionError,
    clearConnectionError,
    copyShareLink
  }
})
