using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d_TimeTrackerDataSet
{
    public class TimeTrackerDataSet : DataSet
    {
        public TimeTrackerDataSet(string dataFile)
        {
            if(!File.Exists(dataFile))
            {
                SQLiteConnection.CreateFile(dataFile);
            }
        }
    }
}
