namespace File_Rename_Tool.Debugging
{
    public interface ILoggable
    {
        void Log(object? context);
        void Log(string message);
        void Log(string message, object? context);
    }
}
