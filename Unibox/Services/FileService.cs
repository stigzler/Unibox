using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Unibox.Services
{
    public class FileService
    {
        /// <summary>
        /// Asynchronously copies a file and reports progress every 500ms.
        /// </summary>
        /// <param name="sourcePath">The source file path.</param>
        /// <param name="destinationPath">The destination file path.</param>
        /// <param name="progress">An action to receive progress messages (percentage copied).</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task CopyFileAsync(
            string sourcePath,
            string destinationPath,
            Action<string> progress,
            CancellationToken cancellationToken = default)
        {
            const int bufferSize = 1024 * 1024; // 1MB buffer for fast copying
            var fileInfo = new FileInfo(sourcePath);
            long totalBytes = fileInfo.Length;
            long totalRead = 0;

            using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
            using var destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, useAsync: true);

            var buffer = new byte[bufferSize];
            var lastReportTime = DateTime.UtcNow;

            while (true)
            {
                int bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead == 0)
                    break;

                await destStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalRead += bytesRead;

                var now = DateTime.UtcNow;
                if ((now - lastReportTime).TotalMilliseconds >= 500 || totalRead == totalBytes)
                {
                    double percent = totalBytes > 0 ? (totalRead * 100.0 / totalBytes) : 100.0;
                    progress?.Invoke($"{percent:F1}%");
                    lastReportTime = now;
                }
            }
        }
    }
}