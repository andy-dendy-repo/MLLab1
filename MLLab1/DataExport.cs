using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        public StringBuilder ExportToHtml(List<(string NameI, string NameJ, double Value)> history, StringBuilder stringBuilder = null, bool addStyles = true)
        {
            if (stringBuilder is null)
                stringBuilder = new StringBuilder();

            if (addStyles)
                stringBuilder.AppendLine(_styles);

            stringBuilder.AppendLine("<ul>");

            foreach (var item in history)
            {
                stringBuilder.AppendLine($"<li>Left column: {item.NameI}; Right column: {item.NameJ}; Minimum: {item.Value}</li>");
            }

            stringBuilder.AppendLine("</ul><hr>");

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

            stringBuilder.AppendLine("</table><hr>");

            return stringBuilder;
        }

        public StringBuilder ExportAsImageToHtml(List<(string NameI, string NameJ, double Value)> history, string order, StringBuilder stringBuilder = null, bool addStyles = true)
        {
            int w = 1000, h = 1000;
            int wShift = 80, hShift = 80;

            Bitmap bitmap = new Bitmap(w, h);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.Black);
            Font font = new Font("arial", 10);

            //----------------

            int[] orders = order.Split(',').Select(x => int.Parse(x)).ToArray();

            //--------------XY
            graphics.DrawLine(pen, 0 + wShift, 0 + hShift, w - wShift, 0 + hShift);
            graphics.DrawLine(pen, 0 + wShift, 0 + hShift, 0 + wShift, h + hShift);

            int length = orders.Length;
            int lenBetween = (h - 2 * hShift) / length;
            for (int i = 0; i < length; i++)
            {
                graphics.FillEllipse(brush, wShift, hShift + i * lenBetween, 4, 4);
                graphics.DrawString(orders[i].ToString(), font, brush, wShift, hShift + i * lenBetween);
            }
            //--------------XY

            var parsedHi = history
                .Select(x => 
                    (
                        x.Value, 
                        x.NameI.Split(',').Select(g => int.Parse(g)).Select(y => Array.IndexOf(orders, y)).ToArray(), 
                        x.NameJ.Split(',').Select(g => int.Parse(g)).Select(y => Array.IndexOf(orders, y)).ToArray()
                    )
                )
                .GroupBy(x => x.Item2.Length + x.Item3.Length)
                .OrderBy(x => x.Key)
                .ToList();

            double lenBetweenW = (w - 2 * wShift) / history.Max(x => x.Value);

            Pen penR = new Pen(Color.Black, 2);
            penR.Alignment = PenAlignment.Inset;

            var added = new List<(int X, int[] Points, int Middle)?>();

            foreach (var pHi in parsedHi)
            {
                foreach (var hi in pHi)
                {
                    var yi = hi.Item2;
                    var yj = hi.Item3;

                    int ai = 0;
                    int aj = 0;

                    int xMinI = 0;
                    int xMinJ = 0;

                    var addedI = added.FirstOrDefault(x => x.Value.Points.Intersect(yi).Count() != 0);
                    if (addedI != null)
                    {
                        added.Remove(addedI);
                        ai = addedI.Value.Middle;
                        xMinI = addedI.Value.X;
                    }
                    else
                    {
                        ai = (int)(hShift + yi.Average() * lenBetween);
                        xMinI = wShift;
                    }

                    var addedJ = added.FirstOrDefault(x => x.Value.Points.Intersect(yj).Count() != 0);
                    if (addedJ != null)
                    {
                        added.Remove(addedJ);
                        aj = addedJ.Value.Middle;
                        xMinJ = addedJ.Value.X;
                    }
                    else
                    {
                        aj = (int)(hShift + yj.Average() * lenBetween);
                        xMinJ = wShift;
                    }

                    graphics.DrawLine(pen, xMinI, ai, (int)(hi.Value * lenBetweenW), ai);

                    graphics.DrawLine(pen, xMinJ, aj, (int)(hi.Value * lenBetweenW), aj);

                    graphics.DrawLine(pen, (int)(hi.Value * lenBetweenW), ai, (int)(hi.Value * lenBetweenW), aj);


                    added.Add(((int)(hi.Value * lenBetweenW), yi.Concat(yj).Distinct().ToArray(), (ai + aj) / 2));
                }
            }

            //----------------

            graphics.Save();

            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            byte[] imageArray = stream.ToArray();
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            stringBuilder.AppendLine($"<img src='data:image/png;base64, {base64ImageRepresentation}'><hr>");

            return stringBuilder;
        }

        public StringBuilder ExportAsImageToHtml(DataContext dataContext, StringBuilder stringBuilder = null, bool addStyles = true)
        {
            int w = 1000, h = 700;
            int wShift = 80, hShift = 80;

            Bitmap bitmap = new Bitmap(w, h);

            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);

            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.Black);
            Font font = new Font("arial", 10);

            //--------------XY
            graphics.DrawLine(pen, 0 + wShift, 0 + hShift, w - wShift, 0 + hShift);
            graphics.DrawLine(pen, 0 + wShift, 0 + hShift, 0 + wShift, h + hShift);

            int length = dataContext.Columns.Length;
            int lenBetween = (w - 2 * wShift) / length;
            for (int i = 0; i < length; i++)
            {
                graphics.FillEllipse(brush, wShift + i * lenBetween, hShift, 4, 4);
                graphics.DrawString(dataContext.Columns[i], font, brush, wShift + i * lenBetween, hShift);
            }
            //--------------XY

            var lines = dataContext.Data.Select(x => x.Values.Select((y, i) => new Point(i * lenBetween + wShift, (int)y)).ToArray()).ToList();

            int max = lines.Max(x => x.Max(x => x.Y));
            double lenBetweenH = (double)(h - 2 * hShift) / max;

            lines = lines.Select(x => x.Select(x => { x.Y = (int)(x.Y * lenBetweenH); return x; }).ToArray()).ToList();


            foreach (var item in lines)
            {
                Random random = new Random();
                Color randomColor = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                graphics.DrawLines(new Pen(randomColor), item);
            }

            graphics.Save();

            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Jpeg);
            byte[] imageArray = stream.ToArray();
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            stringBuilder.AppendLine($"<img src='data:image/png;base64, {base64ImageRepresentation}'><hr>");

            return stringBuilder;
        }
    }
}
