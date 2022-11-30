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

        private static Logger m_logger = new();

        static List<StringVariable> s_variables = new();
        public static List<StringVariable> Variables => s_variables;

        public static string ChangeTextWithVarNew(string inputFormat, string outputFormat, string textToFormat, string title)
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
                if (currentSplit == string.Empty && i == inputFormatSplit.Length - 1)
                    continue;
                if (fullLengthOriginalString + currentSplit.Length > textToFormat.Length)
                {
                    successful = false;
                    break;
                }
                if (textToFormat.Substring(fullLengthOriginalString, currentSplit.Length) != currentSplit)
                {
                    successful = false;
                    break;
                }
                fullLengthOriginalString += currentSplit.Length;
                fullLengthFormatString += currentSplit.Length;

                if (i >= inputFormatSplit.Length - 1)
                    break;

                Match varMatch = getVarReg.Match(inputFormat, fullLengthFormatString);
                if (!varMatch.Success || varMatch.Index != fullLengthFormatString)
                {
                    successful = false;
                    break;
                }

                GetVariableInfo(varMatch, out string variableType, out string variableName);
                GetVarOperation getOP = GetVarOperation.CheckVariableType(variableType);

                successful = getOP.TryMatch(textToFormat, ref fullLengthOriginalString, out object? v);
                if (!successful)
                {
                    break;
                }
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

        [Obsolete("This method is now replaced by ChangeTextWithVarNew")]
        public static string ChangeTextWithVar(string inputFormat, string outputFormat, string textToFormat, string title)
        {
            StringBuilder sb = new();
            void Log(string message)
            {
                sb.AppendLine(message);
            }

            string formattedText = outputFormat;

            string typePattern = @"\w+";
            string varNamePattern = @"\w+";
            Regex getVarReg = new("<" + typePattern + "=" + varNamePattern + ">");
            Regex setVarReg = new("<" + varNamePattern + @"(:\d+)*" + ">");

            Regex intReg = new(@"\d+");
            Regex wordReg = new(@"\w+");
            Regex sentenceReg = new(@"(\w*\s*\S*)+");

            List<StringVariable> variables = new();

            string[] inputFormatSplit = getVarReg.Split(inputFormat);

            int fullLengthFormatString = 0;
            int fullLengthOriginalString = 0;
            bool successful = true;
            for (int i = 0; i < inputFormatSplit.Length; i++)
            {
                string currentSplit = inputFormatSplit[i];
                if (currentSplit == string.Empty && i == inputFormatSplit.Length - 1)
                    continue;
                if (fullLengthOriginalString + currentSplit.Length > textToFormat.Length)
                {
                    successful = false;
                    break;
                }
                string sub = textToFormat.Substring(fullLengthOriginalString, currentSplit.Length);
                Log(sub);
                if (textToFormat.Substring(fullLengthOriginalString, currentSplit.Length) != currentSplit)
                {
                    Log("Split does not match original");
                    successful = false;
                    break;
                }
                fullLengthOriginalString += currentSplit.Length;
                fullLengthFormatString += currentSplit.Length;

                if (i >= inputFormatSplit.Length - 1)
                    break;

                Match varMatch = getVarReg.Match(inputFormat, fullLengthFormatString);
                Log($"Match index is {varMatch.Index}\nFullLength is {fullLengthFormatString}");
                if (!varMatch.Success || varMatch.Index != fullLengthFormatString)
                {
                    successful = false;
                    Log($"This was not expected. Bug!");
                    break;
                }

                GetVariableInfo(varMatch, out string variableType, out string variableName);

                if (variableType == "num")
                {
                    Match m = intReg.Match(textToFormat, fullLengthOriginalString);
                    if (!m.Success || m.Index != fullLengthOriginalString)
                    {
                        successful = false;
                        Log($"Match index is {m.Index}\nFullLength is {fullLengthOriginalString}");
                        Log($"The specified format is not correct\t Match index is {m.Index}");
                        break;
                    }
                    string varValString = m.Value;
                    fullLengthOriginalString += varValString.Length;
                    if (int.TryParse(varValString, out int varVal))
                        variables.Add(new StringVariable<int>(variableName, varVal));
                    Log("(StringVariable) " + variableName + " | val: " + varVal);
                }
                else if (variableType == "txt")
                {
                    Match m = sentenceReg.Match(textToFormat, fullLengthOriginalString);
                    if (!m.Success || m.Index != fullLengthOriginalString)
                    {
                        successful = false;
                        Log($"Match index is {m.Index}\nFullLength is {fullLengthOriginalString}");
                        Log($"The specified format is not correct\t Match index is {m.Index}");
                        break;
                    }
                    string varValString = m.Value;
                    fullLengthOriginalString += varValString.Length;
                    variables.Add(new StringVariable<string>(variableName, varValString));
                }
                fullLengthFormatString += varMatch.Length;
            }
            if (!successful)
                return textToFormat;

            // Set
            var setMatches = setVarReg.Matches(outputFormat).AsEnumerable();
            foreach (var match in setMatches)
            {
                string variableString = match.Value.Trim(new[] { '<', '>' });
                string[] variableExpressions = variableString.Split(":");
                string variableName = variableExpressions.ElementAtOrDefault(0) ?? "";
                string option = variableExpressions.ElementAtOrDefault(1) ?? "1";

                StringVariable? matchingVar = variables.FirstOrDefault(v => v.Name == variableName);
                if (matchingVar != null)
                {
                    if (matchingVar.Value is int intVal)
                        formattedText = formattedText.Replace(match.Value, match.Result(intVal.ToString($"D{option}")));
                    else if (matchingVar.Value is string stringVal)
                        formattedText = formattedText.Replace(match.Value, match.Result(stringVal));
                }
                else
                {
                    if (variableName == "titel")
                        formattedText = formattedText.Replace(match.Value, title);
                    else
                        formattedText = formattedText.Replace(match.Value, "");
                }
            }

            m_logger.Log(sb.ToString());
            return formattedText;
        }
    }
}
