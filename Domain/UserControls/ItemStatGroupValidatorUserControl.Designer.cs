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
			ButtonDeleteGroup = new Button();
			lblAddItemStat = new Label();
			ComboboxItemStats = new ComboBox();
			ButtonAddItemStat = new Button();
			FlowPanelStats = new FlowLayoutPanel();
			PanelAddStats = new Panel();
			panel1 = new Panel();
			headerPanel.SuspendLayout();
			PanelAddStats.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// headerPanel
			// 
			headerPanel.BackColor = Color.FromArgb(70, 130, 180);
			headerPanel.Controls.Add(lblGroupName);
			headerPanel.Controls.Add(ButtonDeleteGroup);
			headerPanel.Dock = DockStyle.Top;
			headerPanel.Location = new Point(0, 0);
			headerPanel.Name = "headerPanel";
			headerPanel.Size = new Size(365, 32);
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
			// ButtonDeleteGroup
			// 
			ButtonDeleteGroup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			ButtonDeleteGroup.BackColor = Color.FromArgb(180, 40, 40);
			ButtonDeleteGroup.Cursor = Cursors.Hand;
			ButtonDeleteGroup.FlatAppearance.BorderColor = Color.FromArgb(140, 30, 30);
			ButtonDeleteGroup.FlatAppearance.MouseDownBackColor = Color.FromArgb(160, 30, 30);
			ButtonDeleteGroup.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 60, 60);
			ButtonDeleteGroup.FlatStyle = FlatStyle.Flat;
			ButtonDeleteGroup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
			ButtonDeleteGroup.ForeColor = Color.White;
			ButtonDeleteGroup.Location = new Point(335, 3);
			ButtonDeleteGroup.Name = "ButtonDeleteGroup";
			ButtonDeleteGroup.Size = new Size(26, 26);
			ButtonDeleteGroup.TabIndex = 1;
			ButtonDeleteGroup.Text = "✕";
			ButtonDeleteGroup.UseVisualStyleBackColor = false;
			ButtonDeleteGroup.Click += ButtonDeleteGroup_Click;
			// 
			// lblAddItemStat
			// 
			lblAddItemStat.Font = new Font("Segoe UI", 8.5F);
			lblAddItemStat.Location = new Point(2, 5);
			lblAddItemStat.Name = "lblAddItemStat";
			lblAddItemStat.Size = new Size(60, 21);
			lblAddItemStat.TabIndex = 0;
			lblAddItemStat.Text = "Add Stat:";
			lblAddItemStat.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// ComboboxItemStats
			// 
			ComboboxItemStats.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboboxItemStats.Font = new Font("Segoe UI", 8.5F);
			ComboboxItemStats.FormattingEnabled = true;
			ComboboxItemStats.Location = new Point(67, 5);
			ComboboxItemStats.Name = "ComboboxItemStats";
			ComboboxItemStats.Size = new Size(230, 21);
			ComboboxItemStats.TabIndex = 1;
			// 
			// ButtonAddItemStat
			// 
			ButtonAddItemStat.BackColor = Color.FromArgb(70, 130, 180);
			ButtonAddItemStat.FlatAppearance.BorderSize = 0;
			ButtonAddItemStat.FlatStyle = FlatStyle.Flat;
			ButtonAddItemStat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			ButtonAddItemStat.ForeColor = Color.White;
			ButtonAddItemStat.Location = new Point(302, 5);
			ButtonAddItemStat.Name = "ButtonAddItemStat";
			ButtonAddItemStat.Size = new Size(44, 21);
			ButtonAddItemStat.TabIndex = 2;
			ButtonAddItemStat.Text = "+";
			ButtonAddItemStat.UseVisualStyleBackColor = false;
			ButtonAddItemStat.Click += ButtonAddItemStat_Click;
			// 
			// FlowPanelStats
			// 
			FlowPanelStats.AutoScroll = true;
			FlowPanelStats.BorderStyle = BorderStyle.FixedSingle;
			FlowPanelStats.Dock = DockStyle.Bottom;
			FlowPanelStats.FlowDirection = FlowDirection.TopDown;
			FlowPanelStats.Location = new Point(0, 32);
			FlowPanelStats.Name = "FlowPanelStats";
			FlowPanelStats.Size = new Size(365, 254);
			FlowPanelStats.TabIndex = 3;
			FlowPanelStats.WrapContents = false;
			// 
			// PanelAddStats
			// 
			PanelAddStats.BorderStyle = BorderStyle.FixedSingle;
			PanelAddStats.Controls.Add(lblAddItemStat);
			PanelAddStats.Controls.Add(ButtonAddItemStat);
			PanelAddStats.Controls.Add(ComboboxItemStats);
			PanelAddStats.Dock = DockStyle.Top;
			PanelAddStats.Location = new Point(0, 0);
			PanelAddStats.Name = "PanelAddStats";
			PanelAddStats.Size = new Size(365, 34);
			PanelAddStats.TabIndex = 4;
			// 
			// panel1
			// 
			panel1.Controls.Add(PanelAddStats);
			panel1.Controls.Add(FlowPanelStats);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(0, 32);
			panel1.Name = "panel1";
			panel1.Size = new Size(365, 286);
			panel1.TabIndex = 5;
			// 
			// ItemStatGroupValidatorUserControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.White;
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(panel1);
			Controls.Add(headerPanel);
			DoubleBuffered = true;
			Name = "ItemStatGroupValidatorUserControl";
			Size = new Size(365, 318);
			Load += ItemStatGroupValidatorUserControl_Load;
			headerPanel.ResumeLayout(false);
			headerPanel.PerformLayout();
			PanelAddStats.ResumeLayout(false);
			panel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel headerPanel;
		private Label lblGroupName;
		private Button ButtonDeleteGroup;
		private Label lblAddItemStat;
		private ComboBox ComboboxItemStats;
		private Button ButtonAddItemStat;
		private FlowLayoutPanel FlowPanelStats;
		private Panel PanelAddStats;
		private Panel panel1;
	}
}
