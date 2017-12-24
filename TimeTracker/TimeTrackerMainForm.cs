using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace TimeTracker
{
    public partial class TimeTrackerMainForm : Form
    {
        SQLiteDataAdapter timerDataAdapter;
        SQLiteCommandBuilder timerCommandBuilder;
        SQLiteConnection timerConnection;
        SQLiteCommand update_command;
        SQLiteCommand insert_command;
        SQLiteCommand delete_command;
        SQLiteCommand create_command;
        SQLiteCommand select_command;
        public DataTable AppDataSource
        {
            get => GetTimerTable();
        }
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

            Update_command = new SQLiteCommand(Properties.Resources.update_command, TimerConnection);
            Insert_command = new SQLiteCommand(Properties.Resources.insert_command, TimerConnection);
            Delete_command = new SQLiteCommand(Properties.Resources.delete_command, TimerConnection);
            Create_command = new SQLiteCommand(Properties.Resources.dbo_CreateTimer, TimerConnection);
            Select_command = new SQLiteCommand(Properties.Resources.select_all_rows, TimerConnection);

            timerDataAdapter = new SQLiteDataAdapter
            {
                UpdateCommand = Update_command,
                DeleteCommand = Delete_command,
                InsertCommand = Insert_command
            };
            TimerCommandBuilder = new SQLiteCommandBuilder(timerDataAdapter);

            SetupDataSource(DataFile);
            SetupDataGridView();
            ConnectDataGridViewEvents();
        }
        private void ConnectDataGridViewEvents()
        {
            TimerDataGridView.UserAddedRow += TimerDataGridView_UserAddedRow;
            TimerDataGridView.RowsAdded += TimerDataGridView_RowsAdded;
            TimerDataGridView.CellEndEdit += TimerDataGridView_CellEndEdit;
            TimerDataGridView.CellBeginEdit += TimerDataGridView_CellBeginEdit;
            TimerDataGridView.RowsRemoved += TimerDataGridView_RowsRemoved;
            TimerDataGridView.UserDeletingRow += DeleteSingleRow;
        }
        private void DeleteSingleRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DeleteRow(e);
        }
        private void DeleteRow(DataGridViewRowCancelEventArgs e)
        {
            Debug.Print($"Row with index {e.Row.Cells[0].EditedFormattedValue.ToString()} is about to be deleted!");
            var rowID = Int32.Parse(e.Row.Cells[0].EditedFormattedValue.ToString());
            timerDataAdapter.DeleteCommand.Connection.Open();

            var idParameter = new SQLiteParameter("@Id", rowID);
            var paramList = new List<SQLiteParameter>
            {
                idParameter
            };
            timerDataAdapter.DeleteCommand.Parameters.AddRange(paramList.ToArray());
            var rowsAffected = timerDataAdapter.DeleteCommand.ExecuteNonQuery();
            timerDataAdapter.DeleteCommand.Connection.Close();
            AppDataSource.AcceptChanges();
            Debug.Print($"Rows affected {rowsAffected}");
        }
        private static void TimerDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Debug.Print($"Row {e.RowIndex} removed!");
        }
        private void TimerDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 2 && TimerDataGridView.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString() == string.Empty)
            {
                e.Cancel = true;
                return;
            }
        }
        private static void TimerDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print($"Cell end Edit occurred at ({e.RowIndex}, {e.ColumnIndex})");
        }
        private static void TimerDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Debug.Print($"Rows added to DataGridView at index {e.RowIndex}");
        }
        private static void TimerDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Debug.Print($"User added Row at View index {e.Row.Index}");
        }
        private void SetupDataGridView()
        {
            ((DataTable)TimersBS.DataSource).AcceptChanges();
            TimerDataGridView.DataSource = TimersBS;
            TimerDataGridView.Columns.FormatColumns();
            TimerDataGridView.Update();
        }
        private void SetupDataSource(string DataFile)
        {
            try
            {
                TimersBS.DataSource = AppDataSource;
            }
            catch (TimerDataBaseInvalidException)
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
                TimersBS.DataSource = AppDataSource;
            }
        }
        private static void VerifyExistsDataBase(string dataFile)
        {
            SQLiteConnection connection = null;
            var connection_string = string.Format(Properties.Resources.connection_string, dataFile);
            connection = new SQLiteConnection(connection_string, true);
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
        public DataTable GetTimerTable()
        {
            // Connect to database.
            var connection_string = ConnectionString;
            var sqlCommand = Properties.Resources.select_all_rows;
            return TimerHelper.SelectAllCommand(Select_command);
        }
        private static string ConnectionString
        {
            get
            {
                var DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
                var connection_string = string.Format(Properties.Resources.connection_string, DataFile);
                return connection_string;
            }
        }

        public SQLiteCommandBuilder TimerCommandBuilder { get => timerCommandBuilder; set => timerCommandBuilder = value; }
        public SQLiteConnection TimerConnection { get => timerConnection; set => timerConnection = value; }
        public SQLiteCommand Update_command { get => Update_command1; set => Update_command1 = value; }
        public SQLiteCommand Update_command1 { get => Update_command2; set => Update_command2 = value; }
        public SQLiteCommand Update_command2 { get => update_command; set => update_command = value; }
        public SQLiteCommand Insert_command { get => insert_command; set => insert_command = value; }
        public SQLiteCommand Delete_command { get => delete_command; set => delete_command = value; }
        public SQLiteCommand Create_command { get => create_command; set => create_command = value; }
        public SQLiteCommand Select_command { get => select_command; set => select_command = value; }

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
        private void BindingNavigatorSaveToDatabase_Click(object sender, EventArgs e)
        {
            var saveData = new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, AppDataSource, timerDataAdapter);
            SaveData(this, saveData);
        }
        private void SaveData(object sender, SaveDataEventArgs e)
        {
            switch (e.Target)
            {
                case SaveDataTarget.SQLiteTimerDatabase:
                    TimerDataGridView.EndEdit();
                    SaveToSQLiteTimerDatabase(TimerDataGridView, e);
                    break;
                default:
                    {
                        break;
                    }

            }
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
                if (updatedRow == 0)
                {
                    updatedRow = TimerHelper.InsertCommand(row, new SaveInsertDataEventArgs(dgv.NewRowIndex, SaveDataTarget.SQLiteTimerDatabase,
                                                                                e.Table, e.Adapter));
                }
                Debug.Print($"Rows Affected = {updatedRow}");
            }
            e.Adapter.Update(e.Table);
            e.Adapter.UpdateCommand.Connection.Close();
            e.Table.AcceptChanges();
            TimersBS.DataSource = AppDataSource;
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
            var deleted = new List<int>();
            if (TimerDataGridView.SelectedRows.Count == 0) TimerDataGridView.CurrentRow.Selected = true;
            TimerDataGridView.Update();
            foreach (DataGridViewRow row in TimerDataGridView.SelectedRows)
            {
                var index = row.Cells[0].EditedFormattedValue.ToString();
                Debug.Print($"Row to be deleted = {index}");
                DeleteSingleRow(TimerDataGridView, new DataGridViewRowCancelEventArgs(row));
            }
            AppDataSource.AcceptChanges();
            SetupDataGridView();
            TimerDataGridView.Update();
        }
    }
    public enum SaveDataTarget
    {
        SQLiteTimerDatabase
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
    }
    public class TimerHelper
    {
        public static DataTable SelectAllCommand(SQLiteCommand command)
        {
            var dt = new DataTable();
            // Create database adapter using specified query
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
            // Create command builder to generate SQL update, insert and delete commands
            using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter))
            {
                // Populate datatable to return, using the database adapter
                try
                {
                    adapter.Fill(dt);
                }
                catch (System.Data.SQLite.SQLiteException)
                {
                    File.Delete(command.Connection.DataSource);
                    throw new TimerDataBaseInvalidException(command.Connection.DataSource, "Invalid database found, creating new database!");
                }
            }
            dt.TableName = "Timer";
            dt.Columns[0].ColumnName = "Id";
            dt.Columns[1].ColumnName = "Name";
            dt.Columns[2].ColumnName = "Elapsed";
            return dt;
        }

        public static int InsertCommand(object sender, SaveInsertDataEventArgs e)
        {
            Debug.Assert(sender.GetType() == typeof(DataGridViewRow));
            var row = (DataGridViewRow)sender;
            var IOpenedIt = (e.Adapter.InsertCommand.Connection.State == ConnectionState.Open);
            if (!IOpenedIt) e.Adapter.InsertCommand.Connection.Open();
            var result = 0;
            var inserts = new List<SQLiteParameter>();
            {
                var cell = row.Cells[1];

                var nameParameter = new SQLiteParameter("@Name", cell.EditedFormattedValue == null ? string.Empty : cell.EditedFormattedValue.ToString());
                cell = row.Cells[2];
                var elapsedParameter = new SQLiteParameter("@Elapsed", cell.EditedFormattedValue == null ? "00:00:00" : cell.EditedFormattedValue.ToString());

                //inserts.Add(idParameter);
                inserts.Add(nameParameter);
                inserts.Add(elapsedParameter);

                e.Adapter.InsertCommand.Parameters.AddRange(inserts.ToArray());
                result = e.Adapter.InsertCommand.ExecuteNonQuery();

                Debug.Print($"Rows Affected = {result}");
            }
            if (!IOpenedIt) e.Adapter.InsertCommand.Connection.Close();
            e.Table.AcceptChanges();
            return result;
        }
    }
    [Serializable]
    internal class TimerDataBaseInvalidException : Exception
    {
        readonly string _invalid_data_file = string.Empty;

        public TimerDataBaseInvalidException()
        {
        }

        public TimerDataBaseInvalidException(string file_path, string message = "Invalid database found, creating new database!") : base(message)
        {
            _invalid_data_file = file_path;
        }

        public TimerDataBaseInvalidException(string file_path, string message = "Invalid database found, creating new database!", Exception innerException = null) : base(message, innerException)
        {
            _invalid_data_file = file_path;
        }

        protected TimerDataBaseInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public string InValidDataFile
        {
            get => _invalid_data_file;
        }
    }
    public static class Extentions
    {
        public static void FormatColumns(this DataGridViewColumnCollection me)
        {
            foreach (DataGridViewColumn column in me)
            {
                if (column.DataPropertyName.ToLower() == "id") // hide the primary key column
                {
                    column.Visible = false;
                    continue;
                }
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
    }
}
