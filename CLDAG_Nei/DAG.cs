using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace LDAG
{
    class DAG
    {
        public int t = 0;
        public int[] Nodes;
        public Dictionary<int, ArrayList> InEdge = new Dictionary<int, ArrayList>();
        public Dictionary<int, double> Actual = new Dictionary<int, double>();
        public Dictionary<int, double> IncFac = new Dictionary<int, double>();
        //public Dictionary<int, ArrayList> OutEdge = new Dictionary<int, ArrayList>();
        //public Dictionary<int, double> OldActual = new Dictionary<int,double>();
        //public Dictionary<int, double> OldIncFac = new Dictionary<int, double>();
    }
}
