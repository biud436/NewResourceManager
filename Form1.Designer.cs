namespace NewResourceManager
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.folderList = new System.Windows.Forms.ListBox();
            this.fileList = new System.Windows.Forms.ListBox();
            this.preview_btn = new System.Windows.Forms.Button();
            this.import_btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.delete_btn = new System.Windows.Forms.Button();
            this.export_btn = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.xml_listBox = new System.Windows.Forms.ListBox();
            this.add_extension_btn = new System.Windows.Forms.Button();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.xml_delete_btn = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // folderList
            // 
            this.folderList.FormattingEnabled = true;
            this.folderList.HorizontalScrollbar = true;
            this.folderList.ItemHeight = 12;
            this.folderList.Location = new System.Drawing.Point(12, 12);
            this.folderList.Name = "folderList";
            this.folderList.Size = new System.Drawing.Size(210, 388);
            this.folderList.TabIndex = 0;
            this.folderList.SelectedIndexChanged += new System.EventHandler(this.folderList_SelectedIndexChanged);
            // 
            // fileList
            // 
            this.fileList.FormattingEnabled = true;
            this.fileList.ItemHeight = 12;
            this.fileList.Location = new System.Drawing.Point(232, 12);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(268, 388);
            this.fileList.TabIndex = 1;
            this.fileList.SelectedIndexChanged += new System.EventHandler(this.fileList_SelectedIndexChanged);
            this.fileList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileList_MouseDoubleClick);
            // 
            // preview_btn
            // 
            this.preview_btn.Location = new System.Drawing.Point(506, 12);
            this.preview_btn.Name = "preview_btn";
            this.preview_btn.Size = new System.Drawing.Size(166, 29);
            this.preview_btn.TabIndex = 2;
            this.preview_btn.Text = "미리보기";
            this.preview_btn.UseVisualStyleBackColor = true;
            this.preview_btn.Click += new System.EventHandler(this.preview_btn_Click);
            // 
            // import_btn
            // 
            this.import_btn.Location = new System.Drawing.Point(506, 47);
            this.import_btn.Name = "import_btn";
            this.import_btn.Size = new System.Drawing.Size(165, 28);
            this.import_btn.TabIndex = 3;
            this.import_btn.Text = "가져오기";
            this.import_btn.UseVisualStyleBackColor = true;
            this.import_btn.Click += new System.EventHandler(this.import_btn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "이미지 파일|*.png";
            // 
            // delete_btn
            // 
            this.delete_btn.Location = new System.Drawing.Point(506, 113);
            this.delete_btn.Name = "delete_btn";
            this.delete_btn.Size = new System.Drawing.Size(165, 28);
            this.delete_btn.TabIndex = 4;
            this.delete_btn.Text = "삭제";
            this.delete_btn.UseVisualStyleBackColor = true;
            this.delete_btn.Click += new System.EventHandler(this.delete_btn_Click);
            // 
            // export_btn
            // 
            this.export_btn.Location = new System.Drawing.Point(506, 79);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(165, 28);
            this.export_btn.TabIndex = 5;
            this.export_btn.Text = "내보내기";
            this.export_btn.UseVisualStyleBackColor = true;
            this.export_btn.Click += new System.EventHandler(this.export_btn_Click);
            // 
            // xml_listBox
            // 
            this.xml_listBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.xml_listBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.xml_listBox.FormattingEnabled = true;
            this.xml_listBox.ItemHeight = 12;
            this.xml_listBox.Location = new System.Drawing.Point(3, 54);
            this.xml_listBox.Name = "xml_listBox";
            this.xml_listBox.Size = new System.Drawing.Size(154, 158);
            this.xml_listBox.TabIndex = 6;
            this.xml_listBox.SelectedIndexChanged += new System.EventHandler(this.xml_listBox_SelectedIndexChanged);
            this.xml_listBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.xml_listBox_KeyDown);
            // 
            // add_extension_btn
            // 
            this.add_extension_btn.Location = new System.Drawing.Point(507, 147);
            this.add_extension_btn.Name = "add_extension_btn";
            this.add_extension_btn.Size = new System.Drawing.Size(165, 28);
            this.add_extension_btn.TabIndex = 7;
            this.add_extension_btn.Text = "확장 프로그램에 추가/제거";
            this.add_extension_btn.UseVisualStyleBackColor = true;
            this.add_extension_btn.Click += new System.EventHandler(this.add_extension_btn_Click);
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.xml_delete_btn);
            this.groupBox1.Controls.Add(this.xml_listBox);
            this.groupBox1.Location = new System.Drawing.Point(510, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 215);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "투명색";
            // 
            // xml_delete_btn
            // 
            this.xml_delete_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.xml_delete_btn.Enabled = false;
            this.xml_delete_btn.Location = new System.Drawing.Point(3, 20);
            this.xml_delete_btn.Name = "xml_delete_btn";
            this.xml_delete_btn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.xml_delete_btn.Size = new System.Drawing.Size(66, 28);
            this.xml_delete_btn.TabIndex = 7;
            this.xml_delete_btn.Text = "Delete";
            this.xml_delete_btn.UseVisualStyleBackColor = true;
            this.xml_delete_btn.Click += new System.EventHandler(this.xml_delete_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 412);
            this.Controls.Add(this.add_extension_btn);
            this.Controls.Add(this.export_btn);
            this.Controls.Add(this.delete_btn);
            this.Controls.Add(this.import_btn);
            this.Controls.Add(this.preview_btn);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.folderList);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "New Resource Manager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListBox folderList;
        private System.Windows.Forms.ListBox fileList;
        private System.Windows.Forms.Button preview_btn;
        private System.Windows.Forms.Button import_btn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button delete_btn;
        private System.Windows.Forms.Button export_btn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ListBox xml_listBox;
        private System.Windows.Forms.Button add_extension_btn;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button xml_delete_btn;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

