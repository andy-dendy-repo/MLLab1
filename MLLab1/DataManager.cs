using System.Text;

namespace MLLab1
{
    internal class DataManager
    {
        private DataContext _context;
        private List<DistanceMatrix> _matrices = new List<DistanceMatrix>();

        public List<(string NameI, string NameJ, double Value)> GetTransformationHistory()
        {
            return _matrices.Where(x => x.Minimum != null)
                .Select(x => (x.ClasterI.Name, x.ClasterJ.Name, x.Minimum.Value))
                .ToList();
        }

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

            stringBuilder = dataExport.ExportToHtml(GetTransformationHistory(), stringBuilder, false);
            stringBuilder = dataExport.ExportAsImageToHtml(_context, stringBuilder, false);

            var last = _matrices.Last();

            stringBuilder = dataExport.ExportAsImageToHtml(
                GetTransformationHistory()
                .Append((last.Clasters[0].Name, last.Clasters[1].Name, last.Distances[0,1])).ToList(), 
                last.Clasters[0].Name+","+last.Clasters[1].Name, stringBuilder, false);

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
