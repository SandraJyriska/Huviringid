import type { Student } from "~/types/student";

export const useStudentStore = defineStore('student', () => {
    const api = useApi();
    const students = ref<Student[]>([]);
    let absenceCount = ref<number>(0); 

    const loadStudents = async () => {
      students.value = await api.customFetch<Student[]>('Students');
    }

    const addStudent = async (student: Student): Promise<Student> => {
      const res = await api.customFetch<Student>('Students', {
        method: 'POST',
        body: student,
      });
      return res;
    }

    const loadPendingStudents = async (extracurricularId: number) => {
      const pendingStudents = await api.customFetch<Student[]>(`Students/Extracurricular/${extracurricularId}/PendingStudents`);
      return pendingStudents;
    };

    const loadApprovedStudents = async (extracurricularId: number) => {
      const approvedStudents = await api.customFetch<Student[]>(`Students/Extracurricular/${extracurricularId}/ApprovedStudents`);
      return approvedStudents;
    };

    const getStudentByUserId = async (userId: number) => {
      const student = await api.customFetch<Student>(`Students/Users/${userId}`);
      return student;
    };

    const fetchAbsenceCount = async (studentId: number, extracurricularId: number) => {
      const response = await api.customFetch<number>(`Students/${studentId}/Extracurricular/${extracurricularId}/absences`); 
      if (response !== undefined && response !== null) {
        absenceCount.value = response;  
      } 
    };

    const isPersonalIdExisting = async (personalId: string) => {
      const res = await api.customFetch<boolean>(`Students/PersonalId-Check/${personalId}`);
      return res;
    }

/*     const updateStudent = async (student: Student) => {
      const result = await api.customFetch(`Students/${student.id}`, {
          method: 'PUT',
          body: student
      });
    }; */

    async function updateStudent(student: Student) {
      const response = await api.customFetch(`Students/${student.id}`, {
          method: "PUT",
          body: student,
      });
  
      return response;
  }
  
    return { students, absenceCount, loadStudents, addStudent, loadPendingStudents, 
      loadApprovedStudents, getStudentByUserId, fetchAbsenceCount, isPersonalIdExisting, updateStudent }
}) 