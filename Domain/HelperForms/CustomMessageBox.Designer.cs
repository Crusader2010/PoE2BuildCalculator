namespace Domain.HelperForms
{
	partial class CustomMessageBox
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
			MainPanel = new Panel();
			BorderPanel = new Panel();
			RichTextBoxMessage = new RichTextBox();
			PanelBottom = new Panel();
			BottomTablePanel = new TableLayoutPanel();
			FlowPanelButtons = new FlowLayoutPanel();
			Button1 = new Button();
			Button2 = new Button();
			CheckboxWrapText = new CheckBox();
			PictureBoxIcon = new PictureBox();
			MainPanel.SuspendLayout();
			BorderPanel.SuspendLayout();
			PanelBottom.SuspendLayout();
			BottomTablePanel.SuspendLayout();
			FlowPanelButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PictureBoxIcon).BeginInit();
			SuspendLayout();
			// 
			// MainPanel
			// 
			MainPanel.BackColor = Color.White;
			MainPanel.Controls.Add(BorderPanel);
			MainPanel.Dock = DockStyle.Fill;
			MainPanel.Location = new Point(0, 0);
			MainPanel.Margin = new Padding(0);
			MainPanel.Name = "MainPanel";
			MainPanel.Padding = new Padding(12, 8, 12, 8);
			MainPanel.Size = new Size(439, 355);
			MainPanel.TabIndex = 0;
			// 
			// BorderPanel
			// 
			BorderPanel.Controls.Add(RichTextBoxMessage);
			BorderPanel.Dock = DockStyle.Fill;
			BorderPanel.Location = new Point(12, 8);
			BorderPanel.Name = "BorderPanel";
			BorderPanel.Size = new Size(415, 339);
			BorderPanel.TabIndex = 0;
			// 
			// RichTextBoxMessage
			// 
			RichTextBoxMessage.BackColor = Color.FromArgb(250, 250, 250);
			RichTextBoxMessage.BorderStyle = BorderStyle.None;
			RichTextBoxMessage.Dock = DockStyle.Fill;
			RichTextBoxMessage.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
			RichTextBoxMessage.ForeColor = Color.FromArgb(50, 50, 50);
			RichTextBoxMessage.Location = new Point(0, 0);
			RichTextBoxMessage.Name = "RichTextBoxMessage";
			RichTextBoxMessage.ReadOnly = true;
			RichTextBoxMessage.ScrollBars = RichTextBoxScrollBars.Both;
			RichTextBoxMessage.Size = new Size(415, 339);
			RichTextBoxMessage.TabIndex = 0;
			RichTextBoxMessage.WordWrap = false;
			// 
			// PanelBottom
			// 
			PanelBottom.Controls.Add(BottomTablePanel);
			PanelBottom.Dock = DockStyle.Bottom;
			PanelBottom.Location = new Point(0, 355);
			PanelBottom.Name = "PanelBottom";
			PanelBottom.Size = new Size(439, 51);
			PanelBottom.TabIndex = 1;
			// 
			// BottomTablePanel
			// 
			BottomTablePanel.ColumnCount = 3;
			BottomTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			BottomTablePanel.ColumnStyles.Add(new ColumnStyle());
			BottomTablePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			BottomTablePanel.Controls.Add(FlowPanelButtons, 1, 0);
			BottomTablePanel.Dock = DockStyle.Fill;
			BottomTablePanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
			BottomTablePanel.Location = new Point(0, 0);
			BottomTablePanel.Name = "BottomTablePanel";
			BottomTablePanel.RowCount = 1;
			BottomTablePanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			BottomTablePanel.Size = new Size(439, 51);
			BottomTablePanel.TabIndex = 0;
			// 
			// FlowPanelButtons
			// 
			FlowPanelButtons.Anchor = AnchorStyles.None;
			FlowPanelButtons.AutoSize = true;
			FlowPanelButtons.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			FlowPanelButtons.Controls.Add(Button1);
			FlowPanelButtons.Controls.Add(Button2);
			FlowPanelButtons.Controls.Add(CheckboxWrapText);
			FlowPanelButtons.Controls.Add(PictureBoxIcon);
			FlowPanelButtons.Location = new Point(30, 3);
			FlowPanelButtons.Margin = new Padding(20, 0, 0, 0);
			FlowPanelButtons.Name = "FlowPanelButtons";
			FlowPanelButtons.Size = new Size(398, 45);
			FlowPanelButtons.TabIndex = 0;
			FlowPanelButtons.WrapContents = false;
			// 
			// Button1
			// 
			Button1.BackColor = Color.FromArgb(192, 255, 192);
			Button1.FlatStyle = FlatStyle.Flat;
			Button1.Location = new Point(3, 0);
			Button1.Margin = new Padding(3, 0, 0, 0);
			Button1.Name = "Button1";
			Button1.Size = new Size(100, 45);
			Button1.TabIndex = 0;
			Button1.Text = "button1";
			Button1.UseVisualStyleBackColor = false;
			Button1.Click += Button1_Click;
			// 
			// Button2
			// 
			Button2.BackColor = Color.FromArgb(255, 192, 192);
			Button2.FlatStyle = FlatStyle.Flat;
			Button2.Location = new Point(106, 0);
			Button2.Margin = new Padding(3, 0, 0, 0);
			Button2.Name = "Button2";
			Button2.Size = new Size(100, 45);
			Button2.TabIndex = 1;
			Button2.Text = "button2";
			Button2.UseVisualStyleBackColor = false;
			Button2.Click += Button2_Click;
			// 
			// CheckboxWrapText
			// 
			CheckboxWrapText.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxWrapText.FlatStyle = FlatStyle.Popup;
			FlowPanelButtons.SetFlowBreak(CheckboxWrapText, true);
			CheckboxWrapText.Location = new Point(256, 0);
			CheckboxWrapText.Margin = new Padding(50, 0, 0, 0);
			CheckboxWrapText.Name = "CheckboxWrapText";
			CheckboxWrapText.Padding = new Padding(2);
			CheckboxWrapText.Size = new Size(89, 45);
			CheckboxWrapText.TabIndex = 3;
			CheckboxWrapText.Text = "Wrap Text";
			CheckboxWrapText.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxWrapText.UseVisualStyleBackColor = false;
			CheckboxWrapText.CheckedChanged += CheckboxWrapText_CheckedChanged;
			// 
			// PictureBoxIcon
			// 
			PictureBoxIcon.BackColor = Color.Transparent;
			PictureBoxIcon.BackgroundImageLayout = ImageLayout.Center;
			PictureBoxIcon.Location = new Point(350, 0);
			PictureBoxIcon.Margin = new Padding(5, 0, 3, 0);
			PictureBoxIcon.Name = "PictureBoxIcon";
			PictureBoxIcon.Size = new Size(45, 45);
			PictureBoxIcon.SizeMode = PictureBoxSizeMode.CenterImage;
			PictureBoxIcon.TabIndex = 2;
			PictureBoxIcon.TabStop = false;
			// 
			// CustomMessageBox
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(439, 406);
			Controls.Add(MainPanel);
			Controls.Add(PanelBottom);
			DoubleBuffered = true;
			Font = new Font("Nirmala UI", 9F);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "CustomMessageBox";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterScreen;
			TopMost = true;
			Load += CustomMessageBox_Load;
			MainPanel.ResumeLayout(false);
			BorderPanel.ResumeLayout(false);
			PanelBottom.ResumeLayout(false);
			BottomTablePanel.ResumeLayout(false);
			BottomTablePanel.PerformLayout();
			FlowPanelButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PictureBoxIcon).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Panel MainPanel;
		internal RichTextBox RichTextBoxMessage;
		private Panel PanelBottom;
		private PictureBox PictureBoxIcon;
		private Button Button2;
		private Button Button1;
		private TableLayoutPanel BottomTablePanel;
		private FlowLayoutPanel FlowPanelButtons;
		private CheckBox CheckboxWrapText;
		private Panel BorderPanel;
	}
}
