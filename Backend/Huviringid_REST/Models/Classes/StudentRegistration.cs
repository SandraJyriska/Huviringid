

namespace Huviringid_REST.Models.Classes
{
    public record StudentRegistration
    {
        public int Id { get; init; }
        public int StudentId { get; init; }
        public int ExtracurricularId { get; init; }
        public string? Status { get; set; }
    }
}