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
    public partial class Contrast : Form
    {
        Bitmap image;
        public Contrast(Bitmap Image)
        {
            InitializeComponent();
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;

            pictureBox1.Image = Image;
            GrayHistogram(Image);
            image = Image;
            textBox1.Text = "0 255";
            textBox2.Text = "100 200";
        }
        private void ContrastStretch(Bitmap image,double r1,double r2,double s1,double s2) 
        {
            int[] Contrasthistogram = new int[256];
            Bitmap ContrastImage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {

                    //contrast 處理
                    double y1 = 0;
                    byte value = image.GetPixel(x, y).R;
                    if (value < r1)
                    {
                        y1 = (value / r1) * s1;
                    }
                    else if (value <= r2)
                    {
                        y1 = ((value - r1) / (r2 - r1)) * (s2 - s1) + s1;
                    }
                    else
                    {
                        y1 = ((value - r1) / (255 - r2)) * (255 - s2) + s2;
                    }
                    byte s = (byte)Math.Round(y1);
                    Contrasthistogram[s]++;
                    Color newColorContrast = Color.FromArgb(s, s, s);
                    ContrastImage.SetPixel(x, y, newColorContrast);
                }
            }
            pictureBox2.Image = ContrastImage;
            chart2.Series[0].Points.Clear();
            for (int i = 0; i < 255; i++)
            {
                chart2.ChartAreas[0].AxisX.Minimum = 0;
                chart2.ChartAreas[0].AxisX.Maximum = 300;
                chart2.Series[0].Points.AddXY(i, Contrasthistogram[i]);
            }
        }
        private void Contrast_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        public void GrayHistogram(Bitmap image)
        {
            int[]  countgray = new int[256];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    countgray[gray.R]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = 300;
                chart1.Series[0].Points.AddXY(i, Convert.ToString(countgray[i]));
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string r = textBox1.Text;
            string s = textBox2.Text;
            double r1 = Convert.ToDouble( r.Split(' ')[0]);
            double r2 = Convert.ToDouble(r.Split(' ')[1]);
            double s1 = Convert.ToDouble(s.Split(' ')[0]);
            double s2 = Convert.ToDouble(s.Split(' ')[1]);
            ContrastStretch(image, r1, r2, s1, s2);
        }
    }
}
