using System;
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
        public TimeTrackerMainForm()
        {
            InitializeComponent();
            string DataFile = Path.Combine(Application.LocalUserAppDataPath,
                Properties.Settings.Default["DataFile"].ToString());
            FileInfo dataFileInfo = new FileInfo(DataFile);
            VerifyCreated(dataFileInfo.DirectoryName);
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
}
