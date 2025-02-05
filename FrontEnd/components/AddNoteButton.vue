<template>
    <div>
      <button 
        class="open-modal-btn" 
        v-if="selectedLessons.length > 0" 
        @click="openModal"
      >
        Lisa märge
      </button>
      <client-only>
        <Modal 
          v-if="showModal" 
          :selectedLessons="selectedLessons" 
          @close="closeModal" 
        />
      </client-only>
  </div>
  </template>
  
  <script>
  import Modal from './NoteModal.vue'
  import { ref, computed } from 'vue';
  import { useLessonStore } from '@/stores/upcomingLessonsStore';
  
  export default {
    components: {
      Modal
    },
    setup() {
    const showModal = ref(false);
    const upcomingLessonsStore = useLessonStore();
    const upcomingLessons = computed(() => upcomingLessonsStore.upcomingLessons);

    const selectedLessons = computed(() => 
      upcomingLessons.value.filter(lesson => lesson.isChecked)
    );

    const openModal = () => {
      showModal.value = true;
    };

    const closeModal = () => {
      showModal.value = false;
    };

    return {
      showModal,
      selectedLessons,
      openModal,
      closeModal
    };
  }
}
  </script>
  
  <style scoped>
  .open-modal-btn {
    justify-content: center;
      align-items: center;
      background-color: #E6CCB2;
      color: black;
      border-radius: 30px;
      box-shadow: 0 6px 12px #4c3926;
      font-size: 1.0rem;
      font-weight: 500;
      width: 150px;
      height: 50px;
      margin-left: 30px;
      margin-top:360px;
  }

  .open-modal-btn:hover {
  transform: scale(1.02);
  box-shadow: 0 8px 16px #4c3926;
}
  </style>