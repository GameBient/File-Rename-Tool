using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using File_Rename_Tool.Debugging;
using File_Rename_Tool.RegexOperations;

namespace File_Rename_Tool
{
    public static class RegexMatcher
    {
        const string PATTERN_TYPE = @"\w+";
        const string PATTERN_VARNAME = @"\w+";

        static List<StringVariable> s_variables = new();
        public static List<StringVariable> Variables => s_variables;

        public static string ChangeTextWithVar(string inputFormat, string outputFormat, string textToFormat, string title)
        {
            // Defines
            Regex setVarReg = new("<" + PATTERN_VARNAME + @"(:\d+)*" + ">");
            s_variables.Clear();

            bool successful = TryEvaluateSplit(inputFormat, textToFormat);
            if (!successful)
                return textToFormat;

            // Set
            var setMatches = setVarReg.Matches(outputFormat).AsEnumerable();
            string formattedText = SetVarOperation.Set(setMatches, outputFormat, s_variables, ref title);
            return formattedText;
        }

        public static bool TryEvaluateSplit(string inputFormat, string textToFormat)
        {
            // Defines
            Regex getVarReg = new("<" + PATTERN_TYPE + "=" + PATTERN_VARNAME + ">");

            s_variables = new();

            string[] inputFormatSplit = getVarReg.Split(inputFormat);

            int fullLengthFormatString = 0;
            int fullLengthOriginalString = 0;
            bool successful = true;
            for (int i = 0; i < inputFormatSplit.Length; i++)
            {
                string currentSplit = inputFormatSplit[i];
                if (currentSplit == string.Empty && i == inputFormatSplit.Length - 1) continue;
                if (fullLengthOriginalString + currentSplit.Length > textToFormat.Length) return false;
                if (textToFormat.Substring(fullLengthOriginalString, currentSplit.Length) != currentSplit) return false;

                fullLengthOriginalString += currentSplit.Length;
                fullLengthFormatString += currentSplit.Length;

                if (i >= inputFormatSplit.Length - 1)
                    break;

                Match varMatch = getVarReg.Match(inputFormat, fullLengthFormatString);
                if (!varMatch.Success || varMatch.Index != fullLengthFormatString) return false;

                GetVariableInfo(varMatch, out string variableType, out string variableName);
                GetVarOperation getOP = GetVarOperation.CheckVariableType(variableType);

                successful = getOP.TryMatch(textToFormat, ref fullLengthOriginalString, out object? v);
                if (!successful) return false;
                if (v != null)
                    s_variables.Add(new StringVariable(variableName, v));
                fullLengthFormatString += varMatch.Length;
            }
            return successful;
        }

        private static void GetVariableInfo(Match match, out string type, out string name)
        {
            string value = match.Value.Trim(new[] { '<', '>' });
            string[] parts = value.Split("=");

            type = parts[0];
            name = parts[1];
        }
    }
}
