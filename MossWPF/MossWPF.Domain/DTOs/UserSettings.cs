using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Domain.DTOs
{
    public record UserSettings(string UserId, string? SubmissionsDirectory=null, string? DefaultFilesLocation=null);
    
}
