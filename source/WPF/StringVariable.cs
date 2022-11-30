namespace File_Rename_Tool
{
    public class StringVariable
    {
        readonly string m_name;
        readonly object? m_value;

        public string Name => m_name;
        public object? Value => m_value;

        public StringVariable(string name, object? value)
        {
            m_name = name;
            m_value = value;
        }

        public override string ToString()
        {
            return $"{m_name} - Value: {m_value}";
        }
    }


    public class StringVariable<T> : StringVariable
    {
        public new T? Value => (T?)base.Value;

        public StringVariable(string name, T value) : base(name, value) { }
    }
}
