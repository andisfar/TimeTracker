using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SingleTimerLib;

namespace TimeTracker.TimerViewEditControls
{
    public class TimerElapsedEditingControl : TextBox, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        int rowIndex;
        private bool valueChanged = false;

        public DataGridView EditingControlDataGridView { get => dataGridView; set => dataGridView = value; }

        public object EditingControlFormattedValue { get => this.Text; set => this.Text = (string)value; }
        public int EditingControlRowIndex { get => rowIndex; set => rowIndex = value; }
        public bool EditingControlValueChanged { get => valueChanged; set { valueChanged = value; } }

        public Cursor EditingPanelCursor => base.Cursor;

        public bool RepositionEditingControlOnValueChange => false;

        public TimerElapsedEditingControl() : base()
        {
            this.TextChanged += TimerElapsedEditingControl_TextChanged;
        }

        private void TimerElapsedEditingControl_TextChanged(object sender, EventArgs e)
        {
            valueChanged = true;
            this.dataGridView.NotifyCurrentCellDirty(true);
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
