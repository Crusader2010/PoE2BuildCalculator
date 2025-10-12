using Domain.Main;
using Domain.Static;
using Domain.Validation;

using System.Reflection;

namespace Domain.UserControls
{
	public partial class ItemStatGroupValidatorUserControl : UserControl
	{
		private ValidationGroupModel _group;
		private static readonly Color HEADER_COLOR = Color.FromArgb(70, 130, 180);

		// Cached data - static to share across all instances
		private static readonly Lazy<IReadOnlyList<PropertyInfo>> _availableProperties = new(() =>
		{
			return [.. typeof(ItemStats).GetProperties()
				.Where(p => p.PropertyType == typeof(int) ||
							p.PropertyType == typeof(double) ||
							p.PropertyType == typeof(long))
				.OrderBy(p => p.Name)];
		});

		// Cache for combo box state restoration
		private readonly Dictionary<string, int> _comboBoxIndexCache = [];

		public event EventHandler DeleteRequested;
		public event EventHandler ValidationChanged;

		public ValidationGroupModel Group
		{
			get => _group;
			set
			{
				_group = value;
				UpdateDisplay();
			}
		}

		public ItemStatGroupValidatorUserControl()
		{
			InitializeComponent();
			InitializeComboBoxCache();
		}

		private void ItemStatGroupValidatorUserControl_Load(object sender, EventArgs e)
		{
			if (_group != null)
			{
				UpdateDisplay();
			}

			numMin.Leave += NumericInput_Leave;
			numMax.Leave += NumericInput_Leave;
		}

		private void InitializeComboBoxCache()
		{
			for (int i = 0; i < _availableProperties.Value.Count; i++)
			{
				_comboBoxIndexCache[_availableProperties.Value[i].Name] = i;
			}
		}

		private void UpdateDisplay()
		{
			if (_group == null) return;

			SuspendLayout();
			try
			{
				lblGroupName.Text = _group.GroupName;

				chkMin.Checked = _group.IsMinEnabled;
				numMin.Value = (decimal)(_group.MinValue ?? 0.00);
				numMin.Enabled = _group.IsMinEnabled;

				chkMax.Checked = _group.IsMaxEnabled;
				numMax.Value = (decimal)(_group.MaxValue ?? 0.00);
				numMax.Enabled = _group.IsMaxEnabled;

				UpdateStatsComboBox();
				RefreshStatsListBox();

				cmbOperator.SelectedItem = _group.GroupOperator ?? "AND";
				ValidateGroup();
			}
			finally
			{
				ResumeLayout(true);
			}
		}

		private void UpdateStatsComboBox()
		{
			var usedStats = new HashSet<string>(_group.Stats.Select(s => s.PropertyName), StringComparer.OrdinalIgnoreCase);

			cmbStats.BeginUpdate();
			try
			{
				cmbStats.Items.Clear();

				// Use LINQ for efficient filtering and ordering
				var availableItems = _availableProperties.Value
					.Where(p => !usedStats.Contains(p.Name))
					.Select(p => p.Name)
					.ToArray();

				cmbStats.Items.AddRange(availableItems);
			}
			finally
			{
				cmbStats.EndUpdate();
			}
		}

		private void RefreshStatsListBox()
		{
			statsListBox.BeginUpdate();
			try
			{
				statsListBox.Items.Clear();
				statsListBox.Items.AddRange([.. _group.Stats.Select(s => s.PropertyName)]);
			}
			finally
			{
				statsListBox.EndUpdate();
			}
		}

		private void ValidateGroup()
		{
			bool isValid = true;
			string errorMsg = "";

			if (!_group.IsMinEnabled && !_group.IsMaxEnabled)
			{
				errorMsg = "Enable Min or Max";
				isValid = false;
			}
			else if (_group.IsMinEnabled && _group.IsMaxEnabled &&
					 _group.MinValue.HasValue && _group.MaxValue.HasValue &&
					 _group.MinValue.Value > _group.MaxValue.Value)
			{
				errorMsg = "Min must be < Max";
				isValid = false;
			}

			lblValidation.Text = errorMsg;
			lblValidation.ForeColor = isValid ? Color.Green : Color.Red;

			ValidationChanged?.Invoke(this, EventArgs.Empty);
		}

		public void UpdateOperatorVisibility(bool visible)
		{
			pnlOperatorContainer.Visible = visible;
			cmbOperator.Enabled = visible;
		}

		private static void NumericInput_Leave(object sender, EventArgs e)
		{
			if (sender is NumericUpDown nud && nud.Parent is ItemStatGroupValidatorUserControl uc)
			{
				uc.ValidateGroup();
			}
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			DeleteRequested?.Invoke(this, EventArgs.Empty);
		}

		private void btnAddStat_Click(object sender, EventArgs e)
		{
			if (cmbStats.SelectedItem is null)
			{
				MessageBox.Show("Please select a stat.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			string propName = cmbStats.SelectedItem.ToString();
			var propInfo = _availableProperties.Value.First(p => p.Name == propName);

			_group.Stats.Add(new GroupStatModel
			{
				PropInfo = propInfo,
				PropertyName = propName,
				Operator = "+"
			});

			// Remove from combo using cached index for efficiency
			int selectedIndex = cmbStats.SelectedIndex;
			cmbStats.Items.RemoveAt(selectedIndex);
			cmbStats.SelectedIndex = -1;

			RefreshStatsListBox();
			ValidateGroup();
		}

		private void chkMin_CheckedChanged(object sender, EventArgs e)
		{
			_group.IsMinEnabled = chkMin.Checked;
			numMin.Enabled = chkMin.Checked;
			ValidateGroup();
		}

		private void numMin_ValueChanged(object sender, EventArgs e)
		{
			_group.MinValue = (double)numMin.Value;
			ValidateGroup();
		}

		private void numMin_Leave(object sender, EventArgs e)
		{
			ValidateGroup();
		}

		private void chkMax_CheckedChanged(object sender, EventArgs e)
		{
			_group.IsMaxEnabled = chkMax.Checked;
			numMax.Enabled = chkMax.Checked;
			if (chkMax.Checked && !_group.MaxValue.HasValue)
				_group.MaxValue = (double)numMax.Value;
			ValidateGroup();
		}

		private void numMax_ValueChanged(object sender, EventArgs e)
		{
			if (chkMax.Checked)
			{
				_group.MaxValue = (double)numMax.Value;
				ValidateGroup();
			}
		}

		private void numMax_Leave(object sender, EventArgs e)
		{
			ValidateGroup();
		}

		private void cmbOperator_SelectedIndexChanged(object sender, EventArgs e)
		{
			_group.GroupOperator = cmbOperator.SelectedItem?.ToString();
		}

		private void statsListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= _group.Stats.Count) return;

			var stat = _group.Stats[e.Index];
			bool isLastStat = e.Index == _group.Stats.Count - 1;
			var bounds = e.Bounds;

			e.DrawBackground();

			// Draw stat name
			using var brush = new SolidBrush(e.ForeColor);
			var textRect = new Rectangle(bounds.Left + 4, bounds.Top + 9, bounds.Width - 100, bounds.Height);
			e.Graphics.DrawString(stat.PropertyName, e.Font, brush, textRect);

			// Operator dropdown (skip for last stat)
			if (!isLastStat)
			{
				var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 5, 50, bounds.Height - 10);
				DrawOperatorBox(e.Graphics, opRect, stat.Operator);
			}

			// Control buttons
			DrawControlButtons(e.Graphics, bounds);
			e.DrawFocusRectangle();
		}

		private static void DrawOperatorBox(Graphics g, Rectangle rect, string op)
		{
			using var bgBrush = new SolidBrush(Color.FromArgb(240, 240, 240));
			g.FillRectangle(bgBrush, rect);
			g.DrawRectangle(Pens.Gray, rect);

			using var textBrush = new SolidBrush(Color.Black);
			var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
			g.DrawString(op, SystemFonts.DefaultFont, textBrush, rect, sf);
		}

		private static void DrawControlButtons(Graphics g, Rectangle bounds)
		{
			var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 3, 18, 14);
			DrawButton(g, upRect, "▲", Color.FromArgb(100, 150, 200));

			var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 18, 18, 14);
			DrawButton(g, downRect, "▼", Color.FromArgb(100, 150, 200));

			var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 3, 18, 29);
			DrawButton(g, removeRect, "×", Color.FromArgb(200, 50, 50), new Font("Segoe UI", 10, FontStyle.Bold));
		}

		private static void DrawButton(Graphics g, Rectangle rect, string text, Color bgColor, Font font = null)
		{
			using var bgBrush = new SolidBrush(bgColor);
			g.FillRectangle(bgBrush, rect);
			g.DrawRectangle(font != null ? Pens.DarkRed : Pens.DarkBlue, rect);

			using var textBrush = new SolidBrush(Color.White);
			var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
			g.DrawString(text, font ?? new Font("Arial", 7), textBrush, rect, sf);
		}

		private void statsListBox_MouseClick(object sender, MouseEventArgs e)
		{
			int index = statsListBox.IndexFromPoint(e.Location);
			if (index < 0 || index >= _group.Stats.Count) return;

			var bounds = statsListBox.GetItemRectangle(index);
			bool isLastStat = index == _group.Stats.Count - 1;

			// Check operator dropdown (skip for last stat)
			// Operator box is at: bounds.Right - 95, width 50
			if (!isLastStat)
			{
				var opRect = new Rectangle(bounds.Right - 95, bounds.Top + 5, 50, bounds.Height - 10);
				if (opRect.Contains(e.Location))
				{
					ShowOperatorMenu(_group.Stats[index], e.Location);
					return;
				}
			}

			// Check up button
			var upRect = new Rectangle(bounds.Right - 42, bounds.Top + 3, 18, 14);
			if (upRect.Contains(e.Location) && index > 0)
			{
				SwapStats(index, index - 1);
				return;
			}

			// Check down button
			var downRect = new Rectangle(bounds.Right - 42, bounds.Top + 18, 18, 14);
			if (downRect.Contains(e.Location) && index < _group.Stats.Count - 1)
			{
				SwapStats(index, index + 1);
				return;
			}

			// Check remove button
			var removeRect = new Rectangle(bounds.Right - 22, bounds.Top + 3, 18, 29);
			if (removeRect.Contains(e.Location))
			{
				RemoveStat(index);
			}
		}

		private void SwapStats(int index1, int index2)
		{
			(_group.Stats[index1], _group.Stats[index2]) = (_group.Stats[index2], _group.Stats[index1]);
			RefreshStatsListBox();
		}

		private void RemoveStat(int index)
		{
			string propName = _group.Stats[index].PropertyName;
			_group.Stats.RemoveAt(index);

			// Restore to combo box at original cached index
			if (_comboBoxIndexCache.TryGetValue(propName, out int originalIndex))
			{
				cmbStats.BeginUpdate();
				try
				{
					// Find correct insertion position based on sorted order
					int insertPos = 0;
					for (int i = 0; i < cmbStats.Items.Count; i++)
					{
						if (_comboBoxIndexCache.TryGetValue(cmbStats.Items[i].ToString(), out int cachedIdx) && cachedIdx < originalIndex)
						{
							insertPos = i + 1;
						}
					}
					cmbStats.Items.Insert(insertPos, propName);
				}
				finally
				{
					cmbStats.EndUpdate();
				}
			}
			else
			{
				// Fallback: re-sort entire list
				UpdateStatsComboBox();
			}

			RefreshStatsListBox();
			ValidateGroup();
		}

		private void ShowOperatorMenu(GroupStatModel stat, Point location)
		{
			var menu = new ContextMenuStrip();

			foreach (var op in Constants.MATH_OPERATORS)
			{
				var item = new ToolStripMenuItem(op) { Checked = stat.Operator == op };
				item.Click += (s, e) =>
				{
					stat.Operator = op;
					statsListBox.Invalidate();
					menu.Close();
				};
				menu.Items.Add(item);
			}

			menu.Show(statsListBox, location);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				numMin.Leave -= NumericInput_Leave;
				numMax.Leave -= NumericInput_Leave;
				components?.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}