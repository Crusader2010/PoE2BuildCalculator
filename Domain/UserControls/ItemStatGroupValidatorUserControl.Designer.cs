namespace Domain.UserControls
{
    partial class ItemStatGroupValidatorUserControl
    {
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        private void InitializeComponent()
        {
            headerPanel = new Panel();
            lblGroupName = new Label();
            btnDelete = new Button();
            contentPanel = new Panel();
            lblAddStat = new Label();
            cmbStats = new ComboBox();
            btnAddStat = new Button();
            statsListBox = new ListBox();
            grpConstraints = new GroupBox();
            chkMin = new CheckBox();
            numMin = new NumericUpDown();
            chkMax = new CheckBox();
            numMax = new NumericUpDown();
            lblValidation = new Label();
            pnlOperatorContainer = new Panel();
            lblOperatorLabel = new Label();
            cmbOperator = new ComboBox();
            lblArrow = new Label();
            headerPanel.SuspendLayout();
            contentPanel.SuspendLayout();
            grpConstraints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMax).BeginInit();
            pnlOperatorContainer.SuspendLayout();
            SuspendLayout();
            // 
            // headerPanel
            // 
            headerPanel.BackColor = Color.FromArgb(70, 130, 180);
            headerPanel.Controls.Add(lblGroupName);
            headerPanel.Controls.Add(btnDelete);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Location = new Point(0, 0);
            headerPanel.Name = "headerPanel";
            headerPanel.Size = new Size(357, 32);
            headerPanel.TabIndex = 0;
            // 
            // lblGroupName
            // 
            lblGroupName.AutoSize = true;
            lblGroupName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblGroupName.ForeColor = Color.White;
            lblGroupName.Location = new Point(8, 7);
            lblGroupName.Name = "lblGroupName";
            lblGroupName.Size = new Size(63, 19);
            lblGroupName.TabIndex = 0;
            lblGroupName.Text = "Group 1";
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.BackColor = Color.FromArgb(180, 40, 40);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatAppearance.BorderColor = Color.FromArgb(140, 30, 30);
            btnDelete.FlatAppearance.MouseDownBackColor = Color.FromArgb(160, 30, 30);
            btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 60, 60);
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(327, 3);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(26, 26);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "✕";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // contentPanel
            // 
            contentPanel.Controls.Add(lblAddStat);
            contentPanel.Controls.Add(cmbStats);
            contentPanel.Controls.Add(btnAddStat);
            contentPanel.Controls.Add(statsListBox);
            contentPanel.Controls.Add(grpConstraints);
            contentPanel.Controls.Add(pnlOperatorContainer);
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Location = new Point(0, 32);
            contentPanel.Name = "contentPanel";
            contentPanel.Padding = new Padding(8);
            contentPanel.Size = new Size(357, 305);
            contentPanel.TabIndex = 1;
            // 
            // lblAddStat
            // 
            lblAddStat.Font = new Font("Segoe UI", 8.5F);
            lblAddStat.Location = new Point(8, 11);
            lblAddStat.Name = "lblAddStat";
            lblAddStat.Size = new Size(60, 18);
            lblAddStat.TabIndex = 0;
            lblAddStat.Text = "Add Stat:";
            // 
            // cmbStats
            // 
            cmbStats.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStats.Font = new Font("Segoe UI", 8.5F);
            cmbStats.FormattingEnabled = true;
            cmbStats.Location = new Point(73, 8);
            cmbStats.Name = "cmbStats";
            cmbStats.Size = new Size(230, 21);
            cmbStats.TabIndex = 1;
            // 
            // btnAddStat
            // 
            btnAddStat.BackColor = Color.FromArgb(70, 130, 180);
            btnAddStat.FlatAppearance.BorderSize = 0;
            btnAddStat.FlatStyle = FlatStyle.Flat;
            btnAddStat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddStat.ForeColor = Color.White;
            btnAddStat.Location = new Point(308, 7);
            btnAddStat.Name = "btnAddStat";
            btnAddStat.Size = new Size(34, 23);
            btnAddStat.TabIndex = 2;
            btnAddStat.Text = "+";
            btnAddStat.UseVisualStyleBackColor = false;
            btnAddStat.Click += btnAddStat_Click;
            // 
            // statsListBox
            // 
            statsListBox.DrawMode = DrawMode.OwnerDrawFixed;
            statsListBox.FormattingEnabled = true;
            statsListBox.ItemHeight = 34;
            statsListBox.Location = new Point(8, 38);
            statsListBox.Name = "statsListBox";
            statsListBox.Size = new Size(334, 106);
            statsListBox.TabIndex = 3;
            statsListBox.MouseClick += statsListBox_MouseClick;
            statsListBox.DrawItem += statsListBox_DrawItem;
            // 
            // grpConstraints
            // 
            grpConstraints.Controls.Add(chkMin);
            grpConstraints.Controls.Add(numMin);
            grpConstraints.Controls.Add(chkMax);
            grpConstraints.Controls.Add(numMax);
            grpConstraints.Controls.Add(lblValidation);
            grpConstraints.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grpConstraints.Location = new Point(8, 183);
            grpConstraints.Name = "grpConstraints";
            grpConstraints.Size = new Size(334, 62);
            grpConstraints.TabIndex = 4;
            grpConstraints.TabStop = false;
            grpConstraints.Text = "Constraints";
            // 
            // chkMin
            // 
            chkMin.Font = new Font("Segoe UI", 8.5F);
            chkMin.Location = new Point(10, 20);
            chkMin.Name = "chkMin";
            chkMin.Size = new Size(48, 20);
            chkMin.TabIndex = 0;
            chkMin.Text = "Min:";
            chkMin.UseVisualStyleBackColor = true;
            chkMin.CheckedChanged += chkMin_CheckedChanged;
            // 
            // numMin
            // 
            numMin.DecimalPlaces = 2;
            numMin.Font = new Font("Segoe UI", 8.5F);
            numMin.Location = new Point(62, 19);
            numMin.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            numMin.Minimum = new decimal(new int[] { 99999, 0, 0, int.MinValue });
            numMin.Name = "numMin";
            numMin.Size = new Size(95, 23);
            numMin.TabIndex = 1;
            numMin.TextAlign = HorizontalAlignment.Center;
            numMin.ValueChanged += numMin_ValueChanged;
            numMin.Leave += numMin_Leave;
            // 
            // chkMax
            // 
            chkMax.Font = new Font("Segoe UI", 8.5F);
            chkMax.Location = new Point(172, 20);
            chkMax.Name = "chkMax";
            chkMax.Size = new Size(52, 20);
            chkMax.TabIndex = 2;
            chkMax.Text = "Max:";
            chkMax.UseVisualStyleBackColor = true;
            chkMax.CheckedChanged += chkMax_CheckedChanged;
            // 
            // numMax
            // 
            numMax.DecimalPlaces = 2;
            numMax.Font = new Font("Segoe UI", 8.5F);
            numMax.Location = new Point(228, 19);
            numMax.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            numMax.Minimum = new decimal(new int[] { 99999, 0, 0, int.MinValue });
            numMax.Name = "numMax";
            numMax.Size = new Size(95, 23);
            numMax.TabIndex = 3;
            numMax.TextAlign = HorizontalAlignment.Center;
            numMax.ValueChanged += numMax_ValueChanged;
            numMax.Leave += numMax_Leave;
            // 
            // lblValidation
            // 
            lblValidation.Font = new Font("Segoe UI", 7F);
            lblValidation.ForeColor = Color.Red;
            lblValidation.Location = new Point(8, 44);
            lblValidation.Name = "lblValidation";
            lblValidation.Size = new Size(318, 12);
            lblValidation.TabIndex = 4;
            lblValidation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlOperatorContainer
            // 
            pnlOperatorContainer.BackColor = Color.FromArgb(250, 250, 250);
            pnlOperatorContainer.BorderStyle = BorderStyle.FixedSingle;
            pnlOperatorContainer.Controls.Add(lblOperatorLabel);
            pnlOperatorContainer.Controls.Add(cmbOperator);
            pnlOperatorContainer.Controls.Add(lblArrow);
            pnlOperatorContainer.Location = new Point(8, 251);
            pnlOperatorContainer.Name = "pnlOperatorContainer";
            pnlOperatorContainer.Size = new Size(334, 50);
            pnlOperatorContainer.TabIndex = 5;
            pnlOperatorContainer.Visible = false;
            // 
            // lblOperatorLabel
            // 
            lblOperatorLabel.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblOperatorLabel.ForeColor = Color.FromArgb(60, 60, 60);
            lblOperatorLabel.Location = new Point(4, 12);
            lblOperatorLabel.Name = "lblOperatorLabel";
            lblOperatorLabel.Size = new Size(169, 23);
            lblOperatorLabel.TabIndex = 0;
            lblOperatorLabel.Text = "Then (next valid group):";
            lblOperatorLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cmbOperator
            // 
            cmbOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOperator.Font = new Font("Segoe UI", 9F);
            cmbOperator.FormattingEnabled = true;
            cmbOperator.Items.AddRange(new object[] { "AND", "OR", "XOR" });
            cmbOperator.Location = new Point(179, 13);
            cmbOperator.Name = "cmbOperator";
            cmbOperator.Size = new Size(90, 23);
            cmbOperator.TabIndex = 1;
            cmbOperator.SelectedIndexChanged += cmbOperator_SelectedIndexChanged;
            // 
            // lblArrow
            // 
            lblArrow.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblArrow.ForeColor = Color.FromArgb(70, 130, 180);
            lblArrow.Location = new Point(275, 0);
            lblArrow.Name = "lblArrow";
            lblArrow.Size = new Size(55, 49);
            lblArrow.TabIndex = 2;
            lblArrow.Text = "→";
            lblArrow.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ItemStatGroupValidatorUserControl
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(contentPanel);
            Controls.Add(headerPanel);
            DoubleBuffered = true;
            Name = "ItemStatGroupValidatorUserControl";
            Size = new Size(357, 337);
            Load += ItemStatGroupValidatorUserControl_Load;
            headerPanel.ResumeLayout(false);
            headerPanel.PerformLayout();
            contentPanel.ResumeLayout(false);
            grpConstraints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMax).EndInit();
            pnlOperatorContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel headerPanel;
        private Label lblGroupName;
        private Button btnDelete;
        private Panel contentPanel;
        private Label lblAddStat;
        private ComboBox cmbStats;
        private Button btnAddStat;
        private ListBox statsListBox;
        private GroupBox grpConstraints;
        private CheckBox chkMin;
        private NumericUpDown numMin;
        private CheckBox chkMax;
        private NumericUpDown numMax;
        private Label lblValidation;
        private Panel pnlOperatorContainer;
        private Label lblOperatorLabel;
        private ComboBox cmbOperator;
        private Label lblArrow;
    }
}