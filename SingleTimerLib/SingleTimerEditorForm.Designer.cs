namespace SingleTimerLib
{
    partial class SingleTimerEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleTimerEditorForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.resetTimerbutton = new System.Windows.Forms.Button();
            this.RunTimerCheckBox = new System.Windows.Forms.CheckBox();
            this.stateImageList = new System.Windows.Forms.ImageList(this.components);
            this.MenuTextLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(287, 91);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 3;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.Controls.Add(this.resetTimerbutton, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.RunTimerCheckBox, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.MenuTextLabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.closeButton, 2, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(287, 91);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // resetTimerbutton
            // 
            this.resetTimerbutton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resetTimerbutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.resetTimerbutton.Location = new System.Drawing.Point(132, 48);
            this.resetTimerbutton.Name = "resetTimerbutton";
            this.resetTimerbutton.Size = new System.Drawing.Size(123, 40);
            this.resetTimerbutton.TabIndex = 4;
            this.resetTimerbutton.Text = "Reset Timer";
            this.resetTimerbutton.UseVisualStyleBackColor = true;
            this.resetTimerbutton.Click += new System.EventHandler(this.ResetTimerbutton_Click);
            // 
            // RunTimerCheckBox
            // 
            this.RunTimerCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
            this.RunTimerCheckBox.AutoSize = true;
            this.RunTimerCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunTimerCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RunTimerCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RunTimerCheckBox.ImageKey = "play";
            this.RunTimerCheckBox.ImageList = this.stateImageList;
            this.RunTimerCheckBox.Location = new System.Drawing.Point(3, 48);
            this.RunTimerCheckBox.Name = "RunTimerCheckBox";
            this.RunTimerCheckBox.Size = new System.Drawing.Size(123, 40);
            this.RunTimerCheckBox.TabIndex = 5;
            this.RunTimerCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunTimerCheckBox.UseVisualStyleBackColor = true;
            this.RunTimerCheckBox.CheckedChanged += new System.EventHandler(this.RunTimerCheckBox_CheckedChanged);
            // 
            // stateImageList
            // 
            this.stateImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("stateImageList.ImageStream")));
            this.stateImageList.TransparentColor = System.Drawing.Color.White;
            this.stateImageList.Images.SetKeyName(0, "play");
            this.stateImageList.Images.SetKeyName(1, "stop");
            this.stateImageList.Images.SetKeyName(2, "Closed");
            // 
            // MenuTextLabel
            // 
            this.MenuTextLabel.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.MenuTextLabel, 3);
            this.MenuTextLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MenuTextLabel.Location = new System.Drawing.Point(3, 0);
            this.MenuTextLabel.MinimumSize = new System.Drawing.Size(0, 45);
            this.MenuTextLabel.Name = "MenuTextLabel";
            this.MenuTextLabel.Size = new System.Drawing.Size(281, 45);
            this.MenuTextLabel.TabIndex = 6;
            this.MenuTextLabel.Text = "[Menu Tex]";
            this.MenuTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // closeButton
            // 
            this.closeButton.AutoSize = true;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.closeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ImageKey = "Closed";
            this.closeButton.ImageList = this.stateImageList;
            this.closeButton.Location = new System.Drawing.Point(261, 48);
            this.closeButton.MaximumSize = new System.Drawing.Size(25, 0);
            this.closeButton.MinimumSize = new System.Drawing.Size(25, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(25, 40);
            this.closeButton.TabIndex = 7;
            this.closeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // SingleTimerEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(287, 91);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(375, 120);
            this.MinimizeBox = false;
            this.Name = "SingleTimerEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button resetTimerbutton;
        private System.Windows.Forms.CheckBox RunTimerCheckBox;
        private System.Windows.Forms.ImageList stateImageList;
        private System.Windows.Forms.Label MenuTextLabel;
        private System.Windows.Forms.Button closeButton;
    }
}

