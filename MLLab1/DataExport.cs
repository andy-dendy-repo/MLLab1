using System.Drawing;
using System.Text;

namespace MLLab1
{
    internal class DataExport
    {
        private const string _styles = @"
        <style>
            table, th, td {
              border: 1px solid black;
              border-collapse: collapse;
              text-align: center;
            }
            td {
                height: 55px;
            }
        </style>
        ";

        public StringBuilder ExportToHtml(DataContext dataContext, StringBuilder stringBuilder = null, bool addStyles = true)
        {
            if (stringBuilder is null)
                stringBuilder = new StringBuilder();

            if (addStyles)
                stringBuilder.AppendLine(_styles);

            stringBuilder.AppendLine("<table>");

            stringBuilder.AppendLine("<thead>");
            stringBuilder.AppendLine("<tr>");
            stringBuilder.AppendLine("<th></th>");

            foreach (var column in dataContext.Columns)
            {
                stringBuilder.AppendLine($"<th>{column}</th>");
            }

            stringBuilder.AppendLine("</tr>");
            stringBuilder.AppendLine("</thead>");

            stringBuilder.AppendLine("<tbody>");

            foreach (var row in dataContext.Data)
            {
                stringBuilder.AppendLine("<tr>");

                stringBuilder.AppendLine($"<td>{row.Number}</td>");

                foreach (var value in row.Values)
                {
                    stringBuilder.AppendLine($"<td>{value}</td>");
                }

                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</tbody>");

            stringBuilder.AppendLine("</table>");

            return stringBuilder;
        }

        public StringBuilder ExportToHtml(DistanceMatrix distanceMatrix, StringBuilder stringBuilder = null, bool addStyles = true)
        {
            if (stringBuilder is null)
                stringBuilder = new StringBuilder();

            if (addStyles)
                stringBuilder.AppendLine(_styles);

            bool minimumFound = distanceMatrix.Minimum != null;

            if (minimumFound)
            {
                stringBuilder.AppendLine($"<p>Minimum = {distanceMatrix.Minimum.Value}. Columns: {distanceMatrix.ClasterI.Name};{distanceMatrix.ClasterJ.Name}</p>");
            }

            stringBuilder.AppendLine("<table>");

            stringBuilder.AppendLine("<tr><td></td>");

            for (int i = 0; i < distanceMatrix.Dimension; i++)
            {
                if (distanceMatrix.Clasters[i] == distanceMatrix.ClasterI || distanceMatrix.Clasters[i] == distanceMatrix.ClasterJ)
                {
                    stringBuilder.AppendLine($"<td style='background:yellow;'>{distanceMatrix.Clasters[i].Name}</td>");
                }
                else
                {
                    stringBuilder.AppendLine($"<td>{distanceMatrix.Clasters[i].Name}</td>");
                }
            }

            stringBuilder.AppendLine("</tr>");

            for (int i = 0; i < distanceMatrix.Dimension; i++)
            {
                stringBuilder.AppendLine("<tr>");

                if (distanceMatrix.Clasters[i] == distanceMatrix.ClasterI || distanceMatrix.Clasters[i] == distanceMatrix.ClasterJ)
                {
                    stringBuilder.AppendLine($"<td style='background:yellow;'>{distanceMatrix.Clasters[i].Name}</td>");
                }
                else
                {
                    stringBuilder.AppendLine($"<td>{distanceMatrix.Clasters[i].Name}</td>");
                }

                for (int j = 0; j < distanceMatrix.Dimension; j++)
                {
                    if (minimumFound &&
                        ((distanceMatrix.ClasterI.Index == i && distanceMatrix.ClasterJ.Index == j) ||
                        (distanceMatrix.ClasterI.Index == j && distanceMatrix.ClasterJ.Index == i)))
                    {
                        stringBuilder.AppendLine($"<td style='background:red;'>{distanceMatrix.Distances[i, j]}</td>");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"<td>{distanceMatrix.Distances[i, j]}</td>");
                    }

                }
                stringBuilder.AppendLine("</tr>");
            }

            stringBuilder.AppendLine("</table><br>");

            return stringBuilder;
        }
    }
}
