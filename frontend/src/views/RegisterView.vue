<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api-service'
import { authService } from '@/services/auth-service'

// Form data
const name = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')
const isSubmitting = ref(false)

const router = useRouter()

const handleSubmit = async () => {
  // Reset error message
  errorMessage.value = ''

  // Form validation
  if (!name.value || !email.value || !password.value || !confirmPassword.value) {
    errorMessage.value = 'Please fill in all fields'
    return
  }

  // Basic email validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(email.value)) {
    errorMessage.value = 'Please enter a valid email address'
    return
  }

  // Password match validation
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match'
    return
  }

  // Password strength validation
  if (password.value.length < 8) {
    errorMessage.value = 'Password must be at least 8 characters long'
    return
  }

  try {
    isSubmitting.value = true

    // Call the backend API for registration
    const response = await api.register({
      name: name.value,
      email: email.value,
      password: password.value,
    })

    console.log('Registration successful:', response.data)

    // Log in after registration (using await to ensure sequence)
    const loginResponse = await api.login({
      email: email.value,
      password: password.value,
    })

    if (loginResponse.data.token) {
      authService.setToken(loginResponse.data.token)

      console.log('Login successful, redirecting to map...')
      await router.push('/map')
    } else {
      console.error('Login response did not contain a token')
      errorMessage.value = 'Auto-login failed after registration'
    }
  } catch (error: any) {
    console.error('Registration error:', error)
    errorMessage.value = error.response?.data || 'An error occurred during registration'
  } finally {
    isSubmitting.value = false
  }
}

const goToLogin = () => {
  router.push('/login')
}
</script>

<template>
  <div class="auth-container">
    <div class="auth-form">
      <h1 class="register-text">REGISTER</h1>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <input type="text" v-model="name" placeholder="Your Name" required />
        </div>

        <div class="form-group">
          <input type="email" v-model="email" placeholder="Your Email" required />
        </div>

        <div class="form-group">
          <input type="password" v-model="password" placeholder="Your Password" required />
        </div>

        <div class="form-group">
          <input type="password" v-model="confirmPassword" placeholder="Re-Enter Your Password" required />
        </div>

        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>

        <button type="submit" class="submit-button" :disabled="isSubmitting">
          {{ isSubmitting ? 'Registering...' : 'Register' }}
        </button>
      </form>

      <div class="auth-footer">
        <button type="button" class="text-button" @click="goToLogin">Already registered?</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  padding: 20px;
}

.register-text {
  font-family: Bayon, sans-serif;
  font-weight: 400;
  font-size: 64px;
  line-height: 0;
  letter-spacing: 0;
  color: #272727;

  margin-bottom: 42px;
}

.auth-form {
  width: 100%;
  /* padding: 20px; */
  background-color: #ffffff;
}

h1 {
  text-align: center;
  margin-bottom: 30px;
  font-weight: 500;
  color: #333;
  /* Dark text for headings regardless of theme */
}

.form-group {
  margin-bottom: 20px;
}

input {
  width: 100%;
  padding: 32px 24px;
  border: 5px solid #272727;
  border-radius: 30px;
  font-size: 16px;
  outline: none;
  transition: border-color 0.3s;
  background-color: #ffffff;
  color: #333;
}

input:focus {
  border-color: #069;
}

.text-button {
  font-family: Inter, sans-serif;
  font-weight: 500;
  font-size: 24px;
  line-height: 0;
  letter-spacing: 0;
  background: none;
  border: none;
  color: #9A999F;
  cursor: pointer;
  text-decoration: none;
  transition: color 0.3s;

  margin-top: 24px;
  margin-bottom: 42px;
}

.text-button:hover {
  color: #069;
  text-decoration: underline;
}

.submit-button {
  width: 100%;
  padding: 25px;
  background-color: #038C7A;
  /* Keep original color */
  color: white;
  border: none;
  border-radius: 16px;
  font-family: Inter, sans-serif;
  font-size: 24px;
  font-weight: 600;
  line-height: 20px;
  letter-spacing: -0.02em;
  cursor: pointer;
  transition: background-color 0.3s;
  margin-top: 15px;
}

.submit-button:hover {
  background-color: #00796b;
  /* Keep original hover color */
}

.submit-button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

.auth-footer {
  margin-top: 30px;
  text-align: center;
  font-size: 14px;
  color: #666;
}

.error-message {
  color: #f44336;
  font-size: 14px;
  text-align: center;
  margin: 10px 0;
  padding: 10px;
  background-color: #ffebee;
  border-radius: 4px;
}
</style>
