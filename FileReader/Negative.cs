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
    public partial class Negative : Form
    {
        public Negative(Bitmap image)
        {
            InitializeComponent();
            Bitmap RGB_Negative = RGBNegative(image);
            Bitmap Gray_Negative = GrayNegative(image);
            pictureBox1.Image = RGB_Negative;
            pictureBox2.Image = Gray_Negative;
        }

        public Bitmap image2Graylevel(Bitmap image)
        {
            int Width = image.Width;
            int Height = image.Height;
            Bitmap Grayimage = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int gray = ((image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) / 3);
                    Color color = Color.FromArgb(gray, gray, gray);
                    Grayimage.SetPixel(x, y, color);
                }
            }
            return Grayimage;
        }
        public Bitmap GrayNegative(Bitmap image)
        {
            Bitmap grayimage = image2Graylevel(image);
            Bitmap negative = new Bitmap(grayimage.Width, grayimage.Height);
            for (int y = 0; y < grayimage.Height; y++)
            {
                for (int x = 0; x < grayimage.Width; x++)
                {
                    int RN = 255 - grayimage.GetPixel(x, y).R;
                    int GN = 255 - grayimage.GetPixel(x, y).G;
                    int BN = 255 - grayimage.GetPixel(x, y).B;
                    Color color = Color.FromArgb(RN, GN, BN);
                    negative.SetPixel(x, y, color);
                }
            }
            return negative;
        }
        public Bitmap RGBNegative(Bitmap image)
        {
            Bitmap negative = new Bitmap(image.Width,image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int RN = 255 - image.GetPixel(x, y).R;
                    int GN = 255 - image.GetPixel(x, y).G;
                    int BN = 255 - image.GetPixel(x, y).B;
                    Color color = Color.FromArgb(RN, GN, BN);
                    negative.SetPixel(x, y, color);
                }
            }
            return negative;
        }

        private void Negative_Load(object sender, EventArgs e)
        {

        }
    }
}
