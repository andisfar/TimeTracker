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
            VerifyExistsDataBase(DataFile);
            Debug.Print(DataFile);
            TimersBS.DataSource = AppDataSource;
            TimerDataGridView.DataSource = TimersBS;
            TimerDataGridView.Columns.FormatColumns();
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
                    adapter.Fill(dt);
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
