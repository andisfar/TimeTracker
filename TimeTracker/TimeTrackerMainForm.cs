using System.Collections.Generic;
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
<<<<<<< HEAD
                { "delete", Properties.Resources.delete_command },
                { "insert", Properties.Resources.insert_command }
=======
                { "delete", Properties.Resources.delete_command }
>>>>>>> Still Implementing DAL.  Debugging init problems
            };
            //
            dal = new DBAccess(DataFile, commands, new DBAccessEventHandlers
            {
                CommandInitHandler = DataAcessLayer_InitializeSQLiteCommands,
                CreateDBHandler = DataAccessLayer_NeedDatabaseCreateCommand,
                ConnectionStringHandler = DataAcessLayer_NeedConnectionString
            });
<<<<<<< HEAD
            //
            dal.FillDataTable(TimerDataSet.Tables["Timer"]);
=======
>>>>>>> Still Implementing DAL.  Debugging init problems
            ConnectEventHandlers();
        }
        private void ConnectEventHandlers()
        {
<<<<<<< HEAD
            TimerDataGridView.DataError += TimerDataGridView_DataError;
        }

        private void TimerDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
=======
>>>>>>> Still Implementing DAL.  Debugging init problems
        }

        private void DataAcessLayer_NeedConnectionString(object sender, NeedConnectionStringEventArgs e)
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