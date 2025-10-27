using System.Collections.Immutable;

using Domain.HelperForms;
using Domain.Helpers;
using Domain.Main;
using Domain.Validation;

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
		private readonly List<Tier> _tiers;
		private readonly List<Group> _validatorGroups;
		private readonly Dictionary<string, (double StatWeight, double TierWeight, int TierIndex)> _tieredStatWeights = [];

		public CombinationDisplay(
			List<(List<Item> Combination, double Score)> scoredCombinations,
			List<Tier> tiers = null,
			List<Group> validatorGroups = null)
		{
			InitializeComponent();
			this.Load += CombinationDisplay_Load;

			_tiers = tiers ?? [];
			_validatorGroups = validatorGroups ?? [];
			_statDescriptors = ItemStatsHelper.GetStatDescriptors();

			ExtractRelevantStats(tiers, validatorGroups);
			BuildTieredStatWeights(tiers);  // NEW METHOD
			PrepareCombinationData(scoredCombinations);
		}

		private void BuildTieredStatWeights(List<Tier> tiers)
		{
			if (tiers == null) return;

			for (int tierIndex = 0; tierIndex < tiers.Count; tierIndex++)
			{
				var tier = tiers[tierIndex];
				foreach (var (statName, statWeight) in tier.StatWeights)
				{
					if (statWeight > 0)
					{
						// Effective weight = (statWeight / totalStatWeight) * (tierWeight / 100)
						double normalizedStatWeight = tier.TotalStatWeight > 0
							? statWeight / tier.TotalStatWeight
							: 0;
						double effectiveWeight = normalizedStatWeight * (tier.TierWeight / 100.0);

						_tieredStatWeights[statName] = (statWeight, effectiveWeight, tierIndex);
					}
				}
			}
		}

		private void ExtractRelevantStats(List<Tier> tiers, List<Group> validatorGroups)
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
			SetupMasterGrid();
			SetupDetailGrid();
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

		private void LoadCombinations()
		{
			if (_combinationsToDisplay.Count == 0)
			{
				CustomMessageBox.Show("No combinations to display.", "Combinations not loaded",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			DataGridViewMaster.RowCount = _combinationsToDisplay.Count;
			StatusBarLabel.Text = $"Loaded {_combinationsToDisplay.Count:N0} combinations - Select rows to compare";
		}

		private void ButtonClose_Click(object sender, EventArgs e)
		{
			this.Close();
			this.Dispose();
		}

		private void ButtonExport_Click(object sender, EventArgs e)
		{
			CustomMessageBox.Show("Export functionality not yet implemented.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}




		private void SetupMasterGrid()
		{
			DataGridViewMaster.SuspendLayout();

			DataGridViewMaster.VirtualMode = true;
			DataGridViewMaster.ReadOnly = true;
			DataGridViewMaster.AllowUserToAddRows = false;
			DataGridViewMaster.AllowUserToDeleteRows = false;
			DataGridViewMaster.AllowUserToResizeRows = false;
			DataGridViewMaster.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewMaster.MultiSelect = true;
			DataGridViewMaster.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			DataGridViewMaster.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			DataGridViewMaster.RowHeadersVisible = false;
			DataGridViewMaster.Dock = DockStyle.Fill;

			DataGridViewMaster.CellValueNeeded += MasterGrid_CellValueNeeded;
			DataGridViewMaster.SelectionChanged += MasterGrid_SelectionChanged;  // ✅ NEW EVENT

			BuildMasterColumns();

			DataGridViewMaster.ResumeLayout();
		}

		private void BuildMasterColumns()
		{
			DataGridViewMaster.Columns.Clear();

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Rank",
				HeaderText = "#",
				Width = 60,
				ValueType = typeof(int),
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
			});

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
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

			DataGridViewMaster.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Items",
				HeaderText = "Item Names",
				Width = 600,
				ValueType = typeof(string),
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
			});
		}

		private void MasterGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _combinationsToDisplay.Count) return;

			var viewModel = _combinationsToDisplay[e.RowIndex];
			string columnName = DataGridViewMaster.Columns[e.ColumnIndex].Name;

			e.Value = columnName switch
			{
				"Rank" => viewModel.Rank,
				"Score" => viewModel.Score,
				"Items" => string.Join(", ", viewModel.Items.Select(i => i.Name)),
				_ => null
			};
		}

		private void MasterGrid_SelectionChanged(object sender, EventArgs e)
		{
			DataGridViewDetail.SuspendLayout();
			RebuildDetailGrid();
			DataGridViewDetail.ResumeLayout();
		}

		private void RebuildDetailGrid()
		{
			DataGridViewDetail.SuspendLayout();

			var selectedIndices = DataGridViewMaster.SelectedRows
				.Cast<DataGridViewRow>()
				.Select(r => r.Index)
				.Where(i => i >= 0 && i < _combinationsToDisplay.Count)
				.OrderBy(i => i)
				.ToList();

			if (selectedIndices.Count == 0)
			{
				DataGridViewDetail.Rows.Clear();
				DataGridViewDetail.Columns.Clear();
				StatusBarLabel.Text = "Select combinations to compare";
				DataGridViewDetail.ResumeLayout();
				return;
			}

			// Collect all stats from selected combinations (UNION)
			var allStats = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (var index in selectedIndices)
			{
				allStats.UnionWith(_combinationsToDisplay[index].AggregatedStats.Keys);
			}

			// Order stats by importance: Tiered (by effective weight DESC), Validator, Others
			var orderedStats = OrderStatsByImportance(allStats);

			// Build columns
			BuildDetailColumns(orderedStats);

			// Populate rows
			PopulateDetailRows(selectedIndices, orderedStats);

			StatusBarLabel.Text = $"Comparing {selectedIndices.Count} combination(s)";
			DataGridViewDetail.ResumeLayout();
		}

		private List<string> OrderStatsByImportance(HashSet<string> allStats)
		{
			var tieredStats = new List<(string StatName, double EffectiveWeight, int TierIndex)>();
			var validatorStats = new List<string>();
			var otherStats = new List<string>();

			foreach (var statName in allStats)
			{
				if (_tieredStatWeights.TryGetValue(statName, out var tierInfo))
				{
					tieredStats.Add((statName, tierInfo.StatWeight * tierInfo.TierWeight, tierInfo.TierIndex));
				}
				else if (_validatorStats.Contains(statName))
				{
					validatorStats.Add(statName);
				}
				else
				{
					otherStats.Add(statName);
				}
			}

			// Order tiered stats: by tier index ASC, then by effective weight DESC
			var orderedTiered = tieredStats
				.OrderBy(x => x.TierIndex)
				.ThenByDescending(x => x.EffectiveWeight)
				.Select(x => x.StatName)
				.ToList();

			// Validator and others: alphabetical
			validatorStats.Sort(StringComparer.OrdinalIgnoreCase);
			otherStats.Sort(StringComparer.OrdinalIgnoreCase);

			// Combine: Tiered → Validator → Others
			var result = new List<string>(orderedTiered.Count + validatorStats.Count + otherStats.Count);
			result.AddRange(orderedTiered);
			result.AddRange(validatorStats);
			result.AddRange(otherStats);

			return result;
		}

		private void BuildDetailColumns(List<string> orderedStats)
		{
			DataGridViewDetail.Columns.Clear();

			// Rank column
			DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Rank",
				HeaderText = "#",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(int),
				DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
				Frozen = true  // Keep visible while scrolling
			});

			// Score column (sortable, default sort DESC)
			DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
			{
				Name = "Score",
				HeaderText = "Score",
				AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
				ValueType = typeof(double),
				DefaultCellStyle = new DataGridViewCellStyle
				{
					Alignment = DataGridViewContentAlignment.MiddleRight,
					Format = "F2"
				},
				Frozen = true
			});

			// Stat columns (ordered by importance)
			foreach (var statName in orderedStats)
			{
				var descriptor = _statDescriptors.FirstOrDefault(d => d.PropertyName.Equals(statName, StringComparison.OrdinalIgnoreCase));
				string headerText = descriptor?.Header ?? statName;

				// Add visual indicator for tiered stats
				if (_tieredStatWeights.ContainsKey(statName))
				{
					headerText = $"★ {headerText}";  // Star for tiered stats
				}

				DataGridViewDetail.Columns.Add(new DataGridViewTextBoxColumn
				{
					Name = $"Stat_{statName}",
					HeaderText = headerText,
					AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
					ValueType = typeof(double),
					DefaultCellStyle = new DataGridViewCellStyle
					{
						Alignment = DataGridViewContentAlignment.MiddleRight,
						Format = "F2"
					},
					Tag = statName  // Store original stat name for click event
				});
			}

			// Enable sorting
			DataGridViewDetail.AllowUserToOrderColumns = false;
			DataGridViewDetail.AutoGenerateColumns = false;
			DataGridViewDetail.ReadOnly = true;
			DataGridViewDetail.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			DataGridViewDetail.MultiSelect = false;
			DataGridViewDetail.Dock = DockStyle.Fill;
		}

		private void PopulateDetailRows(List<int> selectedIndices, List<string> orderedStats)
		{
			DataGridViewDetail.Rows.Clear();

			foreach (var index in selectedIndices)
			{
				var vm = _combinationsToDisplay[index];
				var rowValues = new object[2 + orderedStats.Count];

				rowValues[0] = vm.Rank;
				rowValues[1] = vm.Score;

				for (int i = 0; i < orderedStats.Count; i++)
				{
					var statName = orderedStats[i];
					rowValues[2 + i] = vm.AggregatedStats.TryGetValue(statName, out double value)
						? value
						: 0.0;
				}

				DataGridViewDetail.Rows.Add(rowValues);
			}

			// Default sort by Score DESC
			if (DataGridViewDetail.Rows.Count > 0)
			{
				DataGridViewDetail.Sort(DataGridViewDetail.Columns["Score"], System.ComponentModel.ListSortDirection.Descending);
			}
		}

		private void SetupDetailGrid()
		{
			DataGridViewDetail.CellClick += DetailGrid_CellClick;
		}

		private void DetailGrid_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			// Ignore header and non-stat columns
			if (e.RowIndex < 0 || e.ColumnIndex < 2) return;

			var column = DataGridViewDetail.Columns[e.ColumnIndex];
			if (!column.Name.StartsWith("Stat_", StringComparison.OrdinalIgnoreCase)) return;

			var statName = column.Tag as string;
			var rank = (int)DataGridViewDetail.Rows[e.RowIndex].Cells["Rank"].Value;

			ShowStatBreakdown(rank, statName);
		}

		private void ShowStatBreakdown(int rank, string statName)
		{
			var vm = _combinationsToDisplay.FirstOrDefault(c => c.Rank == rank);
			if (vm == null) return;

			var descriptor = _statDescriptors.FirstOrDefault(d => d.PropertyName.Equals(statName, StringComparison.OrdinalIgnoreCase));
			string displayName = descriptor?.Header ?? statName;

			// Build breakdown message
			var sb = new System.Text.StringBuilder();
			sb.AppendLine($"Combination #{rank} - {displayName} Breakdown");
			sb.AppendLine($"Total: {vm.AggregatedStats.GetValueOrDefault(statName, 0):F2}");
			sb.AppendLine();
			sb.AppendLine("Item Contributions:");

			double sum = 0;
			foreach (var item in vm.Items)
			{
				var itemStatsDict = ItemStatsHelper.ToDictionary(item.ItemStats);
				if (itemStatsDict.TryGetValue(statName, out var value) && value is not string)
				{
					double val = Convert.ToDouble(value);
					if (val != 0)
					{
						sb.AppendLine($"  {item.Name}: {val:F2}");
						sum += val;
					}
				}
			}

			sb.AppendLine();
			sb.AppendLine($"Computed Total: {sum:F2}");

			CustomMessageBox.Show(sb.ToString(), "Stat Breakdown", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
