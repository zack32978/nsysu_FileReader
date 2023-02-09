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
    public partial class Cut : Form
    {
        public Bitmap image;
        //畫布 畫筆
        public Graphics g;
        public Pen crpPen = new Pen(Color.LightGray,2);
        //crop coordinate
        int crpx, crpy, rectW, rectH,ncrpx,ncrpy;
        
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            Controls.Add(pictureBox1);
            if (crpx > 0 && crpy > 0)
            {
                if (radioButton1.Checked == true)
                {
                    pictureBox2.Image = Rectangular();
                    pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else if (radioButton2.Checked == true)
                {
                    pictureBox2.Image = Oval();
                    pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                }
            }
        }
        public Bitmap Rectangular() 
        {
            if (rectW <= 0) { crpx = ncrpx; }
            if (rectH <= 0) { crpy = ncrpy; }
            Bitmap image2 = new Bitmap(image.Width, image.Height);
            pictureBox1.DrawToBitmap(image2, pictureBox1.ClientRectangle);
            Bitmap cut_image = new Bitmap(Math.Abs(rectW), Math.Abs(rectH));
            int width, height;
            for (int i = 0; i < Math.Abs(rectW); i++)
            {
                for (int j = 0; j < Math.Abs(rectH); j++)
                {
                    width = crpx + i;
                    height = crpy + j;
                    if (width >= image.Width) { width = (image.Width) - 1; }
                    else if (width <= 0) { width = 0; }
                    if (height >= image.Height) { height = (image.Height) - 1; }
                    else if (height <= 0) { height = 0; }
                    Color pxlclr = image2.GetPixel(width, height);
                    cut_image.SetPixel(i, j, pxlclr);
                }
            }
            return cut_image;
        }
        public Bitmap Oval()
        {// (x-h)^2/a*a  +  (y-k)^2/b*b = 1
            if (rectW <= 0) { crpx = ncrpx; }
            if (rectH <= 0) { crpy = ncrpy; }
            Bitmap image2 = new Bitmap(image.Width, image.Height);
            pictureBox1.DrawToBitmap(image2, pictureBox1.ClientRectangle);
            Bitmap cut_image = new Bitmap(Math.Abs(rectW), Math.Abs(rectH));
            int centerX = crpx + Math.Abs(rectW) / 2;//算出中心
            int centerY = crpy + Math.Abs(rectH) / 2;
            double A_X = Math.Abs(rectW) / 2;//長短軸
            double B_Y = Math.Abs(rectH) / 2;
            double AA = A_X * A_X;//平方的底
            double BB = B_Y * B_Y;
            int width, height;
            for (int i = 0; i < Math.Abs(rectW); i++)
            {
                for (int j = 0; j < Math.Abs(rectH) ; j++)
                {
                    int drawX = Math.Abs(crpx + i - centerX);
                    int drawY = Math.Abs(crpy + j - centerY);
                    if (((drawX * drawX) / AA + (drawY * drawY) / BB) <= 1)
                    {
                        width = crpx + i;
                        height = crpy + j;
                        if (width >= image.Width ) { width = (image.Width) - 1; }
                        else if (width <= 0) { width = 0; }
                        if (height >= image.Height) { height = (image.Height) - 1; }
                        else if (height <= 0) { height = 0; }
                        Color pxlclr = image2.GetPixel(width, height);
                        cut_image.SetPixel(i, j, pxlclr);
                    }
                }
            }
            return cut_image;
            
        }
        #region MagicWand
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
        Bitmap copyimg;
        int pasteX, pasteY;
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            Controls.Add(pictureBox1);
            if(crpx>0  && crpy > 0) 
            {
                if (radioButton1.Checked == true)
                {
                    copyimg = Rectangular();
                }
                else if (radioButton2.Checked == true)
                {
                    copyimg = Oval();
                }
            }
            
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int width = copyimg.Width;
            int height = copyimg.Height;
            int rangex = pasteX + width;
            int rangey = pasteY + height;
            if (rangex > image.Width) { rangex = image.Width; }
            if (rangey > image.Height) { rangey = image.Height; }
            for (int y = pasteY; y< rangey ;y++)
            {
                for(int x = pasteX; x < rangex; x++)
                {
                    image.SetPixel(x,y,copyimg.GetPixel(x-pasteX,y-pasteY));
                }
            }
            pictureBox1.Image = image;
        }

        private void pictureBox1_ContextMenuStripChanged(object sender, EventArgs e)
        {
            //pictureBox1.ContextMenustrip = contextMenuStrip1;
        }
        #endregion
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                //決定起點
                crpx = e.X;
                crpy = e.Y;
                
            }
            if (e.Button == MouseButtons.Right)
            {
                //決定起點
                pasteX = e.X;
                pasteY = e.Y;

            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            label2.Text = "(" + e.X + "," + e.Y + ")";
            if (e.Button == MouseButtons.Left)
            {
                pictureBox1.Refresh();
                g = pictureBox1.CreateGraphics();
                if (radioButton1.Checked == true)
                {
                    rectW = e.X - crpx;//最後位址-原始位置 會取得x 還有 y
                    rectH = e.Y - crpy;
                    ncrpx = e.X;
                    ncrpy = e.Y;
                    g.DrawRectangle(crpPen, crpx, crpy, rectW, rectH);
                    g.Dispose();
                }
                else if (radioButton2.Checked == true)
                {
                    
                    rectW = e.X - crpx;
                    rectH = e.Y - crpy;
                    ncrpx = e.X;
                    ncrpy = e.Y;
                    //最後位置-原始位置 會取得x 還有 y
                    g.DrawEllipse(crpPen, crpx, crpy, rectW, rectH);
                    g.Dispose();
                }
            }
            

        }

        public Cut(Bitmap Image)
        {
            InitializeComponent();
            image = Image;
            pictureBox1.Image = image;
        }

        private void Cut_Load(object sender, EventArgs e)
        {

        }
    }
}
