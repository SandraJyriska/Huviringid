import type { Note } from '~/types/note';

export const useNoteStore = defineStore('note', () => {
  const api = useApi();
  const notes = ref<Note[]>([]);

  const loadNotes = async () => {
    notes.value = await api.customFetch<Note[]>('Notes');
  }

  const addNote = async (note: Note) => {
    try {
      const res = await api.customFetch<Note>('Notes', {
        method: 'POST',
        body: note,
      });
  
      if (res) {
        notes.value.push(res);
      }
    } catch (error) {
      console.error('Failed to add note:', error);
    }
  };

  const deleteNote = async (id: number) => {
    const result = await api.customFetch(`Notes/${id}`, {
        method: 'DELETE',
    });
};

  return { notes, loadNotes, addNote, deleteNote }
})

