

namespace Huviringid_REST.Models.Classes
{
    public record StudentMessage
    {
        public int Id { get; init; }
        public string? Title { get; init; }
        public string? Sender { get; init; }
        public string? Content { get; init; }
        public string? Time { get; init; }
        public int TeacherId { get; init; }
        public int ExtracurricularId { get; init; }
        public List<int>? StudentIds { get; init; }
    }
}