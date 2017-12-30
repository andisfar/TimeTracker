using SingleTimerLib;
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
        public List<int> Deleted_Rows => _deleted_Rows;
        private readonly SingleTimersCollection _timers;
        private readonly DBAccess dal;
        public TimeTrackerMainForm()
        {
            InitializeComponent();
            _timers = new SingleTimersCollection(new SingleTimerEventHandlers
            {
                ElapsedTimeChanging = TimeTrackerMainForm_ElapsedTimeChanging,
                NameChaning = Timer_NameChanging,
                ResetTimer = Timer_TimerReset
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
            ConnectEventHandlers();
        }

private static void Timer_TimerReset(object sender, SingleTimerLibEventArgs e)
{
    //Timer.AcceptChanges();
}
private DialogResult EditTimer(SingleTimer t)
        {
            using (var editor = new SingleTimerEditorForm(t))
            {
                editor.RequestStartTimer += Editor_RequestStartTimer;
                editor.RequestStopTimer += Editor_RequestStopTimer;
                return editor.ShowDialog(this);
            }
        }
        private void Editor_CheckNameExists(object sender, SingleTimerEditorFormCheckNameEventArgs e)
        {
            e.Exists = dal.NameExists(e.Name, Properties.Resources.name_exists_command);
        }
        private void Editor_RequestStopTimer(object sender, SingleTimerEditorFormStopTimerEventArgs e)
        {
            _timers[e.RowIndex].Log_Message(InfoTypes.TimerEvents);
            _timers[e.RowIndex].StopTimer();
        }
        private void Editor_RequestStartTimer(object sender, SingleTimerEditorFormStartTimerEventArgs e)
        {
            _timers[e.RowIndex].Log_Message(InfoTypes.TimerEvents);
            _timers[e.RowIndex].StartTimer();
        }
        private void Editor_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            e.Timer = _timers[e.RowIndex];
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
            TimerDataGridView.CellMouseDown += TimerDataGridView_CellMouseDown;
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
            #region bindingNavigator Events
            bindingNavigatorDeleteItem.Click += BindingNavigatorDeleteItem_Click;
            bindingNavigatorSaveToDatabase.Click += BindingNavigatorSaveToDatabase_Click;
            #endregion
            #region MainForm Events
            FormClosing += TimeTrackerMainForm_FormClosing;
            #endregion
        }

        private void TimerDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                var key = GetTimerKey(e.RowIndex);
                ShowTimerUI(_timers[key]);
            }
        }

        private void ShowTimerUI(SingleTimer singleTimer)
        {
            EditTimer(singleTimer);
        }

        private int GetTimerKey(int rowIndex)
        {
            return Convert.ToInt32(TimerDataGridView.Rows[rowIndex].Cells[0].EditedFormattedValue);
        }

        private void Timer_NameChanging(object sender, SingleTimerNameChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            EnableSave();
            UpdateDataGridViewRow(e);
        }
        private void UpdateDataGridViewRow(SingleTimerNameChangingEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SingleTimerNameChangingEventArgs>(UpdateDataGridViewRow), e);
                return;
            }
            foreach (DataGridViewRow r in TimerDataGridView.Rows)
            {
                if (r.Cells[0].EditedFormattedValue.ToString() == e.Timer.RowIndex.ToString())
                {
                    if (r.Cells[1].EditedFormattedValue.ToString() != e.NewName)
                    {
                        r.Cells[1].Value = e.NewName;
                    }
                }
            }
            bindingNavigatorSaveToDatabase.PerformClick();
            Application.DoEvents();
        }
        private void TimeTrackerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timers.Dispose();
            SaveToDatabase();
        }
        private void TimeTrackerMainForm_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            UpdateDataGridViewRow(e);
        }
        private void UpdateDataGridViewRow(SingleTimerElapsedTimeChangingEventArgs e)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<SingleTimerElapsedTimeChangingEventArgs>(UpdateDataGridViewRow), e);
                return;
            }

            foreach (DataGridViewRow r in TimerDataGridView.Rows)
            {
                if (r.Cells[0].EditedFormattedValue.ToString() == e.Timer.RowIndex.ToString())
                {
                    r.Cells[2].Value = e.Timer.RunningElapsedTime;
                }
            }
            EnableSave();
            Application.DoEvents();
        }
        private static void Log_Message(DataRow row)
        {
            try
            {
                Log_Message($"State:[{row.RowState.ToString()}\tId:\t[{row[0]}]\tName:\t[{row[1]}]\tElapsed:\t[{row[2]}]");
            }
            catch (System.Data.DeletedRowInaccessibleException)
            {
                Log_Message($"State:[{row.RowState.ToString()}\tId:\t[{row[nameof(Id),DataRowVersion.Original]}]\tName:\t[{row[nameof(Name), DataRowVersion.Original]}]\tElapsed:\t[{row[nameof(Elapsed), DataRowVersion.Original]}]");
            }

        }
        private void BindingNavigatorSaveToDatabase_Click(object sender, EventArgs e)
        {
            SaveToDatabase();
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
            EnableSave();
        }
        private void EnableSave()
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action(EnableSave));
                return;
            }
            bindingNavigatorSaveToDatabase.Enabled = true;
        }
        private void Timer_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            EnableSave();
        }
        private void Timer_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            var rowID = Convert.ToInt32(e.Row[nameof(Id), DataRowVersion.Original].ToString());
            _timers[rowID].Dispose();
            _timers.RemoveAt(rowID);
            EnableSave();
        }
        private static void BindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
        }
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
            Deleted_Rows.Clear();
        }
        private void TimerDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if(this.InvokeRequired)
            {
                this.Invoke(new Action<object, DataGridViewRowCancelEventArgs>(TimerDataGridView_UserDeletingRow), sender, e);
                return;
            }
            Deleted_Rows.Add(e.Row.Key());
        }
        private void TimerDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            UserAddedRow = true;
        }

        private void Connection_StateChanged(object sender, StateChangeEventArgs e)
        {
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
            //if(UserAddedRow)
            //{
            //    var row = Timer.NewRow();
            //    using (DataGridViewRow dvrow = TimerDataGridView.Rows[e.RowIndex])
            //    {
            //        row[1] = dvrow.Cells[1].EditedFormattedValue;
            //        row[2] = dvrow.Cells[2].EditedFormattedValue;
            //    }
            //    Timer.Rows.Add(row);
            //    _timers.AddTimer(row);
            //    UserAddedRow = false;
            //}
        }
        private static void Log_Message(DataTable dataTable)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                try
                {
                    Log_Message(row);
                }
                catch (DeletedRowInaccessibleException)
                {
                    Log_Message("Unable to log info about deleted row!");
                    continue;
                }
            }
        }
        private static void DataAdapter_RowUpdating(object sender, System.Data.Common.RowUpdatingEventArgs e)
        {
        }
        private static void Log_Message(string message)
        {
            Debug.Print(message);
        }
        private static void DataAdapter_RowUpdated(object sender, System.Data.Common.RowUpdatedEventArgs e)
        {
            Log_Message(nameof(DataAdapter_RowUpdated));
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
    public static class Extentions
    {
        public static void SetCanoniicalName(this DataGridViewRow me, string newName)
        {
            me.Cells[1].Value = newName;
        }
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