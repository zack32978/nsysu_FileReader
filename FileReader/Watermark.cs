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
    public partial class Watermark : Form
    {
        public Watermark(Bitmap image3,double SNR)
        {
            InitializeComponent();
            Bitplane(image3);
            label1.Text = "SNR: " + Convert.ToString(SNR) + " dB";
            //pictureBox1.Image = image1;
            //pictureBox2.Image = image2;
            //pictureBox3.Image = image3;
        }
        public void Bitplane(Bitmap image)
        {
            pictureBox1.Image = image;//灰階圖
            Bitmap[] bitmaps = new Bitmap[8];
            for (int i = 0; i < 8; i++)
            {
                bitmaps[i] = new Bitmap(image.Width, image.Height);
            }
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int bitPlane;
                    Color c = image.GetPixel(x, y);
                    int[] planes = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        planes[k] = 0;
                    }
                    //0 bit plane 
                    bitPlane = c.R & 0x1;// 00000001
                    if (bitPlane == 1)
                    {
                        planes[0] = 255;
                    }
                    //1 bit plane 
                    bitPlane = c.R & 0x2;// 00000010
                    if (bitPlane == 0x2)
                    {
                        planes[1] = 255;
                    }
                    //2 bit plane 
                    bitPlane = c.R & 0x4;// 00000100
                    if (bitPlane == 0x4)
                    {
                        planes[2] = 255;
                    }
                    //3 bit plane
                    bitPlane = c.R & 0x8;// 00001000
                    if (bitPlane == 0x8)
                    {
                        planes[3] = 255;
                    }
                    //4 bit plane
                    bitPlane = c.R & 0x10;// 00010000
                    if (bitPlane == 0x10)
                    {
                        planes[4] = 255;
                    }
                    //5 bit plane
                    bitPlane = c.R & 0x20;// 00100000
                    if (bitPlane == 0x20)
                    {
                        planes[5] = 255;
                    }
                    //6 bit plane
                    bitPlane = c.R & 0x40;// 01000000
                    if (bitPlane == 0x40)
                    {
                        planes[6] = 255;
                    }
                    //7 bit plane
                    bitPlane = c.R & 0x80;// 10000000
                    if (bitPlane == 0x80)
                    {
                        planes[7] = 255;
                    }
                    for (int k = 0; k < 8; k++)
                    {
                        bitmaps[k].SetPixel(x, y, Color.FromArgb(planes[k], planes[k], planes[k]));
                    }
                }
            }
            pictureBox2.Image = bitmaps[7];
            pictureBox3.Image = bitmaps[6];
            pictureBox4.Image = bitmaps[5];
            pictureBox5.Image = bitmaps[4];
            pictureBox6.Image = bitmaps[3];
            pictureBox7.Image = bitmaps[2];
            pictureBox8.Image = bitmaps[1];
            pictureBox9.Image = bitmaps[0];


        }
        private void Watermark_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {

        }
    }
}
