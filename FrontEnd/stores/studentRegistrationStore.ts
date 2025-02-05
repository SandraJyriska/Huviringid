

export const useStudentRegistrationStore = defineStore('studentRegistration', () => {
    const api = useApi(); 
    const registrations = ref<StudentRegistration[]>([]);

    const loadStudentRegistrations = async () => {
        registrations.value = await api.customFetch<StudentRegistration[]>('StudentRegistrations'); 
    };

    const addRegistration = async (studentId: number, extracurricularId: number) => {
        const newRegistration: StudentRegistration = {
            id: 0, 
            studentId: studentId,
            extracurricularId: extracurricularId,
            status: 'Pending'
        };

        const res = await api.customFetch<StudentRegistration>('StudentRegistrations', {
            method: 'POST',
            body: newRegistration,
        });
        registrations.value.push(res);
    };

    const getRegistration = async (studentId: number, extracurricularId: number) => {
        const registration = await api.customFetch<StudentRegistration>(`StudentRegistrations/Registration/${studentId}/${extracurricularId}`);
        return registration;
    };

    const updateRegistration = async (registration: StudentRegistration) => {
        const result = await api.customFetch(`StudentRegistrations/${registration.id}`, {
            method: 'PUT',
            body: registration
        });
    };

    const deleteRegistration = async (id: number) => {
        const result = await api.customFetch(`StudentRegistrations/${id}`, {
            method: 'DELETE',
        });
    };

    return { registrations, loadStudentRegistrations, addRegistration, getRegistration, updateRegistration,
        deleteRegistration };
});