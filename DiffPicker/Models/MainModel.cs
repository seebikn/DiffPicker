using System.Security.Cryptography;

namespace DiffPicker.Models
{
    internal class MainModel
    {
        public void CompareAndCopyAsync(string beforePath, string afterPath, string diffPath, List<string> omitFiles, List<string> omitFolders)
        {
            // 差分が一件でもあるか管理するフラグ
            bool diff = false;

            // ファイル一覧を取得
            string beforeZipPath = string.Empty;
            string afterZipPath = string.Empty;
            var beforeFiles = EnumerateFiles(ref beforePath, ref beforeZipPath, omitFiles, omitFolders, diffPath);
            var afterFiles = EnumerateFiles(ref afterPath, ref afterZipPath, omitFiles, omitFolders, diffPath);
            List<string> targetFiles;

            {
                // 互いに存在しないファイルを差分としてdiffPathにコピーする

                // それぞれに存在しないファイルを取得
                var beforeExceptFiles = beforeFiles.Except(afterFiles).ToArray();
                var afterExceptFiles = afterFiles.Except(beforeFiles).ToArray();

                if (beforeFiles.Count() == beforeExceptFiles.Count() 
                    || afterFiles.Count() == afterExceptFiles.Count())
                {
                    throw new Exception("全て一致しないため処理を終了します。");
                }

                // ローカルメソッド：差分ファイルを差分パスにコピー
                void copyFilesToDiffPath(string path, string[] exceptFiles, string folderName)
                {
                    foreach (var filepath in exceptFiles)
                    {
                        var sourceFilePath = Path.Combine(path, filepath);
                        var destFilepath = Path.Combine(diffPath, folderName, filepath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilepath)!);

                        File.Copy(sourceFilePath, destFilepath);
                        diff = true;
                    }
                }
                copyFilesToDiffPath(beforePath, beforeExceptFiles, "1_before");
                copyFilesToDiffPath(afterPath, afterExceptFiles, "2_after");

                // 差分ファイルを取り除く
                targetFiles = beforeFiles.Except(beforeExceptFiles).ToList();
            }

            // ファイルのハッシュを比較し、異なる場合はファイルを複製する
            foreach (var file in targetFiles)
            {
                var beforeFilePath = Path.Combine(beforePath, file);
                var afterFilePath = Path.Combine(afterPath, file);

                if (!EqualFiles(beforeFilePath, afterFilePath))
                {
                    // ファイルに差異がある場合、それぞれのファイルをコピーする

                    void copyFileToDiffPath(string sourceFilePath, string folderName, string filepath)
                    {
                        var destFilepath = Path.Combine(diffPath, folderName, filepath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilepath)!);
                        File.Copy(sourceFilePath, destFilepath);
                        diff = true;
                    }
                    copyFileToDiffPath(beforeFilePath, "1_before", file);
                    copyFileToDiffPath(afterFilePath, "2_after", file);
                }
            }

            if (diff)
            {
                // 1つでも差異があった場合

                var beforeFilePath = Path.Combine(diffPath, "1_before");
                Directory.CreateDirectory(beforeFilePath);

                var afterFilePath = Path.Combine(diffPath, "2_after");
                Directory.CreateDirectory(afterFilePath);

                // todo : winmerge比較ファイルを作る
                string winMergeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diff.WinMerge");
                string destinationPath = Path.Combine(diffPath, "diff.WinMerge");
                File.Copy(winMergeFilePath, destinationPath, true);

                string content = File.ReadAllText(destinationPath);
                content = content.Replace("@@@before@@@", "1_before");
                content = content.Replace("@@@after@@@", "2_after");
                File.WriteAllText(destinationPath, content);
            }

            {
                // zipを解凍した場合はフォルダを削除

                if (!string.IsNullOrEmpty(beforeZipPath))
                {
                    Directory.Delete(beforeZipPath, true);
                }
                if (!string.IsNullOrEmpty(afterZipPath))
                {
                    Directory.Delete(afterZipPath, true);
                }
            }
        }

        /// <summary>
        /// パス配下のファイル一覧を返すメソッド
        /// 一覧から除外フォルダ、除外ファイルは除外する。
        /// パスがzipの場合は差分パス配下に解凍し、解凍したパスをrefで返す。
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="zipPath">パス</param>
        /// <param name="omitFiles">除外ファイル</param>
        /// <param name="omitFolders">除外フォルダ</param>
        /// <param name="diffPath">差分パス</param>
        /// <returns>パス配下のファイル一覧</returns>
        private static List<string> EnumerateFiles(ref string path, ref string zipPath, List<string> omitFiles, List<string> omitFolders, string diffPath)
        {
            if (File.Exists(path))
            {
                zipPath = Path.Combine(diffPath, Guid.NewGuid().ToString());
                System.IO.Compression.ZipFile.ExtractToDirectory(path, zipPath);
                path = Path.Combine(zipPath, System.IO.Path.GetFileNameWithoutExtension(path));
            }

            string path2 = path;
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            return files.Select(file => file.Replace(path2, "").TrimStart('\\'))
                        .Where(file => !omitFiles.Any(omit => file.EndsWith(omit, StringComparison.OrdinalIgnoreCase)) &&
                               !omitFolders.Any(folder => file.Contains(Path.DirectorySeparatorChar + folder + Path.DirectorySeparatorChar))).ToList();
        }

        /// <summary>
        /// 2つのファイルのハッシュを比較する
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns>true:差分あり false:差分なし</returns>
        private static bool EqualFiles(string file1, string file2)
        {
            using var hash = SHA256.Create();
            using var stream1 = File.OpenRead(file1);
            using var stream2 = File.OpenRead(file2);

            var hash1 = hash.ComputeHash(stream1);
            var hash2 = hash.ComputeHash(stream2);

            return hash1.SequenceEqual(hash2);
        }

    }
}
