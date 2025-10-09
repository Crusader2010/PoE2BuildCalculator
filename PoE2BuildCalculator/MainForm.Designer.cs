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
            ButtonManageCustomValidator = new Button();
            ButtonBenchmark = new Button();
            PanelButtons = new Panel();
            StatusBar.SuspendLayout();
            PanelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // OpenPoE2ItemList
            // 
            OpenPoE2ItemList.FileName = "*.txt";
            OpenPoE2ItemList.SupportMultiDottedExtensions = true;
            // 
            // ButtonOpenItemListFile
            // 
            ButtonOpenItemListFile.Location = new Point(14, 13);
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
            StatusBar.Size = new Size(837, 22);
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
            ButtonParseItemListFile.Location = new Point(158, 13);
            ButtonParseItemListFile.Name = "ButtonParseItemListFile";
            ButtonParseItemListFile.Size = new Size(126, 58);
            ButtonParseItemListFile.TabIndex = 2;
            ButtonParseItemListFile.Text = "Begin Parsing File";
            ButtonParseItemListFile.UseVisualStyleBackColor = true;
            ButtonParseItemListFile.Click += ButtonParseItemListFile_Click;
            // 
            // TierManagerButton
            // 
            TierManagerButton.Location = new Point(158, 96);
            TierManagerButton.Name = "TierManagerButton";
            TierManagerButton.Size = new Size(126, 58);
            TierManagerButton.TabIndex = 3;
            TierManagerButton.Text = "Manage tiers and weights";
            TierManagerButton.UseVisualStyleBackColor = true;
            TierManagerButton.Click += TierManagerButton_Click;
            // 
            // ShowItemsDataButton
            // 
            ShowItemsDataButton.Location = new Point(12, 12);
            ShowItemsDataButton.Name = "ShowItemsDataButton";
            ShowItemsDataButton.Size = new Size(126, 58);
            ShowItemsDataButton.TabIndex = 4;
            ShowItemsDataButton.Text = "Display items data";
            ShowItemsDataButton.UseVisualStyleBackColor = true;
            ShowItemsDataButton.Click += ShowItemsDataButton_Click;
            // 
            // ButtonComputeCombinations
            // 
            ButtonComputeCombinations.Location = new Point(158, 177);
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
            TextboxDisplay.Location = new Point(323, 12);
            TextboxDisplay.MaxLength = int.MaxValue;
            TextboxDisplay.Multiline = true;
            TextboxDisplay.Name = "TextboxDisplay";
            TextboxDisplay.ReadOnly = true;
            TextboxDisplay.ScrollBars = ScrollBars.Both;
            TextboxDisplay.Size = new Size(480, 384);
            TextboxDisplay.TabIndex = 6;
            // 
            // ButtonManageCustomValidator
            // 
            ButtonManageCustomValidator.Location = new Point(14, 96);
            ButtonManageCustomValidator.Name = "ButtonManageCustomValidator";
            ButtonManageCustomValidator.Size = new Size(126, 58);
            ButtonManageCustomValidator.TabIndex = 7;
            ButtonManageCustomValidator.Text = "Manage custom validator";
            ButtonManageCustomValidator.UseVisualStyleBackColor = true;
            ButtonManageCustomValidator.Click += ButtonManageCustomValidator_Click;
            // 
            // ButtonBenchmark
            // 
            ButtonBenchmark.Location = new Point(14, 177);
            ButtonBenchmark.Name = "ButtonBenchmark";
            ButtonBenchmark.Size = new Size(126, 58);
            ButtonBenchmark.TabIndex = 8;
            ButtonBenchmark.Text = "Run Benchmark";
            ButtonBenchmark.UseVisualStyleBackColor = true;
            ButtonBenchmark.Click += ButtonBenchmark_Click;
            // 
            // PanelButtons
            // 
            PanelButtons.Controls.Add(ButtonOpenItemListFile);
            PanelButtons.Controls.Add(ButtonBenchmark);
            PanelButtons.Controls.Add(ButtonParseItemListFile);
            PanelButtons.Controls.Add(ButtonManageCustomValidator);
            PanelButtons.Controls.Add(TierManagerButton);
            PanelButtons.Controls.Add(ButtonComputeCombinations);
            PanelButtons.Location = new Point(12, 96);
            PanelButtons.Name = "PanelButtons";
            PanelButtons.Size = new Size(296, 248);
            PanelButtons.TabIndex = 9;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(837, 450);
            Controls.Add(PanelButtons);
            Controls.Add(TextboxDisplay);
            Controls.Add(ShowItemsDataButton);
            Controls.Add(StatusBar);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PoE2 Build Calculator - Main window";
            Load += MainForm_Load;
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            PanelButtons.ResumeLayout(false);
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
        private Button ButtonManageCustomValidator;
        private Button ButtonBenchmark;
        private Panel PanelButtons;
    }
}
