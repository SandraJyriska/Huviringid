<template>
    <div class="modal-overlay" @click.self="closeModal"> <!-- Aken sulgub, kui klõpsatakse väljaspool -->
      <div class="modal-content">
        <h2>Põhjendus:</h2>
        <p v-if="note && note.message">{{ note.message }}</p>
        <p v-else>Põhjus lisamata.</p>
        <p v-if="note?.teacherId !== null">Puudumise lisas: {{ teacher?.name }}</p>

          <button class="close-button" @click="closeModal">Sulge</button>
          <button class="delete-button" @click="deleteNote">Kustuta</button>
      </div>
    </div>
 </template>
  
  <script setup lang="ts">
  import { defineProps, defineEmits } from 'vue';
  import type { Note } from "~/types/note";
  import { useUserStore } from '~/stores/userStore';

  const userStore = useUserStore();
  const teacherStore = useTeacherStore();
  const teacher = ref<Teacher>();
  const noteStore = useNoteStore();
  
  const props = defineProps<{
    note: Note | null;
  }>();
  
  const emit = defineEmits(['close', 'note-deleted']); 
  
  function closeModal() {
    emit('close'); 
  }

  const deleteNote = async () => {
    const isConfirmed = window.confirm("Kas olete kindel, et soovite selle märke kustutada?");
    if (isConfirmed){
      if (props.note !== null && props.note.id !== undefined) {
        await noteStore.deleteNote(props.note.id);
        emit('note-deleted');
        closeModal();   
      }
    }
  };

  onMounted( async () => {
  if (props.note?.teacherId !== undefined && props.note?.teacherId !== null)
    teacher.value = await teacherStore.getTeacherById(props.note?.teacherId);
  });
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

  .modal-buttons {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
  }

  button {
    padding: 10px 15px;
    border: none;
    border-radius: 4px;
    background-color: #ddd;
    cursor: pointer;
  }

  button:hover {
    background-color: #ccc;
  }

  .close-button {
    margin-top: 20px;
    padding: 10px 15px;
    border: none;
    border-radius: 25px;
    cursor: pointer;
    background-color: #A67F58;
    color: white;
    float: right;
  }
  .close-button:hover {
    background-color: #A67F58;
    /*box-shadow: 0 8px 16px #4c3926;*/
    transform: scale(1.02);
  }

  .delete-button {
    cursor: pointer;
    margin-top: 20px;
    margin-right: 150px;
    padding: 10px 15px;
    border: none;
    border-radius: 25px;
    background-color: #aa2b35;
    color: white;
    cursor: pointer;
  }

  .delete-button:hover {
    background-color: #aa2b35;
    transform: scale(1.02);
    /*box-shadow: 0 8px 16px #912121;*/
  }
  </style>