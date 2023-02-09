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
    public partial class Brightness : Form
    {
        Bitmap image,newimage;
        public Brightness(Bitmap Image)
        {
            InitializeComponent();
            image = Image;
            pictureBox1.Image = image;

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            for(int y =0;y<image.Height; y++)
            {
                for(int x =0; x<image.Width;x++)
                {

                }
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {

                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int degree = trackBar1.Value * 255 / 100;
            label5.Text = Convert.ToString(trackBar1.Value);
            newimage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int R  = image.GetPixel(x, y).R + degree;
                    int G = image.GetPixel(x, y).G + degree;
                    int B = image.GetPixel(x, y).B + degree;
                    if (R > 255) { R = 255; }
                    else if (R < 0) { R = 0; }
                    if (G > 255) { G = 255; }
                    else if (G < 0) { G = 0; }
                    if (B > 255) { B = 255; }
                    else if (B < 0) { B = 0; }
                    newimage.SetPixel(x, y, Color.FromArgb(R,G,B));
                }
            }
            pictureBox1.Image = newimage;
        }
    }
}
