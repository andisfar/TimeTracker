using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public partial class SingleTimerUIForm : Form
    {
        int key = -1;
        private SingleTimerLib.SingleTimer _timer = null;
        public int RowIndex { get => key; set => key = value; }
        public SingleTimerUIForm(SingleTimer t)
        {
            InitializeComponent();
            this.key = t.RowIndex;
            Debug.Assert(t != null);
            _timer = t;
            RunTimerCheckBox.ImageKey = _timer.IsRunning ? "stop" : "play";
            RunTimerCheckBox.Checked = _timer.IsRunning;
            ThreadSafeUpdateOfTimerMenuText(_timer.MenuText);
            _timer.ElapsedTimeChanging += Timer_ElapsedTimeChanging;
        }
        
        private void ThreadSafeUpdateOfTimerMenuText(string menuText)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<string>(ThreadSafeUpdateOfTimerMenuText), menuText);
                return;
            }
            MenuTextLabel.Text = menuText;
            var time = menuText.Substring(menuText.IndexOf('[') + 1, 8);
            hours_progress.Value = Convert.ToInt32(time.Split(':')[0]);
            minutes_progress.Value = Convert.ToInt32(time.Split(':')[1]);
            seconds_progress.Value = Convert.ToInt32(time.Split(':')[2]);
        }

        private static void Log_Message(string message)
        {
            var messageWithTimeStamp = string.Format("{0}:\t{1}", DateTime.Now.ToString("HH:mm:ss:fff"), message);
            Debug.Print(messageWithTimeStamp);
        }


        private static void SingleTimerEditorForm_Load(object sender, EventArgs e)
        {            
            Application.DoEvents();
        }

        private void Timer_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            Log_Message(sender.ToString());
            Log_Message(caller);
            ThreadSafeUpdateOfTimerMenuText(e.Timer.MenuText);
        }

        private void ResetTimerbutton_Click(object sender, EventArgs e)
        {
            Application.DoEvents();
            RunTimerCheckBox.Checked = false;            
            CheckRunStopTimer();
            _timer.ResetTimer();            
        }

        private void RunTimerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckRunStopTimer();
        }

        public delegate void StartTimerRequestedHandler(object sender, SingleTimerEditorFormStartTimerEventArgs e);
        public event StartTimerRequestedHandler RequestStartTimer;

        private void OnRequestStartTimer(object sender, SingleTimerEditorFormStartTimerEventArgs e)
        {
            RequestStartTimer?.Invoke(sender, e);
        }

        public delegate void StopTimerRequestedHandler(object sender, SingleTimerEditorFormStopTimerEventArgs e);
        public event StopTimerRequestedHandler RequestStopTimer;

        private void OnRequestStopTimer(object sender, SingleTimerEditorFormStopTimerEventArgs e)
        {
            RequestStopTimer?.Invoke(sender, e);
        }

        private void CheckRunStopTimer()
        {
            UpdateRunStopImage();
            if (RunTimerCheckBox.Checked)
            {
                OnRequestStartTimer(this, new SingleTimerEditorFormStartTimerEventArgs(_timer.RowIndex));
            }
            else
            {
                OnRequestStopTimer(this, new SingleTimerEditorFormStopTimerEventArgs(_timer.RowIndex));
            }
        }

        private void UpdateRunStopImage()
        {
            RunTimerCheckBox.ImageKey = RunTimerCheckBox.Checked ? "stop" : "play";
        }

    }

    public class SingleTimerEditorFormStopTimerEventArgs
    {
        private int _rowIndex;
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormStopTimerEventArgs(int rowIndex)
        {
            _rowIndex = rowIndex;
        }
    }

    public class SingleTimerEditorFormStartTimerEventArgs
    {
        private int _rowIndex;
        public int RowIndex { get => _rowIndex; }

        public SingleTimerEditorFormStartTimerEventArgs(int rowIndex)
        {
            _rowIndex = rowIndex;
        }
    }

    public class SingleTimerEditorFormTimerNeededEventArgs : EventArgs
    {
        private SingleTimer _t = null;
        public SingleTimer Timer { get => _t; set => _t = value; }

        private bool _needNewTimer = false;
        public bool NewTimerNeeded { get => _needNewTimer;}

        private int _rowIndex = -1;
        public int RowIndex { get => _rowIndex; }

        private MouseButtons mouseButtonUsed;
        public MouseButtons MouseButtonUsed { get => mouseButtonUsed; }

        public SingleTimerEditorFormTimerNeededEventArgs(int rowIndex, bool needsNewTimer = false, MouseButtons mbUsed = MouseButtons.Left)
        {
            _rowIndex = rowIndex;
            _needNewTimer = needsNewTimer;
            mouseButtonUsed = mbUsed;
        }
    }

}
