using HSIDataKit.Models;

namespace MaskingKit;

public static class SpectralCorrelationMapper
{
    public static Mask Apply(
        HsiCube cube,
        float[] referenceSpectrum,
        float threshold)
    {
        ArgumentNullException.ThrowIfNull(cube);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        var correlationMap = CalculateCorrelationMap(
            cube,
            referenceSpectrum);

        return Threshold.GreaterThanOrEqual(
            correlationMap,
            threshold);
    }

    public static Image CalculateCorrelationMap(
        HsiCube cube,
        float[] referenceSpectrum)
    {
        ArgumentNullException.ThrowIfNull(cube);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        if (referenceSpectrum.Length != cube.Bands)
        {
            throw new ArgumentException(
                "Reference spectra must have the same number of bands as the cube.",
                nameof(referenceSpectrum));
        }

        var result = new float[cube.Height, cube.Width];

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                result[y, x] = CalculateCorrelation(
                    cube.GetSpectrum(y, x),
                    referenceSpectrum);
            }
        }

        return new Image(result);
    }

    public static float CalculateCorrelation(
        float[] spectrum,
        float[] referenceSpectrum)
    {
        ArgumentNullException.ThrowIfNull(spectrum);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        if (spectrum.Length != referenceSpectrum.Length)
        {
            throw new ArgumentException(
                "Spectra must have the same number of values.",
                nameof(referenceSpectrum));
        }

        if (spectrum.Length == 0)
        {
            throw new ArgumentException(
                "Spectra must contain at least one value",
                nameof(spectrum));
        }

        double spectrumMean = 0.0;
        double referenceMean = 0.0;

        for (int i = 0; i < spectrum.Length; i++)
        {
            spectrumMean += spectrum[i];
            referenceMean += referenceSpectrum[i];
        }

        spectrumMean /= spectrum.Length;
        referenceMean /= referenceSpectrum.Length;

        double numerator = 0.0;
        double spectrumVariance = 0.0;
        double referenceVariance = 0.0;

        for (int i = 0; i < spectrum.Length; ++i)
        {
            double spectrumDifference = spectrum[i] - spectrumMean;
            double referenceDifference = referenceSpectrum[i] - referenceMean;

            numerator += spectrumDifference * referenceDifference;
            spectrumVariance += spectrumDifference * spectrumDifference;
            referenceVariance += referenceDifference * referenceDifference;
        }

        if (spectrumVariance == 0.0 || referenceVariance == 0.0)
        {
            throw new ArgumentException(
                "Spectra must not have zero variance.");
        }

        double denominator =
            Math.Sqrt(spectrumVariance) *
            Math.Sqrt(referenceVariance);

        return (float)(numerator / denominator);
    }
}
