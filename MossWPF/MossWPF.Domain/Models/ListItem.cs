using Prism.Mvvm;
using System.Text.Json.Serialization;

namespace MossWPF.Domain.Models
{
    public class ListItem : BindableBase
    {
        private bool _isSelected;
        private string? _name;
        private string? _description;
        private char _code;


        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        [JsonIgnore]
        public char Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }

        [JsonIgnore]
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        [JsonIgnore]
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }
    }
}
