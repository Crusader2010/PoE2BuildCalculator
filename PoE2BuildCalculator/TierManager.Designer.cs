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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            AddTierButton = new Button();
            RemoveTierButton = new Button();
            TableTiers = new DataGridView();
            TableStatsWeightSum = new DataGridView();
            TextboxTotalTierWeights = new TextBox();
            label1 = new Label();
            ErrorTooltip = new ToolTip(components);
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
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            TableStatsWeightSum.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            TableStatsWeightSum.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableStatsWeightSum.Location = new Point(12, 77);
            TableStatsWeightSum.MultiSelect = false;
            TableStatsWeightSum.Name = "TableStatsWeightSum";
            TableStatsWeightSum.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            TableStatsWeightSum.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            TableStatsWeightSum.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            TableStatsWeightSum.RowsDefaultCellStyle = dataGridViewCellStyle3;
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
            // TierManager
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1303, 498);
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
            Text = "Manage tiers";
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
    }
}