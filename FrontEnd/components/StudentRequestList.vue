<template>
  <div class="pt-4 px-20">
    <h1 class="text-2xl font-bold text-center mb-4 ">Kinnita registreerimised huviringile {{ selectedExtracurricular?.label }}</h1>

    <div class="mt-2 mb-5"> 
      <USelectMenu 
        v-model="selectedExtracurricular" 
        :options="extracurricularOptions" 
        placeholder="Vali huviring"
        @change="updateSelectedExtracurricular"
      />
    </div>

    <div class="custom-spinner" v-if="isLoading">
    <div class="spinner"></div>
    </div>

    <div v-else>
      <div v-if="selectedExtracurricular">
        <UTable 
          v-if="selectedExtracurricular.value !== undefined && pendingStudents.length > 0" 
          :columns="columns" 
          :rows="pendingStudents">
          <template #actions-data="{ row }">
            <button @click="approveStudent(row.id, selectedExtracurricular?.value)" class="btn-approve">
              Kinnita
            </button>
          </template>
        </UTable>
        <p v-else class="text-center">Kinnitust ootavaid registreerimisi pole.</p>
      </div>
    </div>
  </div>
</template>   

<script setup lang="ts">

const columns = [
  {
    key: "firstName",
    label: "Eesnimi",
  },
  {
    key: "lastName",
    label: "Perekonnanimi",
  },
  {
    key: "actions",
    label: "Toimingud",
  },
];

const studentStore = useStudentStore();
const studentRegistrationStore = useStudentRegistrationStore();
const extracurricularStore = useExtracurricularStore();
const userStore = useUserStore();
const teacherStore = useTeacherStore();

const pendingStudents = ref<Student[]>([]);
const {extracurriculars} = storeToRefs(extracurricularStore)
const selectedExtracurricular = ref<{ label: string; value: number }>();
const teacher = ref<Teacher>();
const isLoading = ref(false);


onMounted(async () => {
    await extracurricularStore.loadExtracurriculars();

    if(userStore.userId != null)
      teacher.value = await
    teacherStore.getTeacherByUserId(userStore.userId);
    if(teacher.value !== undefined)
    extracurricularStore.loadTeacherExtracurriculars(teacher.value.id)
}
);

const updateSelectedExtracurricular = async (value: any) => {
  isLoading.value = true;
  const selected = extracurriculars.value.find(extracurricular => extracurricular.id === value.value);
  selectedExtracurricular.value = selected 
    ? { 
        label: selected.name, 
        value: selected.id,  
      } 
    : undefined;
  
  if (selected) {
    pendingStudents.value = await studentStore.loadPendingStudents(selected.id);
  }
  isLoading.value = false;
};

const extracurricularOptions = computed(() => 
  extracurriculars.value.map(extracurricular => ({
    label: extracurricular.name,
    value: extracurricular.id,
  }))
);

const approveStudent = async (studentId: number, extracurricularId: number) => {
    const registration = await studentRegistrationStore.getRegistration(studentId, extracurricularId);  
    registration.status = 'Approved';
    await studentRegistrationStore.updateRegistration(registration);
    pendingStudents.value = await studentStore.loadPendingStudents(extracurricularId);
};
</script>

<style scoped>
.btn-approve {
  justify-content: center;
  align-items: center;
  background-color: #E6CCB2;
  color: black;
  border-radius: 30px;
  box-shadow: 0 6px 12px #4c3926;
  font-size: 1.0rem;
  width: 90px;
  height: 40px;
}
.btn-approve:hover {
  background-color: #E6CCB2;
  transform: scale(1.02);
  box-shadow: 0 8px 16px #4c3926;
}

.mb-4 {
  margin-bottom: 1rem; 
}
</style>