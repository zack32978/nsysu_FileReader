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
    public partial class Rotate : Form
    {
        Bitmap image;
        public Rotate(Bitmap Image)
        {
            InitializeComponent();
            image = Image;
        }
        //顺時針旋轉：圖像座標 --> 直角座標 -->  圖像旋轉  -->  圖像座標
        public Bitmap PosRotate(Bitmap image, double degree)
        {
            int Width = image.Width;
            int Height = image.Height;
            double RWidth = (int)(Math.Abs(Width * Math.Cos(Math.PI * (degree / 180))) + Math.Abs(Height * Math.Sin(Math.PI * (degree / 180))) + 0.5);
            double RHeight = (int)(Math.Abs(Width * Math.Sin(Math.PI * (degree / 180))) + Math.Abs(Height * Math.Cos(Math.PI * (degree / 180))) + 0.5);
            Bitmap Rimage = new Bitmap((int)RWidth, (int)RHeight);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int Rx = (int)Math.Round(x * Math.Cos(Math.PI * (degree / 180)) - y * Math.Sin(Math.PI * (degree / 180)) - (Width - 1) / 2.0 * Math.Cos(Math.PI * (degree / 180)) + (Height - 1) / 2.0 * Math.Sin(Math.PI * (degree / 180)) + (RWidth - 1) / 2.0);
                    int Ry = (int)Math.Round(x * Math.Sin(Math.PI * (degree / 180)) + y * Math.Cos(Math.PI * (degree / 180)) - (Width - 1) / 2.0 * Math.Sin(Math.PI * (degree / 180)) - (Height - 1) / 2.0 * Math.Cos(Math.PI * (degree / 180)) + (RHeight - 1) / 2.0);
                    Rimage.SetPixel(Rx, Ry, image.GetPixel(x, y));
                }
            }
            return Rimage;
        }

        public Bitmap RevRotate(Bitmap image, double degree)
        {
            int Width = image.Width;
            int Height = image.Height;
            double RWidth = (int)(Math.Abs(Width * Math.Cos(Math.PI * (degree / 180))) + Math.Abs(Height * Math.Sin(Math.PI * (degree / 180))) + 0.5);
            double RHeight = (int)(Math.Abs(Width * Math.Sin(Math.PI * (degree / 180))) + Math.Abs(Height * Math.Cos(Math.PI * (degree / 180))) + 0.5);
            Bitmap Rimage = new Bitmap((int)RWidth, (int)RHeight);
            //Rx Ry旋轉後坐標 x y原圖座標

            for (int Ry = 0; Ry < RHeight; Ry++) 
            {
                for (int Rx = 0; Rx < RWidth; Rx++) 
                {                 
                    double x = Rx * Math.Cos(Math.PI * (degree / 180)) + Ry * Math.Sin(Math.PI * (degree / 180)) - (RWidth - 1) / 2.0 * Math.Cos(Math.PI * (degree / 180)) - (RHeight - 1) / 2.0 * Math.Sin(Math.PI * (degree / 180)) + (Width - 1) / 2.0;
                    double y = -Rx * Math.Sin(Math.PI * (degree / 180)) + Ry * Math.Cos(Math.PI * (degree / 180)) + (RWidth - 1) / 2.0 * Math.Sin(Math.PI * (degree / 180)) - (RHeight - 1) / 2.0 * Math.Cos(Math.PI * (degree / 180)) + (Height - 1) / 2.0;
                    if ( 0<=x & x <= (Width - 1) & 0 <= y & y <= (Height - 1))
                    {
                        Color RGB = new Color();
                        int a1 = (int)x;
                        int b1 = (int)y;

                        int a2 = (int)Math.Ceiling(x);
                        int b2 = (int)y;

                        int a3 = (int)x;
                        int b3 = (int)Math.Ceiling(y);

                        int a4 = (int)Math.Ceiling(x);
                        int b4 = (int)Math.Ceiling(y);

                        double xa13 = x - a1;
                        double xa24 = a2 - x;
                        double yb12 = y - b1;
                        double yb34 = b3 - y;
                        //雙線性插值 對應原圖為非整數座標
                        if (xa13 != 0 & xa24 != 0 & yb12 != 0 & yb34 != 0)
                        {
                            byte R1 = image.GetPixel(a1, b1).R;
                            byte G1 = image.GetPixel(a1, b1).G;
                            byte B1 = image.GetPixel(a1, b1).B;

                            byte R2 = image.GetPixel(a2, b2).R;
                            byte G2 = image.GetPixel(a2, b2).G;
                            byte B2 = image.GetPixel(a2, b2).B;

                            byte R3 = image.GetPixel(a3, b3).R;
                            byte G3 = image.GetPixel(a3, b3).G;
                            byte B3 = image.GetPixel(a3, b3).B;

                            byte R4 = image.GetPixel(a4, b4).R;
                            byte G4 = image.GetPixel(a4, b4).G;
                            byte B4 = image.GetPixel(a4, b4).B;

                            byte R = (byte)((R1 * xa24 + R2 * xa13) * yb34 + (R3 * xa24 + R4 * xa13) * yb12);
                            byte G = (byte)((G1 * xa24 + G2 * xa13) * yb34 + (G3 * xa24 + G4 * xa13) * yb12);
                            byte B = (byte)((B1 * xa24 + B2 * xa13) * yb34 + (B3 * xa24 + B4 * xa13) * yb12);

                            RGB = Color.FromArgb(R, G, B);
                        }
                        else
                        {
                            RGB = image.GetPixel(a1, b1);
                        }
                        Rimage.SetPixel(Rx, Ry, RGB);
                    }
                }
            }
            return Rimage;
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                double degree = trackBar1.Value;
                pictureBox1.Image = PosRotate(image,degree);
                label3.Text =Convert.ToString( degree);

            }
            else if (radioButton2.Checked == true)
            {
                double degree = trackBar1.Value;
                pictureBox1.Image = RevRotate(image,degree);
                label3.Text = Convert.ToString(degree);
            }
        }
    }
}
