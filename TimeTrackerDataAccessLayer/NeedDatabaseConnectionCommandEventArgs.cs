using System;
using System.Data.SQLite;

namespace TimeTrackerDataAccessLayer
{
    public class NeedDatabaseConnectionCommandEventArgs : EventArgs, IDisposable
    {
        SQLiteCommand _create_db_command;
        public SQLiteCommand CreateDatabase { get => _create_db_command; set => _create_db_command = value; }
        public NeedDatabaseConnectionCommandEventArgs() : base()
        { }
        public void Dispose()
        {
            _create_db_command.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}