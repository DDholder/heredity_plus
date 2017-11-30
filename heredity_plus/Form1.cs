using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace heredity_plus
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private readonly Handler _hhh = new Handler();
        private readonly Member[] _people = new Member[100];
        private readonly Target[,] _target = new Target[70, 70];
        private readonly Handler[] _asyncHandler = new Handler[5];
        private readonly Member[] _history = new Member[10];
        private int[] _tarh = new int[256];
        private readonly int[] _historyIndex = { 1, 10, 30, 50, 70, 100, 200, 500, 1000, 2000 };
        private int _selectPeople;
        private void Button1_Click(object sender, EventArgs e)
        {

            Starts();
            _tarh = _hhh.GetHisogram(new Bitmap(pictureBox2.Image, 256, 256));
            var bmp2 = new Bitmap(pictureBox2.Image);
            for (var x = 0; x < 64; x++)
            {
                for (var y = 0; y < 64; y++)
                {
                    _target[x, y].R = bmp2.GetPixel(x, y).R;
                    _target[x, y].G = bmp2.GetPixel(x, y).G;
                    _target[x, y].B = bmp2.GetPixel(x, y).B;
                }
            }
            backgroundWorker1.RunWorkerAsync();
        }

        private void Starts()
        {
            for (var i = 0; i < 100; i++)
            {
                _people[i].Tri = new Triangle[100];
                _people[i].Bmp = new Bitmap(64, 64);
                for (var j = 0; j < 100; j++)
                {
                    _people[i].Tri[j] = _hhh.DrawAnyTriangle();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            _bFinishedTest[0] = false;
            ThreadPool.SetMaxThreads(10, 10);
            for (var i = 0; i < 5; i++)
            {
                _asyncHandler[i] = new Handler();
            }
            for (var i = 0; i < 20; i++)
            {
                listBox1.Items.Add(0);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
        }

        private readonly bool[] _bFinishedTest = new bool[5];
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (var j = 0; j < 10000; j++)
            {
                for (var i = 0; i < 5; i++)
                {
                    _bFinishedTest[i] = false;
                    ThreadPool.QueueUserWorkItem(RunTestAsync, i);
                }
                while (!(_bFinishedTest[0] && _bFinishedTest[1] && _bFinishedTest[2] && _bFinishedTest[3] && _bFinishedTest[4]))
                {
                    Thread.Sleep(5);
                }
                _hhh.Queue(_people);
                _hhh.Multiply(_people);
                _hhh.DifferentHandler(_people, 5, 1);
                backgroundWorker1.ReportProgress(j);
                for (var i = 0; i < 10; i++)
                {
                    if (j == _historyIndex[i])
                    {
                        _history[i] = _people[0];
                    }
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (_people[0].Bmp)
            {
                pictureBox1.Image = _people[0].Bmp;
                label1.Text = e.ProgressPercentage.ToString();
                for (var i = 0; i < 20; i++)
                {
                    listBox1.Items[i] = _people[i].Grade;
                }
            }
        }
        private void RunTestAsync(object indexs)
        {
            var index = (int)indexs;
            for (var i = index * 20; i < index * 20 + 20; i++)
            {
                _people[i].Bmp = _asyncHandler[index].DrawTriangles(_people[i].Tri);
                _people[i].Grade = _asyncHandler[index].Test(_tarh, _people[i].Bmp, _target);
            }
            _bFinishedTest[index] = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            _selectPeople = listBox1.SelectedIndex;
            lock (_people[_selectPeople].Bmp)
            {
                pictureBox3.Image = _people[_selectPeople].Bmp;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox4.Image = _history[listBox2.SelectedIndex].Bmp;
        }
    }


}
