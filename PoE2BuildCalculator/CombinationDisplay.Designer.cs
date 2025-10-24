namespace PoE2BuildCalculator
{
	partial class CombinationDisplay
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
			DataGridViewCombinations = new DataGridView();
			ButtonClose = new Button();
			ButtonExport = new Button();
			StatusBar = new StatusStrip();
			StatusBarLabel = new ToolStripStatusLabel();
			PanelButtons = new Panel();
			((System.ComponentModel.ISupportInitialize)DataGridViewCombinations).BeginInit();
			StatusBar.SuspendLayout();
			PanelButtons.SuspendLayout();
			SuspendLayout();
			// 
			// DataGridViewCombinations
			// 
			DataGridViewCombinations.AllowUserToAddRows = false;
			DataGridViewCombinations.AllowUserToDeleteRows = false;
			DataGridViewCombinations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			DataGridViewCombinations.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			DataGridViewCombinations.Location = new Point(12, 12);
			DataGridViewCombinations.Name = "DataGridViewCombinations";
			DataGridViewCombinations.ReadOnly = true;
			DataGridViewCombinations.RowHeadersWidth = 51;
			DataGridViewCombinations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewCombinations.Size = new Size(1160, 537);
			DataGridViewCombinations.TabIndex = 0;
			// 
			// ButtonClose
			// 
			ButtonClose.Location = new Point(3, 3);
			ButtonClose.Name = "ButtonClose";
			ButtonClose.Size = new Size(100, 40);
			ButtonClose.TabIndex = 0;
			ButtonClose.Text = "Close";
			ButtonClose.UseVisualStyleBackColor = true;
			ButtonClose.Click += ButtonClose_Click;
			// 
			// ButtonExport
			// 
			ButtonExport.Location = new Point(109, 3);
			ButtonExport.Name = "ButtonExport";
			ButtonExport.Size = new Size(100, 40);
			ButtonExport.TabIndex = 1;
			ButtonExport.Text = "Export...";
			ButtonExport.UseVisualStyleBackColor = true;
			ButtonExport.Click += ButtonExport_Click;
			// 
			// StatusBar
			// 
			StatusBar.ImageScalingSize = new Size(20, 20);
			StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel });
			StatusBar.Location = new Point(0, 601);
			StatusBar.Name = "StatusBar";
			StatusBar.Size = new Size(1184, 22);
			StatusBar.TabIndex = 2;
			// 
			// StatusBarLabel
			// 
			StatusBarLabel.Name = "StatusBarLabel";
			StatusBarLabel.Size = new Size(39, 17);
			StatusBarLabel.Text = "Ready";
			// 
			// PanelButtons
			// 
			PanelButtons.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			PanelButtons.Controls.Add(ButtonClose);
			PanelButtons.Controls.Add(ButtonExport);
			PanelButtons.Location = new Point(12, 555);
			PanelButtons.Name = "PanelButtons";
			PanelButtons.Size = new Size(215, 46);
			PanelButtons.TabIndex = 1;
			// 
			// CombinationDisplay
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(1184, 623);
			Controls.Add(PanelButtons);
			Controls.Add(StatusBar);
			Controls.Add(DataGridViewCombinations);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.Sizable;
			MinimumSize = new Size(800, 400);
			Name = "CombinationDisplay";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Combination Display";
			((System.ComponentModel.ISupportInitialize)DataGridViewCombinations).EndInit();
			StatusBar.ResumeLayout(false);
			StatusBar.PerformLayout();
			PanelButtons.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private DataGridView DataGridViewCombinations;
		private Button ButtonClose;
		private Button ButtonExport;
		private StatusStrip StatusBar;
		private ToolStripStatusLabel StatusBarLabel;
		private Panel PanelButtons;
	}
}
