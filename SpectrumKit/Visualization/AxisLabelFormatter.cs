using System.Globalization;

namespace SpectrumKit.Visualization
{
    public static class AxisLabelFormatter
    {
        public static string Format(
            double value,
            double step)
        {
            int decimalPlaces = GetDecimalPlaces(step);

            return value.ToString(
                $"F{decimalPlaces}",
                CultureInfo.InvariantCulture);
        }

        private static int GetDecimalPlaces(double value)
        {
            value = Math.Abs(value);

            int decimalPlaces = 0;

            while (decimalPlaces < 10 &&
                   Math.Abs(value - Math.Round(value)) > 1e-9)
            {
                value *= 10;
                decimalPlaces++;
            }

            return decimalPlaces;
        }
    }
}
