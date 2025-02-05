<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <h2 class="modal-title">Lisa märge:</h2>
      <textarea 
        v-model="note" 
        placeholder="Sisesta põhjus ja muu kasulik info" 
        :class="{ 'required-field': isNoteEmpty && isAttempted }"
        required
      ></textarea>
      <div class="buttons">
        <button class="late-button" @click="sendLate">Saada hilinemine</button>
        <button class="absent-button" @click="sendAbsence">Saada puudumine</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useNoteStore } from '@/stores/noteStore';
import { defineProps, defineEmits } from 'vue';
import { useLessonStore } from '@/stores/upcomingLessonsStore';

const props = defineProps<{
  selectedLessons: UpcomingLesson[];
}>();

const emit = defineEmits(['close']);
const noteStore = useNoteStore();
const note = ref(''); 
const isAttempted = ref(false);
const upcomingLessonsStore = useLessonStore();
const { upcomingLessons } = storeToRefs(upcomingLessonsStore);
const student = ref<Student>();

const userStore = useUserStore();
const studentStore = useStudentStore();

const isNoteEmpty = computed(() => !note.value.trim()); 

const sendNote = async (noteType: string) => {
  isAttempted.value = true;
  if (!isNoteEmpty.value && props.selectedLessons.length > 0) {
    
    props.selectedLessons.forEach(async lesson => {
      const lessonDate = new Date(lesson.date);
      lessonDate.setMinutes(lessonDate.getMinutes() - lessonDate.getTimezoneOffset()); // Aeg UTC-sse
      
      if (!student.value?.id) {
      console.error("Student ID puudub. Märget ei saa lisada.");
      return;
      }

      const noteData = {
        noteType,
        message: note.value,
        studentId: student.value?.id, 
        extracurricularId: lesson.extracurricularId,
        lessonDate: lessonDate.toISOString(),
        sendingDateTime: new Date().toISOString(),
      };
      await noteStore.addNote(noteData);      

    });
    window.location.reload();
    setTimeout(() => {
      closeModal(); // Delay closing the modal to ensure the event is received
    }, 300);
  }
};

const sendLate = () => sendNote('Hilinemine');
const sendAbsence = () => sendNote('Puudumine'); 

function closeModal() {
  emit('close');
  note.value = ''; 
  isAttempted.value = false;
  upcomingLessons.value.forEach(lesson => {
    lesson.isChecked = false;
  });

  props.selectedLessons.splice(0, props.selectedLessons.length);
}

onMounted( async () => {
  if (userStore.userId !== null)
    student.value = await studentStore.getStudentByUserId(userStore.userId);
  if (student.value !== undefined)
    await noteStore.loadNotes();
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

textarea {
  width: 100%;
  height: 100px;
  margin-bottom: 20px;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.buttons {
  display: flex;
  justify-content: space-between;
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

.required-field {
  border: 2px solid rgb(210, 82, 82); 
  background-color: #ffebeb; 
}

.late-button {
  padding: 10px 15px;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  background-color: #A67F58;
  color: white;
  float: right;
}
.late-button:hover {
  background-color: #A67F58;
  /*box-shadow: 0 8px 16px #4c3926;*/
  transform: scale(1.02);
}

.absent-button {
  padding: 10px 15px;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  background-color: #A67F58;
  color: white;
  float: right;
}
.absent-button:hover {
  background-color: #A67F58;
  /*box-shadow: 0 8px 16px #4c3926;*/
  transform: scale(1.02);
}

.modal-title {
  font-size: 1.3rem;
  font-weight: bold;
  margin-bottom: 10px; 
  color:black;
}
</style>
