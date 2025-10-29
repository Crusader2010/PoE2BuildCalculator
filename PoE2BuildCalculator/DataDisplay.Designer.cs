namespace PoE2BuildCalculator
{
	partial class DataDisplay
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

		private void InitializeComponent()
		{
			PanelButtons = new Panel();
			ImportDisplayData = new Button();
			ButtonClose = new Button();
			TableDisplayData = new DataGridView();
			PanelButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)TableDisplayData).BeginInit();
			SuspendLayout();
			// 
			// PanelButtons
			// 
			PanelButtons.Controls.Add(ButtonClose);
			PanelButtons.Controls.Add(ImportDisplayData);
			PanelButtons.Dock = DockStyle.Left;
			PanelButtons.Location = new Point(0, 0);
			PanelButtons.Name = "PanelButtons";
			PanelButtons.Size = new Size(90, 561);
			PanelButtons.TabIndex = 0;
			// 
			// ImportDisplayData
			// 
			ImportDisplayData.Location = new Point(8, 12);
			ImportDisplayData.Name = "ImportDisplayData";
			ImportDisplayData.Size = new Size(75, 70);
			ImportDisplayData.TabIndex = 0;
			ImportDisplayData.Text = "Reload parsed data";
			ImportDisplayData.UseVisualStyleBackColor = true;
			ImportDisplayData.Click += ImportDataToDisplay_Click;
			// 
			// ButtonClose
			// 
			ButtonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			ButtonClose.Location = new Point(8, 479);
			ButtonClose.Name = "ButtonClose";
			ButtonClose.Size = new Size(75, 70);
			ButtonClose.TabIndex = 1;
			ButtonClose.Text = "Close form";
			ButtonClose.UseVisualStyleBackColor = true;
			ButtonClose.Click += ButtonClose_Click;
			// 
			// TableDisplayData
			// 
			TableDisplayData.AllowUserToAddRows = false;
			TableDisplayData.AllowUserToDeleteRows = false;
			TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			TableDisplayData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			TableDisplayData.Dock = DockStyle.Fill;
			TableDisplayData.Location = new Point(90, 0);
			TableDisplayData.Name = "TableDisplayData";
			TableDisplayData.ReadOnly = true;
			TableDisplayData.RowHeadersVisible = false;
			TableDisplayData.Size = new Size(894, 561);
			TableDisplayData.TabIndex = 1;
			// 
			// DataDisplay
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(984, 561);
			Controls.Add(TableDisplayData);
			Controls.Add(PanelButtons);
			DoubleBuffered = true;
			MinimumSize = new Size(600, 400);
			Name = "DataDisplay";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Parsed Items Display";
			PanelButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)TableDisplayData).EndInit();
			ResumeLayout(false);
		}

		private Panel PanelButtons;
		private Button ImportDisplayData;
		private DataGridView TableDisplayData;
		private Button ButtonClose;
	}
}
