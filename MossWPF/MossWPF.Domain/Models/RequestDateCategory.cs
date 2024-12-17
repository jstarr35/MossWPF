using System.Collections.ObjectModel;

namespace MossWPF.Domain.Models
{
    public sealed class RequestDateCategory
    {
        public string DisplayDate { get; }
        public ObservableCollection<MossSubmission> Submissions { get; }

        public RequestDateCategory(string displayDate, params MossSubmission[] submissions)
        {
            DisplayDate = displayDate;
            Submissions = new ObservableCollection<MossSubmission>(submissions);
        }
    }
}
