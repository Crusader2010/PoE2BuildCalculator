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
			ButtonClose = new Button();
			ButtonExport = new Button();
			StatusBar = new StatusStrip();
			StatusBarLabel = new ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)SplitContainerMain).BeginInit();
			SplitContainerMain.Panel1.SuspendLayout();
			SplitContainerMain.Panel2.SuspendLayout();
			SplitContainerMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)DataGridViewMaster).BeginInit();
			((System.ComponentModel.ISupportInitialize)DataGridViewDetail).BeginInit();
			SuspendLayout();
			// 
			// SplitContainerMain
			// 
			SplitContainerMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			SplitContainerMain.Location = new Point(12, 12);
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
			SplitContainerMain.Size = new Size(1160, 586);
			SplitContainerMain.SplitterDistance = 434;
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
			DataGridViewMaster.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			DataGridViewMaster.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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
			dataGridViewCellStyle3.Font = new Font("Nirmala UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
			DataGridViewMaster.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			DataGridViewMaster.RowHeadersVisible = false;
			DataGridViewMaster.RowHeadersWidth = 51;
			DataGridViewMaster.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewMaster.Size = new Size(1160, 434);
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
			DataGridViewDetail.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			DataGridViewDetail.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
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
			DataGridViewDetail.Size = new Size(1160, 149);
			DataGridViewDetail.TabIndex = 0;
			// 
			// ButtonClose
			// 
			ButtonClose.Location = new Point(0, 0);
			ButtonClose.Name = "ButtonClose";
			ButtonClose.Size = new Size(75, 23);
			ButtonClose.TabIndex = 0;
			// 
			// ButtonExport
			// 
			ButtonExport.Location = new Point(0, 0);
			ButtonExport.Name = "ButtonExport";
			ButtonExport.Size = new Size(75, 23);
			ButtonExport.TabIndex = 0;
			// 
			// StatusBar
			// 
			StatusBar.Location = new Point(0, 601);
			StatusBar.Name = "StatusBar";
			StatusBar.Size = new Size(1184, 22);
			StatusBar.TabIndex = 1;
			// 
			// StatusBarLabel
			// 
			StatusBarLabel.Name = "StatusBarLabel";
			StatusBarLabel.Size = new Size(120, 17);
			StatusBarLabel.Text = "Ready";
			// 
			// StatusBarProgressBar
			// 
			StatusBarProgressBar = new ToolStripProgressBar();
			StatusBarProgressBar.Name = "StatusBarProgressBar";
			StatusBarProgressBar.Size = new Size(150, 16);
			StatusBarProgressBar.Visible = false;
			// 
			// StatusBarCancelButton
			// 
			StatusBarCancelButton = new ToolStripButton();
			StatusBarCancelButton.Name = "StatusBarCancelButton";
			StatusBarCancelButton.Text = "Cancel";
			StatusBarCancelButton.BackColor = Color.LightPink;
			StatusBarCancelButton.Alignment = ToolStripItemAlignment.Right;
			StatusBarCancelButton.Visible = false;
			StatusBarCancelButton.Click += StatusBarCancelButton_Click;
			// 
			// Add all items to StatusBar in correct order
			// 
			StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel, StatusBarProgressBar, StatusBarCancelButton });
			// 
			// CombinationDisplay
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(1184, 623);
			Controls.Add(StatusBar);
			Controls.Add(SplitContainerMain);
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
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private SplitContainer SplitContainerMain;
		private DataGridView DataGridViewMaster;
		private DataGridView DataGridViewDetail;
		private Button ButtonClose;
		private Button ButtonExport;
		private StatusStrip StatusBar;
		private ToolStripStatusLabel StatusBarLabel;
		private ToolStripProgressBar StatusBarProgressBar;
		private ToolStripButton StatusBarCancelButton;
	}
}
