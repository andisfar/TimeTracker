using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static TimeTracker.SaveDataEventArgs;

namespace TimeTracker
{
    public partial class TimeTrackerMainForm : Form
    {
        SQLiteDataAdapter timerDataAdapter;
        SQLiteCommandBuilder timerCommandBuilder;
        public SQLiteCommandBuilder TimerCommandBuilder { get => timerCommandBuilder; set => timerCommandBuilder = value; }
        SQLiteConnection timerConnection;
        public SQLiteConnection TimerConnection { get => timerConnection; set => timerConnection = value; }
        private static string ConnectionString
        {
            get
            {
                var DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
                var connection_string = string.Format(Properties.Resources.connection_string, DataFile);
                return connection_string;
            }
        }
        SQLiteCommand update_command;
        SQLiteCommand insert_command;
        SQLiteCommand delete_command;
        SQLiteCommand create_command;
        SQLiteCommand select_all_command;
        SQLiteCommand name_exists_command;

        public TimeTrackerMainForm()
        {
            InitializeComponent();
            var DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default[@"DataFile"].ToString();
            var dataFileInfo = new FileInfo(DataFile);
            VerifyCreated(dataFileInfo.DirectoryName);
            Debug.Assert(Directory.Exists(dataFileInfo.DirectoryName));
            VerifyExistsDataBase(DataFile);
            Debug.Print(DataFile);
            TimerConnection = new SQLiteConnection(ConnectionString);
            update_command = new SQLiteCommand(Properties.Resources.update_command, TimerConnection);
            insert_command = new SQLiteCommand(Properties.Resources.insert_command, TimerConnection);
            delete_command = new SQLiteCommand(Properties.Resources.delete_command, TimerConnection);
            create_command = new SQLiteCommand(Properties.Resources.dbo_CreateTimer, TimerConnection);
            select_all_command = new SQLiteCommand(Properties.Resources.select_all_rows, TimerConnection);
            name_exists_command = new SQLiteCommand(Properties.Resources.name_exists_command, TimerConnection);
            timerDataAdapter = new SQLiteDataAdapter
            {
                UpdateCommand = update_command,
                DeleteCommand = delete_command,
                InsertCommand = insert_command,
                SelectCommand = select_all_command
            };
            TimerCommandBuilder = new SQLiteCommandBuilder(timerDataAdapter);
            SetupDataSource(DataFile);
            ConnectEventHandlers();
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
        private void SetupDataSource(string DataFile)
        {
            try
            {
                SelectAllCommand();
            }
            catch (Exception)
            {
                var newFolder = Application.CommonAppDataPath + @"\Invalid_Db\";

                Directory.CreateDirectory(newFolder);
                var dbInfo = new FileInfo(DataFile);
                var newFileName = dbInfo.Name;
                try
                {
                    File.Move(DataFile, newFolder + newFileName);
                }
                catch (System.IO.IOException)
                {
                    var files = Directory.GetFiles(newFolder, newFileName);
                    newFileName = $"{dbInfo.Name};{files.Length}";
                }
                finally
                {
                    File.Move(DataFile, newFolder + newFileName);
                }
                Debug.Print("Invalid database file now residing @ " + newFolder + newFileName);
                VerifyExistsDataBase(DataFile);
            }
            finally
            {
                SelectAllCommand();
            }
        }
        DataTable SelectAllCommand()
        {
            try
            {
                SaveData(this, new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, TimerDataSet.Tables[0], timerDataAdapter));
                timerDataAdapter.Fill(TimerDataSet.Tables["Timer"]);
                return TimerDataSet.Tables["Timer"];
            }
            catch (System.Data.SQLite.SQLiteException)
            {
                File.Delete(timerDataAdapter.SelectCommand.Connection.DataSource);
                throw new Exception("Invalid database found, creating new database!");
            }
        }

        private void ConnectEventHandlers()
        {
            bindingNavigatorAddNewItem.Click += UserAddNewItem_Click;
            bindingNavigatorDeleteItem.Click += BindingNavigatorDeleteItem_Click;
            bindingNavigatorDeleteItem.MouseEnter += BindingNavigatorDeleteItem_MouseEnter;
            bindingNavigatorDeleteItem.MouseMove += BindingNavigatorDeleteItem_MouseMove;
            bindingNavigatorSaveToDatabase.Click += BindingNavigatorSaveToDatabase_Click;
            TimerDataGridView.CellEndEdit += TimerDataGridView_CellEndEdit;
            TimerDataGridView.CellMouseLeave += TimerDataGridView_CellMouseLeave;
            TimerDataGridView.CellValueChanged += TimerDataGridView_CellValueChanged;
            TimerDataGridView.DataError += TimerDataGridView_DataError;
        }

        private void TimerDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Debug.Print(string.Format(e.Exception.Message));
        }

        private void BindingNavigatorDeleteItem_MouseMove(object sender, MouseEventArgs e)
        {
            SetupDeleteSelection();
        }

        private void TimerDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            bindingNavigatorSaveToDatabase.Enabled = true;
        }

        private void TimerDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                var name = TimerDataGridView.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString();
                if (TimerHelper.ExistsName(name,name_exists_command))
                {
                    MessageBox.Show($"'{name}' already exists, choose another name!");
                    TimerDataGridView.CurrentCell = TimerDataGridView.CurrentRow.Cells[1];                    
                }
            }
            SaveData(this, new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, TimerDataSet.Tables["Timer"], timerDataAdapter));
        }

        private void UserAddNewItem_Click(object sender, EventArgs e)
        {
            try
            {
                AddRowToView();
            }
            catch (NameExistsException ex)
            {
               Debug.Print(ex.Message);
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void AddRowToView()
        {
            Debug.Print($"Count before {TimerDataSet.Tables[@"Timer"].Rows.Count}");
            var affectedRows = TimerHelper.InsertNewRowCommand(new SaveInsertDataEventArgs(TimerDataGridView.NewRowIndex, SaveDataEventArgs.SaveDataTarget.SQLiteTimerDatabase, TimerDataSet.Tables["Timer"], timerDataAdapter), name_exists_command);
            if (affectedRows > 0)
            {
                SelectAllCommand();
                TimerBN.MoveLastItem.PerformClick();
                TimerDataGridView.BeginEdit(true);
            }
            Debug.Print(string.Format("Count after {0}", TimerDataSet.Tables["Timer"].Rows.Count));           
        }

        private void BindingNavigatorSaveToDatabase_Click(object sender, EventArgs e)
        {
            var saveData = new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, TimerDataSet.Tables[@"Timer"], timerDataAdapter);
            SaveData(this, saveData);
            TimerDataSet.Tables["Timer"].AcceptChanges();
            timerDataAdapter.Update(TimerDataSet.Tables[@"Timer"]);
            SelectAllCommand();
        }
        private void TimerDataGridView_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            TimerDataGridView.EndEdit();
        }
        private void BindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedRows();
        }
        private void DeleteSelectedRows()
        {            
            foreach (DataGridViewRow row in TimerDataGridView.SelectedRows)
            {
                var index = row.Cells[0].EditedFormattedValue.ToString();
                Debug.Print($"Row to be deleted = {index}");
                DeleteSingleRow(TimerDataGridView, new DataGridViewRowCancelEventArgs(row));
            }
            SaveData(this, new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, TimerDataSet.Tables["Timer"], timerDataAdapter));
        }

        private void SaveData(object sender, SaveDataEventArgs e)
        {
            switch (e.Target)
            {
                case SaveDataTarget.SQLiteTimerDatabase:
                    TimerDataGridView.EndEdit();
                    break;
                default:
                    {
                        break;
                    }

            }
            bindingNavigatorSaveToDatabase.Enabled = false;
        }
        private void SaveToSQLiteTimerDatabase(object sender, SaveDataEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var rows = dgv.Rows;
            e.Adapter.AcceptChangesDuringUpdate = true;
            e.Adapter.UpdateCommand.Connection.Open();
            foreach (DataGridViewRow row in rows)
            {
                if (row.IsNewRow) continue;
                var cell = row.Cells[0];
                var idParameter = new SQLiteParameter("@Id", cell.EditedFormattedValue.ToString());
                cell = row.Cells[1];
                var nameParameter = new SQLiteParameter("@Name", cell.EditedFormattedValue.ToString());
                if (TimerHelper.ExistsName(nameParameter.Value.ToString(), name_exists_command))
                {
                    if(e.Adapter.UpdateCommand.Connection.State == ConnectionState.Open)
                    {
                        e.Adapter.UpdateCommand.Connection.Close();
                    }
                    return;
                }

                cell = row.Cells[2];
                var elapsedParameter = new SQLiteParameter("@Elapsed", cell.EditedFormattedValue.ToString());

                var updates = new List<SQLiteParameter>
                        {
                            idParameter,
                            nameParameter,
                            elapsedParameter
                        };

                e.Adapter.AcceptChangesDuringUpdate = true;
                e.Adapter.UpdateCommand.Parameters.AddRange(updates.ToArray());
                var updatedRow = e.Adapter.UpdateCommand.ExecuteNonQuery();
                Debug.Print($"Rows Affected = {updatedRow}");
            }
            e.Adapter.UpdateCommand.Connection.Close();
            
        }
        private void DeleteSingleRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Debug.Print($"Row with index {e.Row.Cells[0].EditedFormattedValue.ToString()} is about to be deleted!");
            var rowID = Int32.Parse(e.Row.Cells[0].EditedFormattedValue.ToString());
            timerDataAdapter.DeleteCommand.Connection.Open();

            var idParameter = new SQLiteParameter("@Id", rowID);

            timerDataAdapter.DeleteCommand.Parameters.Add(idParameter);
            var rowsAffected = timerDataAdapter.DeleteCommand.ExecuteNonQuery();
            timerDataAdapter.DeleteCommand.Connection.Close();
            TimerDataGridView.Rows.Remove(e.Row);
            Debug.Print($"Rows affected {rowsAffected}");
        }
        private void TimerDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 2 && TimerDataGridView.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString() == string.Empty)
            {
                e.Cancel = true;
                return;
            }
        }

        private void TimersBS_AddingNew(object sender, System.ComponentModel.AddingNewEventArgs e)
        {
            bindingNavigatorSaveToDatabase.Enabled = true;
            bindingNavigatorDeleteItem.Enabled = TimerDataGridView.Rows.Count > 0;
        }

        private void BindingNavigatorDeleteItem_MouseEnter(object sender, EventArgs e)
        {
            SetupDeleteSelection();
        }

        private void SetupDeleteSelection()
        {
            bindingNavigatorDeleteItem.Enabled = TimerDataGridView.Rows.Count > 0;
            if (!bindingNavigatorDeleteItem.Enabled) return;
            if (TimerDataGridView.SelectedRows.Count == 0) TimerDataGridView.CurrentRow.Selected = true;
        }
    }

    public static class TimerHelper
    {
        public static bool ExistsName(string name, SQLiteCommand name_exists)
        {
            var IOpenedIt = name_exists.Connection.State == ConnectionState.Open;
            if(!IOpenedIt)name_exists.Connection.Open();
            var nameParameter = new SQLiteParameter("@Name", name);
            name_exists.Parameters.Add(nameParameter);
            var rowsAffected = name_exists.ExecuteScalar();
            var exists =  Int32.Parse(rowsAffected.ToString()) == 1;
            if(!IOpenedIt)name_exists.Connection.Close();
            return exists;
        }

        public static int InsertNewRowCommand(SaveInsertDataEventArgs e, SQLiteCommand name_exists)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var IOpenedIt = (e.Adapter.InsertCommand.Connection.State == ConnectionState.Open);
            if (ExistsName("Timer Name", name_exists))
            {
                throw new NameExistsException($"'{"Timer Name"}' already exists!");
            }

            if (!IOpenedIt) e.Adapter.InsertCommand.Connection.Open();
            var result = 0;
            var nameParameter = new SQLiteParameter("@Name", "Timer Name");
            var elapsedParameter = new SQLiteParameter("@Elapsed", "00:00:00");
            var inserts = new List<SQLiteParameter>()
            {
                nameParameter,
                elapsedParameter
            };
            e.Adapter.InsertCommand.Parameters.AddRange(inserts.ToArray());
            result = e.Adapter.InsertCommand.ExecuteNonQuery();
            Debug.Print($"Rows Affected = {result}");
            if (!IOpenedIt) e.Adapter.InsertCommand.Connection.Close();
            return result;
        }
    }

    [Serializable]
    public class SaveInsertDataEventArgs : SaveDataEventArgs
    {
        readonly int _rowId = -1;
        public int RowID { get => _rowId; }

        public SaveInsertDataEventArgs() : base() { }
        public SaveInsertDataEventArgs(int rowID, SaveDataTarget target, DataTable table, SQLiteDataAdapter adapter) : base(target, table, adapter)
        {
            _rowId = rowID;
        }
    }
    [Serializable]
    public class SaveDataEventArgs : EventArgs
    {
        private readonly SaveDataTarget _target;
        public SaveDataTarget Target { get => _target; }

        private readonly DataTable _table;
        public DataTable Table { get => _table; }

        private readonly SQLiteDataAdapter _adapter;
        public SQLiteDataAdapter Adapter { get => _adapter; }

        public SaveDataEventArgs()
        {
        }

        public SaveDataEventArgs(SaveDataTarget target, DataTable table, SQLiteDataAdapter adapter)
        {
            _target = target;
            _table = table;
            _adapter = adapter;
        }

        public enum SaveDataTarget
        {
            SQLiteTimerDatabase
        }

        public class NameExistsException : Exception
        {
            private readonly string message;

            public NameExistsException(string message) : base(message){
                this.message = message;
            }
        }
    }
}


//    }


//    [Serializable]
//    internal class TimerDataBaseInvalidException : Exception
//    {
//        readonly string _invalid_data_file = string.Empty;

//        public TimerDataBaseInvalidException()
//        {
//        }

//        public TimerDataBaseInvalidException(string file_path, string message = "Invalid database found, creating new database!") : base(message)
//        {
//            _invalid_data_file = file_path;
//        }

//        public TimerDataBaseInvalidException(string file_path, string message = "Invalid database found, creating new database!", Exception innerException = null) : base(message, innerException)
//        {
//            _invalid_data_file = file_path;
//        }

//        protected TimerDataBaseInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
//        {
//        }
//        public string InValidDataFile
//        {
//            get => _invalid_data_file;
//        }
//    }
//    public static class Extentions
//    {
//        public static void FormatColumns(this DataGridViewColumnCollection me)
//        {
//            foreach (DataGridViewColumn column in me)
//            {
//                if (column.DataPropertyName.ToLower() == "id") // hide the primary key column
//                {
//                    column.Visible = false;
//                    continue;
//                }
//                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
//            }
//        }
//    }
//}
