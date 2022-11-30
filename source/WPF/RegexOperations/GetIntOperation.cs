using File_Rename_Tool.Resources;
using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    public class GetIntOperation : GetVarOperation
    {
        public override string ReadableName => Resource.format_setNumber;
        protected override Regex expression => new Regex(@"\d+");

        public override bool TryMatchVariable(string varValString, out object? variableValue)
        {
            bool successful = int.TryParse(varValString, out int varVal);
            variableValue = varVal;
            return successful;
        }
    }
}
