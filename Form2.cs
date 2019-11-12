using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewResourceManager
{
    public partial class Form2 : Form
    {

        public string imgPath_;

        public Form2(string imgPath)
        {
            InitializeComponent();
            imgPath_ = imgPath;
            
            // ESC로 미리보기 창을 끌 수 있게 만듭니다.
            this.KeyPreview = true;
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            this.Text = Localization.Form2Title;
            close_btn.Text = Localization.Form2CloseButton;

            using (var bmp = new Bitmap(imgPath_))
            {
                pictureBox1.Image = new Bitmap(bmp);
            }

            this.CenterToScreen();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
            {
                pictureBox1.Image.Dispose();
                e.SuppressKeyPress = true;
                Close();
            }
        }
    }
}
