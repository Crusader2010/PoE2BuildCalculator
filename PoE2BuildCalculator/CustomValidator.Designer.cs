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
            ButtonClose = new Button();
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
            dgvRules.BackgroundColor = Color.FromArgb(240, 240, 240);
            dgvRules.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRules.Location = new Point(8, 8);
            dgvRules.MultiSelect = false;
            dgvRules.Name = "dgvRules";
            dgvRules.RowHeadersWidth = 25;
            dgvRules.Size = new Size(955, 405);
            dgvRules.TabIndex = 0;
            dgvRules.CellClick += dgvRules_CellClick;
            dgvRules.CellValidating += dgvRules_CellValidating;
            dgvRules.CellValueChanged += dgvRules_CellValueChanged;
            dgvRules.EditingControlShowing += dgvRules_EditingControlShowing;
            // 
            // btnCreateValidator
            // 
            btnCreateValidator.Anchor = AnchorStyles.None;
            btnCreateValidator.BackColor = Color.FromArgb(70, 130, 180);
            btnCreateValidator.FlatAppearance.BorderColor = Color.Lime;
            btnCreateValidator.FlatStyle = FlatStyle.Flat;
            btnCreateValidator.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCreateValidator.ForeColor = Color.White;
            btnCreateValidator.Location = new Point(310, 12);
            btnCreateValidator.Name = "btnCreateValidator";
            btnCreateValidator.Size = new Size(229, 40);
            btnCreateValidator.TabIndex = 1;
            btnCreateValidator.Text = "Create Validator Function";
            btnCreateValidator.UseVisualStyleBackColor = true;
            btnCreateValidator.Click += btnCreateValidator_Click;
            btnCreateValidator.MouseEnter += btnCreateValidator_MouseEnter;
            btnCreateValidator.MouseLeave += btnCreateValidator_MouseLeave;
            // 
            // panelBottom
            // 
            panelBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBottom.BackColor = Color.FromArgb(224, 224, 224);
            panelBottom.Controls.Add(ButtonClose);
            panelBottom.Controls.Add(btnCreateValidator);
            panelBottom.Location = new Point(8, 419);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(955, 64);
            panelBottom.TabIndex = 2;
            // 
            // ButtonClose
            // 
            ButtonClose.Anchor = AnchorStyles.None;
            ButtonClose.BackColor = Color.FromArgb(70, 130, 180);
            ButtonClose.FlatAppearance.BorderColor = Color.FromArgb(192, 0, 0);
            ButtonClose.FlatStyle = FlatStyle.Flat;
            ButtonClose.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            ButtonClose.ForeColor = Color.White;
            ButtonClose.Location = new Point(734, 12);
            ButtonClose.Name = "ButtonClose";
            ButtonClose.Size = new Size(175, 40);
            ButtonClose.TabIndex = 2;
            ButtonClose.Text = "Close and reset form";
            ButtonClose.UseVisualStyleBackColor = true;
            ButtonClose.Click += ButtonClose_Click;
            // 
            // CustomValidator
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(245, 245, 245);
            ClientSize = new Size(971, 491);
            Controls.Add(panelBottom);
            Controls.Add(dgvRules);
            DoubleBuffered = true;
            MinimumSize = new Size(800, 400);
            Name = "CustomValidator";
            Padding = new Padding(8);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Custom Validator Manager";
            FormClosing += CustomValidator_FormClosing;
            Load += CustomValidator_Load;
            ((System.ComponentModel.ISupportInitialize)dgvRules).EndInit();
            panelBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvRules;
        private Button btnCreateValidator;
        private Panel panelBottom;
        private Button ButtonClose;
    }
}