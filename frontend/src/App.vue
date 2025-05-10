<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <div class="app-container">
    <nav class="navigation">
      <RouterLink to="/about">About</RouterLink>
      <RouterLink to="/login">Login</RouterLink>
      <RouterLink to="/register">Register</RouterLink>
      <RouterLink to="/map">Map</RouterLink>
      <RouterLink to="/api-test">API Test</RouterLink>
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

.content {
  flex: 1;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
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
    margin-left: 220px; /* Match navigation width */
    padding: 2rem;
  }

  nav a {
    width: 100%;
    padding: 0.75rem 1rem;
  }
}
</style>
