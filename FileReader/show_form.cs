using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileReader
{
    public partial class show_form : Form
    {
        public void SetPicture(Image image1, Image image2, Image image3) 
        {
            pictureBox1.Image = image1;
            pictureBox2.Image = image2;
            pictureBox3.Image = image3;
            //pictureBox4.Image = image4;

        }

        public show_form()
        {
            InitializeComponent();
        }
        
        
        private void RGB_form_Load(object sender, EventArgs e)
        {
            
        }

        public void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        public void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        public void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
