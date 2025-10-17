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
			panel1 = new Panel();
			panel3 = new Panel();
			panel2 = new Panel();
			headerPanel.SuspendLayout();
			panel1.SuspendLayout();
			panel3.SuspendLayout();
			panel2.SuspendLayout();
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
			headerPanel.Size = new Size(367, 32);
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
			ButtonDeleteGroup.Location = new Point(337, 3);
			ButtonDeleteGroup.Name = "ButtonDeleteGroup";
			ButtonDeleteGroup.Size = new Size(26, 26);
			ButtonDeleteGroup.TabIndex = 1;
			ButtonDeleteGroup.Text = "✕";
			ButtonDeleteGroup.UseVisualStyleBackColor = false;
			ButtonDeleteGroup.Click += ButtonDeleteGroup_Click;
			// 
			// lblAddItemStat
			// 
			lblAddItemStat.Dock = DockStyle.Left;
			lblAddItemStat.Font = new Font("Segoe UI", 8.5F);
			lblAddItemStat.Location = new Point(1, 5);
			lblAddItemStat.Name = "lblAddItemStat";
			lblAddItemStat.Size = new Size(60, 26);
			lblAddItemStat.TabIndex = 0;
			lblAddItemStat.Text = "Add Stat:";
			lblAddItemStat.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// ComboboxItemStats
			// 
			ComboboxItemStats.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboboxItemStats.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
			ComboboxItemStats.FormattingEnabled = true;
			ComboboxItemStats.Location = new Point(63, 6);
			ComboboxItemStats.Margin = new Padding(0);
			ComboboxItemStats.Name = "ComboboxItemStats";
			ComboboxItemStats.Size = new Size(234, 24);
			ComboboxItemStats.TabIndex = 1;
			// 
			// ButtonAddItemStat
			// 
			ButtonAddItemStat.BackColor = Color.FromArgb(70, 130, 180);
			ButtonAddItemStat.Dock = DockStyle.Right;
			ButtonAddItemStat.FlatAppearance.BorderSize = 0;
			ButtonAddItemStat.FlatStyle = FlatStyle.Flat;
			ButtonAddItemStat.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
			ButtonAddItemStat.ForeColor = Color.White;
			ButtonAddItemStat.Location = new Point(300, 5);
			ButtonAddItemStat.Name = "ButtonAddItemStat";
			ButtonAddItemStat.Size = new Size(65, 26);
			ButtonAddItemStat.TabIndex = 2;
			ButtonAddItemStat.Text = "+";
			ButtonAddItemStat.UseVisualStyleBackColor = false;
			ButtonAddItemStat.Click += ButtonAddItemStat_Click;
			// 
			// FlowPanelStats
			// 
			FlowPanelStats.AutoScroll = true;
			FlowPanelStats.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			FlowPanelStats.Dock = DockStyle.Fill;
			FlowPanelStats.FlowDirection = FlowDirection.TopDown;
			FlowPanelStats.Location = new Point(0, 0);
			FlowPanelStats.Name = "FlowPanelStats";
			FlowPanelStats.Size = new Size(367, 242);
			FlowPanelStats.TabIndex = 3;
			FlowPanelStats.WrapContents = false;
			// 
			// panel1
			// 
			panel1.Controls.Add(panel3);
			panel1.Controls.Add(panel2);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(0, 32);
			panel1.Name = "panel1";
			panel1.Size = new Size(367, 278);
			panel1.TabIndex = 5;
			// 
			// panel3
			// 
			panel3.Controls.Add(FlowPanelStats);
			panel3.Dock = DockStyle.Fill;
			panel3.Location = new Point(0, 36);
			panel3.Name = "panel3";
			panel3.Size = new Size(367, 242);
			panel3.TabIndex = 6;
			// 
			// panel2
			// 
			panel2.Controls.Add(ButtonAddItemStat);
			panel2.Controls.Add(lblAddItemStat);
			panel2.Controls.Add(ComboboxItemStats);
			panel2.Dock = DockStyle.Top;
			panel2.Location = new Point(0, 0);
			panel2.Margin = new Padding(0);
			panel2.Name = "panel2";
			panel2.Padding = new Padding(1, 5, 2, 5);
			panel2.Size = new Size(367, 36);
			panel2.TabIndex = 5;
			// 
			// ItemStatGroupValidatorUserControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			BackColor = Color.White;
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(panel1);
			Controls.Add(headerPanel);
			DoubleBuffered = true;
			Margin = new Padding(0, 0, 2, 2);
			Name = "ItemStatGroupValidatorUserControl";
			Size = new Size(367, 310);
			Load += ItemStatGroupValidatorUserControl_Load;
			headerPanel.ResumeLayout(false);
			headerPanel.PerformLayout();
			panel1.ResumeLayout(false);
			panel3.ResumeLayout(false);
			panel2.ResumeLayout(false);
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
        private Panel panel1;
        private Panel panel2;
		private Panel panel3;
	}
}
