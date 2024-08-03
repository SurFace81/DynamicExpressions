using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic_If
{
    internal class Data
    {
        public int a { get; set; }
        public byte b { get; set; }
        public float c { get; set; }
        public UInt16 d { get; set; }
        public string e { get; set; }

        public Data(int A, byte B, float C, UInt16 D, string E)
        {
            a = A; b = B; c = C; d = D; e = E;
        }
    }
}
