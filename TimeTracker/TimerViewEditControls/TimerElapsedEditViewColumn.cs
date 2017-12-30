using System;
using System.Windows.Forms;

namespace TimeTracker.TimerViewEditControls
{
    public class TimerElapsedEditViewColumn : DataGridViewColumn
    {
        public TimerElapsedEditViewColumn() : base(new TimerElapsedEditViewCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(TimerElapsedEditViewCell)))
                {
                    throw new InvalidCastException("Must be a TimerElapsedEditViewCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}
