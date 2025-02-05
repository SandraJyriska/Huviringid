<template>
    <div class="modal-overlay" @click.self="closeModal">
      <div class="modal-content">
        <h2>Õpilase info:</h2>

        <p v-if="props.student.studentFirstname"><strong>Eesnimi:</strong> {{ props.student.studentFirstname }}</p>
        <p v-if="props.student.studentLastname"><strong>Eesnimi:</strong> {{ props.student.studentLastname }}</p>
        <p v-if="props.student.studentEmail"><strong>E-mail:</strong> {{ props.student.studentEmail }}</p>
        <p v-if="props.student.studentPhone"><strong>Telefon:</strong> {{ props.student.studentPhone }}</p>
        <p v-if="props.student.studentPersonalId"><strong>Isikukood:</strong> {{ props.student.studentPersonalId }}</p>
        
        <p v-if="absenceCount !== null" class="info-text">
        <strong>Puudumiste arv: </strong>{{ absenceCount }}
        </p>

        <button @click="deleteStudent" class="delete-button">Kustuta õpilane</button>
        <button @click="closeModal" class="close-button">Sulge</button>
        
      </div>
    </div>
  </template>
  
  <script setup>

  const registrationStore = useStudentRegistrationStore();
  const studentStore = useStudentStore();
  const { absenceCount } = storeToRefs(studentStore)

  const props = defineProps({
    student: Object, 
    extracurricular: Object,
    extracurricularId: Number
  }); 
  
  const emit = defineEmits(['close', 'student-deleted']);
  
  const closeModal = () => {
    emit('close');
  };

  const deleteStudent = async () => {
    const confirmed = window.confirm(
    `Kas oled kindel, et soovid oma huviringist kustutada õpilase ${props.student.studentFirstname} ${props.student.studentLastname}?`);
    if (confirmed){
      if (props.student.studentId) {
      
      const registration = await registrationStore.getRegistration(props.student.studentId, props.extracurricularId);
      await registrationStore.deleteRegistration(registration.id);
      emit('student-deleted');

      closeModal();   
      }
    }
  };
  
  onMounted(async () => {
    if (props.student) {
        await studentStore.fetchAbsenceCount(props.student.studentId, props.extracurricularId);
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
  
  p, ul {
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

  button.delete-button {
    margin-top: 20px;
    margin-right: 150px;
    padding: 10px 15px;
    border: none;
    border-radius: 25px;
    background-color: #aa2b35;
    color: white;
    cursor: pointer;
  }
  
  button.close-button:hover {
    background-color: #A67F58;
    /*box-shadow: 0 8px 16px #4c3926;*/
    transform: scale(1.02);
  }

  button.delete-button:hover {
    /*box-shadow: 0 8px 16px #912121;*/
    transform: scale(1.02);
  }

  </style>
  