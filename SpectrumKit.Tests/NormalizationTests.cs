using HSIDataKit.Models;
using SpectrumKit.Processing;

namespace SpectrumKit.Tests;

public class NormalizationTests
{
    [Fact]
    public void MinMax_NormalSpectrum_ReturnsValuesBetweenZeroAndOne()
    {
        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0, 400.0 },
            new[] { 2.0, 4.0, 6.0, 8.0 });

        var result = Normalization.MinMax(spectrum);

        Assert.Equal(
            new[] { 0.0, 0.3333333333333333, 0.6666666666666666, 1.0 },
            result.Values);
    }

    [Fact]
    public void MinMax_PreservesWavelengths()
    {
        var wavelengths = new[] { 100.0, 200.0, 300.0 };
        var spectrum = new Spectrum(
            wavelengths,
            new[] { 2.0, 4.0, 6.0 });

        var result = Normalization.MinMax(spectrum);

        Assert.Equal(wavelengths, result.Wavelengths);
    }

    [Fact]
    public void MinMax_ConstantSpectrum_ReturnsOriginalValues()
    {
        var values = new[] { 5.0, 5.0, 5.0, 5.0 };

        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0, 400.0 },
            values);

        var result = Normalization.MinMax(spectrum);

        Assert.Equal(values, result.Values);
    }

    [Fact]
    public void MinMax_DoesNotModifyOriginalSpectrum()
    {
        var values = new[] { 2.0, 4.0, 6.0 };

        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0 },
            values);

        Normalization.MinMax(spectrum);

        Assert.Equal(
            new[] { 2.0, 4.0, 6.0 },
            spectrum.Values);
    }
}
