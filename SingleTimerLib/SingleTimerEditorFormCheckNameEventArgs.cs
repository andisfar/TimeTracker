using System;

namespace SingleTimerLib
{
    public class SingleTimerEditorFormCheckNameEventArgs : EventArgs, IDisposable
    {
        private string name;
        public string Name { get => name; }
        public bool Exists { get; set; }

        public SingleTimerEditorFormCheckNameEventArgs(string name) : base()
        {
            this.name = name;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}