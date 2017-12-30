using System;
using System.Windows.Forms;

namespace TimeTracker.TimerViewEditControls
{
    public class TimerNameEditViewColumn : DataGridViewColumn
    {
        public TimerNameEditViewColumn() : base(new TimerNameEditViewCell())
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
                    !value.GetType().IsAssignableFrom(typeof(TimerNameEditViewCell)))
                {
                    throw new InvalidCastException("Must be a TimerNameEditViewCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}
