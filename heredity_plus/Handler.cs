using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace heredity_plus
{
    class Triangle
    {
        public Point[] pols;
        public Brush bsh;
        public Triangle(Brush b, Point[] ps)
        {
            pols = ps;
            bsh = b;
        }
    }
    struct Member
    {
        public Triangle[] tri;
        public Bitmap bmp;
        public float grade;
    }
    struct Target
    {
        public int R;
        public int G;
        public int B;
    }
    class Handler
    {
        Color col = Color.White;
        Point[] pp = new Point[3];
        Random rans = new Random();
        private static readonly object obj = new object();
        private static readonly object obj2 = new object();
        public Triangle DrawAnyTriangle()
        {
            int top0 = rans.Next(0, 64);
            int top1 = rans.Next(0, 64);
            int top2 = rans.Next(0, 64);
            int top3 = rans.Next(0, 64);
            int top4 = rans.Next(0, 64);
            int top5 = rans.Next(0, 64);
            pp[0] = new Point(top0, top3);
            pp[1] = new Point(top1, top4);
            pp[2] = new Point(top2, top5);
            int A = rans.Next(0, 50);
            int R = rans.Next(0, 255);
            int G = rans.Next(0, 255);
            int B = rans.Next(0, 255);
            col = Color.FromArgb(A, R, G, B);
            return new Triangle(new SolidBrush(col), new Point[] { new Point(top0, top3), new Point(top1, top2), new Point(top4, top5) });
        }
        private float GetAbs(int firstNum, int secondNum)
        {
            float abs = Math.Abs((float)firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (result == 0)
                result = 1;
            return abs / result;
        }
        public int[] GetHisogram(Bitmap img)
        {
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int[] histogram = new int[256];
            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                int remain = data.Stride - data.Width * 3;
                for (int i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (int i = 0; i < data.Height; i++)
                {
                    for (int j = 0; j < data.Width; j++)
                    {
                        int mean = ptr[0] + ptr[1] + ptr[2];
                        mean /= 3;
                        histogram[mean]++;
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }
        private float GetResult(int[] firstNum, int[] scondNum)
        {
            if (firstNum.Length != scondNum.Length)
            {
                return 0;
            }
            else
            {
                float result = 0;
                int j = firstNum.Length;
                for (int i = 0; i < j; i++)
                {
                    result += 1 - GetAbs(firstNum[i], scondNum[i]);
                }
                return result / j;
            }
        }

        public float Test(int[] tarh, Bitmap bmp, Target[,] tar)
        {

            Bitmap bitmapt = new Bitmap(bmp, 256, 256);
            float grade = (1 - GetResult(tarh, GetHisogram(bitmapt)))*10000;
            float tempgrade = 0;
            for (int x = 0; x < 64; x += 2)
            {

                for (int y = 0; y < 64; y += 2)
                {
                    lock (bmp)
                    {
                        int R = bmp.GetPixel(x, y).R, G = bmp.GetPixel(x, y).G, B = bmp.GetPixel(x, y).B;
                        if (R - tar[x, y].R < 0) tempgrade -= R - tar[x, y].R;
                        else tempgrade += R - tar[x, y].R;
                        if (G - tar[x, y].G < 0) tempgrade -= G - tar[x, y].G;
                        else tempgrade += G - tar[x, y].G;
                        if (B - tar[x, y].B < 0) tempgrade -= B - tar[x, y].B;
                        else tempgrade += B - tar[x, y].B;
                    }
                }
            }
            grade += tempgrade;
            return grade;
        }
        public void Queue(Member[] member)
        {
            Member temp;
            for (int i = 0; i < member.Length; i++)
            {
                for (int j = 0; j < member.Length - 1 - i; j++)
                {
                    if (member[j].grade > member[j + 1].grade)
                    {
                        temp = member[j];
                        member[j] = member[j + 1];
                        member[j + 1] = temp;
                    }
                }
            }
        }
        public Bitmap DrawTriangles(Triangle[] triangle)
        {
            Bitmap bitmap = new Bitmap(64, 64);
            Graphics g;
            g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            lock (obj)
            {
                for (int i = 0; i < 100; i++)
                {
                    g.FillPolygon(triangle[i].bsh, triangle[i].pols);
                }
            }
            g.DrawImage(bitmap, 0, 0);
            return bitmap;
        }
        public void Multiply(Member[] member)
        {
            for (int index = 1; index < 7; index++)
            {
                for (int i = 1; i < 15; i += 2)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        if (15 * index + i + 1 >= 100) break;
                        member[15 * index + i].tri[j] = member[i].tri[j];
                        member[15 * index + i].tri[50 + j] = member[i + 1].tri[50 + j];
                        member[15 * index + i + 1].tri[j] = member[i + 1].tri[j];
                        member[15 * index + i + 1].tri[50 + j] = member[i].tri[50 + j];
                    }
                }
            }
        }
        public void DifferentHandler(Member[] member, int num_offset, int times)
        {
            for (int i = num_offset; i < 100; i++)
            {
                for (int j = 0; j < times; j++)
                {
                    member[i].tri[rans.Next(0, 100)] = DrawAnyTriangle();
                }
            }
        }
    }
}
