namespace MossWPF.Domain.DTOs
{
    public record UserSettings(string UserId, string? SubmissionsDirectory=null, string? DefaultFilesLocation=null);
    
}
