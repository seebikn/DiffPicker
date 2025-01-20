using DiffPicker.Models;

namespace DiffPicker.Controllers
{
    internal class MainController
    {
        private MainForm view;
        private MainModel model;

        public MainController()
        {
            model = new MainModel();
            view = new MainForm();

            view.OnHandleExecuteComparison += HandleExecuteComparison;
            view.OnHandleAdjustDiffPath += HandleAdjustDiffPath;
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
                var beforePath = view.GetTextBoxBefore();
                var afterPath = view.GetTextBoxAfter();
                var diffPath = view.GetTextBoxDiffPath();

                ValidatePaths(beforePath, afterPath, diffPath);

                if (Directory.Exists(diffPath))
                {
                    var result = MessageBox.Show("差分フォルダを削除してください。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                Directory.CreateDirectory(diffPath);

                var omitFiles = view.GetTextBoxOmitFilename().Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();
                var omitFolders = view.GetTextBoxOmitFolder().Split(';').Where(str => !string.IsNullOrEmpty(str)).ToList();

                model.CompareAndCopyAsync(
                          beforePath
                        , afterPath
                        , diffPath
                        , omitFiles
                        , omitFolders
                    );

                MessageBox.Show("比較が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidatePaths(string before, string after, string diff)
        {
            if (!Directory.Exists(before) && !File.Exists(before))
            {
                throw new ArgumentException("修正前のパスが存在しません。");
            }

            if (!Directory.Exists(after) && !File.Exists(after))
            {
                throw new ArgumentException("修正後のパスが存在しません。");
            }

            if (string.IsNullOrWhiteSpace(diff))
            {
                throw new ArgumentException("差分出力パスが指定されていません。");
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
                var paths = (string[])e.Data.GetData(DataFormats.FileDrop)!;
                if (paths.Length > 0 && (Directory.Exists(paths[0]) || IsZipFile(paths[0])))
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
            var paths = (string[])e.Data!.GetData(DataFormats.FileDrop)!;
            if (paths.Length > 0 && (Directory.Exists(paths[0]) || IsZipFile(paths[0])))
            {
                ((TextBox)sender!).Text = paths[0];
            }
        }

        private bool IsZipFile(string path)
        {
            return File.Exists(path) && Path.GetExtension(path).Equals(".zip", StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region " 差分出力パスの調整 "
        /// <summary>
        /// 差分出力パスの最後のフォルダが出力フォルダ名でない場合、最後に出力フォルダ名を加える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleAdjustDiffPath(object? sender, EventArgs e)
        {
            var diffFolderName = view.GetTextBoxDiffFolderName();
            var diffPath = view.GetTextBoxDiffPath();

            if (!diffPath.EndsWith(diffFolderName, StringComparison.OrdinalIgnoreCase))
            {
                view.SetTextBoxDiffPath(Path.Combine(diffPath, diffFolderName));
            }
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
