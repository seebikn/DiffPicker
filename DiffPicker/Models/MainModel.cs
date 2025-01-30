using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace DiffPicker.Models
{
    internal class MainModel
    {
        /// <summary>
        /// 2つの比較用パスから差分のあるファイルを別フォルダにコピーする
        /// </summary>
        /// <param name="beforePathModel">修正前パスモデル</param>
        /// <param name="afterPathModel">修正後パスモデル</param>
        /// <exception cref="Exception"></exception>
        public void CompareAndCopy(FilePathModel beforePathModel, FilePathModel afterPathModel)
        {
            // 差分が一件でもあるか管理するフラグ
            bool diff = false;
            var diffBag = new ConcurrentBag<bool>();

            // コピー先・コピー元フォルダパスを取得
            string beforeSourcePath = beforePathModel.GetSourcePath();
            string beforeDestinationPath = beforePathModel.GetDestinationPath();
            string afterSourcePath = afterPathModel.GetSourcePath();
            string afterDestinationPath = afterPathModel.GetDestinationPath();

            // ファイル一覧を取得
            var beforeFiles = beforePathModel.EnumerateFiles();
            var afterFiles = afterPathModel.EnumerateFiles();
            List<string> targetFiles;

            {
                // 互いに存在しないファイルを差分としてdiffPathにコピーする

                // それぞれに存在しないファイルを取得
                var beforeExceptFiles = beforeFiles.Except(afterFiles).ToArray();
                var afterExceptFiles = afterFiles.Except(beforeFiles).ToArray();

                if (beforeFiles.Count == beforeExceptFiles.Length
                    || afterFiles.Count == afterExceptFiles.Length)
                {
                    throw new Exception("ファイルが全て一致しないため処理を終了します。");
                }

                // ローカルメソッド：差分ファイルを差分出力先にコピー
                void CopyFilesToDiffPath(string workingPath, string destinationPath, string[] exceptFiles)
                {
                    Parallel.ForEach(exceptFiles, filepath =>
                    {
                        var sourceFilePath = Path.Combine(workingPath, filepath);
                        var destFilePath = Path.Combine(destinationPath, filepath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilePath)!);
                        File.Copy(sourceFilePath, destFilePath, true);
                        diffBag.Add(true);
                    });
                }

                CopyFilesToDiffPath(beforeSourcePath, beforeDestinationPath, beforeExceptFiles);
                CopyFilesToDiffPath(afterSourcePath, afterDestinationPath, afterExceptFiles);

                // 差分ファイルを取り除く
                targetFiles = beforeFiles.Except(beforeExceptFiles).ToList();
            }

            // ファイルのハッシュを比較し、異なる場合はファイルを複製する
            Parallel.ForEach(targetFiles, file =>
            {
                var beforeFilePath = Path.Combine(beforeSourcePath, file);
                var afterFilePath = Path.Combine(afterSourcePath, file);

                if (!EqualFiles(beforeFilePath, afterFilePath))
                {
                    // ファイルに差異がある場合、それぞれのファイルをコピーする

                    // ローカルメソッド：
                    void CopyFileToDiffPath(string sourceFilepath, string destinationPath, string filepath)
                    {
                        var destFilepath = Path.Combine(destinationPath, filepath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destFilepath)!);
                        File.Copy(sourceFilepath, destFilepath, true);
                        diffBag.Add(true);
                    }

                    CopyFileToDiffPath(beforeFilePath, beforeDestinationPath, file);
                    CopyFileToDiffPath(afterFilePath, afterDestinationPath, file);
                }
            });

            diff = !diffBag.IsEmpty;

            if (diff)
            {
                // 1つでも差異があった場合

                // 差分出力先パスを作成
                Directory.CreateDirectory(beforePathModel.GetDestinationPath());
                Directory.CreateDirectory(afterPathModel.GetDestinationPath());

                // winmerge比較ファイルを作成
                {
                    string winMergeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diff.WinMerge");
                    string destinationPath = Path.Combine(beforePathModel.DiffParentPath, "diff.WinMerge");
                    File.Copy(winMergeFilePath, destinationPath, true);

                    string content = File.ReadAllText(destinationPath);
                    content = content.Replace("@@@before@@@", "01_修正前");
                    content = content.Replace("@@@after@@@", "02_修正後");
                    File.WriteAllText(destinationPath, content);
                }
            }
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
            return hash.ComputeHash(stream1).SequenceEqual(hash.ComputeHash(stream2));
        }

    }
}
