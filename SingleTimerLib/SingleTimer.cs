using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SingleTimerLib
{
    public enum InfoTypes
    {
        Default,
        TimerEvents
    }

    public enum TimerStates
    {
        Running,
        Stopped
    }

    public class SingleTimer : INotifyPropertyChanged, IDisposable
    {
        public delegate void TimerResetHandler(object sender, SingleTimerLibEventArgs e);

        public event TimerResetHandler TimerReset;

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void SingleTimerChangedHandler(object sender, SingleTimerLibEventArgs e);

        public delegate void SingleTimerNameChanging(object sender, SingleTimerNameChangingEventArgs e, [CallerMemberName] string caller="");

        public static void Log_Message(string debug_message)
        {
            Debug.Print(debug_message);
        }

        public event SingleTimerNameChanging NameChanging;

        public delegate void SingleTimerElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [CallerMemberName] string caller = "");
        public event SingleTimerElapsedTimeChanging ElapsedTimeChanging;

        public event SingleTimerChangedHandler SingleTimerChanged;

        private long _running_hours;
        private long _running_minutes;
        private long _running_seconds;

        private long _hours_offset;
        private long _minutes_offset;
        private long _seconds_offset;

        private Int32 _rowIndex;
        public Int32 RowIndex { get => _rowIndex; set { _rowIndex = value; OnPropertyChangedEventHandler(); } }

        private System.Timers.Timer heartBeat;
        private Stopwatch stopWatch;

        public Delegate[] @ElapsedTimeChangingInvocationList
        {
            get
            {
                var m = (MulticastDelegate)ElapsedTimeChanging;
                return m?.GetInvocationList();
            }
        }

        public Delegate[] @NameChangingInvocationList
        {
            get
            {
                var m = (MulticastDelegate)this.NameChanging;
                return m?.GetInvocationList();
            }
        }

        public Delegate[] @TimerResetInvocationList
        {
            get
            {

                var m = (MulticastDelegate)this.TimerReset;
                return m?.GetInvocationList();
            }
        }

        public void OnElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e)
        {
            ElapsedTimeChanging?.Invoke(this, e);
        }

        private void OnNameChanging(object sender, SingleTimerNameChangingEventArgs e)
        {
            NameChanging?.Invoke(sender, e);
        }

        private void OnResetTimer()
        {
            TimerReset?.Invoke(this, new SingleTimerLibEventArgs(this));
        }

        private void OnPropertyChangedEventHandler([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SingleTimerChanged?.Invoke(this, new SingleTimerLibEventArgs(this));
        }

        public SingleTimer(Int32 rowIndex, string name)
        {
            _name = name;
            _rowIndex = rowIndex;
            FinishInit("00:00:00");
        }

        public SingleTimer(Int32 rowIndex, string name, string elapsedTimeOffset, SingleTimerEventHandlers eventHandlers)
        {
            _name = name;
            _rowIndex = rowIndex;
            ElapsedTimeChanging += eventHandlers.ElapsedTimeChanging;
            NameChanging += eventHandlers.NameChaning;
            TimerReset += eventHandlers.ResetTimer;
            FinishInit(elapsedTimeOffset);
        }

        public SingleTimer(int rowId, string canonicalname, string timerOffset)
        {
            this.RowIndex = rowId;
            this.Name = canonicalname;
            FinishInit(timerOffset);
        }

        private void FinishInit(string elapsedTimeOffset)
        {
            OnPropertyChangedEventHandler(nameof(RowIndex));
            OnPropertyChangedEventHandler(nameof(Name));
            heartBeat = new System.Timers.Timer();
            stopWatch = new Stopwatch();

            heartBeat.Interval = 1000;
            heartBeat.Enabled = false;
            heartBeat.Elapsed += HeartBeat_Elapsed;
            stopWatch = new Stopwatch();
            ElapsedTimeOffset = elapsedTimeOffset;
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private void SingleTimerEditorForm_QueryTimerNeeded(object sender, SingleTimerEditorFormTimerNeededEventArgs e)
        {
            e.Timer = this;
        }

        public SingleTimer Instance { get => this; }

        private void SetElapsedTimeLabel()
        {
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
        }

        private void IncrementTime()
        {
            var runningTime = new TimeSpan((int)Hours_offset, (int)Minutes_offset, (int)Seconds_offset);
            runningTime += stopWatch.Elapsed;
            Running_seconds = runningTime.Seconds;
            Running_minutes = runningTime.Minutes;
            Running_hours = runningTime.Hours;
        }

        private string ElapsedTimeOffset
        {
            get
            {
                return $"{Hours_offset:00}:{Minutes_offset:00}:{Seconds_offset:00}";
            }

            set
            {
                if (value == string.Empty)
                {
                    Hours_offset = 0;
                    Minutes_offset = 0;
                    Seconds_offset = 0;
                }
                else
                {
                    var elapsedTime = value.Split(':');
                    Hours_offset = Int32.Parse(elapsedTime[0]);
                    Minutes_offset = Int32.Parse(elapsedTime[1]);
                    Seconds_offset = Int32.Parse(elapsedTime[2]);
                }
            }
        }

        public static string BlankTimerValue { get => $"{0:00}:{0:00}:{0:00}"; }

        public void StartOrStop()
        {
            if (IsRunning)
                StopTimer();
            else
                StartTimer();
        }

        private void HeartBeat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                heartBeat.Enabled = false;
                HandleTimerElapsed();
                heartBeat.Enabled = true;
            }
            catch (System.ObjectDisposedException)
            {
                return;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void HandleTimerElapsed()
        {
            IncrementTime();
            SetElapsedTimeLabel();
        }

        private string _name;
        public string Name
        {
            get { return IsRunning ? _name + "*" : _name; }
            set { OnNameChanging(this, new SingleTimerNameChangingEventArgs(value, this)); _name = value; OnPropertyChangedEventHandler(); }
        }

        public string RunningElapsedTime => $"{Running_hours:00}:{Running_minutes:00}:{Running_seconds:00}";

        public bool IsRunning
        {
            get => stopWatch.IsRunning;
        }

        public string CanonicalName => Name.Trim('*');

        public string MenuText { get => string.Format(CanonicalName + "-[{0}]",RunningElapsedTime); }

        public void SetElapsedTime(string runningElapsedTime)
        {
           var newTime = new TimeSpan(ParseHours(runningElapsedTime)[0], ParseHours(runningElapsedTime)[1], ParseHours(runningElapsedTime)[2]);
            Running_hours = newTime.Hours;
            Running_minutes = newTime.Minutes;
            Running_seconds = newTime.Seconds;
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
        }

        private static int[] ParseHours(string runningElapsedTime)
        {
            var retInts = new List<int>{0,0,0};
            var intStr = runningElapsedTime.Split(':');
            retInts[0] = Int32.Parse(intStr[0]);
            retInts[1] = Int32.Parse(intStr[1]);
            retInts[2] = Int32.Parse(intStr[2]);
            return retInts.ToArray();
        }

        public void ResetTimer()
        {
            try
            {
                Hours_offset = 0;
                Minutes_offset = 0;
                Seconds_offset = 0;

                if (IsRunning)
                {
                    stopWatch.Restart();
                }
                else
                {
                    stopWatch.Reset();
                }

                IncrementTime();
                SetElapsedTimeLabel();
                OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
                OnResetTimer();
            }
            catch (ObjectDisposedException)
            {
                throw;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public void Log_Message(InfoTypes showMe = InfoTypes.Default)
        {
            switch (showMe)
            {
                case InfoTypes.TimerEvents:
                    {
                        Log_Message($"Name  = {nameof(ElapsedTimeChanging)}");
                        try
                        {
                            foreach (Delegate @d in ElapsedTimeChangingInvocationList)
                            {
                                Log_Message($"Value = {d.GetMethodInfo().ReflectedType.Name}.{d.GetMethodInfo().Name}");
                            }
                        }
                        catch (Exception)
                        {
                            Log_Message($"Value = {"Not Set"}");
                        }

                        Log_Message($"Name  = {nameof(NameChanging)}");
                        try
                        {
                            foreach (Delegate @d in NameChangingInvocationList)
                            {
                                Log_Message($"Value = {d.GetMethodInfo().ReflectedType.Name}.{d.GetMethodInfo().Name}");
                            }
                        }
                        catch (Exception)
                        {
                            Log_Message($"Value = {"Not Set"}");
                        }

                        Log_Message($"Name  = {nameof(TimerReset)}");
                        try
                        {
                            foreach (Delegate @d in TimerResetInvocationList)
                            {
                                Log_Message($"Value = {d.GetMethodInfo().ReflectedType.Name}.{d.GetMethodInfo().Name}");
                            }
                        }
                        catch (NullReferenceException)
                        {
                            Log_Message($"Value = {"Not Set"}");
                        }
                        break;
                    }
                default:
                    {
                        Log_Message($"Timer Row Index: {RowIndex}, Timer State: {TimerState.ToString()}");
                        break;
                    }
            }
        }

        public TimerStates TimerState { get => stopWatch.IsRunning ? TimerStates.Running : TimerStates.Stopped; }
        public long Running_hours { get => _running_hours; set => _running_hours = value; }
        public long Running_minutes { get => _running_minutes; set => _running_minutes = value; }
        public long Running_seconds { get => _running_seconds; set => _running_seconds = value; }
        public long Hours_offset { get => _hours_offset; set => _hours_offset = value; }
        public long Minutes_offset { get => _minutes_offset; set => _minutes_offset = value; }
        public long Seconds_offset { get => _seconds_offset; set => _seconds_offset = value; }

        public void StopTimer()
        {
            if (stopWatch.IsRunning)
            {
                stopWatch.Stop();
                heartBeat.Enabled = false;
            }
            Log_Message($"'{CanonicalName}' is now stopped!");
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(IsRunning));
        }

        public void StartTimer()
        {
            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
                heartBeat.Enabled = true;
            }
            Log_Message($"'{CanonicalName}' is now running!");
            Log_Message(InfoTypes.TimerEvents);
            OnPropertyChangedEventHandler(nameof(RunningElapsedTime));
            OnPropertyChangedEventHandler(nameof(IsRunning));
            OnElapsedTimeChanging(this, new SingleTimerElapsedTimeChangingEventArgs(RunningElapsedTime, this));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //release any native resources here
            }
            StopTimer();
            heartBeat.Dispose();
        }

        public void Dispose()
        {
            Log_Message($"Timer '{CanonicalName}' is being disposed!");
            Dispose(true);
            GC.SuppressFinalize(this);
            return;
        }

        public void ReNameTimer(string name)
        {
            Name = name; ;
        }

        private static void Log_Message(string message, [CallerMemberName] string caller = "")
        {
            var messageWithTimeStamp = $"[{DateTime.Now.ToString(@"HH:mm:ss:fff")}]\t{caller} says {message}";
            Debug.Print(messageWithTimeStamp);
        }
    }

    public class SingleTimerElapsedTimeChangingEventArgs : EventArgs
    {
        private readonly SingleTimer _t;
        public SingleTimer Timer { get => _t; }

        private readonly string _elapsedTime = string.Empty;
        public string ElapsedTime { get => _elapsedTime; }

        public SingleTimerElapsedTimeChangingEventArgs(string elapsedTime, SingleTimer t, [CallerMemberName] string caller = "")
        {            
            _t = t;
            _elapsedTime = elapsedTime;
        }
    }

    public class SingleTimerNameChangingEventArgs : EventArgs
    {
        private readonly string _oldName = string.Empty;
        private readonly string _newName = string.Empty;

        public string OldName { get => _oldName; }
        public string NewName { get => _newName; }
        public SingleTimer Timer { get; private set; }

        public SingleTimerNameChangingEventArgs(string newName, SingleTimer t, [CallerMemberName] string caller = "")
        {
            _oldName = t.CanonicalName;
            _newName = newName;
            Timer = t;
        }
    }

    public class SingleTimerLibEventArgs : EventArgs
    {
        private readonly SingleTimer _t;
        public SingleTimer Timer { get => _t; }

        public SingleTimerLibEventArgs(SingleTimer t)
        {
            _t = t;
        }
    }
}
