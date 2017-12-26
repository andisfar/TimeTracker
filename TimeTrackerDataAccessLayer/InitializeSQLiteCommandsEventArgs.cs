using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

namespace TimeTrackerDataAccessLayer
{
    public class InitializeSQLiteCommandsEventArgs : EventArgs, IDisposable
    {
        Dictionary<string,SQLiteCommand> _commands;
        public Dictionary<string,SQLiteCommand> Commands { get => _commands; set => _commands = value; }
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

        public void InitializeCommands()
        {
            if(Commands == null)
            {
                Commands = new Dictionary<string, SQLiteCommand>();
            }
        }
    }

    public static partial class Extentions
    {
        public static void AddRange(this Dictionary<string, SQLiteCommand> me, Dictionary<string, string> commands, SQLiteConnection connection)
        {
            if (me == null) me = new Dictionary<string, SQLiteCommand>();
            Debug.Assert(me != null);
            foreach(string commandKey in commands.Keys)
            {
                me.Add(commandKey, new SQLiteCommand(commands[commandKey], connection));
            }
        }

        public static void AddRange(this Dictionary<string, SQLiteCommand> me, Dictionary<string, string> commands)
        {
            if (me == null) me = new Dictionary<string, SQLiteCommand>();
            Debug.Assert(me != null);
            foreach (string commandKey in commands.Keys)
            {
                using (SQLiteCommand sqlite_cmd = new SQLiteCommand(commands[commandKey]))
                {
                    if (me.ContainsKey(commandKey)) continue;
                    me.Add(commandKey, sqlite_cmd);
                }
            }
        }
    }
}