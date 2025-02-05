<template>
  <div class="pt-4 px-20">
    <h1 class="text-2xl font-bold text-center mb-4 ">Päevik </h1>

    <div class="mt-2 mb-5"> 
      <USelectMenu 
        v-model="selectedExtracurricular" 
        :options="extracurricularOptions" 
        placeholder="Vali huviring"
        @change="updateSelectedExtracurricular"
      />
    </div> 
  
    <h1 v-if="selectedExtracurricular?.label" class="text-2xl font-bold text-center mb-4 ">
      {{ selectedExtracurricular.label }}
    </h1>

    <div class="custom-spinner" v-if="isLoading">
      <div class="spinner"></div>
    </div>
    <div v-else>
      <div v-if="selectedExtracurricular">
        <UTable v-if="studentRows.length > 0" 
          :columns="columns" 
          :rows="studentRows">
          <template #fullName-data="{ row }">
            <span
              class="student-name clickable"
              @click="openStudentInfoModal(row)"
            >
              {{ row.fullName }}
            </span>
          </template>

          <template v-for="i in numOfDays" :key="i" #[`date_${i}-data`] ="{ row }">
            <span
              v-if="row[`date_${i}`]?.noteType === 'Puudumine' && !selectedCheckboxes.has(`${row.fullName}_${i}`)"
              class="dot red dot-position clickable"
              @click="openReasonModal(row, i)"
            ></span>
            <span
              v-else-if="row[`date_${i}`]?.noteType === 'Hilinemine'"
              class="dot yellow dot-position clickable"
              @click="openReasonModal(row, i)"
            ></span>
            <span
              v-else-if="selectedCheckboxes.has(`${row.fullName}_${i}`)"
              class="dot black dot-position clickable"
              @click="toggleAttendance(row, i)"
            ></span>
            <span v-else>
              <input
                type="checkbox"
                :checked="selectedCheckboxes.has(`${row.fullName}_${i}`)"
                @change="toggleAttendance(row, i)"
                class="checkbox" />
            </span>
          </template>
        </UTable>
        <p v-else class="text-center">Õpilasi pole.</p>
      </div>
    </div>

    <div class="button-container">
      <button v-if="anyCheckboxSelected" @click="addAbsences" class="add-absence-button">
        Lisa puudumine
      </button>
    </div>

    <div class="message-button-container">
      <button v-if="selectedExtracurricular" @click="openMessageModal" class="send-message-button">
        Saada teade
      </button>
    </div>

    <MessageModal 
      v-if="showMessageModal && userStore.userId !== null" 
      @close="closeMessageModal" 
      :selectedExtracurricularId="selectedExtracurricular?.value ?? 0"
      :selectedStudentIds="selectedStudentsIds" 
      :teacherId="teacher?.id"
    />
    
    <ReasonModal
      v-if="showReasonModal"
      :note="currentNote"
      @close="closeReasonModal"
      @note-deleted="handleNoteDeleted"
    />

    <StudentInfoModal 
      v-if="isStudentInfoModalOpen" 
      :student="currentStudent" 
      :extracurricularId="selectedExtracurricular?.value"
      @close="closeStudentInfoModal" 
      @student-deleted="handleStudentDeleted"
    />
    
  </div>
</template>

  
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import ReasonModal from './ReasonModal.vue';
import MessageModal from './MessageModal.vue';
import { format } from 'date-fns';
import { useUserStore } from '~/stores/userStore';
import { useTeacherStore } from '~/stores/teacherStore';
import StudentInfoModal from './StudentInfoModal.vue';
import { useNoteStore } from "@/stores/noteStore";
import { id } from 'date-fns/locale/id';

const selectedCheckboxes = ref(new Set<string>());
const showMessageModal = ref(false); 
const showReasonModal = ref(false);  
const currentNote = ref<Note | null>(null);
const userStore = useUserStore();
const teacherStore = useTeacherStore();
const isStudentInfoModalOpen = ref(false); 
const currentStudent = ref<Student | undefined>(undefined); 
const isLoading = ref(false);

const openMessageModal = () => {
  showMessageModal.value = true;
};

const closeMessageModal = () => {
  showMessageModal.value = false;
};

const openReasonModal = (row: any, dayIndex: number) => {
  showReasonModal.value = true;
  currentNote.value = row[`date_${dayIndex}`];
};

const closeReasonModal = () => {
  showReasonModal.value = false;
  currentNote.value = null;
};

const openStudentInfoModal = (student: Student) => {
  currentStudent.value = student;
  isStudentInfoModalOpen.value = true;
};

const closeStudentInfoModal = () => {
  isStudentInfoModalOpen.value = false;
  currentStudent.value = undefined;
};

const handleStudentDeleted = async () => {
  if (selectedExtracurricular.value !== undefined)
  approvedStudents.value = await studentStore.loadApprovedStudents(selectedExtracurricular.value.value);
};

const handleNoteDeleted = async () => {
  if (selectedExtracurricular.value !== undefined){
    await noteStore.loadNotes();
  }
};

function toggleAttendance(row: any, dayIndex: number) {
  const key = `${row.fullName}_${dayIndex}`;
  if (selectedCheckboxes.value.has(key)) {
    selectedCheckboxes.value.delete(key);
  } else {
    selectedCheckboxes.value.add(key);
  }
}

async function addAbsences() {
  let teacherId = null;
  if (teacher && teacher.value !== null){
    teacherId = teacher.value.id;
  }
  
  if (!teacherId) {
    alert("Õpetaja ei ole sisse logitud.");
    return;
  }
  
  if (!selectedExtracurricular.value || !selectedExtracurricular.value.value) {
    alert("Vali kõigepealt huviring.");
    return;
  }

  for (const key of selectedCheckboxes.value) {
    const [fullName, dayIndexStr] = key.split('_');

    const student = studentRows.value.find((s: any) => s.fullName == fullName);

    if (!student) { 
      alert("Õpilast ei leitud.");
      return;
     }

    const dayIndex = parseInt(dayIndexStr);

    const lessonDateString = columnDates.value[dayIndex-1];
    const lessonDate = new Date(lessonDateString);
    lessonDate.setMinutes(lessonDate.getMinutes() - lessonDate.getTimezoneOffset()); // Aeg UTC-sse

    const note = {
      noteType: 'Puudumine',
      message: '',
      studentId: student.studentId,
      teacherId: teacherId,
      extracurricularId: selectedExtracurricular.value.value,
      lessonDate: lessonDate.toISOString(),
      sendingDateTime: new Date().toISOString(),
    };
    try { await noteStore.addNote(note); } catch (error) {}

  };
  selectedCheckboxes.value.clear();
}

  const anyCheckboxSelected = computed(() => {
      return selectedCheckboxes.value.size > 0;
    });
  
  const columns = ref([
    {
      key: "fullName",
      label: "Nimi", 
    }
  ]);
  
  const numOfDays = 10; 
  
  const studentStore = useStudentStore();
  const extracurricularStore = useExtracurricularStore();
  const noteStore = useNoteStore();
  
  const selectedExtracurricular = ref<{ label: string; value: number }>();
  const approvedStudents = ref<Student[]>([]);
  const approvedNotes = ref<Note[]>([]);

  const {extracurriculars} = storeToRefs(extracurricularStore)
  const teacher = ref<Teacher | null>(null);


onMounted(async () => {
  await noteStore.loadNotes();

  if(userStore.userId != null)
    teacher.value = await teacherStore.getTeacherByUserId(userStore.userId);
  if (teacher.value !== null)
  extracurricularStore.loadTeacherExtracurriculars(teacher.value.id)
});

const columnDates = ref<Date[]>([]);

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
    approvedStudents.value = await studentStore.loadApprovedStudents(selected.id);
    columnDates.value = await extracurricularStore.getExtracurricularDates(selected.id);
    await updateColumns();
    isLoading.value = false;
  }
};

async function updateColumns() {
    if (!selectedExtracurricular.value) return;

    columns.value = [];
    columns.value.push({
        key: "fullName",
        label: "Nimi",
        });
    columnDates.value.forEach((date, index) => {
        columns.value.push({
            key: `date_${index + 1}`,
            label: format(date, 'dd.MM'),
        });
    });
}

const extracurricularOptions = computed(() => 
  extracurriculars.value.map(extracurricular => ({
    label: extracurricular.name,
    value: extracurricular.id,
  }))
);

const selectedStudentsIds = computed(() => { 
  return approvedStudents.value
    .map(student => student.id);  
});


const studentRows = computed(() => {
  return approvedStudents.value.map(student => {
    const studentRow: any = {
      fullName: `${student.firstName} ${student.lastName}`,
      studentId: student.id,
      studentFirstname: student.firstName, 
      studentLastname: student.lastName, 
      studentEmail: student.email, 
      studentPhone: student.phoneNr, 
      studentPersonalId: student.personalId 
    };

    columnDates.value.forEach((lessonDate, i) => { 
      const formattedDate = format(new Date(lessonDate), 'yyyy-MM-dd');

      const notesForDate = noteStore.notes.filter(note =>
        note.studentId === student.id &&
        note.extracurricularId === selectedExtracurricular?.value?.value &&
        format(new Date(note.lessonDate), 'yyyy-MM-dd') === formattedDate
      );

      if (notesForDate.length > 0) {
        const latestNote = notesForDate.reduce((prev, current) => 
          new Date(prev.sendingDateTime) > new Date(current.sendingDateTime) ? prev : current
        );

        studentRow[`date_${i + 1}`] = latestNote;
        
      } else {
        studentRow[`date_${i + 1}`] = null; 
      }    
    });
    return studentRow;
  });
});
</script>

<style>
.checkbox {
  width: 20px;
  height: 20px;
  cursor: pointer;
  transform: translateX(5px);
}
.dot-position {
  display: inline-block;
  margin-left: 5px; 
}
.button-container {
  text-align: right;
  margin-top: 50px;
  margin-right: 100px;
}
.message-button-container {
  position: fixed;
  bottom: 0;
  left: 0;
  margin-left: 100px;
  bottom: 50px;

}
.add-absence-button {
  justify-content: center;
  align-items: center;
  background-color: #E6CCB2;
  color: black;
  border-radius: 30px;
  box-shadow: 0 6px 12px #4c3926;
  font-size: 1.0rem;
  font-weight: 500;
  width: 200px;
  height: 50px;
}
.add-absence-button:hover {
  background-color: #E6CCB2;
  transform: scale(1.02);
  box-shadow: 0 8px 16px #4c3926;
}

</style>

<style scoped>
.dot {
  height: 20px;
  width: 20px;
  border-radius: 50%;
  display: inline-block;
  transition: all 0.3s ease;
}

.red {
  background-color: rgba(255, 0, 0, 0.7); 
  /*box-shadow: 0 0 10px rgba(255, 0, 0, 0.5);*/
}

.yellow {
  background-color: rgba(255, 204, 0, 0.7); 
    /*box-shadow: 0 0 10px rgba(255, 204, 0, 0.5); */
}

.black {
  background-color: rgba(3, 3, 3, 0.7); 
    /*box-shadow: 0 0 10px rgba(104, 99, 99, 0.774); */
}

.dot:hover {
  transform: scale(1.1); 
  opacity: 1; 
}

.send-message-button {
  justify-content: center;
  align-items: center;
  background-color: #E6CCB2;
  color: black;
  border-radius: 30px;
  box-shadow: 0 6px 12px #4c3926;
  font-size: 1.0rem;
  font-weight: 500;
  width: 200px;
  height: 50px;
}

.send-message-button:hover {
  transform: scale(1.02);
  box-shadow: 0 8px 16px #4c3926;
}

.clickable {
  cursor: pointer;
}

.clickable:hover {
  color: #A67F58; 
  font-weight: 500;
}

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

</style>
