namespace PoE2BuildCalculator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            PanelButtons = new Panel();
            GroupBoxStrategy = new GroupBox();
            RadioStrict = new RadioButton();
            RadioBalanced = new RadioButton();
            RadioComprehensive = new RadioButton();
            PanelConfig = new Panel();
            NumericBestCombinationsCount = new NumericUpDown();
            label1 = new Label();
            StatusBar.SuspendLayout();
            PanelButtons.SuspendLayout();
            GroupBoxStrategy.SuspendLayout();
            PanelConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericBestCombinationsCount).BeginInit();
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
            StatusBarEstimate = new ToolStripStatusLabel
            {
                Name = "StatusBarEstimate",
                Text = "",
                Spring = true,  // Takes remaining space
                TextAlign = ContentAlignment.MiddleRight,
                Visible = false  // Hidden initially
            };

            StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel, StatusBarEstimate });
            StatusBar.Location = new Point(0, 476);
            StatusBar.Name = "StatusBar";
            StatusBar.Size = new Size(705, 22);
            StatusBar.TabIndex = 1;
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
            TierManagerButton.Location = new Point(158, 77);
            TierManagerButton.Name = "TierManagerButton";
            TierManagerButton.Size = new Size(126, 58);
            TierManagerButton.TabIndex = 3;
            TierManagerButton.Text = "Manage tiers and weights";
            TierManagerButton.UseVisualStyleBackColor = true;
            TierManagerButton.Click += TierManagerButton_Click;
            // 
            // ShowItemsDataButton
            // 
            ShowItemsDataButton.Location = new Point(14, 141);
            ShowItemsDataButton.Name = "ShowItemsDataButton";
            ShowItemsDataButton.Size = new Size(126, 58);
            ShowItemsDataButton.TabIndex = 4;
            ShowItemsDataButton.Text = "Display items data";
            ShowItemsDataButton.UseVisualStyleBackColor = true;
            ShowItemsDataButton.Click += ShowItemsDataButton_Click;
            // 
            // ButtonComputeCombinations
            // 
            ButtonComputeCombinations.Location = new Point(158, 141);
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
            TextboxDisplay.Location = new Point(314, 155);
            TextboxDisplay.MaxLength = int.MaxValue;
            TextboxDisplay.Multiline = true;
            TextboxDisplay.Name = "TextboxDisplay";
            TextboxDisplay.ReadOnly = true;
            TextboxDisplay.ScrollBars = ScrollBars.Both;
            TextboxDisplay.Size = new Size(384, 307);
            TextboxDisplay.TabIndex = 6;
            // 
            // ButtonManageCustomValidator
            // 
            ButtonManageCustomValidator.Location = new Point(14, 77);
            ButtonManageCustomValidator.Name = "ButtonManageCustomValidator";
            ButtonManageCustomValidator.Size = new Size(126, 58);
            ButtonManageCustomValidator.TabIndex = 7;
            ButtonManageCustomValidator.Text = "Manage custom validator";
            ButtonManageCustomValidator.UseVisualStyleBackColor = true;
            ButtonManageCustomValidator.Click += ButtonManageCustomValidator_Click;
            // 
            // PanelButtons
            // 
            PanelButtons.Controls.Add(ButtonOpenItemListFile);
            PanelButtons.Controls.Add(ShowItemsDataButton);
            PanelButtons.Controls.Add(ButtonParseItemListFile);
            PanelButtons.Controls.Add(ButtonManageCustomValidator);
            PanelButtons.Controls.Add(TierManagerButton);
            PanelButtons.Controls.Add(ButtonComputeCombinations);
            PanelButtons.Location = new Point(12, 155);
            PanelButtons.Name = "PanelButtons";
            PanelButtons.Size = new Size(296, 307);
            PanelButtons.TabIndex = 9;
            // 
            // GroupBoxStrategy
            // 
            GroupBoxStrategy.Controls.Add(RadioStrict);
            GroupBoxStrategy.Controls.Add(RadioBalanced);
            GroupBoxStrategy.Controls.Add(RadioComprehensive);
            GroupBoxStrategy.Location = new Point(14, 3);
            GroupBoxStrategy.Name = "GroupBoxStrategy";
            GroupBoxStrategy.Size = new Size(270, 126);
            GroupBoxStrategy.TabIndex = 10;
            GroupBoxStrategy.TabStop = false;
            GroupBoxStrategy.Text = "Combination Filter (when tiers defined)";
            // 
            // RadioStrict
            // 
            RadioStrict.AutoSize = true;
            RadioStrict.Location = new Point(6, 90);
            RadioStrict.Name = "RadioStrict";
            RadioStrict.Size = new Size(193, 19);
            RadioStrict.TabIndex = 2;
            RadioStrict.Text = "Strict (only tiered items, fastest)";
            RadioStrict.UseVisualStyleBackColor = true;
            RadioStrict.CheckedChanged += RadioStrict_CheckedChanged;
            // 
            // RadioBalanced
            // 
            RadioBalanced.AutoSize = true;
            RadioBalanced.Checked = true;
            RadioBalanced.Location = new Point(6, 65);
            RadioBalanced.Name = "RadioBalanced";
            RadioBalanced.Size = new Size(250, 19);
            RadioBalanced.TabIndex = 1;
            RadioBalanced.TabStop = true;
            RadioBalanced.Text = "Balanced (≥1 tiered item, recommended)";
            RadioBalanced.UseVisualStyleBackColor = true;
            RadioBalanced.CheckedChanged += RadioBalanced_CheckedChanged;
            // 
            // RadioComprehensive
            // 
            RadioComprehensive.AutoSize = true;
            RadioComprehensive.Location = new Point(6, 40);
            RadioComprehensive.Name = "RadioComprehensive";
            RadioComprehensive.Size = new Size(209, 19);
            RadioComprehensive.TabIndex = 0;
            RadioComprehensive.Text = "Comprehensive (no filter, slowest)";
            RadioComprehensive.UseVisualStyleBackColor = true;
            RadioComprehensive.CheckedChanged += RadioComprehensive_CheckedChanged;
            // 
            // PanelConfig
            // 
            PanelConfig.Controls.Add(NumericBestCombinationsCount);
            PanelConfig.Controls.Add(GroupBoxStrategy);
            PanelConfig.Controls.Add(label1);
            PanelConfig.Location = new Point(12, 12);
            PanelConfig.Name = "PanelConfig";
            PanelConfig.Size = new Size(686, 137);
            PanelConfig.TabIndex = 11;
            // 
            // NumericBestCombinationsCount
            // 
            NumericBestCombinationsCount.Location = new Point(463, 11);
            NumericBestCombinationsCount.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            NumericBestCombinationsCount.Name = "NumericBestCombinationsCount";
            NumericBestCombinationsCount.Size = new Size(85, 21);
            NumericBestCombinationsCount.TabIndex = 2;
            NumericBestCombinationsCount.TextAlign = HorizontalAlignment.Center;
            NumericBestCombinationsCount.ValueChanged += NumericBestCombinationsCount_ValueChanged;
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Location = new Point(302, 11);
            label1.Name = "label1";
            label1.Size = new Size(155, 21);
            label1.TabIndex = 1;
            label1.Text = "Best combinations count";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(705, 498);
            Controls.Add(PanelConfig);
            Controls.Add(PanelButtons);
            Controls.Add(TextboxDisplay);
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
            GroupBoxStrategy.ResumeLayout(false);
            GroupBoxStrategy.PerformLayout();
            PanelConfig.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)NumericBestCombinationsCount).EndInit();
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
        private Panel PanelButtons;
        private GroupBox GroupBoxStrategy;
        private RadioButton RadioComprehensive;
        private RadioButton RadioBalanced;
        private RadioButton RadioStrict;
        private Panel PanelConfig;
        private NumericUpDown NumericBestCombinationsCount;
        private Label label1;
    }
}
