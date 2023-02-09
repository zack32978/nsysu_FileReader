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
    public partial class Filter : Form
    {
        Bitmap image;
        public Filter(Bitmap Image)
        {
            InitializeComponent();
            pictureBox1.Image = Image;
            image = Image;
        }
        //========================SNR===============================================
        public double SNR(Bitmap imagemg, Bitmap noiseimage)
        {
            int r1,r2,g1,g2,b1,b2;
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
                    sigma += (b1 - b2) * (b1 - b2) + (g1 - g2) * (g1 - g2) +(r1 - r2) * (r1 - r2);
                    square += b1 * b1 + g1 * g1 + r1 * r1;
                }
            }
            return 10 * Math.Log10(square / sigma);
        }
        //=========================Gray=====================================
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
        //=========================Filter===================================
        public Bitmap Outlier(Bitmap image,int size)//取nxn的遮罩，取中間值外的平均，若均值與中間pixel值差大於閥值則取代pixel
        {
            pictureBox1.Image = image;
            Bitmap Outlierimage = new Bitmap(image.Width, image.Height);
            //Color[] mask = new Color[9];
            if (size != 0)
            {
                Color[] mask = new Color[size * size];
                int tmp = size / 2;
                int index = 0;
                Color block = new Color();
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        #region 取nxn的pixel值
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= image.Width || i < 0 || j >= image.Height || j < 0) { mask[index++] = block; }
                                    else { mask[index++] = image.GetPixel(i, j); }
                                }
                            }
                        }
                        #endregion
                        //中間pixel
                        int pointr = mask[size * size / 2].R;
                        int pointg = mask[size * size / 2].G;
                        int pointb = mask[size * size / 2].B;
                        //取除了中間值外平均
                        int dr = 0, dg = 0, db = 0;
                        for(int i =0;i<mask.Length;i++)
                        {
                            dr += mask[i].R;
                            dg += mask[i].G;
                            db += mask[i].B;
                        }
                        dr = (dr - pointr) / (mask.Length - 1);
                        dg = (dg - pointg) / (mask.Length - 1);
                        db = (db - pointb) / (mask.Length - 1);
                        //判斷閥值，若|中間-平均|>閥值則取代
                        if (Math.Abs(pointr - dr) > 50)
                        {
                            pointr = dr;
                        }
                        if (Math.Abs(pointg - dg) > 50)
                        {
                            pointg = dg;
                        }
                        if (Math.Abs(pointb - db) > 50)
                        {
                            pointb = db;
                        }
                        Outlierimage.SetPixel(x, y, Color.FromArgb(pointr, pointg, pointb));
                    }
                }
            }         
            return Outlierimage;
        }
        #region Median: sort mask 取最中间那一个pixel值用中值替代
        public Bitmap SquareMedian(Bitmap image,int size)//方形
        {
            Bitmap grayimage =  image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = 0; y < grayimage.Height ; y++)
                {
                    for (int x = size; x < grayimage.Width ; x++)
                    {
                        #region 取nxn的pixel值
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                    else { mask[index++] = grayimage.GetPixel(i, j).R;}
                                }
                            }
                        }
                        #endregion
                        Array.Sort(mask);
                        int median = mask[size * size / 2];
                        Medianimage.SetPixel(x, y, Color.FromArgb(median, median, median));

                    }
                }
            }
            return Medianimage;
            
        }
        public Bitmap CrossMedian(Bitmap image, int size)//+型
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap Medianimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[(size-1)*4+1];
                int index = 0;
                for (int y = 0 ; y < grayimage.Height; y++)
                {
                    for (int x = 0 ; x < grayimage.Width; x++)
                    {
                        #region 取+型的pixel值
                        if (index >= (size - 1) * 4 ) { index = 0; }
                        else
                        {
                            for (int i = (x - (size - 1)); i < x; i++)
                            {
                                if (i >= grayimage.Width || i < 0 ) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(i, y).R;}
                               
                            }
                            for (int i = x; i < (x + size - 1); i++)
                            {
                                if (i >= grayimage.Width || i < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(i, y).R; }
                            }
                            for (int i = (y - (size - 1)); i < y; i++)
                            {
                                if (i >= grayimage.Height || i < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(x, i).R; }
                               
                            }
                            for (int i = y; i < (y + size - 1); i++)
                            {
                                if (i >= grayimage.Height || i < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(x, i).R; }
                            }
                            mask[index] = grayimage.GetPixel(x, y).R; // n
                        }
                        #endregion
                        Array.Sort(mask);

                        int median = mask[mask.Length/2];
                        Medianimage.SetPixel(x, y, Color.FromArgb(median, median, median));

                    }
                }
            }
            return Medianimage;
        }

        public Bitmap PseudoMedian(Bitmap image, int size)//分別取直的跟橫的mask
        {
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] xmask = new int[(size - 1) * 2 + 1];
                int[] ymask = new int[(size - 1) * 2 + 1];
                int xindex = 0, yindex = 0;
                for (int y = 0; y < grayimage.Height; y++)
                {
                    for (int x = 0; x < grayimage.Width; x++)
                    {
                        if((yindex >= (size - 1) * 2)&& (xindex >= (size - 1) * 2)) { yindex = 0; xindex = 0; }
                        else
                        {
                            //PMED= 0.5*max(MAXMIN_x,MAXMIN_y)+0.5*min(MINMAX_x,MINMAX_y)
                            //==================橫=====================
                            for (int i = (x - (size - 1)); i < (x + size); i++)
                            {
                                if (i >= grayimage.Width || i < 0 ) { xmask[xindex++] = 0; }
                                else { xmask[xindex++] = grayimage.GetPixel(i, y).R; }
                               
                            }
                            //==================直======================
                            for (int i = (y - (size - 1)); i < (y + size); i++)
                            {
                                if (i >= grayimage.Width || i < 0 ) { ymask[yindex++] = 0; }
                                else { ymask[yindex++] = grayimage.GetPixel(x, i).R; }
                            }
                        }
                        //==================MAXMIN========================
                        int MAXMIN_x = 0, MAXMIN_y = 0;
                        for (int j = 0; j < xmask.Length - 1; j++)
                        {
                            MAXMIN_x = Math.Max(MAXMIN_x, Math.Min(xmask[j], xmask[j + 1]));
                            MAXMIN_y = Math.Max(MAXMIN_y, Math.Min(ymask[j], ymask[j + 1]));
                        }
                        //==================MINMAX========================
                        int MINMAX_x = 999, MINMAX_y = 999;
                        for (int j = 0; j < xmask.Length - 1; j++)
                        {
                            MINMAX_x = Math.Min(MINMAX_x, Math.Max(xmask[j], xmask[j + 1]));
                            MINMAX_y = Math.Min(MINMAX_y, Math.Max(ymask[j], ymask[j + 1]));
                        }
                        int PMED =(int)(0.5 * Math.Max(MAXMIN_x, MAXMIN_y) + 0.5 * Math.Min(MINMAX_x, MINMAX_y));
                        newimage.SetPixel(x, y, Color.FromArgb(PMED, PMED, PMED));

                    }
                }
            }
            return newimage;

        }
        #endregion
        public Bitmap Highpass(Bitmap image, int size)
        {
            //{(-1-1-1),(-1 8 -1),(-1-1-1)}
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            int[] kernel = new int[size * size];
            for(int i = 0;i<kernel.Length;i++)
            {
                kernel[i] = -1;
            }
            kernel[size * size / 2] = kernel.Length - 1;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = 0; y < grayimage.Height; y++)
                {
                    for (int x = 0; x < grayimage.Width ; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                    else { mask[index++] = grayimage.GetPixel(i, j).R; }
                                }
                            }
                        }
                        double d =0;
                        for (int i = 0; i < mask.Length; i++)
                        {
                            d += mask[i]*kernel[i];
                        }
                        d /= mask.Length;
                        if (d < 0) { d = 0; }
                        else if (d > 255) { d = 255; }
                        newimage.SetPixel(x, y, Color.FromArgb((int)d, (int)d, (int)d));

                    }
                }
            }
            return newimage;

        }
        public Bitmap Lowpass(Bitmap image, int size)
        {
            //{(111),(111),(111)}
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = 0; y < grayimage.Height; y++)
                {
                    for (int x = 0; x < grayimage.Width ; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                    else { mask[index++] = grayimage.GetPixel(i, j).R; }
                                }
                            }
                        }
                        int sum = 0;
                        for(int i = 0; i<mask.Length;i++)
                        {
                            sum += mask[i];
                        }
                        int g = sum / mask.Length;
                        newimage.SetPixel(x, y, Color.FromArgb(g,g,g));

                    }
                }
            }
            return newimage;

        }
        public Bitmap EdgeCrispening(Bitmap image, int size)
        {
            //取I(x,y)周圍size*size個點算平均值，I(x,y)減平均值得到一个差值。差值乘係數（也就是銳化的程度），加上自己的原始值
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = 0; y < grayimage.Height; y++)
                {
                    for (int x = 0; x < grayimage.Width; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                    else { mask[index++] = grayimage.GetPixel(i, j).R; }
                                }
                            }
                        }
                        int mean = 0;
                        for(int i =0;i<mask.Length;i++)
                        {
                            mean += mask[i];
                        }
                        mean -= mask[mask.Length / 2];
                        mean /= mask.Length;
                        int g = (mean - mask[mask.Length / 2])*100 + mask[mask.Length / 2];
                        if (g < 0) { g = 0; }
                        else if (g > 255) { g = 255; }
                        newimage.SetPixel(x, y, Color.FromArgb(g, g, g));

                    }
                }
            }
            return newimage;
        }
        public Bitmap HighBoost(Bitmap image, int size,double A)
        {
            //{(-1-1-1),(-1 9A-1 -1),(-1-1-1)}
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            double[] kernel = new double[size * size];
            for (int i = 0; i < kernel.Length; i++)
            {
                kernel[i] = -1;
            }
            kernel[size * size / 2] = kernel.Length*A - 1;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            if (size != 0)
            {
                int[] mask = new int[size * size];
                int tmp = size / 2;
                int index = 0;
                for (int y = 0; y < grayimage.Height ; y++)
                {
                    for (int x = 0; x < grayimage.Width ; x++)
                    {
                        for (int j = y - tmp; j <= y + tmp; j++)
                        {
                            for (int i = x - tmp; i <= x + tmp; i++)
                            {
                                if (index >= size * size) { index = 0; }
                                else
                                {
                                    if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                    else { mask[index++] = grayimage.GetPixel(i, j).R; }
                                }
                            }
                        }
                        double d = 0;
                        for (int i = 0; i < mask.Length; i++)
                        {
                            d += mask[i] * kernel[i];
                        }
                        d /= mask.Length;
                        if (d < 0) { d = 0; }
                        else if (d > 255) { d = 255; }
                        newimage.SetPixel(x, y, Color.FromArgb((int)d, (int)d, (int)d));

                    }
                }
            }
            return newimage;
        }
        public Bitmap Roberts(Bitmap image) 
        {
            //| 1  0 |    | 0  1 |
            //| 0 -1 |  & |-1  0 |
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            int[] mask = new int[4];
            int index = 0;
            int tmp = 1;
            for (int y = 0; y < grayimage.Height; y++)
            {
                for (int x = 0; x < grayimage.Width; x++)
                {
                    for (int j = y - tmp; j < y + tmp; j++)
                    {
                        for (int i = x - tmp; i < x + tmp; i++)
                        {
                            if (index >= 4) { index = 0; }
                            else
                            {
                                if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(i, j).R; }
                            }
                        }
                    }

                    int d = (int)Math.Sqrt((mask[0] - mask[3]) * (mask[0] - mask[3]) + (mask[1] - mask[2]) * (mask[1] - mask[2]));
                    if (d < 0) { d = 0; }
                    else if (d > 255) { d = 255; }
                    newimage.SetPixel(x, y, Color.FromArgb(d,d,d));
                }
            }
            return newimage;
        }
        public Bitmap Sobel(Bitmap image)
        {  
            //Gx:[1 0 -1]  Gy:[ 1  2  1]
            //   [2 0 -2]     [ 0  0  0]
            //   [1 0 -1]     [-1 -2 -1]
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            Bitmap xnewimage = new Bitmap(grayimage.Width, grayimage.Height);
            Bitmap ynewimage = new Bitmap(grayimage.Width, grayimage.Height);
            int[] mask = new int[9];
            int tmp = 1;
            int index = 0;
            for (int y = 0; y < grayimage.Height; y++)
            {
                for (int x = 0; x < grayimage.Width; x++)
                {
                    for (int j = y - tmp; j <= y + tmp; j++)
                    {
                        for (int i = x - tmp; i <= x + tmp; i++)
                        {
                            if (index >= 9) { index = 0; }
                            else
                            {
                                if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(i, j).R; }
                            }
                        }
                    }
                    int dy = (int)Math.Abs( (-mask[0] - 2 * mask[3] - mask[6]) + (mask[2] + 2 * mask[5] + mask[8]));
                    int dx = (int)Math.Abs( (mask[0] + 2 * mask[1] + mask[2]) + (-mask[6] - 2 * mask[7] - mask[8]));
                    int d = dx + dy;
                    if (dx < 0) { dx = 0; }
                    else if (dx > 255) { dx = 255; }
                    if (dy < 0) { dy = 0; }
                    else if (dy > 255) { dy = 255; }
                    if (d < 0) { d = 0; }
                    else if (d > 255) { d = 255; }
                    
                    xnewimage.SetPixel(x, y, Color.FromArgb(dx, dx, dx));
                    ynewimage.SetPixel(x, y, Color.FromArgb(dy, dy, dy));
                    newimage.SetPixel(x, y, Color.FromArgb(d, d, d));
                }
            }
            pictureBox3.Image = xnewimage;
            pictureBox4.Image = ynewimage;
            return newimage;
        }
        public Bitmap Prewitt(Bitmap image)
        {
            //Gx:[1 0 -1]  Gy:[ 1  1  1]
            //   [1 0 -1]     [ 0  0  0]
            //   [1 0 -1]     [-1 -1 -1]
            Bitmap grayimage = image2Graylevel(image);
            pictureBox1.Image = grayimage;
            Bitmap newimage = new Bitmap(grayimage.Width, grayimage.Height);
            Bitmap xnewimage = new Bitmap(grayimage.Width, grayimage.Height);
            Bitmap ynewimage = new Bitmap(grayimage.Width, grayimage.Height);
            int[] mask = new int[9];
            int tmp = 1;
            int index = 0;
            for (int y = 0; y < grayimage.Height; y++)
            {
                for (int x = 0; x < grayimage.Width; x++)
                {
                    for (int j = y - tmp; j <= y + tmp; j++)
                    {
                        for (int i = x - tmp; i <= x + tmp; i++)
                        {
                            if (index >= 9) { index = 0; }
                            else
                            {
                                if (i >= grayimage.Width || i < 0 || j >= grayimage.Height || j < 0) { mask[index++] = 0; }
                                else { mask[index++] = grayimage.GetPixel(i, j).R; }
                            }
                        }
                    }
                    int dy = (int)Math.Abs((-mask[0] - mask[3] - mask[6]) + (mask[2] + mask[5] + mask[8]));
                    int dx = (int)Math.Abs((-mask[0] - mask[1] - mask[2]) + (mask[6] + mask[7] + mask[8]));
                    int d = dx + dy;
                    if (dx < 0) { dx = 0; }
                    else if (dx > 255) { dx = 255; }
                    if (dy < 0) { dy = 0; }
                    else if (dy > 255) { dy = 255; }
                    if (d < 0) { d = 0; }
                    else if (d > 255) { d = 255; }

                    xnewimage.SetPixel(x, y, Color.FromArgb(dx, dx, dx));
                    ynewimage.SetPixel(x, y, Color.FromArgb(dy, dy, dy));
                    newimage.SetPixel(x, y, Color.FromArgb(d, d, d));
                }
            }
            pictureBox3.Image = xnewimage;
            pictureBox4.Image = ynewimage;
            return newimage;
        }
        #region button
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Outlier(image, Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image, Outlier(image, Convert.ToInt32(textBox1.Text))))+" dB";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = SquareMedian(image,Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), SquareMedian(image, Convert.ToInt32(textBox1.Text))))+" dB";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = CrossMedian(image, Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), CrossMedian(image, Convert.ToInt32(textBox1.Text))))+" dB";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "1" || textBox1.Text == "0") { pictureBox2.Image = image2Graylevel(image); }
            else { pictureBox2.Image = PseudoMedian(image, Convert.ToInt32(textBox1.Text)); }
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), PseudoMedian(image, Convert.ToInt32(textBox1.Text)))) + " dB";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Highpass(image, Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), Highpass(image, Convert.ToInt32(textBox1.Text)))) + " dB";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Lowpass(image, Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), Lowpass(image, Convert.ToInt32(textBox1.Text)))) + " dB";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = EdgeCrispening(image, Convert.ToInt32(textBox1.Text));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), EdgeCrispening(image, Convert.ToInt32(textBox1.Text)))) + " dB";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = HighBoost(image, Convert.ToInt32(textBox2.Text.Split(' ')[0]), Convert.ToDouble(textBox2.Text.Split(' ')[1]));
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), HighBoost(image,Convert.ToInt32(textBox2.Text.Split(' ')[0]), Convert.ToDouble(textBox2.Text.Split(' ')[1])))) + " dB";
        }
        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Roberts(image);
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), Roberts(image))) + " dB";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Sobel(image);
            richTextBox1.Text = "SNR: " + Convert.ToString(SNR(image2Graylevel(image), Sobel(image))) + " dB";
        }
        private void button11_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = Prewitt(image);
            richTextBox1.Text ="SNR: "+Convert.ToString(SNR(image2Graylevel(image), Prewitt(image)))+" dB";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Filter_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
