export type StudentMessage = {
    id?: number
    title: string
    sender: string
    content: string
    time: string
    teacherId: number;
    studentIds: number[];
    extracurricularId: number; 
}