export type Note = {
    id?: number
    noteType: string
    message: string
    studentId: number
    teacherId?: number
    extracurricularId: number
    lessonDate: string 
    sendingDateTime: string
}