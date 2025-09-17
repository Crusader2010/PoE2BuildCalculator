using Domain;
using System.Collections.Immutable;
using System.Text;

namespace Manager
{
    /// <summary>
    /// Single-pass file parser that reports progress based on underlying stream position (bytes).
    /// </summary>
    public partial class FileParser(string filePath) : IFileParser
    {
        private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        private List<Item> _items = [];

        public async Task ParseFileAsync(IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                throw new FileNotFoundException("File path cannot be null or empty.", nameof(_filePath));
            }

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", _filePath);
            }

            _items = []; // reset item list whenever we parse

            // Open as async stream. Enable useAsync for IO.
            await using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            using (var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
            {
                long totalBytes = fs.Length;
                int lastReported = -1;
                int itemId = 0;
                Item currentItem = null;

                while (!sr.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await ProcessItemsFromFileData(currentItem, sr, itemId, cancellationToken).ConfigureAwait(false);
                    if (!result.Success) break;

                    // update local references with values returned from the helper
                    currentItem = result.Item;
                    itemId = result.CurrentItemId;

                    long position = fs.Position;
                    if (position >= 0 && totalBytes > 0)
                    {
                        int percent = (int)((position * 100L) / totalBytes);
                        if (percent > 100) percent = 100;

                        // Only report when changed to reduce UI churn.
                        if (percent != lastReported)
                        {
                            lastReported = percent;
                            progress?.Report(percent);
                        }
                    }

                    // Yield occasionally to keep responsiveness.
                    if (position >= 0 && (position % 8) == 0) await Task.Yield();
                }

                // Ensure final 100% report.
                progress?.Report(100);
                return;
            }
        }

        public ImmutableList<Item> GetParsedItems()
        {
            return [.. _items];
        }

        private static void MapItemStatsForItems(string line, Item item)
        {
            if (item?.ItemStats == null) return;

            var regex = RegexPatterns.ItemClassPattern().Match(line);
            if (regex.Success) item.Class = regex.Groups[1].Value.Trim();

            regex = RegexPatterns.ArmourAmountImplicitPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var armourImplicit)))
            {
                item.ItemStats.ArmourAmountImplicit = armourImplicit;
            }

            regex = RegexPatterns.ArmourAmountExplicitPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var armourExplicit)))
            {
                item.ItemStats.ArmourAmountExplicit = armourExplicit;
            }

            regex = RegexPatterns.EnergyShieldAmountImplicitPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var esImplicit)))
            {
                item.ItemStats.EnergyShieldAmount = esImplicit;
            }

            regex = RegexPatterns.MaximumLifeAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var maximumLife)))
            {
                item.ItemStats.MaximumLifeAmount = maximumLife;
            }

            regex = RegexPatterns.MaximumManaAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var maximumMana)))
            {
                item.ItemStats.MaximumManaAmount = maximumMana;
            }

            regex = RegexPatterns.SpiritAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var spirit)))
            {
                item.ItemStats.SpiritAmount = spirit;
            }

            regex = RegexPatterns.StunThresholdAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var stun)))
            {
                item.ItemStats.StunThresholdAmount = stun;
            }

            regex = RegexPatterns.PhysicalThornsRangePattern().Match(line);
            if (regex.Success)
            {
                if (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var thMin)
                    && Int32.TryParse(regex.Groups[2].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var thMax))
                {
                    item.ItemStats.PhysicalThornsMinDamageAmount = thMin;
                    item.ItemStats.PhysicalThornsMaxDamageAmount = thMax;
                }
            }


            // Spell skills
            regex = RegexPatterns.ColdSpellSkillsLevelPattern().Match(line);
            if (regex.Success && Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var lc))
            {
                item.ItemStats.ColdSpellSkillsLevel = lc;
            }

            regex = RegexPatterns.FireSpellSkillsLevelPattern().Match(line);
            if (regex.Success && Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var lf))
            {
                item.ItemStats.FireSpellSkillsLevel = lf;
            }

            regex = RegexPatterns.LightningSpellSkillsLevelPattern().Match(line);
            if (regex.Success && Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var ll))
            {
                item.ItemStats.LightningSpellSkillsLevel = ll;
            }

            regex = RegexPatterns.ChaosSpellSkillsLevelPattern().Match(line);
            if (regex.Success && Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var lcha))
            {
                item.ItemStats.ChaosSpellSkillsLevel = lcha;
            }
            // ----

            // Attributes
            regex = RegexPatterns.StrengthAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var str)))
            {
                item.ItemStats.Strength = str;
            }

            regex = RegexPatterns.DexterityAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var dex)))
            {
                item.ItemStats.Dexterity = dex;
            }

            regex = RegexPatterns.IntelligenceAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var intel)))
            {
                item.ItemStats.Intelligence = intel;
            }

            regex = RegexPatterns.AllAttributesAmountPattern().Match(line);
            if (regex.Success && (Int32.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var allAttrib)))
            {
                item.ItemStats.AllAttributes = allAttrib;
            }
            // ----

            // Resistances
            regex = RegexPatterns.FireResistancePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var fireRes))
            {
                item.ItemStats.FireResistancePercent = fireRes;
            }

            regex = RegexPatterns.LightningResistancePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var lightningRes))
            {
                item.ItemStats.LightningResistancePercent = lightningRes;
            }

            regex = RegexPatterns.ColdResistancePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var coldRes))
            {
                item.ItemStats.ColdResistancePercent = coldRes;
            }

            regex = RegexPatterns.ChaosResistancePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var chaosRes))
            {
                item.ItemStats.ChaosResistancePercent = chaosRes;
            }

            regex = RegexPatterns.AllElementalResistancesPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var allElemRes))
            {
                item.ItemStats.AllElementalResistancesPercent = allElemRes;
            }

            // ----

            // Damage %s
            regex = RegexPatterns.FireDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var fireDmg))
            {
                item.ItemStats.FireDamagePercent = fireDmg;
            }

            regex = RegexPatterns.LightningDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var lightningDmg))
            {
                item.ItemStats.LightningDamagePercent = lightningDmg;
            }

            regex = RegexPatterns.ColdDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var coldDmg))
            {
                item.ItemStats.ColdDamagePercent = coldDmg;
            }

            regex = RegexPatterns.ChaosDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var chaosDmg))
            {
                item.ItemStats.ChaosDamagePercent = chaosDmg;
            }

            regex = RegexPatterns.AllDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var allDmg))
            {
                item.ItemStats.AllDamagePercent = allDmg;
            }

            regex = RegexPatterns.PhysicalDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var physDmg))
            {
                item.ItemStats.PhysicalDamagePercent = physDmg;
            }

            regex = RegexPatterns.SpellDamagePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var spellDmg))
            {
                item.ItemStats.SpellDamagePercent = spellDmg;
            }
            // ----

            // Other %
            regex = RegexPatterns.MaximumLifePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var maxLife))
            {
                item.ItemStats.MaximumLifePercent = maxLife;
            }

            regex = RegexPatterns.EnergyShieldPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var energyShieldPercent))
            {
                item.ItemStats.EnergyShieldPercent = energyShieldPercent;
            }

            regex = RegexPatterns.ArmourPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var armourPercent))
            {
                item.ItemStats.ArmourPercent = armourPercent;
            }

            regex = RegexPatterns.ManaRegenPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var manaRegenPercent))
            {
                item.ItemStats.ManaRegenPercent = manaRegenPercent;
            }

            regex = RegexPatterns.LifeRegenAmountPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var lifeRegenAmount))
            {
                item.ItemStats.LifeRegenAmount = lifeRegenAmount;
            }

            regex = RegexPatterns.BlockChancePercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var blockChance))
            {
                item.ItemStats.BlockChancePercent = blockChance;
            }

            regex = RegexPatterns.PhysicalDamageReductionPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var phyDamageReductionPercent))
            {
                item.ItemStats.PhysicalDamageReductionPercent = phyDamageReductionPercent;
            }

            // ----

            // Speed / Rarity
            regex = RegexPatterns.CastSpeedPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var castSpeed))
            {
                item.ItemStats.CastSpeedPercent = castSpeed;
            }

            regex = RegexPatterns.AttackSpeedPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var attackSpeed))
            {
                item.ItemStats.AttackSpeedPercent = attackSpeed;
            }

            regex = RegexPatterns.RarityPercentPattern().Match(line);
            if (regex.Success && double.TryParse(regex.Groups[1].Value.Trim(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out var rarity))
            {
                item.ItemStats.RarityPercent = rarity;
            }

            // Enchant / custom string field
            regex = RegexPatterns.EnchantPattern().Match(line);
            if (regex.Success)
            {
                item.ItemStats.Enchant = regex.Groups[1].Value.Trim();
            }
        }

        // Return a tuple so the helper can create/replace the current item and increment the id
        private async Task<(bool Success, Item Item, int CurrentItemId)> ProcessItemsFromFileData(Item item, StreamReader sr, int currentItemId, CancellationToken cancellationToken)
        {
            // Read one line (async with cancellation)
            string line = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null) return (false, item, currentItemId);

            // Item delimiter logic
            if (line.StartsWith(Constants.ITEM_CLASS_TAG, StringComparison.OrdinalIgnoreCase))
            {
                string name1 = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                string name2 = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (name1 is null || name2 is null) return (false, item, currentItemId);

                var newItem = new Item()
                {
                    Id = ++currentItemId,
                    Name = $"{name1.Trim()} {name2.Trim()}",
                    IsMine = name1.Contains(Constants.ITEM_IS_MINE_TAG, StringComparison.OrdinalIgnoreCase),
                    ItemStats = new()
                };

                _items.Add(newItem);
                item = newItem;
            }

            // Map data into the current item
            if (item != null)
            {
                MapItemStatsForItems(line, item);
            }

            return (true, item, item.Id);
        }
    }
}
