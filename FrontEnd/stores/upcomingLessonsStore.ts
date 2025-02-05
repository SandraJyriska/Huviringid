import { defineStore } from 'pinia';
import type { UpcomingLesson } from '~/types/upcomingLesson';

export const useLessonStore = defineStore('upcomingLesson', () => {
  const api = useApi();
  const upcomingLessons = ref<UpcomingLesson[]>([]);

  const loadUpcomingLessonsForStudent = async (studentId: number) => {
    upcomingLessons.value = await api.customFetch<UpcomingLesson[]>(`UpcomingLessons/Student/${studentId}`);
  }

  const seedUpcomingLessons = async () => {
    await api.customFetch<UpcomingLesson[]>('UpcomingLessons/Seed', {
      method: 'POST'
    });
  }

  const refreshUpcomingLessons = async () => {
    await api.customFetch<UpcomingLesson[]>('UpcomingLessons/Refresh-upcoming-lessons', {
      method: 'POST'
    });
  }

  const isLessonAbsent = async (studentId: number, lessonId: number) => {
    const res = await api.customFetch<boolean>(`UpcomingLessons/Students/${studentId}/is-lesson-absent/${lessonId}`);
    return res;
  }

    return {upcomingLessons, loadUpcomingLessonsForStudent, seedUpcomingLessons, refreshUpcomingLessons, isLessonAbsent};
});
