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
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 0;
            label1.Text = "修正前パス";
            // 
            // TextBoxBefore
            // 
            TextBoxBefore.AllowDrop = true;
            TextBoxBefore.Location = new Point(108, 15);
            TextBoxBefore.Name = "TextBoxBefore";
            TextBoxBefore.Size = new Size(510, 23);
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
            TextBoxAfter.Location = new Point(108, 44);
            TextBoxAfter.Name = "TextBoxAfter";
            TextBoxAfter.Size = new Size(510, 23);
            TextBoxAfter.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 76);
            label3.Name = "label3";
            label3.Size = new Size(66, 15);
            label3.TabIndex = 4;
            label3.Text = "差分フォルダ";
            // 
            // TextBoxDiffFolderName
            // 
            TextBoxDiffFolderName.AllowDrop = true;
            TextBoxDiffFolderName.Location = new Point(108, 73);
            TextBoxDiffFolderName.Name = "TextBoxDiffFolderName";
            TextBoxDiffFolderName.Size = new Size(108, 23);
            TextBoxDiffFolderName.TabIndex = 5;
            TextBoxDiffFolderName.Text = "xx_差分比較";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 105);
            label4.Name = "label4";
            label4.Size = new Size(74, 15);
            label4.TabIndex = 6;
            label4.Text = "差分出力パス";
            // 
            // TextBoxDiffPath
            // 
            TextBoxDiffPath.AllowDrop = true;
            TextBoxDiffPath.Location = new Point(108, 102);
            TextBoxDiffPath.Name = "TextBoxDiffPath";
            TextBoxDiffPath.Size = new Size(510, 23);
            TextBoxDiffPath.TabIndex = 7;
            // 
            // TextBoxOmitFilename
            // 
            TextBoxOmitFilename.Location = new Point(108, 131);
            TextBoxOmitFilename.Name = "TextBoxOmitFilename";
            TextBoxOmitFilename.Size = new Size(309, 23);
            TextBoxOmitFilename.TabIndex = 8;
            TextBoxOmitFilename.Text = ".dll;.exe;";
            // 
            // TextBoxOmitFolder
            // 
            TextBoxOmitFolder.Location = new Point(108, 160);
            TextBoxOmitFolder.Name = "TextBoxOmitFolder";
            TextBoxOmitFolder.Size = new Size(309, 23);
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
            ButtomComplementBefore.Location = new Point(624, 15);
            ButtomComplementBefore.Name = "ButtomComplementBefore";
            ButtomComplementBefore.Size = new Size(87, 23);
            ButtomComplementBefore.TabIndex = 12;
            ButtomComplementBefore.Text = "差分出力パス";
            ButtomComplementBefore.UseVisualStyleBackColor = true;
            // 
            // ButtomComplementAfter
            // 
            ButtomComplementAfter.Location = new Point(624, 43);
            ButtomComplementAfter.Name = "ButtomComplementAfter";
            ButtomComplementAfter.Size = new Size(87, 23);
            ButtomComplementAfter.TabIndex = 13;
            ButtomComplementAfter.Text = "差分出力パス";
            ButtomComplementAfter.UseVisualStyleBackColor = true;
            // 
            // ButtomExecute
            // 
            ButtomExecute.Location = new Point(624, 160);
            ButtomExecute.Name = "ButtomExecute";
            ButtomExecute.Size = new Size(87, 23);
            ButtomExecute.TabIndex = 14;
            ButtomExecute.Text = "比較";
            ButtomExecute.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(723, 201);
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
    }
}
