<template>
  <div>
    <div class="custom-spinner" v-if="isLoading">
    <div class="spinner"></div>
    </div>

    <div v-else>
      <h2 class="upcoming-title" v-if="upcomingLessons.length > 0">Saabuvad tunnid</h2>
      <h2 class="text-xl" v-else>SAABUVAID TUNDE POLE</h2>
      <div class="info-box" v-for="(lesson, index) in upcomingLessons" :key="index"
          :class="{ 'absent-lesson': lessonAbsenceStatus[lesson.id] }">
        <h3 class="info-title">{{ new Date(lesson.date).toLocaleDateString('en-GB', { day: '2-digit', month: '2-digit' }).replace('/', '.') }}</h3>
        <div class="info-content">
          <div class="red-text">{{ lesson.lessonName }}</div>
          <div class="red-text">
            {{ lesson.startTime }} -
            {{ lesson.endTime }}
          </div>
          <div class="red-text">
            <span v-for="(teacher, idx) in lesson.teacherNames" :key="idx">
              {{ teacher }}<span v-if="idx < lesson.teacherNames.length - 1">, </span>
            </span>
          </div>
          <div class="small-box">
            <input
              type="checkbox"
              :id="'check' + index"
              v-model="lesson.isChecked"
              class="checkbox"
              :disabled="lessonAbsenceStatus[lesson.id]"
            />
            <label :for="'check' + index" class="check-label"></label>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal kuvatakse tingimuslikult -->
    <NoteModal v-if="isModalOpen" @close="isModalOpen = false" :selectedLessons="selectedLessons"/>
  </div>
</template>

<script setup lang="ts">
import { useLessonStore } from '~/stores/upcomingLessonsStore';
import { ref, onMounted } from 'vue';
import { storeToRefs } from 'pinia';

const lessonStore = useLessonStore();
const { upcomingLessons } = storeToRefs(lessonStore);
const isModalOpen = ref(false); 
const selectedLessons = ref<UpcomingLesson[]>([]); 
const lessonAbsenceStatus = ref<{ [key: number]: boolean }>({});
const userStore = useUserStore();
const studentStore = useStudentStore();
const student = ref<Student>();
const isLoading = ref(true);

const loadLessonAbsenceStatus = async (): Promise<void> => {
  const statusMap: { [key: number]: boolean } = {};
  for (const lesson of upcomingLessons.value) {
    if (student.value !== undefined){
      const isAbsent = await lessonStore.isLessonAbsent(student.value.id, lesson.id);
      statusMap[lesson.id] = isAbsent;
    }
  }
  lessonAbsenceStatus.value = { ...statusMap };

}
  
onMounted(async () => {
  try {

    await lessonStore.seedUpcomingLessons();
    await lessonStore.refreshUpcomingLessons();

    if (userStore.userId !== null)
    student.value = await studentStore.getStudentByUserId(userStore.userId);

    if (student.value !== undefined)
    await lessonStore.loadUpcomingLessonsForStudent(student.value.id);

    await loadLessonAbsenceStatus();
  } finally {
    isLoading.value = false;
  }
});

</script>
  
<style scoped>
  .custom-spinner {
    position: fixed; 
    top: 0;           
    left: 0;        
    width: 100vw;    
    height: 100vh;   
    display: flex;    
    justify-content: center; 
    align-items: center;     
  }

  .spinner {
    border: 4px solid #f3f3f3; 
    border-top: 4px solid #4c3926; 
    border-radius: 50%;
    width: 40px;
    height: 40px;
    animation: spin 2s linear infinite;
  }

  @keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
  }

  .absent-lesson {
  background-color: gray;
  opacity: 0.5; 
  pointer-events: none;
  }
  
  .info-box {
  padding: 0.5rem 16px;
  border-bottom: 1px solid #d3d3d3;
  background-color: transparent;
  margin: 13px 0;
  height: 120px;
  width: 100%;
  }

  .info-box:hover {
  transform: scale(1.02);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
  }
  
  .red-text {
    color: black;
    margin-bottom: 0.5px;
  }
  .info-title {
  color: #333;
  font-weight: bold;
  margin-bottom: 1px;
  }

  .info-content {
      display: flex;
      flex-direction: column;
  }

  .checkbox {
    transform: scale(1.5);
    margin-right: 2px;
    margin-left: 480px;
    cursor: pointer;
    position: relative;
    top: -60px;
  }

  .upcoming-title {
    font-size: 1.5rem;
    color: #A67F58;
    font-weight: 500;
    font-family: sans-serif;
    margin-top: 1px;
    margin-left: 15px;
    margin-right: 30px;
    width:500px;
  }
</style>