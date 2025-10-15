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
			lblTitle = new Label();
			btnAddGroup = new Button();
			btnHelp = new Button();
			groupsContainer = new Panel();
			bottomPanel = new Panel();
			btnCreateValidator = new Button();
			btnClose = new Button();
			mainPanel.SuspendLayout();
			headerPanel.SuspendLayout();
			bottomPanel.SuspendLayout();
			SuspendLayout();
			// 
			// mainPanel
			// 
			mainPanel.ColumnCount = 1;
			mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			mainPanel.Controls.Add(headerPanel, 0, 0);
			mainPanel.Controls.Add(groupsContainer, 0, 1);
			mainPanel.Controls.Add(bottomPanel, 0, 2);
			mainPanel.Dock = DockStyle.Fill;
			mainPanel.Location = new Point(0, 0);
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
			headerPanel.Controls.Add(lblTitle);
			headerPanel.Controls.Add(btnAddGroup);
			headerPanel.Controls.Add(btnHelp);
			headerPanel.Dock = DockStyle.Fill;
			headerPanel.Location = new Point(13, 13);
			headerPanel.Name = "headerPanel";
			headerPanel.Size = new Size(1174, 44);
			headerPanel.TabIndex = 0;
			// 
			// lblTitle
			// 
			lblTitle.AutoSize = true;
			lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			lblTitle.Location = new Point(5, 10);
			lblTitle.Name = "lblTitle";
			lblTitle.Size = new Size(173, 25);
			lblTitle.TabIndex = 0;
			lblTitle.Text = "Validation Groups";
			// 
			// btnAddGroup
			// 
			btnAddGroup.BackColor = Color.FromArgb(70, 130, 180);
			btnAddGroup.FlatAppearance.BorderSize = 0;
			btnAddGroup.FlatStyle = FlatStyle.Flat;
			btnAddGroup.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
			btnAddGroup.ForeColor = Color.White;
			btnAddGroup.Location = new Point(200, 8);
			btnAddGroup.Name = "btnAddGroup";
			btnAddGroup.Size = new Size(120, 35);
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
			btnHelp.Location = new Point(330, 8);
			btnHelp.Name = "btnHelp";
			btnHelp.Size = new Size(35, 35);
			btnHelp.TabIndex = 2;
			btnHelp.Text = "?";
			btnHelp.UseVisualStyleBackColor = false;
			btnHelp.Click += BtnHelp_Click;
			// 
			// groupsContainer
			// 
			groupsContainer.AutoScroll = true;
			groupsContainer.BackColor = Color.FromArgb(240, 240, 240);
			groupsContainer.Dock = DockStyle.Fill;
			groupsContainer.Location = new Point(13, 63);
			groupsContainer.Name = "groupsContainer";
			groupsContainer.Size = new Size(1174, 564);
			groupsContainer.TabIndex = 1;
			groupsContainer.Resize += GroupsContainer_Resize;
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
			// CustomValidator
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(1200, 700);
			Controls.Add(mainPanel);
			DoubleBuffered = true;
			MinimumSize = new Size(450, 600);
			Name = "CustomValidator";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Custom Validator - Group-Based Configuration";
			FormClosing += CustomValidator_FormClosing;
			Load += CustomValidator_Load;
			mainPanel.ResumeLayout(false);
			headerPanel.ResumeLayout(false);
			headerPanel.PerformLayout();
			bottomPanel.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel mainPanel;
        private Panel headerPanel;
        private Label lblTitle;
        private Button btnAddGroup;
        private Button btnHelp;
        private Panel groupsContainer;
        private Panel bottomPanel;
        private Button btnCreateValidator;
        private Button btnClose;

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
