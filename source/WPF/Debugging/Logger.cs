namespace File_Rename_Tool.Debugging
{
    public class Logger
    {
        public void Log(object? context)
        {
            if (context == null)
                Log("NULL");
            else
                Log(context.ToString() ?? "NULL", context);
        }

        public void Log(string message)
        {
            Log(message, null);
        }

        public void Log(string message, object? context)
        {
            string contextString = context != null ? $"({context.GetType().Name}) : " : "";
            System.Diagnostics.Debug.WriteLine(contextString + message);
        }
    }
}
