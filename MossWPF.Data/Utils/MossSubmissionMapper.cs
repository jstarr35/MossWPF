using MossWPF.Domain.Entities;
using MossWPF.Domain.Models;

namespace MossWPF.Data.Utils
{
    public static class MossSubmissionMapper
    {
        public static MossSubmission ToModel(SubmissionSettings entity, List<ProgrammingLanguage> availableLanguages)
        {
            var selectedLanguage = availableLanguages.FirstOrDefault(lang => lang.Name == entity.SelectedLanguage)
                                   ?? throw new Exception($"Programming language '{entity.SelectedLanguage}' not found.");

            return new MossSubmission
            {
                Id = Guid.NewGuid(),
                SubmissionId = entity.SubmissionId,
                UserId = entity.UserId,
                SelectedLanguage = selectedLanguage, // Map to ProgrammingLanguage object
                Sensitivity = entity.Sensitivity,
                ResultsToShow = entity.ResultsToShow,
                UseDirectoryMode = entity.UseDirectoryMode,
                UseExperimental = entity.UseExperimental,
                Comments = entity.Comments,
                Title = entity.Title,
                DateCreated = entity.DateCreated,
                DateSubmitted = entity.DateSubmitted,
                ResultsLink = string.IsNullOrEmpty(entity.ResultsLink) ? null : new Uri(entity.ResultsLink),
                SourceFiles = new List<FileListItem>(entity.SourceFiles),
                BaseFiles = new List<FileListItem>(entity.BaseFiles)
            };
        }

        // Optional: Convert MossSubmission to SubmissionSettings (for saving to database)
        public static SubmissionSettings ToEntity(MossSubmission model)
        {
            return new SubmissionSettings
            {
                SubmissionId = model.SubmissionId,
                UserId = model.UserId,
                SelectedLanguage = model.SelectedLanguage.Name, // Assuming enum to int mapping
                Sensitivity = model.Sensitivity,
                ResultsToShow = model.ResultsToShow,
                UseDirectoryMode = model.UseDirectoryMode,
                UseExperimental = model.UseExperimental,
                Comments = model.Comments,
                Title = model.Title,
                DateCreated = model.DateCreated,
                DateSubmitted = model.DateSubmitted,
                ResultsLink = model.ResultsLink?.ToString(),
                SourceFiles = model.SourceFiles.ToList(), // Convert ObservableCollection to List
                BaseFiles = model.BaseFiles.ToList() // Convert ObservableCollection to List
            };
        }
    }

}
