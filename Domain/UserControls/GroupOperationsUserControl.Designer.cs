namespace Domain.UserControls
{
	partial class GroupOperationsUserControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			PanelGroupValidatorList = new Panel();
			ButtonDeleteOperation = new Button();
			PanelNextGroupOperator = new Panel();
			label2 = new Label();
			ComboBoxGroupLevelOperator = new ComboBox();
			PanelMinMax = new Panel();
			ComboBoxMinMaxOperator = new ComboBox();
			CheckboxMin = new CheckBox();
			InputBoxMax = new NumericUpDown();
			ComboBoxOperatorMin = new ComboBox();
			InputBoxMin = new NumericUpDown();
			ComboBoxOperatorMax = new ComboBox();
			CheckboxMax = new CheckBox();
			PanelItemCount = new Panel();
			InputBoxItemsCount = new NumericUpDown();
			lblItems = new Label();
			CheckboxPercentage = new CheckBox();
			GroupBoxOptions = new GroupBox();
			ComboBoxGroup = new ComboBox();
			OptionEachItem = new RadioButton();
			OptionSumAll = new RadioButton();
			OptionAtMost = new RadioButton();
			OptionAtLeast = new RadioButton();
			PanelGroupValidatorList.SuspendLayout();
			PanelNextGroupOperator.SuspendLayout();
			PanelMinMax.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)InputBoxMax).BeginInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMin).BeginInit();
			PanelItemCount.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)InputBoxItemsCount).BeginInit();
			GroupBoxOptions.SuspendLayout();
			SuspendLayout();
			// 
			// PanelGroupValidatorList
			// 
			PanelGroupValidatorList.Controls.Add(ButtonDeleteOperation);
			PanelGroupValidatorList.Controls.Add(PanelNextGroupOperator);
			PanelGroupValidatorList.Controls.Add(PanelMinMax);
			PanelGroupValidatorList.Controls.Add(PanelItemCount);
			PanelGroupValidatorList.Controls.Add(GroupBoxOptions);
			PanelGroupValidatorList.Location = new Point(-1, -1);
			PanelGroupValidatorList.Name = "PanelGroupValidatorList";
			PanelGroupValidatorList.Size = new Size(780, 78);
			PanelGroupValidatorList.TabIndex = 0;
			// 
			// ButtonDeleteOperation
			// 
			ButtonDeleteOperation.BackColor = Color.FromArgb(255, 128, 128);
			ButtonDeleteOperation.BackgroundImageLayout = ImageLayout.Stretch;
			ButtonDeleteOperation.FlatAppearance.BorderColor = Color.Black;
			ButtonDeleteOperation.FlatAppearance.BorderSize = 0;
			ButtonDeleteOperation.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 192, 255);
			ButtonDeleteOperation.FlatStyle = FlatStyle.Flat;
			ButtonDeleteOperation.Font = new Font("Verdana", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
			ButtonDeleteOperation.ForeColor = Color.Blue;
			ButtonDeleteOperation.Location = new Point(730, 11);
			ButtonDeleteOperation.Margin = new Padding(0);
			ButtonDeleteOperation.Name = "ButtonDeleteOperation";
			ButtonDeleteOperation.Size = new Size(40, 23);
			ButtonDeleteOperation.TabIndex = 12;
			ButtonDeleteOperation.Text = "X";
			ButtonDeleteOperation.UseVisualStyleBackColor = false;
			ButtonDeleteOperation.Click += ButtonDeleteOperation_Click;
			// 
			// PanelNextGroupOperator
			// 
			PanelNextGroupOperator.BorderStyle = BorderStyle.FixedSingle;
			PanelNextGroupOperator.Controls.Add(label2);
			PanelNextGroupOperator.Controls.Add(ComboBoxGroupLevelOperator);
			PanelNextGroupOperator.Location = new Point(557, 3);
			PanelNextGroupOperator.Name = "PanelNextGroupOperator";
			PanelNextGroupOperator.Size = new Size(170, 40);
			PanelNextGroupOperator.TabIndex = 11;
			// 
			// label2
			// 
			label2.Font = new Font("Segoe UI", 8F);
			label2.Location = new Point(3, 3);
			label2.Name = "label2";
			label2.Size = new Size(66, 31);
			label2.TabIndex = 10;
			label2.Text = "Next group operator";
			label2.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// ComboBoxGroupLevelOperator
			// 
			ComboBoxGroupLevelOperator.BackColor = Color.PaleTurquoise;
			ComboBoxGroupLevelOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxGroupLevelOperator.Enabled = false;
			ComboBoxGroupLevelOperator.FormattingEnabled = true;
			ComboBoxGroupLevelOperator.Location = new Point(76, 7);
			ComboBoxGroupLevelOperator.Name = "ComboBoxGroupLevelOperator";
			ComboBoxGroupLevelOperator.Size = new Size(89, 23);
			ComboBoxGroupLevelOperator.TabIndex = 1;
			// 
			// PanelMinMax
			// 
			PanelMinMax.BackColor = Color.FromArgb(255, 224, 192);
			PanelMinMax.Controls.Add(ComboBoxMinMaxOperator);
			PanelMinMax.Controls.Add(CheckboxMin);
			PanelMinMax.Controls.Add(InputBoxMax);
			PanelMinMax.Controls.Add(ComboBoxOperatorMin);
			PanelMinMax.Controls.Add(InputBoxMin);
			PanelMinMax.Controls.Add(ComboBoxOperatorMax);
			PanelMinMax.Controls.Add(CheckboxMax);
			PanelMinMax.Location = new Point(248, 45);
			PanelMinMax.Name = "PanelMinMax";
			PanelMinMax.Size = new Size(526, 33);
			PanelMinMax.TabIndex = 1;
			// 
			// ComboBoxMinMaxOperator
			// 
			ComboBoxMinMaxOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxMinMaxOperator.FormattingEnabled = true;
			ComboBoxMinMaxOperator.Location = new Point(223, 4);
			ComboBoxMinMaxOperator.MaxDropDownItems = 5;
			ComboBoxMinMaxOperator.Name = "ComboBoxMinMaxOperator";
			ComboBoxMinMaxOperator.Size = new Size(81, 23);
			ComboBoxMinMaxOperator.TabIndex = 12;
			ComboBoxMinMaxOperator.SelectedIndexChanged += ComboBoxEachItemOperator_SelectedIndexChanged;
			// 
			// CheckboxMin
			// 
			CheckboxMin.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxMin.Checked = true;
			CheckboxMin.CheckState = CheckState.Checked;
			CheckboxMin.Location = new Point(6, 4);
			CheckboxMin.Name = "CheckboxMin";
			CheckboxMin.Size = new Size(58, 23);
			CheckboxMin.TabIndex = 10;
			CheckboxMin.Text = "Min";
			CheckboxMin.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxMin.UseVisualStyleBackColor = false;
			CheckboxMin.CheckedChanged += CheckboxMin_CheckedChanged;
			// 
			// InputBoxMax
			// 
			InputBoxMax.BorderStyle = BorderStyle.FixedSingle;
			InputBoxMax.DecimalPlaces = 2;
			InputBoxMax.Enabled = false;
			InputBoxMax.Location = new Point(440, 4);
			InputBoxMax.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxMax.Name = "InputBoxMax";
			InputBoxMax.Size = new Size(82, 23);
			InputBoxMax.TabIndex = 12;
			InputBoxMax.TextAlign = HorizontalAlignment.Center;
			// 
			// ComboBoxOperatorMin
			// 
			ComboBoxOperatorMin.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxOperatorMin.FormattingEnabled = true;
			ComboBoxOperatorMin.Location = new Point(67, 4);
			ComboBoxOperatorMin.Name = "ComboBoxOperatorMin";
			ComboBoxOperatorMin.Size = new Size(45, 23);
			ComboBoxOperatorMin.TabIndex = 3;
			// 
			// InputBoxMin
			// 
			InputBoxMin.BorderStyle = BorderStyle.FixedSingle;
			InputBoxMin.DecimalPlaces = 2;
			InputBoxMin.Location = new Point(115, 4);
			InputBoxMin.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxMin.Name = "InputBoxMin";
			InputBoxMin.Size = new Size(82, 23);
			InputBoxMin.TabIndex = 9;
			InputBoxMin.TextAlign = HorizontalAlignment.Center;
			// 
			// ComboBoxOperatorMax
			// 
			ComboBoxOperatorMax.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxOperatorMax.Enabled = false;
			ComboBoxOperatorMax.FormattingEnabled = true;
			ComboBoxOperatorMax.Location = new Point(392, 4);
			ComboBoxOperatorMax.Name = "ComboBoxOperatorMax";
			ComboBoxOperatorMax.Size = new Size(45, 23);
			ComboBoxOperatorMax.TabIndex = 8;
			// 
			// CheckboxMax
			// 
			CheckboxMax.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxMax.Location = new Point(331, 4);
			CheckboxMax.Name = "CheckboxMax";
			CheckboxMax.Size = new Size(58, 23);
			CheckboxMax.TabIndex = 11;
			CheckboxMax.Text = "Max";
			CheckboxMax.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxMax.UseVisualStyleBackColor = false;
			CheckboxMax.CheckedChanged += CheckboxMax_CheckedChanged;
			// 
			// PanelItemCount
			// 
			PanelItemCount.BackColor = Color.FromArgb(255, 255, 192);
			PanelItemCount.Controls.Add(InputBoxItemsCount);
			PanelItemCount.Controls.Add(lblItems);
			PanelItemCount.Controls.Add(CheckboxPercentage);
			PanelItemCount.Location = new Point(4, 45);
			PanelItemCount.Name = "PanelItemCount";
			PanelItemCount.Size = new Size(244, 33);
			PanelItemCount.TabIndex = 9;
			PanelItemCount.Visible = false;
			// 
			// InputBoxItemsCount
			// 
			InputBoxItemsCount.BorderStyle = BorderStyle.FixedSingle;
			InputBoxItemsCount.Location = new Point(4, 5);
			InputBoxItemsCount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxItemsCount.Name = "InputBoxItemsCount";
			InputBoxItemsCount.Size = new Size(81, 23);
			InputBoxItemsCount.TabIndex = 5;
			InputBoxItemsCount.TextAlign = HorizontalAlignment.Center;
			// 
			// lblItems
			// 
			lblItems.BackColor = Color.Transparent;
			lblItems.BorderStyle = BorderStyle.FixedSingle;
			lblItems.Font = new Font("Segoe UI", 8F);
			lblItems.Location = new Point(143, 5);
			lblItems.Name = "lblItems";
			lblItems.Size = new Size(97, 23);
			lblItems.TabIndex = 8;
			lblItems.Text = "Number of items";
			lblItems.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// CheckboxPercentage
			// 
			CheckboxPercentage.AutoSize = true;
			CheckboxPercentage.BackColor = Color.Transparent;
			CheckboxPercentage.BackgroundImageLayout = ImageLayout.Center;
			CheckboxPercentage.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
			CheckboxPercentage.ForeColor = Color.Black;
			CheckboxPercentage.Location = new Point(88, 7);
			CheckboxPercentage.Margin = new Padding(1);
			CheckboxPercentage.Name = "CheckboxPercentage";
			CheckboxPercentage.Size = new Size(50, 19);
			CheckboxPercentage.TabIndex = 6;
			CheckboxPercentage.Text = "as %";
			CheckboxPercentage.UseVisualStyleBackColor = false;
			// 
			// GroupBoxOptions
			// 
			GroupBoxOptions.Controls.Add(ComboBoxGroup);
			GroupBoxOptions.Controls.Add(OptionEachItem);
			GroupBoxOptions.Controls.Add(OptionSumAll);
			GroupBoxOptions.Controls.Add(OptionAtMost);
			GroupBoxOptions.Controls.Add(OptionAtLeast);
			GroupBoxOptions.Location = new Point(4, 2);
			GroupBoxOptions.Name = "GroupBoxOptions";
			GroupBoxOptions.Padding = new Padding(0);
			GroupBoxOptions.Size = new Size(551, 41);
			GroupBoxOptions.TabIndex = 2;
			GroupBoxOptions.TabStop = false;
			// 
			// ComboBoxGroup
			// 
			ComboBoxGroup.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxGroup.FormattingEnabled = true;
			ComboBoxGroup.Location = new Point(351, 12);
			ComboBoxGroup.Name = "ComboBoxGroup";
			ComboBoxGroup.Size = new Size(197, 23);
			ComboBoxGroup.TabIndex = 0;
			ComboBoxGroup.DropDown += ComboBoxGroup_DropDown;
			// 
			// OptionEachItem
			// 
			OptionEachItem.BackColor = Color.FromArgb(255, 224, 192);
			OptionEachItem.FlatAppearance.BorderColor = Color.Lime;
			OptionEachItem.Location = new Point(178, 12);
			OptionEachItem.Name = "OptionEachItem";
			OptionEachItem.Size = new Size(81, 24);
			OptionEachItem.TabIndex = 3;
			OptionEachItem.Text = "Each item";
			OptionEachItem.TextAlign = ContentAlignment.MiddleCenter;
			OptionEachItem.UseVisualStyleBackColor = false;
			OptionEachItem.CheckedChanged += OptionEachItem_CheckedChanged;
			// 
			// OptionSumAll
			// 
			OptionSumAll.BackColor = Color.FromArgb(255, 224, 192);
			OptionSumAll.Checked = true;
			OptionSumAll.FlatAppearance.BorderColor = Color.Lime;
			OptionSumAll.Location = new Point(265, 12);
			OptionSumAll.Name = "OptionSumAll";
			OptionSumAll.Size = new Size(81, 24);
			OptionSumAll.TabIndex = 2;
			OptionSumAll.TabStop = true;
			OptionSumAll.Text = "SUM(all)";
			OptionSumAll.TextAlign = ContentAlignment.MiddleCenter;
			OptionSumAll.UseVisualStyleBackColor = false;
			OptionSumAll.CheckedChanged += OptionSumAll_CheckedChanged;
			// 
			// OptionAtMost
			// 
			OptionAtMost.BackColor = Color.FromArgb(255, 255, 192);
			OptionAtMost.FlatAppearance.BorderColor = Color.Lime;
			OptionAtMost.Location = new Point(91, 12);
			OptionAtMost.Name = "OptionAtMost";
			OptionAtMost.Size = new Size(81, 24);
			OptionAtMost.TabIndex = 1;
			OptionAtMost.Text = "At most";
			OptionAtMost.TextAlign = ContentAlignment.MiddleCenter;
			OptionAtMost.UseVisualStyleBackColor = false;
			OptionAtMost.CheckedChanged += OptionAtMost_CheckedChanged;
			// 
			// OptionAtLeast
			// 
			OptionAtLeast.BackColor = Color.FromArgb(255, 255, 192);
			OptionAtLeast.FlatAppearance.BorderColor = Color.Lime;
			OptionAtLeast.Location = new Point(4, 12);
			OptionAtLeast.Name = "OptionAtLeast";
			OptionAtLeast.Size = new Size(81, 24);
			OptionAtLeast.TabIndex = 0;
			OptionAtLeast.Text = "At least";
			OptionAtLeast.TextAlign = ContentAlignment.MiddleCenter;
			OptionAtLeast.UseVisualStyleBackColor = false;
			OptionAtLeast.CheckedChanged += OptionAtLeast_CheckedChanged;
			// 
			// GroupOperationsUserControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			BackColor = Color.LightGray;
			BorderStyle = BorderStyle.FixedSingle;
			Controls.Add(PanelGroupValidatorList);
			DoubleBuffered = true;
			Margin = new Padding(1, 1, 2, 2);
			Name = "GroupOperationsUserControl";
			Size = new Size(774, 76);
			Load += GroupOperationsUserControl_Load;
			PanelGroupValidatorList.ResumeLayout(false);
			PanelNextGroupOperator.ResumeLayout(false);
			PanelMinMax.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)InputBoxMax).EndInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMin).EndInit();
			PanelItemCount.ResumeLayout(false);
			PanelItemCount.PerformLayout();
			((System.ComponentModel.ISupportInitialize)InputBoxItemsCount).EndInit();
			GroupBoxOptions.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private Panel PanelGroupValidatorList;
		private ComboBox ComboBoxGroup;
		private GroupBox GroupBoxOptions;
		private RadioButton OptionEachItem;
		private RadioButton OptionSumAll;
		private RadioButton OptionAtMost;
		private RadioButton OptionAtLeast;
		private ComboBox ComboBoxGroupLevelOperator;
		private NumericUpDown InputBoxItemsCount;
		private CheckBox CheckboxPercentage;
		private Label lblItems;
		private CheckBox CheckboxMax;
		private CheckBox CheckboxMin;
		private ComboBox ComboBoxOperatorMax;
		private ComboBox ComboBoxOperatorMin;
		private ComboBox ComboBoxMinMaxOperator;
		private NumericUpDown InputBoxMax;
		private NumericUpDown InputBoxMin;
		private Panel PanelMinMax;
		private Panel PanelItemCount;
		private Label label2;
		private Panel PanelNextGroupOperator;
		private Button ButtonDeleteOperation;
	}
}
