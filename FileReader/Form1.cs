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
using System.Threading;
namespace FileReader
{
    public partial class Form1 : Form
    {
        //=============================SplashScreen=================================
        public void SplashStart()
        {
            Application.Run(new SplashScreen());
        }
        public Form1()
        {
            InitializeComponent();
            SplashScreen SS = new SplashScreen();
            Thread t = new Thread(new ThreadStart(SplashStart));
            t.Start();
            Thread.Sleep(3000);
            //InitializeComponent();
            t.Abort();
        }
        //=================================開檔=====================================
        string filename;
        PReadHeader PCX_header = new PReadHeader();
        PDecodeFile PCX_decode = new PDecodeFile();
        PReadPalette Palette = new PReadPalette();
        BReadHeader BMP_header = new BReadHeader();
        BDecodeFile BMP_decode = new BDecodeFile();
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
                Palette.readpalette(imagefile);
                if (PCX_header.NPlanes == 1) { PaletteView(Palette.getPalette()); }
                else 
                { 
                    dataGridView1.Rows.Clear();
                    dataGridView1.Columns.Clear();
                }
                pictureBox1.Image = PCX_decode.decode_image;

                richTextBox1.Text = "Open the file: " + filename;
                if (PCX_header.Manfacturer == 10)
                {
                    richTextBox2.Text = "Manfacturer: " + PCX_header.Manfacturer + "\n" +
                                   "Version: " + PCX_header.Version + "\n" +
                                   "Encoding: " + PCX_header.Encoding + "\n" +
                                   "BitPerPixel: " + PCX_header.BitPerPixel + "\n" +
                                   "Xmin: " + PCX_header.Xmin + "\n" +
                                   "Xmax: " + PCX_header.Xmax + "\n" +
                                   "Ymin: " + PCX_header.Ymin + "\n" +
                                   "Ymax: " + PCX_header.Ymax + "\n" +
                                   "Hdpi: " + PCX_header.Hdpi + "\n" +
                                   "Vdpi: " + PCX_header.Vdpi + "\n" +
                                   "Reserved: " + PCX_header.Reserved + "\n" +
                                   "NPlanes: " + PCX_header.NPlanes + "\n" +
                                   "BytesPerLine: " + PCX_header.BytesPerLine + "\n" +
                                   "PaletteInfo: " + PCX_header.PaletteInfo + "\n" +
                                   "HscreenSize: " + PCX_header.HscreenSize + "\n" +
                                   "VscreenSize: " + PCX_header.VscreenSize + "\n" +
                                   "width=" + PCX_header.Width + "\n" +
                                   "Height=" + PCX_header.Height + "\n";
                }
                else { richTextBox2.Text = "The file is not .pcx file\n"; }
            }
            else if (filename.Contains(".bmp"))
            {

                BMP_header.readheader(imagefile);
                BMP_decode.decoPixel(imagefile);

                BReadPalette BMP_palette = new BReadPalette();
                BMP_palette.readpalette(imagefile);
                pictureBox1.Image = BMP_decode.decode_image;

                richTextBox1.Text = "Open the file: " + filename;
                if (BMP_header.FileType[0] == 'B' && BMP_header.FileType[1] == 'M')
                {
                    richTextBox2.Text = "FileType: " + Convert.ToString((char)BMP_header.FileType[0]) + Convert.ToString((char)BMP_header.FileType[1]) + "\n" +
                                   "FileSize: " + BMP_header.FileSize + "\n" +
                                   "Offset: " + BMP_header.Offset + "\n" +
                                   "InfoHeaderSize: " + BMP_header.InfoHeaderSize + "\n" +
                                   "Width: " + BMP_header.BitWidth + "\n" +
                                   "Height: " + BMP_header.BitHeight + "\n" +
                                   "NPlanes: " + BMP_header.NPlanes + "\n" +
                                   "BitsPerPixel: " + BMP_header.BitsPerPixel + "\n" +
                                   "Compression: " + BMP_header.Compression + "\n" +
                                   "ImageSize: " + BMP_header.ImageSize + "\n" +
                                   "Hresolution: " + BMP_header.Hdpi + "\n" +
                                   "Vresolution: " + BMP_header.Vdpi + "\n" +
                                   "NColors: " + BMP_header.NColors + "\n" +
                                   "ImportantColors: " + BMP_header.ImportantColors + "\n";
                }
                else { richTextBox2.Text = "The file is not .bmp file\n"; }

            }
        }
        //=================================SNR======================================
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
        //=================================縮放=====================================
        private void button2_Click(object sender, EventArgs e)
        {
            double Wzoom = double.Parse(textBox2.Text);
            double Hzoom = double.Parse(textBox1.Text);
            if (filename.Contains(".pcx"))
            {
                double Width = Math.Round(Wzoom * (double)PCX_decode.decode_image.Width);
                double Height = Math.Round(Hzoom * (double)PCX_decode.decode_image.Height);
                Bitmap zoomimage = new Bitmap((int)Width, (int)Height);
                if (Wzoom == 1 && Hzoom == 1 && Wzoom != 0 && Hzoom != 0)
                { pictureBox5.Image = PCX_decode.decode_image; }
                else
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int i = (int)Math.Round(x * (1 / Wzoom));
                            int j = (int)Math.Round(y * (1 / Hzoom));
                            if (i > (PCX_decode.decode_image.Width - 1))
                            {
                                i = PCX_decode.decode_image.Width - 1;
                            }
                            if (j > (PCX_decode.decode_image.Height - 1))
                            {
                                j = PCX_decode.decode_image.Height - 1;
                            }
                            zoomimage.SetPixel(x, y, PCX_decode.decode_image.GetPixel(i, j));
                        }
                    }
                    pictureBox5.Image = zoomimage;
                    
                }
            }
            else if (filename.Contains(".bmp"))
            {
                double Width = Math.Round(Wzoom * (double)BMP_decode.decode_image.Width);
                double Height = Math.Round(Hzoom * (double)BMP_decode.decode_image.Height);
                Bitmap zoomimage = new Bitmap((int)Width, (int)Height);
                if (Wzoom == 1 && Hzoom == 1 && Wzoom != 0 && Hzoom != 0)
                { pictureBox5.Image = BMP_decode.decode_image; }
                else
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            int i = (int)Math.Round(x * (1 / Wzoom));
                            int j = (int)Math.Round(y * (1 / Hzoom));
                            if (i > (BMP_decode.decode_image.Width - 1))
                            {
                                i = BMP_decode.decode_image.Width - 1;
                            }
                            if (j > (BMP_decode.decode_image.Height - 1))
                            {
                                j = BMP_decode.decode_image.Height - 1;
                            }
                            zoomimage.SetPixel(x, y, BMP_decode.decode_image.GetPixel(i, j));
                        }
                    }
                    pictureBox5.Image = zoomimage;
                    
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            double Wzoom = double.Parse(textBox2.Text);
            double Hzoom = double.Parse(textBox1.Text);
            int width, height, Width, Height;
            Bitmap image, zoomimage;
            if (filename.Contains(".pcx"))
            {
                width = PCX_decode.decode_image.Width;
                height = PCX_decode.decode_image.Height;
                Width = (int)Math.Round(Wzoom * width);
                Height = (int)Math.Round(Hzoom * height);
                zoomimage = new Bitmap((int)Width, (int)Height);
                image = PCX_decode.decode_image;
            }
            else
            {
                width = BMP_decode.decode_image.Width;
                height = BMP_decode.decode_image.Height;
                image = BMP_decode.decode_image;
                Width = (int)Math.Round(Wzoom * width);
                Height = (int)Math.Round(Hzoom * height);
                zoomimage = new Bitmap((int)Width, (int)Height);
            }
            if (Wzoom == 1 && Hzoom == 1 && Wzoom != 0 && Hzoom != 0)
                { pictureBox5.Image = image; }
            else
            {
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        double x = j * (1 / Wzoom);
                        double y = i * (1 / Hzoom);
                        if (x <= (width - 1) & y <= (height - 1))
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
                                RGB = image.GetPixel((int)Math.Round(x), (int)Math.Round(y));
                            }
                            zoomimage.SetPixel(j, i, RGB);
                        }
                        else
                        {
                            int a = (int)Math.Round(x);
                            int b = (int)Math.Round(y);
                            if (a > (width - 1))
                            {
                                a = width - 1;
                            }
                            if (b > (height - 1))
                            {
                                b = height - 1;
                            }
                            zoomimage.SetPixel(j, i, image.GetPixel(a, b));
                        }
                    }
                }
                pictureBox5.Image = zoomimage;
                
            }
            
        }
        //==================================HSI=====================================
        public Bitmap Rimage;
        public Bitmap Gimage;
        public Bitmap Bimage;
        public Bitmap Himage;
        public Bitmap Simage;
        public Bitmap Iimage;
        public double[][] RGB2HSI(Bitmap image)
        {
            int index = 0;
            int Height = image.Height;
            int Width = image.Width;
            double[][] hsi = new double[Width * Height][];

            double R, G, B;
           
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Width; y++)
                {
                    R = image.GetPixel(y, x).R;
                    G = image.GetPixel(y, x).G;
                    B = image.GetPixel(y, x).B;
                    hsi[index] = new double[3];
   
                    hsi[index][0] = (0.299*R+0.587*G+0.114*B);
                    hsi[index][1] = (0.211 * R - 0.523 * G + 0.312 * B);
                    hsi[index][2] = (0.596 * R - 0.274 * G - 0.322 * B);
                    index++;
                }
            }
            return hsi;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            double[][] hsi;
            int Height, Width, index;
            if (filename.Contains(".pcx"))
            {
                Width = PCX_decode.decode_image.Width;
                Height = PCX_decode.decode_image.Height;
            }
            else
            {
                Width = BMP_decode.decode_image.Width;
                Height = BMP_decode.decode_image.Height;
            }
            Himage = new Bitmap(Width, Height);
            Simage = new Bitmap(Width, Height);
            Iimage = new Bitmap(Width, Height);
            Color[] Hpixel = new Color[Width * Height];
            Color[] Spixel = new Color[Width * Height];
            Color[] Ipixel = new Color[Width * Height];
            if (filename.Contains(".pcx"))
            {
                hsi = RGB2HSI(PCX_decode.decode_image);
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        Color h = Color.FromArgb((int)hsi[index][0], (int)hsi[index][0], (int)hsi[index][0]);
                        Color s = Color.FromArgb((int)hsi[index][1], (int)hsi[index][1], (int)hsi[index][1]);
                        Color i = Color.FromArgb((int)hsi[index][2], (int)hsi[index][2], (int)hsi[index][2]);
                        Hpixel.SetValue(h, index);
                        Spixel.SetValue(s, index);
                        Ipixel.SetValue(i, index);
                        index++;
                    }
                }
                index = 0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Himage.SetPixel(j, i, Hpixel[index]);
                        Simage.SetPixel(j, i, Spixel[index]);
                        Iimage.SetPixel(j, i, Ipixel[index]);
                        index++;
                    }
                }
            }
            else if (filename.Contains(".bmp"))
            {
                hsi = RGB2HSI(BMP_decode.decode_image);
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        Color h = Color.FromArgb((byte)hsi[index][0], (byte)hsi[index][0], (byte)hsi[index][0]);
                        Color s = Color.FromArgb((byte)hsi[index][1], (byte)hsi[index][1], (byte)hsi[index][1]);
                        Color i = Color.FromArgb((byte)hsi[index][2], (byte)hsi[index][2], (byte)hsi[index][2]);
                        Hpixel.SetValue(h, index);
                        Spixel.SetValue(s, index);
                        Ipixel.SetValue(i, index);
                        index++;
                    }
                }
                index = 0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Himage.SetPixel(j, i, Hpixel[index]);
                        Simage.SetPixel(j, i, Spixel[index]);
                        Iimage.SetPixel(j, i, Ipixel[index]);
                        index++;
                    }
                }
            }
            show_form HSI_form = new show_form();
            HSI_form.SetPicture(Himage, Simage, Iimage);
            HSI_form.Show();
        }
        //======================RGB=================================================
        private void button3_Click(object sender, EventArgs e)
        {
            int Height, Width;
            if (filename.Contains(".pcx"))
            {
                Width = PCX_decode.decode_image.Width;
                Height = PCX_decode.decode_image.Height;
            }
            else
            {
                Width = BMP_decode.decode_image.Width;
                Height = BMP_decode.decode_image.Height;
            }
            Rimage = new Bitmap(Width, Height);
            Gimage = new Bitmap(Width, Height);
            Bimage = new Bitmap(Width, Height);
            Color[] Rpixel = new Color[Width * Height];
            Color[] Gpixel = new Color[Width * Height];
            Color[] Bpixel = new Color[Width * Height];
            Color R, G, B;
            int index;
            if (filename.Contains(".pcx"))
            {
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        R = Color.FromArgb(PCX_decode.decode_image.GetPixel(y, x).R, 0, 0);
                        G = Color.FromArgb(0, PCX_decode.decode_image.GetPixel(y, x).G, 0);
                        B = Color.FromArgb(0, 0, PCX_decode.decode_image.GetPixel(y, x).B);
                        Rpixel.SetValue(R, index);
                        Gpixel.SetValue(G, index);
                        Bpixel.SetValue(B, index);
                        index++;
                    }
                }
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        Rimage.SetPixel(y, x, Rpixel[index]);
                        Gimage.SetPixel(y, x, Gpixel[index]);
                        Bimage.SetPixel(y, x, Bpixel[index]);
                        index++;
                    }
                }
                show_form RGB_form = new show_form();
                RGB_form.SetPicture(Rimage, Gimage, Bimage);
                RGB_form.Show();
            }
            else if (filename.Contains(".bmp"))
            {
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        R = Color.FromArgb(BMP_decode.decode_image.GetPixel(y, x).R, 0, 0);
                        G = Color.FromArgb(0, BMP_decode.decode_image.GetPixel(y, x).G, 0);
                        B = Color.FromArgb(0, 0, BMP_decode.decode_image.GetPixel(y, x).B);
                        Rpixel.SetValue(R, index);
                        Gpixel.SetValue(G, index);
                        Bpixel.SetValue(B, index);
                        index++;
                    }
                }
                index = 0;
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Width; y++)
                    {
                        Rimage.SetPixel(y, x, Rpixel[index]);
                        Gimage.SetPixel(y, x, Gpixel[index]);
                        Bimage.SetPixel(y, x, Bpixel[index]);
                        index++;
                    }
                }
                show_form RGB_form = new show_form();
                RGB_form.SetPicture(Rimage, Gimage, Bimage);
                RGB_form.Show();
            }
        }
        //=============================旋轉=========================================
        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")){ image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Rotate rotate = new Rotate(image);
            rotate.Show();
        }
        //=============================Cut==========================================
        private void button15_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Cut c = new Cut(image);
            c.Show();
        }
        //=============================灰階=========================================
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
        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            pictureBox5.Image =image2Graylevel(image);
        }
        //==============================疊圖========================================
        private void button8_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Overlap overlap = new Overlap(image);
            overlap.Show();

        }
        //========================滑鼠抓座標========================================
        private void pictureBox1_Move(object sender, EventArgs e)
        {
            
        }
        private void mouse_click(object sender, MouseEventArgs e) 
        {
            Bitmap image;
            double[][] HSI;
            int x;
            int y;
            if (pictureBox1.Image != null)
            {
                if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
                else { image = BMP_decode.decode_image; }
                x = e.X;
                y = e.Y;
                HSI = RGB2HSI(image);
                Color RGB = image.GetPixel(x, y);
                int index = 0;
                string[] hsi = new string[3];
                for (int i = 0; i < y; i++)
                {
                    for (int j = 0; j < x; j++)
                    {
                        index++;
                    }
                }
                
                hsi[0] = Convert.ToString(Convert.ToInt32(HSI[index][0]));
                hsi[1] = Convert.ToString(Convert.ToInt32(HSI[index][1]));
                hsi[2] = Convert.ToString(Convert.ToInt32(HSI[index][2]));
                richTextBox4.Text = "X:" + Convert.ToString(x) + " ,Y:" + Convert.ToString(y) + "\n" +
                                    "R:" + Convert.ToString(RGB.R) + " ,G:" + Convert.ToString(RGB.G) + " ,B:" + Convert.ToString(RGB.B) + "\n" +
                                    "H:" + hsi[0]  + " ,S:" + hsi[1] + " ,I:" + hsi[2];
            }
        }
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Bitmap image;
            double[][] HSI;
            int x = 0;
            int y = 0;
            if (pictureBox1.Image != null)
            {
                if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
                else { image = BMP_decode.decode_image; }
                x = e.X;
                y = e.Y;
                if (x >= 0 && x < image.Width && y >= 0 && y < image.Height)
                {
                    HSI = RGB2HSI(image);

                    Color RGB = image.GetPixel(x, y);
                    int index = 0;
                    string[] hsi = new string[3];
                    for (int i = 0; i < y; i++)
                    {
                        for (int j = 0; j < x; j++)
                        {
                            index++;
                        }
                    }
                    hsi[0] = Convert.ToString(Convert.ToInt32(HSI[index][0]));
                    hsi[1] = Convert.ToString(Convert.ToInt32(HSI[index][1]));
                    hsi[2] = Convert.ToString(Convert.ToInt32(HSI[index][2]));
                    richTextBox3.Text = "X: " + Convert.ToString(x) + " ,Y:" + Convert.ToString(y) + "\n" +
                                        "R:" + Convert.ToString(RGB.R) + " ,G:" + Convert.ToString(RGB.G) + " ,B:" + Convert.ToString(RGB.B) + "\n" +
                                        "H:" + hsi[0] + " ,S:" + hsi[1] + " ,I:" + hsi[2];

                }
            }
            
        }
        //========================histogram=========================================
        private void Histogram_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Histogram histogram = new Histogram(image);
            histogram.Show();

        }
        //==========================負片============================================
        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Negative n = new Negative(image);
            n.Show();
        }
        //=======================Thresholding=======================================
        private void button10_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Binarization t = new Binarization(image2Graylevel(image));
            t.Show();

        }
        //==========================Slicing=========================================
        private void button11_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Slicing s = new Slicing(image2Graylevel(image));
            s.Show();
        }
        //==========================對比============================================
        private void button12_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }

            Contrast c = new Contrast(image2Graylevel(image));
            c.Show();
        }
        //=========================調色盤===========================================
        public void PaletteView(Color[] palette)
        {
           
            dataGridView1.ColumnCount = 16;
            dataGridView1.RowCount = 16;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;
            int color_index = 0;
            for (int i = 0; i< 16; i++)
            {
                dataGridView1.Rows[i].Height = 12;
                dataGridView1.Columns[i].Width = 12;
                for (int j = 0; j< 16; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = palette[color_index];
                    color_index++;
                }
            }
        }
        //==========================明亮度==========================================
        private void button13_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Brightness b = new Brightness(image);
            b.Show();
        }
        //=========================Filter===========================================
        private void button14_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Filter f = new Filter(image);
            f.Show();
        }
        //=========================Connected component========================
        private void button16_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Connected cc = new Connected(image2Graylevel(image));
            cc.Show();
        }
        //=========================ball========================
        private void button17_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            BouncingBalls b = new BouncingBalls(image);
            b.Show();
        }
        //==========================================================================
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {


        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        //================================Mpeg============================
        private void button18_Click(object sender, EventArgs e)
        {
            Mpeg m = new Mpeg();
            m.Show();
        }
        //=======================Huffman============================
        private void button19_Click(object sender, EventArgs e)
        {
            Bitmap image;
            if (filename.Contains(".pcx")) { image = PCX_decode.decode_image; }
            else { image = BMP_decode.decode_image; }
            Huffman h = new Huffman(image2Graylevel(image));
            h.Show();
        }
    }

}




