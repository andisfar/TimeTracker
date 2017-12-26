using SingleTimerLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using TimeTrackerDataAccessLayer;
using static SingleTimerLib.SingleTimer;

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

        public List<int> Deleted_Rows => _deleted_Rows;

        private readonly SingleTimersCollection _timers;
        private readonly DBAccess dal;
        public TimeTrackerMainForm()
        {
            InitializeComponent();
             _timers = new SingleTimersCollection(new SingleTimerEventHandlers
             {
                 ElapsedTimeChanging = TimeTrackerMainForm_ElapsedTimeChanging
             });
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
            ConnectionStatusButton.Image = ConnectionStateImageList.Images["Closed"];
            DBAccess.FillTimersCollection(ref _timers, Timer);
            foreach(SingleTimer t in _timers.Values)
            {
                t.DebugPrint(InfoTypes.TimerEvents);
                t.StartOrStop();
            }
            ConnectEventHandlers();
        }
        private void ConnectEventHandlers()
        {
            #region Timer Data Table Events
            Timer.RowDeleted += Timer_RowDeleted;
            Timer.RowChanged += Timer_RowChanged;
            Timer.TableNewRow += Timer_TableNewRow;
            #endregion
            #region TimerDataGridView Events
            TimerDataGridView.DataError += TimerDataGridView_DataError;
            TimerDataGridView.CellEndEdit += TimerDataGridView_CellEndEdit;
            TimerDataGridView.UserAddedRow += TimerDataGridView_UserAddedRow;
            TimerDataGridView.UserDeletingRow += TimerDataGridView_UserDeletingRow;
            TimerDataGridView.UserDeletedRow += TimerDataGridView_UserDeletedRow;
            #endregion
            #region dal CommandBuilder DataAdapter Events
            dal.TimerCommandBuilder.DataAdapter.FillError += DataAdapter_FillError;
            dal.TimerCommandBuilder.DataAdapter.RowUpdated += DataAdapter_RowUpdated;
            dal.TimerCommandBuilder.DataAdapter.RowUpdating += DataAdapter_RowUpdating;
            #endregion
            #region dal CommandBuilder DataAdapter Connection Events
            dal.TimerCommandBuilder.DataAdapter.InsertCommand.Connection.StateChange += Connection_StateChanged;
            dal.TimerCommandBuilder.DataAdapter.UpdateCommand.Connection.StateChange += Connection_StateChanged;
            dal.TimerCommandBuilder.DataAdapter.DeleteCommand.Connection.StateChange += Connection_StateChanged;
            dal.TimerCommandBuilder.DataAdapter.SelectCommand.Connection.StateChange += Connection_StateChanged;
            #endregion
            #region bindingNavigatorAddNewItem Events
            bindingNavigatorAddNewItem.Click += BindingNavigatorAddNewItem_Click;
            bindingNavigatorDeleteItem.Click += BindingNavigatorDeleteItem_Click;
            #endregion
            #region bindingNavigatorSaveToDatabase Events
            bindingNavigatorSaveToDatabase.Click += BindingNavigatorSaveToDatabase_Click;
            #endregion
            #region MainForm Events
            FormClosing += TimeTrackerMainForm_FormClosing;
            #endregion
        }

        private void TimeTrackerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timers.Dispose();
            SaveToDatabase();
        }

        private void TimeTrackerMainForm_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            Log_Message($"Timer with Elapsed Time Value {e.ElapsedTime}");
            Log_Message($"Timer with row index of {e.Timer.RowIndex}");
            Log_Message($"Timer with name of {e.Timer.CanonicalName}");
            Log_Message($"Timer with menu text {e.Timer.MenuText}");
            e.Timer.DebugPrint(InfoTypes.TimerEvents);
            UpdateDataGridViewRow(e.Timer);            
        }

        private void UpdateDataGridViewRow(SingleTimer t)
        {            
            if(InvokeRequired)
            {
                Invoke(new Action<SingleTimer>(UpdateDataGridViewRow), t);
                return;
            }

            foreach (DataGridViewRow r in TimerDataGridView.Rows)
            {
                if (r.Cells[0].EditedFormattedValue.ToString() == t.RowIndex.ToString())
                {
                    r.Cells[2].Value = t.RunningElapsedTime;
                }
            }
            bindingNavigatorSaveToDatabase.PerformClick();
            Application.DoEvents();
        }

        private static void Log_Message(DataRow row)
        {
            Log_Message($"State:[{row.RowState.ToString()}\tId:\t[{row[0]}]\tName:\t[{row[1]}]\tElapsed:\t[{row[2]}]");
        }

        private void BindingNavigatorSaveToDatabase_Click(object sender, EventArgs e)
        {
            SaveToDatabase();
            Log_Message(Timer);
        }

        private void SaveToDatabase()
        {
            dal.SaveToDataBase(Timer);
            DisableSave();
        }

        private void DisableSave()
        {
            bindingNavigatorSaveToDatabase.Enabled = false;
        }

        private void Timer_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            Log_Message("TableNewRow");
            Log_Message(Timer);
            EnableSave();
        }
        private void EnableSave()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(EnableSave));
                Log_Message($"Invoke from owning thread!");
                return;
            }
            bindingNavigatorSaveToDatabase.Enabled = true;
        }
        private void Timer_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            Log_Message("RowChanged");
            Log_Message(Timer);
            EnableSave();
        }
        private void Timer_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            Log_Message("RowDeleted");
            Log_Message(Timer);
            EnableSave();
        }
        private void BindingNavigatorDeleteItem_Click(object sender, EventArgs e) => Log_Message(Timer);
        readonly List<int> _deleted_Rows = new List<int>();
        private void TimerDataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            foreach(int key in Deleted_Rows)
            {
                var row = Timer.Rows.Find(key);
                if (row != null)
                {
                    row.Delete();
                }
            }
            Log_Message(Timer);
            Deleted_Rows.Clear();
        }
        private void TimerDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<object, DataGridViewRowCancelEventArgs>(TimerDataGridView_UserDeletingRow), sender, e);
                Log_Message("Invoking on owning thread!");
                return;
            }
            Log_Message($"Removing row at index {e.Row.Index}, with RowID of {e.Row.Key()}");
            Deleted_Rows.Add(e.Row.Key());
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
        private void Connection_StateChanged(object sender, StateChangeEventArgs e)
        {
            Log_Message($"Connection currently '{e.CurrentState.ToString()}'");
            switch(e.CurrentState)
            {
                case ConnectionState.Connecting:
                case ConnectionState.Executing:
                case ConnectionState.Fetching:
                case ConnectionState.Open:
                    {
                        ConnectionStatusButton.Image = ConnectionStateImageList.Images["Open"];
                        break;
                    }
                case ConnectionState.Closed:
                    {
                        ConnectionStatusButton.Image = ConnectionStateImageList.Images["Closed"];
                        break;
                    }
                default:
                    {
                        ConnectionStatusButton.Image = ConnectionStateImageList.Images["Broken"];
                        break;
                    }
            }
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
                TimerDataGridView.Refresh();
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
                try
                {
                    Log_Message(row);
                }
                catch (DeletedRowInaccessibleException ex)
                {
                    Log_Message("Unable to log info about deleted row!");
                    continue;
                }
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
        public static void SetTimerElapsedTime(this DataGridViewRow me, string RunningElapsedTime)
        {
            me.Cells[2].Value = RunningElapsedTime;
        }
        public static int Key(this DataGridViewRow me)
        {
            return Convert.ToInt32(me.Cells[0].EditedFormattedValue.ToString());
        }
        public static string ToString(this DataGridViewCell me)
        {
            return me.EditedFormattedValue.ToString();
        }
    }
}