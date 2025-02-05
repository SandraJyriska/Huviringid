<template>
  <div class="notifications-title">
    <h2 class="notpage-title">Teated</h2>
    <div v-for="(notification, index) in messages" :key="index" class="notification-item">
      <h3 class="text-lg font-bold">{{ notification.title }}</h3>
      <p>{{ notification.content }}</p>
      <p class="sender-text">{{ notification.sender }}</p>
      <p class = "time-text">{{ formatDate(notification.time) }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useMessageStore } from '~/stores/studentmessagestore';

const messageStore = useMessageStore();
const { messages } = storeToRefs(messageStore);
const userStore = useUserStore();
const studentStore = useStudentStore();
const student = ref<Student>();

const formatDate = (isoString: string) => {
  const date = new Date(isoString);
  return new Intl.DateTimeFormat('et-EE', {
    dateStyle: 'long',
    timeStyle: 'short',
  }).format(date);
};

onMounted(async () => {
  if (userStore.userId !== null)
    student.value = await studentStore.getStudentByUserId(userStore.userId);

  if (student.value !== undefined)
    await messageStore.loadStudentMessages(student.value.id); 
  });

</script>

<style scoped> 
.notifications-title {
  padding: 0.5rem;
  background-color: #ffffff;
  width: 250px;
  height: 100vh;
  min-height: 100vh;
}

.notification-item {
padding: 0.5rem 16px;
background-color: transparent;
margin: 13px 0;
width: 100%;
}

.notification-item:hover {
transform: scale(1.02);
box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.sender-text {
  font-size: 0.875rem;
  color: #666;
}

.time-text {
  font-size: 0.875rem;
  color: #666;
  font-style: italic;
}

.notpage-title {
font-size: 1.5rem;
color: #A67F58;
font-weight: 500;
font-family: sans-serif;
margin-top: -5px;
margin-left: 15px;
}
</style>


  
  
  