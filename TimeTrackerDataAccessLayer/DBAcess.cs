using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TimeTrackerDataAccessLayer
{
    public class DBAccess
    {
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

        SQLiteCommand name_exists_command;
        SQLiteCommandBuilder commandbuilder;
        public SQLiteCommandBuilder TimerCommandBuilder { get => commandbuilder; set => commandbuilder = value; }
        readonly SQLiteConnection connection;
        readonly SQLiteDataAdapter adapter;

        public string DatabaseFile { get; set; }
        public DBAccess(string database_file, Dictionary<string,string> commands)
        {
            DatabaseFile = database_file;
            var dataFileInfo = new FileInfo(database_file);
            VerifyCreated(dataFileInfo.DirectoryName);
            Debug.Assert(Directory.Exists(dataFileInfo.DirectoryName));
            VerifyExistsDataBase(DatabaseFile);
            Debug.Print(DatabaseFile);
            var connectionstring = string.Empty;
            using (NeedConnectionStringEventArgs needConnEventArgs = new NeedConnectionStringEventArgs(connectionstring))
            {
                OnNeedConnectionString(this, needConnEventArgs);
                if (connectionstring == string.Empty) throw new InvalidConnectionStringProvidedException($"'{connectionstring}' is not a valid connection string!");
                connection = new SQLiteConnection(connectionstring);
            }
            using (InitializeSQLiteCommandsEventArgs initCmds = new InitializeSQLiteCommandsEventArgs(commands, connection))
            {
                OnInitializeSQLiteCommands(this, initCmds);
                adapter = new SQLiteDataAdapter()
                {
                    UpdateCommand = initCmds.Commands[@"update"],
                    SelectCommand = initCmds.Commands[@"select"],
                    DeleteCommand = initCmds.Commands[@"delete"],
                    InsertCommand = initCmds.Commands["@insert"]
                };
            }
            commandbuilder = new SQLiteCommandBuilder(adapter);
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
            if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        }
        private static void VerifyExistsDataBase(string dataFile)
        {
            var connection_string = string.Format(Properties.Resources.connection_string, dataFile);
            var connection = new SQLiteConnection(connection_string, true);
            if (!File.Exists(dataFile))
            {
                var sql_create_text = Properties.Resources.dbo_CreateTimer;
                // open the database connection
                connection.Open();
                using (var create_db_command = new SQLiteCommand(sql_create_text, connection))
                {
                    create_db_command.ExecuteNonQuery();
                    // close the database connection
                    connection.Close();
                }
            }
            // if all is well the database exists and is populated with default info or alredy existed
        }
    }
}
