using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    class SetStringOperation : SetVarOperation<string>
    {
        public override string ExecuteOP(Match match, string formattedText, string value, string option)
        {
            return formattedText.Replace(match.Value, match.Result(value));
        }
    }
}
