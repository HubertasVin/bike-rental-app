<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import { ref, onMounted, computed, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { authService } from '@/services/auth-service'

const isAuthenticated = ref(false)
const router = useRouter()
const route = useRoute()

// Check authentication status on component mount and when route changes
const checkAuth = () => {
  isAuthenticated.value = authService.isTokenValid()
}

onMounted(() => {
  checkAuth()
})

// Watch for route changes to update auth status
watch(
  () => route.path,
  () => {
    checkAuth()
  },
)

// Navigation links based on authentication status
const navLinks = computed(() => {
  const links = [
    { name: 'About', path: '/about' },
    { name: 'Map', path: '/map' },
    { name: 'API Test', path: '/api-test' },
  ]

  if (isAuthenticated.value) {
    links.push({ name: 'Auth Test', path: '/auth-test' }, { name: 'User', path: '/user' })
  } else {
    links.push({ name: 'Login', path: '/login' }, { name: 'Register', path: '/register' })
  }

  return links
})
</script>

<template>
  <div class="app-container">
    <nav class="navigation desktop-only">
      <RouterLink v-for="link in navLinks" :key="link.path" :to="link.path">
        {{ link.name }}
      </RouterLink>
    </nav>

    <main class="content">
      <RouterView />
    </main>
  </div>
</template>

<style>
/* Global styles */
:root {
  --primary-color: #4285f4;
  --secondary-color: #34a853;
  --accent-color: #ea4335;
  --text-color: #202124;
  --light-text: #5f6368;
  --background-color: #ffffff;
  --nav-background: #f8f9fa;
  --border-color: #dadce0;
  --hover-bg: rgba(66, 133, 244, 0.1);
  --active-bg: rgba(66, 133, 244, 0.2);
}

/* Reset and base styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family:
    'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif;
  color: var(--text-color);
  background-color: var(--background-color);
  line-height: 1.6;
}

#app {
  margin: 0;
  padding: 0;
}

h1,
h2,
h3,
h4,
h5,
h6 {
  color: var(--light-text);
  margin-bottom: 0.5rem;
}

a {
  text-decoration: none;
  color: var(--primary-color);
}
</style>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.navigation {
  background-color: var(--nav-background);
  border-bottom: 1px solid var(--border-color);
  padding: 0.75rem 1rem;
  display: flex;
  gap: 0.5rem;
  justify-content: flex-start;
  align-items: center;
  flex-wrap: wrap;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.desktop-only {
  display: none;
}

.content {
  flex: 1;
  padding: 0;
  max-width: 100%;
  margin: 0;
  width: 100%;
  height: 100vh;
  overflow: hidden;
}

nav a {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 4px;
  color: var(--text-color);
  font-weight: 500;
  transition: all 0.2s ease;
}

nav a:hover {
  background-color: var(--hover-bg);
}

nav a.router-link-active {
  color: var(--primary-color);
  background-color: var(--active-bg);
  font-weight: 600;
}

@media (min-width: 1024px) {
  .app-container {
    flex-direction: row;
  }

  .desktop-only {
    display: flex;
  }

  .navigation {
    width: 220px;
    flex-direction: column;
    gap: 0.5rem;
    border-right: 1px solid var(--border-color);
    border-bottom: none;
    padding: 1.5rem 1rem;
    height: 100vh;
    position: fixed;
    left: 0;
    top: 0;
  }

  .content {
    margin-left: 220px;
    /* padding: 2rem; */
    height: auto;
    overflow: visible;
  }

  nav a {
    width: 100%;
    padding: 0.75rem 1rem;
  }
}
</style>
