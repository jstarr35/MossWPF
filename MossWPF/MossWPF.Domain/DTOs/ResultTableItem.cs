namespace MossWPF.Domain.DTOs
{
   public record ResultTableItem(string FirstFilePath, string SecondFilePath, int FirstFileScore, int SecondFileScore, int LinesMatched, string Link);
}
