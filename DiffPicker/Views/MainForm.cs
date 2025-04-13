using System.Windows.Forms;

namespace DiffPicker
{
    public partial class MainForm : Form
    {
        public event EventHandler? OnHandleExecuteComparison;
        public event EventHandler? OnHandleComplementDiffPath;
        public event DragEventHandler? OnHandleDragEnter;
        public event DragEventHandler? OnHandleDragDrop;

        public string BeforePath
        {
            get => TextBoxBefore.Text;
            set => TextBoxBefore.Text = value;
        }
        public string AfterPath
        {
            get => TextBoxAfter.Text;
            set => TextBoxAfter.Text = value;
        }
        public string DiffPath
        {
            get => TextBoxDiffPath.Text;
            set => TextBoxDiffPath.Text = value;
        }
        public string DiffFolderName
        {
            get => TextBoxDiffFolder.Text;
            set => TextBoxDiffFolder.Text = value;
        }
        public string OmitFile
        {
            get => TextBoxOmitFile.Text;
            set => TextBoxOmitFile.Text = value;
        }
        public string OmitFolder
        {
            get => TextBoxOmitFolder.Text;
            set => TextBoxOmitFolder.Text = value;
        }
        public string Result
        {
            set => TextBoxResult.Text = value;
        }

        public MainForm()
        {
            InitializeComponent();
            ButtomExecute.Click += (s, e) => OnHandleExecuteComparison?.Invoke(s, e);

            ButtomComplementBefore.Click += (s, e) => OnHandleComplementDiffPath?.Invoke(TextBoxBefore, e);
            ButtomComplementAfter.Click += (s, e) => OnHandleComplementDiffPath?.Invoke(TextBoxAfter, e);

            this.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            this.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
            TextBoxBefore.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxBefore.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
            TextBoxAfter.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxAfter.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
            TextBoxDiffPath.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxDiffPath.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
        }

        public void SetReadOnlyState(bool isReadOnly)
        {
            TextBoxBefore.ReadOnly = isReadOnly;
            TextBoxAfter.ReadOnly = isReadOnly;
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            if (TextBoxBefore.ReadOnly || TextBoxAfter.ReadOnly)
            {
                // ロックされている場合は解除する
                SetReadOnlyState(false);
            }
            else
            {
                // ロックされていない場合はテキストクリア
                TextBoxBefore.Text = string.Empty;
                TextBoxAfter.Text = string.Empty;
                TextBoxDiffPath.Text = string.Empty;
                TextBoxResult.Text = string.Empty;
            }
        }

        private void ButtomChanges_Click(object sender, EventArgs e)
        {
            var before = TextBoxBefore.Text;
            var after = TextBoxAfter.Text;
            TextBoxBefore.Text = after;
            TextBoxAfter.Text = before;
        }
    }

}
