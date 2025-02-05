<template>
  <div class="login-page">
    <div class="left-side"></div>
    <div class="right-side">
      <div class="login-box">
        <h1 class="login-title">Tere tulemast!</h1>
        <UForm :state="user" class="space-y-4" @submit.prevent="submit">
          <UFormGroup label="Kasutajanimi" name="user">
            <UInput v-model="user.username" placeholder="Sisestage kasutajanimi" />
          </UFormGroup>

          <UFormGroup label="Salasõna" name="password">
            <UInput v-model="user.password" type="password" placeholder="Sisestage salasõna" />
          </UFormGroup>

          <div v-if="isError" class="error-message">
            Vale kasutajanimi või salasõna. Palun proovige uuesti.
          </div>

          <UButton type="submit" class="login-button">Logi sisse</UButton>

          <div class="text-center mt-4">
            <span>Pole veel kasutajat? </span>
            <button
              @click.prevent="navigateToRegister"
              style="color: #8B4513;" 
              class="hover:text-black underline focus:outline-none">
              Registreeri
            </button>
          </div>
        </UForm>

        <div v-if="isLoading" class="loading-overlay">
          <div class="spinner"></div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '~/composables/useAuth';

const auth = useAuth();
const user: User = { username: '', password: '' };
const isError = ref(false);
const userStore = useUserStore();
const router = useRouter();
const isLoading = ref(false);

const submit = async () => {
  if (!user.username || !user.password) {
    isError.value = true;
    return;
  }
  isLoading.value = true;
  const loginSuccess = await auth.logIn(user);
  isError.value = !loginSuccess;

  if (loginSuccess) {
    await userStore.initializeUser();
    if (userStore.role === 'Student') {
      await navigateTo('/upcoming-lessons');
    } else if (userStore.role === 'Teacher') {
      await navigateTo('/registered-student-list');
    }
  }
  isLoading.value = false;
};

const navigateToRegister = () => {
  isError.value = false;
  router.push('/register-user-form');
};
</script>

<style scoped>

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: rgba(0, 0, 0, 0.5); /* Semi-transparent background */
  z-index: 1000; /* On top of everything */
}

.spinner {
  width: 50px;
  height: 50px;
  border: 5px solid rgba(255, 255, 255, 0.3);
  border-top: 5px solid white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.login-page {
  display: flex;
  height: 100vh;
  margin: 0; 
  padding: 0;
}

.left-side {
  flex: 1;
  background-image: url('https://img.freepik.com/free-vector/beautiful-illustration-autumn-children-hobbies_52683-71558.jpg?t=st=1734257785~exp=1734261385~hmac=22ebac7a1b1e035ed1d02982347bf570589fac8e7b8a62f783ea6161e206899f&w=1800');
  background-size: cover;
  background-position: center;
}

.right-side {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f9fafb;
}

.login-box {
  background: white;
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
  text-align: center;
  position: relative;
  box-shadow: 
    0 0 0 2px rgba(210, 180, 140, 0.71),
    0 0 0 6px rgba(222, 184, 135, 0.3),
    0 0 0 10px rgba(139, 69, 19, 0.2);
}

.login-title {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  color: #333;
}

.error-message {
  color: #e53e3e;
  font-size: 0.9rem;
  margin-top: -0.5rem;
}

.login-button {
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #E6CCB2;
  color: black;
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s;
}

.login-button:hover {
  background-color: #D2B48C;
}
</style>
  