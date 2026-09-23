using HSIDataKit.Models;
using SpectrumKit.Processing;

namespace SpectrumKit.Tests;

public class SavitzkyGolayTests
{
    [Fact]
    public void Smooth_ConstantSpectrum_RemainsConstant()
    {
        var spectrum = new Spectrum(
            new double[] { 1, 2, 3, 4, 5, 6, 7 },
            new double[] { 5, 5, 5, 5, 5, 5, 5 },
            "Test");

        var result = SavitzkyGolay.Smooth(
            spectrum,
            windowSize: 5,
            polynomialOrder: 2);

        foreach (double value in result.Values)
        {
            Assert.Equal(5, value, 10);
        }
    }

    [Fact]
    public void Smooth_LinearSpectrum_RemainsLinear()
    {
        var spectrum = new Spectrum(
            new double[] { 1, 2, 3, 4, 5, 6, 7 },
            new double[] { 1, 2, 3, 4, 5, 6, 7 },
            "Test");

        var result = SavitzkyGolay.Smooth(
            spectrum,
            windowSize: 5,
            polynomialOrder: 2);

        for (int i = 0; i < result.Values.Length; i++)
        {
            Assert.Equal(
                spectrum.Values[i],
                result.Values[i],
                10);
        }
    }

    [Fact]
    public void Smooth_ReducesLocalSpike()
    {
        var spectrum = new Spectrum(
            new double[] { 1, 2, 3, 4, 5, 6, 7 },
            new double[] { 1, 2, 10, 4, 5, 6, 7 },
            "Test");

        var result = SavitzkyGolay.Smooth(
            spectrum,
            windowSize: 5,
            polynomialOrder: 2);

        Assert.True(result.Values[2] < 10);
    }


}
