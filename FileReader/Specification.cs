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

    public partial class Specification : Form
    {
        #region 宣告
        PReadHeader PCX_header = new PReadHeader();
        PDecodeFile PCX_decode = new PDecodeFile();
        BReadHeader BMP_header = new BReadHeader();
        BDecodeFile BMP_decode = new BDecodeFile();
        Bitmap image1;
        Bitmap image2;

        double width1, height1, width2, height2;
        #endregion

        public Bitmap ImageResize(Bitmap image, double Wzoom, double Hzoom)
        {
            double Width = Math.Round(Wzoom * (double)image.Width);
            double Height = Math.Round(Hzoom * (double)image.Height);
            Bitmap zoomimage = new Bitmap((int)Width, (int)Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = (int)Math.Round(x * (1 / Wzoom));
                    int j = (int)Math.Round(y * (1 / Hzoom));
                    if (i > (image.Width - 1))
                    {
                        i = image.Width - 1;
                    }
                    if (j > (image.Height - 1))
                    {
                        j = image.Height - 1;
                    }
                    zoomimage.SetPixel(x, y, image.GetPixel(i, j));
                }
            }
            return zoomimage;
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
        public Specification(Bitmap Image)
        {
            InitializeComponent();
            #region chart設定
            chart5.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart5.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart5.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart6.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart6.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart6.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart8.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart8.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart8.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart9.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart9.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart9.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart10.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart10.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart10.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            chart7.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            chart7.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            chart7.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
            #endregion
            image1 = Image;
            pictureBox1.Image = Image;
            int[] CDFGray = getCDF(image1);
            int[] countGray = Histogram(Image);
            for (int i = 0; i < 256; i++)
            {
                chart8.Series[0].Points.AddXY(i, Convert.ToString(countGray[i]));
                chart9.Series[0].Points.AddXY(i, Convert.ToString(CDFGray[i]));
            }
        }
        public int[] getCDF(Bitmap image)
        {
            int[] count,CDF;
            count = Histogram(image);
            CDF = new int[256];
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDF[i] = CDF[i - 1] + count[i];
                }
                else
                {
                    CDF[0] = CDF[0];
                }
            }
            return CDF;
        }
        public int[] Histogram(Bitmap image)
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
        public Bitmap getSpecification(Bitmap image1, Bitmap image2)
        {
            byte[] SpecGray = new byte[256];
            int[] SpCDFGray = new int[256];
            Bitmap  Spimage = new Bitmap(image1.Width, image1.Height);
            int[] his1 = Histogram(image1);
            int[] his2 = Histogram(image2);
            double[] CDF1 = new double[256];
            double[] CDF2 = new double[256];
            double[] Gray1 = new double[256];
            double[] Gray2 = new double[256];
            double total = (image1.Width * image1.Height);
            //=========image1 & image2 累計機率=================
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    CDF1[i] = CDF1[i - 1] + his1[i];
                    CDF2[i] = CDF2[i - 1] + his2[i];
                }
                else
                {
                    CDF1[0] = CDF1[0];
                    CDF2[0] = CDF2[0];
                }
                Gray1[i] = CDF1[i] /  total;
                Gray2[i] =  CDF2[i] /  total;
            }
            double diff1, diff2;
            byte kG = 0;
            for (int i = 0; i < 256; i++)
            {
                diff2 = 1;
                for (int j = kG; j < 256; j++)
                {
                    //找到Gray1 Gray2中最相似的位置  
                    diff1 = Math.Abs(Gray1[i] - Gray2[j]);
                    if ((diff1 - diff2) < 1.0E-08)
                    {
                        diff2 = diff1;
                        kG = (byte)j;
                    }
                    else
                    {
                        kG = (byte)Math.Abs(j - 1);
                        break;
                    }
                }
                if (kG == 255)
                {
                    for (int l = i; l < 256; l++)
                    {
                        SpecGray[l] = kG;
                    }
                    break;
                }
                SpecGray[i] = kG;
            }
            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    Color gray = image1.GetPixel(x, y);
                    Spimage.SetPixel(x, y, Color.FromArgb((int)SpecGray[gray.R], (int)SpecGray[gray.R], (int)SpecGray[gray.R]));
                }
            }
            int[] ScountGray = Histogram(Spimage);
            for (int i = 0; i < 256; i++)
            {
                if (i != 0)
                {
                    SpCDFGray[i] = SpCDFGray[i - 1] + ScountGray[i];
                }
                else
                {
                    SpCDFGray[0] = SpCDFGray[0];
                }
            }
            int[] spCDF = getCDF(Spimage);
            chart5.Series[0].Points.Clear();
            chart6.Series[0].Points.Clear();
            for (int i = 0; i < 256; i++)
            {
                chart5.Series[0].Points.AddXY(i, Convert.ToString(ScountGray[i]));
                chart6.Series[0].Points.AddXY(i, Convert.ToString(spCDF[i])); 
            }
            return Spimage;
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            chart10.Series[0].Points.Clear();
            chart7.Series[0].Points.Clear();
            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "Image Files (PCX,BMP)|*.PCX;*.BMP;";
            string filename = openFileDialog1.FileName;
            byte[] imagefile = File.ReadAllBytes(filename);
            if (filename.Contains(".pcx"))
            {
                PCX_header.readheader(imagefile);
                PCX_decode.decoPixel(imagefile);
                image2 = PCX_decode.decode_image;
            }
            else if (filename.Contains(".bmp"))
            {
                BMP_header.readheader(imagefile);
                BMP_decode.decoPixel(imagefile);
                BReadPalette BMP_palette = new BReadPalette();
                BMP_palette.readpalette(imagefile);
                image2 = BMP_decode.decode_image;
            }
            width2 = image2.Width;
            height2 = image2.Height;
            width1 = image1.Width;
            height1 = image1.Height;
            double Wzoom, Hzoom;
            if (width1 != width2) { Wzoom = width1 / width2; }
            else { Wzoom = 1.0; }
            if (height1 != height2) { Hzoom = height1 / height2; }
            else { Hzoom = 1.0; }
            image2 = ImageResize(image2, Wzoom, Hzoom);
            pictureBox2.Image = image2Graylevel(image2);
            int[] CDFGray = getCDF(image2Graylevel(image2));
            int[] countGray = Histogram(image2Graylevel(image2));
            for (int i = 0; i < 256; i++)
            {
                chart10.Series[0].Points.AddXY(i, Convert.ToString(countGray[i]));
                chart7.Series[0].Points.AddXY(i, Convert.ToString(CDFGray[i]));
            }


            
            Bitmap Spimage=getSpecification(image2Graylevel(image1), image2Graylevel(image2));
            pictureBox3.Image = Spimage;
        }
    }
}
