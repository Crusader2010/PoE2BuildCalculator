namespace PoE2BuildCalculator
{
    partial class Compute
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
            AddTierButton = new Button();
            RemoveTierButton = new Button();
            TableTiers = new CustomDataGridView();
            TableStatsWeightSum = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)TableTiers).BeginInit();
            ((System.ComponentModel.ISupportInitialize)TableStatsWeightSum).BeginInit();
            SuspendLayout();
            // 
            // AddTierButton
            // 
            AddTierButton.Location = new Point(151, 12);
            AddTierButton.Name = "AddTierButton";
            AddTierButton.Size = new Size(76, 54);
            AddTierButton.TabIndex = 4;
            AddTierButton.Text = "Add tier";
            AddTierButton.UseVisualStyleBackColor = true;
            AddTierButton.Click += AddTierButton_Click;
            // 
            // RemoveTierButton
            // 
            RemoveTierButton.Location = new Point(13, 12);
            RemoveTierButton.Name = "RemoveTierButton";
            RemoveTierButton.Size = new Size(76, 54);
            RemoveTierButton.TabIndex = 5;
            RemoveTierButton.Text = "Remove tiers";
            RemoveTierButton.UseVisualStyleBackColor = true;
            RemoveTierButton.Click += RemoveTierButton_Click;
            // 
            // TableTiers
            // 
            TableTiers.AllowUserToAddRows = false;
            TableTiers.AllowUserToDeleteRows = false;
            TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            TableTiers.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
            TableTiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableTiers.Location = new Point(244, 12);
            TableTiers.Name = "TableTiers";
            TableTiers.RowTemplate.Resizable = DataGridViewTriState.False;
            TableTiers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TableTiers.Size = new Size(868, 383);
            TableTiers.TabIndex = 6;
            TableTiers.CellPainting += TableTiers_CellPainting;
            TableTiers.CellValidating += TableTiers_CellValidating;
            TableTiers.CellValueChanged += TableTiers_CellValueChanged;
            TableTiers.UserDeletingRow += TableTiers_UserDeletingRow;
            // 
            // TableStatsWeightSum
            // 
            TableStatsWeightSum.AllowUserToAddRows = false;
            TableStatsWeightSum.AllowUserToDeleteRows = false;
            TableStatsWeightSum.AllowUserToResizeColumns = false;
            TableStatsWeightSum.AllowUserToResizeRows = false;
            TableStatsWeightSum.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            TableStatsWeightSum.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            TableStatsWeightSum.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            TableStatsWeightSum.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableStatsWeightSum.Location = new Point(12, 104);
            TableStatsWeightSum.Name = "TableStatsWeightSum";
            TableStatsWeightSum.Size = new Size(215, 291);
            TableStatsWeightSum.TabIndex = 7;
            // 
            // Compute
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1133, 411);
            Controls.Add(TableStatsWeightSum);
            Controls.Add(TableTiers);
            Controls.Add(RemoveTierButton);
            Controls.Add(AddTierButton);
            DoubleBuffered = true;
            Name = "Compute";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Compute";
            FormClosing += Compute_FormClosing;
            Load += Compute_Load;
            ((System.ComponentModel.ISupportInitialize)TableTiers).EndInit();
            ((System.ComponentModel.ISupportInitialize)TableStatsWeightSum).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button AddTierButton;
        private Button RemoveTierButton;
        private DataGridView TableStatsWeightSum;
        private CustomDataGridView TableTiers;
    }
}