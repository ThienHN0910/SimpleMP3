using System.Diagnostics;

namespace BusinessLogic.Helpers
{
    public static class YouTubeDownloader
    {
        public static async Task<string?> DownloadMp3Async(string url, string saveDirectory)
        {
            string outputTemplate = Path.Combine(saveDirectory, "%(title)s.%(ext)s");

            // Lấy danh sách file mp3 trước khi tải
            var beforeFiles = new HashSet<string>(
                Directory.GetFiles(saveDirectory, "*.mp3")
            );

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp",
                    Arguments = $"-x --audio-format mp3 --no-playlist -o \"{outputTemplate}\" \"{url}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            try
            {
                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    // Lấy danh sách file mp3 sau khi tải
                    var afterFiles = new HashSet<string>(
                        Directory.GetFiles(saveDirectory, "*.mp3")
                    );
                    // Tìm file mới
                    var newFiles = afterFiles.Except(beforeFiles).ToList();
                    if (newFiles.Count == 1)
                        return newFiles[0];
                    // Nếu có nhiều file mới, lấy file mới nhất
                    if (newFiles.Count > 1)
                        return newFiles.OrderByDescending(f => File.GetLastWriteTime(f)).First();
                }
                else
                {
                    string error = await process.StandardError.ReadToEndAsync();
                    Debug.WriteLine($"yt-dlp exited with code {process.ExitCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during YouTube download: {ex.Message}");
            }

            return null;
        }
    }
}