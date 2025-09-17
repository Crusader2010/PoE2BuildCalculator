namespace PoE2BuildCalculator
{
    partial class DataDisplay
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
            ImportDisplayData = new Button();
            TableDisplayData = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)TableDisplayData).BeginInit();
            SuspendLayout();
            // 
            // ImportDisplayData
            // 
            ImportDisplayData.Location = new Point(12, 12);
            ImportDisplayData.Name = "ImportDisplayData";
            ImportDisplayData.Size = new Size(75, 55);
            ImportDisplayData.TabIndex = 1;
            ImportDisplayData.Text = "Import Data";
            ImportDisplayData.UseVisualStyleBackColor = true;
            ImportDisplayData.Click += ImportDisplayData_Click;
            // 
            // TableDisplayData
            // 
            TableDisplayData.AllowUserToAddRows = false;
            TableDisplayData.AllowUserToDeleteRows = false;
            TableDisplayData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            TableDisplayData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableDisplayData.Location = new Point(93, 12);
            TableDisplayData.Name = "TableDisplayData";
            TableDisplayData.Size = new Size(454, 260);
            TableDisplayData.TabIndex = 2;
            // 
            // DataDisplay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(559, 297);
            Controls.Add(TableDisplayData);
            Controls.Add(ImportDisplayData);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DataDisplay";
            StartPosition = FormStartPosition.CenterParent;
            Text = "DataDisplay";
            ((System.ComponentModel.ISupportInitialize)TableDisplayData).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button ImportDisplayData;
        private DataGridView TableDisplayData;
    }
}