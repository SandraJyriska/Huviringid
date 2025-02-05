import type { StudentMessage } from '~/types/studentmessage';

export const useMessageStore = defineStore('studentmessage', () => {
  const api = useApi(); 
  const messages = ref<StudentMessage[]>([]);

  const loadStudentMessages = async (studentId: number) => {
    messages.value = await api.customFetch<StudentMessage[]>(`StudentMessages/student/${studentId}`);
  };

  const sendMessage = async (message: StudentMessage) => {
    await api.customFetch('StudentMessages', {
      method: 'POST',
      body: message,
    });
  };

  return { messages, loadStudentMessages, sendMessage };
});