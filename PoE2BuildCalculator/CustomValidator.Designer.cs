namespace PoE2BuildCalculator
{
    partial class CustomValidator
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
            this.SuspendLayout();
            // 
            // CustomValidator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Name = "CustomValidator";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Validator";
            this.Load += new System.EventHandler(this.CustomValidator_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomValidator_FormClosing);
            this.ResumeLayout(false);
        }
    }
}