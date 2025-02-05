  <template> 
  <div class="pt-4 px-20">
    <h1 class="text-3xl font-bold mb-4">
      Registreerimine 
    </h1>
    <div class="mt-2 mb-5"> 
      <USelectMenu 
        v-model="selectedExtracurricularId" 
        :options="extracurricularOptions"
        placeholder="Vali huviring"
        @change="updateSelectedExtracurricular"
        searchable
      />
    </div>
  
    <div v-if="selectedExtracurricular">
      <div class="mb-6 p-4 border rounded shadow-md">
        <h2 class="text-lg font-semibold mb-2">
          {{ selectedExtracurricular.name }}
        </h2>

        <p v-if="selectedExtracurricular.description" class="text-base mb-2">
          <strong>Kirjeldus: </strong> {{ selectedExtracurricular.description }}
        </p>

        <p v-if="selectedExtracurricular.days.length > 0" class="text-base mb-2">
          <strong>Päevad: </strong> 
          <span v-for="(day, index) in selectedExtracurricular.days" :key="index">
            {{ getDayName(day) }}<span v-if="index < selectedExtracurricular.days.length - 1">, </span>
          </span>
        </p>

        <p v-if="selectedExtracurricular.startTime && selectedExtracurricular.endTime" class="text-base mb-2">
          <strong>Aeg: </strong> {{ selectedExtracurricular.startTime }} - {{ selectedExtracurricular.endTime }}
        </p>

        <p v-if="selectedExtracurricular.location" class="text-base mb-2">
          <strong>Asukoht:</strong> {{ selectedExtracurricular.location }}
        </p>

        <p v-if="teachers.length > 0" class="text-base mb-2">
          <strong>Õpetaja(d): </strong> 
          <span v-for="(teacher, index) in teachers" :key="index">
          {{ teacher.name }}<span v-if="index < teachers.length - 1">, </span>
          </span>
        </p>
      </div>
         
      <div class="flex justify-end items-center">
        <UButton 
          type="submit" class="register-button" 
          @click="registerForExtracurricular">Registreeri
        </UButton>
    </div>
  </div>

  </div>
  </template>
  
  <style scoped>
    .grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      grid-column-gap: 4rem; 
      grid-row-gap: 2rem; 
    }
    .register-button {
      justify-content: center;
      align-items: center;
      background-color: #E6CCB2;
      color: black;
      border-radius: 30px;
      box-shadow: 0 6px 12px #4c3926;
      font-size: 1.0rem;
      width: 200px;
      height: 50px;
    }
    .register-button:hover {
      background-color: #E6CCB2;
      transform: scale(1.02);
      box-shadow: 0 8px 16px #4c3926;
    }
  </style>

<script setup lang="ts">
import type { Student } from "~/types/student";
import type { Extracurricular } from "~/types/extracurricular";
import { ref, onMounted, computed, reactive, watch } from 'vue';
import { useExtracurricularStore } from '~/stores/extracurricularStore';
import { useStudentStore } from "~/stores/studentStore";
import { useStudentRegistrationStore } from "~/stores/studentRegistrationStore";

const extracurricularStore = useExtracurricularStore();
const extracurriculars = ref<Extracurricular[]>([]);
const selectedExtracurricular = ref<Extracurricular | null>(null);
const selectedExtracurricularId = ref<number | undefined>(undefined); 
const userStore = useUserStore();
const studentStore = useStudentStore();
const student = ref<Student>();
const teacherStore = useTeacherStore();
const { teachers } = storeToRefs(teacherStore);

const daysOfWeek = [
  "pühapäev", "esmaspäev", "teisipäev", "kolmapäev", "neljapäev", "reede", "laupäev"
];

const getDayName = (dayIndex: any) => {
  return daysOfWeek[dayIndex] || ""; 
};

onMounted(async () => {
  if (userStore.userId !== null)
    student.value = await studentStore.getStudentByUserId(userStore.userId);
  if (student.value !== undefined)
    extracurriculars.value = await extracurricularStore.getAvailableExtracurricularsForStudent(student.value.id);
  });

const extracurricularOptions = computed(() => 
extracurriculars.value.map(e => ({ label: e.name, value: e.id }))
);

async function updateSelectedExtracurricular(value: any) {
  const selected = extracurriculars.value.find(extracurricular => extracurricular.id === value.value);
  selectedExtracurricular.value = selected || null;
  if (selectedExtracurricular.value !== null)
  await teacherStore.loadTeachersForExtracurricular(selectedExtracurricular.value.id);
}

const { addRegistration } = useStudentRegistrationStore();


async function registerForExtracurricular() {
    if (selectedExtracurricular.value !== null && student.value !== undefined) {
      try {
        await addRegistration(student.value.id, selectedExtracurricular.value.id);
        await navigateTo("/upcoming-lessons");
      } catch (error) {
        alert("Problem adding the registration.");
      }
    }
}

</script>