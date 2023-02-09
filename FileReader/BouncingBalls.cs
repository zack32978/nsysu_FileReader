using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace FileReader
{
    public partial class BouncingBalls : Form
    {
        private const string ERROR_MESSAGE = "Sorry can only have up to 10 balls at one time!";
        private const int TIMER_SLEEP = 15;
        private const int MAXIMUM_BALLS = 100;
        private Thread t;
        private List<Ball> balls;
        private Random rand;
        public BouncingBalls(Bitmap image)
        {
            InitializeComponent();
            //宣告thread 還有random
            rand = new Random();
            pictureBox1.Invalidate();
            //new thread
            t = new Thread(new ThreadStart(Run));
            balls = new List<Ball>();
            t.Start();
        }
        private void Run()
        {
            //開始跑  執行然後檢測碰撞
            while (true)
            {
                foreach (Ball b in balls)
                {
                    CheckCollisions(b);
                    b.Go();
                }
                pictureBox1.Invalidate();
                Thread.Sleep(TIMER_SLEEP);
            }

        }
        private void CheckCollisions(Ball ball)
        {
            // Make list of balls to check collisions against
            //用一個list去裝檢測
            List<Ball> collisionList = new List<Ball>();
            foreach (Ball b in balls)
            {
                collisionList.Add(b);
            }

            //自己跟自己不用檢測
            collisionList.Remove(ball);

            // Check for collision with every other ball
            foreach (Ball b2 in collisionList)
            {
                if (ball.CollidesWith(b2))
                {
                    //有時會沾黏
                    ball.speedx *= -1;
                    ball.speedy *= -1;

                    b2.speedx *= -1;
                    b2.speedy *= -1;
                }
            }
        }
        public void AddBall()
        {
            if (balls.Count < MAXIMUM_BALLS)
            { balls.Add(new Ball(rand.Next(pictureBox1.Bounds.Width), rand.Next(pictureBox1.Bounds.Height), pictureBox1)); }
            else
            { MessageBox.Show(ERROR_MESSAGE); }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AddBall();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            balls.Clear();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void BouncingBallMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            foreach(Ball b in balls)
            { b.Paint(e.Graphics); }
            label1.Text = balls.Count.ToString();
        }
    }
    public class Ball
    {
        public int dx { get; set; }
        public int dy { get; set; }
        public int speedx { get; set; }
        public int speedy { get; set; }
        public int size { get; set; }
        public SolidBrush solid;
        private PictureBox picturebox1;
        private Random randonGen;
        private const int UPPER_RANGE_V = 2;
        private const double EDGE1 = 1.5;
        private const double EDGE2 = 1.5;
        private const double EDGE3 = 1.5;
        //constructor 
        public Ball(int dx, int dy, PictureBox p)
        {
            this.dx = dx;
            this.dy = dy;
            randonGen = new Random();
            RandomSpeedAndDirection();
            RandomSize();
            this.picturebox1 = p;
            solid = new SolidBrush(CreateRandomColor());
        }
        //球的大小就是30
        private void RandomSize()
        {
            size = randonGen.Next(30, 30);
        }
        //隨機方向還有位置
        private void RandomSpeedAndDirection()
        {
            speedx = randonGen.Next(5, 5);
            speedy = speedx;
            if (randonGen.Next(0, UPPER_RANGE_V) == 0)
            { speedx *= -1; }

            if (randonGen.Next(0, UPPER_RANGE_V) == 0)
            { speedy *= -1; }
        }
        //球球跑動//x 會往x  speedx這樣的速度
        public void Go()
        {
            Checkdx();//檢測碰壁
            Checkdy();//檢測碰壁
            dx += speedx;
            dy += speedy;
        }
        //撞牆檢測
        private void Checkdx()
        {
            if (picturebox1.Bounds.Right < dx + size * EDGE1 + 3)
            { speedx *= -1; }

            if (picturebox1.Bounds.Left > dx + size - EDGE2)
            { speedx *= -1; }
        }
        private void Checkdy()
        {
            if (picturebox1.Bounds.Bottom < dy + size * EDGE3)
            { speedy *= -1; }

            if (picturebox1.Bounds.Top > dy + size)
            { speedy *= -1; }
        }
        
        //creates random color
        private Color CreateRandomColor()
        {
            //隨機給定球的顏色
            Color randomColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255), randonGen.Next(255));
            return randomColor;
        }
        public void Paint(Graphics c)
        {
            //用畫圖工具中的橢圓工具畫橢圓
            c.FillEllipse(solid, dx, dy, size, size);
        }
        //檢測碰撞
        public bool CollidesWith(Ball anotherBall)
        {
            // Get differences of coords
            int dx = this.dx - anotherBall.dx;
            int dy = this.dy - anotherBall.dy;

            // Euclidean Distance between the balls
            double dist = Math.Sqrt((double)(dx * dx) + (double)(dy * dy));

            // If distance is less than the diameter, the balls are touching
            return (dist < (double)(size + 2));
        }
        public void MoveHorizontal(int dx)
        {
            this.dx += dx;
        }
        public void MoveVertical(int dy)
        {
            this.dy += dy;
        }
    }
}
