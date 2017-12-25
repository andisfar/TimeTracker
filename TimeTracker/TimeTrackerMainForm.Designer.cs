namespace TimeTracker
{
    partial class TimeTrackerMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeTrackerMainForm));
            this.TimerDataGridView = new System.Windows.Forms.DataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.elapsedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimersBS = new System.Windows.Forms.BindingSource(this.components);
            this.TimerDataSet = new System.Data.DataSet();
            this.Timer = new System.Data.DataTable();
            this.Id = new System.Data.DataColumn();
            this.TimerName = new System.Data.DataColumn();
            this.Elapsed = new System.Data.DataColumn();
            this.TimerBN = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSaveToDatabase = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.TimerDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimersBS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Timer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerBN)).BeginInit();
            this.TimerBN.SuspendLayout();
            this.SuspendLayout();
            // 
            // TimerDataGridView
            // 
            this.TimerDataGridView.AllowUserToAddRows = false;
            this.TimerDataGridView.AllowUserToDeleteRows = false;
            this.TimerDataGridView.AutoGenerateColumns = false;
            this.TimerDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TimerDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.elapsedDataGridViewTextBoxColumn});
            this.TimerDataGridView.DataSource = this.TimersBS;
            this.TimerDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TimerDataGridView.Location = new System.Drawing.Point(0, 25);
            this.TimerDataGridView.Name = "TimerDataGridView";
            this.TimerDataGridView.Size = new System.Drawing.Size(544, 260);
            this.TimerDataGridView.TabIndex = 0;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.idDataGridViewTextBoxColumn.DataPropertyName = "id";
            this.idDataGridViewTextBoxColumn.HeaderText = "id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.Visible = false;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // elapsedDataGridViewTextBoxColumn
            // 
            this.elapsedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.elapsedDataGridViewTextBoxColumn.DataPropertyName = "Elapsed";
            this.elapsedDataGridViewTextBoxColumn.HeaderText = "Elapsed";
            this.elapsedDataGridViewTextBoxColumn.Name = "elapsedDataGridViewTextBoxColumn";
            // 
            // TimersBS
            // 
            this.TimersBS.DataMember = "Timer";
            this.TimersBS.DataSource = this.TimerDataSet;
            this.TimersBS.AddingNew += new System.ComponentModel.AddingNewEventHandler(this.TimersBS_AddingNew);
            // 
            // TimerDataSet
            // 
            this.TimerDataSet.CaseSensitive = true;
            this.TimerDataSet.DataSetName = "TimerDS";
            this.TimerDataSet.Namespace = "TimerDataSetNS";
            this.TimerDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.Timer});
            // 
            // Timer
            // 
            this.Timer.CaseSensitive = true;
            this.Timer.Columns.AddRange(new System.Data.DataColumn[] {
            this.Id,
            this.TimerName,
            this.Elapsed});
            this.Timer.Constraints.AddRange(new System.Data.Constraint[] {
            new System.Data.UniqueConstraint("Constraint1", new string[] {
                        "id"}, true),
            new System.Data.UniqueConstraint("Constraint2", new string[] {
                        "Name"}, false)});
            this.Timer.Namespace = "TimerDataSetNS";
            this.Timer.PrimaryKey = new System.Data.DataColumn[] {
        this.Id};
            this.Timer.TableName = "Timer";
            // 
            // Id
            // 
            this.Id.AllowDBNull = false;
            this.Id.AutoIncrement = true;
            this.Id.Caption = "id";
            this.Id.ColumnName = "id";
            this.Id.DataType = typeof(int);
            // 
            // TimerName
            // 
            this.TimerName.Caption = "Name";
            this.TimerName.ColumnName = "Name";
            // 
            // Elapsed
            // 
            this.Elapsed.Caption = "Elapsed";
            this.Elapsed.ColumnName = "Elapsed";
            // 
            // TimerBN
            // 
            this.TimerBN.AddNewItem = null;
            this.TimerBN.BindingSource = this.TimersBS;
            this.TimerBN.CountItem = this.bindingNavigatorCountItem;
            this.TimerBN.DeleteItem = null;
            this.TimerBN.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.bindingNavigatorSaveToDatabase});
            this.TimerBN.Location = new System.Drawing.Point(0, 0);
            this.TimerBN.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.TimerBN.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.TimerBN.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.TimerBN.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.TimerBN.Name = "TimerBN";
            this.TimerBN.PositionItem = this.bindingNavigatorPositionItem;
            this.TimerBN.Size = new System.Drawing.Size(544, 25);
            this.TimerBN.TabIndex = 1;
            this.TimerBN.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(35, 22);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 23);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // bindingNavigatorSaveToDatabase
            // 
            this.bindingNavigatorSaveToDatabase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorSaveToDatabase.Enabled = false;
            this.bindingNavigatorSaveToDatabase.Image = global::TimeTracker.Properties.Resources.Save;
            this.bindingNavigatorSaveToDatabase.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bindingNavigatorSaveToDatabase.Name = "bindingNavigatorSaveToDatabase";
            this.bindingNavigatorSaveToDatabase.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorSaveToDatabase.Text = "Save To Database";
            this.bindingNavigatorSaveToDatabase.ToolTipText = "Save To Database";
            // 
            // TimeTrackerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 285);
            this.Controls.Add(this.TimerDataGridView);
            this.Controls.Add(this.TimerBN);
            this.Name = "TimeTrackerMainForm";
            this.Text = "Time Trackers";
            ((System.ComponentModel.ISupportInitialize)(this.TimerDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimersBS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Timer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TimerBN)).EndInit();
            this.TimerBN.ResumeLayout(false);
            this.TimerBN.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView TimerDataGridView;
        private System.Windows.Forms.BindingSource TimersBS;
        private System.Windows.Forms.BindingNavigator TimerBN;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton bindingNavigatorSaveToDatabase;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn elapsedDataGridViewTextBoxColumn;
        private System.Data.DataSet TimerDataSet;
        private System.Data.DataTable Timer;
        private System.Data.DataColumn Id;
        private System.Data.DataColumn TimerName;
        private System.Data.DataColumn Elapsed;
    }
}

