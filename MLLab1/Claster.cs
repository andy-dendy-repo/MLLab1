using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLLab1
{
    internal class Claster
    {
        public int Index { get; set; }

        public int Dimension { get; set; }

        public string Name { get; set; }

        public Claster Clone()
        {
            return new Claster 
            {
                Dimension = Dimension, 
                Index = Index,
                Name = Name 
            };
        }
    }
}
