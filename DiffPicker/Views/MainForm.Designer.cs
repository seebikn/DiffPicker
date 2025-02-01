namespace DiffPicker
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            TextBoxBefore = new TextBox();
            label2 = new Label();
            TextBoxAfter = new TextBox();
            label3 = new Label();
            TextBoxDiffFolderName = new TextBox();
            label4 = new Label();
            TextBoxDiffPath = new TextBox();
            TextBoxOmitFilename = new TextBox();
            TextBoxOmitFolder = new TextBox();
            label5 = new Label();
            label6 = new Label();
            ButtomComplementBefore = new Button();
            ButtomComplementAfter = new Button();
            ButtomExecute = new Button();
            TextBoxResult = new TextBox();
            ButtonClear = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 18);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "修正前パス";
            // 
            // TextBoxBefore
            // 
            TextBoxBefore.AllowDrop = true;
            TextBoxBefore.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TextBoxBefore.Location = new Point(108, 15);
            TextBoxBefore.Name = "TextBoxBefore";
            TextBoxBefore.Size = new Size(476, 23);
            TextBoxBefore.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 47);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 2;
            label2.Text = "修正後パス";
            // 
            // TextBoxAfter
            // 
            TextBoxAfter.AllowDrop = true;
            TextBoxAfter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TextBoxAfter.Location = new Point(108, 44);
            TextBoxAfter.Name = "TextBoxAfter";
            TextBoxAfter.Size = new Size(476, 23);
            TextBoxAfter.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 105);
            label3.Name = "label3";
            label3.Size = new Size(66, 15);
            label3.TabIndex = 4;
            label3.Text = "差分フォルダ";
            // 
            // TextBoxDiffFolderName
            // 
            TextBoxDiffFolderName.Location = new Point(108, 102);
            TextBoxDiffFolderName.Name = "TextBoxDiffFolderName";
            TextBoxDiffFolderName.Size = new Size(248, 23);
            TextBoxDiffFolderName.TabIndex = 5;
            TextBoxDiffFolderName.Text = "比較結果";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 76);
            label4.Name = "label4";
            label4.Size = new Size(74, 15);
            label4.TabIndex = 6;
            label4.Text = "差分出力パス";
            // 
            // TextBoxDiffPath
            // 
            TextBoxDiffPath.AllowDrop = true;
            TextBoxDiffPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            TextBoxDiffPath.Location = new Point(108, 73);
            TextBoxDiffPath.Name = "TextBoxDiffPath";
            TextBoxDiffPath.Size = new Size(476, 23);
            TextBoxDiffPath.TabIndex = 7;
            // 
            // TextBoxOmitFilename
            // 
            TextBoxOmitFilename.Location = new Point(108, 131);
            TextBoxOmitFilename.Name = "TextBoxOmitFilename";
            TextBoxOmitFilename.Size = new Size(248, 23);
            TextBoxOmitFilename.TabIndex = 8;
            TextBoxOmitFilename.Text = ".dll;.exe;";
            // 
            // TextBoxOmitFolder
            // 
            TextBoxOmitFolder.Location = new Point(108, 160);
            TextBoxOmitFolder.Name = "TextBoxOmitFolder";
            TextBoxOmitFolder.Size = new Size(248, 23);
            TextBoxOmitFolder.TabIndex = 9;
            TextBoxOmitFolder.Text = "bin;Debug;Release;.git;.github;.vs;";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 134);
            label5.Name = "label5";
            label5.Size = new Size(65, 15);
            label5.TabIndex = 10;
            label5.Text = "除外ファイル";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 163);
            label6.Name = "label6";
            label6.Size = new Size(66, 15);
            label6.TabIndex = 11;
            label6.Text = "除外フォルダ";
            // 
            // ButtomComplementBefore
            // 
            ButtomComplementBefore.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtomComplementBefore.Location = new Point(590, 15);
            ButtomComplementBefore.Name = "ButtomComplementBefore";
            ButtomComplementBefore.Size = new Size(87, 23);
            ButtomComplementBefore.TabIndex = 12;
            ButtomComplementBefore.Text = "差分出力パス";
            ButtomComplementBefore.UseVisualStyleBackColor = true;
            // 
            // ButtomComplementAfter
            // 
            ButtomComplementAfter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ButtomComplementAfter.Location = new Point(590, 43);
            ButtomComplementAfter.Name = "ButtomComplementAfter";
            ButtomComplementAfter.Size = new Size(87, 23);
            ButtomComplementAfter.TabIndex = 13;
            ButtomComplementAfter.Text = "差分出力パス";
            ButtomComplementAfter.UseVisualStyleBackColor = true;
            // 
            // ButtomExecute
            // 
            ButtomExecute.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtomExecute.Location = new Point(590, 156);
            ButtomExecute.Name = "ButtomExecute";
            ButtomExecute.Size = new Size(87, 23);
            ButtomExecute.TabIndex = 14;
            ButtomExecute.Text = "比較";
            ButtomExecute.UseVisualStyleBackColor = true;
            // 
            // TextBoxResult
            // 
            TextBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            TextBoxResult.BackColor = SystemColors.Control;
            TextBoxResult.Font = new Font("ＭＳ ゴシック", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            TextBoxResult.Location = new Point(362, 102);
            TextBoxResult.Multiline = true;
            TextBoxResult.Name = "TextBoxResult";
            TextBoxResult.ReadOnly = true;
            TextBoxResult.Size = new Size(220, 103);
            TextBoxResult.TabIndex = 15;
            // 
            // ButtonClear
            // 
            ButtonClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonClear.Location = new Point(590, 185);
            ButtonClear.Name = "ButtonClear";
            ButtonClear.Size = new Size(87, 23);
            ButtonClear.TabIndex = 16;
            ButtonClear.Text = "クリア";
            ButtonClear.UseVisualStyleBackColor = true;
            ButtonClear.Click += ButtonClear_Click;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(689, 218);
            Controls.Add(ButtonClear);
            Controls.Add(TextBoxResult);
            Controls.Add(ButtomExecute);
            Controls.Add(ButtomComplementAfter);
            Controls.Add(ButtomComplementBefore);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(TextBoxOmitFolder);
            Controls.Add(TextBoxOmitFilename);
            Controls.Add(TextBoxDiffPath);
            Controls.Add(label4);
            Controls.Add(TextBoxDiffFolderName);
            Controls.Add(label3);
            Controls.Add(TextBoxAfter);
            Controls.Add(label2);
            Controls.Add(TextBoxBefore);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DiffPicker";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox TextBoxBefore;
        private Label label2;
        private TextBox TextBoxAfter;
        private Label label3;
        private TextBox TextBoxDiffFolderName;
        private Label label4;
        private TextBox TextBoxDiffPath;
        private TextBox TextBoxOmitFilename;
        private TextBox TextBoxOmitFolder;
        private Label label5;
        private Label label6;
        private Button ButtomComplementBefore;
        private Button ButtomComplementAfter;
        private Button ButtomExecute;
        private TextBox TextBoxResult;
        private Button ButtonClear;
    }
}
