using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LDAG
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = args[0];
            string testGraph = filename.Substring(0, filename.LastIndexOf('.'));

            Graph g = new Graph();
            g.Load(filename);
            Console.WriteLine("load over!");

            int theta = int.Parse(args[1]);
            int numOfSeeds = int.Parse(args[2]);
            DateTime start = DateTime.Now;
            g.generateDAG((double)1 / theta);
            Console.WriteLine("generate over!");

            TimeSpan ts1 = DateTime.Now.Subtract(start);
            double time1 = ts1.TotalSeconds;
            Console.WriteLine("Generate DAG time = " + time1);
            //g.choose(numOfSeeds);
            g.choose_neighbor(numOfSeeds);
            //g.choose_neighbor2(numOfSeeds);
            TimeSpan ts2 = DateTime.Now.Subtract(start);
            double time2 = ts2.TotalSeconds;
            Console.WriteLine("Total running time = " + time2);

            Console.WriteLine("saving...");
            StreamWriter sw = new StreamWriter(testGraph + "_" + theta + "_time.txt");
            sw.WriteLine("Generate DAG time = " + time1);
            sw.WriteLine("Total running time = " + time2);
            sw.Close();

            g.saveSeeds(testGraph + "_" + theta + "_positiveSeeds.txt");
            Console.WriteLine("over!");
        }
    }
}
