namespace PoE2BuildCalculator
{
	partial class CombinationDisplay
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
			SplitContainerMain = new SplitContainer();
			DataGridViewMaster = new DataGridView();
			DataGridViewDetail = new DataGridView();
			StatusBar = new StatusStrip();
			StatusBarLabel = new ToolStripStatusLabel();
			PanelButtons = new Panel();
			ButtonHelp = new Button();
			ButtonClose = new Button();
			((System.ComponentModel.ISupportInitialize)SplitContainerMain).BeginInit();
			SplitContainerMain.Panel1.SuspendLayout();
			SplitContainerMain.Panel2.SuspendLayout();
			SplitContainerMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)DataGridViewMaster).BeginInit();
			((System.ComponentModel.ISupportInitialize)DataGridViewDetail).BeginInit();
			StatusBar.SuspendLayout();
			PanelButtons.SuspendLayout();
			SuspendLayout();
			// 
			// SplitContainerMain
			// 
			SplitContainerMain.Dock = DockStyle.Fill;
			SplitContainerMain.Location = new Point(0, 40);
			SplitContainerMain.Name = "SplitContainerMain";
			SplitContainerMain.Orientation = Orientation.Horizontal;
			// 
			// SplitContainerMain.Panel1
			// 
			SplitContainerMain.Panel1.Controls.Add(DataGridViewMaster);
			SplitContainerMain.Panel1MinSize = 35;
			// 
			// SplitContainerMain.Panel2
			// 
			SplitContainerMain.Panel2.Controls.Add(DataGridViewDetail);
			SplitContainerMain.Panel2MinSize = 35;
			SplitContainerMain.Size = new Size(1184, 605);
			SplitContainerMain.SplitterDistance = 323;
			SplitContainerMain.SplitterWidth = 3;
			SplitContainerMain.TabIndex = 0;
			// 
			// DataGridViewMaster
			// 
			DataGridViewMaster.AllowUserToAddRows = false;
			DataGridViewMaster.AllowUserToDeleteRows = false;
			DataGridViewMaster.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
			DataGridViewMaster.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			DataGridViewMaster.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.Window;
			dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9F);
			dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
			DataGridViewMaster.DefaultCellStyle = dataGridViewCellStyle2;
			DataGridViewMaster.Dock = DockStyle.Fill;
			DataGridViewMaster.Location = new Point(0, 0);
			DataGridViewMaster.Name = "DataGridViewMaster";
			DataGridViewMaster.ReadOnly = true;
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = SystemColors.Control;
			dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
			DataGridViewMaster.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			DataGridViewMaster.RowHeadersVisible = false;
			DataGridViewMaster.RowHeadersWidth = 51;
			DataGridViewMaster.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewMaster.Size = new Size(1184, 323);
			DataGridViewMaster.TabIndex = 0;
			// 
			// DataGridViewDetail
			// 
			DataGridViewDetail.AllowUserToAddRows = false;
			DataGridViewDetail.AllowUserToDeleteRows = false;
			DataGridViewDetail.AllowUserToResizeRows = false;
			dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
			DataGridViewDetail.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
			DataGridViewDetail.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = SystemColors.Window;
			dataGridViewCellStyle5.Font = new Font("Nirmala UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
			DataGridViewDetail.DefaultCellStyle = dataGridViewCellStyle5;
			DataGridViewDetail.Dock = DockStyle.Fill;
			DataGridViewDetail.Location = new Point(0, 0);
			DataGridViewDetail.Name = "DataGridViewDetail";
			DataGridViewDetail.ReadOnly = true;
			DataGridViewDetail.RowHeadersVisible = false;
			DataGridViewDetail.RowHeadersWidth = 51;
			DataGridViewDetail.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewDetail.Size = new Size(1184, 279);
			DataGridViewDetail.TabIndex = 0;
			// 
			// StatusBar
			// 
			StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel });
			StatusBar.Location = new Point(0, 645);
			StatusBar.Name = "StatusBar";
			StatusBar.RenderMode = ToolStripRenderMode.Professional;
			StatusBar.Size = new Size(1184, 22);
			StatusBar.TabIndex = 1;
			// 
			// StatusBarLabel
			// 
			StatusBarLabel.Name = "StatusBarLabel";
			StatusBarLabel.Size = new Size(39, 17);
			StatusBarLabel.Text = "Ready";
			// 
			// PanelButtons
			// 
			PanelButtons.BackColor = Color.Lavender;
			PanelButtons.BorderStyle = BorderStyle.FixedSingle;
			PanelButtons.Controls.Add(ButtonHelp);
			PanelButtons.Controls.Add(ButtonClose);
			PanelButtons.Dock = DockStyle.Top;
			PanelButtons.Location = new Point(0, 0);
			PanelButtons.Name = "PanelButtons";
			PanelButtons.Size = new Size(1184, 40);
			PanelButtons.TabIndex = 2;
			// 
			// ButtonHelp
			// 
			ButtonHelp.BackColor = Color.FromArgb(100, 149, 237);
			ButtonHelp.Cursor = Cursors.Help;
			ButtonHelp.FlatAppearance.BorderSize = 0;
			ButtonHelp.FlatStyle = FlatStyle.Flat;
			ButtonHelp.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
			ButtonHelp.ForeColor = Color.White;
			ButtonHelp.Location = new Point(711, 3);
			ButtonHelp.Name = "ButtonHelp";
			ButtonHelp.Size = new Size(43, 32);
			ButtonHelp.TabIndex = 3;
			ButtonHelp.Text = "?";
			ButtonHelp.UseVisualStyleBackColor = false;
			ButtonHelp.Click += ButtonHelp_Click;
			// 
			// ButtonClose
			// 
			ButtonClose.Location = new Point(363, 3);
			ButtonClose.Name = "ButtonClose";
			ButtonClose.Size = new Size(150, 32);
			ButtonClose.TabIndex = 0;
			ButtonClose.Text = "Close";
			ButtonClose.UseVisualStyleBackColor = true;
			ButtonClose.Click += ButtonClose_Click;
			// 
			// CombinationDisplay
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(1184, 667);
			Controls.Add(SplitContainerMain);
			Controls.Add(PanelButtons);
			Controls.Add(StatusBar);
			DoubleBuffered = true;
			MinimumSize = new Size(800, 400);
			Name = "CombinationDisplay";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Combination Display - Comparison View";
			SplitContainerMain.Panel1.ResumeLayout(false);
			SplitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)SplitContainerMain).EndInit();
			SplitContainerMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)DataGridViewMaster).EndInit();
			((System.ComponentModel.ISupportInitialize)DataGridViewDetail).EndInit();
			StatusBar.ResumeLayout(false);
			StatusBar.PerformLayout();
			PanelButtons.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private SplitContainer SplitContainerMain;
		private DataGridView DataGridViewMaster;
		private DataGridView DataGridViewDetail;
		private StatusStrip StatusBar;
		private ToolStripStatusLabel StatusBarLabel;
		private Panel PanelButtons;
		private Button ButtonClose;
		private Button ButtonHelp;
	}
}
