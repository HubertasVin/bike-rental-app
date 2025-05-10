import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

import './assets/main.css'

const app = createApp(App)

// Add global error handler
app.config.errorHandler = (err, instance, info) => {
  console.error('Vue Global Error:', err)
  console.info('Vue Error Additional Info:', info)
}

app.use(router)
app.mount('#app')
