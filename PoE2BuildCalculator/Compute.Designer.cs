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
            TableTiers = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)TableTiers).BeginInit();
            SuspendLayout();
            // 
            // AddTierButton
            // 
            AddTierButton.Location = new Point(12, 12);
            AddTierButton.Name = "AddTierButton";
            AddTierButton.Size = new Size(76, 54);
            AddTierButton.TabIndex = 4;
            AddTierButton.Text = "Add tier";
            AddTierButton.UseVisualStyleBackColor = true;
            AddTierButton.Click += AddTierButton_Click;
            // 
            // RemoveTierButton
            // 
            RemoveTierButton.Location = new Point(12, 82);
            RemoveTierButton.Name = "RemoveTierButton";
            RemoveTierButton.Size = new Size(76, 54);
            RemoveTierButton.TabIndex = 5;
            RemoveTierButton.Text = "Remove tier";
            RemoveTierButton.UseVisualStyleBackColor = true;
            RemoveTierButton.Click += RemoveTierButton_Click;
            // 
            // TableTiers
            // 
            TableTiers.AllowUserToAddRows = false;
            TableTiers.AllowUserToDeleteRows = false;
            TableTiers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            TableTiers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableTiers.Location = new Point(94, 12);
            TableTiers.Name = "TableTiers";
            TableTiers.ReadOnly = true;
            TableTiers.Size = new Size(454, 213);
            TableTiers.TabIndex = 6;
            TableTiers.CellFormatting += TableTiers_CellFormatting;
            TableTiers.CellValidating += TableTiers_CellValidating;
            TableTiers.CellValueChanged += TableTiers_CellValueChanged;
            TableTiers.UserDeletingRow += TableTiers_UserDeletingRow;
            // 
            // Compute
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(567, 237);
            Controls.Add(TableTiers);
            Controls.Add(RemoveTierButton);
            Controls.Add(AddTierButton);
            DoubleBuffered = true;
            Name = "Compute";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Compute";
            Load += Compute_Load;
            ((System.ComponentModel.ISupportInitialize)TableTiers).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button AddTierButton;
        private Button RemoveTierButton;
        private DataGridView TableTiers;
    }
}