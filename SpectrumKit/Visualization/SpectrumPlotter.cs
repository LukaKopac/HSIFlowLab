using SpectrumKit.Models;
using System.Globalization;
using System.Text;

namespace SpectrumKit.Visualization
{
    public static class SpectrumPlotter
    {
        public static string ToSvg(
            Spectrum spectrum,
            SpectrumPlotOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(spectrum);

            SpectrumPlotOptions plotOptions =
                options ?? new SpectrumPlotOptions();

            // Min and Max wavelength
            double minWavelength = spectrum.Wavelengths[0];
            double maxWavelength = spectrum.Wavelengths[0];

            for (int i = 1; i < spectrum.Wavelengths.Length; i++)
            {
                if (spectrum.Wavelengths[i] < minWavelength)
                    minWavelength = spectrum.Wavelengths[i];

                if (spectrum.Wavelengths[i] > maxWavelength)
                    maxWavelength = spectrum.Wavelengths[i];
            }

            // Min and Max value
            double minValue = spectrum.Min();
            double maxValue = spectrum.Max();

            // X axis
            double xMin = minWavelength;
            double xMax = maxWavelength;

            if (xMin == xMax)
            {
                xMax = xMin + 1;
            }

            // Y axis
            double yMin = plotOptions.YMin ?? minValue;
            double yMax = plotOptions.YMax ?? maxValue;

            if (yMin == yMax)
            {
                yMax = yMin + 1;
            }

            double yRange = yMax - yMin;

            if (plotOptions.YMin == null)
            {
                yMin -= yRange * plotOptions.YMinPadding;
            }

            if (plotOptions.YMax == null)
            {
                yMax += yRange * plotOptions.YMaxPadding;
            }

            // Ticks
            AxisTickResult yTicks = AxisTicks.Generate(
                yMin,
                yMax,
                desiredCount: 6);

            yMin = yTicks.Minimum;
            yMax = yTicks.Maximum;

            // Plot area
            double plotLeft = plotOptions.MarginLeft;
            double plotRight = plotOptions.Width - plotOptions.MarginRight;
            double plotTop = plotOptions.MarginTop;
            double plotBottom = plotOptions.Height - plotOptions.MarginBottom;

            double PlotX(double wavelength)
            {
                return plotLeft +
                       (wavelength - xMin) /
                       (xMax - xMin) *
                       (plotRight - plotLeft);
            }

            double PlotY(double value)
            {
                return plotBottom -
                       (value - yMin) /
                       (yMax - yMin) *
                       (plotBottom - plotTop);
            }

            var yAxisTicks = new StringBuilder();

            foreach (double tick in yTicks.Ticks)
            {
                double y = PlotY(tick);

                string yString =
                    y.ToString(CultureInfo.InvariantCulture);

                yAxisTicks.AppendLine($"""
                    <line x1="{plotLeft - 5}" y1="{yString}"
                          x2="{plotLeft}" y2="{yString}"
                          stroke="black"
                          stroke-width="1" />
                    """);
            }
            
            var path = new StringBuilder();

            for (int i = 0; i < spectrum.Wavelengths.Length; i++)
            {
                double x = PlotX(spectrum.Wavelengths[i]);
                double y = PlotY(spectrum.Values[i]);

                string xString = x.ToString(CultureInfo.InvariantCulture);
                string yString = y.ToString(CultureInfo.InvariantCulture);

                if (i == 0)
                    path.Append($"M {xString} {yString}");
                else
                    path.Append($" L {xString} {yString}");
            }

            return $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <svg xmlns="http://www.w3.org/2000/svg"
                     width="{plotOptions.Width}"
                     height="{plotOptions.Height}"
                     viewBox="0 0 {plotOptions.Width} {plotOptions.Height}">

                    <line x1="{plotLeft}" y1="{plotBottom}"
                          x2="{plotRight}" y2="{plotBottom}"
                          stroke="black"
                          stroke-width="1" />

                    <line x1="{plotLeft}" y1="{plotBottom}"
                          x2="{plotLeft}" y2="{plotTop}"
                          stroke="black"
                          stroke-width="1" />

                    {yAxisTicks}

                    <path
                        d="{path}"
                        fill="none"
                        stroke="blue"
                        stroke-width="1" />

                </svg>
                """;
        }
    }
}
