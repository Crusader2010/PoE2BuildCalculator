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
            dgvRules.AllowUserToAddRows = false;
            dgvRules.AllowUserToDeleteRows = false;
            dgvRules.AllowUserToResizeColumns = false;
            dgvRules.AllowUserToResizeRows = false;
            dgvRules.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvRules.BackgroundColor = Color.FromArgb(224, 224, 224);
            dgvRules.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
            dgvRules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRules.Location = new Point(0, 0);
            dgvRules.MultiSelect = false;
            dgvRules.Name = "dgvRules";
            dgvRules.Size = new Size(1386, 478);
            dgvRules.TabIndex = 0;
            dgvRules.CellClick += dgvRules_CellClick;
            dgvRules.CellValidating += dgvRules_CellValidating;
            dgvRules.CellValueChanged += dgvRules_CellValueChanged;
            dgvRules.EditingControlShowing += dgvRules_EditingControlShowing;
            // 
            // btnCreateValidator
            // 
            btnCreateValidator.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnCreateValidator.BackColor = Color.FromArgb(255, 224, 192);
            btnCreateValidator.Dock = DockStyle.Bottom;
            btnCreateValidator.Location = new Point(0, 15);
            btnCreateValidator.Margin = new Padding(20);
            btnCreateValidator.Name = "btnCreateValidator";
            btnCreateValidator.Size = new Size(1384, 43);
            btnCreateValidator.TabIndex = 1;
            btnCreateValidator.Text = "Create Validator Function";
            btnCreateValidator.UseVisualStyleBackColor = false;
            btnCreateValidator.Click += btnCreateValidator_Click;
            // 
            // panelBottom
            // 
            panelBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBottom.BackColor = Color.FromArgb(224, 224, 224);
            panelBottom.BorderStyle = BorderStyle.FixedSingle;
            panelBottom.Controls.Add(btnCreateValidator);
            panelBottom.Location = new Point(0, 478);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1386, 60);
            panelBottom.TabIndex = 2;
            // 
            // CustomValidator
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(1386, 538);
            Controls.Add(panelBottom);
            Controls.Add(dgvRules);
            DoubleBuffered = true;
            Name = "CustomValidator";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Custom Validator Manager";
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