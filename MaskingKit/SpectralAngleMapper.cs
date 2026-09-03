using HSIDataKit.Models;

namespace MaskingKit;

public static class SpectralAngleMapper
{
    public static Mask Apply(
        HsiCube cube,
        float[] referenceSpectrum,
        float thresholdDegrees)
    {
        ArgumentNullException.ThrowIfNull(cube);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        if (thresholdDegrees < 0.0f || thresholdDegrees > 180.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(thresholdDegrees),
                "Threshold must be between 0 and 180 degrees.");
        }

        var angleMap = CalculateAngleMap(
            cube,
            referenceSpectrum);

        return Threshold.LessThanOrEqual(
            angleMap,
            thresholdDegrees);
    }

    public static Image CalculateAngleMap(
        HsiCube cube,
        float[] referenceSpectrum)
    {
        ArgumentNullException.ThrowIfNull(cube);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        if (referenceSpectrum.Length != cube.Bands)
        {
            throw new ArgumentException(
                "Reference spectrum must have the same number of bands as the cube.",
                nameof(referenceSpectrum));
        }

        var result = new float[cube.Height, cube.Width];

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                result[y, x] = CalculateAngle(
                    cube.GetSpectrum(y, x),
                    referenceSpectrum);
            }
        }

        return new Image(result);
    }

    public static float CalculateAngle(
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
                "Spectra must contain at least one value.",
                nameof(spectrum));
        }

        double dotProduct = 0.0;
        double spectrumMagnitude = 0.0;
        double referenceMagnitude = 0.0;

        for (int i = 0; i < spectrum.Length; i++)
        {
            dotProduct += spectrum[i] * referenceSpectrum[i];
            spectrumMagnitude += spectrum[i] * spectrum[i];
            referenceMagnitude += referenceSpectrum[i] * referenceSpectrum[i];
        }

        if (spectrumMagnitude == 0.0 || referenceMagnitude == 0.0)
        {
            throw new ArgumentException(
                "Spectra must not have zero magnitude.");
        }

        double denominator =
            Math.Sqrt(spectrumMagnitude) *
            Math.Sqrt(referenceMagnitude);

        double cosine = dotProduct / denominator;

        // Protect against small floating-point errors
        cosine = Math.Clamp(cosine, -1.0, 1.0);

        double angleRadians = Math.Acos(cosine);

        return (float)(angleRadians * 180.0 / Math.PI);
    }
}

