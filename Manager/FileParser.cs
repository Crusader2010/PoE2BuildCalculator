using System.Text;

namespace Manager
{
    /// <summary>
    /// Single-pass file parser that reports progress based on underlying stream position (bytes).
    /// </summary>
    public class FileParser(string filePath) : IFileParser
    {
        private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

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

            // Open as async stream. Leave default buffer size; enable useAsync for async IO.
            await using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

            // Prefer to detect encoding from BOM.
            using var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true);

            long totalBytes = -1;
            bool canReportByBytes = false;

            try
            {
                if (fs.CanSeek)
                {
                    totalBytes = fs.Length;
                    canReportByBytes = totalBytes > 0;
                }
            }
            catch
            {
                // If we can't get length for any reason, fall back to line counting later.
                canReportByBytes = false;
            }

            // If we can measure bytes, perform a single-pass parse and report progress based on stream position.
            if (canReportByBytes)
            {
                int lastReported = -1;
                while (!sr.EndOfStream)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Read one line (async with cancellation)
                    string line = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                    if (line is null) break;

                    // TODO: per-line processing here
                    // e.g. ProcessLine(line);

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
                    if ((position % 10) == 0) await Task.Yield();
                }

                // Ensure final 100% report.
                progress?.Report(100);
                return;
            }

            // Fallback: if we can't determine length, do a two-pass line count to get exact line-based progress.
            long totalLines = 0;
            // Reset stream to start
            fs.Seek(0, SeekOrigin.Begin);
            sr.DiscardBufferedData();

            while (!sr.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (line is null) break;

                totalLines++;

                if ((totalLines % 100) == 0) await Task.Yield();
            }

            if (totalLines == 0)
            {
                progress?.Report(100);
                return;
            }

            // Second pass: process lines and report based on line count.
            long linesRead = 0;
            fs.Seek(0, SeekOrigin.Begin);
            sr.DiscardBufferedData();

            int lastPercent = -1;
            while (!sr.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var line = await sr.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                if (line is null) break;

                // TODO: per-line processing here
                // e.g. ProcessLine(line);

                linesRead++;
                int percent = (int)((linesRead * 100L) / totalLines);
                if (percent < 0) percent = 0;
                if (percent > 100) percent = 100;

                if (percent != lastPercent)
                {
                    lastPercent = percent;
                    progress?.Report(percent);
                }

                if ((linesRead % 100) == 0) await Task.Yield();
            }

            progress?.Report(100);
        }
    }
}