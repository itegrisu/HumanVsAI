import { ref } from 'vue'

// Sound system using Web Audio API and synthesized sounds
export function useSound() {
  const isMuted = ref(false)
  const volume = ref(0.3)

  // Audio context for generating sounds
  let audioContext: AudioContext | null = null

  const getAudioContext = () => {
    if (!audioContext) {
      audioContext = new (window.AudioContext || (window as any).webkitAudioContext)()
    }
    return audioContext
  }

  // Synthesize a tone
  const playTone = (frequency: number, duration: number, type: OscillatorType = 'sine', volumeLevel: number = volume.value) => {
    if (isMuted.value) return

    try {
      const ctx = getAudioContext()
      const oscillator = ctx.createOscillator()
      const gainNode = ctx.createGain()

      oscillator.connect(gainNode)
      gainNode.connect(ctx.destination)

      oscillator.frequency.setValueAtTime(frequency, ctx.currentTime)
      oscillator.type = type

      gainNode.gain.setValueAtTime(0, ctx.currentTime)
      gainNode.gain.linearRampToValueAtTime(volumeLevel, ctx.currentTime + 0.01)
      gainNode.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + duration)

      oscillator.start(ctx.currentTime)
      oscillator.stop(ctx.currentTime + duration)
    } catch (error) {
      console.warn('Audio playback failed:', error)
    }
  }

  // Sound effects
  const sounds = {
    // UI Interactions
    click: () => playTone(800, 0.1, 'square', 0.2),
    hover: () => playTone(600, 0.05, 'sine', 0.1),
    
    // Game Actions
    correct: () => {
      // Happy ascending chord
      setTimeout(() => playTone(523, 0.15, 'sine'), 0) // C
      setTimeout(() => playTone(659, 0.15, 'sine'), 100) // E
      setTimeout(() => playTone(784, 0.2, 'sine'), 200) // G
    },
    
    wrong: () => {
      // Sad descending tone
      playTone(400, 0.3, 'sawtooth', 0.25)
      setTimeout(() => playTone(300, 0.3, 'sawtooth', 0.25), 150)
    },
    
    // Game Events
    nextQuestion: () => playTone(1000, 0.1, 'triangle', 0.15),
    
    gameStart: () => {
      // Energetic startup sound
      setTimeout(() => playTone(440, 0.1, 'sine'), 0)
      setTimeout(() => playTone(554, 0.1, 'sine'), 100)
      setTimeout(() => playTone(659, 0.15, 'sine'), 200)
    },
    
    gameComplete: () => {
      // Victory fanfare
      const notes = [523, 659, 784, 1047] // C, E, G, C
      notes.forEach((note, index) => {
        setTimeout(() => playTone(note, 0.2, 'sine'), index * 100)
      })
    },
    
    // Timer sounds
    tick: () => playTone(1200, 0.05, 'square', 0.1),
    timeWarning: () => playTone(800, 0.2, 'triangle', 0.3),
    timeUp: () => {
      // Urgent alarm
      for (let i = 0; i < 3; i++) {
        setTimeout(() => playTone(1000, 0.2, 'sawtooth', 0.4), i * 250)
      }
    },
    
    // Duel mode sounds
    duelStart: () => {
      // Epic duel beginning
      setTimeout(() => playTone(330, 0.2, 'sine'), 0) // E
      setTimeout(() => playTone(440, 0.2, 'sine'), 200) // A
      setTimeout(() => playTone(523, 0.3, 'sine'), 400) // C
    },
    
    duelWin: () => {
      // Triumph melody
      const melody = [659, 784, 880, 1047, 1175] // E, G, A, C, D
      melody.forEach((note, index) => {
        setTimeout(() => playTone(note, 0.15, 'sine'), index * 80)
      })
    },
    
    duelLose: () => {
      // Defeat sound
      setTimeout(() => playTone(330, 0.4, 'sawtooth'), 0)
      setTimeout(() => playTone(294, 0.4, 'sawtooth'), 200)
      setTimeout(() => playTone(247, 0.6, 'sawtooth'), 400)
    },
    
    // Special effects
    streak: (streakCount: number) => {
      // Streak sound gets higher with each streak
      const baseFreq = 800
      const freq = baseFreq + (streakCount * 100)
      playTone(freq, 0.15, 'sine', Math.min(0.4, 0.2 + streakCount * 0.05))
    },
    
    powerUp: () => {
      // Power up ascending
      for (let i = 0; i < 8; i++) {
        setTimeout(() => playTone(440 + i * 110, 0.08, 'square', 0.2), i * 50)
      }
    }
  }

  // Volume control
  const setVolume = (newVolume: number) => {
    volume.value = Math.max(0, Math.min(1, newVolume))
  }

  // Mute toggle
  const toggleMute = () => {
    isMuted.value = !isMuted.value
  }

  // Test sound
  const testSound = () => {
    sounds.correct()
  }

  return {
    sounds,
    isMuted,
    volume,
    setVolume,
    toggleMute,
    testSound
  }
}
