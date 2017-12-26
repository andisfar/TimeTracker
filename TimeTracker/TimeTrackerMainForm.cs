using System;
using System.Collections.Generic;
using System.Data;
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
        public bool UserAddedRow { get; private set; }        
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
            dal.FillDataTable(Timer);
            ConnectEventHandlers();
        }
        private void ConnectEventHandlers()
        {
            #region TimerDataGridView Events
            TimerDataGridView.DataError += TimerDataGridView_DataError;
            TimerDataGridView.CellEndEdit += TimerDataGridView_CellEndEdit;
            TimerDataGridView.UserAddedRow += TimerDataGridView_UserAddedRow;
            TimerDataGridView.UserDeletingRow += TimerDataGridView_UserDeletingRow;
            #endregion
            #region dal CommandBuilder DataAdapter Events
            dal.TimerCommandBuilder.DataAdapter.FillError += DataAdapter_FillError;
            dal.TimerCommandBuilder.DataAdapter.RowUpdated += DataAdapter_RowUpdated;
            dal.TimerCommandBuilder.DataAdapter.RowUpdating += DataAdapter_RowUpdating;
            #endregion
            #region dal CommandBuilder DataAdapter InsertCommand Connection Events
            dal.TimerCommandBuilder.DataAdapter.InsertCommand.Connection.Update += InsertConnection_Update;
            #endregion
            #region bindingNavigatorAddNewItem Events
            bindingNavigatorAddNewItem.Click += BindingNavigatorAddNewItem_Click;
            #endregion
        }

        private void TimerDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Log_Message($"Removing row at index {e.Row.Index}, with RowID of {e.Row.Cells[0].EditedFormattedValue}");
            int delete_index = Int32.Parse(e.Row.Cells[0].EditedFormattedValue.ToString());
            var row = Timer.Rows.Find(delete_index);
            Timer.Rows.Remove(row);
            Timer.AcceptChanges();
            Log_Message(Timer);
        }

        private void TimerDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            UserAddedRow = true;
            Log_Message(Timer);
        }

        private static void BindingNavigatorAddNewItem_Click(object sender, EventArgs e)
        {
            Log_Message("User CLicked Add Button!");
        }

        private static void InsertConnection_Update(object sender, System.Data.SQLite.UpdateEventArgs e)
        {
            Log_Message(e.Event.ToString());
        }

        private void TimerDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                if (TimerDataGridView.Rows[e.RowIndex].Cells[2].EditedFormattedValue.ToString() == string.Empty)
                {
                    TimerDataGridView.Rows[e.RowIndex].Cells[2].Value = "00:00:00";
                }
            }
            if(UserAddedRow)
            {
                var row = Timer.NewRow();
                using (DataGridViewRow dvrow = TimerDataGridView.Rows[e.RowIndex])
                {
                    row[1] = dvrow.Cells[1].EditedFormattedValue;
                    row[2] = dvrow.Cells[2].EditedFormattedValue;                    
                }                
                Timer.Rows.Add(row);
                UserAddedRow = false;
            }
            Log_Message($"(Row,\tColumn)");
            Log_Message($"({e.RowIndex},\t\t{e.ColumnIndex})");
            Log_Message($"Cell End Edit!");
            Log_Message(Timer);
        }

        private static void Log_Message(DataTable dataTable)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                Log_Message($"Id:\t[{row[0]}]\tName:\t[{row[1]}]\tElapsed:\t[{row[2]}]");
            }
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
            Log_Message($"{e.Exception.Message} : Number {e.Exception.HResult}");   
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

    public static class Extentions
    {
    }
}