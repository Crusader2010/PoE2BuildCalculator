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
			lblAddStat = new Label();
			cmbStats = new ComboBox();
			btnAddStat = new Button();
			statsListBox = new ListBox();
			headerPanel.SuspendLayout();
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
			// lblAddStat
			// 
			lblAddStat.Font = new Font("Segoe UI", 8.5F);
			lblAddStat.Location = new Point(2, 38);
			lblAddStat.Name = "lblAddStat";
			lblAddStat.Size = new Size(60, 21);
			lblAddStat.TabIndex = 0;
			lblAddStat.Text = "Add Stat:";
			lblAddStat.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// cmbStats
			// 
			cmbStats.DropDownStyle = ComboBoxStyle.DropDownList;
			cmbStats.Font = new Font("Segoe UI", 8.5F);
			cmbStats.FormattingEnabled = true;
			cmbStats.Location = new Point(67, 38);
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
			btnAddStat.Location = new Point(302, 38);
			btnAddStat.Name = "btnAddStat";
			btnAddStat.Size = new Size(44, 21);
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
			statsListBox.Location = new Point(3, 65);
			statsListBox.Name = "statsListBox";
			statsListBox.Size = new Size(343, 174);
			statsListBox.TabIndex = 3;
			// 
			// ItemStatGroupValidatorUserControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.White;
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(statsListBox);
			Controls.Add(lblAddStat);
			Controls.Add(cmbStats);
			Controls.Add(btnAddStat);
			Controls.Add(headerPanel);
			DoubleBuffered = true;
			Name = "ItemStatGroupValidatorUserControl";
			Size = new Size(357, 250);
			Load += ItemStatGroupValidatorUserControl_Load;
			headerPanel.ResumeLayout(false);
			headerPanel.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private Panel headerPanel;
		private Label lblGroupName;
		private Button btnDelete;
		private Label lblAddStat;
		private ComboBox cmbStats;
		private Button btnAddStat;
		private ListBox statsListBox;
	}
}
