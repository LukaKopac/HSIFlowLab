using HSIDataKit.Models;

namespace MaskingKit;

public static class MeanSpectrumAngleMapper
{
    public static float[] CalculateMeanSpectrum(HsiCube cube)
    {
        ArgumentNullException.ThrowIfNull(cube);

        var meanSpectrum = new float[cube.Bands];

        int pixelCount = cube.Height * cube.Width;

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                var spectrum = cube.GetSpectrum(y, x);

                for (int band = 0; band < cube.Bands; band++)
                {
                    meanSpectrum[band] += spectrum[band];
                }
            }
        }

        for (int band = 0; band < cube.Bands; band++)
        {
            meanSpectrum[band] /= pixelCount;
        }

        return meanSpectrum;
    }

    public static Image CalculateAngleMap(HsiCube cube)
    {
        ArgumentNullException.ThrowIfNull(cube);

        var meanSpectrum = CalculateMeanSpectrum(cube);

        var result = new float[cube.Height, cube.Width];

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                result[y, x] = SpectralAngleMapper.CalculateAngle(
                    cube.GetSpectrum(y, x),
                    meanSpectrum);
            }
        }

        return new Image(result);
    }

    public static Mask Apply(
        HsiCube cube,
        float thresholdDegrees)
    {
        ArgumentNullException.ThrowIfNull(cube);

        if (thresholdDegrees < 0.0f || thresholdDegrees > 180.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(thresholdDegrees),
                "Threshold must be between 0 and 180 degrees.");
        }

        var angleMap = CalculateAngleMap(cube);

        return Threshold.LessThanOrEqual(
            angleMap,
            thresholdDegrees);
    }
}
