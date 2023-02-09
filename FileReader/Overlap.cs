using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PCXDecoder;
using BMPDecoder;

namespace FileReader
{
    public partial class Overlap : Form
    {
        string filename;
        PReadHeader PCX_header = new PReadHeader();
        PDecodeFile PCX_decode = new PDecodeFile();
        BReadHeader BMP_header = new BReadHeader();
        BDecodeFile BMP_decode = new BDecodeFile();
        Bitmap image1;
        Bitmap image2;
        Bitmap image3;
        double width1,height1,width2, height2;

        public Overlap(Bitmap image)
        {
            InitializeComponent();
            pictureBox2.Image = image;
            
            image1 = image;
            
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        public Bitmap ImageResize(Bitmap image,double Wzoom,double Hzoom)
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
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double BarValue = trackBar1.Value;
            label6.Text = Convert.ToString(BarValue);
            
            image3 = new Bitmap((int)width1, (int)height1);
            for (int y = 0; y < height1; y++)
            {
                for (int x = 0; x < width1; x++)
                {
                    int Ra = (int)Math.Round((1 - (BarValue / 255.0)) * image1.GetPixel(x, y).R + (BarValue / 255.0) * image2.GetPixel(x, y).R);
                    int Ga = (int)Math.Round((1 - (BarValue / 255.0)) * image1.GetPixel(x, y).G + (BarValue / 255.0) * image2.GetPixel(x, y).G);
                    int Ba = (int)Math.Round((1 - (BarValue / 255.0)) * image1.GetPixel(x, y).B + (BarValue / 255.0) * image2.GetPixel(x, y).B);
                    Color RGB = Color.FromArgb(Ra, Ga, Ba);
                    image3.SetPixel(x, y, RGB);
                }
            }
            pictureBox3.Image = image3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            openFileDialog1.Filter = "Image Files (PCX,BMP)|*.PCX;*.BMP;";
            filename = openFileDialog1.FileName;
            byte[] imagefile = File.ReadAllBytes(filename);
            if (filename.Contains(".pcx"))
            {
                PCX_header.readheader(imagefile);
                PCX_decode.decoPixel(imagefile);
                pictureBox1.Image = PCX_decode.decode_image;
                image2 = PCX_decode.decode_image;
                richTextBox1.Text = "Open the file: " + filename;
            }
            else if (filename.Contains(".bmp"))
            {
                BMP_header.readheader(imagefile);
                BMP_decode.decoPixel(imagefile);
                BReadPalette BMP_palette = new BReadPalette();
                BMP_palette.readpalette(imagefile);
                pictureBox1.Image = BMP_decode.decode_image;
                image2 = BMP_decode.decode_image;
                richTextBox1.Text = "Open the file: " + filename;
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
            pictureBox3.Image = image1;
            trackBar1.Value=0;
        }
    }
}
