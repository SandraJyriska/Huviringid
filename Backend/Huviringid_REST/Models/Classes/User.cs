
namespace Huviringid_REST.Models.Classes
{
    public record User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = ""; // "Student" or "Teacher"

    }
}