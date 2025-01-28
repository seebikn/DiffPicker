using System.Diagnostics;
using DiffPicker.Models;

namespace DiffPicker.Controllers
{
    internal class MainController
    {
        private readonly MainForm view;
        private readonly MainModel model;

        public MainController()
        {
            model = new MainModel();
            view = new MainForm();

            view.OnHandleExecuteComparison += HandleExecuteComparison;
            view.OnHandleComplementDiffPath += HandleComplementDiffPath;

            view.OnHandleDragEnter += HandleDragEnter;
            view.OnHandleDragDrop += HandleDragDrop;
        }

        public void Run()
        {
            Application.Run(view);
        }

        private void HandleExecuteComparison(object? sender, EventArgs e)
        {
            try
            {
                var diffFolderName = view.GetTextBoxDiffFolderName();
                if (string.IsNullOrWhiteSpace(diffFolderName))
                {
                    MessageBox.Show("差分フォルダを入力してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 各フォルダパスの取得とエラーチェック
                var diffPathModel = new FilePathModel(view.GetTextBoxDiffPath(), "差分出力パス");
                var beforePathModel = new FilePathModel(view.GetTextBoxBefore(), "修正前パス");
                var afterPathModel = new FilePathModel(view.GetTextBoxAfter(), "修正後パス");

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
                    var result = MessageBox.Show("差分フォルダを削除してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 除外ファイル名、除外フォルダ名を取得
                var omitFiles = view.GetTextBoxOmitFilename().Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();
                var omitFolders = view.GetTextBoxOmitFolder().Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();

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

                var bepath = beforePathModel.EnumerateFiles();
                foreach (var be in bepath)
                {
                    Debug.WriteLine(be);
                }

                // 差分抽出
                model.CompareAndCopy(
                          beforePathModel
                        , afterPathModel
                    );

                // 作業フォルダの後始末
                beforePathModel.CleanupWorkingDirectory();
                afterPathModel.CleanupWorkingDirectory();

                MessageBox.Show("比較が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var (isPath, _) = IsDragDrop(e);
                if (isPath)
                {
                    e.Effect = DragDropEffects.Copy; // フォルダまたはZIPファイルならコピー可能
                    return;
                }
            }
            e.Effect = DragDropEffects.None; // ドロップ不可
        }

        /// <summary>
        /// テキストボックスにフォルダまたはZIPファイルのドラッグ＆ドロップならコピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleDragDrop(object? sender, DragEventArgs e)
        {
            var (isPath, path) = IsDragDrop(e);
            if (isPath)
            {
                ((TextBox)sender!).Text = path;
            }
        }

        /// <summary>
        /// ディレクトリパスかzipパスであればtrueを返す
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static (bool isPath, string path) IsDragDrop(DragEventArgs e)
        {
            var paths = (string[])e.Data!.GetData(DataFormats.FileDrop)!;
            var model = new FilePathModel(paths[0], string.Empty);
            var res = model.IsValidPath() || model.IsZipFile();
            return (res, model.ManagedPath );
        }
        #endregion

        #region " 差分出力パスにディレクトリを設定する "
        private void HandleComplementDiffPath(object? sender, EventArgs e)
        {
            var path = ((TextBox)sender!).Text;
            string parentPath = Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar))!;
            view.SetTextBoxDiffPath(parentPath);
        }
        #endregion

    }

}
