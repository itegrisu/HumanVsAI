import { createRouter, createWebHashHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import GameView from '../views/GameView.vue'
import ResultView from '../views/ResultView.vue'

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/game',
      name: 'game',
      component: GameView
    },
    {
      path: '/result',
      name: 'result',
      component: ResultView
    },
    {
      path: '/leaderboard',
      name: 'leaderboard',
      component: () => import('../views/LeaderboardView.vue')
    },
    {
      path: '/time-rush',
      name: 'timeRush',
      component: () => import('../views/TimeRushView.vue')
    },
    {
      path: '/duel',
      name: 'duel',
      component: () => import('../views/DuelViewRealTime.vue')
    },
    {
      path: '/duel/join/:roomId',
      name: 'duelJoin',
      component: () => import('../views/DuelViewRealTime.vue')
    },
    {
      path: '/time-rush/result/:score/:answered/:correct/:accuracy/:qps',
      name: 'timeRushResult',
      component: () => import('../views/TimeRushResultView.vue')
    }
  ]
})

export default router
