namespace PoE2BuildCalculator
{
    partial class CustomValidator
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
            dgvRules = new DataGridView();
            btnCreateValidator = new Button();
            panelBottom = new Panel();
            ((System.ComponentModel.ISupportInitialize)dgvRules).BeginInit();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // dgvRules
            // 
            dgvRules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRules.Dock = DockStyle.Fill;
            dgvRules.Location = new Point(0, 0);
            dgvRules.Name = "dgvRules";
            dgvRules.Size = new Size(1278, 513);
            dgvRules.TabIndex = 0;
            dgvRules.CellClick += dgvRules_CellClick;
            dgvRules.CellValidating += dgvRules_CellValidating;
            dgvRules.CellValueChanged += dgvRules_CellValueChanged;
            dgvRules.CurrentCellDirtyStateChanged += dgvRules_CurrentCellDirtyStateChanged;
            dgvRules.EditingControlShowing += dgvRules_EditingControlShowing;
            // 
            // btnCreateValidator
            // 
            btnCreateValidator.Dock = DockStyle.Bottom;
            btnCreateValidator.Location = new Point(0, 17);
            btnCreateValidator.Name = "btnCreateValidator";
            btnCreateValidator.Size = new Size(1278, 40);
            btnCreateValidator.TabIndex = 1;
            btnCreateValidator.Text = "Create Validator Function";
            btnCreateValidator.UseVisualStyleBackColor = true;
            btnCreateValidator.Click += btnCreateValidator_Click;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(btnCreateValidator);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 456);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1278, 57);
            panelBottom.TabIndex = 2;
            // 
            // CustomValidator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1278, 513);
            Controls.Add(panelBottom);
            Controls.Add(dgvRules);
            Name = "CustomValidator";
            Text = "Custom Validator";
            Load += CustomValidator_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRules).EndInit();
            panelBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvRules;
        private Button btnCreateValidator;
        private Panel panelBottom;
    }
}