using System.Diagnostics;
using System.Reflection;
using System.Text;
using DDic.Controllers;
using DiffPicker.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiffPicker.Controllers
{
    internal class MainController
    {
        private readonly MainForm view;
        private readonly MainModel model;
        private readonly IniController iniController;

        public MainController(string iniFilePath)
        {
            model = new MainModel();
            view = new MainForm();

            view.OnHandleExecuteComparison += HandleExecuteComparison;
            view.OnHandleComplementDiffPath += HandleComplementDiffPath;

            view.OnHandleDragEnter += HandleDragEnter;
            view.OnHandleDragDrop += HandleDragDrop;

            iniController = new IniController(iniFilePath);
        }

        public void Run()
        {
            {
                // アセンブリ名とバージョンを取得
                Assembly assembly = Assembly.GetExecutingAssembly();
                AssemblyName assemblyName = assembly.GetName();
                string appName = assemblyName.Name!;
                Version version = assemblyName.Version!;
                string majorVersion = version.Major.ToString();
                string minorVersion = version.Minor.ToString();

                // フォームのタイトルを設定
                view.Text = $"{appName} - Version {majorVersion}.{minorVersion}";
            }
            {
                // iniファイルを読み込み
                this.LoadSettings();
            }

            Application.Run(view);
        }
        private void LoadSettings()
        {
            iniController.InitializeFile();

            // カラム一覧 右クリックメニューの表示設定
            view.DiffFolderName = iniController.Get(Constants.IniMain.section, Constants.IniMain.diffFolderName, string.Empty);
            view.OmitFile = iniController.Get(Constants.IniMain.section, Constants.IniMain.omitFile, string.Empty);
            view.OmitFolder = iniController.Get(Constants.IniMain.section, Constants.IniMain.omitFolder, string.Empty);
        }

        private void HandleExecuteComparison(object? sender, EventArgs e)
        {
            try
            {
                var diffFolderName = view.DiffFolderName;
                if (string.IsNullOrWhiteSpace(diffFolderName))
                {
                    MessageBox.Show("差分フォルダを入力してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 各フォルダパスの取得とエラーチェック
                var diffPathModel = new FilePathModel(view.DiffPath, "差分出力パス");
                var beforePathModel = new FilePathModel(view.BeforePath, "修正前パス");
                var afterPathModel = new FilePathModel(view.AfterPath, "修正後パス");

                // ローカル関数：パスのエラーチェック
                static bool isPathError(FilePathModel model, bool zip)
                {
                    if (zip)
                    {
                        if (!(model.IsValidPath() || model.IsZipFile()))
                        {
                            MessageBox.Show($"{model.Name}にはフォルダパスかzipファイルパスを入力してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return true;
                        }
                    }
                    else
                    {
                        if (!model.IsValidPath())
                        {
                            MessageBox.Show($"{model.Name}にはフォルダパスを入力してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return true;
                        }
                    }
                    return false;
                }
                if (isPathError(diffPathModel, false) || isPathError(beforePathModel, true) || isPathError(afterPathModel, true))
                {
                    return;
                }

                // 差分フォルダパスのエラーチェック
                string diffParentPath = diffPathModel.GetManagedPathCombine(diffFolderName);
                if (Directory.Exists(diffParentPath))
                {
                    MessageBox.Show("差分フォルダを削除してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 除外ファイル名、除外フォルダ名を取得
                var omitFiles = view.OmitFile.Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();
                var omitFolders = view.OmitFolder.Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();

                // ファイルパスモデルに情報追加
                {
                    beforePathModel.DiffParentPath = diffParentPath;
                    beforePathModel.DiffFolder = "01_修正前";
                    beforePathModel.OmitFiles = omitFiles;
                    beforePathModel.OmitFolders = omitFolders;

                    afterPathModel.DiffParentPath = diffParentPath;
                    afterPathModel.DiffFolder = "02_修正後";
                    afterPathModel.OmitFiles = omitFiles;
                    afterPathModel.OmitFolders = omitFolders;
                }

                // 作業フォルダの確定指示
                beforePathModel.ConfirmWorkingDirectory();
                afterPathModel.ConfirmWorkingDirectory();

                // ReadOnlyに変更
                view.SetReadOnlyState(true);

                // 差分抽出
                Cursor.Current = Cursors.WaitCursor;
                int[] ret = model.CompareAndCopy(
                                  beforePathModel
                                , afterPathModel
                            );

                // 作業フォルダの後始末
                beforePathModel.CleanupWorkingDirectory();
                afterPathModel.CleanupWorkingDirectory();

                // 結果表示
                string result = $"実行時刻   {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\r\n"
                              + $"修正前ファイル数     " + ret[0].ToString("N0").PadLeft(9) + "\r\n"
                              + $"修正後ファイル数     " + ret[1].ToString("N0").PadLeft(9) + "\r\n"
                              + $"修正前専用ファイル数 " + ret[2].ToString("N0").PadLeft(9) + "\r\n"
                              + $"修正後専用ファイル数 " + ret[3].ToString("N0").PadLeft(9) + "\r\n"
                              + $"異なるファイル数     " + ret[4].ToString("N0").PadLeft(9) + "\r\n";
                view.Result = result;

                if (ret[2] + ret[3] + ret[4] > 0)
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

                    Encoding enc = Encoding.GetEncoding("UTF-8");
                    using (StreamWriter writer = new StreamWriter(Path.Combine(beforePathModel.DiffParentPath, "result.txt"), false, enc))
                    {
                        writer.WriteLine(result);
                    }

                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("比較が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("差異が見つかりません", "差異なし", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                view.SetReadOnlyState(false);
            }
        }

        #region " テキストボックスへのドラッグ＆ドロップ "
        /// <summary>
        /// テキストボックスにフォルダまたはZIPファイルのドラッグ＆ドロップならコピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data!.GetDataPresent(DataFormats.FileDrop))
            {
                var list = IsDragDrops(e);
                if (list.Count == list.Where(e => e.kind > 0).Count())
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// テキストボックスにフォルダまたはZIPファイルのドラッグ＆ドロップならコピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDragDrop(object? sender, DragEventArgs e)
        {
            var list = IsDragDrops(e);

            if (ReferenceEquals(sender, view))
            {
                // フォームにドロップされた場合、
                // 各テキストボックスの空き状況に応じてパスを設定

                foreach (var item in list)
                {
                    if (item.kind == 0)
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(view.BeforePath))
                    {
                        view.BeforePath = item.path;
                    }
                    else if (string.IsNullOrWhiteSpace(view.AfterPath))
                    {
                        view.AfterPath = item.path;
                    }
                    else if (item.kind == 1 && string.IsNullOrWhiteSpace(view.DiffPath))
                    {
                        // パスは可、zip不可
                        view.DiffPath = item.path;
                    }
                }
            }
            else
            {
                // ドロップされたテキストボックスにパスを設定

                if (list[0].kind > 0)
                {
                    if (!((TextBox)sender!).ReadOnly)
                    {
                        ((TextBox)sender!).Text = list[0].path;
                    }
                }
            }
        }

        /// <summary>
        /// ドラッグ＆ドロップされたパスがフォルダかzipかをチェックし、種類とそのパスを返す
        /// 種類  1:フォルダ  2:zip
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static List<(int kind, string path)> IsDragDrops(DragEventArgs e)
        {
            var list = new List<(int kind, string path)>();
            var paths = (string[])e.Data!.GetData(DataFormats.FileDrop)!;

            // ドロップされるパスの順序が任意のためソート
            Array.Sort(paths);

            foreach (var path in paths)
            {
                var model = new FilePathModel(path, string.Empty);
                var kind = (model.IsValidPath() ? 1 : 0) + (model.IsZipFile() ? 2 : 0);
                list.Add((kind, model.ManagedPath));
            }

            return list;
        }
        #endregion

        #region " 差分出力パスにディレクトリを設定する "
        private void HandleComplementDiffPath(object? sender, EventArgs e)
        {
            var path = ((TextBox)sender!).Text;
            string parentPath = Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar))!;
            view.DiffPath = parentPath;
        }
        #endregion

    }

}
