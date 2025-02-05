
namespace Huviringid_REST.Models.Classes
{
    public record Extracurricular
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public required List<int> TeacherIds { get; init; }
        public required List<DayOfWeek> Days { get; init; }
        public string? StartTime { get; init; }
        public string? EndTime { get; init; }        
        public string? Location { get; init; }
        public string? Description { get; init; }
    }
}