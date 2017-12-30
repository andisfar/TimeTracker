using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SingleTimerLib;

namespace TimeTracker.TimerViewEditControls
{
    public class TimerNameEditingControl : TextBox, IDataGridViewEditingControl
    {

        DataGridView dataGridView;
        int rowIndex;
        private bool NameValueChanged = false;

        public DataGridView EditingControlDataGridView { get => dataGridView; set => dataGridView = value; }

        public object EditingControlFormattedValue { get => this.Text; set => this.Text = (string)value; }
        public int EditingControlRowIndex { get => rowIndex; set => rowIndex = value; }
        public bool EditingControlValueChanged { get => NameValueChanged; set { NameValueChanged = value; } }

        public Cursor EditingPanelCursor => base.Cursor;

        public bool RepositionEditingControlOnValueChange => true;

        public TimerNameEditingControl() : base()
        {
            this.TextChanged += TimerNameEditingControl_TextChanged;
        }

        private void TimerNameEditingControl_TextChanged(object sender, EventArgs e)
        {
            NameValueChanged = true;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            //;
        }
    }
}
