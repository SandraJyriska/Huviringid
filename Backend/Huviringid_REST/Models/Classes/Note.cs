namespace Huviringid_REST.Models.Classes
{
    public record Note
    {
        public int Id { get; init; }
        public string? NoteType { get; init; }
        public string? Message { get; init; }
        public int StudentId { get; init; }
        public int? TeacherId { get; init;}
        public int ExtracurricularId { get; init; }
        public DateTime LessonDate { get; init; }
        public DateTime SendingDateTime { get; init; } 
    }
}