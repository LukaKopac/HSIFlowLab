using HSIDataKit.Models;
using SpectrumKit.Processing;

namespace SpectrumKit.Tests;

public class MSCTests
{
    [Fact]
    public void Apply_IdenticalSpectrum_ReturnsUnchangedSpectrum()
    {
        double[] wavelengths =
        {
        100, 110, 120, 130, 140
    };

        double[] values =
        {
        1, 2, 3, 4, 5
    };

        var spectrum =
            new Spectrum(
                wavelengths,
                values,
                "test");

        var reference =
            new Spectrum(
                wavelengths,
                values,
                "reference");

        Spectrum result =
            MSC.Apply(
                spectrum,
                reference);

        Assert.Equal(
            values,
            result.Values);
    }

    [Fact]
    public void Apply_CorrectsOffsetAndScaling()
    {
        double[] wavelengths = {100, 110, 120, 130, 140};

        double[] referenceValues = {1, 2, 3, 4, 5};

        double[] spectrumValues = {3, 5, 7, 9, 11};

        var spectrum =
            new Spectrum(
                wavelengths,
                spectrumValues);

        var reference =
            new Spectrum(
                wavelengths,
                referenceValues);

        Spectrum result =
            MSC.Apply(
                spectrum,
                reference);

        for (int i = 0; i < referenceValues.Length; i++)
        {
            Assert.Equal(
                referenceValues[i],
                result.Values[i],
                precision: 10);
        }
    }

    [Fact]
    public void Apply_RejectsConstantReference()
    {
        double[] wavelengths = { 100, 110, 120, 130, 140 };

        double[] referenceValues = { 1, 1, 1, 1, 1 };

        double[] spectrumValues = { 2, 3, 4, 5, 6 };

        var spectrum =
            new Spectrum(
                wavelengths,
                spectrumValues);

        var reference =
            new Spectrum(
                wavelengths,
                referenceValues);

        Assert.Throws<ArgumentException>(() =>
            MSC.Apply(
                spectrum,
                reference));
    }

    [Fact]
    public void Apply_RejectsDifferentWavelengthCount()
    {
        double[] spectrumWavelengths = { 100, 110, 120, 130, 140 };
        double[] referenceWavelengths = { 100, 110, 120, 130 };

        var spectrum =
            new Spectrum(
                spectrumWavelengths,
                new double[] { 1, 2, 3, 4, 5 });

        var reference =
            new Spectrum(
                referenceWavelengths,
                new double[] { 1, 2, 3, 4 });

        Assert.Throws<ArgumentException>(() =>
            MSC.Apply(
                spectrum,
                reference));
    }

    [Fact]
    public void Apply_RejectsDifferentWavelengths()
    {
        double[] spectrumWavelengths = { 100, 110, 120, 130, 140 };
        double[] referenceWavelengths = { 100, 110, 120, 135, 140 };

        var spectrum =
            new Spectrum(
                spectrumWavelengths,
                new double[] { 1, 2, 3, 4, 5 });

        var reference =
            new Spectrum(
                referenceWavelengths,
                new double[] { 1, 2, 3, 4, 5 });

        Assert.Throws<ArgumentException>(() =>
            MSC.Apply(
                spectrum,
                reference));
    }

    [Fact]
    public void Apply_CorrectsOffsetOnly()
    {
        double[] wavelengths = { 100, 110, 120, 130, 140 };

        double[] referenceValues = { 1, 2, 3, 4, 5 };

        double[] spectrumValues = { 3, 4, 5, 6, 7 };

        var spectrum =
            new Spectrum(
                wavelengths,
                spectrumValues);

        var reference =
            new Spectrum(
                wavelengths,
                referenceValues);

        Spectrum result =
            MSC.Apply(
                spectrum,
                reference);

        for (int i = 0; i < referenceValues.Length; i++)
        {
            Assert.Equal(
                referenceValues[i],
                result.Values[i],
                precision: 10);
        }
    }
}
