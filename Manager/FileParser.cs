using Domain;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Manager
{
    /// <summary>
    /// Single-pass file parser that reports progress based on underlying stream position (bytes).
    /// </summary>
    public partial class FileParser(string filePath) : IFileParser
    {
        private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        private readonly List<Item> _items = [];

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

            // Open as async stream. Enable useAsync for IO.
            await using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            using (var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
            {
                long totalBytes = fs.Length;
                int lastReported = -1;
                int itemId = 0;
                var item = new Item();

                while (!sr.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!await ProcessFileDataForItems(item, sr, itemId, cancellationToken)) break;

                    long position;
                    try
                    {
                        position = fs.Position;
                    }
                    catch
                    {
                        position = -1;
                    }

                    if (position >= 0 && totalBytes > 0)
                    {
                        int percent = (int)((position * 100L) / totalBytes);
                        if (percent < 0) percent = 0;
                        if (percent > 100) percent = 100;

                        // Only report when changed to reduce UI churn.
                        if (percent != lastReported)
                        {
                            lastReported = percent;
                            progress?.Report(percent);
                        }
                    }

                    // Yield occasionally to keep responsiveness.
                    if ((position % 5) == 0) await Task.Yield();
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

        private static void MapItemData(string line, Item item)
        {
            var m1 = RegexPatterns.ItemClassPattern().Match(line);
            if (m1.Success) item.Class = m1.Groups[1].Value.Trim();

            var m2 = RegexPatterns.ArmourAmountPattern().Match(line);
            if (m2.Success) item.ItemStats.ArmourAmount = Int32.Parse(m2.Groups[1].Value.Trim(), System.Globalization.NumberStyles.None);
        }

        private async Task<bool> ProcessFileDataForItems(Item item, StreamReader sr, int currentItemId, CancellationToken cancellationToken)
        {
            // Read one line (async with cancellation)
            string line = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null) return false;

            // Item delimiter logic
            if (line.StartsWith("Item Class:", StringComparison.OrdinalIgnoreCase))
            {
                string name1 = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                string name2 = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (name1 is null || name2 is null) return false;

                item = new Item() { Id = ++currentItemId, Name = $"{name1.Trim()} {name2.Trim()}", ItemStats = new() };
                _items.Add(item);
            }

            MapItemData(line, item);
            return true;
        }
    }
}