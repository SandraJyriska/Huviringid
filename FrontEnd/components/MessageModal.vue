<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <h2 class="modal-title">Saada sõnum:</h2>
      
      <textarea 
        v-model="title"
        placeholder="Sõnumi pealkiri"
        :class="{ 'required-field': isMessageEmpty && isAttempted, 'small-textarea': true }"
        required
      ></textarea>
      
      <textarea 
        v-model="message" 
        placeholder="Sisesta sõnum" 
        :class="{ 'required-field': isMessageEmpty && isAttempted }"
        required
      ></textarea>

      <div class="buttons">
        <button class="send-button" @click="sendMessage">Saada sõnum</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useMessageStore } from '@/stores/studentmessagestore';
import { defineProps, defineEmits } from 'vue';
import { useTeacherStore } from '@/stores/teacherStore';

const props = defineProps<{
  selectedExtracurricularId: number;
  selectedStudentIds: number[];  
  teacherId: number | null | undefined;
}>();  

const emit = defineEmits(['close']);
const messageStore = useMessageStore();  
const teacherStore = useTeacherStore();
const message = ref('');
const title = ref('');
const teacher = ref<Teacher>();
const isAttempted = ref(false);
const isMessageEmpty = computed(() => !message.value.trim());

onMounted(async () => {
  if (props.teacherId !== null && props.teacherId !== undefined){
    teacher.value = await teacherStore.getTeacherById(props.teacherId);
  } 
});

const sendMessage = () => {
  isAttempted.value = true;

  if (!isMessageEmpty.value && props.teacherId !== null 
  && props.teacherId !== undefined && teacher.value !== undefined) {
    const messageData = {
      title: title.value,
      sender: teacher.value?.name, 
      content: message.value,  
      time: new Date().toISOString(),  
      teacherId: props.teacherId,  
      studentIds: props.selectedStudentIds,
      extracurricularId: props.selectedExtracurricularId,  
    };

    messageStore.sendMessage(messageData);  
    closeModal();  
  }
};

const closeModal = () => {
  emit('close');
  message.value = ''; 
  isAttempted.value = false;
};
</script>


  
<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 8px;
  width: 400px;
  box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
}

textarea {
  width: 100%;
  height: 100px;
  margin-bottom: 20px;
  padding: 5px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.small-textarea {
  height: 30px;
  font-size: 14px;
  resize: none;
  overflow: hidden;
}

.buttons {
  display: flex;
  justify-content: flex-start;
}

button {
  padding: 10px 15px;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  background-color: #A67F58;
  color: white;
  float: right;
}

button:hover {
  background-color: #A67F58;
  box-shadow: 0 8px 16px #4c3926;
  transform: scale(1.02);
}

.required-field {
  border: 2px solid rgb(210, 82, 82); 
  background-color: #ffebeb; 
}

.modal-title {
  font-size: 1.3rem;
  margin-bottom: 10px; 
  color: black;
  font-weight: bold;
}
</style>



  
  
  