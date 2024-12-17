using System.Collections.ObjectModel;

namespace MossWPF.Domain.Models
{
    public class NavigationItem
    {
        public string Caption { get; set; }
        public string NavigationPath { get; set; }

        public bool IsExpanded { get; set; }

        public ObservableCollection<NavigationItem> Items { get; set; } = new ObservableCollection<NavigationItem>();

        public NavigationItem()
        {

        }
    }
}
