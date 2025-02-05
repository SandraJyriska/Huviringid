<template>
  <div class="pt-4 px-20">
    <h1 class="text-3xl font-bold mb-4">Lisa uus huviring</h1>

    <UForm
      :validate="validate"
      :state="state"
      class="grid grid-cols-1 gap-4"
      @submit="onSubmit"
      @error="onError"
    >
      <UFormGroup label="Huviringi nimi" name="name">
        <UInput v-model="state.name" />
      </UFormGroup>

      <UFormGroup label="Asukoht" name="location">
        <UInput v-model="state.location" />
      </UFormGroup>

      <UFormGroup label="Algusaeg" name="startTime">
        <UInput v-model="state.startTime" placeholder="HH:MM" />
      </UFormGroup>

      <UFormGroup label="Lõppaeg" name="endTime">
        <UInput v-model="state.endTime" placeholder="HH:MM" />
      </UFormGroup>

      <UFormGroup label="Valikuline - lisa teised õpetajad" name="otherTeachers">
        <USelectMenu
          v-model="state.teacherIds"
          :options="teacherOptions"
          placeholder="Vali õpetajad"
          multiple
        />
      </UFormGroup>

      <UFormGroup label="Vali päev(ad)" name="days">
        <USelectMenu
          v-model="state.days"
          :options="dayOptions"
          placeholder="Vali õpetajad"
          multiple
        />
      </UFormGroup>

      <UFormGroup label="Kirjeldus" name="description">
        <UTextarea v-model="state.description" />
      </UFormGroup>

      <div class="flex justify-end items-center">
        <UButton
          type="submit"
          class="save-button"
        >
          Salvesta
        </UButton>
      </div>
    </UForm>
  </div>
</template>

<script setup lang="ts">
import { reactive } from "vue";
import type { FormError, FormErrorEvent, FormSubmitEvent } from "#ui/types";
import { useExtracurricularStore } from "~/stores/extracurricularStore";
import { useUserStore } from "~/stores/userStore";
import { useTeacherStore } from "~/stores/teacherStore";

const dayOptions = [
  { label: "Esmaspäev", value: 1 },
  { label: "Teisipäev", value: 2 },
  { label: "Kolmapäev", value: 3 },
  { label: "Neljapäev", value: 4 },
  { label: "Reede", value: 5 },
  { label: "Laupäev", value: 6 },
  { label: "Pühapäev", value: 0 },
];

const extracurricularStore = useExtracurricularStore();
const userStore = useUserStore();
const teacherStore = useTeacherStore();
const teacher = ref<Teacher>();
const otherTeachers = ref<Teacher[]>([]);

  onMounted(async () => {
  if (userStore.userId !== null) {
    teacher.value = await teacherStore.getTeacherByUserId(userStore.userId);
  }

  const allTeachers = await teacherStore.loadAllTeachers();
  if (teacher.value !== undefined){
    otherTeachers.value = allTeachers.filter(t => t.id !== teacher.value?.id);
  }
});

const teacherOptions = computed(() => {
  const options = otherTeachers.value.map(teacher => ({
    label: teacher.name,
    value: teacher.id,
  }));
  return options;
});

const state = reactive({
  id: 0,
  name: "",
  teacherIds: [],
  startTime: "",
  endTime: "",
  location: "",
  description: "",
  days: []
});

const validate = async (state: any): Promise<FormError[]> => { 
  const errors: FormError[] = [];

  if (!state.name) {
    errors.push({ path: "name", message: "Required" });
  } else if (teacher.value !== undefined) {
    const exists = await extracurricularStore.checkExtracurricularExists(teacher.value?.id, state.name);
    if (exists) {
      errors.push({ path: "name", message: "You already have an extracurricular with the same name" });
    }
  }
  if (state.days.length === 0) {
    errors.push({ path: "days", message: "At least one day must be selected" });
  }

  if (!state.startTime) {
    errors.push({ path: "startTime", message: "Required" });
  } else if (!isValidTimeFormat(state.startTime)) {
    errors.push({ path: "startTime", message: "Start time must be in HH:MM format" });
  }

  if (!state.endTime) {
    errors.push({ path: "endTime", message: "Required" });
  } else if (!isValidTimeFormat(state.endTime)) {
    errors.push({ path: "endTime", message: "End time must be in HH:MM format" });
  }

  if (state.startTime && state.endTime && !isStartTimeBeforeEndTime(state.startTime, state.endTime)) {
    errors.push({ path: "endTime", message: "End time must be after start time" });
  }

  if (!state.location) {
    errors.push({ path: "location", message: "Required" });
  }

  if (!state.description) {
    errors.push({ path: "description", message: "Required" });
  }

  return errors;
};

// Abifunktsioonid aja valideerimiseks
function isValidTimeFormat(time: string): boolean {
  const timeRegex = /^([0-9]{2}):([0-9]{2})$/;
  const matches = time.match(timeRegex);

  if (!matches) return false;

  const hours = parseInt(matches[1], 10);
  const minutes = parseInt(matches[2], 10);

  return hours >= 0 && hours <= 23 && minutes >= 0 && minutes <= 59;
}

function isStartTimeBeforeEndTime(startTime: string, endTime: string): boolean {
  const [startHours, startMinutes] = startTime.split(":").map(Number);
  const [endHours, endMinutes] = endTime.split(":").map(Number);

  return startHours < endHours || (startHours === endHours && startMinutes < endMinutes);
}

async function onSubmit(event: FormSubmitEvent<any>) {

  const daysArray = state.days.map((day: any) => day.value);
  const teacherIdsArray = state.teacherIds.map((teacher: any) => teacher.value);

  if (teacher.value !== undefined){
    const extracurricularData = {
      id: state.id,
      name: state.name,
      teacherIds: [teacher.value.id, ...teacherIdsArray],
      startTime: state.startTime,
      endTime: state.endTime,
      location: state.location,
      description: state.description,
      days: daysArray
    };

  try {
    await extracurricularStore.addExtracurricular(extracurricularData);
    await navigateTo("/registered-student-list")

  } catch (error) {
    if (error instanceof Error) {
      alert("Midagi läks valesti: " + error.message);
    } else {
      alert("Midagi läks valesti!");
      console.error(error);
    }
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
.grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-column-gap: 2rem;
  grid-row-gap: 2rem;
}
.save-button {
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
.save-button:hover {
  background-color: #E6CCB2;
  transform: scale(1.02);
  box-shadow: 0 8px 16px #4c3926;
}

@media (max-width: 640px) {
  .sm\:grid-cols-2 {
    grid-template-columns: 1fr;
  }
}
</style>

  
  
  