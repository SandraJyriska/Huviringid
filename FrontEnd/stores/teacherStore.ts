import type { Teacher } from "~/types/teacher";

export const useTeacherStore = defineStore('teacher', () => {
    const api = useApi();
    const teachers = ref<Teacher[]>([]);

    const loadAllTeachers = async () => {
      const allTeachers = await api.customFetch<Teacher[]>('Teachers');
      return allTeachers;
    }

    const loadTeachersForExtracurricular = async (extracurricularId: number) => {
      teachers.value = await api.customFetch<Teacher[]>(`Teachers/${extracurricularId}/Teachers`);
    };

    const getTeacherById = async (id: number) => {
      const teacher = await api.customFetch<Teacher>(`Teachers/${id}`);
      return teacher;
    };

    const getTeacherByUserId = async (userId: number) => {
      const teacher = await api.customFetch<Teacher>(`Teachers/Users/${userId}`);
      return teacher;
    };

    const addTeacher = async (teacher: Teacher): Promise<Teacher> => {
      const res = await api.customFetch<Teacher>('Teachers', {
        method: 'POST',
        body: teacher,
      });
      return res;
    }

    const updateTeacher = async (teacher: Teacher) => {
      const response = await api.customFetch(`Teachers/${teacher.id}`, {
          method: 'PUT',
          body: teacher
      });
      return response;
    };

    return { teachers, loadTeachersForExtracurricular, getTeacherByUserId, getTeacherById, addTeacher,
      loadAllTeachers, updateTeacher }
})