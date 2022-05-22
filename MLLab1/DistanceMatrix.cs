using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLLab1
{
    internal class DistanceMatrix
    {
        public double[,] Distances;

        public List<Claster> Clasters { get; set; }

        public Minimum Minimum { get; set; }

        public Claster ClasterI { get; set; }

        public Claster ClasterJ { get; set; }

        public int Dimension { get; set; }

        public DistanceMatrix(double[,] distances, int dimension)
        {
            Distances = distances;
            Dimension = dimension;

            Clasters = new int[Dimension]
                .Select((x, y) => new Claster { Index = y, Dimension = 1, Name = (y+1).ToString() })
                .OrderBy(x => x.Index)
                .ToList();
        }

        public DistanceMatrix(List<Claster> clasters, double[,] distances, int dimension)
        {
            Distances = distances;
            Dimension = dimension;

            Clasters = clasters;
        }

        public void FindMinimum()
        {
            Minimum = GetMinimum();

            ClasterI = Clasters.First(x => x.Index == Math.Min(Minimum.I, Minimum.J));
            ClasterI.ContainsMinimum = true;

            ClasterJ = Clasters.First(x => x.Index == Math.Max(Minimum.I, Minimum.J));
            ClasterJ.ContainsMinimum = true;
        }

        public void FindClasters(Method method)
        {
            ClasterI = Clasters.First(x => x.Index == ClasterI.Index);
            ClasterJ = Clasters.First(x => x.Index == ClasterJ.Index);

            int clastersAmount = ClasterI.Dimension + ClasterJ.Dimension;

            for (int i = 0; i < Dimension; i++)
            {
                if (i == ClasterI.Index)
                    continue;

                double newValue = 0;

                switch(method)
                {
                    case Method.Nearest:
                        newValue = Distances[i, ClasterI.Index] / 2 + Distances[i, ClasterJ.Index] / 2 - 
                            Math.Abs(Distances[i, ClasterI.Index] - Distances[i, ClasterJ.Index]) / 2;
                        break;
                    case Method.Distant:
                        newValue = Distances[i, ClasterI.Index] / 2 + Distances[i, ClasterJ.Index] / 2 -
                            Math.Abs(Distances[i, ClasterI.Index] + Distances[i, ClasterJ.Index]) / 2;
                        break;
                    case Method.AvarageGroup:
                        newValue = (double)ClasterI.Dimension / clastersAmount * Distances[i, ClasterI.Index] +
                            (double)ClasterJ.Dimension / clastersAmount * Distances[i, ClasterJ.Index];
                        break;
                }

                Distances[i, ClasterI.Index] = Math.Round(newValue,3);

                Distances[ClasterI.Index, i] = Math.Round(newValue, 3);
            }

            double[,] newDistances = new double[Dimension-1, Dimension - 1];

            int shiftI = 0;
            int shiftJ = 0;

            for (int i = 0; i < Dimension; i++)
            {
                if(i == ClasterJ.Index)
                {
                    shiftI++;
                    continue;
                }
                for (int j = 0; j < Dimension; j++)
                {
                    if(j == ClasterJ.Index)
                    {
                        shiftJ++;
                        continue;
                    }
                    newDistances[i - shiftI, j - shiftJ] = Distances[i, j];
                }
                shiftJ--;
            }

            ClasterI.Dimension++;
            ClasterI.Name += "," + ClasterJ.Name;

            Clasters.Remove(ClasterJ);

            Clasters = Clasters.OrderBy(x => x.Index).ToList();

            foreach(var claster in Clasters.Where(x=>x.Index > ClasterJ.Index))
            {
                claster.Index--;
            }

            Distances = newDistances;
            Dimension--;

            Minimum = null;
            ClasterI = null;
            ClasterJ = null;
        }

        public double GetMaximumValue()
        {
            double maximum = 0;
            int iMin = 0, jMin = 0;

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (maximum < Distances[i, j])
                    {
                        maximum = Distances[i, j];
                        iMin = i;
                        jMin = j;
                    }
                }
            }

            return maximum;
        }

        private Minimum GetMinimum()
        {
            double minimum = -1;
            int iMin = -1, jMin = -1;

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (Distances[i, j] == 0)
                        continue;
                    else if(minimum == -1)
                    {
                        minimum = Distances[i, j];
                        iMin = i;
                        jMin = j;
                    }

                    if (minimum > Distances[i, j])
                    {
                        minimum = Distances[i, j];
                        iMin = i;
                        jMin = j;
                    }
                }
            }

            return new Minimum { Value = minimum, I = iMin, J = jMin};
        }

        public DistanceMatrix Clone()
        {
            double[,] distances = new double[Dimension, Dimension];

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    distances[i, j] = Distances[i, j];
                }
            }

            var clasters = Clasters.Select(x=>x.Clone()).ToList();

            return new DistanceMatrix(clasters, distances, Dimension)
            {
                Dimension = Dimension,
                Minimum = Minimum == null ? null : Minimum.Clone(),
                ClasterI = ClasterI == null ? null : ClasterI.Clone(),
                ClasterJ = ClasterJ == null ? null : ClasterJ.Clone()
            };
        }
    }
}
