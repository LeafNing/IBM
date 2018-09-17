using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDAG
{
    class FiboHeap
    {
        int n;
        int total;
        int[] degree;
        int[] p;
        int[] child;
        int[] left;
        int[] right;
        bool[] mark;
        int[] A;
        public int[] list;

        public double[] data;
        int D;
        int nroot;
        public int len;
        public int max;


        public FiboHeap(int n)
        {
            total = n;
            nroot = 0;
            degree = new int[n];
            p = new int[n];
            child = new int[n];
            left = new int[n];
            right = new int[n];
            mark = new bool[n];
            data = new double[n];
            max = -1;
            //       list = new int[n];
            A = new int[n];
            this.n = 0;
            D = (int)(Math.Log(n) / Math.Log((Math.Sqrt(5.0) + 1.0) / 2.0)) + 1;
        }

        void insertNextto(int x, int y)
        {
            left[x] = left[y];
            right[left[x]] = x;
            right[x] = y;
            left[y] = x;
        }


        public void insert(int key, double value)
        {
            degree[key] = 0;
            p[key] = -1;
            child[key] = -1;
            left[key] = key;
            right[key] = key;
            mark[key] = false;
            data[key] = value;

            if (max == -1)
            {
                max = key;
            }
            else
                insertNextto(key, max);

            if (value > data[max])
                max = key;

            n++;
            //       list[len++] = key;
            nroot++;
        }

        void consolidate()
        {
            for (int i = 0; i < D; i++)
                A[i] = -1;

            int used = 0;
            int w = max;
            while (used < nroot)
            {
                used++;
                int x = w;
                w = right[w];
                int d = degree[x];
                while (A[d] != -1)
                {
                    int y = A[d];
                    if (data[x] < data[y])
                    {
                        int temp = x;
                        x = y;
                        y = temp;
                    }

                    //if (y == max)
                    //   max = left[max];
                    //remove y
                    if (right[y] == y)
                        max = -1;
                    else
                    {
                        right[left[y]] = right[y];
                        left[right[y]] = left[y];
                    }

                    //let y child of x
                    p[y] = x;
                    if (child[x] == -1)
                    {
                        child[x] = y;
                        left[y] = y;
                        right[y] = y;
                    }
                    else
                    {
                        left[y] = left[child[x]];
                        right[left[y]] = y;
                        left[child[x]] = y;
                        right[y] = child[x];
                    }

                    degree[x]++;

                    mark[y] = false;

                    A[d] = -1;
                    d++;
                }
                A[d] = x;

            }

            max = -1;
            nroot = 0;
            for (int i = 0; i < D; i++)
            {
                if (A[i] != -1)
                {
                    int y = A[i];
                    if (max == -1)
                    {
                        max = y;
                        left[y] = y;
                        right[y] = y;
                    }
                    else
                    {
                        left[y] = left[max];
                        right[left[y]] = y;
                        left[max] = y;
                        right[y] = max;
                    }
                    if (max == -1 || data[y] > data[max])
                        max = y;
                    nroot++;
                }
            }
        }

        void clear(int x)
        {
            data[x] = 0;
            int w = child[x];
            if (w == -1)
                return;
            do
            {
                clear(w);
                w = right[w];
            } while (w != child[x]);
        }

        public void reInit()
        {
            //for (int i = 0; i < len; i++)
            //    data[list[i]] = 0;
            //len = 0;
            //max = -1;
            //nroot = 0;

            if (max == -1)
                return;
            int w = max;
            do
            {
                clear(w);
                w = right[w];
            } while (w != max);
            max = -1;
            nroot = 0;
            n = 0;
        }

        public void delete()
        {
            int z = max;
            if (z != -1)
            {
                data[z] = -1;
                int x = child[z];
                while (degree[z] > 0)
                {
                    int next = right[x];

                    //                    insertNextto(x, max);
                    left[x] = left[max];
                    right[left[x]] = x;
                    left[max] = x;
                    right[x] = max;

                    p[x] = -1;

                    x = next;
                    degree[z]--;
                    if (degree[z] < 0)
                        Console.WriteLine(1);
                    nroot++;
                }

                if (z == right[z])
                {
                    max = -1;
                    nroot = 0;
                }
                else
                {
                    right[left[z]] = right[z];
                    left[right[z]] = left[z];
                    max = right[z];
                    nroot--;
                    consolidate();
                }

                n--;
            }
        }

        void cut(int x, int y)
        {
            if (right[x] == x)
                child[y] = -1;
            else
            {
                child[y] = right[x];
                left[right[x]] = left[x];
                right[left[x]] = right[x];
            }

            degree[y]--;
            if (degree[y] < 0)
                Console.WriteLine(2);

            left[x] = left[max];
            right[left[x]] = x;
            right[x] = max;
            left[max] = x;

            nroot++;

            p[x] = -1;
            mark[x] = false;
        }

        void cascading_cut(int y)
        {
            int z = p[y];
            if (z != -1)
            {
                if (!mark[y])
                {
                    mark[y] = true;
                }
                else
                {
                    cut(y, z);
                    cascading_cut(z);
                }
            }
        }

        public void update(int x, double k)
        {
            if (data[x] <= 0)
                insert(x, k);
            else
            {
                data[x] = k;
                int y = p[x];
                if (y != -1 && data[x] > data[y])
                {
                    cut(x, y);
                    cascading_cut(y);
                }
            }

            if (data[x] > data[max])
                max = x;
        }
    }
}
