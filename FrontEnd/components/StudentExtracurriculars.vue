<template>
  <div>
    <h1 class="main-header">Minu info</h1>
    <h2>Huviringid:</h2>

      <!-- Loend huviringidest (ainult nimed) -->
      <div v-for="(extracurricular, index) in extracurriculars" :key="index">
        <button class="extracurricular-button" @click="openModal(extracurricular)">
          {{ extracurricular.name }}
        </button>
      </div>

    <ExtracurricularModal 
      v-if="isModalOpen" 
      :extracurricular="selectedExtracurricular" 
      @close="closeModal" 
    />
  </div>
</template>

<script setup lang="ts">
import { useExtracurricularStore } from '~/stores/extracurricularStore';

const extracurricularStore = useExtracurricularStore();
const { extracurriculars } = storeToRefs(extracurricularStore);
const student = ref<Student>();
const userStore = useUserStore();
const studentStore = useStudentStore();

let isModalOpen = ref(false);
let selectedExtracurricular = ref<Extracurricular>();

onMounted( async () => {
  if (userStore.userId !== null)
    student.value = await studentStore.getStudentByUserId(userStore.userId);
  if (student.value !== undefined)
    await extracurricularStore.loadStudentExtracurriculars(student.value.id);
});

const openModal = (extracurricular: Extracurricular) => {
  selectedExtracurricular.value = extracurricular;
  isModalOpen.value = true;
};

const closeModal = () => {
  isModalOpen.value = false;
};
</script>

<style scoped>
.main-header {
  font-size: 1.5rem;
  color: #A67F58;
  font-weight: 500;
  font-family: sans-serif;
  margin-top: -20px;
  margin-left: 30px;
  position: right;
  width: 180px;
}
h2 {
  font-size: 1.15rem;
  font-weight: bold;
  color: black;
  font-family: sans-serif;
  margin-top: 20px;
  margin-left: 30px;

  margin-bottom: 5px;
}
.extracurricular-button {
  font-size: 1rem;
  color: #A67F58;
  font-family: sans-serif;
  margin-top: 5px;
  margin-left: 30px;

}
</style>

  