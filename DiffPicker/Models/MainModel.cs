using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;

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
        public int[] CompareAndCopy(FilePathModel beforePathModel, FilePathModel afterPathModel)
        {
            // コピー先・コピー元フォルダパスを取得
            string beforeSourcePath = beforePathModel.GetSourcePath();
            string beforeDestinationPath = beforePathModel.GetDestinationPath();
            string afterSourcePath = afterPathModel.GetSourcePath();
            string afterDestinationPath = afterPathModel.GetDestinationPath();

            // ファイル一覧を取得
            var beforeFiles = beforePathModel.EnumerateFiles();
            var afterFiles = afterPathModel.EnumerateFiles();
            List<string> targetFiles;

            // 戻り値
            int retBeforeCount = beforeFiles.Count;     // ファイル総数
            int retAfterCount = afterFiles.Count;
            int retBeforeDiffCount = 0;                 // beforeにのみ存在するファイル数
            int retAfterDiffCount = 0;
            int retDiffCount = 0;                       // before/afterで互いに異なる数

            {
                // 互いに存在しないファイルを差分としてdiffPathにコピーする

                // それぞれに存在しないファイルを取得
                var beforeExceptFiles = beforeFiles.Except(afterFiles).ToArray();
                var afterExceptFiles = afterFiles.Except(beforeFiles).ToArray();
                retBeforeDiffCount = beforeExceptFiles.Length;
                retAfterDiffCount = afterExceptFiles.Length;

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
                        Interlocked.Increment(ref retDiffCount); // スレッドセーフにカウントアップ
                    }

                    CopyFileToDiffPath(beforeFilePath, beforeDestinationPath, file);
                    CopyFileToDiffPath(afterFilePath, afterDestinationPath, file);
                }
            });

            return [retBeforeCount, retAfterCount, retBeforeDiffCount, retAfterDiffCount, retDiffCount];
        }

        #region " ハッシュ比較 "
        /// <summary>
        /// 2つのファイルのハッシュを比較する
        /// エクセルの場合は特殊処理
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns>true:差分あり false:差分なし</returns>
        private static bool EqualFiles(string file1, string file2)
        {
            if (Path.GetExtension(file1).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                    Path.GetExtension(file2).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return EqualXlsxFiles(file1, file2);
            }

            return _EqualFiles(file1, file2);
        }
        /// <summary>
        /// 2つのファイルのハッシュを比較する
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns>true:差分あり false:差分なし</returns>
        private static bool _EqualFiles(string file1, string file2)
        {
            using var hash = SHA256.Create();
            using var stream1 = File.OpenRead(file1);
            using var stream2 = File.OpenRead(file2);
            return hash.ComputeHash(stream1).SequenceEqual(hash.ComputeHash(stream2));
        }

        /// <summary>
        /// 2つのexcelのsheetファイルのハッシュを比較する
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        private static bool _EqualExcelsheetFiles(string file1, string file2)
        {
            using var hash = SHA256.Create();
            byte[] hash1 = ComputeXmlHashWithoutSelection(file1, hash);
            byte[] hash2 = ComputeXmlHashWithoutSelection(file2, hash);
            return hash1.SequenceEqual(hash2);
        }

        /// <summary>
        /// XMLファイルを読み込み、不要なタグを削除してハッシュを計算
        /// 不要なタグ：selectionタグにアクティブセルの情報が記録される
        /// </summary>
        private static byte[] ComputeXmlHashWithoutSelection(string filePath, HashAlgorithm hash)
        {
            string content = File.ReadAllText(filePath);

            // <sheetView> タグの中に <selection> がある場合、<sheetView> タグごとを削除
            content = Regex.Replace(content, @"<sheetView([^>]*)>\s*<selection[^>]+/>\s*</sheetView>", "", RegexOptions.Singleline);

            // 単体の <sheetView> タグを削除
            content = Regex.Replace(content, @"<sheetView[^>]+/>", "", RegexOptions.Singleline);

            return hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));
        }

        #endregion

        #region " Excelファイル(xlsx)のハッシュ比較 "
        /// <summary>
        /// Excelファイル(xlsx)のハッシュ比較
        /// xlsxのzipを解凍し、中のファイルに差異がないかチェックする。
        /// 一部のファイルは更新日付などを持つため除外する。
        /// </summary>
        /// <param name="xlsx1"></param>
        /// <param name="xlsx2"></param>
        /// <returns></returns>
        private static bool EqualXlsxFiles(string xlsx1, string xlsx2)
        {
            string tempDir1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string tempDir2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            // ローカルメソッド：フォルダのファイルパスを返す。引数に除外リストを指定可能。
            List<string> GetFilteredFileList(string directory, HashSet<string> excludeFiles)
            {
                // ローカルメソッド：絶対パスを相対パスに変換
                string GetRelativePath(string basePath, string fullPath)
                {
                    return fullPath.Substring(basePath.Length + 1);
                }

                return Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                    .Where(f => !excludeFiles.Contains(GetRelativePath(directory, f))) // 除外リストにあるファイルを除く
                    .OrderBy(f => f)
                    .ToList();
            }

            try
            {
                // ZIP を解凍
                System.IO.Compression.ZipFile.ExtractToDirectory(xlsx1, tempDir1);
                System.IO.Compression.ZipFile.ExtractToDirectory(xlsx2, tempDir2);

                // 除外するファイルリスト
                HashSet<string> excludeFiles = new(StringComparer.OrdinalIgnoreCase)
                {
                    "docProps\\core.xml",        // 更新日付や更新者を持つため除外する
                    "xl\\workbook.xml",          // documentIdが変化するため除外する
                    "xl\\calcChain.xml",         // 再計算順序などが自動生成されるため除外する
                    // これ以外にも難しい比較が存在する。「セルに文字入力してセル確定。文字削除。」これで編集を加えた情報が残るため差異が発生する。現時点で対策なし。
                };

                // ファイル一覧を取得（除外リストにあるファイルを省く）
                var files1 = GetFilteredFileList(tempDir1, excludeFiles);
                var files2 = GetFilteredFileList(tempDir2, excludeFiles);

                // ファイル数が違えば false
                if (files1.Count != files2.Count)
                {
                    return false;
                }

                using var hash = SHA256.Create();

                // 各ファイルのハッシュを比較
                for (int i = 0; i < files1.Count; i++)
                {
                    if (files1[i].Contains("xl\\worksheets\\") && files1[i].EndsWith(".xml"))
                    {
                        // xl/worksheets/sheet1.xlsなどの場合
                        if (!_EqualExcelsheetFiles(files1[i], files2[i]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!_EqualFiles(files1[i], files2[i]))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            finally
            {
                // 一時フォルダのクリーンアップ
                if (Directory.Exists(tempDir1)) Directory.Delete(tempDir1, true);
                if (Directory.Exists(tempDir2)) Directory.Delete(tempDir2, true);
            }
        }
        #endregion
    }
}
