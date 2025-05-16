<script setup lang="ts">import { ref, onMounted } from 'vue';
import { api } from '@/services/api-service';

const authStatus = ref({
    token: null as string | null,
    isPresent: false,
    isValid: false
});

const authHeaderTest = ref({
    loading: false,
    success: false,
    error: null as string | null,
    response: null as any
});

// Check if auth token exists
onMounted(() => {
    const token = localStorage.getItem('auth_token');
    authStatus.value.token = token;
    authStatus.value.isPresent = !!token;
});

// Test API call with auth header
const testAuthenticatedRequest = async () => {
    authHeaderTest.value.loading = true;
    authHeaderTest.value.error = null;

    try {
        // Try to get bikes - this endpoint requires authentication
        // This will automatically use the token from localStorage via the interceptor
        const response = await api.fetch('/api/bike');

        authHeaderTest.value.success = true;
        authHeaderTest.value.response = response.data;
        authStatus.value.isValid = true;
    } catch (error: any) {
        authHeaderTest.value.success = false;
        authHeaderTest.value.error = error.response?.data || error.message;
        authStatus.value.isValid = false;
    } finally {
        authHeaderTest.value.loading = false;
    }
};

// Check token from local storage
const displayToken = (token: string | null): string => {
    if (!token) return 'No token found';
    if (token.length <= 15) return token;
    // Show the first and last few characters of the token
    return `${token.substring(0, 6)}...${token.substring(token.length - 6)}`;
};</script>

<template>
    <div class="auth-test-container">
        <h1>Authentication Verification</h1>

        <section class="test-section">
            <h2>Authentication Token Status</h2>
            <div class="status-card">
                <div class="status-item">
                    <strong>Token Present:</strong>
                    <span :class="authStatus.isPresent ? 'status-success' : 'status-error'">
                        {{ authStatus.isPresent ? 'Yes' : 'No' }}
                    </span>
                </div>

                <div class="status-item">
                    <strong>Token Value:</strong>
                    <code>{{ displayToken(authStatus.token) }}</code>
                </div>

                <div class="status-item" v-if="authHeaderTest.loading || authHeaderTest.success !== null">
                    <strong>Token Valid:</strong>
                    <span v-if="authHeaderTest.loading">Checking...</span>
                    <span v-else :class="authStatus.isValid ? 'status-success' : 'status-error'">
                        {{ authStatus.isValid ? 'Yes' : 'No' }}
                    </span>
                </div>
            </div>
        </section>

        <section class="test-section">
            <h2>Test Authenticated API Request</h2>
            <p>This will send a request to the API with the Authorization header</p>

            <button @click="testAuthenticatedRequest"
                    :disabled="authHeaderTest.loading"
                    class="test-button">
                {{ authHeaderTest.loading ? 'Testing...' : 'Test Authentication' }}
            </button>

            <div v-if="authHeaderTest.error" class="error-message">
                <h3>Error:</h3>
                <pre>{{ authHeaderTest.error }}</pre>

                <div class="help-tips">
                    <p v-if="!authStatus.isPresent">
                        <strong>Tip:</strong> You need to login first to get an authentication token.
                    </p>
                    <p v-else>
                        <strong>Tip:</strong> Your token might be invalid or expired. Try logging in again.
                    </p>
                </div>
            </div>

            <div v-if="authHeaderTest.success" class="success-message">
                <h3>Success! Your token is valid.</h3>
                <p>The API responded with:</p>
                <pre>{{ JSON.stringify(authHeaderTest.response, null, 2) }}</pre>
            </div>
        </section>
    </div>
</template>

<style scoped>
    .auth-test-container {
        max-width: 800px;
        margin: 0 auto;
        padding: 20px;
    }

    .test-section {
        background-color: white;
        border-radius: 8px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        padding: 20px;
        margin-bottom: 20px;
    }

    .status-card {
        background-color: #f5f5f5;
        border-radius: 4px;
        padding: 15px;
        margin: 15px 0;
    }

    .status-item {
        margin-bottom: 10px;
        display: flex;
        justify-content: space-between;
    }

    .status-success {
        color: #4caf50;
        font-weight: bold;
    }

    .status-error {
        color: #f44336;
        font-weight: bold;
    }

    .test-button {
        background-color: #2196f3;
        color: white;
        border: none;
        padding: 10px 15px;
        border-radius: 4px;
        cursor: pointer;
        font-size: 16px;
        margin: 15px 0;
    }

        .test-button:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }

    .error-message {
        margin-top: 15px;
        padding: 15px;
        background-color: #ffebee;
        border-left: 4px solid #f44336;
        border-radius: 4px;
    }

    .success-message {
        margin-top: 15px;
        padding: 15px;
        background-color: #e8f5e9;
        border-left: 4px solid #4caf50;
        border-radius: 4px;
    }

    pre {
        background-color: #f5f5f5;
        padding: 10px;
        border-radius: 4px;
        overflow-x: auto;
        max-height: 300px;
    }

    .help-tips {
        margin-top: 15px;
        padding: 10px;
        background-color: #fff8e1;
        border-radius: 4px;
    }

    code {
        font-family: monospace;
        background-color: #e0e0e0;
        padding: 2px 5px;
        border-radius: 3px;
        word-break: break-all;
    }
</style>