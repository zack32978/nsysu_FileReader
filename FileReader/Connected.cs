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
    public partial class Connected : Form
    {
        
        public Connected(Bitmap Image)
        {
            InitializeComponent();
            pictureBox1.Image = binarization(Image, Otsu(Image));
            pictureBox2.Image = ConnectedComponent(binarization(Image, Otsu(Image)));

        }
        #region binary
        public int Otsu(Bitmap image)
        {
            //0:前景 1:背景
            double w0, w1; //pixel num比例
            double m0, m1;
            int totpixel = image.Width * image.Height;
            int val;
            int threshold = 0;
            double max_s1 = 0, s1 = 0;
            for (int t = 0; t < 256; t++)
            {
                w0 = 0;
                w1 = 0;
                m0 = 0;
                m1 = 0;
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        val = (int)image.GetPixel(x, y).R;
                        if (val < t){w0++;m0+=val;}
                        else{ w1++;m1+=val;}
                    }
                }
                m0 /= w0;
                m1 /= w1;
                w0 /= totpixel;
                w1 /= totpixel;
                s1 = w0 * w1 * Math.Pow((m0 - m1), 2);
                if (s1 > max_s1){max_s1 = s1;threshold = t;}
            }
            return threshold;
        }
        public Bitmap binarization(Bitmap image, int value)
        {
            int pixel;
            Bitmap newimage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (image.GetPixel(x, y).R < value)
                    {pixel = 0;}
                    else
                    {pixel = 255;}
                    Color color = Color.FromArgb(pixel, pixel, pixel);
                    newimage.SetPixel(x, y, color);
                }
            }
            return newimage;
        }
        #endregion
        public Bitmap ConnectedComponent(Bitmap image)
        {
            int num=1;
            int[,] LabelRegion = new int[image.Width, image.Height];
            Bitmap CCimage = new Bitmap(image.Width, image.Height);
            for (int y= 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int c = image.GetPixel(x,y).R;
                    if (c == 0) { LabelRegion[x, y] = 0; }//黑
                    else                                  //白
                    {
                        if (x == 0 && y == 0)
                        {
                            LabelRegion[x, y] = num;
                            num++;
                        }
                        else if (x != 0 && y == 0)   //最上一列
                        {
                            if (LabelRegion[x - 1, y] != 0)
                            { LabelRegion[x, y] = LabelRegion[x - 1, y]; }
                            else
                            {
                                LabelRegion[x, y] = num;
                                num++;
                            }
                        }
                        else if (x == 0 && y != 0) //最左行
                        {
                            if (LabelRegion[x, y - 1] != 0)
                            { LabelRegion[x, y] = LabelRegion[x, y - 1]; }
                            else
                            { LabelRegion[x, y] = num; num++;}
                        }
                        else
                        {                                                                 // .T.
                                                                                          // Lp.
                            if (LabelRegion[x, y - 1] != 0 && LabelRegion[x - 1, y] == 0)     //if(L==0 && T!=0) p = T
                            { LabelRegion[x, y] = LabelRegion[x, y - 1]; }
                            else if (LabelRegion[x - 1, y] != 0 && LabelRegion[x, y - 1] == 0)//if(L!=0 && T==0) p = L
                            { LabelRegion[x, y] = LabelRegion[x - 1, y]; }
                            else if (LabelRegion[x - 1, y] != 0 && LabelRegion[x, y - 1] != 0)//if(L!=0 && T!=0) p = the small one
                            {
                                int TOPLabel = LabelRegion[x, y - 1];
                                int LeftLabel = LabelRegion[x - 1, y];
                                
                                if (TOPLabel != LeftLabel) //left!=top
                                {
                                    if (TOPLabel >= LeftLabel) { LabelRegion[x, y] = LeftLabel; }
                                    else { LabelRegion[x, y] = TOPLabel; }

                                    for (int j = 0; j < image.Height; j++)
                                    {
                                        for (int i = 0; i < image.Width; i++)
                                        {
                                            if (LabelRegion[i, j] == LeftLabel)
                                            { LabelRegion[i, j] = TOPLabel; }
                                        }
                                    }
                                }
                                else{ LabelRegion[x, y] = LabelRegion[x, y - 1]; }
                            }
                            else                                                            //if(L==0 && T==0)
                            {
                                LabelRegion[x, y] = num;
                                num++;
                            }
                        }
                    }
                }
            }
            Color[] pixel = { Color.Red,Color.Green,Color.Blue, Color.Yellow, Color.White };
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (LabelRegion[x, y] != 0)
                    {
                        Color color = pixel[LabelRegion[x, y] % 5];
                        CCimage.SetPixel(x, y, color);
                    }
                    else
                    {
                        LabelRegion[x, y] = 0;
                        Color color = Color.FromArgb(LabelRegion[x, y], LabelRegion[x, y], LabelRegion[x, y]);
                        CCimage.SetPixel(x, y, color);
                    }
                }
            }
            return CCimage;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
