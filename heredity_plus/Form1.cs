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
        Handler[] AsyncHandler = new Handler[5];
        int[] tarh = new int[256];
        private static readonly object obj = new object();
        int selectPeople = 0;
        private void button1_Click(object sender, EventArgs e)
        {

            Starts();
            tarh = hhh.GetHisogram(new Bitmap(pictureBox2.Image, 256, 256));
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
            ThreadPool.SetMaxThreads(10, 10);
            for (int i = 0; i < 5; i++)
            {
                AsyncHandler[i] = new Handler();
            }
            for (int i = 0; i < 20; i++)
            {
                listBox1.Items.Add(0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }
        bool[] bFinishedTest = new bool[5];
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int j = 0; j < 10000; j++)
            {
                //for (int i = 0; i < 100; i++)
                //{
                //    people[i].bmp = hhh.DrawTriangles(people[i].tri);
                //    people[i].grade = hhh.Test(tarh, people[i].bmp, target);
                //}
                for (int i = 0; i < 5; i++)
                {
                    bFinishedTest[i] = false;
                    ThreadPool.QueueUserWorkItem(RunTestAsync, i);
                }
                while (!(bFinishedTest[0] && bFinishedTest[1] && bFinishedTest[2] && bFinishedTest[3] && bFinishedTest[4]))
                {
                    Thread.Sleep(5);
                }
                hhh.Queue(people);
                hhh.Multiply(people);
                hhh.DifferentHandler(people, 5, 1);
                backgroundWorker1.ReportProgress(j);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (people[0].bmp)
            {
                pictureBox1.Image = people[0].bmp;
                label1.Text = e.ProgressPercentage.ToString();
                for (int i = 0; i < 20; i++)
                {
                    listBox1.Items[i] = people[i].grade;
                }
            }
        }
        void RunTestAsync(object index)
        {
            int Index = (int)index;

            for (int i = Index * 20; i < Index * 20 + 20; i++)
            {
                people[i].bmp = AsyncHandler[Index].DrawTriangles(people[i].tri);
                people[i].grade = AsyncHandler[Index].Test(tarh, people[i].bmp, target);
            }
            bFinishedTest[Index] = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            selectPeople = listBox1.SelectedIndex;
            lock (people[selectPeople].bmp)
                pictureBox3.Image = people[selectPeople].bmp;

        }
    }


}
