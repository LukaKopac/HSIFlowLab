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

    [Fact]
    public void Derivative_FirstOrder_LinearSpectrum_ReturnsConstantDerivative()
    {
        double[] wavelengths =
        {
        100, 110, 120, 130, 140,
        150, 160, 170, 180, 190
    };

        double[] values =
        {
        2, 4, 6, 8, 10,
        12, 14, 16, 18, 20
    };

        var spectrum =
            new Spectrum(
                wavelengths,
                values);

        Spectrum result =
            SavitzkyGolay.Derivative(
                spectrum,
                windowSize: 5,
                polynomialOrder: 2,
                derivativeOrder: 1);

        foreach (double value in result.Values)
        {
            Assert.Equal(
                0.2,
                value,
                precision: 10);
        }
    }

    [Fact]
    public void Derivative_SecondOrder_QuadraticSpectrum_ReturnsConstantDerivative()
    {
        double[] wavelengths =
        {
        100, 110, 120, 130, 140,
        150, 160, 170, 180, 190
    };

        double[] values =
        {
        1, 4, 9, 16, 25,
        36, 49, 64, 81, 100
    };

        var spectrum =
            new Spectrum(
                wavelengths,
                values);

        Spectrum result =
            SavitzkyGolay.Derivative(
                spectrum,
                windowSize: 5,
                polynomialOrder: 2,
                derivativeOrder: 2);

        foreach (double value in result.Values)
        {
            Assert.Equal(
                0.02,
                value,
                precision: 10);
        }
    }

    [Fact]
    public void Derivative_RejectsUnevenWavelengthSpacing()
    {
        double[] wavelengths =
        {
        100, 110, 120, 135, 140,
        150, 160, 170, 180, 190
    };

        double[] values =
        {
        2, 4, 6, 8, 10,
        12, 14, 16, 18, 20
    };

        var spectrum =
            new Spectrum(
                wavelengths,
                values);

        Assert.Throws<ArgumentException>(() =>
            SavitzkyGolay.Derivative(
                spectrum,
                windowSize: 5,
                polynomialOrder: 2,
                derivativeOrder: 1));
    }
}
