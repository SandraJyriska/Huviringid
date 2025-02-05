<template>
    <div class="flex justify-center items-center h-screen">
      <div class="form">
        <div v-if="isLoading" class="spinner-container">
            <div class="spinner"></div>
          </div>

        <div v-else>
        <h1 class="text-2xl font-bold text-center mb-4 mt-6 ">Muuda andmeid</h1>
        <UForm
          :validate="validate"
          :state="state"
          class="space-y-2"
          @error="onError">
  
  
          <div v-if="state.role === 'Student'">        
            <UFormGroup label="Eesnimi" name="firstName">
              <UInput v-model="state.firstName" />
            </UFormGroup>
          </div>

          <div v-if="state.role === 'Student'">
            <UFormGroup label="Perekonnanimi" name="lastName">
                <UInput v-model="state.lastName" />
            </UFormGroup>
          </div>

          <div v-if="state.role === 'Teacher'">
            <UFormGroup label="Nimi" name="fullName">
                <UInput v-model="state.fullName" />
            </UFormGroup>
          </div>

          <div v-if="state.role === 'Student'">
            <UFormGroup label="Isikukood" name="personalId">
              <UInput v-model="state.personalId" />
            </UFormGroup>
          </div>
  
          <div v-if="state.role === 'Student'">
            <UFormGroup label="Telefoninumber" name="phoneNr">
              <UInput v-model="state.phoneNr" />
            </UFormGroup>
          </div>

          <UFormGroup label="E-post" name="email">
            <UInput v-model="state.email" />
          </UFormGroup>         

          <div v-if="savedSuccessfully" class="success-message">
            Andmete salvestamine oli edukas!
          </div>
  
          <div class="flex justify-end items-center text-align-center">
            <UButton 
            type="submit" 
            :disabled="!isFormModified"
            @click="onSubmit"
            class="update-button">
              Salvesta muudatused
            </UButton>
          </div>
        </UForm>
        </div>
      </div>
    </div>
  </template>
  
<script setup lang="ts">
  import type { FormError, FormErrorEvent, FormSubmitEvent } from "#ui/types";

  import { reactive, watch, ref } from 'vue';
  import { useStudentStore } from "~/stores/studentStore";
  import { useUserStore } from "~/stores/userStore";
  import { useTeacherStore } from "~/stores/teacherStore";
  
  const userStore = useUserStore();
  const studentStore = useStudentStore();
  const teacherStore = useTeacherStore();
  const isLoading = ref(false);

  const user = ref<User | null>(null);
  
  const state = reactive({
    id: 0, 
    personalId: '',
    firstName: '',
    fullName: '',
    lastName: '',
    phoneNr: '',
    email: '',
    role: '',
    username: '',
    userId: 0 ,
  });
  
  const initialState = reactive({
    personalId: '',
    firstName: '',
    fullName: '',
    lastName: '',
    phoneNr: '',
    email: '',
  }); 

  const isFormModified = ref(false);
  const savedSuccessfully = ref(false);

  watch(
    () => [state.firstName, state.email, state.fullName, state.lastName, state.personalId, state.phoneNr], 
    () => {
        isFormModified.value = !(
            state.firstName === initialState.firstName &&
            state.lastName === initialState.lastName &&
            state.fullName === initialState.fullName &&
            state.email === initialState.email &&
            state.phoneNr === initialState.phoneNr &&
            state.personalId === initialState.personalId
        );
  });

  onMounted(async () => {
    isLoading.value = true;

    if (userStore.userId != null) {

        // Kontrolli, kas kasutaja on Õpilane või Õpetaja
        const student = await studentStore.getStudentByUserId(userStore.userId);
        if (student) {
            state.id = student.id;
            state.personalId = student.personalId;
            state.firstName = student.firstName;
            state.lastName = student.lastName;
            state.phoneNr = student.phoneNr;
            state.email = student.email;
            state.role = 'Student';
            state.userId = userStore.userId;
            //state.username = user.value?.username;

            initialState.firstName = student.firstName;
            initialState.lastName = student.lastName;
            initialState.personalId = student.personalId;
            initialState.phoneNr = student.phoneNr;
            initialState.email = student.email;
        } else {
            const teacher = await teacherStore.getTeacherByUserId(userStore.userId);
            if (teacher) {
                state.id = teacher.id;
                state.fullName = teacher.name;
                state.email = teacher.email;
                state.role = 'Teacher';
                state.userId = userStore.userId;
                //state.username = user.value?.username;

                initialState.fullName = teacher.name;
                initialState.email = teacher.email;
            }
        }
        isLoading.value = false;

    }
  });

  const validate = async (state: any) => {
    const errors = [];
    if (!state.role) errors.push({ path: "role", message: "Required" });
    if (!state.username) errors.push({ path: "username", message: "Required" });
    if (!state.firstName) errors.push({ path: "firstName", message: "Required" });
    if (!state.lastName) errors.push({ path: "lastName", message: "Required" });
    if (!state.fullName) errors.push({ path: "fullName", message: "Required" });
    if (!state.email) errors.push({ path: "email", message: "Required" });
    if (state.email && !/\S+@\S+\.\S+/.test(state.email)) {
      errors.push({ path: "email", message: "Invalid email format" });
    }
    if (state.role === 'Student') {
      if (!state.personalId) errors.push({ path: "personalId", message: "Required" });
      if (!state.phoneNr) errors.push({ path: "phoneNr", message: "Required" });
    }
    return errors;
  };

  async function onSubmit(event: FormSubmitEvent<any>){
    const isConfirmed = window.confirm("Kas oled kindel, et soovid enda andmeid muuta?");

    if (isConfirmed){
        if (user!== null && state.role === "Student"){
        const studentData = {
            id: state.id,
            firstName: state.firstName,
            lastName: state.lastName,
            phoneNr: state.phoneNr,
            email: state.email,
            personalId: state.personalId,
            userId: state.userId,
        }
        await studentStore.updateStudent(studentData)
        }

        if (user!== null && state.role === "Teacher"){
            const teacherData = {
                id: state.id,
                name: state.fullName,
                email: state.email,
                userId: state.userId,
            }
            await teacherStore.updateTeacher(teacherData)
        }
        isFormModified.value = false;
        savedSuccessfully.value = true;
        setTimeout(() => {savedSuccessfully.value = false;}, 2500); 
        setTimeout(() => {window.location.reload();}, 1000);
    }
  }

  async function onError(event: FormErrorEvent) {
    const element = document.getElementById(event.errors[0].id);
    element?.focus();
    element?.scrollIntoView({ behavior: "smooth", block: "center" });
  }
</script>
  
<style scoped>

  .update-title {
    font-size: 1.5rem;
    font-weight: 500;
    margin-bottom: 2rem;
    color: #A67F58;
    text-align: left;
  }

  .form {
    align-self: first baseline;
    width: 500px;
  }

  .update-button {
    background-color: #E6CCB2;
    color: black;
    box-shadow: 0 6px 12px #4c3926;
    padding: 0.75rem;
    border-radius: 30px;
    cursor: pointer;
    font-size: 1rem;
    font-weight: 400;
    transition: background-color 0.3s;
    margin-top: 2rem;
    height: 50px;
    width: 200px;
    text-align: center;
    display: inline-flex; 
    justify-content: center; 
    align-items: center; 
  }

  .update-button:disabled {
    background-color: #d5d5d5; /* Põhi värv, kui nupp on disabled */
    color:rgb(148, 148, 148);
    cursor: not-allowed; /* Muudab kursorit, et näidata, et nupp on keelatud */
    box-shadow: none;

  }

  .update-button:hover {
    /*background-color: #D2B48C;*/
    transform: scale(1.02);
    box-shadow: 0 8px 16px #4c3926;
  }

  .update-button:disabled:hover {
    background-color: #d5d5d5; /* Ei muuda värvi, kui nupp on disabled */
    transform: scale(1.0);
    box-shadow: none;
  }

  .success-message{
    color:rgb(138, 162, 138);
    font-size: 12px;
  }

  .spinner-container {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000; /* Spinner on kõige ees */
    }
  .spinner {
    border: 4px solid #f3f3f3; 
    border-top: 4px solid #4c3926; 
    border-radius: 50%;
    width: 40px;
    height: 40px;
    animation: spin 2s linear infinite;
    display: flex;
    align-items: center;
    justify-content: center; /* Horisontaalne ja vertikaalne tsentreerimine */
    }

    @keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
    }

</style>
  