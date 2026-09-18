using HSIDataKit.Models;
using SpectrumKit.Processing;

namespace SpectrumKit.Tests;

public class SNVTests
{
    [Fact]
    public void Apply_StandardSpectrum_ReturnsStandardizedValues()
    {
        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0, 400.0 },
            new[] { 2.0, 4.0, 6.0, 8.0 });

        var result = SNV.Apply(spectrum);

        Assert.Equal(
            new[]
            {
                -1.3416407864998738,
                -0.4472135954999579,
                0.4472135954999579,
                1.3416407864998738
            },
            result.Values);
    }

    [Fact]
    public void Apply_ConstantSpectrum_ReturnsZeros()
    {
        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0, 400.0 },
            new[] { 5.0, 5.0, 5.0, 5.0 });

        var result = SNV.Apply(spectrum);

        Assert.Equal(
            new[] { 0.0, 0.0, 0.0, 0.0 },
            result.Values);
    }

    [Fact]
    public void Apply_PreservesWavelengths()
    {
        var wavelengths = new[] { 100.0, 200.0, 300.0 };

        var spectrum = new Spectrum(
            wavelengths,
            new[] { 2.0, 4.0, 6.0 });

        var result = SNV.Apply(spectrum);

        Assert.Equal(wavelengths, result.Wavelengths);
    }

    [Fact]
    public void Apply_DoesNotModifyOriginalSpectrum()
    {
        var spectrum = new Spectrum(
            new[] { 100.0, 200.0, 300.0 },
            new[] { 2.0, 4.0, 6.0 });

        SNV.Apply(spectrum);

        Assert.Equal(
            new[] { 2.0, 4.0, 6.0 },
            spectrum.Values);
    }
}
