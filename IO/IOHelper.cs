namespace Allycs.Common.IO
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// IO工具类，提供文件和目录操作相关功能
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// 确保目录存在，如果不存在则创建
        /// </summary>
        /// <param name="path">目录路径</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 安全删除目录，忽略异常
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <param name="recursive">是否递归删除子目录</param>
        /// <returns>删除成功返回true</returns>
        public static bool SafeDeleteDirectory(string path, bool recursive = false)
        {
            if (!Directory.Exists(path))
                return true;

            try
            {
                Directory.Delete(path, recursive);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 安全删除文件，忽略异常
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>删除成功返回true</returns>
        public static bool SafeDeleteFile(string path)
        {
            if (!File.Exists(path))
                return true;

            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取文本文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码，默认UTF-8</param>
        /// <returns>文件内容</returns>
        public static string ReadAllText(string path, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            return File.ReadAllText(path, encoding);
        }

        /// <summary>
        /// 写入文本文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="contents">文件内容</param>
        /// <param name="encoding">编码，默认UTF-8</param>
        public static void WriteAllText(string path, string contents, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            File.WriteAllText(path, contents, encoding);
        }

        /// <summary>
        /// 追加内容到文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="contents">要追加的内容</param>
        /// <param name="encoding">编码，默认UTF-8</param>
        public static void AppendAllText(string path, string contents, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            File.AppendAllText(path, contents, encoding);
        }

        /// <summary>
        /// 复制目录
        /// </summary>
        /// <param name="sourcePath">源目录路径</param>
        /// <param name="destPath">目标目录路径</param>
        /// <param name="overwrite">是否覆盖</param>
        public static void CopyDirectory(string sourcePath, string destPath, bool overwrite = true)
        {
            var sourceDir = new DirectoryInfo(sourcePath);
            if (!sourceDir.Exists)
            {
                throw new DirectoryNotFoundException($"源目录不存在: {sourcePath}");
            }

            EnsureDirectoryExists(destPath);

            // 复制文件
            foreach (var file in sourceDir.GetFiles())
            {
                var destFilePath = Path.Combine(destPath, file.Name);
                file.CopyTo(destFilePath, overwrite);
            }

            // 递归复制子目录
            foreach (var subDir in sourceDir.GetDirectories())
            {
                var destSubDir = Path.Combine(destPath, subDir.Name);
                CopyDirectory(subDir.FullName, destSubDir, overwrite);
            }
        }

        /// <summary>
        /// 计算文件大小并格式化显示
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>格式化的文件大小字符串</returns>
        public static string FormatFileSize(string filePath)
        {
            if (!File.Exists(filePath))
                return "0 B";

            var fileInfo = new FileInfo(filePath);
            return FormatFileSize(fileInfo.Length);
        }

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        /// <param name="bytes">字节数</param>
        /// <returns>格式化的文件大小字符串</returns>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 获取目录下所有文件（递归）
        /// </summary>
        /// <param name="directory">目录路径</param>
        /// <param name="searchPattern">搜索模式，默认*.*</param>
        /// <returns>文件路径数组</returns>
        public static string[] GetAllFiles(string directory, string searchPattern = "*.*")
        {
            if (!Directory.Exists(directory))
                return new string[0];

            try
            {
                return Directory.GetFiles(directory, searchPattern, SearchOption.AllDirectories);
            }
            catch
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 获取文件扩展名（不带点）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>扩展名</returns>
        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path)?.TrimStart('.') ?? string.Empty;
        }

        /// <summary>
        /// 获取不包含扩展名的文件名
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件名</returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 检查文件是否被占用
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>如果文件被占用返回true</returns>
        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// 安全读取所有文本，如果文件不存在返回null
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码，默认UTF-8</param>
        /// <returns>文件内容或null</returns>
        public static string SafeReadAllText(string path, Encoding encoding = null)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                return ReadAllText(path, encoding);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 清理临时文件和旧文件
        /// </summary>
        /// <param name="directory">目录路径</param>
        /// <param name="maxAgeHours">最大保留小时数</param>
        /// <param name="searchPattern">搜索模式</param>
        /// <returns>删除的文件数量</returns>
        public static int CleanOldFiles(string directory, int maxAgeHours, string searchPattern = "*.*")
        {
            if (!Directory.Exists(directory))
                return 0;

            var cutoffTime = DateTime.Now.AddHours(-maxAgeHours);
            var filesToDelete = new DirectoryInfo(directory)
                .GetFiles(searchPattern, SearchOption.TopDirectoryOnly)
                .Where(f => f.CreationTime <= cutoffTime)
                .ToList();

            int deletedCount = 0;
            foreach (var file in filesToDelete)
            {
                if (SafeDeleteFile(file.FullName))
                {
                    deletedCount++;
                }
            }

            return deletedCount;
        }
    }
}
