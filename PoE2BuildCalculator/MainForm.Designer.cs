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
            TierManagerButton = new Button();
            ShowItemsDataButton = new Button();
            ButtonComputeCombinations = new Button();
            TextboxDisplay = new TextBox();
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
            ButtonOpenItemListFile.Location = new Point(376, 91);
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
            ButtonParseItemListFile.Location = new Point(520, 91);
            ButtonParseItemListFile.Name = "ButtonParseItemListFile";
            ButtonParseItemListFile.Size = new Size(126, 58);
            ButtonParseItemListFile.TabIndex = 2;
            ButtonParseItemListFile.Text = "Begin Parsing File";
            ButtonParseItemListFile.UseVisualStyleBackColor = true;
            ButtonParseItemListFile.Click += ButtonParseItemListFile_Click;
            // 
            // TierManagerButton
            // 
            TierManagerButton.Location = new Point(12, 91);
            TierManagerButton.Name = "TierManagerButton";
            TierManagerButton.Size = new Size(137, 73);
            TierManagerButton.TabIndex = 3;
            TierManagerButton.Text = "Manage tiers and weights";
            TierManagerButton.UseVisualStyleBackColor = true;
            TierManagerButton.Click += TierManagerButton_Click;
            // 
            // ShowItemsDataButton
            // 
            ShowItemsDataButton.Location = new Point(12, 12);
            ShowItemsDataButton.Name = "ShowItemsDataButton";
            ShowItemsDataButton.Size = new Size(137, 73);
            ShowItemsDataButton.TabIndex = 4;
            ShowItemsDataButton.Text = "Display items data";
            ShowItemsDataButton.UseVisualStyleBackColor = true;
            ShowItemsDataButton.Click += ShowItemsDataButton_Click;
            // 
            // ButtonComputeCombinations
            // 
            ButtonComputeCombinations.Location = new Point(662, 91);
            ButtonComputeCombinations.Name = "ButtonComputeCombinations";
            ButtonComputeCombinations.Size = new Size(126, 58);
            ButtonComputeCombinations.TabIndex = 5;
            ButtonComputeCombinations.Text = "Compute combinations";
            ButtonComputeCombinations.UseVisualStyleBackColor = true;
            ButtonComputeCombinations.Click += ButtonComputeCombinations_Click;
            // 
            // TextboxDisplay
            // 
            TextboxDisplay.BorderStyle = BorderStyle.FixedSingle;
            TextboxDisplay.Location = new Point(376, 12);
            TextboxDisplay.MaxLength = int.MaxValue;
            TextboxDisplay.Multiline = true;
            TextboxDisplay.Name = "TextboxDisplay";
            TextboxDisplay.ReadOnly = true;
            TextboxDisplay.ScrollBars = ScrollBars.Both;
            TextboxDisplay.Size = new Size(412, 58);
            TextboxDisplay.TabIndex = 6;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(800, 450);
            Controls.Add(TextboxDisplay);
            Controls.Add(ButtonComputeCombinations);
            Controls.Add(ShowItemsDataButton);
            Controls.Add(TierManagerButton);
            Controls.Add(ButtonParseItemListFile);
            Controls.Add(StatusBar);
            Controls.Add(ButtonOpenItemListFile);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
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
        private Button TierManagerButton;
        private Button ShowItemsDataButton;
        private Button ButtonComputeCombinations;
        private TextBox TextboxDisplay;
    }
}
