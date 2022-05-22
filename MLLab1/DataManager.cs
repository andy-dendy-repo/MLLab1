using System.Text;

namespace MLLab1
{
    internal class DataManager
    {
        private DataContext _context;
        private List<DistanceMatrix> _matrices = new List<DistanceMatrix>();

        public void ImportData(string fileName)
        {
            var contextImport = new DataContextImport();

            _context = contextImport.ImportFromFile(fileName);
        }

        public void ExportData(string fileName)
        {
            DataExport dataExport = new DataExport();

            StringBuilder stringBuilder = dataExport.ExportToHtml(_context);

            foreach (var matrix in _matrices)
                stringBuilder = dataExport.ExportToHtml(matrix, stringBuilder, false);

            File.WriteAllText(fileName, stringBuilder.ToString());
        }

        public void Calculate(Method method)
        {
            var distanceMatrix = _context.ToDistanceMatrix();

            _matrices.Add(distanceMatrix);

            distanceMatrix = distanceMatrix.Clone();

            while (distanceMatrix.Dimension != 2)
            {
                distanceMatrix.FindMinimum();

                _matrices.Add(distanceMatrix);

                distanceMatrix = distanceMatrix.Clone();

                distanceMatrix.FindClasters(method);

                _matrices.Add(distanceMatrix);

                distanceMatrix = distanceMatrix.Clone();
            }
        }
    }
}
