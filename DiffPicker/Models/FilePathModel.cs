using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffPicker.Models
{

    /// <summary>
    /// ファイルパス管理モデル
    /// </summary>
    internal class FilePathModel
    {
        /// <summary>
        /// 画面上で指定されているパス
        /// </summary>
        public string ManagedPath { get;  }

        /// <summary>
        /// 画面上の名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 差分出力親パス
        /// </summary>
        public string DiffParentPath { get; set; }

        /// <summary>
        /// 差分フォルダ
        /// </summary>
        public string DiffFolder { get; set; }

        /// <summary>
        /// zip解凍フォルダ
        /// </summary>
        public string ZipFolder { get; }

        /// <summary>
        /// 除外ファイル名
        /// </summary>
        public List<string> OmitFiles { get; set; }

        /// <summary>
        /// 除外フォルダ名
        /// </summary>
        public List<string> OmitFolders { get; set; }

        /// <summary>
        /// 作業フォルダ名
        /// </summary>
        public string WorkingPath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public FilePathModel(string path, string type)
        {
            this.ManagedPath = path;
            this.Name = type;
            this.DiffParentPath = string.Empty;
            this.DiffFolder = string.Empty;
            this.ZipFolder = Guid.NewGuid().ToString();
            this.OmitFiles = [];
            this.OmitFolders = [];
            this.WorkingPath = string.Empty;
        }

        /// <summary>
        /// 管理パスが存在するディレクトリか確認
        /// </summary>
        /// <returns></returns>
        public bool IsValidPath()
        {
            return Directory.Exists(ManagedPath);
        }

        /// <summary>
        /// 管理パスがZIPファイルか確認
        /// </summary>
        /// <returns></returns>
        public bool IsZipFile()
        {
            return File.Exists(ManagedPath) && Path.GetExtension(ManagedPath).Equals(".zip", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 管理フォルダと指定のフォルダを結合して返す
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetManagedPathCombine(string path)
        {
            return Path.Combine(ManagedPath, path).ToString();
        }

        /// <summary>
        /// zip解凍先フォルダを返す
        /// </summary>
        /// <returns></returns>
        private string GetUnzipPath()
        {
            return Path.Combine(this.DiffParentPath, this.ZipFolder);
        }

        /// <summary>
        /// コピー先フォルダを返す
        /// </summary>
        /// <returns></returns>
        public string GetDestinationPath()
        {
            return Path.Combine(this.DiffParentPath, this.DiffFolder);
        }

        /// <summary>
        /// 作業フォルダを確定する。zipなら解凍してそこを作業フォルダに。
        /// 中にフォルダ1つしかない場合はさらにそこを作業フォルダに。
        /// </summary>
        public void ConfirmWorkingDirectory()
        {
            // 仮作業フォルダ
            string workingPath;

            if (IsZipFile())
            {
                // zipファイルを解凍する
                workingPath = GetUnzipPath();
                System.IO.Compression.ZipFile.ExtractToDirectory(this.ManagedPath, workingPath);
            }
            else
            {
                workingPath = this.ManagedPath;
            }

            // 仮作業フォルダ内のディレクトリとファイルを取得
            var directories = Directory.GetDirectories(workingPath);
            var files = Directory.GetFiles(workingPath);

            if (files.Length == 0 && directories.Length == 1)
            {
                // 仮作業フォルダ内にファイルがなく、フォルダが1つの場合はそこを作業フォルダとする
                this.WorkingPath = directories[0];
            }
            else
            {
                // 仮作業フォルダを作業フォルダとする
                this.WorkingPath = workingPath;
            }
        }

        /// <summary>
        /// 作業フォルダの後始末をする。
        /// zipなら作業フォルダを削除する。
        /// </summary>
        public void CleanupWorkingDirectory()
        {
            if (IsZipFile())
            {
                // zip解凍先フォルダを削除する
                var path = GetUnzipPath();
                if (Path.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        /// <summary>
        /// 作業フォルダのファイルの一覧を返す。
        /// 除外指定されたファイルやフォルダは除外したもの。
        /// </summary>
        /// <returns></returns>
        public List<string> EnumerateFiles()
        {
            string path = this.WorkingPath;
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            return files.Select(file => file.Replace(path, "").TrimStart('\\'))
                        .Where(file => !OmitFiles.Any(omit => file.EndsWith(omit, StringComparison.OrdinalIgnoreCase)) &&
                               !OmitFolders.Any(folder => file.Contains(Path.DirectorySeparatorChar + folder + Path.DirectorySeparatorChar))).ToList();
        }

    }
}
