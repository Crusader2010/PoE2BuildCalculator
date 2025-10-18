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
			panel3 = new Panel();
			panel2 = new Panel();
			panel1.SuspendLayout();
			panel3.SuspendLayout();
			panel2.SuspendLayout();
			SuspendLayout();
			// 
			// TextboxItemStat
			// 
			TextboxItemStat.Dock = DockStyle.Fill;
			TextboxItemStat.Location = new Point(1, 2);
			TextboxItemStat.Name = "TextboxItemStat";
			TextboxItemStat.ReadOnly = true;
			TextboxItemStat.Size = new Size(251, 21);
			TextboxItemStat.TabIndex = 5;
			// 
			// ComboboxOperator
			// 
			ComboboxOperator.BackColor = Color.FromArgb(192, 255, 192);
			ComboboxOperator.Dock = DockStyle.Left;
			ComboboxOperator.DropDownStyle = ComboBoxStyle.DropDownList;
			ComboboxOperator.Font = new Font("Courier New", 12F, FontStyle.Bold);
			ComboboxOperator.ForeColor = Color.Red;
			ComboboxOperator.ItemHeight = 18;
			ComboboxOperator.Location = new Point(0, 0);
			ComboboxOperator.Margin = new Padding(0);
			ComboboxOperator.MaxDropDownItems = 5;
			ComboboxOperator.Name = "ComboboxOperator";
			ComboboxOperator.Size = new Size(35, 26);
			ComboboxOperator.TabIndex = 6;
			ComboboxOperator.SelectedIndexChanged += ComboboxOperator_SelectedIndexChanged;
			// 
			// ButtonMoveUp
			// 
			ButtonMoveUp.Dock = DockStyle.Right;
			ButtonMoveUp.FlatStyle = FlatStyle.Flat;
			ButtonMoveUp.Font = new Font("Courier New", 10F, FontStyle.Bold);
			ButtonMoveUp.Location = new Point(36, 0);
			ButtonMoveUp.Name = "ButtonMoveUp";
			ButtonMoveUp.Size = new Size(25, 26);
			ButtonMoveUp.TabIndex = 7;
			ButtonMoveUp.Text = "▲";
			ButtonMoveUp.UseVisualStyleBackColor = true;
			ButtonMoveUp.Click += ButtonMoveUp_Click;
			// 
			// ButtonMoveDown
			// 
			ButtonMoveDown.Dock = DockStyle.Right;
			ButtonMoveDown.FlatStyle = FlatStyle.Flat;
			ButtonMoveDown.Font = new Font("Courier New", 10F, FontStyle.Bold);
			ButtonMoveDown.Location = new Point(61, 0);
			ButtonMoveDown.Name = "ButtonMoveDown";
			ButtonMoveDown.Size = new Size(25, 26);
			ButtonMoveDown.TabIndex = 8;
			ButtonMoveDown.Text = "▼";
			ButtonMoveDown.UseVisualStyleBackColor = true;
			ButtonMoveDown.Click += ButtonMoveDown_Click;
			// 
			// ButtonRemove
			// 
			ButtonRemove.BackColor = Color.FromArgb(224, 224, 224);
			ButtonRemove.Dock = DockStyle.Right;
			ButtonRemove.FlatStyle = FlatStyle.Flat;
			ButtonRemove.Font = new Font("Courier New", 10F, FontStyle.Bold);
			ButtonRemove.ForeColor = Color.Red;
			ButtonRemove.Location = new Point(342, 0);
			ButtonRemove.Margin = new Padding(0);
			ButtonRemove.Name = "ButtonRemove";
			ButtonRemove.Size = new Size(25, 28);
			ButtonRemove.TabIndex = 9;
			ButtonRemove.Text = "X";
			ButtonRemove.UseVisualStyleBackColor = true;
			ButtonRemove.Click += ButtonRemove_Click;
			// 
			// panel1
			// 
			panel1.Controls.Add(panel3);
			panel1.Controls.Add(panel2);
			panel1.Controls.Add(ButtonRemove);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(0, 0);
			panel1.Name = "panel1";
			panel1.Size = new Size(367, 28);
			panel1.TabIndex = 10;
			// 
			// panel3
			// 
			panel3.BorderStyle = BorderStyle.FixedSingle;
			panel3.Controls.Add(TextboxItemStat);
			panel3.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			panel3.Location = new Point(0, 0);
			panel3.Margin = new Padding(0);
			panel3.Name = "panel3";
			panel3.Padding = new Padding(1, 2, 1, 4);
			panel3.Size = new Size(255, 28);
			panel3.TabIndex = 11;
			// 
			// panel2
			// 
			panel2.BorderStyle = BorderStyle.FixedSingle;
			panel2.Controls.Add(ComboboxOperator);
			panel2.Controls.Add(ButtonMoveUp);
			panel2.Controls.Add(ButtonMoveDown);
			panel2.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			panel2.Location = new Point(254, 0);
			panel2.Name = "panel2";
			panel2.Size = new Size(88, 28);
			panel2.TabIndex = 10;
			// 
			// ItemStatRow
			// 
			AutoScaleDimensions = new SizeF(96F, 96F);
			AutoScaleMode = AutoScaleMode.Dpi;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Controls.Add(panel1);
			DoubleBuffered = true;
			Name = "ItemStatRow";
			Size = new Size(367, 28);
			Load += ItemStatRow_Load;
			panel1.ResumeLayout(false);
			panel3.ResumeLayout(false);
			panel3.PerformLayout();
			panel2.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private TextBox TextboxItemStat;
        private ComboBox ComboboxOperator;
        private Button ButtonMoveUp;
        private Button ButtonMoveDown;
        private Button ButtonRemove;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
    }
}
