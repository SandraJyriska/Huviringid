namespace Huviringid_REST.Models.Classes
{
    public record Student {
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PersonalId { get; set; }
    public string? PhoneNr { get; set; }
    public string? Email { get; set; }
    public int UserId { get; set; }
    }
    
}