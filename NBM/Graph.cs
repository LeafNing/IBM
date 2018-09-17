using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace LDAG
{
    struct Edge
    {
        public int u, v, c;
        public double w1, w2;
        public double w11, w22;
    };

    class Graph
    {
        public int n, m;
        public int[] index;
        public Edge[] edges;
        public DAG[] inDAG;
        public double[] IncInf;
        public int[] N0;

        void qsort_edges(int h, int t)
        {
            if (h < t)
            {
                int i = h, j = t;
                Edge mid = edges[(i + j) / 2];
                edges[(i + j) / 2] = edges[i];

                while (i < j)
                {
                    while ((i < j) && ((edges[j].u > mid.u) || ((edges[j].u == mid.u) && (edges[j].v > mid.v))))
                        j--;
                    if (i < j)
                    {
                        edges[i] = edges[j];
                        i++;
                    }
                    while ((i < j) && ((edges[i].u < mid.u) || ((edges[i].u == mid.u) && (edges[i].v < mid.v))))
                        i++;
                    if (i < j)
                    {
                        edges[j] = edges[i];
                        j--;
                    }
                }

                edges[i] = mid;
                qsort_edges(h, i - 1);
                qsort_edges(i + 1, t);
            }

        }
        BitArray isInN0;
        ArrayList nei;
        double[] ap_nei;

        public void Load(string filename)
        {
            
            StreamReader sr = new StreamReader(filename);
            string line = sr.ReadLine();

            string[] data = line.Split(' ');

            n = int.Parse(data[0]);
            m = int.Parse(data[1]);
            isInN0 = new BitArray(n, false);

            edges = new Edge[2 * m];

            for (int i = 0; i < m; i++)
            {
                line = sr.ReadLine();
                if (line == "</graph>") break;
                data = line.Split(' ');
                edges[i].u = int.Parse(data[0]);
                edges[i].v = int.Parse(data[1]);
                edges[i + m].u = edges[i].v;
                edges[i + m].v = edges[i].u;
                edges[i].w1 = double.Parse(data[2]);
                edges[i].w2 = double.Parse(data[3]);
                edges[i + m].w1 = edges[i].w2;
                edges[i + m].w2 = edges[i].w1;
                edges[i].w11 = edges[i].w1;
                edges[i].w22 = edges[i].w2;
                edges[i + m].w11 = edges[i].w22;
                edges[i + m].w22 = edges[i].w11;
                edges[i].c = 1;
                edges[i + m].c = 1;
            }

            qsort_edges(0, 2 * m - 1);

            int m1 = 0;
            for (int i = 1; i < 2 * m; i++)
            {
                if ((edges[i].u != edges[m1].u) || (edges[i].v != edges[m1].v))
                {
                    m1++;
                    edges[m1] = edges[i];
                }
                else
                {
                    edges[m1].w1 += edges[i].w1;
                    edges[m1].w2 += edges[i].w2;
                    edges[m1].c++;
                }
            }
            if (m != 0)
                m = m1 + 1;

            index = new int[n];
            for (int i = 0; i < n; i++)
                index[i] = 0;
            for (int i = 0; i < m; i++)
                index[edges[i].u] = i;
            for (int i = 1; i < n; i++)
                if (index[i] < index[i - 1])
                    index[i] = index[i - 1];

            //StreamReader sr1 = new StreamReader("negativeSeed.txt");
            //line = sr1.ReadLine();
            //int k_seeds = int.Parse(line);
            //Console.WriteLine("number of negative seeds:" + k_seeds);
            //N0 = new int[k_seeds];
            //for (int i = 0; i < k_seeds; i++)
            //{
            //    line = sr1.ReadLine();
            //    N0[i] = int.Parse(line);
            //    isInN0[N0[i]] = true;
            //}
            StreamWriter sw = File.CreateText("negativeSeed.txt");
            Random ro = new Random();
            int k_seeds = 50;
            N0 = new int[k_seeds];
            //int tmp;
            sw.WriteLine(k_seeds);
            for (int i = 0; i < k_seeds; i++)
            {
                N0[i] = ro.Next(n);
                isInN0[N0[i]] = true;
                sw.WriteLine(N0[i]);

            }
            sw.Close();
            Console.WriteLine("seeds done...");
            ap_nei = new double[n];
            for (int i = 0; i < n; i++)
                ap_nei[i] = 0.0;
            for (int i = 0; i < m; i++)
            {
                if (isInN0[edges[i].u])
                    ap_nei[edges[i].v] += edges[i].w1;
            }
        }

        public int GetNeighbor(int node)
        {
            if (node == 0)
                return index[node] + 1;
            else
                return index[node] - index[node - 1];
        }

        Edge GetEdge(int node, int idx)
        {
            if (node == 0)
                return edges[idx];
            else
                return edges[index[node - 1] + 1 + idx];
        }

        ArrayList[] outDAG;

        public void generateDAG(double theta)
        {
            inDAG = new DAG[n];
            IncInf = new double[n];
            PriorityQueue queue = new PriorityQueue(n, 0);
            int[] childlist = new int[n];
            double[] incfac = new double[n];

            outDAG = new ArrayList[n];
            for (int u = 0; u < n; u++)
                outDAG[u] = new ArrayList();

            double influence;
            int w;
            int count = 0;
            for (int u = 0; u < n; u++)
            {
                count = 0;
                influence = 1;
                w = u;

                queue.reInit();
                queue.index[w] = -2;

                inDAG[u] = new DAG();

                while (true)
                {
                    childlist[count++] = w;
                    incfac[count - 1] = influence;

                    outDAG[w].Add(u);
                    inDAG[u].InEdge.Add(w, new ArrayList());

                    for (int j = 0; j < GetNeighbor(w); j++)
                    {
                        Edge e = GetEdge(w, j);
                        int v = e.v;
                        if (queue.index[v] == -2 && v != w && e.w1 > 0)
                            inDAG[u].InEdge[v].Add(e);
                        else if (queue.index[v] != -2 && e.w2 > 0)
                        {
                            queue.update(v, queue.data[v] + influence * e.w2);

                        }
                    }

                    if (queue.empty()) break;
                    influence = queue.data[queue.heap[0]];



                    if (influence <= theta) break;
                    w = queue.pop();
                }

                for (int i = 0; i < count; i++)
                {
                    int child = childlist[i];
                    queue.index[child] = -1;
                }

                inDAG[u].t = count;
                inDAG[u].Nodes = new int[count];
                for (int i = 0; i < count; i++)
                {
                    inDAG[u].Nodes[i] = childlist[i];
                    inDAG[u].IncFac.Add(childlist[i], incfac[i]);
                    inDAG[u].Actual.Add(childlist[i], 0);

                    IncInf[childlist[i]] += incfac[i];

                }
            }
        }
        ArrayList[] outLS;
        
        public void generate_outLS()
        {
            outLS = new ArrayList[n];
            for (int i = 0; i < n; i++)
            {
                outLS[i] = new ArrayList();
                //ap_nei[i] = 0.0;
            }
                
            bool flag;
            for (int s = 0; s < n; s++)
            {
                for (int i = 0; i < GetNeighbor(s); i++)
                {
                    Edge e = GetEdge(s, i);
                    int v = e.v;
                    flag = false;
                    foreach (int u in inDAG[v].Nodes)
                    {
                        if (u == s)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        outLS[s].Add(v);
                        
                }
            }
        }

        double[] ap_p;
        double[] ap_n;
        double[] P_p;
        double[] P_n;
        
        
        public void init(ArrayList S)
        {
            
            ap_p = new double[n];
            ap_n = new double[n];
            P_p = new double[n];
            P_n = new double[n];
            for (int v = 0; v < n; v++)
            {
                ap_p[v] = 0;
                ap_n[v] = 0;
                P_p[v] = 0;
                P_n[v] = 0;
            }
            foreach (int v in S)
            {
                ap_p[v] = 1;
            }
            foreach (int v in N0)
            {
                ap_n[v] = 1;
            }
        }



        public double updateIncInf(int v, ArrayList S)
        {
            BitArray isInS;
            isInS = new BitArray(n, false);
            //Dictionary<int, ArrayList> positiveQ = new Dictionary<int, ArrayList>();
            //Dictionary<int, ArrayList> negativeQ = new Dictionary<int, ArrayList>();
            ArrayList positiveQ = new ArrayList();
            ArrayList negativeQ = new ArrayList();
            Queue myqueue_p = new Queue();
            Queue myqueue_n = new Queue();
            myqueue_p.Clear();
            myqueue_n.Clear();
            double ap = 0;

            double[] sum_p_n = new double[n];
            double[] sum_p_p = new double[n];

            foreach (int s in S)
                isInS[s] = true;

            foreach (int v1 in inDAG[v].Nodes)
            {
                if (isInS[v1])
                {
                    positiveQ.Add(v1);
                    myqueue_p.Enqueue(v1);
                }

            }

            foreach (int v1 in inDAG[v].Nodes)
            {
                if (isInN0[v1])
                {
                    negativeQ.Add(v1);
                    myqueue_n.Enqueue(v1);
                }
            }
            init(S);

            for (int i = 0; i < n; i++)
            {
                sum_p_n[i] = 0.0;
                sum_p_p[i] = 0.0;
            }

            BitArray isIn_negative, isIn_positive;
            while (myqueue_p.Count != 0 || myqueue_n.Count != 0)
            {
                ap += ap_p[v];
                isIn_negative = new BitArray(n, false);
                isIn_positive = new BitArray(n, false);
                foreach (int u in positiveQ)
                {
                    foreach (int x in inDAG[v].Nodes)
                    {
                        if (!isInN0[x] && !isInS[x])
                        {
                            foreach (Edge e in inDAG[v].InEdge[x])
                            {
                                if (e.u == u)
                                {
                                    sum_p_n[x] += P_n[x];
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < n; i++)
                    P_n[i] = 0;

                //negative
                foreach (int u in negativeQ)
                {
                    foreach (int x in inDAG[v].Nodes)
                    {
                        if (!isInN0[x] && !isInS[x] && !isIn_negative[x])
                        {

                            foreach (Edge e in inDAG[v].InEdge[x])
                            {
                                if (e.u == u)
                                {
                                    sum_p_p[x] += P_p[x];
                                    myqueue_n.Enqueue(x);
                                    isIn_negative[x] = true;
                                    P_n[x] += ap_n[u] * e.w1;
                                    break;
                                }
                            }
                        }
                    }
                    myqueue_n.Dequeue();
                }
                foreach (int x in myqueue_n)
                {
                    ap_n[x] = P_n[x] * (1 - sum_p_p[x]);
                }

                for (int i = 0; i < n; i++)
                    P_p[i] = 0;

                //positive
                foreach (int u in positiveQ)
                {
                    foreach (int x in inDAG[v].Nodes)
                    {
                        if (!isInN0[x] && !isInS[x] && !isIn_positive[x])
                        {
                            foreach (Edge e in inDAG[v].InEdge[x])
                            {
                                if (e.u == u)
                                {
                                    //sum_p_n[x] += P_n[x];
                                    myqueue_p.Enqueue(x);
                                    isIn_positive[x] = true;
                                    P_p[x] += ap_p[u] * e.w1;
                                    break;
                                }
                            }
                        }
                    }
                    myqueue_p.Dequeue();
                }
                foreach (int x in myqueue_p)
                {
                    sum_p_n[x] += P_n[x];
                    ap_p[x] = P_p[x] * (1 - sum_p_n[x]);
                }
                positiveQ.Clear();
                negativeQ.Clear();
                foreach (int x in myqueue_p)
                    positiveQ.Add(x);
                //Console.WriteLine("size of positiveQ: " + positiveQ.Count);
                foreach (int x in myqueue_n)
                    negativeQ.Add(x);
                //Console.WriteLine("size of negativeQ: " + negativeQ.Count);
            }
            return ap;
        }
        ArrayList Seeds;
        double[] value;

        
        public void choose_neighbor(int k)
        {
            value = new double[k];
            generate_outLS();
            nei = new ArrayList();
            int[] flag = new int[n];
            for (int i = 0; i < n; i++)
            {
                flag[i] = 0;
            }
            foreach (int n0 in N0)
            {
                foreach (int v in outLS[n0])
                {
                    if (flag[v] == 0)
                    {
                        nei.Add(v);
                        flag[v] = 1;
                    }
                }
            }
            Console.WriteLine("generate outLS done...");

            double tmp0, tmp1, tmp;
            Seeds = new ArrayList();
            ArrayList tmpS = new ArrayList();
            PriorityQueue myqueue = new PriorityQueue(n, 0);
            double[] IncInf = new double[n];
            BitArray isInNei = new BitArray(n, false);
            foreach(int v in nei)
            {
                IncInf[v] = 0;
                isInNei[v] = true;
                myqueue.push(v, IncInf[v]);
            }
            Console.WriteLine("number of neighbors: " + nei.Count);
            if (nei.Count <= k)
            {
                for (int i = 0; i < nei.Count; i++ )
                {
                    Seeds.Add(nei[i]);
                    Console.WriteLine(i + " seed add done...");
                }
            }
            else
            {
                DateTime start = DateTime.Now;
                for (int v = 0; v < n; v++)
                {
                    tmpS.Clear();
                    tmp0 = updateIncInf(v, tmpS);
                    if (!isInN0[v])
                    {
                        foreach (int u in inDAG[v].Nodes)
                        {
                            if (isInNei[u])
                            {
                                tmpS.Clear();
                                tmpS.Add(u);
                                tmp1 = updateIncInf(v, tmpS);
                                tmp = tmp1 - tmp0;
                                IncInf[u] += tmp * ap_nei[u];
                                //Console.WriteLine("decInf = " + decInf[u]);
                                myqueue.update(u, IncInf[u]);
                            }
                        }
                        Console.WriteLine("v = " + v);
                    }
                }
                Console.WriteLine("init done...");
                TimeSpan ts = DateTime.Now.Subtract(start);
                double time = ts.TotalSeconds;
                Console.WriteLine("initial time = " + time);
                int s;
                double va;
                BitArray isIn = new BitArray(n, false);
                for (int i = 0; i < k; i++)
                {
                    va = myqueue.data[myqueue.heap[0]];
                    Console.WriteLine("value = " + va);
                    s = myqueue.pop();
                    //Console.WriteLine("pop done...");
                    isIn[s] = true;
                    //Console.WriteLine("myqueue count = " + myqueue.count);
                    //myqueue.deleta(s);
                    //isInS[s] = true;
                    foreach (int v in outLS[s])
                    {
                        tmpS.Clear();
                        foreach (int t in Seeds)
                        {
                            tmpS.Add(t);
                        }
                        tmp0 = updateIncInf(v, tmpS);
                        foreach (int u in inDAG[v].Nodes)
                        {
                            if (!isIn[u] && isInNei[u])
                            {
                                tmpS.Add(u);
                                tmp1 = updateIncInf(v, tmpS);
                                tmp = tmp1 - tmp0;
                                IncInf[u] -= tmp * ap_nei[u];
                                tmpS.Add(s);
                                tmp1 = updateIncInf(v, tmpS);
                                tmpS.Remove(u);
                                tmp0 = updateIncInf(v, tmpS);
                                tmp = tmp1 - tmp0;
                                IncInf[u] += tmp * ap_nei[u];
                                myqueue.update(u, IncInf[u]);
                            }
                        }
                    }
                    Seeds.Add(s);
                    value[i] = va;
                    Console.WriteLine(Seeds[i] + "   " + value[i]);
                    Console.WriteLine(i + " seed add done...");
                }
            }
        }



        public void saveSeeds(string filename)
        {
            StreamWriter sw = File.CreateText(filename);
            foreach (int s in Seeds)
            {
                sw.WriteLine(s);
            }
            sw.Close();
        }
        public void saveNei(string filename)
        {
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine(nei.Count);
            foreach (int v in nei)
            {
                sw.WriteLine(v);
            }
            sw.Close();
        }
    }
}
