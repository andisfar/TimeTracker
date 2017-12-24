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
            SetupDataSource(DataFile);
            SetupDataGridView();
            ConnectDataGridViewEvents();
        }


        private void SaveData(object sender, SaveDataEventArgs e)
        {
            switch (e.Target)
            {
                case SaveDataTarget.SQLiteTimerDatabase:
                    SaveToSQLiteTimerDatabase();
                    break;
                default:
                    {
                        break;
                    }

            }
        }

        private void SaveToSQLiteTimerDatabase(DataGridView dgv, string connection_string, string database_file)
        { 

            foreach (DataGridViewRow row in dgv.Rows)
            {
                using (SQLiteConnection connection = new SQLiteConnection(connection_string))
                {
                    using (SQLiteCommand insert_command = new SQLiteCommand("INSERT INTO [Timers] VALUES(@Name, @Elapsed)", connection))
                    {
                        insert_command.Parameters.AddWithValue("@Name", row.Cells["Id"].Value);
                        cmd.Parameters.AddWithValue("@Name", row.Cells["Name"].Value);
                        cmd.Parameters.AddWithValue("@Country", row.Cells["Country"].Value);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            MessageBox.Show("Records inserted.");
        }


        private void ConnectDataGridViewEvents()
        {
            TimerDataGridView.UserAddedRow += TimerDataGridView_UserAddedRow;
            TimerDataGridView.RowsAdded += TimerDataGridView_RowsAdded;
            TimerDataGridView.CellEndEdit += TimerDataGridView_CellEndEdit;
            TimerDataGridView.CellBeginEdit += TimerDataGridView_CellBeginEdit;
            TimerDataGridView.RowsRemoved += TimerDataGridView_RowsRemoved;
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
            string DataFile = Application.LocalUserAppDataPath + Properties.Settings.Default["DataFile"].ToString();
            string connection_string = string.Format(Properties.Resources.connection_string, DataFile);
            string sqlCommand = Properties.Resources.select_all_rows;            
            return TimerHelper.SelectAllCommand(sqlCommand, connection_string);
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
               
    }

    public enum SaveDataTarget
    {
        SQLiteTimerDatabase
    }

    [Serializable]
    public class SaveDataEventArgs : EventArgs
    {
        private SaveDataTarget _target;
        public SaveDataTarget Target { get => _target; }

        public SaveDataEventArgs()
        {
        }

        public SaveDataEventArgs(SaveDataTarget target)
        {
            _target = target;
        }
    }

    public class TimerHelper
    {
        public static DataTable SelectAllCommand(string select_all_text, string connection_string)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection connection = new SQLiteConnection(connection_string))
            {
                // Create database adapter using specified query
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(select_all_text, connection))
                // Create command builder to generate SQL update, insert and delete commands
                using (SQLiteCommandBuilder command = new SQLiteCommandBuilder(adapter))
                {
                    // Populate datatable to return, using the database adapter
                    try
                    {
                        adapter.Fill(dt);
                    }
                    catch (System.Data.SQLite.SQLiteException)
                    {

                        connection.Close();
                        File.Delete(connection.DataSource);
                        throw new TimerDataBaseInvalidException(connection.DataSource,"Invalid database found, creating new database!");                        
                    }
                }
                return dt;
            }
        }

        public static int InsertCommand(string insert_text, SQLiteParameter[] parameters, string connectionString)
        {
            int result = 0;
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand(insert_text, connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    result = cmd.ExecuteNonQuery();
                }
            }
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
