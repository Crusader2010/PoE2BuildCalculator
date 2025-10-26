using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Text;

using Domain.Enums;
using Domain.Helpers;
using Domain.Main;
using Domain.Serialization;
using Domain.Static;
using Domain.UserControls;
using Domain.Validation;

using PoE2BuildCalculator.Helpers;

namespace PoE2BuildCalculator
{
	public partial class CustomValidator : BaseForm, IConfigurable
	{
		private Func<List<Item>, bool> _masterValidator = x => true;
		public bool _customValidatorCreated { get; private set; } = false;

		private readonly BindingList<Group> _groups = [];
		private readonly BindingList<GroupOperationsUserControl> _operationControls = [];

		private readonly MainForm _ownerForm;
		private readonly ConfigurationManager _configManager;
		private int _nextGroupId = 1;

		// Cached layout calculations
		private readonly (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) _cachedSizes;
		private const int GROUP_ITEMSTATSROWS_VISIBLE = 5;

		private ImmutableDictionary<int, string> _immutableGroupDescriptions
		{
			get
			{
				field ??= _groups.Count == 0
						? ImmutableDictionary<int, string>.Empty
						: _groups.ToImmutableDictionary(g => g.GroupId, g => g.GroupName);

				return field;
			}

			set;
		}

		public CustomValidator(MainForm ownerForm, ConfigurationManager configManager)
		{
			ArgumentNullException.ThrowIfNull(ownerForm);
			ArgumentNullException.ThrowIfNull(configManager);
			InitializeComponent();

			_ownerForm = ownerForm;
			_configManager = configManager;
			_groups.ListChanged += (s, e) => _immutableGroupDescriptions = null;
			_cachedSizes = GetUserControlSizes();

			// Enable double buffering for smoother rendering
			SetStyle(ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);
			UpdateStyles();
		}

		public List<Group> GetGroups()
		{
			return [.. _groups];
		}

		#region JSON import - export

		public bool HasData => _groups.Count > 0 || _operationControls.Count > 0;

		public object ExportConfig() => ExportData();

		public void ImportConfig(object data)
		{
			if (data is not (List<GroupDto> groups, List<ValidationModel> operations)) return;
			ImportData(groups, operations);
		}

		public (List<GroupDto>, List<ValidationModel>) ExportData()
		{
			var groups = _groups.Select(g => new GroupDto
			{
				GroupId = g.GroupId,
				GroupName = g.GroupName,
				Stats = g.Stats?.Select(s => new GroupStatDto
				{
					PropertyName = s.PropertyName,
					Operator = s.Operator
				}).ToList() ?? []
			}).ToList();

			var operations = _operationControls.Select(c => c.GetValidationModel()).ToList();
			return (groups, operations);
		}

		public void ImportData(List<GroupDto> groupDtos, List<ValidationModel> operations)
		{
			_groups.Clear();
			_operationControls.Clear();
			_immutableGroupDescriptions = null;

			FlowPanelGroups.SuspendLayout();
			FlowPanelOperations.SuspendLayout();

			foreach (var control in FlowPanelGroups.Controls.OfType<ItemStatGroupValidatorUserControl>())
				control.Dispose();
			foreach (var control in FlowPanelOperations.Controls.OfType<GroupOperationsUserControl>())
				control.Dispose();

			FlowPanelGroups.Controls.Clear();
			FlowPanelOperations.Controls.Clear();

			// Rebuild groups
			foreach (var dto in groupDtos)
			{
				var control = new ItemStatGroupValidatorUserControl(dto.GroupId, dto.GroupName)
				{
					Width = _cachedSizes.widthStat + 25,
					Height = _cachedSizes.heightGroupTop + (_cachedSizes.heightStat * GROUP_ITEMSTATSROWS_VISIBLE) + 5
				};

				control.LoadStatsFromConfig(dto.Stats);
				control.GroupDeleted += (s, e) => DeleteGroup(control);

				_groups.Add(control._group);
				FlowPanelGroups.Controls.Add(control);
			}

			// Rebuild operations
			foreach (var op in operations)
			{
				var control = new GroupOperationsUserControl(_immutableGroupDescriptions, this)
				{
					BackColor = _operationControls.Count % 2 == 0 ? Color.LightGray : Color.LightSlateGray
				};

				control.LoadFromValidationModel(op);
				control.GroupOperationDeleted += OperationControl_GroupOperationDeleted;

				_operationControls.Add(control);
				FlowPanelOperations.Controls.Add(control);
			}

			FlowPanelGroups.ResumeLayout(true);
			FlowPanelOperations.ResumeLayout(true);

			_nextGroupId = _groups.Any() ? _groups.Max(g => g.GroupId) + 1 : 1;

			_masterValidator = x => true; // reset
			var validatorFunction = CreateValidatorFunction(true);
			if (!validatorFunction.Created)
			{
				// Reset to safe state
				_groups.Clear();
				_operationControls.Clear();
				FlowPanelGroups.Controls.Clear();
				FlowPanelOperations.Controls.Clear();
				CustomMessageBox.Show($"Configuration loaded but validator recreation failed:\n\n{validatorFunction.Message}\n\nGroups and operations have been cleared. Please reconfigure.", "Unable to recreate validator function", MessageBoxButtons.OK, MessageBoxIcon.Error, this);
			}
		}

		#endregion

		private void BtnHelp_Click(object sender, EventArgs e)
		{
			CustomMessageBox.Show(Constants.VALIDATOR_HELP_TEXT, "Validation Help", MessageBoxButtons.OK, MessageBoxIcon.Information, this);
		}

		private void BtnAddOperation_Click(object sender, EventArgs e)
		{
			FlowPanelOperations.SuspendLayout();
			try
			{
				var operationControl = new GroupOperationsUserControl(_immutableGroupDescriptions, this)
				{
					BackColor = _operationControls.Count % 2 == 0 ? Color.LightGray : Color.LightSlateGray
				};

				FlowPanelOperations.Controls.Add(operationControl);

				if (_operationControls.Count > 0) _operationControls[^1].SetComboBoxGroupLevelOperatorEnabled(true);
				operationControl.GroupOperationDeleted += OperationControl_GroupOperationDeleted;

				_operationControls.Add(operationControl);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error creating group operation control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				FlowPanelOperations.ResumeLayout();
			}
		}

		private void OperationControl_GroupOperationDeleted(object sender, EventArgs e)
		{
			this.SuspendLayout();
			FlowPanelOperations.SuspendLayout();
			try
			{
				if (sender is not null and GroupOperationsUserControl operationControl)
				{
					_operationControls.Remove(operationControl);
					RefreshGroupOperationsAfterDelete();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error when deleting a group operation control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				FlowPanelOperations.ResumeLayout();
				this.ResumeLayout();
			}
		}

		private void BtnAddGroup_Click(object sender, EventArgs e)
		{
			FlowPanelGroups.SuspendLayout();
			try
			{
				var control = new ItemStatGroupValidatorUserControl(_nextGroupId++, $"Group {_groups.Count + 1}")
				{
					Width = _cachedSizes.widthStat + 25, // account for scrollbar
					Height = _cachedSizes.heightGroupTop + (_cachedSizes.heightStat * GROUP_ITEMSTATSROWS_VISIBLE) + 5,
					AllowDrop = false
				};

				control.GroupDeleted += (s, e) => DeleteGroup(control);
				FlowPanelGroups.Controls.Add(control);

				_groups.Add(control._group);
				RefreshGroupOperationsAfterGroupsChanged();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error creating group control: {ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				FlowPanelGroups.ResumeLayout(true);
			}
		}

		private void DeleteGroup(ItemStatGroupValidatorUserControl control)
		{
			_groups.Remove(control._group);

			FlowPanelGroups.SuspendLayout();
			try
			{
				FlowPanelGroups.Controls.Remove(control);
				control.Dispose();

				RefreshGroupOperationsAfterGroupsChanged();
			}
			finally
			{
				FlowPanelGroups.ResumeLayout(true);
			}
		}

		private void BtnCreateValidator_Click(object sender, EventArgs e)
		{
			CreateValidatorFunction(false);
		}

		private void ButtonTranslateValidationFunction_Click(object sender, EventArgs e)
		{
			var item = new Item() { Class = "TestItem", Id = -1, Name = "Test1", ItemStats = new() };

			// Get validation message
			var (isValid, message) = TestValidationFunctionTranslation([item]);
			if (!isValid)
			{
				MessageBox.Show("Error during validation function translation:\r\n\r\n" + message, "Validation function sample", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			message = message.Replace("\r\n\t\t => true", string.Empty)
							 .Replace("\r\n\t\t => false", string.Empty)
							 .Replace("\r\n\r\n => true", string.Empty)
							 .Replace("\r\n\r\n => false", string.Empty);
			MessageBox.Show("Validation function translation:\r\n\r\n" + message, "Validation function sample", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private static (int widthStat, int heightStat, int heightGroupTop, int widthGroup, int widthGroupOperation) GetUserControlSizes()
		{
			using var tempGroupOperation = new GroupOperationsUserControl(ImmutableDictionary<int, string>.Empty, new Form());
			using var tempGroup = new ItemStatGroupValidatorUserControl(int.MaxValue, string.Empty);
			using var tempRow = new ItemStatRow(int.MaxValue, string.Empty);
			{
				tempGroup.Width = tempRow.Width + 25; // account for scrollbar
			}

			return (tempRow.Width + 2, tempRow.Height, tempGroup.GetTopRowsHeight(), tempGroup.Width + 2, tempGroupOperation.Width);
		}

		private void CustomValidator_Load(object sender, EventArgs e)
		{
			this.SuspendLayout();

			FlowPanelOperations.Padding = new Padding(0, 0, 1, 0);
			FlowPanelOperations.Width = _cachedSizes.widthGroupOperation + 20;

			Panel delimiter = new()
			{
				Width = 2,
				Dock = DockStyle.Left,
				BackColor = Color.YellowGreen
			};
			panel1.Controls.Add(delimiter);

			FlowPanelOperations.BringToFront();
			delimiter.BringToFront();
			FlowPanelGroups.BringToFront();

			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, FlowPanelGroups, [true]);
			typeof(Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, FlowPanelOperations, [true]);

			this.AutoSize = false;
			this.Width = FlowPanelOperations.Width + FlowPanelOperations.Padding.Left + FlowPanelOperations.Padding.Right
							+ mainPanel.Padding.Left + mainPanel.Padding.Right + delimiter.Width
							+ panel1.Padding.Left + panel1.Padding.Right + _cachedSizes.widthGroup * 2 + 45;
			this.CenterToScreen();

			this.ResumeLayout(true);
		}

		private void ButtonCloseAndDispose_Click(object sender, EventArgs e)
		{
			ButtonCloseAndDispose.CausesValidation = false;
			components?.Dispose();
			this.Dispose();
		}

		private void ButtonHide_Click(object sender, EventArgs e)
		{
			ButtonHide.CausesValidation = true;
			this.Hide();
			this.Owner?.BringToFront();
		}

		private void CustomValidator_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
				this.Owner?.BringToFront();
			}
		}


		private void RefreshGroupOperationsAfterDelete()
		{
			for (int i = 0; i < _operationControls.Count; i++)
			{
				_operationControls[i].RefreshGroupOperationAfterDelete(_operationControls.Count, i);
			}
		}

		private void RefreshGroupOperationsAfterGroupsChanged()
		{
			for (int i = 0; i < _operationControls.Count; i++)
			{
				_operationControls[i].UpdateGroups(_immutableGroupDescriptions);
			}
		}

		private (bool isValid, string message) CheckInvalidSelectedGroupsOrOperations()
		{
			var grpMsg = string.Empty;
			var operMsg = string.Empty;
			bool isValid = true;

			var inactiveGroups = _groups.Where(x => x == null || !x.IsActive).ToList();
			if (inactiveGroups.Count > 0)
			{
				grpMsg = $"The following groups are inactive: {string.Join(",", inactiveGroups.Select(x => x.GroupName))}. Make sure they have at least one item stat selected.";
				isValid = false;
			}

			var inactiveGroupIds = inactiveGroups.Select(x => x.GroupId).ToHashSet();
			var validationModels = _operationControls.Select(x => x.GetValidationModel());

			int inactiveOperations = validationModels.Count(x => x == null || !x.IsActive || x.GroupId == -1 || inactiveGroupIds.Contains(x.GroupId));
			if (inactiveOperations > 0)
			{
				operMsg = $"There are currently {inactiveOperations} group operations that are either inactive or have an inactive or no group selected. Make sure the groups have at least one item stat selected, " +
							$"and that each operation has a group selected and at least the MIN or the MAX checkboxes set.";
				isValid = false;
			}

			var invalidOperations = validationModels.Where(x => x.IsActive && x.MinValue.HasValue && x.MaxValue.HasValue && x.MinValue > x.MaxValue).Select(x => x.GroupId).ToHashSet();
			if (invalidOperations.Count > 0)
			{
				var groupNames = _immutableGroupDescriptions.Where(x => invalidOperations.Contains(x.Key)).Select(x => x.Value).ToList();
				operMsg = $"At least some of the operations linked to group names {string.Join(",", groupNames)} are invalid. Make sure the MAX value is greater or equal to the MIN value.";
				isValid = false;
			}

			return (isValid, string.Join("\r\n", grpMsg, operMsg, "\r\nBefore a validation function can be created, you must remove or edit the inactive or invalid groups and operations."));
		}

		private (bool Created, string Message) CreateValidatorFunction(bool suppressMessageBoxes)
		{
			try
			{
				var (isValid, message) = CheckInvalidSelectedGroupsOrOperations();
				if (!isValid)
				{
					if (!suppressMessageBoxes) CustomMessageBox.Show(message, "Inactive/invalid groups or operations", MessageBoxButtons.OK, MessageBoxIcon.Warning, this);
					return (false, message);
				}

				var operations = BuildValidationModels();
				var validatorFunction = BuildValidatorFunction(operations);

				if (validatorFunction == null)
				{
					string msg = "No validation function can be computed based on the existing groups and/or operations.\n\nKeeping default logic -> all combinations are valid.";
					if (!suppressMessageBoxes) CustomMessageBox.Show(msg, "No active/valid groups/operations", MessageBoxButtons.OK, MessageBoxIcon.Warning, this);
					return (false, msg);
				}

				_masterValidator = validatorFunction;
				_ownerForm._itemValidatorFunction = _masterValidator;
				_customValidatorCreated = validatorFunction.Target != null;

				if (!suppressMessageBoxes) CustomMessageBox.Show($"Validator created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information, this);
				return (true, $"Validator created successfully!");
			}
			catch (Exception ex)
			{
				if (!suppressMessageBoxes) ErrorHelper.ShowError(ex, $"{nameof(CustomValidator)} - {nameof(CreateValidatorFunction)}");
				return (false, $"Error: {ex}");
			}
		}

		#region Validation and parsing methods

		/// <summary>
		/// Builds the validator function from validation models(operations) and groups.
		/// Shows a message box with the validation logic for the FIRST combination only.
		/// All subsequent calls use optimized validation (no message building).
		/// </summary>
		private Func<List<Item>, bool> BuildValidatorFunction(List<ValidationModel> operations)
		{
			var groups = _groups.ToDictionary(g => g.GroupId, g => g); // all groups presumed active due to CheckInactiveSelectedGroupsOrOperations() method
			if (groups.Count == 0 || operations.Count == 0) return null;

			return items =>
			{
				return EvaluateValidationModels(operations, items, groups, messageBuilder: null);
			};
		}

		private List<ValidationModel> BuildValidationModels()
		{
			var result = _operationControls.Select(x => x.GetValidationModel()).ToList(); // all are presumed active!
			return result;
		}

		/// <summary>
		/// Evaluates all validation operations and combines their results using GroupLevelOperators.
		/// Main entry point for the validation function.
		/// Optionally builds a detailed message showing the evaluation process if StringBuilder is provided.
		/// Pass NULL for messageBuilder during batch validation for maximum performance.
		/// </summary>
		private static bool EvaluateValidationModels(List<ValidationModel> activeOperations, List<Item> items, Dictionary<int, Group> activeGroups, StringBuilder messageBuilder = null)
		{
			if (items is not { Count: > 0 }) return true;
			if (activeOperations is not { Count: > 0 }) return true;

			// Evaluate first operation to initialize result
			var firstOperation = activeOperations[0];
			if (!activeGroups.TryGetValue(firstOperation.GroupId, out var firstGroup) || firstGroup == null)
			{
				// Invalid group - should not happen if validation passed, but handle gracefully
				return true;
			}

			bool result = EvaluateSingleOperation(firstOperation, items, firstGroup, messageBuilder);

			// Process remaining operations, combining results with GroupLevelOperators
			for (int i = 1; i < activeOperations.Count; i++)
			{
				var currentOperation = activeOperations[i];
				var previousOperation = activeOperations[i - 1];

				// Get the group for current operation
				if (!activeGroups.TryGetValue(currentOperation.GroupId, out var currentGroup) || currentGroup == null)
				{
					continue; // Skip operations with invalid groups
				}

				// Add operator to message before evaluating next operation
				if (messageBuilder != null && previousOperation.GroupLevelOperator.HasValue)
				{
					messageBuilder.Append(' ');
					messageBuilder.Append(previousOperation.GroupLevelOperator.Value.GetDescription());
					messageBuilder.Append("\r\n\r\n");
				}

				// Evaluate current operation
				bool currentResult = EvaluateSingleOperation(currentOperation, items, currentGroup, messageBuilder);

				// Combine with accumulated result using operator from PREVIOUS operation
				// (operator is stored on the operation that comes before the next one)
				result = CombineOperationResults(result, previousOperation.GroupLevelOperator, currentResult);
			}

			// Add final result to message
			if (messageBuilder != null)
			{
				messageBuilder.Append("\r\n\r\n");
				messageBuilder.Append(" => ");
				messageBuilder.Append(result ? "true" : "false");
			}

			return result;
		}

		private static bool EvaluateArithmeticOperation(MinMaxOperatorsEnum? arithOperator, double existingValue, double testValue)
		{
			return arithOperator switch
			{
				MinMaxOperatorsEnum.Greater => existingValue > testValue,
				MinMaxOperatorsEnum.GreaterEqual => existingValue >= testValue,
				MinMaxOperatorsEnum.Equal => existingValue == testValue,
				MinMaxOperatorsEnum.Less => existingValue < testValue,
				MinMaxOperatorsEnum.LessEqual => existingValue <= testValue,
				_ => false,
			};
		}

		/// <summary>
		/// Evaluates the arithmetic expression for a group across a single item's stats.
		/// Example: If group has [MaxLife, Armour, Spirit] with operators [*, +], 
		/// this computes: MaxLife * Armour + Spirit
		/// </summary>
		private static double EvaluateGroupExpression(Group group, ImmutableDictionary<string, object> itemStats)
		{
			if (group?.Stats is not { Count: > 0 }) return 0.0;

			// Start with first stat value (defaults to 0 if missing)
			double result = itemStats.TryGetValue(group.Stats[0].PropertyName, out var initialStat)
				? Convert.ToDouble(initialStat)
				: 0.0;

			// Apply each operator successively to build the expression result
			for (int i = 1; i < group.Stats.Count; i++)
			{
				double nextValue = itemStats.TryGetValue(group.Stats[i].PropertyName, out var nextStat)
					? Convert.ToDouble(nextStat)
					: 0.0;

				result = group.Stats[i - 1].Operator switch
				{
					ArithmeticOperationsEnum.Sum => result + nextValue,
					ArithmeticOperationsEnum.Diff => result - nextValue,
					ArithmeticOperationsEnum.Mult => result * nextValue,
					ArithmeticOperationsEnum.Div => nextValue != 0.0 ? result / nextValue : result,
					_ => result
				};
			}

			return result;
		}

		/// <summary>
		/// Checks if a computed value satisfies the Min/Max constraints of a validation operation.
		/// </summary>
		private static bool CheckValueAgainstConstraints(ValidationModel validation, double value)
		{
			bool minSatisfied = true;
			bool maxSatisfied = true;

			// Check minimum constraint if present
			if (validation.MinValue.HasValue && validation.MinOperator.HasValue)
			{
				minSatisfied = EvaluateArithmeticOperation(
					validation.MinOperator.Value,
					value,
					validation.MinValue.Value);
			}

			// Check maximum constraint if present
			if (validation.MaxValue.HasValue && validation.MaxOperator.HasValue)
			{
				maxSatisfied = EvaluateArithmeticOperation(
					validation.MaxOperator.Value,
					value,
					validation.MaxValue.Value);
			}

			// Combine min and max results if both are present
			if (validation.MinValue.HasValue && validation.MaxValue.HasValue && validation.MinMaxOperator.HasValue)
			{
				return validation.MinMaxOperator.Value switch
				{
					MinMaxCombinedOperatorsEnum.AND => minSatisfied && maxSatisfied,
					MinMaxCombinedOperatorsEnum.OR => minSatisfied || maxSatisfied,
					_ => minSatisfied && maxSatisfied // Default to AND
				};
			}

			// If only one constraint present, return that result
			return minSatisfied && maxSatisfied;
		}

		/// <summary>
		/// Evaluates a single validation operation against the item list.
		/// Dispatches to the appropriate validation type handler.
		/// Optionally builds a detailed message if StringBuilder is provided.
		/// </summary>
		private static bool EvaluateSingleOperation(ValidationModel operation, List<Item> items, Group group, StringBuilder messageBuilder)
		{
			if (group?.Stats is not { Count: > 0 }) return true;
			if (items is not { Count: > 0 }) return true;

			// Pre-compute all item stat dictionaries once for efficiency
			var itemStatsDicts = items
				.Select(item => ItemStatsHelper.ToDictionary(item.ItemStats))
				.ToList();

			// Perform evaluation
			bool result = operation.ValidationType switch
			{
				ValidationTypeEnum.EachItem => EvaluateEachItem(operation, itemStatsDicts, group),
				ValidationTypeEnum.SumALL => EvaluateSumAll(operation, itemStatsDicts, group),
				ValidationTypeEnum.AtLeast => EvaluateAtLeast(operation, itemStatsDicts, group, items.Count),
				ValidationTypeEnum.AtMost => EvaluateAtMost(operation, itemStatsDicts, group, items.Count),
				_ => true
			};

			// Build message if requested
			if (messageBuilder != null)
			{
				messageBuilder.Append("((");
				messageBuilder.Append(FormatGroupExpression(group));
				messageBuilder.Append(") ");
				messageBuilder.Append(FormatConstraints(operation));
				messageBuilder.Append(" for ");
				messageBuilder.Append(FormatValidationType(operation));
				messageBuilder.Append("\r\n\t\t => ");
				messageBuilder.Append(result ? "true" : "false");
				messageBuilder.Append(')');
			}

			return result;
		}

		/// <summary>
		/// Validates that EVERY item in the list satisfies the constraints.
		/// Returns false as soon as one item fails (early exit optimization).
		/// </summary>
		private static bool EvaluateEachItem(ValidationModel operation, List<ImmutableDictionary<string, object>> itemStatsDicts, Group group)
		{
			foreach (var itemStats in itemStatsDicts)
			{
				double value = EvaluateGroupExpression(group, itemStats);
				if (!CheckValueAgainstConstraints(operation, value))
				{
					return false; // Early exit on first failure
				}
			}
			return true;
		}

		/// <summary>
		/// Validates that the SUM of all items' computed values satisfies the constraints.
		/// </summary>
		private static bool EvaluateSumAll(ValidationModel operation, List<ImmutableDictionary<string, object>> itemStatsDicts, Group group)
		{
			double sum = 0.0;
			foreach (var itemStats in itemStatsDicts)
			{
				sum += EvaluateGroupExpression(group, itemStats);
			}
			return CheckValueAgainstConstraints(operation, sum);
		}

		/// <summary>
		/// Validates that AT LEAST N (or N%) items satisfy the constraints.
		/// Optimized for common cases: 0% always true, 100% delegates to EachItem, early exit when threshold met.
		/// </summary>
		private static bool EvaluateAtLeast(ValidationModel operation, List<ImmutableDictionary<string, object>> itemStatsDicts, Group group, int totalItemCount)
		{
			if (totalItemCount == 0) return true;

			// Calculate threshold (handling both absolute count and percentage)
			double threshold;
			if (operation.NumberOfItemsAsPercentage)
			{
				// Clamp percentage to valid range for safety
				double percentage = Math.Clamp(operation.NumberOfItems, 0, 100);
				threshold = totalItemCount * percentage / 100.0;
			}
			else
			{
				threshold = operation.NumberOfItems;
			}

			// Optimize: 0 threshold means "at least 0 items" - always true
			if (threshold <= 0.0) return true;

			// Optimize: threshold >= total means all items must satisfy
			if (threshold >= totalItemCount)
			{
				return EvaluateEachItem(operation, itemStatsDicts, group);
			}

			// Count items satisfying constraints
			int satisfyingCount = 0;
			foreach (var itemStats in itemStatsDicts)
			{
				double value = EvaluateGroupExpression(group, itemStats);
				if (CheckValueAgainstConstraints(operation, value))
				{
					satisfyingCount++;
					// Early exit optimization: once threshold met, no need to check remaining
					if (satisfyingCount >= threshold)
					{
						return true;
					}
				}
			}

			return satisfyingCount >= threshold;
		}

		/// <summary>
		/// Validates that AT MOST N (or N%) items satisfy the constraints.
		/// Optimized for common cases: 100% always true, 0% means no items can satisfy, early exit when threshold exceeded.
		/// </summary>
		private static bool EvaluateAtMost(ValidationModel operation, List<ImmutableDictionary<string, object>> itemStatsDicts, Group group, int totalItemCount)
		{
			if (totalItemCount == 0) return true;

			// Calculate threshold (handling both absolute count and percentage)
			double threshold;
			if (operation.NumberOfItemsAsPercentage)
			{
				// Clamp percentage to valid range for safety
				double percentage = Math.Clamp(operation.NumberOfItems, 0, 100);
				threshold = totalItemCount * percentage / 100.0;
			}
			else
			{
				threshold = operation.NumberOfItems;
			}

			// Optimize: threshold >= total means all items can satisfy - always true
			if (threshold >= totalItemCount) return true;

			// Optimize: 0 threshold means no items should satisfy
			if (threshold <= 0)
			{
				foreach (var itemStats in itemStatsDicts)
				{
					double value = EvaluateGroupExpression(group, itemStats);
					if (CheckValueAgainstConstraints(operation, value))
					{
						return false; // Found one that satisfies - fails "at most 0"
					}
				}
				return true;
			}

			// Count items satisfying constraints
			int satisfyingCount = 0;
			foreach (var itemStats in itemStatsDicts)
			{
				double value = EvaluateGroupExpression(group, itemStats);
				if (CheckValueAgainstConstraints(operation, value))
				{
					satisfyingCount++;
					// Early exit optimization: once exceeded, no need to check remaining
					if (satisfyingCount > threshold)
					{
						return false;
					}
				}
			}

			return satisfyingCount <= threshold;
		}

		/// <summary>
		/// Combines two boolean results using the specified logical operator (AND/OR/XOR).
		/// </summary>
		private static bool CombineOperationResults(bool leftResult, GroupLevelOperatorsEnum? op, bool rightResult)
		{
			if (!op.HasValue) return leftResult;

			return op.Value switch
			{
				GroupLevelOperatorsEnum.AND => leftResult && rightResult,
				GroupLevelOperatorsEnum.OR => leftResult || rightResult,
				GroupLevelOperatorsEnum.XOR => leftResult ^ rightResult,
				_ => leftResult
			};
		}

		#endregion

		#region Validation Function Text Output

		/// <summary>
		/// Formats a group's arithmetic expression as a human-readable string.
		/// Example: "MaxLife * Armour + Spirit"
		/// </summary>
		private static string FormatGroupExpression(Group group)
		{
			if (group?.Stats is not { Count: > 0 }) return "EMPTY_GROUP";

			var parts = new List<string> { group.Stats[0].PropertyName };

			for (int i = 1; i < group.Stats.Count; i++)
			{
				string operatorSymbol = group.Stats[i - 1].Operator?.GetDescription() ?? "+";
				parts.Add(operatorSymbol);
				parts.Add(group.Stats[i].PropertyName);
			}

			return string.Join(" ", parts);
		}

		/// <summary>
		/// Formats a validation type with its parameters as a human-readable string.
		/// Examples: "AT_LEAST 50% ITEMS", "SUM_ALL", "EACH_ITEM"
		/// </summary>
		private static string FormatValidationType(ValidationModel operation)
		{
			return operation.ValidationType switch
			{
				ValidationTypeEnum.EachItem => "EACH_ITEM",
				ValidationTypeEnum.SumALL => "SUM_ALL",
				ValidationTypeEnum.AtLeast when operation.NumberOfItemsAsPercentage
					=> $"AT_LEAST {operation.NumberOfItems}% ITEMS",
				ValidationTypeEnum.AtLeast
					=> $"AT_LEAST {operation.NumberOfItems} ITEMS",
				ValidationTypeEnum.AtMost when operation.NumberOfItemsAsPercentage
					=> $"AT_MOST {operation.NumberOfItems}% ITEMS",
				ValidationTypeEnum.AtMost
					=> $"AT_MOST {operation.NumberOfItems} ITEMS",
				_ => "UNKNOWN"
			};
		}

		/// <summary>
		/// Formats the Min/Max constraints as a human-readable string.
		/// Handles BETWEEN, NOT BETWEEN, and single constraints.
		/// Examples: "BETWEEN 10.00 AND 50.00", "NOT BETWEEN 10.00 AND 50.00", ">= 50.00", "< 100.00"
		/// </summary>
		private static string FormatConstraints(ValidationModel operation)
		{
			bool hasMin = operation.MinValue.HasValue && operation.MinOperator.HasValue;
			bool hasMax = operation.MaxValue.HasValue && operation.MaxOperator.HasValue;

			// Single constraint cases
			if (hasMin && !hasMax)
			{
				return $"{operation.MinOperator.Value.GetDescription()} {operation.MinValue.Value:F2}";
			}

			if (!hasMin && hasMax)
			{
				return $"{operation.MaxOperator.Value.GetDescription()} {operation.MaxValue.Value:F2}";
			}

			if (!hasMin && !hasMax)
			{
				return "NO_CONSTRAINTS";
			}

			// Both min and max present - check for BETWEEN/NOT BETWEEN patterns
			var minOp = operation.MinOperator.Value;
			var maxOp = operation.MaxOperator.Value;
			var minVal = operation.MinValue.Value;
			var maxVal = operation.MaxValue.Value;
			var combinedOp = operation.MinMaxOperator ?? MinMaxCombinedOperatorsEnum.AND;

			// Detect if min/max operators form a range (for BETWEEN)
			bool minIsLowerBound = minOp is MinMaxOperatorsEnum.Greater or MinMaxOperatorsEnum.GreaterEqual;
			bool maxIsUpperBound = maxOp is MinMaxOperatorsEnum.Less or MinMaxOperatorsEnum.LessEqual;

			// AND operator with range operators → BETWEEN
			if (combinedOp == MinMaxCombinedOperatorsEnum.AND && minIsLowerBound && maxIsUpperBound)
			{
				// Determine if inclusive or exclusive
				bool minInclusive = minOp == MinMaxOperatorsEnum.GreaterEqual;
				bool maxInclusive = maxOp == MinMaxOperatorsEnum.LessEqual;

				if (minInclusive && maxInclusive)
				{
					return $"BETWEEN {minVal:F2} AND {maxVal:F2}";
				}
				else if (!minInclusive && !maxInclusive)
				{
					return $"BETWEEN {minVal:F2} AND {maxVal:F2} (exclusive)";
				}
				else if (minInclusive && !maxInclusive)
				{
					return $"BETWEEN {minVal:F2} AND {maxVal:F2} (exclusive max)";
				}
				else // !minInclusive && maxInclusive
				{
					return $"BETWEEN {minVal:F2} AND {maxVal:F2} (exclusive min)";
				}
			}

			// OR operator with opposite range operators → NOT BETWEEN
			// Pattern: (X <= minVal OR X >= maxVal) means "not between minVal and maxVal"
			bool minIsUpperBound = minOp is MinMaxOperatorsEnum.Less or MinMaxOperatorsEnum.LessEqual;
			bool maxIsLowerBound = maxOp is MinMaxOperatorsEnum.Greater or MinMaxOperatorsEnum.GreaterEqual;

			if (combinedOp == MinMaxCombinedOperatorsEnum.OR && minIsUpperBound && maxIsLowerBound)
			{
				// For NOT BETWEEN, we need to swap the values (min becomes max, max becomes min)
				bool minInclusive = minOp == MinMaxOperatorsEnum.LessEqual;
				bool maxInclusive = maxOp == MinMaxOperatorsEnum.GreaterEqual;

				if (minInclusive && maxInclusive)
				{
					return $"NOT BETWEEN {minVal:F2} AND {maxVal:F2}";
				}
				else if (!minInclusive && !maxInclusive)
				{
					return $"NOT BETWEEN {minVal:F2} AND {maxVal:F2} (exclusive)";
				}
				else
				{
					// Mixed inclusive/exclusive for NOT BETWEEN is unusual, show explicitly
					return $"{minOp.GetDescription()} {minVal:F2} OR {maxOp.GetDescription()} {maxVal:F2}";
				}
			}

			// Not a standard BETWEEN/NOT BETWEEN pattern, show both constraints explicitly
			string minPart = $"{minOp.GetDescription()} {minVal:F2}";
			string maxPart = $"{maxOp.GetDescription()} {maxVal:F2}";
			string opStr = combinedOp.GetDescription();
			return $"{minPart} {opStr} {maxPart}";
		}

		/// <summary>
		/// Tests the validation logic on a single combination and returns both the result and a detailed message.
		/// Useful for debugging and understanding what the validator does.
		/// </summary>
		public (bool IsValid, string Message) TestValidationFunctionTranslation(List<Item> items)
		{
			if (items == null || items.Count == 0) return (false, "There are no items present.");

			var (isValid, message) = CheckInvalidSelectedGroupsOrOperations();
			if (!isValid) return (false, message);

			var groups = _groups.ToDictionary(g => g.GroupId, g => g); // all groups presumed active due to CheckInactiveSelectedGroupsOrOperations() method
			var operations = BuildValidationModels();

			if (groups.Count == 0 || operations.Count == 0) return (true, "NO_CONSTRAINTS");

			var messageBuilder = new StringBuilder();
			bool result = EvaluateValidationModels(operations, items, groups, messageBuilder);
			return (true, messageBuilder.ToString());
		}

		#endregion

		private void ButtonLoadConfig_Click(object sender, EventArgs e)
		{
			if (!_configManager.HasConfigData(ConfigSections.Validator))
			{
				CustomMessageBox.Show(
					"No validator configuration data available in memory.\n\nPlease load a configuration file from MainForm first.",
					"No Data",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
				return;
			}

			var validatorConfig = _configManager.GetConfigData(ConfigSections.Validator);
			if (validatorConfig == null) return;

			try
			{
				this.SuspendLayout();
				ImportConfig(validatorConfig);
				this.ResumeLayout(true);
			}
			catch (Exception ex)
			{
				ErrorHelper.ShowError(ex, "Load Validator Configuration");
			}
		}
	}
}
