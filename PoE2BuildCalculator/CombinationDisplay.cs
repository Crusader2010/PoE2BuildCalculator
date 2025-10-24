using System.Collections.Immutable;

using Domain.Helpers;
using Domain.Main;

using PoE2BuildCalculator.Helpers;

namespace PoE2BuildCalculator
{
	public partial class CombinationDisplay : BaseForm
	{
		private sealed class CombinationViewModel
		{
			public int Rank { get; init; }
			public double Score { get; init; }
			public List<Item> Items { get; init; }
			public Dictionary<string, double> AggregatedStats { get; init; }
		}

		private ImmutableList<CombinationViewModel> _combinationsToDisplay = [];
		private readonly HashSet<string> _tieredStats = [];
		private readonly HashSet<string> _validatorStats = [];
		private IReadOnlyList<ItemStatsHelper.StatDescriptor> _statDescriptors;

		public CombinationDisplay(
			List<(List<Item> Combination, double Score)> scoredCombinations,
			List<Tier> tiers = null,
			List<Domain.Validation.Group> validatorGroups = null)
		{
			InitializeComponent();
			this.Load += CombinationDisplay_Load;

			_statDescriptors = ItemStatsHelper.GetStatDescriptors();
			ExtractRelevantStats(tiers, validatorGroups);
			PrepareCombinationData(scoredCombinations);
		}

		private void ExtractRelevantStats(List<Tier> tiers, List<Domain.Validation.Group> validatorGroups)
		{
			if (tiers != null)
			{
				foreach (var tier in tiers)
				{
					foreach (var statName in tier.StatWeights.Keys.Where(k => tier.StatWeights[k] > 0))
					{
						_tieredStats.Add(statName);
					}
				}
			}

			if (validatorGroups != null)
			{
				foreach (var group in validatorGroups)
				{
					if (group.Stats != null)
					{
						foreach (var stat in group.Stats)
						{
							_validatorStats.Add(stat.PropertyName);
						}
					}
				}
			}
		}

		private void PrepareCombinationData(List<(List<Item> Combination, double Score)> scoredCombinations)
		{
			if (scoredCombinations == null || scoredCombinations.Count == 0)
			{
				_combinationsToDisplay = [];
				return;
			}

			var viewModels = new List<CombinationViewModel>(scoredCombinations.Count);

			for (int i = 0; i < scoredCombinations.Count; i++)
			{
				var (combination, score) = scoredCombinations[i];
				var aggregatedStats = ComputeAggregatedStats(combination);

				viewModels.Add(new CombinationViewModel
				{
					Rank = i + 1,
					Score = score,
					Items = combination,
					AggregatedStats = aggregatedStats
				});
			}

			_combinationsToDisplay = [.. viewModels];
		}

		private Dictionary<string, double> ComputeAggregatedStats(List<Item> combination)
		{
			var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
			var relevantStats = _tieredStats.Union(_validatorStats).ToHashSet(StringComparer.OrdinalIgnoreCase);

			foreach (var statName in relevantStats)
			{
				double sum = 0;
				foreach (var item in combination)
				{
					var itemStatsDict = ItemStatsHelper.ToDictionary(item.ItemStats);
					if (itemStatsDict.TryGetValue(statName, out var value) && value is not string)
					{
						sum += Convert.ToDouble(value);
					}
				}
				result[statName] = sum;
			}

			return result;
		}

		private void CombinationDisplay_Load(object sender, EventArgs e)
		{
			ConfigureForm();
			SetupDataGridView();
			LoadCombinations();
		}

		private void ConfigureForm()
		{
			this.AutoSize = false;
			this.AutoSizeMode = AutoSizeMode.GrowOnly;
			this.StartPosition = FormStartPosition.CenterScreen;

			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
		}

		private void SetupDataGridView()
		{
			DataGridViewCombinations.SuspendLayout();

			DataGridViewCombinations.VirtualMode = true;
			DataGridViewCombinations.ReadOnly = true;
			DataGridViewCombinations.AllowUserToAddRows = false;
			DataGridViewCombinations.AllowUserToDeleteRows = false;
			DataGridViewCombinations.AllowUserToResizeRows = false;
			DataGridViewCombinations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewCombinations.MultiSelect = false;
			DataGridViewCombinations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			DataGridViewCombinations.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			DataGridViewCombinations.RowHeadersVisible = false;
			DataGridViewCombinations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

			DataGridViewCombinations.CellValueNeeded += DataGridViewCombinations_CellValueNeeded;

			BuildColumns();

			DataGridViewCombinations.ResumeLayout();
		}

		private void BuildColumns()
		{
			DataGridViewCombinations.Columns.Clear();

			DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Rank",
				HeaderText = "#",
				Width = 60,
				ValueType = typeof(int),
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
			});

			DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Score",
				HeaderText = "Score",
				Width = 80,
				ValueType = typeof(double),
				DefaultCellStyle = new DataGridViewCellStyle
				{
					Alignment = DataGridViewContentAlignment.MiddleRight,
					Format = "F2"
				}
			});

			int maxItemsInCombination = _combinationsToDisplay.Count > 0
				? _combinationsToDisplay.Max(c => c.Items.Count)
				: 0;

			for (int i = 0; i < maxItemsInCombination; i++)
			{
				int itemIndex = i;
				DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Item{itemIndex}Name",
					HeaderText = $"Item {itemIndex + 1} Name",
					Width = 180,
					ValueType = typeof(string)
				});

				DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Item{itemIndex}Class",
					HeaderText = $"Item {itemIndex + 1} Class",
					Width = 120,
					ValueType = typeof(string)
				});

				DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Item{itemIndex}IsMine",
					HeaderText = $"Item {itemIndex + 1} Mine?",
					Width = 60,
					ValueType = typeof(string),
					DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
				});

				DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Item{itemIndex}Potential",
					HeaderText = $"Item {itemIndex + 1} Potential?",
					Width = 80,
					ValueType = typeof(string),
					DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
				});
			}

			var relevantStats = _tieredStats.Union(_validatorStats).OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToList();
			foreach (var statName in relevantStats)
			{
				var descriptor = _statDescriptors.FirstOrDefault(d => d.PropertyName.Equals(statName, StringComparison.OrdinalIgnoreCase));
				string headerText = descriptor?.Header ?? statName;

				DataGridViewCombinations.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Stat_{statName}",
					HeaderText = $"Σ {headerText}",
					Width = 100,
					ValueType = typeof(double),
					DefaultCellStyle = new DataGridViewCellStyle
					{
						Alignment = DataGridViewContentAlignment.MiddleRight,
						Format = "F2"
					}
				});
			}
		}

		private void LoadCombinations()
		{
			if (_combinationsToDisplay.Count == 0)
			{
				MessageBox.Show("No combinations to display.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			DataGridViewCombinations.RowCount = _combinationsToDisplay.Count;
			StatusBarLabel.Text = $"Loaded {_combinationsToDisplay.Count:N0} combinations";
		}

		private void DataGridViewCombinations_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _combinationsToDisplay.Count) return;

			var viewModel = _combinationsToDisplay[e.RowIndex];
			string columnName = DataGridViewCombinations.Columns[e.ColumnIndex].Name;

			if (columnName == "Rank")
			{
				e.Value = viewModel.Rank;
			}
			else if (columnName == "Score")
			{
				e.Value = viewModel.Score;
			}
			else if (columnName.StartsWith("Item") && columnName.EndsWith("Name"))
			{
				int itemIndex = ExtractItemIndex(columnName);
				e.Value = itemIndex < viewModel.Items.Count ? viewModel.Items[itemIndex].Name : string.Empty;
			}
			else if (columnName.StartsWith("Item") && columnName.EndsWith("Class"))
			{
				int itemIndex = ExtractItemIndex(columnName);
				e.Value = itemIndex < viewModel.Items.Count ? viewModel.Items[itemIndex].Class : string.Empty;
			}
			else if (columnName.StartsWith("Item") && columnName.EndsWith("IsMine"))
			{
				int itemIndex = ExtractItemIndex(columnName);
				e.Value = itemIndex < viewModel.Items.Count ? (viewModel.Items[itemIndex].IsMine ? "YES" : "NO") : string.Empty;
			}
			else if (columnName.StartsWith("Item") && columnName.EndsWith("Potential"))
			{
				int itemIndex = ExtractItemIndex(columnName);
				e.Value = itemIndex < viewModel.Items.Count ? viewModel.Items[itemIndex].ItemStats.Potential : string.Empty;
			}
			else if (columnName.StartsWith("Stat_"))
			{
				string statName = columnName["Stat_".Length..];
				e.Value = viewModel.AggregatedStats.TryGetValue(statName, out double value) ? value : 0.0;
			}
		}

		private static int ExtractItemIndex(string columnName)
		{
			int startIndex = "Item".Length;
			int endIndex = columnName.IndexOfAny(['N', 'C', 'I', 'P'], startIndex);
			if (endIndex > startIndex && int.TryParse(columnName[startIndex..endIndex], out int index))
			{
				return index;
			}
			return -1;
		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private void ButtonExport_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Export functionality not yet implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
