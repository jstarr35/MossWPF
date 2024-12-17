using MossWPF.Domain.Enums;

namespace MossWPF.Domain.Models
{
    public class FileListItem : ListItem
    {
        public string Path { get; }
        public string Extension { get; }
        public string Parent { get; set; }
        public FileListItem(string path, char code)
        {
            Path = path;

            Code = code;

            Extension = GetExt(path);
        }
        private string GetExt(string path)
        {
            string[] strings = path.Split(new char[] { '.' });
            if (strings.Length == 2)
            {
                return strings[1];
            }
            return string.Empty;
        }
        private FileExtension GetExtensionFromPath(string path)
        {
            string[] pathParts = path.Split('.');
            FileExtension rtn; ;
            if (pathParts.Length > 1)
            {
                _ = Enum.TryParse(pathParts[pathParts.Length - 1], out rtn);
                Name = pathParts[pathParts.Length - 2];
                if (pathParts.Length > 2)
                    Parent = pathParts[pathParts.Length - 3];
            }
            else
            {
                rtn = FileExtension.none;
            }
            return rtn;
        }

    }
}
