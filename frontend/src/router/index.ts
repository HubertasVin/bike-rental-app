import { authService } from '@/services/auth-service'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/LoginView.vue'),
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
    },
    {
      path: '/map',
      name: 'map',
      component: () => import('../views/MapView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/user',
      name: 'user',
      component: () => import('../views/UserView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/api-test',
      name: 'api-test',
      component: () => import('../views/ApiTestView.vue'),
    },
    {
      path: '/auth-test',
      name: 'auth-test',
      component: () => import('../views/AuthTestView.vue'),
    },
  ],
})

// Navigation guard to check authentication for protected routes
router.beforeEach((to, from, next) => {
  const isAuthenticated = authService.isTokenValid()

  if (to.meta.requiresAuth && !isAuthenticated) {
    // Redirect to login page if route requires auth and user is not authenticated
    next({ name: 'login' })
  } else if ((to.name === 'login' || to.name === 'register') && isAuthenticated) {
    // Redirect to user profile if trying to access login/register while authenticated
    next({ name: 'user' })
  } else {
    // Otherwise proceed as normal
    next()
  }
})

export default router
