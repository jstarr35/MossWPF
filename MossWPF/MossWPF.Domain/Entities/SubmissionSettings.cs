using MossWPF.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Domain.Entities
{
    public class SubmissionSettings : EntityBase
    {
        public int SubmissionId { get; set; } // Foreign Key
        public string UserId { get; set; } = null!;
        public string SelectedLanguage { get; set; }
        public int Sensitivity { get; set; }
        public int ResultsToShow { get; set; }
        public bool UseDirectoryMode { get; set; }
        public bool UseExperimental { get; set; }
        public string Comments { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime DateSubmitted { get; set; }
        public string? ResultsLink { get; set; }

        // JSON-serialized lists of FileListItem
        public List<FileListItem> SourceFiles { get; set; } = new List<FileListItem>();
        public List<FileListItem> BaseFiles { get; set; } = new List<FileListItem>();

        // Navigation Properties
        public Submission Submission { get; set; } = null!;
    }
}
