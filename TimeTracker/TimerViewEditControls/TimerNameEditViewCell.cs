using System;
using System.Windows.Forms;

namespace TimeTracker.TimerViewEditControls
{
    public class TimerNameEditViewCell : DataGridViewTextBoxCell
    {
        public TimerNameEditViewCell() : base()
        { }

        public override object DefaultNewRowValue
        {
            get => "<Timer Name>";
        }

        public override Type EditType
        {
            get
            {
                return typeof(TimerNameEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                return typeof(string);
            }
        }

        public override void InitializeEditingControl(int rowIndex, object
        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            var ctl = DataGridView.EditingControl as TimerNameEditingControl;
            // Use the default row value when Value property is null.
            if (this.Value == null || typeof(System.DBNull) == this.Value.GetType())
            {
                ctl.Text = (string)this.DefaultNewRowValue;
            }
            else
            {
                ctl.Text = (string)this.Value;
            }
        }
    }
}