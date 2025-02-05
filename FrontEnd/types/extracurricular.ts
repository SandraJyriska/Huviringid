export type Extracurricular = {
    id: number
    name: string
    teacherIds: number[];
    startTime: string
    endTime: string
    location: string
    description: string
    days: number[]
}