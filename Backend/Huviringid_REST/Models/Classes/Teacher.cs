
namespace Huviringid_REST.Models.Classes
{
    public record Teacher
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; } 
        public int UserId { get; set; }
    }
}