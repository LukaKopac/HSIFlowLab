using HSIDataKit.Models;

namespace MaskingKit;

public static class SpectralDistanceMapper
{
    public static Mask Apply(
        HsiCube cube,
        float[] referenceSpectrum,
        float threshold)
    {
        ArgumentNullException.ThrowIfNull(cube);
        ArgumentNullException.ThrowIfNull(referenceSpectrum);

        var distanceMap = CalculateMeanSquaredDistanceMap(
            cube,
            referenceSpectrum);

        return Threshold.LessThanOrEqual(
            distanceMap,
            threshold);
    }

    public static Image CalculateMeanSquaredDistanceMap(
        HsiCube cube,
        float[] referenceSpectrum)
    {
        if (referenceSpectrum.Length != cube.Bands)
        {
            throw new ArgumentException(
                "Reference spectra must have the same number of bands as the cube",
                nameof(referenceSpectrum));
        }

        var result = new float[cube.Height, cube.Width];

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                result[y, x] = CalculateMeanSquaredDistance(
                    cube.GetSpectrum(y, x),
                    referenceSpectrum);
            }
        }

        return new Image(result);
    }

    public static float CalculateMeanSquaredDistance(
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

        double sum = 0.0;

        for (int i = 0; i < spectrum.Length; i++)
        {
            double difference =
                spectrum[i] - referenceSpectrum[i];

            sum += difference * difference;
        }

        return (float)(sum / spectrum.Length);
    }
}
