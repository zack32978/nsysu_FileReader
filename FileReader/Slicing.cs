using System;
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
using System.IO;
namespace FileReader
{

    public partial class Slicing : Form
    {
        string filename;
        PReadHeader PCX_header = new PReadHeader();
        PDecodeFile PCX_decode = new PDecodeFile();
        BReadHeader BMP_header = new BReadHeader();
        BDecodeFile BMP_decode = new BDecodeFile();
        Bitmap image1;
        Bitmap image2;
        Bitmap image3;
        double width1, height1, width2, height2;
        public Slicing(Bitmap image)
        {
            InitializeComponent();
            Bitplane(image);
            Graycode(image);
            image1 = image;
        }
        
        public void Graycode(Bitmap image) 
        {
            Bitmap[] graybitmaps = new Bitmap[8];
            for (int i = 0; i < 8; i++)
            {
                graybitmaps[i] = new Bitmap(image.Width, image.Height);
            }
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color c = image.GetPixel(x, y);
                    int[] planes = new int[8];
                    int[] Gray = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        planes[k] = 0;
                        Gray[k] = 0;
                    }
                    //0 bit plane 
                    planes[0] = c.R & 0x1;// 00000001
                    if (planes[0] != 0)//第一位元有東西
                    {
                        planes[0] = 1;
                    }
                    //1 bit plane 
                    planes[1] = c.R & 0x2;// 00000010
                    if (planes[1] != 0)
                    {
                        planes[1] = 1;
                    }
                    //2 bit plane 
                    planes[2] = c.R & 0x4;// 00000100
                    if (planes[2] != 0)
                    {
                        planes[2] = 1;
                    }
                    //3 bit plane
                    planes[3] = c.R & 0x8;// 00001000
                    if (planes[3] != 0)
                    {
                        planes[3] = 1;
                    }
                    //4 bit plane
                    planes[4] = c.R & 0x10;// 00010000
                    if (planes[4] != 0)
                    {
                        planes[4] = 1;
                    }
                    //5 bit plane
                    planes[5] = c.R & 0x20;// 00100000
                    if (planes[5] != 0)
                    {
                        planes[5] = 1;
                    }
                    //6 bit plane
                    planes[6] = c.R & 0x40;// 01000000
                    if (planes[6] != 0)
                    {
                        planes[6] = 1;
                    }
                    //7 bit plane
                    planes[7] = c.R & 0x80;// 10000000
                    if (planes[7] != 0)
                    {
                        planes[7] = 1;
                    }
                    //gray code 避免進位 每次只有一個bit變化
                    Gray[7] = planes[7];
                    if (Gray[7] != 0)
                    {
                        Gray[7] = 255;
                    }
                    Gray[6] = planes[7]^planes[6];
                    if (Gray[6] != 0)
                    {
                        Gray[6] = 255;
                    }
                    Gray[5] = planes[6] ^ planes[5];
                    if (Gray[5] != 0)
                    {
                        Gray[5] = 255;
                    }
                    Gray[4] = planes[5] ^ planes[4];
                    if (Gray[4] != 0)
                    {
                        Gray[4] = 255;
                    }
                    Gray[3] = planes[4] ^ planes[3];
                    if (Gray[3] != 0)
                    {
                        Gray[3] = 255;
                    }
                    Gray[2] = planes[3] ^ planes[2];
                    if (Gray[2] != 0)
                    {
                        Gray[2] = 255;
                    }
                    Gray[1] = planes[2] ^ planes[1];
                    if (Gray[1] != 0)
                    {
                        Gray[1] = 255;
                    }
                    Gray[0] = planes[1] ^ planes[0];
                    if (Gray[0] != 0)
                    {
                        Gray[0] = 255;
                    }
                    for (int k = 0; k < 8; k++)
                    {
                        graybitmaps[k].SetPixel(x, y, Color.FromArgb(Gray[k], Gray[k], Gray[k]));
                    }
                }
            }
            pictureBox9.Image = graybitmaps[0];
            pictureBox10.Image = graybitmaps[1];
            pictureBox11.Image = graybitmaps[2];
            pictureBox12.Image = graybitmaps[3];
            pictureBox13.Image = graybitmaps[4];
            pictureBox14.Image = graybitmaps[5];
            pictureBox15.Image = graybitmaps[6];
            pictureBox16.Image = graybitmaps[7];
        }
        public void Bitplane(Bitmap image) 
        {
            pictureBox17.Image = image;//灰階圖
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
            pictureBox1.Image = bitmaps[0];
            pictureBox2.Image = bitmaps[1];
            pictureBox3.Image = bitmaps[2];
            pictureBox4.Image = bitmaps[3];
            pictureBox5.Image = bitmaps[4];
            pictureBox6.Image = bitmaps[5];
            pictureBox7.Image = bitmaps[6];
            pictureBox8.Image = bitmaps[7];


        }
    
        private void Slicing_Load(object sender, EventArgs e)
        {

        }
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

        public double SNR(Bitmap imagemg, Bitmap noiseimage)
        {
            int r1, r2, g1, g2, b1, b2;
            double sigma = 0, square = 0;

            for (int y = 0; y < imagemg.Height; y++)
            {
                for (int x = 0; x < imagemg.Width; x++)
                {
                    r1 = (int)imagemg.GetPixel(x, y).R;
                    g1 = (int)imagemg.GetPixel(x, y).G;
                    b1 = (int)imagemg.GetPixel(x, y).B;
                    r2 = (int)noiseimage.GetPixel(x, y).R;
                    g2 = (int)noiseimage.GetPixel(x, y).R;
                    b2 = (int)noiseimage.GetPixel(x, y).R;
                    sigma += (b1 - b2) * (b1 - b2) + (g1 - g2) * (g1 - g2) + (r1 - r2) * (r1 - r2);
                    square += b1 * b1 + g1 * g1 + r1 * r1;
                }
            }
            return 10 * Math.Log10(square / sigma);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
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
            image3 = watermark(image1, image2Graylevel(image2), Convert.ToInt32(textBox1.Text));
            double snr = SNR(image1,image3);
            Watermark w = new Watermark(image3,snr);
            w.Show();
        }
        public Bitmap watermark(Bitmap image1,Bitmap image2,int plane)
        {
            Bitmap image3 = new Bitmap(image1.Width, image1.Height);
            for (int y = 0; y < image1.Height; y++)
            {
                for (int x = 0; x < image1.Width; x++)
                {
                    int img1bitPlane,img2bitplane;
                    Color c1 = image1.GetPixel(x, y);
                    Color c2= image2.GetPixel(x, y);
                    int[] planes = new int[8];
                    for (int k = 0; k < 8; k++)
                    {
                        planes[k] = c1.R;
                    }
                    if (plane == 0) 
                    {
                        img1bitPlane = c1.R & 0x01;
                        img2bitplane = c2.R & 0x01;
                        planes[0] = planes[0] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 1) 
                    {
                        img1bitPlane = c1.R & 0x02;
                        img2bitplane = c2.R & 0x02;
                        planes[1] = planes[1] -img1bitPlane+ img2bitplane;

                    }
                    else if (plane == 2) 
                    {
                        img1bitPlane = c1.R & 0x04;
                        img2bitplane = c2.R & 0x04;
                        planes[2] = planes[2] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 3) 
                    {
                        img1bitPlane = c1.R & 0x08;
                        img2bitplane = c2.R & 0x08;
                        planes[3] = planes[3] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 4) 
                    {
                        img1bitPlane = c1.R & 0x10;
                        img2bitplane = c2.R & 0x10;
                        planes[4] = planes[4] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 5) 
                    { 
                        img1bitPlane = c1.R & 0x20;
                        img2bitplane = c2.R & 0x20;
                        planes[5] = planes[5] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 6) 
                    { 
                        img1bitPlane = c1.R & 0x40;
                        img2bitplane = c2.R & 0x40;
                        planes[6] = planes[6] - img1bitPlane + img2bitplane;
                    }
                    else if (plane == 7)
                    { 
                        img1bitPlane = c1.R & 0x80;
                        img2bitplane = c2.R & 0x80;
                        planes[7] = planes[7] - img1bitPlane + img2bitplane;
                    }
                    
                    image3.SetPixel(x, y, Color.FromArgb(planes[plane], planes[plane], planes[plane]));
                    
                }
            }
            return image3;
        }
    }
}
