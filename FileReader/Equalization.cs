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
    public partial class Equalization : Form
    {
        
        public Equalization(Bitmap Image)
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
            int[] CDFGray, EqCDFGray;
            Bitmap Eimage;
            pictureBox1.Image = Image;
            GrayhistogramEqualization(Image, out Eimage, out CDFGray, out EqCDFGray);
            int[] countGray = GrayHistogram(Image);
            int[] EcountGray = GrayHistogram(Eimage);
            pictureBox2.Image = Eimage;
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, Convert.ToString(countGray[i]));
                chart2.Series[0].Points.AddXY(i, Convert.ToString(CDFGray[i]));
                chart3.Series[0].Points.AddXY(i, Convert.ToString(EcountGray[i]));
                chart4.Series[0].Points.AddXY(i, Convert.ToString(EqCDFGray[i]));
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
        //==================Equal===========================
        public void GrayhistogramEqualization(Bitmap image, out Bitmap Eimage, out int[] CDFGray, out int[] EqCDFGray)
        {
            int[] countGray = GrayHistogram(image);
            byte[] EqualGray = new byte[256];
            CDFGray = new int[256];
            EqCDFGray = new int[256];
            Eimage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                { CDFGray[i] = CDFGray[i - 1] + countGray[i]; }
                else
                { CDFGray[0] = CDFGray[0]; }

                EqualGray[i] = (byte)(255 * CDFGray[i] / (image.Width * image.Height) + 0.5);
            }
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color gray = image.GetPixel(x, y);
                    Eimage.SetPixel(x, y, Color.FromArgb(EqualGray[gray.R], EqualGray[gray.R], EqualGray[gray.R]));
                }
            }
            int[] EcountGray = GrayHistogram(Eimage);
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    EqCDFGray[i] = EqCDFGray[i - 1] + EcountGray[i];
                }
                else
                {
                    EqCDFGray[0] = EqCDFGray[0];
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
