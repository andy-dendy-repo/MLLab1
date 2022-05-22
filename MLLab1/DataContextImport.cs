using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLLab1
{
    internal class DataContextImport
    {
        public char Delimeter { init; get; }

        public DataContextImport()
        {
            Delimeter = ';';
        }

        public DataContext ImportFromFile(string fileName)
        {
            IList<string> lines = File.ReadAllLines(fileName);

            string[] columns = lines[0].Split(Delimeter).Skip(1).ToArray();

            var notParsedData = lines.Skip(1).Select(x => x.Split(Delimeter).Select(y => double.Parse(y)).ToList()).ToList();

            var parsedData = notParsedData.Select(x => ((int)x[0], x.Skip(1).ToList())).ToList();

            return new DataContext(parsedData, columns);
        }
    }
}
