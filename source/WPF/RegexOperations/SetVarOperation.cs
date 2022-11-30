using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    public abstract class SetVarOperation
    {
        private static SetIntOperation intOperation = new();
        private static SetStringOperation stringOperation = new();

        public static string Set(IEnumerable<Match> matches, string formattedText, List<StringVariable> variables, ref string title)
        {
            foreach (var match in matches)
            {
                string variableString = match.Value.Trim(new[] { '<', '>' });
                string[] variableExpressions = variableString.Split(":");
                string variableName = variableExpressions.ElementAtOrDefault(0) ?? "";
                string option = variableExpressions.ElementAtOrDefault(1) ?? "2";

                StringVariable? matchingVar = variables.FirstOrDefault(v => v.Name == variableName);
                if (matchingVar != null)
                {
                    if (matchingVar.Value is int intVal)
                        formattedText = intOperation.ExecuteOP(match, formattedText, intVal, option);
                    else if (matchingVar.Value is string stringVal)
                        formattedText = stringOperation.ExecuteOP(match, formattedText, stringVal, option);
                }
                else
                {
                    if (variableName == "title")
                        formattedText = formattedText.Replace(match.Value, title);
                    else
                        formattedText = formattedText.Replace(match.Value, "");
                }
            }
            return formattedText;
        }
    }

    public abstract class SetVarOperation<VariableType> : SetVarOperation
    {
        public abstract string ExecuteOP(Match match, string formattedText, VariableType value, string option);
    }
}
