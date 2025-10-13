namespace Domain.UserControls
{
	partial class GroupValidatorListUserControl
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
			ComboBoxGroup = new ComboBox();
			ComboBoxGroupLevelOperator = new ComboBox();
			GroupBoxOptions = new GroupBox();
			OptionAtLeast = new RadioButton();
			OptionAtMost = new RadioButton();
			OptionSumAll = new RadioButton();
			OptionEachItem = new RadioButton();
			InputBoxItemsCount = new NumericUpDown();
			CheckboxPercentage = new CheckBox();
			label1 = new Label();
			ComboBoxOperatorMin = new ComboBox();
			ComboBoxOperatorMax = new ComboBox();
			CheckboxMin = new CheckBox();
			CheckboxMax = new CheckBox();
			ComboBoxMinMaxOperator = new ComboBox();
			InputBoxMin = new NumericUpDown();
			InputBoxMax = new NumericUpDown();
			PanelMinMax = new Panel();
			PanelItemCount = new Panel();
			label2 = new Label();
			PanelNextGroupOperator = new Panel();
			PanelGroupValidatorList.SuspendLayout();
			GroupBoxOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)InputBoxItemsCount).BeginInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMin).BeginInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMax).BeginInit();
			PanelMinMax.SuspendLayout();
			PanelItemCount.SuspendLayout();
			PanelNextGroupOperator.SuspendLayout();
			SuspendLayout();
			// 
			// PanelGroupValidatorList
			// 
			PanelGroupValidatorList.Controls.Add(PanelNextGroupOperator);
			PanelGroupValidatorList.Controls.Add(PanelMinMax);
			PanelGroupValidatorList.Controls.Add(PanelItemCount);
			PanelGroupValidatorList.Controls.Add(GroupBoxOptions);
			PanelGroupValidatorList.Location = new Point(3, 3);
			PanelGroupValidatorList.Name = "PanelGroupValidatorList";
			PanelGroupValidatorList.Size = new Size(776, 78);
			PanelGroupValidatorList.TabIndex = 0;
			// 
			// ComboBoxGroup
			// 
			ComboBoxGroup.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxGroup.FormattingEnabled = true;
			ComboBoxGroup.Location = new Point(366, 12);
			ComboBoxGroup.Name = "ComboBoxGroup";
			ComboBoxGroup.Size = new Size(160, 23);
			ComboBoxGroup.TabIndex = 0;
			// 
			// ComboBoxGroupLevelOperator
			// 
			ComboBoxGroupLevelOperator.BackColor = Color.PaleTurquoise;
			ComboBoxGroupLevelOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxGroupLevelOperator.Enabled = false;
			ComboBoxGroupLevelOperator.FormattingEnabled = true;
			ComboBoxGroupLevelOperator.Location = new Point(130, 2);
			ComboBoxGroupLevelOperator.Name = "ComboBoxGroupLevelOperator";
			ComboBoxGroupLevelOperator.Size = new Size(104, 23);
			ComboBoxGroupLevelOperator.TabIndex = 1;
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
			GroupBoxOptions.Size = new Size(530, 41);
			GroupBoxOptions.TabIndex = 2;
			GroupBoxOptions.TabStop = false;
			// 
			// OptionAtLeast
			// 
			OptionAtLeast.BackColor = Color.FromArgb(255, 255, 192);
			OptionAtLeast.FlatAppearance.BorderColor = Color.Lime;
			OptionAtLeast.Location = new Point(6, 12);
			OptionAtLeast.Name = "OptionAtLeast";
			OptionAtLeast.Size = new Size(84, 24);
			OptionAtLeast.TabIndex = 0;
			OptionAtLeast.Text = "At least";
			OptionAtLeast.TextAlign = ContentAlignment.MiddleCenter;
			OptionAtLeast.UseVisualStyleBackColor = false;
			// 
			// OptionAtMost
			// 
			OptionAtMost.BackColor = Color.FromArgb(255, 255, 192);
			OptionAtMost.FlatAppearance.BorderColor = Color.Lime;
			OptionAtMost.Location = new Point(96, 12);
			OptionAtMost.Name = "OptionAtMost";
			OptionAtMost.Size = new Size(84, 24);
			OptionAtMost.TabIndex = 1;
			OptionAtMost.Text = "At most";
			OptionAtMost.TextAlign = ContentAlignment.MiddleCenter;
			OptionAtMost.UseVisualStyleBackColor = false;
			// 
			// OptionSumAll
			// 
			OptionSumAll.BackColor = Color.FromArgb(255, 255, 192);
			OptionSumAll.Checked = true;
			OptionSumAll.FlatAppearance.BorderColor = Color.Lime;
			OptionSumAll.Location = new Point(276, 12);
			OptionSumAll.Name = "OptionSumAll";
			OptionSumAll.Size = new Size(84, 24);
			OptionSumAll.TabIndex = 2;
			OptionSumAll.TabStop = true;
			OptionSumAll.Text = "SUM(all)";
			OptionSumAll.TextAlign = ContentAlignment.MiddleCenter;
			OptionSumAll.UseVisualStyleBackColor = false;
			// 
			// OptionEachItem
			// 
			OptionEachItem.BackColor = Color.FromArgb(255, 255, 192);
			OptionEachItem.FlatAppearance.BorderColor = Color.Lime;
			OptionEachItem.Location = new Point(186, 12);
			OptionEachItem.Name = "OptionEachItem";
			OptionEachItem.Size = new Size(84, 24);
			OptionEachItem.TabIndex = 3;
			OptionEachItem.Text = "Each item";
			OptionEachItem.TextAlign = ContentAlignment.MiddleCenter;
			OptionEachItem.UseVisualStyleBackColor = false;
			// 
			// InputBoxItemsCount
			// 
			InputBoxItemsCount.BorderStyle = BorderStyle.FixedSingle;
			InputBoxItemsCount.Location = new Point(3, 3);
			InputBoxItemsCount.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxItemsCount.Name = "InputBoxItemsCount";
			InputBoxItemsCount.Size = new Size(82, 23);
			InputBoxItemsCount.TabIndex = 5;
			InputBoxItemsCount.TextAlign = HorizontalAlignment.Center;
			// 
			// CheckboxPercentage
			// 
			CheckboxPercentage.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxPercentage.BackgroundImageLayout = ImageLayout.Center;
			CheckboxPercentage.FlatAppearance.BorderColor = Color.FromArgb(255, 128, 128);
			CheckboxPercentage.ForeColor = Color.Black;
			CheckboxPercentage.Location = new Point(132, 3);
			CheckboxPercentage.Name = "CheckboxPercentage";
			CheckboxPercentage.Size = new Size(102, 23);
			CheckboxPercentage.TabIndex = 6;
			CheckboxPercentage.Text = "Is Percentage";
			CheckboxPercentage.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxPercentage.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			label1.BorderStyle = BorderStyle.FixedSingle;
			label1.Font = new Font("Segoe UI", 8F);
			label1.Location = new Point(85, 3);
			label1.Name = "label1";
			label1.Size = new Size(45, 23);
			label1.TabIndex = 8;
			label1.Text = "items";
			label1.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// ComboBoxOperatorMin
			// 
			ComboBoxOperatorMin.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxOperatorMin.FormattingEnabled = true;
			ComboBoxOperatorMin.Location = new Point(70, 3);
			ComboBoxOperatorMin.Name = "ComboBoxOperatorMin";
			ComboBoxOperatorMin.Size = new Size(59, 23);
			ComboBoxOperatorMin.TabIndex = 3;
			// 
			// ComboBoxOperatorMax
			// 
			ComboBoxOperatorMax.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxOperatorMax.Enabled = false;
			ComboBoxOperatorMax.FormattingEnabled = true;
			ComboBoxOperatorMax.Location = new Point(372, 3);
			ComboBoxOperatorMax.Name = "ComboBoxOperatorMax";
			ComboBoxOperatorMax.Size = new Size(59, 23);
			ComboBoxOperatorMax.TabIndex = 8;
			// 
			// CheckboxMin
			// 
			CheckboxMin.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxMin.Checked = true;
			CheckboxMin.CheckState = CheckState.Checked;
			CheckboxMin.Location = new Point(6, 3);
			CheckboxMin.Name = "CheckboxMin";
			CheckboxMin.Size = new Size(58, 23);
			CheckboxMin.TabIndex = 10;
			CheckboxMin.Text = "Min";
			CheckboxMin.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxMin.UseVisualStyleBackColor = false;
			// 
			// CheckboxMax
			// 
			CheckboxMax.BackColor = Color.FromArgb(224, 224, 224);
			CheckboxMax.Location = new Point(310, 3);
			CheckboxMax.Name = "CheckboxMax";
			CheckboxMax.Size = new Size(58, 23);
			CheckboxMax.TabIndex = 11;
			CheckboxMax.Text = "Max";
			CheckboxMax.TextAlign = ContentAlignment.MiddleCenter;
			CheckboxMax.UseVisualStyleBackColor = false;
			// 
			// ComboBoxMinMaxOperator
			// 
			ComboBoxMinMaxOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboBoxMinMaxOperator.FormattingEnabled = true;
			ComboBoxMinMaxOperator.Items.AddRange(new object[] { "-", "AND", "OR" });
			ComboBoxMinMaxOperator.Location = new Point(223, 3);
			ComboBoxMinMaxOperator.MaxDropDownItems = 5;
			ComboBoxMinMaxOperator.Name = "ComboBoxMinMaxOperator";
			ComboBoxMinMaxOperator.Size = new Size(81, 23);
			ComboBoxMinMaxOperator.TabIndex = 12;
			ComboBoxMinMaxOperator.SelectedIndexChanged += ComboBoxEachItemOperator_SelectedIndexChanged;
			// 
			// InputBoxMin
			// 
			InputBoxMin.BorderStyle = BorderStyle.FixedSingle;
			InputBoxMin.DecimalPlaces = 2;
			InputBoxMin.Location = new Point(135, 3);
			InputBoxMin.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxMin.Name = "InputBoxMin";
			InputBoxMin.Size = new Size(82, 23);
			InputBoxMin.TabIndex = 9;
			InputBoxMin.TextAlign = HorizontalAlignment.Center;
			// 
			// InputBoxMax
			// 
			InputBoxMax.BorderStyle = BorderStyle.FixedSingle;
			InputBoxMax.DecimalPlaces = 2;
			InputBoxMax.Enabled = false;
			InputBoxMax.Location = new Point(437, 3);
			InputBoxMax.Maximum = new decimal(new int[] { 99999999, 0, 0, 0 });
			InputBoxMax.Name = "InputBoxMax";
			InputBoxMax.Size = new Size(82, 23);
			InputBoxMax.TabIndex = 12;
			InputBoxMax.TextAlign = HorizontalAlignment.Center;
			// 
			// PanelMinMax
			// 
			PanelMinMax.Controls.Add(ComboBoxMinMaxOperator);
			PanelMinMax.Controls.Add(CheckboxMin);
			PanelMinMax.Controls.Add(InputBoxMax);
			PanelMinMax.Controls.Add(ComboBoxOperatorMin);
			PanelMinMax.Controls.Add(InputBoxMin);
			PanelMinMax.Controls.Add(ComboBoxOperatorMax);
			PanelMinMax.Controls.Add(CheckboxMax);
			PanelMinMax.Location = new Point(4, 44);
			PanelMinMax.Name = "PanelMinMax";
			PanelMinMax.Size = new Size(530, 30);
			PanelMinMax.TabIndex = 1;
			// 
			// PanelItemCount
			// 
			PanelItemCount.Controls.Add(InputBoxItemsCount);
			PanelItemCount.Controls.Add(label1);
			PanelItemCount.Controls.Add(CheckboxPercentage);
			PanelItemCount.Location = new Point(536, 44);
			PanelItemCount.Name = "PanelItemCount";
			PanelItemCount.Size = new Size(238, 30);
			PanelItemCount.TabIndex = 9;
			PanelItemCount.Visible = false;
			// 
			// label2
			// 
			label2.BorderStyle = BorderStyle.FixedSingle;
			label2.Font = new Font("Segoe UI", 8F);
			label2.Location = new Point(3, 2);
			label2.Name = "label2";
			label2.Size = new Size(121, 23);
			label2.TabIndex = 10;
			label2.Text = "Next group operator";
			label2.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// PanelNextGroupOperator
			// 
			PanelNextGroupOperator.Controls.Add(label2);
			PanelNextGroupOperator.Controls.Add(ComboBoxGroupLevelOperator);
			PanelNextGroupOperator.Location = new Point(536, 13);
			PanelNextGroupOperator.Name = "PanelNextGroupOperator";
			PanelNextGroupOperator.Size = new Size(238, 28);
			PanelNextGroupOperator.TabIndex = 11;
			// 
			// GroupValidatorListUserControl
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			Controls.Add(PanelGroupValidatorList);
			DoubleBuffered = true;
			Name = "GroupValidatorListUserControl";
			Size = new Size(781, 82);
			PanelGroupValidatorList.ResumeLayout(false);
			GroupBoxOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)InputBoxItemsCount).EndInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMin).EndInit();
			((System.ComponentModel.ISupportInitialize)InputBoxMax).EndInit();
			PanelMinMax.ResumeLayout(false);
			PanelItemCount.ResumeLayout(false);
			PanelNextGroupOperator.ResumeLayout(false);
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
		private Label label1;
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
	}
}
