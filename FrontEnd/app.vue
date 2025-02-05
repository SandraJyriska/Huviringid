<template>
  <div v-if="isInitialized">
    <TheMenu />
    <div class="p-2">
      <NuxtPage />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useUserStore } from '~/stores/userStore';

const userStore = useUserStore();
const isInitialized = ref(false);

onMounted(async () => {
  await userStore.initializeUser();
  isInitialized.value = true; 
});
</script>

<style>
html, body {
  height: 100%;
  margin: 0;
  overflow: hidden;
}
</style>