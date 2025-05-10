<script setup lang="ts">
import { ref } from 'vue'
import { api } from '@/services/api-service'

const helloWorldResponse = ref('')
const weatherForecastResponse = ref([])
const isHelloWorldLoading = ref(false)
const isWeatherForecastLoading = ref(false)
const errorMessage = ref('')
const corsStatus = ref('Not tested')

const testHelloWorld = async () => {
  isHelloWorldLoading.value = true
  errorMessage.value = ''
  try {
    console.log('Attempting to fetch from: http://localhost:5270/api/test')
    const response = await api.getHelloWorld()
    helloWorldResponse.value = response.data
    corsStatus.value = 'Working'
  } catch (error: any) {
    corsStatus.value = 'Failed - Error'
    errorMessage.value = `Error fetching Hello World: ${error.response?.data || error.message || error}`
    console.error('Hello World API Error:', error)
  } finally {
    isHelloWorldLoading.value = false
  }
}

const testWeatherForecast = async () => {
  isWeatherForecastLoading.value = true
  errorMessage.value = ''
  try {
    console.log('Fetching weather from: http://localhost:5270/weatherforecast')
    const response = await api.getWeatherForecast()
    weatherForecastResponse.value = response.data
    corsStatus.value = 'Working'
  } catch (error: any) {
    corsStatus.value = 'Failed - Error'
    errorMessage.value = `Error fetching Weather Forecast: ${error.response?.data || error.message || error}`
    console.error('Weather Forecast API Error:', error)
  } finally {
    isWeatherForecastLoading.value = false
  }
}
</script>

<template>
  <div class="api-test">
    <h1>API Test Page</h1>

    <div class="config-section">
      <h2>API Configuration</h2>
      <div class="cors-status"><strong>API Server:</strong> http://localhost:5270</div>
      <div class="cors-status">
        <strong>CORS Status:</strong>
        <span
          :class="{
            'status-ok': corsStatus === 'Working',
            'status-error': corsStatus.includes('Failed'),
          }"
        >
          {{ corsStatus }}
        </span>
      </div>
    </div>

    <div class="test-section">
      <h2>Test Hello World API</h2>
      <button @click="testHelloWorld" :disabled="isHelloWorldLoading">
        {{ isHelloWorldLoading ? 'Loading...' : 'Test Hello World API' }}
      </button>
      <div v-if="helloWorldResponse" class="response-display">
        <h3>Response:</h3>
        <pre>{{ helloWorldResponse }}</pre>
      </div>
    </div>

    <div class="test-section">
      <h2>Test Weather Forecast API</h2>
      <button @click="testWeatherForecast" :disabled="isWeatherForecastLoading">
        {{ isWeatherForecastLoading ? 'Loading...' : 'Test Weather Forecast API' }}
      </button>
      <div v-if="weatherForecastResponse.length > 0" class="response-display">
        <h3>Response:</h3>
        <pre>{{ JSON.stringify(weatherForecastResponse, null, 2) }}</pre>
      </div>
    </div>
  </div>
</template>

<style scoped>
.api-test {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  color: #333; /* Adding base text color */
}

.test-section,
.config-section {
  margin-bottom: 30px;
  border: 1px solid #ccc;
  padding: 20px;
  border-radius: 8px;
  background-color: white; /* Ensuring white background */
  color: #333; /* Explicit text color */
}

h1,
h2,
h3 {
  color: #222; /* Darker text for headings */
}

button {
  background-color: #4caf50;
  color: white;
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.response-display {
  margin-top: 20px;
  padding: 10px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  color: #000;
  border-radius: 4px;
  overflow-x: auto;
}

pre {
  white-space: pre-wrap;
  word-wrap: break-word;
}

.error-message {
  margin-top: 20px;
  padding: 10px;
  background-color: #ffebee;
  border: 1px solid #ffcdd2;
  color: #b71c1c;
  border-radius: 4px;
}

.troubleshooting {
  margin-top: 15px;
  padding: 10px;
  background-color: #fff8e1;
  border-radius: 4px;
}

.input-group {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.input-group label {
  width: 120px;
  margin-right: 10px;
}

.input-group input {
  flex-grow: 1;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.cors-status {
  padding: 10px;
  border-radius: 4px;
  background-color: #f5f5f5;
  color: #000;
}

.status-ok {
  color: #4caf50;
  font-weight: bold;
}

.status-error {
  color: #f44336;
  font-weight: bold;
}

.cors-code {
  background-color: #f5f5f5;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-family: monospace;
  color: #000;
}
</style>
