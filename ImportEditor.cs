using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace NewResourceManager
{
    public partial class ImportEditor : Form
    {

        public Color currentColor = new Color();
        public string imagePath_;
        public string targetPath_;
        public Image image_;
        public bool isOk;
        public string transparentKey;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="targetPath"></param>
        public ImportEditor(string filePath, string targetPath)
        {
            InitializeComponent();

            using (var bmpFile = new Bitmap(filePath))
            {
                image_ = new Bitmap(bmpFile);
            }

            imagePath_ = filePath;
            pictureBox1.Image = image_;
            targetPath_ = targetPath;
            isOk = false;
            transparentKey = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button != MouseButtons.Left)
            {
                return;
            }

            Rectangle rect = panel1.Bounds;
            Bitmap bitmap = (Bitmap)pictureBox1.Image;

            int mouseX = e.Location.X;
            int mouseY = e.Location.Y;

            if (mouseX < 0)
            {
                mouseX = 0;
            }
            if (mouseX > pictureBox1.Image.Width)
            {
                mouseX = pictureBox1.Image.Width;
            }
            if (mouseY < 0)
            {
                mouseY = 0;
            }
            if (mouseY > pictureBox1.Image.Height)
            {
                mouseY = pictureBox1.Image.Height;
            }

            Color c = bitmap.GetPixel(mouseX, mouseY);
            currentColor = c;
            panel1.BackColor = currentColor;
            //panel1.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        public async Task CopyFileAsync(string sourcePath, string destinationPath)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ok_btn_Click(object sender, EventArgs e)
        {
            string[] splitStrings = imagePath_.Split(Path.DirectorySeparatorChar);
            int len = splitStrings.Length;
            string fileName = splitStrings[len - 1];

            string sourceFileName = imagePath_;
            string destFileName = Path.Combine(targetPath_, fileName);
            
            string r = currentColor.R.ToString();
            string g = currentColor.G.ToString();
            string b = currentColor.B.ToString();
            string a = currentColor.A.ToString();

            string[] destStrings = targetPath_.Split(Path.DirectorySeparatorChar);

            //string path = String.Format("{0}/{1}/{2}", destStrings[len - 2], destStrings[len - 1], fileName);

            transparentKey = String.Format(@"{0}:{1}:{2}:{3}:{4}", fileName, r, g, b, a);

            // 소스 폴더에 리소스가 있고, 복사 할 폴더에 리소스가 없다면.
            //if (File.Exists(sourceFileName) && !File.Exists(destFileName))
            if (File.Exists(sourceFileName))
            {
                // 복사 할 폴더에 파일을 복사합니다.
                //File.Copy(sourceFileName, destFileName, true);
                CopyFileAsync(sourceFileName, destFileName);

            } // 소스 폴더에 리소스가 있고, 복사 할 폴더에도 리소스가 있다면
            //else if (File.Exists(sourceFileName) && File.Exists(destFileName))
            //{
            //    // 복사 할 폴더에 파일을 복사합니다.
            //    File.Copy(sourceFileName, destFileName, true);

            //}

            // OK
            isOk = true;

            // 비트맵 메모리를 해제합니다.
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            
            Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_btn_Click(object sender, EventArgs e)
        {
            // 비트맵 메모리를 해제합니다.
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImportEditor_Load(object sender, EventArgs e)
        {
            ok_btn.Text = Localization.ImportEditorOkButton;
            cancel_btn.Text = Localization.ImportEditorCancelButton;
            this.Text = Localization.ImportEditorTitle;
            label1.Text = Localization.TransparentLabel;
        }
    }
}
