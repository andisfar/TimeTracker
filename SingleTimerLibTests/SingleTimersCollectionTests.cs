using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleTimerLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerDataAccessLayer;

namespace SingleTimerLib.Tests
{
    [TestClass()]
    public class SingleTimersCollectionTests : IDisposable
    {
        public static SingleTimerEventHandlers eventHandlers = new SingleTimerEventHandlers
        {
            ElapsedTimeChanging = Timer_ElapsedTimeChanging,
            NameChaning = Timer_NameChanging,
            ResetTimer = Timer_TimerReset
        };

        public SingleTimersCollection collection = new SingleTimersCollection(eventHandlers);

        [TestMethod()]
        public void SingleTimersCollectionTest()
        {
            Assert.IsInstanceOfType(collection, typeof(SingleTimersCollection));
        }

        private static void Timer_TimerReset(object sender, SingleTimerLibEventArgs e)
        {
            if (e.Timer != null) SingleTimer.Log_Message($"{sender.ToString()} with - {e.Timer.RunningElapsedTime}");
        }

        private static void Timer_NameChanging(object sender, SingleTimerNameChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            if (e.Timer != null) SingleTimer.Log_Message($"{sender.ToString()}/{caller} - Old {e.OldName} New {e.NewName}");
        }

        private static void Timer_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            if (e.Timer != null) SingleTimer.Log_Message($"{sender.ToString()}/{caller} with - {e.Timer.RunningElapsedTime}");
        }

        [TestMethod()]
        public void AddTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            using (var singleTimer = new SingleTimer(232, "Test Timer"))
            {
                collection.Add(singleTimer.RowIndex, singleTimer);
            }
            Assert.IsTrue(collection.Count == 1);
            Assert.IsNotNull(collection[232]);
            SingleTimer.Log_Message(collection[232].CanonicalName);
            collection.Dispose();          
        }

        [TestMethod()]
        public void AddTimerTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232,"Test Timer", "00:00:00");
            Assert.IsTrue(collection.Count == 1);
            Assert.IsNotNull(collection[232]);
            SingleTimer.Log_Message(collection[232].CanonicalName);
            collection.Dispose();
        }

        [TestMethod()]
        public void AddTest1()
        {
            collection.Clear();
            Assert.IsTrue(collection.Count == 0);
            var item = new KeyValuePair<int, SingleTimer>(0, new SingleTimer(0, "Test Timer"));
            collection.Add(item);
            Assert.IsTrue(collection.Count == 1);
            collection.Dispose();
        }

        [TestMethod()]
        public void ClearTest()
        {
            collection.Clear();
            Assert.IsTrue(collection.Count == 0);
            for(int i = 0; i < 100;++i)
            {
                collection.AddTimer(i + 1, $"Timer{i + 1}","00:00:00");
            }
            Assert.IsTrue(collection.Count == 100);
            collection.Clear();
            Assert.IsTrue(collection.Count == 0);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            collection.Clear();
            collection.AddTimer(231, "cannonical name", "04:43:23");
            using (SingleTimer t = collection[231])
            {
                Assert.IsNotNull(t);
                Assert.IsTrue(collection.Contains(new KeyValuePair<int, SingleTimer>(231, t)));
            }
        }

        [TestMethod()]
        public void ContainsKeyTest()
        {
            collection.AddTimer(231, "cannonical name", "04:43:23");
            Assert.IsTrue(collection.ContainsKey(231));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            collection.Clear();
            var list = new List<KeyValuePair<int, SingleTimer>>();
            Assert.IsTrue(collection.Count == 0);
            for (int i = 0; i < 100; ++i)
            {
                collection.AddTimer(i + 1, $"Timer{i + 1}", "00:00:00");
                if(i<11)list.Add(new KeyValuePair<int, SingleTimer>(i+1,null));
            }
            Assert.IsTrue(collection.Count == 100);
            var array = list.ToArray();
            array.Initialize();
            collection.CopyTo(array, 0);
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer1", "00:45:45");
            collection.AddTimer(233, "Test Timer2", "00:45:45");
            collection.AddTimer(234, "Test Timer3", "00:45:45");
            collection.AddTimer(235, "Test Timer4", "00:45:45");
            collection.AddTimer(236, "Test Timer5", "00:45:45");
            collection.AddTimer(237, "Test Timer6", "00:45:45");
            Assert.IsTrue(collection.Count == 6);
            IEnumerator singleTimerEnumerator = collection.GetEnumerator();
        }

        [TestMethod()]
        public void RemoveTest()
        {
            InitCollection(); 
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer", "00:45:45");
            Assert.IsTrue(collection.Count == 1);
            collection.Remove(232);
            Assert.IsTrue(collection.Count == 0);
        }

        [TestMethod()]
        public void RemoveTest1()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer", "00:45:45");
            Assert.IsTrue(collection.Count == 1);
            collection.TryGetValue(232, out SingleTimer timer);
            var item = new KeyValuePair<int, SingleTimer>(232, timer);
            collection.Remove(item);
            Assert.IsTrue(collection.Count == 0);
        }

        [TestMethod()]
        public void TryGetValueTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer", "00:45:45");            
            Assert.IsTrue(collection.Count == 1);
            collection.TryGetValue(232, out SingleTimer timer);
            Assert.IsInstanceOfType(timer, typeof(SingleTimer));
            timer.ReNameTimer("new timer name");
            timer.StartOrStop();
            timer.Dispose();
        }

        private void InitCollection()
        {
            eventHandlers = new SingleTimerEventHandlers
            {
                NameChaning = Timer_NameChanging,
                ElapsedTimeChanging = Timer_ElapsedTimeChanging,
                ResetTimer = Timer_TimerReset
            };

            collection = new SingleTimersCollection(eventHandlers);           
        }

        [TestMethod()]
        public void DisposeTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer", "00:45:45");
            Assert.IsTrue(collection.Count == 1);
            Dispose();
        }

        [TestMethod()]
        public void AddTimerTest1()
        {
            var DataFile = @"Test_Database.db";
            var dataFileInfo = new FileInfo(DataFile);            
            DataTable dt;
            dt = CreateTimerTable();
            //
            var Commands = new Dictionary<string, string>
            {
                { "update", @"UPDATE [Timer] SET Name = @Name, Elapsed = @Elapsed WHERE Id = @Id;" },
                { "select", @"SELECT id, Name, Elapsed FROM [Timer];" },
                { "delete", @"DELETE FROM [Timer] WHERE Id = @Id;" },
                { "insert", @"insert into [Timer] values(null, @Name, @Elapsed);" }
            };
            //
            var dal = new DBAccess(DataFile, Commands, new DBAccessEventHandlers
            {
                CommandInitHandler = DataAcessLayer_InitializeSQLiteCommands,
                CreateDBHandler = DataAccessLayer_NeedDatabaseCreateCommand,
                ConnectionStringHandler = DataAcessLayer_NeedConnectionString
            });
            Assert.IsTrue(File.Exists(DataFile));
            Debug.Print($"{dataFileInfo.FullName} exists!");
            //
            int affected = dal.FillDataTable(dt);
            Assert.IsTrue(affected > 0);
            Debug.Print($"Table got filled! {affected} rows affected!");
            var dr = dt.NewRow();
            dr[0] = 232;
            dr[1] = "Timer Test";
            dr[2] = "00:00:00";

            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(dr);
            Assert.IsTrue(collection.Count == 1);
            Assert.IsNotNull(collection[232]);
            SingleTimer.Log_Message(collection[232].CanonicalName);
            collection.Dispose();
            Assert.IsTrue(collection.Count == 0);
        }

        private static DataTable CreateTimerTable()
        {
            DataTable dt;
            using (DataTable _dt = new DataTable("Timer"))
            {
                dt = _dt;
                DataColumn dc;
                using (DataColumn _dc = new DataColumn("id", typeof(int)))
                {
                    dc = _dc;
                }
                dt.Columns.Add(dc);
                using (DataColumn _dc = new DataColumn("Name", typeof(string)))
                {
                    dc = _dc;
                }
                dt.Columns.Add(dc);
                using (DataColumn _dc = new DataColumn("Elapsed", typeof(string)))
                {
                    dc = _dc;
                }
                dt.Columns.Add(dc);
                dc = null;
            }

            return dt;
        }

        private static void DataAcessLayer_NeedConnectionString(object sender, NeedConnectionStringEventArgs e)
        {
            e.ConnectionString = $"Data Source=Test_Database.db; Version = 3;";
        }

        private static void DataAccessLayer_NeedDatabaseCreateCommand(object sender, NeedDatabaseConnectionCommandEventArgs e)
        {
            var bldr = new StringBuilder();
            bldr.AppendLine(@"--Script Date: 12 / 23 / 2017 9:30 AM - ErikEJ.SqlCeScripting version 3.5.2.74');");
            bldr.AppendLine(@"DROP TABLE IF EXISTS[Timer];");
            bldr.AppendLine(@"CREATE TABLE[Timer] (");

            bldr.AppendLine(@"[Id] INTEGER PRIMARY KEY AUTOINCREMENT");
            bldr.AppendLine(@", [Name] TEXT NULL UNIQUE");
            bldr.AppendLine(@", [Elapsed] TEXT DEFAULT '00:00:00' NULL");
            bldr.AppendLine(@");");

            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'One','00:01:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Two','00:02:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Three','00:03:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Four','00:04:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Five','00:05:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Six','00:06:00');");
            bldr.AppendLine(@"INSERT INTO[Timer] VALUES(null,'Seven','00:07:00');");

            e.CreateDatabase = new SQLiteCommand(bldr.ToString());
        }

        private static void DataAcessLayer_InitializeSQLiteCommands(object sender, InitializeSQLiteCommandsEventArgs e)
        {
            var Commands = new Dictionary<string, string>
            {
                { "update", @"UPDATE [Timer] SET Name = @Name, Elapsed = @Elapsed WHERE Id = @Id;" },
                { "select", @"SELECT id, Name, Elapsed FROM [Timer];" },
                { "delete", @"DELETE FROM [Timer] WHERE Id = @Id;" },
                { "insert", @"insert into [Timer] values(null, @Name, @Elapsed);" }
            };
            e.Commands.AddRange(Commands);
        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            InitCollection();
            Assert.IsTrue(collection.Count == 0);
            collection.AddTimer(232, "Test Timer", "00:45:45");
            Assert.IsTrue(collection.Count == 1);
            collection.RemoveAt(232);
            Assert.IsTrue(collection.Count == 0);
        }

        public void Dispose()
        {
            collection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}