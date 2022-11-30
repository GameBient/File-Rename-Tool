using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace File_Rename_Tool.RegexOperations
{
    public abstract class GetVarOperation
    {
        public abstract string ReadableName { get; }

        protected abstract Regex expression { get; }

        private static Dictionary<string, GetVarOperation> m_operations = new()
        {
            { "num", new GetIntOperation() },
            { "txt", new GetStringOperation() }
        };

        public static Dictionary<string, GetVarOperation> Operations => m_operations;

        public static GetVarOperation CheckVariableType(string variableType)
        {
            return m_operations.First(x => x.Key == variableType).Value;
        }

        public bool TryMatch(string textToFormat, ref int fullLengthOriginalString, out object? variableValue)
        {
            variableValue = null;
            Match m = expression.Match(textToFormat, fullLengthOriginalString);
            if (!m.Success || m.Index != fullLengthOriginalString)
            {
                //Log($"Match index is {m.Index}\nFullLength is {fullLengthOriginalString}");
                //Log($"The specified format is not correct\t Match index is {m.Index}");
                return false;
            }
            string varValString = m.Value;
            fullLengthOriginalString += varValString.Length;

            return TryMatchVariable(varValString, out variableValue);
        }

        public abstract bool TryMatchVariable(string varValString, out object? variableValue);
    }
}
