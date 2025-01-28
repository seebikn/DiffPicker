namespace DiffPicker
{
    public partial class MainForm : Form
    {
        public event EventHandler? OnHandleExecuteComparison;
        public event EventHandler? OnHandleComplementDiffPath;
        public event DragEventHandler? OnHandleDragEnter;
        public event DragEventHandler? OnHandleDragDrop;

        public string GetTextBoxBefore() => TextBoxBefore.Text;
        public string GetTextBoxAfter() => TextBoxAfter.Text;
        public string GetTextBoxDiffFolderName() => TextBoxDiffFolderName.Text;
        public string GetTextBoxDiffPath() => TextBoxDiffPath.Text;
        public string GetTextBoxOmitFilename() => TextBoxOmitFilename.Text;
        public string GetTextBoxOmitFolder() => TextBoxOmitFolder.Text;

        public void SetTextBoxDiffPath(string str)
        {
            TextBoxDiffPath.Text = str;
        }

        public MainForm()
        {
            InitializeComponent();
            ButtomExecute.Click += (s, e) => OnHandleExecuteComparison?.Invoke(s, e);

            ButtomComplementBefore.Click += (s, e) => OnHandleComplementDiffPath?.Invoke(TextBoxBefore, e);
            ButtomComplementAfter.Click += (s, e) => OnHandleComplementDiffPath?.Invoke(TextBoxAfter, e);

            TextBoxBefore.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxBefore.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
            TextBoxAfter.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxAfter.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
            TextBoxDiffPath.DragEnter += (s, e) => OnHandleDragEnter?.Invoke(s, e);
            TextBoxDiffPath.DragDrop += (s, e) => OnHandleDragDrop?.Invoke(s, e);
        }

    }

}
