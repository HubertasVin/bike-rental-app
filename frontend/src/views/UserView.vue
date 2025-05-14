<script setup lang="ts">
    import { ref, onMounted } from 'vue'
    import { useRouter } from 'vue-router'
    import { api } from '@/services/api-service'

    const router = useRouter()
    const email = ref('')
    const isLoading = ref(true)

    onMounted(async () => {
        try {
            // In a real app, you would fetch user data from an API
            // For now, we'll simulate this with localStorage
            isLoading.value = true

            // Simulate API call with timeout
            await new Promise(resolve => setTimeout(resolve, 500))

            // Check if user is authenticated
            const isAuthenticated = localStorage.getItem('user_authenticated') === 'true'
            if (!isAuthenticated) {
                // Redirect to login if not authenticated
                router.push('/login')
                return
            }

            // Get stored email from localStorage (in a real app, you'd get this from an API)
            email.value = localStorage.getItem('user_email') || 'user@example.com'
        } catch (error) {
            console.error('Error loading user profile:', error)
        } finally {
            isLoading.value = false
        }
    })

    const handleLogout = async () => {
        try {
            isLoading.value = true

            // Call logout API
            await api.logout()

            // Redirect to login page
            router.push('/login')
        } catch (error) {
            console.error('Error during logout:', error)
        }
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
                    <span>{{ email.charAt(0).toUpperCase() }}</span>
                </div>

                <div class="user-details">
                    <h2>{{ email }}</h2>
                    <p>Account Member</p>
                </div>

                <div class="profile-actions">
                    <button class="action-button" @click="handleLogout">
                        Log Out
                    </button>
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
    }

        .user-details h2 {
            margin-bottom: 5px;
            color: #333;
        }

        .user-details p {
            color: #666;
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