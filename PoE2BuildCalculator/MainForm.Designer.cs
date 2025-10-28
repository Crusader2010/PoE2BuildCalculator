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
			StatusBarEstimate = new ToolStripStatusLabel();
			ButtonParseItemListFile = new Button();
			TierManagerButton = new Button();
			ShowItemsDataButton = new Button();
			ButtonComputeCombinations = new Button();
			TextboxDisplay = new TextBox();
			ButtonManageCustomValidator = new Button();
			PanelButtons = new Panel();
			ShowScoredCombinationsButton = new Button();
			GroupBoxStrategy = new GroupBox();
			RadioStrict = new RadioButton();
			RadioBalanced = new RadioButton();
			RadioComprehensive = new RadioButton();
			PanelConfig = new Panel();
			NumericBestCombinationsCount = new NumericUpDown();
			LabelBestCombinationsCount = new Label();
			MenuStrip = new MenuStrip();
			LoadConfigMenuButton = new ToolStripMenuItem();
			SaveConfigMenuButton = new ToolStripMenuItem();
			StatusBar.SuspendLayout();
			PanelButtons.SuspendLayout();
			GroupBoxStrategy.SuspendLayout();
			PanelConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)NumericBestCombinationsCount).BeginInit();
			MenuStrip.SuspendLayout();
			SuspendLayout();
			// 
			// OpenPoE2ItemList
			// 
			OpenPoE2ItemList.FileName = "*.txt";
			OpenPoE2ItemList.SupportMultiDottedExtensions = true;
			// 
			// ButtonOpenItemListFile
			// 
			ButtonOpenItemListFile.Location = new Point(12, 77);
			ButtonOpenItemListFile.Name = "ButtonOpenItemListFile";
			ButtonOpenItemListFile.Size = new Size(126, 58);
			ButtonOpenItemListFile.TabIndex = 0;
			ButtonOpenItemListFile.Text = "(1) Choose File";
			ButtonOpenItemListFile.UseVisualStyleBackColor = true;
			ButtonOpenItemListFile.Click += ButtonOpenItemListFile_Click;
			// 
			// StatusBar
			// 
			StatusBar.Items.AddRange(new ToolStripItem[] { StatusBarLabel, StatusBarEstimate });
			StatusBar.Location = new Point(0, 476);
			StatusBar.Name = "StatusBar";
			StatusBar.Size = new Size(741, 22);
			StatusBar.TabIndex = 1;
			// 
			// StatusBarLabel
			// 
			StatusBarLabel.Name = "StatusBarLabel";
			StatusBarLabel.Size = new Size(0, 17);
			// 
			// StatusBarEstimate
			// 
			StatusBarEstimate.Name = "StatusBarEstimate";
			StatusBarEstimate.Size = new Size(0, 17);
			// 
			// ButtonParseItemListFile
			// 
			ButtonParseItemListFile.Location = new Point(144, 77);
			ButtonParseItemListFile.Name = "ButtonParseItemListFile";
			ButtonParseItemListFile.Size = new Size(126, 58);
			ButtonParseItemListFile.TabIndex = 2;
			ButtonParseItemListFile.Text = "(2) Parse File";
			ButtonParseItemListFile.UseVisualStyleBackColor = true;
			ButtonParseItemListFile.Click += ButtonParseItemListFile_Click;
			// 
			// TierManagerButton
			// 
			TierManagerButton.Location = new Point(144, 141);
			TierManagerButton.Name = "TierManagerButton";
			TierManagerButton.Size = new Size(126, 58);
			TierManagerButton.TabIndex = 3;
			TierManagerButton.Text = "(4) Manage tiers and weights";
			TierManagerButton.UseVisualStyleBackColor = true;
			TierManagerButton.Click += TierManagerButton_Click;
			// 
			// ShowItemsDataButton
			// 
			ShowItemsDataButton.Location = new Point(78, 13);
			ShowItemsDataButton.Name = "ShowItemsDataButton";
			ShowItemsDataButton.Size = new Size(126, 58);
			ShowItemsDataButton.TabIndex = 4;
			ShowItemsDataButton.Text = "Display items data";
			ShowItemsDataButton.UseVisualStyleBackColor = true;
			ShowItemsDataButton.Click += ShowItemsDataButton_Click;
			// 
			// ButtonComputeCombinations
			// 
			ButtonComputeCombinations.Location = new Point(12, 205);
			ButtonComputeCombinations.Name = "ButtonComputeCombinations";
			ButtonComputeCombinations.Size = new Size(126, 58);
			ButtonComputeCombinations.TabIndex = 5;
			ButtonComputeCombinations.Text = "(5) Compute combinations";
			ButtonComputeCombinations.UseVisualStyleBackColor = true;
			ButtonComputeCombinations.Click += ButtonComputeCombinations_Click;
			// 
			// TextboxDisplay
			// 
			TextboxDisplay.BorderStyle = BorderStyle.FixedSingle;
			TextboxDisplay.Dock = DockStyle.Fill;
			TextboxDisplay.Location = new Point(296, 165);
			TextboxDisplay.MaxLength = int.MaxValue;
			TextboxDisplay.Multiline = true;
			TextboxDisplay.Name = "TextboxDisplay";
			TextboxDisplay.ReadOnly = true;
			TextboxDisplay.ScrollBars = ScrollBars.Both;
			TextboxDisplay.Size = new Size(445, 311);
			TextboxDisplay.TabIndex = 6;
			// 
			// ButtonManageCustomValidator
			// 
			ButtonManageCustomValidator.Location = new Point(12, 141);
			ButtonManageCustomValidator.Name = "ButtonManageCustomValidator";
			ButtonManageCustomValidator.Size = new Size(126, 58);
			ButtonManageCustomValidator.TabIndex = 7;
			ButtonManageCustomValidator.Text = "(3) Manage custom validator";
			ButtonManageCustomValidator.UseVisualStyleBackColor = true;
			ButtonManageCustomValidator.Click += ButtonManageCustomValidator_Click;
			// 
			// PanelButtons
			// 
			PanelButtons.BorderStyle = BorderStyle.FixedSingle;
			PanelButtons.Controls.Add(ShowScoredCombinationsButton);
			PanelButtons.Controls.Add(ButtonOpenItemListFile);
			PanelButtons.Controls.Add(ShowItemsDataButton);
			PanelButtons.Controls.Add(ButtonParseItemListFile);
			PanelButtons.Controls.Add(ButtonManageCustomValidator);
			PanelButtons.Controls.Add(TierManagerButton);
			PanelButtons.Controls.Add(ButtonComputeCombinations);
			PanelButtons.Dock = DockStyle.Left;
			PanelButtons.Location = new Point(0, 165);
			PanelButtons.Name = "PanelButtons";
			PanelButtons.Size = new Size(296, 311);
			PanelButtons.TabIndex = 9;
			// 
			// ShowScoredCombinationsButton
			// 
			ShowScoredCombinationsButton.Location = new Point(144, 205);
			ShowScoredCombinationsButton.Name = "ShowScoredCombinationsButton";
			ShowScoredCombinationsButton.Size = new Size(126, 58);
			ShowScoredCombinationsButton.TabIndex = 8;
			ShowScoredCombinationsButton.Text = "(6) Display scored combinations";
			ShowScoredCombinationsButton.UseVisualStyleBackColor = true;
			ShowScoredCombinationsButton.Click += ShowScoredCombinationsButton_Click;
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
			PanelConfig.BorderStyle = BorderStyle.FixedSingle;
			PanelConfig.Controls.Add(NumericBestCombinationsCount);
			PanelConfig.Controls.Add(GroupBoxStrategy);
			PanelConfig.Controls.Add(LabelBestCombinationsCount);
			PanelConfig.Dock = DockStyle.Top;
			PanelConfig.Location = new Point(0, 27);
			PanelConfig.Name = "PanelConfig";
			PanelConfig.Size = new Size(741, 138);
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
			// LabelBestCombinationsCount
			// 
			LabelBestCombinationsCount.BorderStyle = BorderStyle.FixedSingle;
			LabelBestCombinationsCount.Location = new Point(302, 11);
			LabelBestCombinationsCount.Name = "LabelBestCombinationsCount";
			LabelBestCombinationsCount.Size = new Size(155, 21);
			LabelBestCombinationsCount.TabIndex = 11;
			LabelBestCombinationsCount.Text = "Best combinations count:";
			LabelBestCombinationsCount.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// MenuStrip
			// 
			MenuStrip.BackColor = Color.LightBlue;
			MenuStrip.Items.AddRange(new ToolStripItem[] { LoadConfigMenuButton, SaveConfigMenuButton });
			MenuStrip.Location = new Point(0, 0);
			MenuStrip.Name = "MenuStrip";
			MenuStrip.Size = new Size(741, 27);
			MenuStrip.TabIndex = 12;
			// 
			// LoadConfigMenuButton
			// 
			LoadConfigMenuButton.AutoToolTip = true;
			LoadConfigMenuButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			LoadConfigMenuButton.Font = new Font("Nirmala UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
			LoadConfigMenuButton.ForeColor = Color.FromArgb(192, 0, 0);
			LoadConfigMenuButton.Name = "LoadConfigMenuButton";
			LoadConfigMenuButton.Size = new Size(100, 23);
			LoadConfigMenuButton.Text = "Load config";
			LoadConfigMenuButton.ToolTipText = "Load the configured tiers and validation groups/operations";
			LoadConfigMenuButton.Click += LoadConfigMenuButton_Click;
			// 
			// SaveConfigMenuButton
			// 
			SaveConfigMenuButton.AutoToolTip = true;
			SaveConfigMenuButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
			SaveConfigMenuButton.Font = new Font("Nirmala UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
			SaveConfigMenuButton.ForeColor = Color.FromArgb(192, 0, 0);
			SaveConfigMenuButton.Name = "SaveConfigMenuButton";
			SaveConfigMenuButton.Size = new Size(99, 23);
			SaveConfigMenuButton.Text = "Save config";
			SaveConfigMenuButton.ToolTipText = "Save the configured tiers and validation groups/operations";
			SaveConfigMenuButton.Click += SaveConfigMenuButton_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			ClientSize = new Size(741, 498);
			Controls.Add(TextboxDisplay);
			Controls.Add(PanelButtons);
			Controls.Add(PanelConfig);
			Controls.Add(StatusBar);
			Controls.Add(MenuStrip);
			DoubleBuffered = true;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MainMenuStrip = MenuStrip;
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
			MenuStrip.ResumeLayout(false);
			MenuStrip.PerformLayout();
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
		private Label LabelBestCombinationsCount;
		private MenuStrip MenuStrip;
		private ToolStripMenuItem LoadConfigMenuButton;
		private ToolStripMenuItem SaveConfigMenuButton;
		private Button ShowScoredCombinationsButton;
	}
}
