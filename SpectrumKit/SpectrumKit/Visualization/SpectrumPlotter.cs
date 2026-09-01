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

            if (spectrum.Wavelengths.Length == 0)
            {
                throw new ArgumentException(
                    "Spectrum must contain at least one data point.",
                    nameof(spectrum));
            }

            if (spectrum.Wavelengths.Length != spectrum.Values.Length)
            {
                throw new ArgumentException(
                    "Wavelength and value arrays must have the same length.",
                    nameof(spectrum));
            }

            SpectrumPlotOptions plotOptions =
                options ?? new SpectrumPlotOptions();

            if (!double.IsFinite(plotOptions.Width) ||
                !double.IsFinite(plotOptions.Height) ||
                plotOptions.Width <= 0 ||
                plotOptions.Height <= 0)
            {
                throw new ArgumentException(
                    "Plot width and height must be positive finite values.",
                    nameof(options));
            }

            if (plotOptions.MarginLeft < 0 ||
                plotOptions.MarginRight < 0 ||
                plotOptions.MarginTop < 0 ||
                plotOptions.MarginBottom < 0)
            {
                throw new ArgumentException(
                    "Plot margins cannot be negative.",
                    nameof(options));
            }

            if (plotOptions.MarginLeft + plotOptions.MarginRight >=
                plotOptions.Width)
            {
                throw new ArgumentException(
                    "Left and right margins leave no horizontal plot area.",
                    nameof(options));
            }

            if (plotOptions.MarginTop + plotOptions.MarginBottom >=
                plotOptions.Height)
            {
                throw new ArgumentException(
                    "Top and bottom margins leave no vertical plot area.",
                    nameof(options));
            }


            foreach (double wavelength in spectrum.Wavelengths)
            {
                if (!double.IsFinite(wavelength))
                {
                    throw new ArgumentException(
                        "Spectrum wavelengths must contain only finite values.",
                        nameof(spectrum));
                }
            }

            foreach (double value in spectrum.Values)
            {
                if (!double.IsFinite(value))
                {
                    throw new ArgumentException(
                        "Spectrum values must contain only finite values.",
                        nameof(spectrum));
                }
            }


            // Min and Max wavelength
            double minWavelength = spectrum.MinWavelength();
            double maxWavelength = spectrum.MaxWavelength();

            // Min and Max value
            double minValue = spectrum.Min();
            double maxValue = spectrum.Max();

            // X axis
            var (xMin, xMax) = CalculateXAxisRange(
                minWavelength,
                maxWavelength);

            // Y axis
            var (yMin, yMax) = CalculateYAxisRange(
                minValue,
                maxValue,
                plotOptions);

            // Plot area
            double plotLeft = plotOptions.MarginLeft;
            double plotRight = plotOptions.Width - plotOptions.MarginRight;
            double plotTop = plotOptions.MarginTop;
            double plotBottom = plotOptions.Height - plotOptions.MarginBottom;

            // Generate Y-axis ticks
            AxisTickResult yTicks = CreateYAxisTicks(
                yMin,
                yMax);

            // Use the final axis range for plotting
            yMin = yTicks.Minimum;
            yMax = yTicks.Maximum;

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

            // Create Y-axis SVG
            var (
                yAxisTicks,
                yGridLines) = CreateYAxis(
                    yTicks,
                    plotLeft,
                    plotRight,
                    plotTop,
                    plotBottom,
                    plotOptions.ShowGrid);



            // xTicks
            AxisTickResult xTicks = AxisTicks.Generate(
                xMin,
                xMax,
                desiredCount: 6);

            var xAxisTicks = new StringBuilder();
            var xGridLines = new StringBuilder();

            foreach (double tick in xTicks.Ticks)
            {
                double x = PlotX(tick);

                string xString =
                    x.ToString(CultureInfo.InvariantCulture);

                string label =
                    AxisLabelFormatter.Format(
                        tick,
                        xTicks.Step);

                if (plotOptions.ShowGrid)
                {
                    xGridLines.AppendLine($"""
                            <line x1="{xString}" y1="{SvgNumber(plotTop)}"
                                  x2="{xString}" y2="{SvgNumber(plotBottom)}"
                                  stroke="#DDDDDD"
                                  stroke-width="1" />
                        """);
                }

                xAxisTicks.AppendLine($"""
                    <line x1="{xString}" y1="{SvgNumber(plotBottom)}"
                          x2="{xString}" y2="{SvgNumber(plotBottom + 5)}"
                          stroke="black"
                          stroke-width="1" />

                    <text x="{xString}" y="{SvgNumber(plotBottom + 20)}"
                          text-anchor="middle"
                          font-size="12">
                        {label}
                    </text>
                    """);
            }


            // Create spectrum path
            string path = CreateSpectrumPath(
                spectrum.Wavelengths,
                spectrum.Values,
                PlotX,
                PlotY);

            // Add title
            var title = new StringBuilder();

            if (plotOptions.Title != null)
            {
                title.AppendLine($"""
                    <text x="{plotOptions.Width / 2}"
                          y="{plotOptions.MarginTop / 2}"
                          text-anchor="middle"
                          font-size="16">
                        {EscapeXml(plotOptions.Title)}
                    </text>
                    """);
            }

            // Add x axis label
            var xAxisLabel = new StringBuilder();

            if (plotOptions.XLabel != null)
            {
                xAxisLabel.AppendLine($"""
                    <text x="{(plotLeft + plotRight) / 2}"
                          y="{plotOptions.Height - 10}"
                          text-anchor="middle"
                          font-size="14">
                        {EscapeXml(plotOptions.XLabel)}
                    </text>
                    """);
            }

            // Add y axis label
            var yAxisLabel = new StringBuilder();

            if (plotOptions.YLabel != null)
            {
                double yLabelX = plotOptions.MarginLeft / 2;
                double yLabelY = (plotTop + plotBottom) / 2;

                yAxisLabel.AppendLine($"""
                    <g transform="translate({yLabelX},{yLabelY})">
                        <text
                            x="0"
                            y="0"
                            text-anchor="middle"
                            font-size="14"
                            transform="rotate(-90)">
                            {EscapeXml(plotOptions.YLabel)}
                        </text>
                    </g>
                    """);
            }

            // Create SVG XML
            return $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <svg xmlns="http://www.w3.org/2000/svg"
                     width="{SvgNumber(plotOptions.Width)}"
                     height="{SvgNumber(plotOptions.Height)}"
                     viewBox="0 0 {SvgNumber(plotOptions.Width)} {SvgNumber(plotOptions.Height)}">

                     {title}

                     {yGridLines}

                     {xGridLines}
                     
                     <line x1="{SvgNumber(plotLeft)}" y1="{SvgNumber(plotBottom)}"
                          x2="{SvgNumber(plotRight)}" y2="{SvgNumber(plotBottom)}"
                          stroke="black"
                          stroke-width="1" />

                    <line x1="{SvgNumber(plotLeft)}" y1="{SvgNumber(plotBottom)}"
                          x2="{SvgNumber(plotLeft)}" y2="{SvgNumber(plotTop)}"
                          stroke="black"
                          stroke-width="1" />

                    {yAxisTicks}

                    {xAxisTicks}

                    {xAxisLabel}

                    {yAxisLabel}

                    <path
                        d="{path}"
                        fill="none"
                        stroke="{plotOptions.SpectrumColor}"
                        stroke-width="{SvgNumber(plotOptions.SpectrumLineWidth)}" />

                </svg>
                """;
        }

        /*
        public static string ToSvg(
            SpectrumSet spectrumSet,
            SpectrumPlotOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(spectrumSet);

            SpectrumPlotOptions plotOptions =
                options ?? new SpectrumPlotOptions();

            if (spectrumSet.Values.Count == 0)
                throw new ArgumentException(
                    "Spectrum set must contain at least one spectrum.",
                    nameof(spectrumSet));

            // Calculate shared X range
            double minWavelength = spectrumSet.Wavelengths.Min();
            double maxWavelength = spectrumSet.Wavelengths.Max();

            var (xMin, xMax) = CalculateXAxisRange(
                minWavelength,
                maxWavelength);

            // Calculate shared Y range
            double minValue = double.PositiveInfinity;
            double maxValue = double.NegativeInfinity;

            foreach (double[] values in spectrumSet.Values)
            {
                foreach (double value in values)
                {
                    if (value < minValue)
                        minValue = value;

                    if (value > maxValue)
                        maxValue = value;
                }
            }

            var (yMin, yMax) = CalculateYAxisRange(
                minValue,
                maxValue,
                plotOptions);

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

            // Create spectrum paths
            var spectrumPaths = new StringBuilder();

            for (int i = 0; i < spectrumSet.Values.Count; i++)
            {
                string path = CreateSpectrumPath(
                    spectrumSet.Wavelengths,
                    spectrumSet.Values[i],
                    PlotX,
                    PlotY);

                string color =
                    DefaultSpectrumColors[i % DefaultSpectrumColors.Length];

                spectrumPaths.AppendLine(
                    CreateSpectrumPathElement(
                        path,
                        color,
                        plotOptions.SpectrumLineWidth));
            }
        }
        */

        private static string CreateSpectrumPath(
            double[] wavelengths,
            double[] values,
            Func<double, double> plotX,
            Func<double, double> plotY)
        {
            var path = new StringBuilder();

            for (int i = 0; i < wavelengths.Length; i++)
            {
                double x = plotX(wavelengths[i]);
                double y = plotY(values[i]);

                string xString =
                    x.ToString(CultureInfo.InvariantCulture);

                string yString =
                    y.ToString(CultureInfo.InvariantCulture);

                if (i == 0)
                    path.Append($"M {xString} {yString}");
                else
                    path.Append($" L {xString} {yString}");
            }

            return path.ToString();
        }

        private static (double Minimum, double Maximum) CalculateXAxisRange(
            double minWavelength,
            double maxWavelength)
        {
            if (minWavelength > maxWavelength)
                throw new ArgumentException(
                    "Minimum wavelength cannot be greater than maximum wavelength.");

            if (minWavelength == maxWavelength)
            {
                double padding = Math.Abs(minWavelength) * 0.01;

                if (padding == 0)
                    padding = 1;

                minWavelength -= padding;
                maxWavelength += padding;
            }

            return (minWavelength, maxWavelength);
        }


        private static (double Minimum, double Maximum) CalculateYAxisRange(
            double minValue,
            double maxValue,
            SpectrumPlotOptions plotOptions)
        {
            double yMin = plotOptions.YMin ?? minValue;
            double yMax = plotOptions.YMax ?? maxValue;

            if (!double.IsFinite(yMin) || !double.IsFinite(yMax))
            {
                throw new ArgumentException(
                    "Y-axis limits must be finite.");
            }

            if (yMin > yMax)
            {
                throw new ArgumentException(
                    "YMin cannot be greater than YMax.");
            }

            if (yMin == yMax)
            {
                double padding = Math.Abs(yMin) * 0.05;

                if (padding == 0)
                    padding = 1;

                if (plotOptions.YMin == null)
                    yMin -= padding;

                if (plotOptions.YMax == null)
                    yMax += padding;

                // Both limits were explicitly set to the same value.
                if (yMin == yMax)
                {
                    yMin -= 1;
                    yMax += 1;
                }

                return (yMin, yMax);
            }

            double range = yMax - yMin;

            if (plotOptions.YMin == null)
                yMin -= range * plotOptions.YMinPadding;

            if (plotOptions.YMax == null)
                yMax += range * plotOptions.YMaxPadding;

            return (yMin, yMax);
        }


        private static string CreateSpectrumPathElement(
            string path,
            string color,
            double lineWidth)
        {
            return $"""
                <path
                    d="{path}"
                    fill="none"
                    stroke="{EscapeXml(color)}"
                    stroke-width="{SvgNumber(lineWidth)}" />
                """;
        }

        private static AxisTickResult CreateYAxisTicks(
            double yMin,
            double yMax)
        {
            return AxisTicks.Generate(
                yMin,
                yMax,
                desiredCount: 6);
        }



        private static (
            string AxisTicks,
            string GridLines) CreateYAxis(
            AxisTickResult yTicks,
            double plotLeft,
            double plotRight,
            double plotTop,
            double plotBottom,
            bool showGrid)

        {
            var yAxisTicks = new StringBuilder();
            var yGridLines = new StringBuilder();

            foreach (double tick in yTicks.Ticks)
            {
                double y =
                    plotBottom -
                    (tick - yTicks.Minimum) /
                    (yTicks.Maximum - yTicks.Minimum) *
                    (plotBottom - plotTop);


                string yString =
                    y.ToString(CultureInfo.InvariantCulture);

                string label =
                    AxisLabelFormatter.Format(
                        tick,
                        yTicks.Step);

                if (showGrid)
                {
                    yGridLines.AppendLine($"""
                    <line x1="{SvgNumber(plotLeft)}" y1="{yString}"
                          x2="{SvgNumber(plotRight)}" y2="{yString}"
                          stroke="#DDDDDD"
                          stroke-width="1" />
                """);
                }

                yAxisTicks.AppendLine($"""
                                <line x1="{SvgNumber(plotLeft - 5)}" y1="{yString}"
                                      x2="{SvgNumber(plotLeft)}" y2="{yString}"
                                      stroke="black"
                                      stroke-width="1" />

                                <text x="{SvgNumber(plotLeft - 10)}" y="{yString}"
                                      text-anchor="end"
                                      dominant-baseline="middle"
                                      font-size="12">
                                    {label}
                                </text>
                                """);
            }

            return (
                yAxisTicks.ToString(),
                yGridLines.ToString());

        }

        private static string EscapeXml(string value)
        {
            return System.Security.SecurityElement.Escape(value) ?? string.Empty;
        }


        private static string SvgNumber(double value)
        {
            return value.ToString(
                CultureInfo.InvariantCulture);
        }

        private static readonly string[] DefaultSpectrumColors =
        {
            "#1F77B4",
            "#FF7F0E",
            "#2CA02C",
            "#D62728",
            "#9467BD",
            "#8C564B"
        };
    }
}
