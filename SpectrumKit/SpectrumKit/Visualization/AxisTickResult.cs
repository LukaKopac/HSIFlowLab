namespace SpectrumKit.Visualization
{
    public readonly record struct AxisTickResult(
        double Minimum,
        double Maximum,
        double Step,
        double[] Ticks);
}
