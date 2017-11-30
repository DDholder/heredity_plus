using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace heredity_plus
{
    internal class Triangle
    {
        public Point[] Pols;
        public Brush Bsh;
        public Triangle(Brush b, Point[] ps)
        {
            Pols = ps;
            Bsh = b;
        }
    }

    internal struct Member
    {
        public Triangle[] Tri;
        public Bitmap Bmp;
        public float Grade;
    }

    internal struct Target
    {
        public int R;
        public int G;
        public int B;
    }

    internal class Handler
    {
        private Color _col = Color.White;
        private readonly Point[] _pp = new Point[3];
        private readonly Random _rans = new Random();
        private static readonly object Obj = new object();

        public Triangle DrawAnyTriangle()
        {
            var top0 = _rans.Next(0, 64);
            var top1 = _rans.Next(0, 64);
            var top2 = _rans.Next(0, 64);
            var top3 = _rans.Next(0, 64);
            var top4 = _rans.Next(0, 64);
            var top5 = _rans.Next(0, 64);
            _pp[0] = new Point(top0, top3);
            _pp[1] = new Point(top1, top4);
            _pp[2] = new Point(top2, top5);
            var a = _rans.Next(0, 50);
            var r = _rans.Next(0, 255);
            var g = _rans.Next(0, 255);
            var b = _rans.Next(0, 255);
            _col = Color.FromArgb(a, r, g, b);
            return new Triangle(new SolidBrush(_col), new[] { new Point(top0, top3), new Point(top1, top2), new Point(top4, top5) });
        }
        private float GetAbs(int firstNum, int secondNum)
        {
            var abs = Math.Abs(firstNum - (float)secondNum);
            float result = Math.Max(firstNum, secondNum);
            if (Math.Abs(result) < 0.01)
                result = 1;
            return abs / result;
        }
        public int[] GetHisogram(Bitmap img)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            var histogram = new int[256];
            unsafe
            {
                var ptr = (byte*)data.Scan0;
                var remain = data.Stride - data.Width * 3;
                for (var i = 0; i < histogram.Length; i++)
                    histogram[i] = 0;
                for (var i = 0; i < data.Height; i++)
                {
                    for (var j = 0; j < data.Width; j++)
                    {
                        if (ptr != null)
                        {
                            var mean = ptr[0] + ptr[1] + ptr[2];
                            mean /= 3;
                            histogram[mean]++;
                        }
                        ptr += 3;
                    }
                    ptr += remain;
                }
            }
            img.UnlockBits(data);
            return histogram;
        }
        private float GetResult(IReadOnlyList<int> firstNum, IReadOnlyList<int> scondNum)
        {
            if (firstNum.Count != scondNum.Count)return 0;
            float result = 0;
            var j = firstNum.Count;
            for (var i = 0; i < j; i++)
            {
                result += 1 - GetAbs(firstNum[i], scondNum[i]);
            }
            return result / j;
        }

        public float Test(int[] tarh, Bitmap bmp, Target[,] tar)
        {

            var bitmapt = new Bitmap(bmp, 256, 256);
            var grade = (1 - GetResult(tarh, GetHisogram(bitmapt)))*30000;
            float tempgrade = 0;
            for (var x = 0; x < 256; x += 8)
            {

                for (var y = 0; y < 256; y += 8)
                {
                    lock (bmp)
                    {
                        int r = bitmapt.GetPixel(x, y).R, g = bitmapt.GetPixel(x, y).G, b = bitmapt.GetPixel(x, y).B;
                        if (r - tar[x, y].R < 0) tempgrade -= r - tar[x, y].R;
                        else tempgrade += r - tar[x, y].R;
                        if (g - tar[x, y].G < 0) tempgrade -= g - tar[x, y].G;
                        else tempgrade += g - tar[x, y].G;
                        if (b - tar[x, y].B < 0) tempgrade -= b - tar[x, y].B;
                        else tempgrade += b - tar[x, y].B;
                    }
                }
            }
            grade += tempgrade;
            return grade;
        }
        public void Queue(Member[] member)
        {
            for (var i = 0; i < member.Length; i++)
            {
                for (var j = 0; j < member.Length - 1 - i; j++)
                {
                    if (!(member[j].Grade > member[j + 1].Grade)) continue;
                    var temp = member[j];
                    member[j] = member[j + 1];
                    member[j + 1] = temp;
                }
            }
        }
        public Bitmap DrawTriangles(Triangle[] triangle)
        {
            var bitmap = new Bitmap(64, 64);
            var g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            lock (Obj)
            {
                for (var i = 0; i < 100; i++)
                {
                    g.FillPolygon(triangle[i].Bsh, triangle[i].Pols);
                }
            }
            g.DrawImage(bitmap, 0, 0);
            return bitmap;
        }
        public void Multiply(Member[] member)
        {
            for (var index = 1; index < 7; index++)
            {
                for (var i = 1; i < 15; i += 2)
                {
                    for (var j = 0; j < 50; j++)
                    {
                        if (15 * index + i + 1 >= 100) break;
                        member[15 * index + i].Tri[j] = member[i].Tri[j];
                        member[15 * index + i].Tri[50 + j] = member[i + 1].Tri[50 + j];
                        member[15 * index + i + 1].Tri[j] = member[i + 1].Tri[j];
                        member[15 * index + i + 1].Tri[50 + j] = member[i].Tri[50 + j];
                    }
                }
            }
        }
        public void DifferentHandler(Member[] member, int numOffset, int times)
        {
            for (var i = numOffset; i < 100; i++)
            {
                for (var j = 0; j < times; j++)
                {
                    member[i].Tri[_rans.Next(0, 100)] = DrawAnyTriangle();
                }
            }
        }
    }
}
