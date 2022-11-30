using File_Rename_Tool.Resources;
using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    class GetStringOperation : GetVarOperation
    {
        public override string ReadableName => Resource.format_setText;
        protected override Regex expression => new Regex(@"(\w*\s*\S*)+");

        public override bool TryMatchVariable(string varValString, out object? variableValue)
        {
            variableValue = varValString;
            return true;
        }
    }
}
