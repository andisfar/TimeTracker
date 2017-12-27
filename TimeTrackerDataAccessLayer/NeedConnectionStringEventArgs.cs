using System;

namespace TimeTrackerDataAccessLayer
{
    public class NeedConnectionStringEventArgs : EventArgs, IDisposable
    {
        string connection_string;
        public string ConnectionString { get => connection_string; set => connection_string = value; }
        public NeedConnectionStringEventArgs(string connectionstring)
        {
            connection_string = connectionstring;
        }
        public void Dispose()
        {
            connection_string = string.Empty;
            GC.SuppressFinalize(this);
        }
    }
}