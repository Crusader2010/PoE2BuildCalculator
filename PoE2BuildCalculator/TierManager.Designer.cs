namespace PoE2BuildCalculator
{
    partial class TierManager
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
			components = new System.ComponentModel.Container();
			DataGridViewCellStyle dataGridViewCellStyle16 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle17 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle18 = new DataGridViewCellStyle();
			AddTierButton = new Button();
			RemoveTierButton = new Button();
			TableTiers = new DataGridView();
			TableStatsWeightSum = new DataGridView();
			TextboxTotalTierWeights = new TextBox();
			label1 = new Label();
			ErrorTooltip = new ToolTip(components);
			label2 = new Label();
			ButtonHide = new Button();
			ButtonClose = new Button();
			ButtonLoadConfig = new Button();
			((System.ComponentModel.ISupportInitialize)TableTiers).BeginInit();
			((System.ComponentModel.ISupportInitialize)TableStatsWeightSum).BeginInit();
			SuspendLayout();
			// 
			// AddTierButton
			// 
			AddTierButton.Location = new Point(142, 4);
			AddTierButton.Name = "AddTierButton";
			AddTierButton.Size = new Size(112, 31);
			AddTierButton.TabIndex = 4;
			AddTierButton.Text = "Add tier";
			AddTierButton.UseVisualStyleBackColor = true;
			AddTierButton.Click += AddTierButton_Click;
			// 
			// RemoveTierButton
			// 
			RemoveTierButton.Location = new Point(142, 40);
			RemoveTierButton.Name = "RemoveTierButton";
			RemoveTierButton.Size = new Size(112, 31);
			RemoveTierButton.TabIndex = 5;
			RemoveTierButton.Text = "Remove tiers";
			RemoveTierButton.UseVisualStyleBackColor = true;
			RemoveTierButton.Click += RemoveTierButton_Click;
			// 
			// TableTiers
			// 
			TableTiers.AllowUserToAddRows = false;
			TableTiers.AllowUserToDeleteRows = false;
			TableTiers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
			TableTiers.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
			TableTiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			TableTiers.Location = new Point(260, 104);
			TableTiers.Name = "TableTiers";
			TableTiers.RowTemplate.Resizable = DataGridViewTriState.False;
			TableTiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			TableTiers.Size = new Size(1031, 373);
			TableTiers.TabIndex = 6;
			TableTiers.CellEndEdit += TableTiers_CellEndEdit;
			TableTiers.CellFormatting += TableTiers_CellFormatting;
			TableTiers.CellPainting += TableTiers_CellPainting;
			TableTiers.CellValidating += TableTiers_CellValidating;
			TableTiers.EditingControlShowing += TableTiers_EditingControlShowing;
			TableTiers.SelectionChanged += TableTiers_SelectionChanged;
			TableTiers.KeyDown += TableTiers_KeyDown;
			// 
			// TableStatsWeightSum
			// 
			TableStatsWeightSum.AllowUserToAddRows = false;
			TableStatsWeightSum.AllowUserToDeleteRows = false;
			TableStatsWeightSum.AllowUserToResizeColumns = false;
			TableStatsWeightSum.AllowUserToResizeRows = false;
			TableStatsWeightSum.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			TableStatsWeightSum.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
			TableStatsWeightSum.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
			TableStatsWeightSum.CausesValidation = false;
			dataGridViewCellStyle16.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle16.BackColor = SystemColors.Control;
			dataGridViewCellStyle16.Font = new Font("Microsoft Sans Serif", 9F);
			dataGridViewCellStyle16.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle16.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle16.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle16.WrapMode = DataGridViewTriState.True;
			TableStatsWeightSum.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle16;
			TableStatsWeightSum.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			TableStatsWeightSum.Location = new Point(12, 77);
			TableStatsWeightSum.MultiSelect = false;
			TableStatsWeightSum.Name = "TableStatsWeightSum";
			TableStatsWeightSum.ReadOnly = true;
			dataGridViewCellStyle17.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle17.BackColor = SystemColors.Control;
			dataGridViewCellStyle17.Font = new Font("Microsoft Sans Serif", 9F);
			dataGridViewCellStyle17.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle17.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle17.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle17.WrapMode = DataGridViewTriState.False;
			TableStatsWeightSum.RowHeadersDefaultCellStyle = dataGridViewCellStyle17;
			TableStatsWeightSum.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
			dataGridViewCellStyle18.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle18.WrapMode = DataGridViewTriState.True;
			TableStatsWeightSum.RowsDefaultCellStyle = dataGridViewCellStyle18;
			TableStatsWeightSum.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			TableStatsWeightSum.RowTemplate.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
			TableStatsWeightSum.SelectionMode = DataGridViewSelectionMode.CellSelect;
			TableStatsWeightSum.ShowEditingIcon = false;
			TableStatsWeightSum.Size = new Size(242, 400);
			TableStatsWeightSum.TabIndex = 7;
			TableStatsWeightSum.CellFormatting += TableStatsWeightSum_CellFormatting;
			// 
			// TextboxTotalTierWeights
			// 
			TextboxTotalTierWeights.BackColor = Color.FromArgb(192, 255, 255);
			TextboxTotalTierWeights.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
			TextboxTotalTierWeights.ForeColor = Color.ForestGreen;
			TextboxTotalTierWeights.Location = new Point(12, 50);
			TextboxTotalTierWeights.Name = "TextboxTotalTierWeights";
			TextboxTotalTierWeights.ReadOnly = true;
			TextboxTotalTierWeights.Size = new Size(99, 21);
			TextboxTotalTierWeights.TabIndex = 8;
			TextboxTotalTierWeights.Text = "test";
			TextboxTotalTierWeights.TextAlign = HorizontalAlignment.Center;
			TextboxTotalTierWeights.TextChanged += TextboxTotalTierWeights_TextChanged;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.CausesValidation = false;
			label1.Location = new Point(12, 32);
			label1.Name = "label1";
			label1.Size = new Size(99, 15);
			label1.TabIndex = 9;
			label1.Text = "Total tier weights";
			// 
			// ErrorTooltip
			// 
			ErrorTooltip.AutoPopDelay = 5000;
			ErrorTooltip.InitialDelay = 100;
			ErrorTooltip.IsBalloon = true;
			ErrorTooltip.ReshowDelay = 100;
			ErrorTooltip.ToolTipIcon = ToolTipIcon.Error;
			ErrorTooltip.ToolTipTitle = "Invalid input";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.CausesValidation = false;
			label2.Location = new Point(260, 86);
			label2.Name = "label2";
			label2.Size = new Size(411, 15);
			label2.TabIndex = 10;
			label2.Text = "List of custom tiers - each tier's stats weights and the weight of the tier itself";
			// 
			// ButtonHide
			// 
			ButtonHide.Location = new Point(509, 4);
			ButtonHide.Name = "ButtonHide";
			ButtonHide.Size = new Size(150, 31);
			ButtonHide.TabIndex = 11;
			ButtonHide.Text = "Hide";
			ButtonHide.UseVisualStyleBackColor = true;
			ButtonHide.Click += ButtonHide_Click;
			// 
			// ButtonClose
			// 
			ButtonClose.BackColor = Color.Thistle;
			ButtonClose.FlatStyle = FlatStyle.Popup;
			ButtonClose.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
			ButtonClose.ForeColor = Color.FromArgb(128, 128, 255);
			ButtonClose.Location = new Point(690, 4);
			ButtonClose.Name = "ButtonClose";
			ButtonClose.Size = new Size(150, 31);
			ButtonClose.TabIndex = 12;
			ButtonClose.Text = "Close and dispose";
			ButtonClose.UseVisualStyleBackColor = false;
			ButtonClose.Click += ButtonClose_Click;
			// 
			// ButtonLoadConfig
			// 
			ButtonLoadConfig.BackColor = Color.FromArgb(0, 192, 0);
			ButtonLoadConfig.FlatAppearance.BorderSize = 0;
			ButtonLoadConfig.FlatStyle = FlatStyle.Popup;
			ButtonLoadConfig.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
			ButtonLoadConfig.ForeColor = Color.White;
			ButtonLoadConfig.Location = new Point(320, 4);
			ButtonLoadConfig.Name = "ButtonLoadConfig";
			ButtonLoadConfig.Size = new Size(150, 31);
			ButtonLoadConfig.TabIndex = 13;
			ButtonLoadConfig.Text = "Load from JSON";
			ButtonLoadConfig.UseVisualStyleBackColor = false;
			ButtonLoadConfig.Click += ButtonLoadConfig_Click;
			// 
			// TierManager
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			ClientSize = new Size(1303, 498);
			Controls.Add(ButtonLoadConfig);
			Controls.Add(ButtonClose);
			Controls.Add(ButtonHide);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(TextboxTotalTierWeights);
			Controls.Add(TableStatsWeightSum);
			Controls.Add(TableTiers);
			Controls.Add(RemoveTierButton);
			Controls.Add(AddTierButton);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Name = "TierManager";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Manage Custom Tiers";
			FormClosing += TierManager_FormClosing;
			Load += TierManager_Load;
			((System.ComponentModel.ISupportInitialize)TableTiers).EndInit();
			((System.ComponentModel.ISupportInitialize)TableStatsWeightSum).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Button AddTierButton;
        private Button RemoveTierButton;
        private DataGridView TableStatsWeightSum;
        private DataGridView TableTiers;
        private TextBox TextboxTotalTierWeights;
        private Label label1;
        private ToolTip ErrorTooltip;
        private Label label2;
		private Button ButtonHide;
		private Button ButtonClose;
		private Button ButtonLoadConfig;
	}
}