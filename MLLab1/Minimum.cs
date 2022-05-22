using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLLab1
{
    internal class Minimum
    {
        public double Value { get; set; }
            
        public int I { get; set; }

        public int J { get; set; }

        public Minimum Clone()
        {
            return new Minimum { Value = Value, I = I, J = J };
        }
    }
}
