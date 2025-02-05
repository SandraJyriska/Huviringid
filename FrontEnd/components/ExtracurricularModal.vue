<template>
  <div class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <h2>Huviring:</h2>
      <p><strong>{{ extracurricular?.name }}</strong></p>

      <p v-if="extracurricular?.description"> {{ extracurricular.description }}</p>

      <p v-if="teachers.length > 0" class="info-text">
        <strong>Õpetajad: </strong>
        <span v-for="(teacher, index) in teachers" :key="index">
          {{ teacher.name }}<span v-if="index < teachers.length - 1">, </span>
        </span>
      </p>

      <p v-if="teachers.length > 0" class="info-text">
        <strong>Kontaktinfo: </strong>
        <span v-for="(teacher, index) in teachers" :key="index">
          {{ teacher.email }}<span v-if="index < teachers.length - 1">, </span>
        </span>
      </p>

      <p v-if="extracurricular?.location" class="info-text"><strong>Asukoht:</strong> {{ extracurricular?.location }}</p>
      
      <p v-if="extracurricular.days.length > 0" class="info-text">
        <strong>Päevad: </strong>
        <span v-for="(day, index) in extracurricular.days" :key="index">
            {{ getDayName(day) }}<span v-if="index < extracurricular.days.length - 1">, </span>
        </span>
      </p>
      
      <p v-if="extracurricular?.startTime && extracurricular?.endTime" class="info-text">
          <strong>Aeg: </strong> {{ extracurricular.startTime }} - {{ extracurricular.endTime }}
      </p>

      <p v-if="absenceCount !== null" class="info-text">
        <strong>Puudumiste arv: </strong>{{ absenceCount }}
      </p>

      <button @click="closeModal" class="close-button">Sulge</button>
    </div>
  </div>
</template>


<script setup>
const props = defineProps({
  extracurricular: Object, 
});

const emit = defineEmits(['close']);
const teacherStore = useTeacherStore();
const { teachers } = storeToRefs(teacherStore);

const studentStore = useStudentStore();
const { absenceCount } = storeToRefs(studentStore)

const userStore = useUserStore();

const extracurricularStore = useExtracurricularStore
const { extracurricularId } = storeToRefs(extracurricularStore)

const daysOfWeek = [
  "pühapäev", "esmaspäev", "teisipäev", "kolmapäev", "neljapäev", "reede", "laupäev"
];

const getDayName = (dayIndex) => {
  return daysOfWeek[dayIndex] || ""; 
};

const closeModal = () => {
  emit('close');
};

  onMounted(async () => {
    try {
      await teacherStore.loadTeachersForExtracurricular(props.extracurricular.id);

      if (props.extracurricular?.id && userStore.userId != null) {
        const student = await studentStore.getStudentByUserId(userStore.userId);

        if (student) {
          await studentStore.fetchAbsenceCount(student.id, props.extracurricular.id);
        }
      }
    } catch (error) {
    }
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

.info-text {
  font-size: 0.9rem;
}

h2 {
  font-size: 1.25rem;
  color: black;
  font-weight: bold;
  font-family: sans-serif;
}

p {
  font-size: 1rem;
  color: black;
  margin-top: 10px;
}

button.close-button {
  margin-top: 20px;
  padding: 10px 15px;
  border: none;
  border-radius: 25px;
  cursor: pointer;
  background-color: #A67F58;
  color: white;
  float: right;
}

button.close-button:hover {
  background-color: #A67F58;
  /*box-shadow: 0 8px 16px #4c3926;*/
  transform: scale(1.02);
}
</style>



  