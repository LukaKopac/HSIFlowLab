using SpectrumKit.Models;
using System.Globalization;
using System.Text;

namespace SpectrumKit.Visualization
{
    public static class SpectrumPlotter
    {
        public static string ToSvg(Spectrum spectrum)
        {
            ArgumentNullException.ThrowIfNull(spectrum);

            double minWavelength = spectrum.Wavelengths[0];
            double maxWavelength = spectrum.Wavelengths[0];

            for (int i = 1; i < spectrum.Wavelengths.Length; i++)
            {
                if (spectrum.Wavelengths[i] < minWavelength)
                    minWavelength = spectrum.Wavelengths[i];

                if (spectrum.Wavelengths[i] > maxWavelength)
                    maxWavelength = spectrum.Wavelengths[i];
            }

            double minValue = spectrum.Min();
            double maxValue = spectrum.Max();

            double plotLeft = 70;
            double plotRight = 770;
            double plotTop = 30;
            double plotBottom = 450;

            double PlotX(double wavelength)
            {
                return plotLeft +
                       (wavelength - minWavelength) /
                       (maxWavelength - minWavelength) *
                       (plotRight - plotLeft);
            }

            double PlotY(double value)
            {
                return plotBottom -
                       (value - minValue) /
                       (maxValue - minValue) *
                       (plotBottom - plotTop);
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
                     width="800"
                     height="500"
                     viewBox="0 0 800 500">

                    <line x1="{plotLeft}" y1="{plotBottom}"
                          x2="{plotRight}" y2="{plotBottom}"
                          stroke="black"
                          stroke-width="1" />

                    <line x1="{plotLeft}" y1="{plotBottom}"
                          x2="{plotLeft}" y2="{plotTop}"
                          stroke="black"
                          stroke-width="1" />

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
