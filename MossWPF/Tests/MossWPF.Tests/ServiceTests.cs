

using MossWPF.Domain;
using MossWPF.Services;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MossWPF.Tests
{
    public class ServiceTests
    {
        static List<MossSubmission> _submissions = new List<MossSubmission>()
        {
            new MossSubmission()
            {
                Id = Guid.NewGuid(),
                SelectedLanguage = new ProgrammingLanguage()
                {
                    Name = "C++",
                    Icon = 'C',
                    Code = "c",
                    Extensions = new List<string>()
                    {
                        ".cpp",
                        ".h",
                        ".hpp",
                        ".cc"
                    }
                },
                ResultsToShow = 250,
                Sensitivity = 10,
                UseDirectoryMode = false,
                UseExperimental = false,
                Comments = "1016 Java II - Week 6 Assignment",
                DateSubmitted = DateTime.Now,
                SourceFiles = new ObservableCollection<FileListItem>(new List<FileListItem>()
                {
                    new FileListItem("F:\\repos\\demos\\c89\\C89Demo\\C89Demo\\C89Demo.cpp", 'S'),
                    new FileListItem("F:\\repos\\demos\\c89\\C89_Copy\\C89_Copy\\C89_Copy.cpp", 'S')
                }),
                BaseFiles = new ObservableCollection<FileListItem>()
                {
                    new FileListItem("F:\\repos\\demos\\c89\\C89Demo\\C89Demo\\InstructorPrompt.cpp", 'B')
                },
                ResultsLink = new Uri("http://moss.stanford.edu/results/8/2260694544265/"),
                Title = "Week 6 Assignment"
            }
        };

        [Fact]
        public void ShouldSerializeSubmissionToJsonFile()
        {
            string exeDirectory = Path.GetDirectoryName(
                Assembly.GetEntryAssembly()?.Location)!;
            var service = new SubmissionService();
            service.SaveSubmissionAsync(_submissions[0], exeDirectory);
        }

        [Fact]
        public void ShouldParseResults()
        {
            
            var service = new SubmissionService();
            _ = service.ParseResultsAsync();
        }
    }

}