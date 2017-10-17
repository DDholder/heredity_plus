using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;

namespace heredity_plus
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Graphics g;
        Handler hhh = new Handler();
        Member[] people = new Member[100];
        Target[,] target = new Target[70, 70];
        int[] tarh = new int[256];
        private void button1_Click(object sender, EventArgs e)
        {
            
            Starts();
            tarh = hhh.GetHisogram(new Bitmap(pictureBox2.Image,256,256));
            Bitmap bmp2 = new Bitmap(pictureBox2.Image);
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    target[x, y].R = bmp2.GetPixel(x, y).R;
                    target[x, y].G = bmp2.GetPixel(x, y).G;
                    target[x, y].B = bmp2.GetPixel(x, y).B;
                }
            }
            backgroundWorker1.RunWorkerAsync();
        }
        void Starts()
        {
            for (int i = 0; i < 100; i++)
            {
                people[i].tri = new Triangle[100];
                people[i].bmp = new Bitmap(64, 64);
                for (int j = 0; j < 100; j++)
                {
                    people[i].tri[j] = hhh.DrawAnyTriangle();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            tarh = hhh.GetHisogram(new Bitmap(pictureBox2.Image));
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int j = 0; j < 10000; j++)
            {
                for (int i = 0; i < 100; i++)
                {

                    people[i].bmp = hhh.DrawTriangles(people[i].tri);
                    people[i].grade = hhh.Test(tarh, people[i].bmp,target);
                }
                hhh.Queue(people);
                hhh.Multiply(people);
                hhh.DifferentHandler(people, 5, 95, 1);
                backgroundWorker1.ReportProgress(j);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pictureBox1.Image = people[0].bmp;
            label1.Text = people[0].grade.ToString();
            label3.Text = e.ProgressPercentage.ToString();
        }
    }


}
