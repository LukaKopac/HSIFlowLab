namespace SpectrumKit.Visualization
{
    public static class AxisLabelFormatter
    {
        public static string Format(
            double value,
            double step)
        {
            if (step >= 1)
                return value.ToString("0");

            if (step >= 0.1)
                return value.ToString("0.0");

            if (step >= 0.01)
                return value.ToString("0.00");

            if (step >= 0.001)
                return value.ToString("0.000");

            return value.ToString("0.#####");
        }
    }
}
