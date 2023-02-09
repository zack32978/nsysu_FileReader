using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Collections.Concurrent;

namespace FileReader
{
    public partial class Mpeg : Form
    {
        
        public Mpeg()
        {
            InitializeComponent();
            timer1.Interval = 600;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Enabled = false;
            timer2.Interval = 1000;
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisX.Minimum = 0;

        }
        List<Bitmap> Listimg;
        int total, index;
        string folderName;
        List<Bitmap> imageList;
        int flag = 0; //player(0),encode(1),decode(2);
        public Graphics g,g1,g2;
        public Pen crpPen = new Pen(Color.Red, 1);
        List<byte[,]> cblocks = new List<byte[,]>();
        List<byte[,]> tblocks = new List<byte[,]>();
        List<List<PointF>> MVList = new List<List<PointF>>();
        #region Encode && Decode
        //Encode
        private void button6_Click(object sender, EventArgs e)
        {
            //flag = 1;
            crpPen.DashStyle = DashStyle.DashDot;
            for (int i = 0; i < total - 1; i++)
            {
                PointF candP= new PointF(), targP = new PointF();
                List<PointF> MV = new List<PointF>();
                pictureBox1.Image = imageList[i];
                pictureBox2.Image = imageList[i + 1];
                richTextBox2.Text = Convert.ToString(i + 1) + "/" + total;
                richTextBox3.Text = Convert.ToString(i + 2) + "/" + total;

                pictureBox1.Refresh();
                pictureBox2.Refresh();
                pictureBox3.Refresh();
                richTextBox2.Refresh();
                richTextBox3.Refresh();
                cblocks.Clear();
                tblocks.Clear();
                Bitmap tbitmap = imageList[i+1];
                Bitmap cbitmap = imageList[i];
                GetTblocks(tbitmap);
                GetCblocks(cbitmap);
                double t = Math.Sqrt(tblocks.Count());
                double c = Math.Sqrt(cblocks.Count());
                for (int j=0; j < tblocks.Count();j++)
                {
                    double diff = 9999;
                    double totald;
                    int tx = (int)(j % t);
                    int ty = (int)(j / t);
                    targP = new PointF(tx*8, ty*8);
                    show_rect(targP,pictureBox2);
                    show_block(tblocks[j], pictureBox6);
                    pictureBox6.Refresh();
                    for (int k=0;k< cblocks.Count(); k++)
                    {
                        int cx = (int)(k % c);
                        int cy = (int)(k / c);
                        PointF P = new PointF(cx, cy);
                        show_rect(P, pictureBox1);
                        show_block(cblocks[k], pictureBox5);
                        pictureBox5.Refresh();
                        totald = MatchBlock(tblocks[j], cblocks[k]);
                        if (diff >= totald)
                        {
                            diff = totald;
                            candP = P;
                        }
                        pictureBox1.Refresh();
                    }
                    pictureBox2.Refresh();
                    MV.Add(candP);
                    MV.Add(targP);
                    ShowMV(MV);
                }
                imageList[i + 1] = Encode_decode(MV, cbitmap);
                SaveMV(MV, i+1);
                //ShowMV(MV);
            }
        }
        public Bitmap Encode_decode(List<PointF> vector, Bitmap cbitmap)
        {
            byte[,] tArray = new byte[cbitmap.Width, cbitmap.Height];
            for (int j = 0; j < vector.Count() - 1; j += 2)
            {
                byte[,] cArray = Bitmap2ByteArray(cbitmap);
                PointF candP = vector[j];
                byte[,] candBlock = GetRectanglePixel((int)candP.X, (int)candP.Y, cArray);
                PointF targP = vector[j + 1];
                tArray = PasteRectanglePixel((int)targP.X, (int)targP.Y, candBlock, tArray);
            }
            return Byte2Bitmap(tArray);
        }
        //Decode
        public void Decode()
        {
            flag = 2;
            if (button7.Visible == true)
            {
                List<List<PointF>> ListMV = OpenMV();
                MVList =  OpenMV();
                if (ListMV.Count == 0) { richTextBox1.Text = "NO motion vector file"; }
                else
                {
                    for (int i = 0; i < total-1; i++)
                    {
                        pictureBox1.Refresh();
                        pictureBox2.Refresh();
                        pictureBox3.Refresh();
                        List<PointF> MV = ListMV[i];
                        pictureBox1.Image = imageList[i];
                        richTextBox2.Text = Convert.ToString(i + 1) + "/" + total;
                        richTextBox3.Text = Convert.ToString(i + 2) + "/" + total;
                        richTextBox2.Refresh();
                        richTextBox3.Refresh();
                        Bitmap cbitmap = imageList[i];
                        byte[,] tArray = new byte[imageList[i].Width, imageList[i].Height];
                        for (int j = 0; j < MV.Count() - 1; j += 2)
                        {
                            pictureBox1.Refresh();
                            pictureBox2.Refresh();
                            drawVector(MV[j], MV[j + 1], Color.DarkRed);
                            g1 = pictureBox1.CreateGraphics();
                            byte[,] cArray = Bitmap2ByteArray(cbitmap);

                            PointF candP = MV[j];
                            g1.DrawRectangle(crpPen, candP.X, candP.Y, 8, 8);
                            g1.Dispose();
                            byte[,] candBlock = GetRectanglePixel((int)candP.X, (int)candP.Y, cArray);
                            PointF targP = MV[j + 1];
                            tArray = PasteRectanglePixel((int)targP.X, (int)targP.Y, candBlock, tArray);
                            pictureBox5.Image = Byte2Bitmap(candBlock);
                            pictureBox5.Refresh();

                            pictureBox2.Image = Byte2Bitmap(tArray);
                            //pictureBox2.Refresh();

                        }
                        chart1.Series[0].Points.AddXY(i + 1, Convert.ToString(Math.Abs(PSNR(Byte2Bitmap(tArray),imageList[i+1]))));
                        imageList[i + 1] = Byte2Bitmap(tArray);
                        chart1.Refresh();

                    }
                }
                
            }
            
        }
        #region function
        public double PSNR(Bitmap cand,Bitmap target)
        {
            double psnr,MSE=0;
            int width = cand.Width,height = cand.Height;

            for(int y=0;y<height;y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double d = Math.Abs(cand.GetPixel(x,y).R-target.GetPixel(x,y).R);
                    MSE += d*d;
                }
            }
            MSE = (MSE / height * width);
            psnr = 10 * Math.Log10(255*255/MSE);
            return psnr;
        }
        public List<List<PointF>> OpenMV()
        {
            List<List<PointF>> ListMV = new List<List<PointF>>();
            if (File.Exists(folderName + "/encode/" + "01.txt"))
            {
                foreach (string fname in Directory.GetFiles(folderName + "/encode/"))
                {
                    List < PointF > MV = new List<PointF>();
                    StreamReader sr = new StreamReader(fname);
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        string[] Line = line.Split(',');
                        String value0 = System.Text.RegularExpressions.Regex.Replace(Line[0], "[^0-9]", ""); //僅保留數字
                        String value1 = System.Text.RegularExpressions.Regex.Replace(Line[1], "[^0-9]", ""); //僅保留數字
                        PointF p = new PointF(Convert.ToInt32(value0), Convert.ToInt32(value1));
                        MV.Add(p);
                        line = sr.ReadLine();
                    }
                    //close the file
                    sr.Close();
                    ListMV.Add(MV);
                }
            }
            else
            {
                richTextBox1.Text = "NO motion vector file";
            }
            return ListMV;
        }
        public byte[,] Bitmap2ByteArray(Bitmap image)
        {
            byte[,] Array = new byte[image.Width, image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Array[x, y] = image.GetPixel(x, y).R;
                }
            }
            return Array;
        }
        public Bitmap Byte2Bitmap(byte[,] ByteArray)
        {
            int Width = (int)Math.Sqrt(ByteArray.Length);
            int Height = (int)Math.Sqrt(ByteArray.Length);
            Bitmap Bmp = new Bitmap(Width, Height);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    byte color = ByteArray[x, y];
                    Bmp.SetPixel(x, y, Color.FromArgb(color, color, color));
                }
            }
            return Bmp;
        }
        public byte[,] PasteRectanglePixel(int x, int y, byte[,] cblock_Byte, byte[,] Target_Byte)
        {
            for (int h = 0; h < 8; h++)
            {
                for (int w = 0 ; w <  8; w++)
                {
                    Target_Byte[x+w, y+h] = cblock_Byte[w,h];
                }
            }
            return Target_Byte;
        }
        public byte[,] GetRectanglePixel(int x, int y, byte[,] Picture_Byte)
        {
            byte[,] Byte_Reg = new byte[8, 8];
            for (int h = y; h < y + 8; h++)
            {
                for (int w = x; w < x + 8; w++)
                {
                    Byte_Reg[w - x, h - y] = Picture_Byte[w, h];
                }
            }
            return Byte_Reg;
        }
        public void GetCblocks(Bitmap cbitmap)
        {
            int Width = cbitmap.Width;
            int Height = cbitmap.Height;
            for (int y = 0; y < Height - 8; y++)
            {
                for (int x = 0; x < Width - 8; x++)
                {
                    Rectangle candrect = new Rectangle(x,y, 8, 8);
                    Bitmap candImage = cbitmap.Clone(candrect, System.Drawing.Imaging.PixelFormat.DontCare);
                    cblocks.Add(Bitmap2ByteArray(candImage));
                    //cblocks.Add(candImage);
                }
            }
        }
        public void GetTblocks(Bitmap tbitmap) 
        {
            int Width = tbitmap.Width;
            int Height = tbitmap.Height;
            for (int y = 0; y < Height/8; y++)
            {
                for (int x = 0; x < Width/8; x++)
                {
                    Rectangle targetrect = new Rectangle(x * 8, y * 8, 8, 8);
                    Bitmap targetImage = tbitmap.Clone(targetrect, System.Drawing.Imaging.PixelFormat.DontCare);
                    tblocks.Add(Bitmap2ByteArray(targetImage));

                    //tblocks.Add(targetImage);
                }
            }
        }
        public int MatchBlock(byte[,] candImage, byte[,] targetImage/*,int x,int y*/)
        {
            //Match_Vector match_ = new Match_Vector();
            int totald = 0;
            for (int j = 0; j < 8; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    totald += Math.Abs(candImage[i, j] - targetImage[i, j]);
                }
            }
            return totald;
        }
        public void SaveMV(List<PointF> MV, int i)
        {
            if (Directory.Exists(folderName + "/encode/"))
            {
                File.AppendAllLines(folderName + "/encode/" + (String.Format("{0:00}", i) + ".txt"), MV.Select(p => p.ToString()));
            }
            else
            {
                //新增資料夾
                Directory.CreateDirectory(folderName + "/encode/");
                File.AppendAllLines(folderName + "/encode/" + (String.Format("{0:00}",i) + ".txt"), MV.Select(p => p.ToString()));
                
            }
        }
        #endregion
        #endregion
        #region 顯示

        //=====================畫線========================
        private void drawVector(PointF A, PointF B, Color color)
        {
            //pictureBox3.Refresh();
            g = pictureBox3.CreateGraphics();
            if(A == B) 
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillRectangle(Brushes.Black, A.X+3, A.Y + 3, 2, 2);
            }
            else 
            {
                Pen pen = new Pen(color,1);
                pen.DashStyle = DashStyle.Solid;
                pen.EndCap = LineCap.ArrowAnchor;//定義線尾的樣式為箭頭
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(pen, A.X+3,A.Y+3, B.X+3,B.Y+3);
            }
        }
        public void ShowMV(List<PointF> MV) 
        {
            for(int i =0;i<MV.Count();i+=2) 
            {
                drawVector(MV[i], MV[i + 1], Color.DarkRed);
            }
        }
        public void show_rect(PointF p, PictureBox box)
        {
            Graphics G = box.CreateGraphics();
            G.DrawRectangle(crpPen, p.X, p.Y, 8, 8);

        }
        public void show_block(byte[,] blocks, PictureBox box)
        {
            box.Image = Byte2Bitmap(blocks);

        }
        public void Display(int t, int c, List<PointF> MV)
        {
            for (int j = 0; j < tblocks.Count(); j++)
            {
                int tx = (int)(j % t);
                int ty = (int)(j / t);
                pictureBox6.Image = Byte2Bitmap(tblocks[j]);
                pictureBox6.Refresh();
                g2 = pictureBox2.CreateGraphics();
                g2.DrawRectangle(crpPen, tx * 8, ty * 8, 8, 8);
                g2.Dispose();
                for (int k = 0; k < cblocks.Count(); k++)
                {
                    int cx = (int)(k % c);
                    int cy = (int)(k / c);
                    pictureBox5.Image = Byte2Bitmap(cblocks[k]);
                    g1 = pictureBox1.CreateGraphics();
                    g1.DrawRectangle(crpPen, cx, cy, 8, 8);
                    g1.Dispose();
                    pictureBox1.Refresh();
                    pictureBox5.Refresh();
                }
                pictureBox2.Refresh();
            }
        }
        #endregion
        #region player function
        private void button5_Click(object sender, EventArgs e)//Open file
        {
            index = 0;
            total = 0;
            imageList = new List<Bitmap>();
            Listimg = new List<Bitmap>();
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.Description = "請選擇目錄";
            path.ShowDialog();
            richTextBox1.Text = path.SelectedPath;
            folderName = path.SelectedPath;
            // 取得資料夾內所有檔案
            foreach (string fname in Directory.GetFiles(folderName))
            {
                byte[] imagefile = File.ReadAllBytes(fname);
                MemoryStream ms = new MemoryStream(imagefile);
                Bitmap image = (Bitmap)Bitmap.FromStream(ms);
                total++;
                imageList.Add(image);
                Listimg.Add(image);
                //Frame_Byte.Add(Bitmap2ByteArray(image));
            }
            pictureBox1.Image = imageList[0];
            pictureBox2.Image = imageList[1];
            richTextBox2.Text = (string)(1 + "/" + total);
            richTextBox3.Text = (string)(2 + "/" + total);
            if (MVList.Count() != 0) { pictureBox3.Refresh(); ShowMV(MVList[index]); }
        }
        //play
        private void button1_Click(object sender, EventArgs e)
        {
            if (flag == 0)
            { timer1.Enabled = true; }
            else if (flag == 2)
            { timer1.Enabled = true; }
        }
        //forward
        private void button2_Click(object sender, EventArgs e)
        {
            if (index < total - 2)
            {
                index++;
                pictureBox1.Image = imageList[index];
                pictureBox2.Image = imageList[index + 1];
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                richTextBox2.Text = (string)((index + 1) + "/" + total);
                richTextBox3.Text = (string)((index + 2) + "/" + total);
            }
            if (flag == 2)
            {
                if (index < total - 2)
                {
                    index++;
                    if (MVList.Count() != 0)
                    {
                        richTextBox2.Text = (string)((index + 2) + "/" + total);
                        richTextBox3.Text = (string)((index + 2) + "/" + total);
                        pictureBox1.Refresh();
                        pictureBox1.Image = Listimg[index + 1];
                        pictureBox1.Refresh();
                        pictureBox2.Image = imageList[index + 1];
                        pictureBox2.Refresh();
                        pictureBox3.Refresh();
                        ShowMV(MVList[index]);
                    }
                }
            }
        }
        //back
        private void button3_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index--;
                pictureBox1.Image = imageList[index];
                pictureBox1.Refresh();
                pictureBox2.Image = imageList[index+1];
                pictureBox2.Refresh();
                richTextBox2.Text = (string)((index + 1) + "/" + total);
                richTextBox3.Text = (string)((index + 2) + "/" + total);
                
            }
            if(flag == 2) 
            {
                if (index > 0)
                {
                    index--;
                    if (MVList.Count() != 0)
                    {
                        richTextBox2.Text = (string)((index + 2) + "/" + total);
                        richTextBox3.Text = (string)((index + 2) + "/" + total);
                        pictureBox1.Refresh();
                        pictureBox1.Image = Listimg[index + 1];
                        pictureBox1.Refresh();
                        pictureBox2.Image = imageList[index + 1];
                        pictureBox2.Refresh();
                        pictureBox3.Refresh();
                        ShowMV(MVList[index]);
                    }
                }
            }
        }
        //pause
        private void button4_Click(object sender, EventArgs e)
        {
            if (flag == 0)
            { timer1.Enabled = false; }
            else if(flag == 2)
            { timer2.Enabled = false; }
        }
        //Decode
        private void button7_Click(object sender, EventArgs e)
        {
            flag = 2;
            Decode();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (index < total - 1)
            {
                richTextBox2.Refresh();
                richTextBox3.Refresh();
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                pictureBox1.Image = imageList[index];
                pictureBox2.Image = imageList[index + 1];
                richTextBox2.Text = (string)((index + 1) + "/" + total);
                richTextBox3.Text = (string)((index + 2) + "/" + total);
                if (index == total - 2) { timer1.Enabled = false; }
                index++;
            }
            if (flag == 2)
            {
                if (index < total - 1)
                {

                    richTextBox2.Refresh();
                    richTextBox3.Refresh();
                    pictureBox1.Refresh();
                    pictureBox2.Refresh();
                    pictureBox1.Image = Listimg[index+1];
                    pictureBox2.Image = imageList[index + 1];
                    richTextBox2.Text = (string)((index + 2) + "/" + total);
                    richTextBox3.Text = (string)((index + 2) + "/" + total);
                    if (index == total - 2) { timer1.Enabled = false; }
                   
                    index++;
                }
            }


        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            Decode();
            //Display((int)Math.Sqrt(tblocks.Count()), (int)Math.Sqrt(cblocks.Count()),MVlist);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Mpeg_Load(object sender, EventArgs e)
        {

        }

        
        
        #endregion
    }
    
}
