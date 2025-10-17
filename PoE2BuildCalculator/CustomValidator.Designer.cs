namespace PoE2BuildCalculator
{
	partial class CustomValidator
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			mainPanel = new TableLayoutPanel();
			headerPanel = new Panel();
			btnAddOperation = new Button();
			lblTitle = new Label();
			btnAddGroup = new Button();
			btnHelp = new Button();
			bottomPanel = new Panel();
			btnCreateValidator = new Button();
			btnClose = new Button();
			panel1 = new Panel();
			FlowPanelGroups = new FlowLayoutPanel();
			FlowPanelOperations = new FlowLayoutPanel();
			mainPanel.SuspendLayout();
			headerPanel.SuspendLayout();
			bottomPanel.SuspendLayout();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// mainPanel
			// 
			mainPanel.ColumnCount = 1;
			mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			mainPanel.Controls.Add(headerPanel, 0, 0);
			mainPanel.Controls.Add(bottomPanel, 0, 2);
			mainPanel.Controls.Add(panel1, 0, 1);
			mainPanel.Dock = DockStyle.Fill;
			mainPanel.Location = new Point(0, 0);
			mainPanel.Margin = new Padding(1);
			mainPanel.Name = "mainPanel";
			mainPanel.Padding = new Padding(10);
			mainPanel.RowCount = 3;
			mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
			mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			mainPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
			mainPanel.Size = new Size(1200, 700);
			mainPanel.TabIndex = 0;
			// 
			// headerPanel
			// 
			headerPanel.Controls.Add(btnAddOperation);
			headerPanel.Controls.Add(lblTitle);
			headerPanel.Controls.Add(btnAddGroup);
			headerPanel.Controls.Add(btnHelp);
			headerPanel.Dock = DockStyle.Fill;
			headerPanel.Location = new Point(11, 11);
			headerPanel.Margin = new Padding(1);
			headerPanel.Name = "headerPanel";
			headerPanel.Size = new Size(1178, 48);
			headerPanel.TabIndex = 0;
			// 
			// btnAddOperation
			// 
			btnAddOperation.BackColor = Color.FromArgb(70, 130, 180);
			btnAddOperation.FlatAppearance.BorderSize = 0;
			btnAddOperation.FlatStyle = FlatStyle.Flat;
			btnAddOperation.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnAddOperation.ForeColor = Color.White;
			btnAddOperation.Location = new Point(362, 3);
			btnAddOperation.Name = "btnAddOperation";
			btnAddOperation.Size = new Size(182, 38);
			btnAddOperation.TabIndex = 3;
			btnAddOperation.Text = "+ Add Group Operation";
			btnAddOperation.UseVisualStyleBackColor = false;
			btnAddOperation.Click += BtnAddOperation_Click;
			// 
			// lblTitle
			// 
			lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			lblTitle.Location = new Point(3, 3);
			lblTitle.Name = "lblTitle";
			lblTitle.Size = new Size(173, 38);
			lblTitle.TabIndex = 0;
			lblTitle.Text = "Validation Groups";
			lblTitle.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// btnAddGroup
			// 
			btnAddGroup.BackColor = Color.FromArgb(70, 130, 180);
			btnAddGroup.FlatAppearance.BorderSize = 0;
			btnAddGroup.FlatStyle = FlatStyle.Flat;
			btnAddGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnAddGroup.ForeColor = Color.White;
			btnAddGroup.Location = new Point(193, 3);
			btnAddGroup.Name = "btnAddGroup";
			btnAddGroup.Size = new Size(162, 38);
			btnAddGroup.TabIndex = 1;
			btnAddGroup.Text = "+ Add Group";
			btnAddGroup.UseVisualStyleBackColor = false;
			btnAddGroup.Click += BtnAddGroup_Click;
			// 
			// btnHelp
			// 
			btnHelp.BackColor = Color.FromArgb(100, 149, 237);
			btnHelp.Cursor = Cursors.Help;
			btnHelp.FlatAppearance.BorderSize = 0;
			btnHelp.FlatStyle = FlatStyle.Flat;
			btnHelp.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			btnHelp.ForeColor = Color.White;
			btnHelp.Location = new Point(550, 3);
			btnHelp.Name = "btnHelp";
			btnHelp.Size = new Size(43, 38);
			btnHelp.TabIndex = 2;
			btnHelp.Text = "?";
			btnHelp.UseVisualStyleBackColor = false;
			btnHelp.Click += BtnHelp_Click;
			// 
			// bottomPanel
			// 
			bottomPanel.Controls.Add(btnCreateValidator);
			bottomPanel.Controls.Add(btnClose);
			bottomPanel.Dock = DockStyle.Fill;
			bottomPanel.Location = new Point(13, 633);
			bottomPanel.Name = "bottomPanel";
			bottomPanel.Size = new Size(1174, 54);
			bottomPanel.TabIndex = 2;
			// 
			// btnCreateValidator
			// 
			btnCreateValidator.BackColor = Color.FromArgb(34, 139, 34);
			btnCreateValidator.FlatAppearance.BorderSize = 0;
			btnCreateValidator.FlatStyle = FlatStyle.Flat;
			btnCreateValidator.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
			btnCreateValidator.ForeColor = Color.White;
			btnCreateValidator.Location = new Point(10, 10);
			btnCreateValidator.Name = "btnCreateValidator";
			btnCreateValidator.Size = new Size(150, 40);
			btnCreateValidator.TabIndex = 0;
			btnCreateValidator.Text = "Create Validator";
			btnCreateValidator.UseVisualStyleBackColor = false;
			btnCreateValidator.Click += BtnCreateValidator_Click;
			// 
			// btnClose
			// 
			btnClose.BackColor = Color.FromArgb(220, 220, 220);
			btnClose.FlatStyle = FlatStyle.Flat;
			btnClose.Font = new Font("Segoe UI", 10F);
			btnClose.Location = new Point(170, 10);
			btnClose.Name = "btnClose";
			btnClose.Size = new Size(100, 40);
			btnClose.TabIndex = 1;
			btnClose.Text = "Close";
			btnClose.UseVisualStyleBackColor = false;
			btnClose.Click += BtnClose_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(FlowPanelGroups);
			panel1.Controls.Add(FlowPanelOperations);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(11, 61);
			panel1.Margin = new Padding(1);
			panel1.Name = "panel1";
			panel1.Size = new Size(1178, 568);
			panel1.TabIndex = 3;
			// 
			// FlowPanelGroups
			// 
			FlowPanelGroups.AutoScroll = true;
			FlowPanelGroups.BackColor = SystemColors.ControlLight;
			FlowPanelGroups.Dock = DockStyle.Fill;
			FlowPanelGroups.Location = new Point(270, 0);
			FlowPanelGroups.Margin = new Padding(0);
			FlowPanelGroups.Name = "FlowPanelGroups";
			FlowPanelGroups.Size = new Size(908, 568);
			FlowPanelGroups.TabIndex = 1;
			// 
			// FlowPanelOperations
			// 
			FlowPanelOperations.AutoScroll = true;
			FlowPanelOperations.BackColor = Color.WhiteSmoke;
			FlowPanelOperations.Dock = DockStyle.Left;
			FlowPanelOperations.Location = new Point(0, 0);
			FlowPanelOperations.Margin = new Padding(0, 0, 1, 0);
			FlowPanelOperations.Name = "FlowPanelOperations";
			FlowPanelOperations.Size = new Size(270, 568);
			FlowPanelOperations.TabIndex = 0;
			// 
			// CustomValidator
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(1200, 700);
			Controls.Add(mainPanel);
			DoubleBuffered = true;
			Name = "CustomValidator";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Custom Validator - Group-Based Configuration";
			FormClosing += CustomValidator_FormClosing;
			Load += CustomValidator_Load;
			mainPanel.ResumeLayout(false);
			headerPanel.ResumeLayout(false);
			bottomPanel.ResumeLayout(false);
			panel1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel mainPanel;
		private Panel headerPanel;
		private Label lblTitle;
		private Button btnAddGroup;
		private Button btnHelp;
		private Panel bottomPanel;
		private Button btnCreateValidator;
		private Button btnClose;

		private void BtnClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private void CustomValidator_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
				this.Owner?.BringToFront();
			}
		}
		private Button btnAddOperation;
		private Panel panel1;
		private FlowLayoutPanel FlowPanelGroups;
		private FlowLayoutPanel FlowPanelOperations;
	}
}
