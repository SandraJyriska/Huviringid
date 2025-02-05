namespace Huviringid_REST.Models.Classes
{
    public record UpcomingLesson {
        public int Id { get; init; }
        public int ExtracurricularId { get; init; }
        public DateTime Date { get; set; }
        public string? LessonName { get; init; }
        public List<string?>? TeacherNames { get; init; }
        public Boolean? IsChecked { get; init; }
        public string? StartTime { get; init; }
        public string? EndTime { get; init; } 
    
    }
}