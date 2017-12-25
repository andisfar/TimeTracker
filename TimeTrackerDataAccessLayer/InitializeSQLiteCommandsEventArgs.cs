using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace TimeTrackerDataAccessLayer
{
    public class InitializeSQLiteCommandsEventArgs : EventArgs, IDisposable
    {
        Dictionary<string,SQLiteCommand> _commands;
        public Dictionary<string,SQLiteCommand> Commands { get => _commands; }
        public InitializeSQLiteCommandsEventArgs(Dictionary<string,string> commands, SQLiteConnection connection):base()
        {
            _commands = new Dictionary<string,SQLiteCommand>();
            _commands.AddRange(commands,connection);
        }

        public void Dispose()
        {
            foreach (var cmd in _commands.Values)
            {
                cmd.Dispose();
            }
            _commands.Clear();
            _commands = null;
            GC.SuppressFinalize(this);
        }
    }

    public static class Extentions
    {
        public static void AddRange(this Dictionary<string, SQLiteCommand> me, Dictionary<string, string> commands, SQLiteConnection connection)
        {
            Debug.Assert(me != null);
            foreach(string commandKey in commands.Keys)
            {
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(commands[commandKey], connection))
                {
                    me.Add(commandKey, sqlite_cmd);
                }
            }
        }

        public static void AddRange(this Dictionary<string, SQLiteCommand> me, Dictionary<string, string> commands)
        {
            Debug.Assert(me != null);
            foreach (string commandKey in commands.Keys)
            {
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(commands[commandKey]))
                {
                    me.Add(commandKey, sqlite_cmd);
                }
            }
        }
    }
}