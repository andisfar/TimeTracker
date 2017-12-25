﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Text;
using static TimeTrackerDataAccessLayer.DBAccess;

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
                    InsertCommand = initCmds.Commands["@insert"]
                };
            }

            var connectionstring = string.Empty;
            using (NeedConnectionStringEventArgs needConnEventArgs = new NeedConnectionStringEventArgs(connectionstring))
            {
                OnNeedConnectionString(this, needConnEventArgs);
                if (connectionstring == string.Empty) throw new InvalidConnectionStringProvidedException($"'{connectionstring}' is not a valid connection string!");
                Connection = new SQLiteConnection(connectionstring);
            }
        }
        #endregion
        public Dictionary<string, string> Commands { get; set; }
        SQLiteCommand name_exists_command;
        SQLiteCommandBuilder commandbuilder;
        public SQLiteCommandBuilder TimerCommandBuilder { get => commandbuilder; set => commandbuilder = value; }
        public SQLiteConnection Connection { get; set; }
        public SQLiteDataAdapter Adapter { get; set; }
        public string DatabaseFile { get; set; }
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

            commandbuilder = new SQLiteCommandBuilder(Adapter);
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

    public class DBAccessEventHandlers : EventArgs
    {
        public NeedCommandsInitializedHandler CommandInitHandler { get; set; }
        public NeedConnectionStringHandler ConnectionStringHandler { get; set; }
        public NeedDatabaseCreateCommandHandler CreateDBHandler { get; set; }
    }
}