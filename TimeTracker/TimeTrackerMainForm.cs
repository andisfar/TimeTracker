using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TimeTrackerDataAccessLayer;

namespace TimeTracker
{
    public partial class TimeTrackerMainForm : Form
    {
        private static string ConnectionString
        {
            get => Get_Connection_String();
        }
        private static string Get_Connection_String()
        {
            var DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
            var connection_string = string.Format(Properties.Resources.connection_string, DataFile);
            return connection_string;
        }
        private Dictionary<string, string> commands;
        public Dictionary<string, string> Commands
        {
            get => commands;
            set => commands = value;
        }

        private DBAccess dal;

        public TimeTrackerMainForm()
        {
            InitializeComponent();
            var DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default[@"DataFile"].ToString();
            var dataFileInfo = new FileInfo(DataFile);
            //
            Commands = new Dictionary<string, string>
            {
                { "update", Properties.Resources.update_command },
                { "select", Properties.Resources.select_all_rows },
                { "delete", Properties.Resources.delete_command },
                { "insert", Properties.Resources.insert_command }
            };
            //
            dal = new DBAccess(DataFile, commands, new DBAccessEventHandlers
            {
                CommandInitHandler = DataAcessLayer_InitializeSQLiteCommands,
                CreateDBHandler = DataAccessLayer_NeedDatabaseCreateCommand,
                ConnectionStringHandler = DataAcessLayer_NeedConnectionString
            });
            //
            dal.FillDataTable(TimerDataSet.Tables[nameof(Timer)]);
            ConnectEventHandlers();
        }
        private void ConnectEventHandlers()
        {
            TimerDataGridView.DataError += TimerDataGridView_DataError;
            dal.TimerCommandBuilder.DataAdapter.FillError += DataAdapter_FillError;
            dal.TimerCommandBuilder.DataAdapter.RowUpdated += DataAdapter_RowUpdated;
            dal.TimerCommandBuilder.DataAdapter.RowUpdating += DataAdapter_RowUpdating;

        }

        private static void DataAdapter_RowUpdating(object sender, System.Data.Common.RowUpdatingEventArgs e)
        {
            Log_Message(e.StatementType.ToString());
            Log_Message(e.Status.ToString());
        }

        private static void Log_Message(string message)
        {
            Debug.Print(message);
        }

        private static void DataAdapter_RowUpdated(object sender, System.Data.Common.RowUpdatedEventArgs e)
        {
            Log_Message($"{e.RecordsAffected} rows affected!");
            Log_Message(e.StatementType.ToString());
            Log_Message(e.Status.ToString());
        }

        private static void DataAdapter_FillError(object sender, System.Data.FillErrorEventArgs e)
        {
            Log_Error(e.Errors.Message);
        }

        private static void Log_Error(string message)
        {
            Debug.Print(message);
        }

        private static void TimerDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private static void DataAcessLayer_NeedConnectionString(object sender, NeedConnectionStringEventArgs e)
        {
            e.ConnectionString = ConnectionString;
        }

        private void DataAcessLayer_InitializeSQLiteCommands(object sender, InitializeSQLiteCommandsEventArgs e)
        {
            e.Commands.AddRange(Commands);
        }

        private static void DataAccessLayer_NeedDatabaseCreateCommand(object sender, NeedDatabaseConnectionCommandEventArgs e) =>
            e.CreateDatabase = new System.Data.SQLite.SQLiteCommand(Properties.Resources.dbo_CreateTimer);
    }
}