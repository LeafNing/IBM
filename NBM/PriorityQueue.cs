using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDAG
{
    public class PriorityQueue
    {
        public int[] heap;
        public int count = 0;
        public double[] data;
        public double theta;

        public int[] index;

        public void up(int n)
        {
            int key = heap[n];
            double temp = data[key];

            int n1;
            while (n > 0)
            {
                n1 = (n - 1) >> 1;
                if (data[heap[n1]] >= temp) break;
                heap[n] = heap[n1];
                index[heap[n]] = n;
                n = n1;
            }
            heap[n] = key;
            index[heap[n]] = n;
        }

        public void down(int n)
        {
            int key = heap[n];
            double temp = data[key];
            while ((n << 1) + 1 < count)
            {
                int n1 = (n << 1) + 1;
                int n2 = n1 + 1;
                int n3 = (n2 < count && data[heap[n2]] > data[heap[n1]]) ? n2 : n1;
                if (data[heap[n3]] <= temp) break;
                heap[n] = heap[n3];
                index[heap[n]] = n;
                n = n3;
            }
            heap[n] = key;
            index[heap[n]] = n;
        }

        public PriorityQueue(int n, double theta)
        {
            heap = new int[n];
            data = new double[n];
            index = new int[n];
            this.theta = theta;
            for (int i = 0; i < n; i++)
            {
                data[i] = theta;
                index[i] = -1;
            }
        }

        public void reInit()
        {
            //for (int i = 0; i < data.Length; i++)
            //{
            //    data[i] = 0;
            //    index[i] = -1;
            //}
            //count = 0;
            for (int i = 0; i < count; i++)
            {
                data[heap[i]] = theta;
                index[heap[i]] = -1;
            }
            count = 0;
        }

        public bool empty()
        {
            return (count == 0);
        }

        public void push(int key, double value)
        {
            data[key] = value;
            //Console.WriteLine("key = " + key + " count = " + count);
            heap[count++] = key;
            index[key] = count - 1;
            up(count - 1);
        }

        public int pop()
        {
            int re = heap[0];
            //            double value = data[re];
            index[re] = -2;
            data[re] = theta;
            heap[0] = heap[--count];
            if (count > 0)
                down(0);
            return re;
        }

        public void deleta(int key)
        {
            int k = index[key];
            index[key] = -2;
            data[key] = theta;
            if (k == count - 1)
            {
                count--;
                return;
            }
            heap[k] = heap[--count];
            if (count > 0)
                down(k);

        }

        public void update(int key, double value)
        {
            if (index[key] < 0)
                push(key, value);
            else
            {
                if (value > data[key])
                {
                    data[key] = value;
                    up(index[key]);
                }
                else
                {
                    data[key] = value;
                    down(index[key]);
                }
            }
        }
    }
}
