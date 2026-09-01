using SpectrumKit.Models;
using System.Globalization;

namespace SpectrumKit.IO
{
    
    public static class SpectrumLoader
    {
        /// <summary>
        /// Loads a set of spectra from a CSV file.
        /// </summary>
        /// <remarks>
        /// Expected CSV structure:
        /// <code>
        /// "Wavelength","label1","label2"
        /// 400,0.32,0.41
        /// 401,0.35,0.43
        /// </code>
        /// The first column contains wavelengths.
        /// Each additional column contains one spectrum.
        /// </remarks>
        /// <param name="path">Path to the CSV file.</param>
        /// <returns>The loaded spectrum set.</returns>
        public static SpectrumSet LoadCsv(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            string[] lines = File.ReadAllLines(path);

            if (lines.Length < 2)
                throw new FormatException(
                    "CSV file must contain a header and at least one data row.");

            string[] headers = ParseCsvLine(lines[0]);

            if (headers.Length < 2)
                throw new FormatException(
                    "CSV file must contain a wavelength column and at least one spectrum.");

            if (!string.Equals(
                    headers[0].Trim(),
                    "Wavelength",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(
                    "The first column must be named 'Wavelength'");
            }

            List<string> names = headers
                .Skip(1)
                .Select(name => name.Trim())
                .ToList();

            List<double> wavelengths = new();

            List<List<double>> values = names
                .Select(_ => new List<double>())
                .ToList();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                string[] parts = ParseCsvLine(lines[i]);

                if (parts.Length != headers.Length)
                    throw new FormatException(
                        $"Line {i + 1} contains {parts.Length} columns, " +
                        $"but {headers.Length} were expected.");

                wavelengths.Add(
                    double.Parse(
                        parts[0],
                        CultureInfo.InvariantCulture));

                for (int j = 1; j < parts.Length; j++)
                {
                    values[j - 1].Add(
                        double.Parse(
                            parts[j],
                            CultureInfo.InvariantCulture));
                }
            }

            return new SpectrumSet(
                wavelengths.ToArray(),
                names,
                values.Select(v => v.ToArray()).ToList());
        }

        private static string[] ParseCsvLine(string line)
        {
            List<string> fields = new();
            var field = new System.Text.StringBuilder();

            bool insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char character = line[i];

                if (character == '"')
                {
                    // Two quotes inside a quoted field represent one quote
                    if (insideQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = !insideQuotes;
                    }
                }
                else if (character == ',' && !insideQuotes)
                {
                    fields.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(character);
                }
            }

            if (insideQuotes)
            {
                throw new FormatException(
                    "CSV contains an unterminated quoted field.");
            }

            fields.Add(field.ToString());

            return fields.ToArray();
        }
    }
}
