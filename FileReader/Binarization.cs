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
    public partial class Binarization : Form
    {
        Bitmap image;
        public Binarization(Bitmap Image)
        {
            InitializeComponent();
            image = Image;
            
            pictureBox1.Image = image;
            
        }
        public int Otsu(Bitmap image) 
        {
            int[] histogram = GrayHistogram(image);
            //0:前景 1:背景
            double w0, w1; //pixel num比例
            double m0, m1;             
            int totpixel = image.Width * image.Height;
            int val;
            int threshold = 0;
            double max_s1 =0 , s1 =0 ;
            for( int t = 0; t < 256; t++)
            {
                w0 = 0;
                w1 = 0;
                m0 = 0;
                m1 = 0;
                for( int y = 0; y < image.Height;y ++)
                {
                    for(int  x = 0;x<image.Width;x++)
                    {
                        val = (int)image.GetPixel(x, y).R;
                        if (val < t) 
                        {
                            w0++;
                            m0 += val;
                        }
                        else
                        {
                            w1++;
                            m1 += val;
                        }
                    }
                }
                m0 /= w0;
                m1 /= w1;
                w0 /= totpixel;
                w1 /= totpixel;
                s1 = w0 * w1 * Math.Pow((m0 - m1), 2);
                if(s1 > max_s1) 
                {
                    max_s1 = s1;
                    threshold = t;
                }
            }
            return threshold;
        }
        public int[] GrayHistogram(Bitmap image)
        {
            
            int[] countgray = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    countgray[gray.R]++;
                }
            }
            return countgray;
        }
        
        public Bitmap binarization(Bitmap image,int value)
        {
            int pixel;
            Bitmap newimage =  new Bitmap(image.Width,image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image.GetPixel(x, y).R < value)
                    {
                        pixel = 0;
                    }
                    else 
                    {
                        pixel = 255;
                    }
                    Color color = Color.FromArgb(pixel,pixel,pixel);
                    newimage.SetPixel(x, y, color);
                }
            }
            return newimage;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = binarization(image, Convert.ToInt32(textBox1.Text));
        }

        private void Binarization_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int threshold = Otsu(image);
            pictureBox3.Image = binarization(image, threshold);
            label1.Text = "Thresthold:" + Convert.ToString(threshold);
        }
    }
}
