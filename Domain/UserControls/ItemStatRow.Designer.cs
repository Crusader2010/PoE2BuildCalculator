namespace Domain.UserControls
{
	partial class ItemStatRow
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
			TextboxItemStat = new TextBox();
			ComboboxOperator = new ComboBox();
			ButtonMoveUp = new Button();
			ButtonMoveDown = new Button();
			ButtonRemove = new Button();
			panel1 = new Panel();
			panel1.SuspendLayout();
			SuspendLayout();
			// 
			// TextboxItemStat
			// 
			TextboxItemStat.BorderStyle = BorderStyle.FixedSingle;
			TextboxItemStat.Location = new Point(1, 1);
			TextboxItemStat.Name = "TextboxItemStat";
			TextboxItemStat.ReadOnly = true;
			TextboxItemStat.Size = new Size(204, 23);
			TextboxItemStat.TabIndex = 5;
			// 
			// ComboboxOperator
			// 
			ComboboxOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboboxOperator.FormattingEnabled = true;
			ComboboxOperator.Location = new Point(205, 1);
			ComboboxOperator.Name = "ComboboxOperator";
			ComboboxOperator.Size = new Size(29, 23);
			ComboboxOperator.TabIndex = 6;
			ComboboxOperator.SelectedIndexChanged += ComboboxOperator_SelectedIndexChanged;
			// 
			// ButtonMoveUp
			// 
			ButtonMoveUp.FlatStyle = FlatStyle.Flat;
			ButtonMoveUp.Font = new Font("Segoe UI Variable Small", 6F, FontStyle.Bold);
			ButtonMoveUp.Location = new Point(235, 1);
			ButtonMoveUp.Name = "ButtonMoveUp";
			ButtonMoveUp.Size = new Size(25, 23);
			ButtonMoveUp.TabIndex = 7;
			ButtonMoveUp.Text = "▲";
			ButtonMoveUp.UseVisualStyleBackColor = true;
			ButtonMoveUp.Click += ButtonMoveUp_Click;
			// 
			// ButtonMoveDown
			// 
			ButtonMoveDown.FlatStyle = FlatStyle.Flat;
			ButtonMoveDown.Font = new Font("Segoe UI Variable Small", 6F, FontStyle.Bold);
			ButtonMoveDown.Location = new Point(261, 1);
			ButtonMoveDown.Name = "ButtonMoveDown";
			ButtonMoveDown.Size = new Size(25, 23);
			ButtonMoveDown.TabIndex = 8;
			ButtonMoveDown.Text = "▼";
			ButtonMoveDown.UseVisualStyleBackColor = true;
			ButtonMoveDown.Click += ButtonMoveDown_Click;
			// 
			// ButtonRemove
			// 
			ButtonRemove.BackColor = Color.FromArgb(224, 224, 224);
			ButtonRemove.FlatStyle = FlatStyle.Flat;
			ButtonRemove.Font = new Font("Segoe UI Variable Small", 6F, FontStyle.Bold);
			ButtonRemove.ForeColor = Color.Red;
			ButtonRemove.Location = new Point(287, 1);
			ButtonRemove.Margin = new Padding(0);
			ButtonRemove.Name = "ButtonRemove";
			ButtonRemove.Size = new Size(25, 23);
			ButtonRemove.TabIndex = 9;
			ButtonRemove.Text = "X";
			ButtonRemove.UseVisualStyleBackColor = true;
			ButtonRemove.Click += ButtonRemove_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(TextboxItemStat);
			panel1.Controls.Add(ButtonRemove);
			panel1.Controls.Add(ComboboxOperator);
			panel1.Controls.Add(ButtonMoveDown);
			panel1.Controls.Add(ButtonMoveUp);
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(313, 25);
			panel1.TabIndex = 10;
			// 
			// ItemStatRow
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			Controls.Add(panel1);
			DoubleBuffered = true;
			Name = "ItemStatRow";
			Size = new Size(313, 25);
			Load += ItemStatRow_Load;
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			ResumeLayout(false);
		}

		#endregion
		private TextBox TextboxItemStat;
		private ComboBox ComboboxOperator;
		private Button ButtonMoveUp;
		private Button ButtonMoveDown;
		private Button ButtonRemove;
		private Panel panel1;
	}
}
