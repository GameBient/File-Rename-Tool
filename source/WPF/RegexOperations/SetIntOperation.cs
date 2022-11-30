using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    public class SetIntOperation : SetVarOperation<int>
    {
        public override string ExecuteOP(Match match, string formattedText, int value, string option)
        {
            return formattedText.Replace(match.Value, match.Result(value.ToString($"D{option}")));
        }
    }
}
