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
        SQLiteDataAdapter timerDataAdapter = null;
        SQLiteCommandBuilder timerCommandBuilder = null;
        SQLiteConnection timerConnection = null;
        SQLiteCommand update_command = null;
        SQLiteCommand insert_command = null;
        SQLiteCommand delete_command = null;
        SQLiteCommand create_command = null;
        SQLiteCommand select_command = null;
        public DataTable AppDataSource
        {
            get => GetTimerTable();
        }
        public TimeTrackerMainForm()
        {
            InitializeComponent();
            string DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
            FileInfo dataFileInfo = new FileInfo(DataFile);
            VerifyCreated(dataFileInfo.DirectoryName);
            Debug.Assert(Directory.Exists(dataFileInfo.DirectoryName));
            VerifyExistsDataBase(DataFile);
            Debug.Print(DataFile);
            timerConnection = new SQLiteConnection(ConnectionString);

            update_command = new SQLiteCommand(Properties.Resources.update_command, timerConnection);
            insert_command = new SQLiteCommand(Properties.Resources.insert_command, timerConnection);
            delete_command = new SQLiteCommand(Properties.Resources.delete_command, timerConnection);
            create_command = new SQLiteCommand(Properties.Resources.dbo_CreateTimer, timerConnection);
            select_command = new SQLiteCommand(Properties.Resources.select_all_rows, timerConnection);

            timerDataAdapter = new SQLiteDataAdapter();

            timerDataAdapter.UpdateCommand = update_command;
            timerDataAdapter.DeleteCommand = delete_command;
            timerDataAdapter.InsertCommand = insert_command;
            timerCommandBuilder = new SQLiteCommandBuilder(timerDataAdapter);            

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
            Debug.Print(string.Format("Row with index {0} is about to be deleted!", e.Row.Cells[0].EditedFormattedValue.ToString()));
            int rowID = Int32.Parse(e.Row.Cells[0].EditedFormattedValue.ToString());
            timerDataAdapter.DeleteCommand.Connection.Open();

            SQLiteParameter idParameter = new SQLiteParameter("@Id", rowID);
            List<SQLiteParameter> paramList = new List<SQLiteParameter>();
            paramList.Add(idParameter);
            timerDataAdapter.DeleteCommand.Parameters.AddRange(paramList.ToArray());
            int rowsAffected = timerDataAdapter.DeleteCommand.ExecuteNonQuery();
            timerDataAdapter.DeleteCommand.Connection.Close();
            AppDataSource.AcceptChanges();
            Debug.Print(string.Format("Rows affected {0}", rowsAffected));
        }

        private void TimerDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Debug.Print(string.Format("Row {0} removed!", e.RowIndex));
        }
        private void TimerDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.ColumnIndex == 2 && TimerDataGridView.Rows[e.RowIndex].Cells[1].EditedFormattedValue.ToString() == string.Empty)
            {
                e.Cancel = true;
                return;
            }
        }
        private void TimerDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Debug.Print(string.Format("Cell end Edit occurred at ({0}, {1})", e.RowIndex, e.ColumnIndex)); 
        }
        private void TimerDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Debug.Print(string.Format("Rows added to DataGridView at index {0}", e.RowIndex));
        }
        private void TimerDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Debug.Print(string.Format("User added Row at View index {0}", e.Row.Index));            
        }
        private void SetupDataGridView()
        {
            TimerDataGridView.DataSource = TimersBS;
            TimerDataGridView.Columns.FormatColumns();
        }
        private void SetupDataSource(string DataFile)
        {
            try
            {
                TimersBS.DataSource = AppDataSource;
            }
            catch (TimerDataBaseInvalidException ex)
            {
                string newFolder = Application.CommonAppDataPath + @"\Invalid_Db\";

                Directory.CreateDirectory(newFolder);
                FileInfo dbInfo = new FileInfo(DataFile);
                string newFileName = dbInfo.Name;
                try
                {
                    File.Move(DataFile, newFolder + newFileName);
                }
                catch (System.IO.IOException)
                {
                    string[] files = Directory.GetFiles(newFolder, newFileName);
                    newFileName = string.Format("{0};{1}", dbInfo.Name, files.Length);
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
        private void VerifyExistsDataBase(string dataFile)
        {
            SQLiteConnection connection = null;
            string connection_string = string.Format(Properties.Resources.connection_string, dataFile);
            connection = new SQLiteConnection(connection_string, true);
            if (!File.Exists(dataFile))
            {
                string sql_create_text = Properties.Resources.dbo_CreateTimer;
                // open the database connection
                connection.Open();
                SQLiteCommand create_db_command = new SQLiteCommand(sql_create_text, connection);
                create_db_command.ExecuteNonQuery();
                // close the database connection
                connection.Close();
            }
            // if all is well the database exists and is populated with default info or alredy existed
        }
        public DataTable GetTimerTable()
        {
            // Connect to database.
            string connection_string = ConnectionString;
            string sqlCommand = Properties.Resources.select_all_rows;
            return TimerHelper.SelectAllCommand(select_command);
        }
        private static string ConnectionString
        {
            get
            {
                string DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
                string connection_string = string.Format(Properties.Resources.connection_string, DataFile);
                return connection_string;
            }
        }
        private void VerifyCreated(string directoryName)
        {
            if (Directory.Exists(directoryName)) return;
            DirectoryInfo di = new DirectoryInfo(directoryName);
            Queue<string> dirsNames = new Queue<string>();
            foreach(string part in directoryName.Split(Path.DirectorySeparatorChar))
            {
                Debug.Print(part);
                dirsNames.Enqueue(part);
            }
            string baseDir = dirsNames.Dequeue() + Path.DirectorySeparatorChar.ToString(); Debug.Print(baseDir);
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
                baseDir += Path.DirectorySeparatorChar.ToString() + dirsNames.Dequeue();
            } while (dirsNames.Count > 0);
            if (!Directory.Exists(baseDir)) Directory.CreateDirectory(baseDir);
        }
        private void BindingNavigatorSaveToDatabase_Click(object sender, EventArgs e)
        {
            SaveDataEventArgs saveData = new SaveDataEventArgs(SaveDataTarget.SQLiteTimerDatabase, UpdatedTable, timerDataAdapter);
            SaveData(this, saveData);
        }
        private DataTable UpdatedTable
        {
            get
            {
                DataTable dt = (DataTable)TimersBS.DataSource;
                dt.TableName = "Timer";
                dt.Columns[0].ColumnName = "Id";
                dt.Columns[1].ColumnName = "Name";
                dt.Columns[2].ColumnName = "Elapsed";
                return  dt;
            }
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
            DataGridView dgv = (DataGridView)sender;
            DataGridViewRowCollection rows = dgv.Rows;            
            e.Adapter.AcceptChangesDuringUpdate = true;
            e.Adapter.UpdateCommand.Connection.Open();
            foreach (DataGridViewRow row in rows)
            {
                if (row.IsNewRow) continue;
                List<SQLiteParameter> updates = new List<SQLiteParameter>();
                {
                    DataGridViewCell cell = row.Cells[0];
                    SQLiteParameter idParameter = new SQLiteParameter("@Id", cell.EditedFormattedValue.ToString());
                    cell = row.Cells[1];
                    SQLiteParameter nameParameter = new SQLiteParameter("@Name", cell.EditedFormattedValue.ToString());
                    cell = row.Cells[2];
                    SQLiteParameter elapsedParameter = new SQLiteParameter("@Elapsed", cell.EditedFormattedValue.ToString());
                    
                    updates.Add(idParameter);
                    updates.Add(nameParameter);
                    updates.Add(elapsedParameter);

                    e.Adapter.AcceptChangesDuringUpdate = true;
                    e.Adapter.UpdateCommand.Parameters.AddRange(updates.ToArray());
                    int updatedRow = e.Adapter.UpdateCommand.ExecuteNonQuery();
                    if (updatedRow == 0)
                    {
                        updatedRow = TimerHelper.InsertCommand(row, new SaveInsertDataEventArgs(dgv.NewRowIndex, SaveDataTarget.SQLiteTimerDatabase,
                                                                                  e.Table, e.Adapter));
                    }
                    Debug.Print(string.Format("Rows Affected = {0}", updatedRow));
                }
            }
            e.Adapter.Update(e.Table);
            e.Adapter.UpdateCommand.Connection.Close();
            e.Table.AcceptChanges();
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
            List<int> deleted = new List<int>();
            if (TimerDataGridView.SelectedRows.Count == 0) TimerDataGridView.CurrentRow.Selected = true;
            TimerDataGridView.Update();
            foreach (DataGridViewRow row in TimerDataGridView.SelectedRows)
            {
                string index = row.Cells[0].EditedFormattedValue.ToString();
                Debug.Print(string.Format("Row to be deleted = {0}", index));
                DeleteSingleRow(TimerDataGridView, new DataGridViewRowCancelEventArgs(row));
            }
            AppDataSource.AcceptChanges();
            TimerDataGridView.DataSource = AppDataSource;
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
        int _rowId = -1;
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
        private SaveDataTarget _target;
        public SaveDataTarget Target { get => _target; }

        private DataTable _table;
        public DataTable Table { get => _table; }

        private SQLiteDataAdapter _adapter;
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
            DataTable dt = new DataTable();
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
                    throw new TimerDataBaseInvalidException(command.Connection.DataSource,"Invalid database found, creating new database!");                        
                }
            }
            return dt;
        }

        public static int InsertCommand(object sender, SaveInsertDataEventArgs e)
        {
            Debug.Assert(sender.GetType() == typeof(DataGridViewRow));
            DataGridViewRow row = (DataGridViewRow)sender;
            bool IOpenedIt = (e.Adapter.InsertCommand.Connection.State == ConnectionState.Open);
            if(!IOpenedIt) e.Adapter.InsertCommand.Connection.Open();
            int result = 0;
            List<SQLiteParameter> inserts = new List<SQLiteParameter>();
            {                
                DataGridViewCell cell = row.Cells[1];

                SQLiteParameter nameParameter = new SQLiteParameter("@Name", cell.EditedFormattedValue == null ? string.Empty : cell.EditedFormattedValue.ToString());
                cell = row.Cells[2];
                SQLiteParameter elapsedParameter = new SQLiteParameter("@Elapsed", cell.EditedFormattedValue == null ? "00:00:00" : cell.EditedFormattedValue.ToString());

                //inserts.Add(idParameter);
                inserts.Add(nameParameter);
                inserts.Add(elapsedParameter);
                                
                e.Adapter.InsertCommand.Parameters.AddRange(inserts.ToArray());
                result = e.Adapter.InsertCommand.ExecuteNonQuery();

                Debug.Print(string.Format("Rows Affected = {0}", result));                
            }
            if(!IOpenedIt) e.Adapter.InsertCommand.Connection.Close();
            e.Table.AcceptChanges();
            return result;
        }
    }
    [Serializable]
    internal class TimerDataBaseInvalidException : Exception
    {
        string _invalid_data_file = string.Empty;
        public string InValidDataFile
        {
            get => _invalid_data_file;
        }

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
