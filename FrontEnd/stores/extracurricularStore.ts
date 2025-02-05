import type { Extracurricular } from "~/types/extracurricular";

export const useExtracurricularStore = defineStore('extracurricular', () => {
    const api = useApi();
    const extracurriculars = ref<Extracurricular[]>([]);
    
    const loadExtracurriculars = async () => {
      extracurriculars.value = await api.customFetch<Extracurricular[]>('Extracurriculars');
    }

    const getExtracurricularById = async (id: number) => {
      const extracurricular = await api.customFetch<Extracurricular>(`Extracurriculars/${id}`);
      return extracurricular;
    }

    const loadStudentExtracurriculars = async (studentId: number) => {
      extracurriculars.value = await api.customFetch<Extracurricular[]>(`Extracurriculars/student/${studentId}`);
    };

    const loadTeacherExtracurriculars = async (teacherId: number) => {
      extracurriculars.value = await api.customFetch<Extracurricular[]>(`Extracurriculars/teacher/${teacherId}`);
    };

    const getAvailableExtracurricularsForStudent = async (studentId: number) => {
      const extracurriculars = await api.customFetch<Extracurricular[]>(`Extracurriculars/Student/${studentId}/Available`);
      return extracurriculars;
    };

    const getExtracurricularDates = async (extracurricularId: number) => {
      const dates = await api.customFetch<Date[]>(`Extracurriculars/${extracurricularId}/lesson-dates`);
      return dates;
    };

    const addExtracurricular = async (extracurricular: Extracurricular): Promise<Extracurricular> => {
      const res = await api.customFetch<Extracurricular>('Extracurriculars', {
        method: 'POST',
        body: extracurricular,
      });
      extracurriculars.value.push(res);
      return res;
    };

    const checkExtracurricularExists = async (teacherId: number, name: string) => {
      const exists = await api.customFetch<boolean>(`Extracurriculars/teachers/${teacherId}/${name}`);
      return exists;
    };
    
    return {extracurriculars, loadExtracurriculars, getExtracurricularById, loadStudentExtracurriculars, 
      loadTeacherExtracurriculars, getAvailableExtracurricularsForStudent, getExtracurricularDates, addExtracurricular,
      checkExtracurricularExists};
});