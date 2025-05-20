<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '@/services/api-service'
import { authService } from '@/services/auth-service'

const router = useRouter()
const email = ref('')
const name = ref('')
const isLoading = ref(true)
const userId = ref('')
const newEmail = ref('')
const newName = ref('')
const newPassword = ref('')
const updateMessage = ref('')
const isEditing = ref(false)

onMounted(async () => {
  try {
    isLoading.value = true

    if (!authService.isTokenValid()) {
      console.log('User not authenticated, redirecting to login...')
      router.push('/login')
      return
    }

    email.value = authService.getUserEmail() || 'user@example.com'
    name.value = authService.getUserName() || 'Test User'
    newEmail.value = email.value
    newName.value = name.value
    userId.value = authService.getUserId() || ''
  } catch (error) {
    console.error('Error loading user profile:', error)
  } finally {
    isLoading.value = false
  }
})

const handleLogout = () => {
  try {
    isLoading.value = true
    authService.clearToken()
    router.push('/login')
    console.log('User logged out successfully')
  } catch (error) {
    console.error('Error during logout:', error)
  }
}

const updateProfile = async () => {
  try {
    isLoading.value = true
    updateMessage.value = ''

    const payload: any = {
      name: newName.value,
      email: newEmail.value,
    }

    if (newPassword.value) {
      payload.password = newPassword.value
    }

    await api.put(`/api/User/${userId.value}`, payload)

    name.value = newName.value
    email.value = newEmail.value
    newPassword.value = ''
    updateMessage.value = 'Profile updated successfully!'
    isEditing.value = false
  } catch (error: any) {
    console.error('Error updating profile:', error)

    if (error.response) {
      console.error('Status:', error.response.status)
      console.error('Response data:', error.response.data)
      updateMessage.value = `Error: ${error.response.data || 'Failed to update profile.'}`
    } else {
      updateMessage.value = 'Unexpected error occurred.'
    }
  } finally {
    isLoading.value = false
  }
}

const cancelEdit = () => {
  newName.value = name.value
  newEmail.value = email.value
  newPassword.value = ''
  updateMessage.value = ''
  isEditing.value = false
}
</script>

<template>
  <div class="user-profile">
    <div v-if="isLoading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Loading your profile...</p>
    </div>

    <div v-else class="profile-container">
      <h1>User Profile</h1>

      <div class="profile-card">
        <div class="avatar-placeholder">
          <span>{{ name.charAt(0).toUpperCase() }}</span>
        </div>

        <div class="user-details">
          <template v-if="!isEditing">
            <h2>{{ name }}</h2>
            <p><strong>Email:</strong> {{ email }}</p>
            <button class="edit-button" @click="isEditing = true">Edit Profile</button>
          </template>

          <template v-else>
            <input v-model="newName" placeholder="Name" />
            <input v-model="newEmail" placeholder="Email" />
            <input v-model="newPassword" type="password" placeholder="New Password" />
            <button @click="updateProfile" class="update-button">Save Changes</button>
            <button @click="cancelEdit" class="cancel-button">Cancel</button>
            <p class="update-message">{{ updateMessage }}</p>
          </template>
        </div>

        <div class="profile-actions">
          <button class="action-button" @click="handleLogout">Log Out</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.user-profile {
  padding: 20px;
  max-width: 600px;
  margin: 0 auto;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 300px;
}

.loading-spinner {
  border: 4px solid rgba(0, 0, 0, 0.1);
  border-radius: 50%;
  border-top: 4px solid #009688;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 10px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

.profile-container h1 {
  margin-bottom: 30px;
  text-align: center;
}

.profile-card {
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  padding: 30px;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.avatar-placeholder {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background-color: #009688;
  color: white;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 2rem;
  font-weight: bold;
  margin-bottom: 20px;
}

.user-details {
  text-align: center;
  margin-bottom: 30px;
  width: 100%;
}

.user-details p {
  margin: 5px 0;
  color: #555;
}


.user-details input {
  display: block;
  margin: 10px auto;
  padding: 10px;
  width: 80%;
  border: 1px solid #ccc;
  border-radius: 20px;
}

.edit-button {
  padding: 10px 20px;
  margin-top: 15px;
  border: none;
  background-color: #2196f3;
  color: white;
  border-radius: 30px;
  cursor: pointer;
}

.edit-button:hover {
  background-color: #1976d2;
}

.update-button {
  background-color: #4caf50;
  color: white;
  border: none;
  padding: 12px 24px;
  margin-top: 10px;
  border-radius: 30px;
  cursor: pointer;
  transition: background-color 0.3s;
}

.update-button:hover {
  background-color: #388e3c;
}

.cancel-button {
  background-color: #9e9e9e;
  color: white;
  border: none;
  padding: 12px 24px;
  margin-top: 10px;
  border-radius: 30px;
  cursor: pointer;
  margin-left: 10px;
}

.cancel-button:hover {
  background-color: #757575;
}

.update-message {
  margin-top: 10px;
  color: #333;
}

.profile-actions {
  width: 100%;
}

.action-button {
  width: 100%;
  padding: 15px;
  background-color: #f44336;
  color: white;
  border: none;
  border-radius: 30px;
  font-size: 16px;
  font-weight: 500;
  cursor: pointer;
  transition: background-color 0.3s;
  margin-top: 10px;
}

.action-button:hover {
  background-color: #d32f2f;
}
</style>