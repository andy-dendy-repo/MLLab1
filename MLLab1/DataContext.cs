using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLLab1
{
    internal class DataContext
    {
        public List<(int Number, List<double> Values)> Data { get; init; }

        public string[] Columns { get; init; }

        public int PointsAmout { get; init; }

        public int Dimension { get; init; }

        public DataContext(List<(int Number, List<double> Values)> data, string[] columns)
        {
            Data = data.OrderBy(x=>x.Number).ToList();
            Columns = columns;
            PointsAmout = Data.Count;
            Dimension = Data[0].Values.Count;
        }

        private double GetDistance(int i, int j)
        {
            var pointI = Data[i];
            var pointJ = Data[j];

            double distance = 0;

            for (int d = 0; d < Dimension; d++)
            {
                distance += Math.Pow(pointI.Values[d] - pointJ.Values[d], 2);
            }

            distance = Math.Sqrt(distance);

            return distance;
        }

        public DistanceMatrix ToDistanceMatrix()
        {
            double[,] distances = new double[PointsAmout, PointsAmout];

            for (int i = 0; i < PointsAmout; i++)
            {
                for (int j = 0; j < PointsAmout; j++)
                {
                    distances[i, j] = Math.Round(GetDistance(i,j),3);
                }
            }

            return new DistanceMatrix(distances, PointsAmout);
        }
    }
}
