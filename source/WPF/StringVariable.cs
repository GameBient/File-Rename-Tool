namespace File_Rename_Tool
{
    public class StringVariable
    {
        protected readonly string m_name;
        readonly object? m_value;

        public string Name => m_name;
        public virtual object? Value => m_value;

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
        readonly T m_value;
        public override object? Value => m_value;

        public StringVariable(string name, T value) : base(name, value)
        {
            m_value = value;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
