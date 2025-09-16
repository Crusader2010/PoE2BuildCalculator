namespace PoE2BuildCalculator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            OpenPoE2ItemList = new OpenFileDialog();
            ButtonOpenItemListFile = new Button();
            StatusBar = new StatusStrip();
            StatusBarLabel = new ToolStripStatusLabel();
            ButtonParseItemListFile = new Button();
            button1 = new Button();
            StatusBar.SuspendLayout();
            SuspendLayout();
            // 
            // OpenPoE2ItemList
            // 
            OpenPoE2ItemList.FileName = "*.txt";
            OpenPoE2ItemList.SupportMultiDottedExtensions = true;
            // 
            // ButtonOpenItemListFile
            // 
            ButtonOpenItemListFile.Location = new Point(200, 110);
            ButtonOpenItemListFile.Name = "ButtonOpenItemListFile";
            ButtonOpenItemListFile.Size = new Size(126, 58);
            ButtonOpenItemListFile.TabIndex = 0;
            ButtonOpenItemListFile.Text = "Choose File";
            ButtonOpenItemListFile.UseVisualStyleBackColor = true;
            ButtonOpenItemListFile.Click += ButtonOpenItemListFile_Click;
            // 
            // StatusBar
            // 
            StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel });
            StatusBar.Location = new Point(0, 428);
            StatusBar.Name = "StatusBar";
            StatusBar.Size = new Size(800, 22);
            StatusBar.TabIndex = 1;
            StatusBar.Text = "statusStrip1";
            // 
            // StatusBarLabel
            // 
            StatusBarLabel.Name = "StatusBarLabel";
            StatusBarLabel.Size = new Size(0, 17);
            // 
            // ButtonParseItemListFile
            // 
            ButtonParseItemListFile.Location = new Point(425, 110);
            ButtonParseItemListFile.Name = "ButtonParseItemListFile";
            ButtonParseItemListFile.Size = new Size(126, 58);
            ButtonParseItemListFile.TabIndex = 2;
            ButtonParseItemListFile.Text = "Begin Parsing File";
            ButtonParseItemListFile.UseVisualStyleBackColor = true;
            ButtonParseItemListFile.Click += ButtonParseItemListFile_Click;
            // 
            // button1
            // 
            button1.Location = new Point(351, 264);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(ButtonParseItemListFile);
            Controls.Add(StatusBar);
            Controls.Add(ButtonOpenItemListFile);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PoE2 Build Calculator";
            Load += MainForm_Load;
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog OpenPoE2ItemList;
        private Button ButtonOpenItemListFile;
        private StatusStrip StatusBar;
        private ToolStripStatusLabel StatusBarLabel;
        private Button ButtonParseItemListFile;
        private Button button1;
    }
}
