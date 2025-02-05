export type UpcomingLesson = {
    id: number
    extracurricularId: number
    date: Date
    lessonName: string
    teacherNames: string[]
    startTime: string
    endTime: string
    isChecked: boolean
}