using SingleTimerLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using static TimeTrackerDataAccessLayer.DBAccess;

namespace TimeTrackerDataAccessLayer
{
    public class DBAccess
    {
        #region EventHandlerDefs
        #region InitializeCommandEvent
        public delegate void NeedCommandsInitializedHandler(object sender, InitializeSQLiteCommandsEventArgs e);
        public event NeedCommandsInitializedHandler InitializeSQLiteCommands;
        public void OnInitializeSQLiteCommands(object sender, InitializeSQLiteCommandsEventArgs e)
        {
            InitializeSQLiteCommands?.Invoke(sender, e);
        }
        #endregion
        #region NeedConectionStringEvent
        public delegate void NeedConnectionStringHandler(object sender, NeedConnectionStringEventArgs e);
        public event NeedConnectionStringHandler NeedConnectionString;
        private void OnNeedConnectionString(object sender, NeedConnectionStringEventArgs e)
        {
            NeedConnectionString?.Invoke(sender, e);
        }
        #endregion
        #region CreateDatabaseCommandEvent
        public delegate void NeedDatabaseCreateCommandHandler(object sender, NeedDatabaseConnectionCommandEventArgs e);
        public event NeedDatabaseCreateCommandHandler NeedDatabaseCreateCommand;
        public void OnNeedCreateDataBaseQuery(object sender, NeedDatabaseConnectionCommandEventArgs e)
        {
            NeedDatabaseCreateCommand?.Invoke(sender, e);

            using (InitializeSQLiteCommandsEventArgs initCmds = new InitializeSQLiteCommandsEventArgs(Commands, Connection))
            {
                OnInitializeSQLiteCommands(this, initCmds);
                Adapter = new SQLiteDataAdapter
                {
                    UpdateCommand = initCmds.Commands[@"update"],
                    SelectCommand = initCmds.Commands[@"select"],
                    DeleteCommand = initCmds.Commands[@"delete"],
                    InsertCommand = initCmds.Commands[@"insert"]
                };
            }

            var connectionstring = string.Empty;
            using (NeedConnectionStringEventArgs needConnEventArgs = new NeedConnectionStringEventArgs(connectionstring))
            {
                OnNeedConnectionString(this, needConnEventArgs);
                connectionstring = needConnEventArgs.ConnectionString;
                if (connectionstring == string.Empty) throw new InvalidConnectionStringProvidedException($"'{connectionstring}' is not a valid connection string!");
                e.CreateDatabase.Connection = Connection = new SQLiteConnection(connectionstring);
            }
        }

        public static void FillTimersCollection(ref SingleTimerLib.SingleTimersCollection timers, DataTable timer) => timers.AddRange(timer.Rows);
        #endregion
        #endregion
        #region FillDatabase
        public void FillDataTable(DataTable dataTable)
        {
            commandbuilder.DataAdapter.Fill(dataTable);
        }
        #endregion
        #region Firld_Property_Defs
        public Dictionary<string, string> Commands { get; set; }
        public Dictionary<string, SQLiteCommand> sqliteCommands = new Dictionary<string, SQLiteCommand>();
        SQLiteCommand name_exists_command;
        SQLiteCommandBuilder commandbuilder;
        public SQLiteCommandBuilder TimerCommandBuilder { get => commandbuilder; }
        public SQLiteConnection Connection { get; set; }
        public SQLiteDataAdapter Adapter { get; set; }
        public string DatabaseFile { get; set; }
        #endregion
        public DBAccess(string database_file, Dictionary<string,string> commands, DBAccessEventHandlers handlers)
        {
            DatabaseFile = database_file;
            Commands = commands;
            NeedDatabaseCreateCommand += handlers.CreateDBHandler;
            NeedConnectionString += handlers.ConnectionStringHandler;
            InitializeSQLiteCommands += handlers.CommandInitHandler;
            var dataFileInfo = new FileInfo(database_file);
            VerifyCreated(dataFileInfo.DirectoryName);
            Debug.Assert(Directory.Exists(dataFileInfo.DirectoryName));
            VerifyExistsDataBase(DatabaseFile);
            Debug.Print(DatabaseFile);
            if (Adapter == null)
            {
                if (Connection == null)
                {
                    var connectionstring = string.Empty;
                    var needConnEventArgs = new NeedConnectionStringEventArgs(connectionstring);
                    OnNeedConnectionString(this, needConnEventArgs);
                    connectionstring = needConnEventArgs.ConnectionString;
                    if (connectionstring == string.Empty) throw new InvalidConnectionStringProvidedException($"'{connectionstring}' is not a valid connection string!");
                    Connection = new SQLiteConnection(connectionstring);
                }
                var initCmds = new InitializeSQLiteCommandsEventArgs(Commands, Connection);
                Adapter = new SQLiteDataAdapter
                {
                    UpdateCommand = initCmds.Commands[@"update"],
                    SelectCommand = initCmds.Commands[@"select"],
                    DeleteCommand = initCmds.Commands[@"delete"],
                    InsertCommand = initCmds.Commands[@"insert"]
                };
            }
            commandbuilder = new SQLiteCommandBuilder(Adapter);
        }

        public void SaveToDataBase(DataTable timer)
        {
            var update = TimerCommandBuilder.DataAdapter.UpdateCommand;
            var insert = TimerCommandBuilder.DataAdapter.InsertCommand;
            var delete = TimerCommandBuilder.DataAdapter.DeleteCommand;

            var affected = 0;
            foreach (DataRow row in timer.Rows)
            {
                if(row.RowState == DataRowState.Added)
                {
                    var name = new SQLiteParameter("@Name", row[1]);
                    var elapsed = new SQLiteParameter("@Elapsed", row[2]);
                    var paramList = new List<SQLiteParameter> { name, elapsed };
                    insert.Parameters.AddRange(paramList.ToArray());
                    insert.Open();
                    affected += insert.ExecuteNonQuery();
                    insert.Close();
                }
                if (row.RowState == DataRowState.Modified)
                {
                    var id = new SQLiteParameter("@Id", row[0]);
                    var name = new SQLiteParameter("@Name", row[1]);
                    var elapsed = new SQLiteParameter("@Elapsed", row[2]);
                    var paramList = new List<SQLiteParameter> { id, name, elapsed };
                    update.Parameters.AddRange(paramList.ToArray());
                    update.Open();
                    affected += update.ExecuteNonQuery();
                    update.Close();
                }
                if (row.RowState == DataRowState.Deleted)
                {
                    var id = new SQLiteParameter("@Id", row.OriginalId());
                    update.Parameters.Add(id);
                    delete.Open();
                    affected += delete.ExecuteNonQuery();
                    delete.Close();
                }
            }
            Log_Message($"{affected} rows affected!");
            timer.AcceptChanges();
            TimerCommandBuilder.DataAdapter.Update(timer);
        }

        private static void Log_Message(string message)
        {
            Debug.Print(message);
        }

        private static void VerifyCreated(string directoryName)
        {
            if (Directory.Exists(directoryName)) return;
            var di = new DirectoryInfo(directoryName);
            var dirsNames = new Queue<string>();
            foreach (string part in directoryName.Split(Path.DirectorySeparatorChar))
            {
                Debug.Print(part);
                dirsNames.Enqueue(part);
            }
            var baseDir = CheckBuildBaseDir(dirsNames);
            if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        }
        private static string CheckBuildBaseDir(Queue<string> dirsNames)
        {
            var baseDir = dirsNames.Dequeue() + Path.DirectorySeparatorChar.ToString(); Debug.Print(baseDir);
            var builder = new StringBuilder();
            builder.Append(baseDir);
            do
            {
                if (Directory.Exists(baseDir))
                {
                    Debug.Print(baseDir + " exists!");
                }
                else
                {
                    Directory.CreateDirectory(baseDir);
                }
                builder.Append(Path.DirectorySeparatorChar.ToString() + dirsNames.Dequeue());
            } while (dirsNames.Count > 0);
            baseDir = builder.ToString();
            return baseDir;
        }
        private void VerifyExistsDataBase(string dataFile)
        {
            if (!File.Exists(dataFile))
            {
                using (NeedDatabaseConnectionCommandEventArgs cr_db = new NeedDatabaseConnectionCommandEventArgs())
                {
                    OnNeedCreateDataBaseQuery(this, cr_db);
                    if(cr_db.CreateDatabase == null)
                    {
                        throw new MissingCreateDatabaseCommandException("Database creation script not found!");
                    }
                    // open the database connection
                    cr_db.CreateDatabase.Connection.Open();
                    cr_db.CreateDatabase.ExecuteNonQuery();
                    // close the database connection
                    Connection.Close();
                }
            }
            // if all is well the database exists and is populated with default info or alredy existed
        }
    }
    #region EventHandlerTypes
    public class DBAccessEventHandlers : EventArgs
    {
        public NeedCommandsInitializedHandler CommandInitHandler { get; set; }
        public NeedConnectionStringHandler ConnectionStringHandler { get; set; }
        public NeedDatabaseCreateCommandHandler CreateDBHandler { get; set; }
    }
    #endregion

    public static partial class Extentions
    {
        public static void AddRange(this SingleTimersCollection me, DataRowCollection timersToAdd)
        {
            foreach(DataRow timer_db in timersToAdd)
            {
                me.AddTimer(timer_db.KeyToInt(), timer_db.TimerName(), timer_db.ElapsedTimeOffset());
            }
        }

        public static string ElapsedTimeOffset(this DataRow me)
        {
            return me[2].ToString();
        }

        public static string TimerName(this DataRow me)
        {
            return me[1].ToString();
        }

        public static int KeyToInt(this DataRow me)
        {
            return Int32.Parse(me[0].ToString());
        }

        public static int OriginalId(this DataRow me)
        {
            return Int32.Parse(me["Id", DataRowVersion.Original].ToString());
        }

        public static void Open(this SQLiteCommand me)
        {
            me.Connection.Open();
        }

        public static void Close(this SQLiteCommand me)
        {
            me.Connection.Close();
        }
    }
}
