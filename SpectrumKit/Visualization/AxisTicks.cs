namespace SpectrumKit.Visualization
{
    public static class AxisTicks
    {
        public static AxisTickResult Generate(
            double min,
            double max,
            int desiredCount)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                min,
                max);

            ArgumentOutOfRangeException.ThrowIfLessThan(
                desiredCount,
                2);

            double range = max - min;

            double roughStep = range / (desiredCount - 1);

            double magnitude =
                Math.Pow(10, Math.Floor(Math.Log10(roughStep)));

            double normalizedStep =
                roughStep / magnitude;

            double niceStep;

            if (normalizedStep <= 1)
                niceStep = 1;
            else if (normalizedStep <= 2)
                niceStep = 2;
            else if (normalizedStep <= 2.5)
                niceStep = 2.5;
            else if (normalizedStep <= 5)
                niceStep = 5;
            else
                niceStep = 10;

            double step = niceStep * magnitude;

            double firstTick =
                Math.Floor(min / step) * step;

            double lastTick =
                Math.Ceiling(max / step) * step;

            List<double> ticks = new();

            for (double tick = firstTick;
                 tick <= lastTick + step * 0.000001;
                 tick += step)
            {
                ticks.Add(tick);
            }

            return new AxisTickResult(
                firstTick,
                lastTick,
                step,
                ticks.ToArray());
        }
    }
}