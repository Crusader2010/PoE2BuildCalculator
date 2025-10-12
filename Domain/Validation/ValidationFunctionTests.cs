using Domain.Main;

using System.Reflection;

namespace Domain.Validation
{
	/// <summary>
	/// Comprehensive tests for validation function generation
	/// Tests: 1-4 groups, 1-4 stats per group, all operators
	/// </summary>
	public class ValidationFunctionTests
	{
		private static readonly PropertyInfo PropMaxLife = typeof(ItemStats).GetProperty(nameof(ItemStats.MaximumLifeAmount));
		private static readonly PropertyInfo PropMana = typeof(ItemStats).GetProperty(nameof(ItemStats.MaximumManaAmount));
		private static readonly PropertyInfo PropSpirit = typeof(ItemStats).GetProperty(nameof(ItemStats.SpiritAmount));
		private static readonly PropertyInfo PropFireRes = typeof(ItemStats).GetProperty(nameof(ItemStats.FireResistancePercent));
		private static readonly PropertyInfo PropColdRes = typeof(ItemStats).GetProperty(nameof(ItemStats.ColdResistancePercent));

		#region Test Data Creation

		private static List<Item> CreateTestItems(params (int life, int mana, int spirit, double fireRes, double coldRes)[] values)
		{
			return [.. values.Select((v, i) => new Item
			{
				Id = i + 1,
				ItemStats = new ItemStats
				{
					MaximumLifeAmount = v.life,
					MaximumManaAmount = v.mana,
					SpiritAmount = v.spirit,
					FireResistancePercent = v.fireRes,
					ColdResistancePercent = v.coldRes
				}
			})];
		}

		#endregion

		#region 1 Group Tests

		public static void Test_1Group_1Stat_Addition()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 100,
					IsMaxEnabled = true,
					MaxValue = 200,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }
					]
				}
			};

			var validator = BuildValidator(groups);
			var items = CreateTestItems((50, 0, 0, 0, 0), (60, 0, 0, 0, 0)); // Sum = 110

			Assert(validator(items), "1G/1S: Sum 110 should be in range [100,200]");

			items = CreateTestItems((50, 0, 0, 0, 0), (40, 0, 0, 0, 0)); // Sum = 90
			Assert(!validator(items), "1G/1S: Sum 90 should fail min constraint");

			items = CreateTestItems((100, 0, 0, 0, 0), (120, 0, 0, 0, 0)); // Sum = 220
			Assert(!validator(items), "1G/1S: Sum 220 should fail max constraint");

			Console.WriteLine("✓ 1Group/1Stat/Addition tests passed");
		}

		public static void Test_1Group_2Stats_AdditionSubtraction()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 50,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "-" }
					]
				}
			};

			var validator = BuildValidator(groups);
			// Life=100, Mana=30 => Sum per item = 100-30 = 70 => Total = 140
			var items = CreateTestItems((100, 30, 0, 0, 0), (100, 30, 0, 0, 0));

			Assert(validator(items), "1G/2S: (100-30)*2 = 140 should pass min 50");

			items = CreateTestItems((30, 40, 0, 0, 0), (20, 30, 0, 0, 0)); // (30-40)+(20-30) = -20
			Assert(!validator(items), "1G/2S: Negative sum should fail min 50");

			Console.WriteLine("✓ 1Group/2Stats/AddSub tests passed");
		}

		public static void Test_1Group_3Stats_MultiplicationDivision()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 100,
					IsMaxEnabled = true,
					MaxValue = 500,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "*" },
						new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "/" }
					]
				}
			};

			var validator = BuildValidator(groups);
			// Item1: (10 * 2) / 1 = 20
			// Item2: (15 * 3) / 1 = 45
			// Total: 65 (fails min)
			var items = CreateTestItems((10, 2, 1, 0, 0), (15, 3, 1, 0, 0));
			Assert(!validator(items), "1G/3S: Sum 65 should fail min 100");

			// Item1: (50 * 4) / 2 = 100
			// Item2: (60 * 5) / 3 = 100
			// Total: 200
			items = CreateTestItems((50, 4, 2, 0, 0), (60, 5, 3, 0, 0));
			Assert(validator(items), "1G/3S: Sum 200 should pass [100,500]");

			Console.WriteLine("✓ 1Group/3Stats/MulDiv tests passed");
		}

		public static void Test_1Group_4Stats_AllOperators()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 0,
					IsMaxEnabled = true,
					MaxValue = 100,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "-" },
						new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "*" },
						new() { PropInfo = PropFireRes, PropertyName = nameof(ItemStats.FireResistancePercent), Operator = "/" }
					]
				}
			};

			var validator = BuildValidator(groups);
			// Item1: ((10 - 5) * 2) / 2 = 5
			// Item2: ((20 - 10) * 3) / 3 = 10
			// Total: 15
			var items = CreateTestItems((10, 5, 2, 2, 0), (20, 10, 3, 3, 0));
			Assert(validator(items), "1G/4S: Sum 15 should pass [0,100]");

			Console.WriteLine("✓ 1Group/4Stats/AllOps tests passed");
		}

		#endregion

		#region 2 Groups Tests

		public static void Test_2Groups_AND_Operator()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "AND",
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 2,
					GroupName = "Group 2",
					IsMinEnabled = true,
					MinValue = 50,
					Stats = [new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Life=120, Mana=60 => Both pass
			var items = CreateTestItems((60, 30, 0, 0, 0), (60, 30, 0, 0, 0));
			Assert(validator(items), "2G/AND: Both conditions met should pass");

			// Life=120, Mana=40 => Second fails
			items = CreateTestItems((60, 20, 0, 0, 0), (60, 20, 0, 0, 0));
			Assert(!validator(items), "2G/AND: Second condition fails, should fail overall");

			// Life=80, Mana=60 => First fails
			items = CreateTestItems((40, 30, 0, 0, 0), (40, 30, 0, 0, 0));
			Assert(!validator(items), "2G/AND: First condition fails, should fail overall");

			Console.WriteLine("✓ 2Groups/AND tests passed");
		}

		public static void Test_2Groups_OR_Operator()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "OR",
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 2,
					GroupName = "Group 2",
					IsMinEnabled = true,
					MinValue = 50,
					Stats = [new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Life=120, Mana=30 => First passes
			var items = CreateTestItems((60, 15, 0, 0, 0), (60, 15, 0, 0, 0));
			Assert(validator(items), "2G/OR: First condition met should pass");

			// Life=80, Mana=60 => Second passes
			items = CreateTestItems((40, 30, 0, 0, 0), (40, 30, 0, 0, 0));
			Assert(validator(items), "2G/OR: Second condition met should pass");

			// Life=80, Mana=40 => Both fail
			items = CreateTestItems((40, 20, 0, 0, 0), (40, 20, 0, 0, 0));
			Assert(!validator(items), "2G/OR: Both conditions fail, should fail overall");

			Console.WriteLine("✓ 2Groups/OR tests passed");
		}

		public static void Test_2Groups_XOR_Operator()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					GroupName = "Group 1",
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "XOR",
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 2,
					GroupName = "Group 2",
					IsMinEnabled = true,
					MinValue = 50,
					Stats = [new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Life=120, Mana=30 => Only first passes
			var items = CreateTestItems((60, 15, 0, 0, 0), (60, 15, 0, 0, 0));
			Assert(validator(items), "2G/XOR: Only first passes should pass");

			// Life=80, Mana=60 => Only second passes
			items = CreateTestItems((40, 30, 0, 0, 0), (40, 30, 0, 0, 0));
			Assert(validator(items), "2G/XOR: Only second passes should pass");

			// Life=120, Mana=60 => Both pass
			items = CreateTestItems((60, 30, 0, 0, 0), (60, 30, 0, 0, 0));
			Assert(!validator(items), "2G/XOR: Both pass should fail");

			// Life=80, Mana=40 => Both fail
			items = CreateTestItems((40, 20, 0, 0, 0), (40, 20, 0, 0, 0));
			Assert(!validator(items), "2G/XOR: Both fail should fail");

			Console.WriteLine("✓ 2Groups/XOR tests passed");
		}

		#endregion

		#region 3 Groups Tests

		public static void Test_3Groups_Complex_AND_OR()
		{
			// G1 AND G2 OR G3 => (G1 AND G2) OR G3
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "AND",
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 2,
					IsMinEnabled = true,
					MinValue = 50,
					GroupOperator = "OR",
					Stats = [new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 3,
					IsMinEnabled = true,
					MinValue = 30,
					Stats = [new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Life=120, Mana=60, Spirit=20 => (T AND T) OR F = T OR F = T
			var items = CreateTestItems((60, 30, 10, 0, 0), (60, 30, 10, 0, 0));
			Assert(validator(items), "3G: (T AND T) OR F should pass");

			// Life=120, Mana=40, Spirit=40 => (T AND F) OR T = F OR T = T
			items = CreateTestItems((60, 20, 20, 0, 0), (60, 20, 20, 0, 0));
			Assert(validator(items), "3G: (T AND F) OR T should pass");

			// Life=80, Mana=40, Spirit=20 => (F AND F) OR F = F OR F = F
			items = CreateTestItems((40, 20, 10, 0, 0), (40, 20, 10, 0, 0));
			Assert(!validator(items), "3G: (F AND F) OR F should fail");

			Console.WriteLine("✓ 3Groups/Complex tests passed");
		}

		public static void Test_3Groups_MultipleStats()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 50,
					GroupOperator = "AND",
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }
					]
				},
				new()
				{
					GroupId = 2,
					IsMinEnabled = true,
					MinValue = 20,
					GroupOperator = "OR",
					Stats =
					[
						new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "+" },
						new() { PropInfo = PropFireRes, PropertyName = nameof(ItemStats.FireResistancePercent), Operator = "*" }
					]
				},
				new()
				{
					GroupId = 3,
					IsMinEnabled = true,
					MinValue = 10,
					Stats = [new() { PropInfo = PropColdRes, PropertyName = nameof(ItemStats.ColdResistancePercent), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// G1: (20+10)+(20+10) = 60 (pass)
			// G2: (5*2)+(5*2) = 20 (pass)
			// G3: 5+5 = 10 (pass)
			// Result: T AND T OR T = T
			var items = CreateTestItems((20, 10, 5, 2, 5), (20, 10, 5, 2, 5));
			Assert(validator(items), "3G/MultiStats: All pass should pass");

			Console.WriteLine("✓ 3Groups/MultipleStats tests passed");
		}

		#endregion

		#region 4 Groups Tests

		public static void Test_4Groups_AllOperators()
		{
			// G1 AND G2 XOR G3 OR G4 => ((G1 AND G2) XOR G3) OR G4
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "AND",
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 2,
					IsMinEnabled = true,
					MinValue = 50,
					GroupOperator = "XOR",
					Stats = [new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 3,
					IsMinEnabled = true,
					MinValue = 30,
					GroupOperator = "OR",
					Stats = [new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "+" }]
				},
				new()
				{
					GroupId = 4,
					IsMinEnabled = true,
					MinValue = 20,
					Stats = [new() { PropInfo = PropFireRes, PropertyName = nameof(ItemStats.FireResistancePercent), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Life=120, Mana=60, Spirit=20, FireRes=10
			// G1=T, G2=T, G3=F, G4=F
			// ((T AND T) XOR F) OR F = (T XOR F) OR F = T OR F = T
			var items = CreateTestItems((60, 30, 10, 5, 0), (60, 30, 10, 5, 0));
			Assert(validator(items), "4G: ((T AND T) XOR F) OR F should pass");

			// Life=120, Mana=60, Spirit=40, FireRes=10
			// G1=T, G2=T, G3=T, G4=F
			// ((T AND T) XOR T) OR F = (T XOR T) OR F = F OR F = F
			items = CreateTestItems((60, 30, 20, 5, 0), (60, 30, 20, 5, 0));
			Assert(!validator(items), "4G: ((T AND T) XOR T) OR F should fail");

			// Life=120, Mana=60, Spirit=40, FireRes=25
			// G1=T, G2=T, G3=T, G4=T
			// ((T AND T) XOR T) OR T = F OR T = T
			items = CreateTestItems((60, 30, 20, 12.5, 0), (60, 30, 20, 12.5, 0));
			Assert(validator(items), "4G: ((T AND T) XOR T) OR T should pass");

			Console.WriteLine("✓ 4Groups/AllOperators tests passed");
		}

		public static void Test_4Groups_ComplexStats()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 100,
					GroupOperator = "AND",
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "-" }
					]
				},
				new()
				{
					GroupId = 2,
					IsMinEnabled = true,
					MinValue = 40,
					GroupOperator = "OR",
					Stats =
					[
						new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "+" },
						new() { PropInfo = PropFireRes, PropertyName = nameof(ItemStats.FireResistancePercent), Operator = "*" }
					]
				},
				new()
				{
					GroupId = 3,
					IsMinEnabled = true,
					MinValue = 30,
					GroupOperator = "XOR",
					Stats =
					[
						new() { PropInfo = PropColdRes, PropertyName = nameof(ItemStats.ColdResistancePercent), Operator = "+" },
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "/" }
					]
				},
				new()
				{
					GroupId = 4,
					IsMinEnabled = true,
					MinValue = 15,
					Stats =
					[
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "+" },
						new() { PropInfo = PropSpirit, PropertyName = nameof(ItemStats.SpiritAmount), Operator = "-" },
						new() { PropInfo = PropFireRes, PropertyName = nameof(ItemStats.FireResistancePercent), Operator = "*" }
					]
				}
			};

			var validator = BuildValidator(groups);

			// Complex calculation test
			// Item: Life=100, Mana=20, Spirit=10, FireRes=2, ColdRes=20
			// G1: (100-20)+(100-20) = 160 (pass)
			// G2: (10*2)+(10*2) = 40 (pass)
			// G3: (20/100)+(20/100) = 0.4 (fail)
			// G4: ((20-10)*2)+((20-10)*2) = 40 (pass)
			// ((T AND T) OR F) XOR T = (T OR F) XOR T = T XOR T = F
			var items = CreateTestItems((100, 20, 10, 2, 20), (100, 20, 10, 2, 20));
			Assert(!validator(items), "4G/Complex: T XOR T should fail");

			Console.WriteLine("✓ 4Groups/ComplexStats tests passed");
		}

		#endregion

		#region Edge Cases

		public static void Test_DivisionByZero()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 10,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "/" }
					]
				}
			};

			var validator = BuildValidator(groups);

			// Mana=0 should not crash, should use life value
			var items = CreateTestItems((50, 0, 0, 0, 0));
			Assert(validator(items), "Edge: Division by zero should not crash");

			Console.WriteLine("✓ Edge case: DivisionByZero tests passed");
		}

		public static void Test_MinMaxConstraints()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = 50,
					IsMaxEnabled = true,
					MaxValue = 100,
					Stats = [new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" }]
				}
			};

			var validator = BuildValidator(groups);

			// Exactly min
			var items = CreateTestItems((25, 0, 0, 0, 0), (25, 0, 0, 0, 0));
			Assert(validator(items), "Edge: Exactly min (50) should pass");

			// Exactly max
			items = CreateTestItems((50, 0, 0, 0, 0), (50, 0, 0, 0, 0));
			Assert(validator(items), "Edge: Exactly max (100) should pass");

			// Just below min
			items = CreateTestItems((24, 0, 0, 0, 0), (25, 0, 0, 0, 0));
			Assert(!validator(items), "Edge: Just below min (49) should fail");

			// Just above max
			items = CreateTestItems((50, 0, 0, 0, 0), (51, 0, 0, 0, 0));
			Assert(!validator(items), "Edge: Just above max (101) should fail");

			Console.WriteLine("✓ Edge case: Min/Max constraints tests passed");
		}

		public static void Test_NegativeValues()
		{
			var groups = new List<ValidationGroupModel>
			{
				new()
				{
					GroupId = 1,
					IsMinEnabled = true,
					MinValue = -50,
					IsMaxEnabled = true,
					MaxValue = 0,
					Stats =
					[
						new() { PropInfo = PropMaxLife, PropertyName = nameof(ItemStats.MaximumLifeAmount), Operator = "+" },
						new() { PropInfo = PropMana, PropertyName = nameof(ItemStats.MaximumManaAmount), Operator = "-" }
					]
				}
			};

			var validator = BuildValidator(groups);

			// Life=10, Mana=30 => 10-30 = -20 per item => -40 total
			var items = CreateTestItems((10, 30, 0, 0, 0), (10, 30, 0, 0, 0));
			Assert(validator(items), "Edge: Negative result in range should pass");

			Console.WriteLine("✓ Edge case: Negative values tests passed");
		}

		#endregion

		#region Helper Methods

		private static Func<List<Item>, bool> BuildValidator(List<ValidationGroupModel> groups)
		{
			return items =>
			{
				if (groups.Count == 0) return true;

				bool result = EvaluateGroup(groups[0], items);

				for (int i = 1; i < groups.Count; i++)
				{
					bool nextResult = EvaluateGroup(groups[i], items);
					string op = groups[i - 1].GroupOperator ?? "AND";

					result = op switch
					{
						"AND" => result && nextResult,
						"OR" => result || nextResult,
						"XOR" => result ^ nextResult,
						_ => result && nextResult
					};
				}

				return result;
			};
		}

		private static bool EvaluateGroup(ValidationGroupModel group, List<Item> items)
		{
			if (group.Stats.Count == 0) return true;

			double sum = items.Sum(item => EvaluateExpression(group.Stats, item.ItemStats));

			if (group.IsMinEnabled && group.MinValue.HasValue && sum < group.MinValue.Value)
				return false;

			if (group.IsMaxEnabled && group.MaxValue.HasValue && sum > group.MaxValue.Value)
				return false;

			return true;
		}

		private static double EvaluateExpression(List<GroupStatModel> stats, ItemStats itemStats)
		{
			if (stats.Count == 0) return 0;

			double result = Convert.ToDouble(stats[0].PropInfo.GetValue(itemStats));

			for (int i = 1; i < stats.Count; i++)
			{
				double nextValue = Convert.ToDouble(stats[i].PropInfo.GetValue(itemStats));

				result = stats[i].Operator switch
				{
					"+" => result + nextValue,
					"-" => result - nextValue,
					"*" => result * nextValue,
					"/" => nextValue != 0 ? result / nextValue : result,
					_ => result + nextValue
				};
			}

			return result;
		}

		private static void Assert(bool condition, string message)
		{
			if (!condition)
				throw new Exception($"TEST FAILED: {message}");
		}

		#endregion

		#region Test Runner

		public static void RunAllTests()
		{
			Console.WriteLine("=== VALIDATION FUNCTION TESTS ===\n");

			try
			{
				// 1 Group Tests
				Test_1Group_1Stat_Addition();
				Test_1Group_2Stats_AdditionSubtraction();
				Test_1Group_3Stats_MultiplicationDivision();
				Test_1Group_4Stats_AllOperators();

				// 2 Groups Tests
				Test_2Groups_AND_Operator();
				Test_2Groups_OR_Operator();
				Test_2Groups_XOR_Operator();

				// 3 Groups Tests
				Test_3Groups_Complex_AND_OR();
				Test_3Groups_MultipleStats();

				// 4 Groups Tests
				Test_4Groups_AllOperators();
				Test_4Groups_ComplexStats();

				// Edge Cases
				Test_DivisionByZero();
				Test_MinMaxConstraints();
				Test_NegativeValues();

				Console.WriteLine("\n=== ALL TESTS PASSED ===");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"\n!!! TEST SUITE FAILED !!!\n{ex.Message}");
				throw;
			}
		}

		#endregion
	}
}