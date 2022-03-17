using System;
using System.Collections.Generic;
using System.Linq;

namespace ScanLineDemo
{
    public class Program
    {
        const long MAXN = 210;  //记录矩形数量        
        static List<long> X = new List<long>();//记录矩形边的左下和右上x轴坐标
        static List<ScanLine> scanLine = new List<ScanLine>();//记录扫描线数据
        static SegTree[] segTree = new SegTree[MAXN << 2];//存储线段，segTree[x]存储第x个节点的[L,R]区间的数据，其中segTree[x].lson=L,segTree[x].lson=R

        static void Main(string[] args)
        {
            Console.WriteLine("请输入矩形数量：");
            var n=int.Parse(Console.ReadLine());
            for (int i = 1; i <= n; i++)
            {
                Console.WriteLine($"请输入第{i}矩形的角点坐标：（以空格结束每个坐标的输入）");
                var str = Console.ReadLine().Split(' ');
                X.Add(long.Parse(str[0]));
                X.Add(long.Parse(str[2]));
                scanLine.Add(new ScanLine
                {
                    l = long.Parse(str[0]),
                    r = long.Parse(str[2]),
                    h = long.Parse(str[1]),
                    mark = 1
                });
                scanLine.Add(new ScanLine
                {
                    l = long.Parse(str[0]),
                    r = long.Parse(str[2]),
                    h = long.Parse(str[3]),
                    mark = -1
                });
            }
            X=X.Distinct().OrderBy(a=>a).ToList();
            scanLine=scanLine.OrderBy(a=>a.h).ToList();

            var iCount = X.Count;
            BuildTree(1, 1, iCount-1);
            long ans = 0;
            for (int i = 0; i < 2*n-1; i++)
            {
                UpDateTree(1, scanLine[i].l, scanLine[i].r, scanLine[i].mark);
                ans += segTree[1].len * (scanLine[i+1].h - scanLine[i].h);
            }
            Console.WriteLine(ans);
        }

        /// <summary>
        /// 创建线段树
        /// </summary>
        /// <param name="k">树节点号</param>
        /// <param name="l">节点的左儿子</param>
        /// <param name="r">节点的右儿子</param>
        public static void BuildTree(int k, int l, int r)
        {
            segTree[k].l = l;
            segTree[k].r = r;
            segTree[k].len = 0;
            segTree[k].cnt = 0;
            if (l == r)
            {
                return;
            }
            int mid = (l + r) >> 1;
            BuildTree(k << 1, l, mid);
            BuildTree(k << 1 | 1, mid + 1, r);
            return;
        }

        /// <summary>
        /// 维护当前节点线段树的长度
        /// </summary>
        /// <param name="k"></param>
        public static void PushUpTree(int k)
        {
            int l = segTree[k].l;
            int r = segTree[k].r;
            if (segTree[k].cnt!=0)
            {
                segTree[k].len = X[r] - X[l-1];
            }
            else
            {
                segTree[k].len = segTree[k << 1].len + segTree[k << 1 | 1].len;
            }
        }

        public static void UpDateTree(int k, long L, long R, int mark)
        {
            int l = segTree[k].l;
            int r = segTree[k].r;
            if (X[r] <= L || R <= X[l-1])//给定区间[L,R]在当前线段的左侧或右侧，即未覆盖
            {
                return;
            }
            if (L <= X[l-1] && X[r] <= R)//给定区间[L,R]在当前线段的两端，即区间完全覆盖线段
            {
                segTree[k].cnt += mark;
                PushUpTree(k);
                return;
            }
            UpDateTree(k << 1, L, R, mark);
            UpDateTree(k << 1 | 1, L, R, mark);
            PushUpTree(k);
        }
    }

    /// <summary>
    /// 扫描线，平行于x轴的线做为扫描线
    /// </summary>
    public struct ScanLine:IComparable<ScanLine>
    {
        public long l, r; //l和r为线段的左、右端点的X值，
        public long h;    //h为线段的Y值
        public int mark;  //用于保存权值，1代表是下边，-1代表是上边

        public int CompareTo(ScanLine other)
        {
            if (h > other.h)
                return 1;
            else if (h == other.h)
                return 0;
            else
                return -1;
        }

        //public static bool operator <(ScanLine a, ScanLine b)
        //{
        //    return a.h < b.h;
        //}

        //public static bool operator >(ScanLine a, ScanLine b)
        //{
        //    return a.h > b.h;
        //}
    }

    /// <summary>
    /// 线段树
    /// </summary>
    public struct SegTree
    {
        public int l, r;        //l和r为线段的左、右端点的X值，
        public int cnt;         //cnt记录覆盖的次数
        public long len;        //len记录线段的长度
    }
}
