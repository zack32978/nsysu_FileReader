using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCXDecoder;
using BMPDecoder;
namespace FileReader
{
    public partial class Histogram : Form
    {
        Bitmap image1;
        public Histogram(Bitmap image)
        {
            InitializeComponent();
            #region chart設定
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart3.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart3.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart3.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart4.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart4.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart4.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            #endregion
            image1 = image;
            pictureBox1.Image = image;
            pictureBox2.Image = image2Graylevel(image);
            int[] countR, countG, countB, CDFR, CDFG, CDFB;

            RGBHistogram(image, out countR, out countG, out countB);
            getCDF(image, out CDFR, out CDFG, out CDFB);
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, Convert.ToString(countR[i]));
                chart1.Series[1].Points.AddXY(i, Convert.ToString(countG[i]));
                chart1.Series[2].Points.AddXY(i, Convert.ToString(countB[i]));
                chart2.Series[0].Points.AddXY(i, Convert.ToString(CDFR[i]));
                chart2.Series[1].Points.AddXY(i, Convert.ToString(CDFG[i]));
                chart2.Series[2].Points.AddXY(i, Convert.ToString(CDFB[i]));
            }
            int[] countGray = GrayHistogram(image2Graylevel(image));
            getCDF(image2Graylevel(image), out CDFR, out CDFG, out CDFB);
            for (int i = 0; i < 256; i++)
            {
                chart3.Series[0].Points.AddXY(i, Convert.ToString(CDFR[i]));
                chart4.Series[0].Points.AddXY(i, Convert.ToString(countGray[i]));
            }
        }
        public void getCDF(Bitmap image, out int[] CDFR, out int[] CDFG, out int[] CDFB)
        {
            int[] countR, countG, countB;
            RGBHistogram(image, out countR, out countG, out countB);
            CDFR = new int[256];
            CDFG = new int[256];
            CDFB = new int[256];
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDFR[i] = CDFR[i - 1] + countR[i];
                    CDFG[i] = CDFG[i - 1] + countG[i];
                    CDFB[i] = CDFB[i - 1] + countB[i];
                }
                else
                {
                    CDFR[0] = CDFR[0];
                    CDFG[0] = CDFG[0];
                    CDFB[0] = CDFB[0];
                }
            }
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
        //==================getRGBhis=============================
        public void RGBHistogram(Bitmap image, out int[] countR, out int[] countG, out int[] countB )
        {
            countR = new int[256];
            countG = new int[256];
            countB = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color RGB = image.GetPixel(x, y);
                    countR[RGB.R]++;
                    countG[RGB.G]++;
                    countB[RGB.B]++;
                }
            }
        }
        //==================getGrayhis============================
        public int[] GrayHistogram(Bitmap image)
        {
            int[] countGray = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    countGray[gray.R]++;
                }
            }
            return countGray;
        }
        
        private void Histogram_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Equalization eq = new Equalization(image2Graylevel(image1));
            eq.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Specification s = new Specification(image2Graylevel(image1));
            s.Show();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

    }
}
