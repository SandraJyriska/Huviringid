<template>
  <div class="register-page">
    <div class="left-side"></div>
    <div class="right-side">
      <div class="register-box">
        <h1 class="register-title">Registreeri kasutaja</h1>
        <UForm
          :validate="validate"
          :state="state"
          class="space-y-4"
          @submit="onSubmit"
          @error="onError">

          <div class="space-y-1">
            <UFormGroup label="Kasutaja tüüp" name="role">
              <USelect 
                v-model="state.role" 
                :options="['Õpilane', 'Õpetaja']" 
                placeholder="Vali tüüp"/>
            </UFormGroup>

            <UFormGroup label="Eesnimi" name="firstName">
              <UInput v-model="state.firstName" />
            </UFormGroup>

            <UFormGroup label="Perekonnanimi" name="lastName">
              <UInput v-model="state.lastName" />
            </UFormGroup>

            <div v-if="state.role === 'Õpilane'">
              <UFormGroup label="Isikukood" name="personalId">
                <UInput v-model="state.personalId" />
              </UFormGroup>
            </div>

            <div v-if="state.role === 'Õpilane'">
              <UFormGroup label="Telefoninumber" name="phoneNr">
                <UInput v-model="state.phoneNr" />
              </UFormGroup>
            </div>

              <UFormGroup label="E-mail" name="email">
                <UInput v-model="state.email" />
              </UFormGroup>

            <UFormGroup label="Kasutajanimi" name="username">
              <UInput v-model="state.username" />
            </UFormGroup>

            <UFormGroup label="Parool" name="password">
              <UInput v-model="state.password" type="password" />
            </UFormGroup>

            <UFormGroup label="Sisesta sama parool" name="password2">
              <UInput v-model="state.password2" type="password" />
            </UFormGroup>
          </div>

          <div class="mt-4">
            <UButton 
              type="submit" 
              class="register-button">
              Registreeri
            </UButton>
          </div>
        </UForm>
      </div>
    </div>
  </div>
</template>


<script setup lang="ts">
import type { FormError, FormErrorEvent, FormSubmitEvent } from "#ui/types";
import { ref, onMounted, reactive } from 'vue';
import { useStudentStore } from "~/stores/studentStore";

const userStore = useUserStore();
const studentStore = useStudentStore();
const teacherStore = useTeacherStore();

const state = reactive({
  id: 0,
  personalId: '',
  firstName: '',
  lastName: '',
  phoneNr: '',
  email: '',
  role: '',
  username: '',
  password: '',
  password2: ''
});

const validate = async (state: any): Promise<FormError[]> => {
  const errors = [];
  if (!state.role) errors.push({ path: "role", message: "Required" });

  if (!state.username) errors.push({ path: "username", message: "Required" });
  else if (await userStore.isUsernameExisting(state.username)){
    errors.push({ path: "username", message: "See kasutajanimi juba eksisteerib!" });
  }

  if (!state.password) errors.push({ path: "password", message: "Required" });
  else if (state.password.length < 5) {
    errors.push({ path: "password", message: "Password must be 5 characters long" });
  }

  if (!state.password2) errors.push({ path: "password2", message: "Required" });
  if (state.password !== state.password2) {
    errors.push({ path: "password2", message: "Enter the same password!" });
  }

  if (!state.firstName) errors.push({ path: "firstName", message: "Required" });
  if (!state.lastName) errors.push({ path: "lastName", message: "Required" });
  if (!state.email) errors.push({ path: "email", message: "Required" });

  if (state.email && !/\S+@\S+\.\S+/.test(state.email)) {
    errors.push({ path: "email", message: "Email is invalid" });
  }

  if (state.role === "Õpilane"){
    if (!state.personalId) errors.push({ path: "personalId", message: "Required" });
    else if (state.personalId.length !== 11) {
      errors.push({ path: "personalId", message: "Personal ID must be 11 characters long" });
    } else if (!/^\d+$/.test(state.personalId)) {
      errors.push({ path: "personalId", message: "Personal ID must contain only numbers" });
    } else if (await studentStore.isPersonalIdExisting(state.personalId)) {
      errors.push({ path: "personalId", message: "User with this personal ID exists" });
    }
    
    if (!state.phoneNr) errors.push({ path: "phoneNr", message: "Required" });
    
    if (state.phoneNr && !/^\+?[0-9]{8,12}$/.test(state.phoneNr)) {
      errors.push({ path: "phoneNr", message: "Phone number is invalid" });
    }
  }
  
  return errors;
};

async function onSubmit(event: FormSubmitEvent<any>) {
  let user = null;
  if (state.role === "Õpilane"){
    state.role = "Student";
  }
  else {
    state.role = "Teacher";
  }
  const userData = {
        id: state.id,
        username: state.username,
        password: state.password, 
        role: state.role
      };
  try {
    user = await userStore.addUser(userData);
  } catch (error) {
    alert("An unexpected error occurred while adding the user.");
  }

  if (user!== null && state.role === "Student"){
    const studentData = {
        id: state.id,
        firstName: state.firstName,
        lastName: state.lastName, 
        email: state.email,
        phoneNr: state.phoneNr,
        personalId: state.personalId,
        userId: user.id
      };
    try {
      await studentStore.addStudent(studentData);
      await navigateTo("/");
    } catch (error) {
      alert("An unexpected error occurred while adding the student.");
    }
  } else if (user!== null && state.role === "Teacher") {
    const teacherData = {
        id: state.id,
        name: state.firstName + " " + state.lastName,
        email: state.email,
        userId: user.id
      };
      try {
        await teacherStore.addTeacher(teacherData);
        await navigateTo("/");
      } catch (error) {
        alert("An unexpected error occurred while adding the teacher.");
      }
  }
}

async function onError(event: FormErrorEvent) {
  const element = document.getElementById(event.errors[0].id);
  element?.focus();
  element?.scrollIntoView({ behavior: "smooth", block: "center" });
}
</script>

<style scoped>
.register-page {
  display: flex;
  height: 100vh;
}

.left-side {
  flex: 1;
  background-image: url('https://img.freepik.com/free-vector/beautiful-illustration-autumn-children-hobbies_52683-71558.jpg?t=st=1734257785~exp=1734261385~hmac=22ebac7a1b1e035ed1d02982347bf570589fac8e7b8a62f783ea6161e206899f&w=1800');
  background-size: cover;
  background-position: center;
}

.right-side {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #f9fafb;
}

.register-button {
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #E6CCB2;
  color: black;
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.3s;
}
.register-button:hover {
  background-color: #D2B48C;
}

.register-box {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 
    0 0 0 2px rgba(210, 180, 140, 0.71),
    0 0 0 6px rgba(222, 184, 135, 0.3),
    0 0 0 10px rgba(139, 69, 19, 0.2);
  width: 100%;
  max-width: 400px;
  text-align: center;
  position: relative;
  margin-top: auto;
  margin-bottom: auto;
  min-height: auto;
}
.register-title {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  color: #333;
}
.flex {
  overflow-y: auto;
}
.flex-grow {
  flex-grow: 1;
  overflow-y: auto;
}
footer {
  position: sticky;
  bottom: 0;
  background-color: white;
  z-index: 10;
}
</style>