using HSIDataKit.Models;

namespace MaskingKit;

public static class TopContrastMapper
{
    public static float CalculateContrast(float[,] band)
    {
        ArgumentNullException.ThrowIfNull(band);

        if (band.Length == 0)
        {
            throw new ArgumentException(
                "Band must contain at least one value",
                nameof(band));
        }

        float min = band[0, 0];
        float max = band[0, 0];

        double sum = 0.0;

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                float value = band[y, x];

                min = Math.Min(min, value);
                max = Math.Max(max, value);
                sum += value;
            }
        }

        double mean = sum / band.Length;

        if (max == min)
        {
            return 0.0f;
        }

        double squaredDifferenceSum = 0.0;

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                double difference = band[y, x] - mean;
                squaredDifferenceSum += difference * difference;
            }
        }

        double standardDeviation =
            Math.Sqrt(squaredDifferenceSum /  band.Length);

        return (float)(standardDeviation / (max - min));
    }

    public static int[] FindTopContrastBands(
        HsiCube cube,
        int topN)
    {
        ArgumentNullException.ThrowIfNull(cube);

        if (topN <= 0 || topN > cube.Bands)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topN),
                "topN must be greater than zero and no greater than the number of bands.");
        }

        var contrastValues = new float[cube.Bands];

        for (int bandIndex = 0; bandIndex < cube.Bands; bandIndex++)
        {
            var band = new float[cube.Height, cube.Width];

            for (int y = 0; y < cube.Height; y++)
            {
                for (int x = 0; x < cube.Width; x++)
                {
                    band[y, x] = cube[y, x, bandIndex];
                }
            }

            contrastValues[bandIndex] = CalculateContrast(band);
        }

        return Enumerable
            .Range(0, cube.Bands)
            .OrderByDescending(index => contrastValues[index])
            .Take(topN)
            .ToArray();
    }

    public static float[,] NormalizeBand(float[,] band)
    {
        ArgumentNullException.ThrowIfNull(band);

        if (band.Length == 0)
        {
            throw new ArgumentException(
                "Band must contain at least one value.",
                nameof(band));
        }

        float min = band[0, 0];
        float max = band[0, 0];

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                float value = band[y, x];

                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }

        var result = new float[
            band.GetLength(0),
            band.GetLength(1)];

        if (max == min)
        {
            return result;
        }

        float range = max - min;

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                result[y, x] =
                    (band[y, x] - min) / range;
            }
        }

        return result;
    }

    public static float[,] BoostContrast(
        float[,] band,
        float low,
        float high)
    {
        ArgumentNullException.ThrowIfNull(band);

        if (band.Length == 0)
        {
            throw new ArgumentException(
                "Band must contain at least one value.",
                nameof(band));
        }

        if (low >= high)
        {
            throw new ArgumentException(
                "Low limit must be less than high limit.");
        }

        var result = new float[
            band.GetLength(0),
            band.GetLength(1)];

        float range = high - low;

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                float value = band[y, x];

                value = (value - low) / range;

                result[y, x] = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        return result;
    }

    public static float CalculateQuantile(
        float[] values,
        float quantile)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.Length == 0)
        {
            throw new ArgumentException(
                "Values must contain at least one value.",
                nameof(values));
        }

        if (quantile < 0.0f || quantile > 1.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantile),
                "Quantile must be between 0 and 1.");
        }

        var sortedValues = (float[])values.Clone();
        Array.Sort(sortedValues);

        if (sortedValues.Length == 1)
        {
            return sortedValues[0];
        }

        // NumPy's default linear interpolation:
        // index = q * (n - 1)
        double index = quantile * (sortedValues.Length - 1);

        int lowerIndex = (int)Math.Floor(index);
        int upperIndex = (int)Math.Ceiling(index);

        if (lowerIndex == upperIndex)
        {
            return sortedValues[lowerIndex];
        }

        double fraction = index - lowerIndex;

        return (float)(
            sortedValues[lowerIndex] +
            fraction *
            (sortedValues[upperIndex] - sortedValues[lowerIndex]));
    }

    public static float[,] ApplyShadowThreshold(
        float[,] band,
        float quantile)
    {
        ArgumentNullException.ThrowIfNull(band);

        if (band.Length == 0)
        {
            throw new ArgumentException(
                "Band must contain at least one value.",
                nameof(band));
        }

        if (quantile < 0.0f || quantile > 1.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantile),
                "Quantile must be between 0 and 1.");
        }

        var values = new float[band.Length];

        int index = 0;

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                values[index++] = band[y, x];
            }
        }

        float threshold =
            CalculateQuantile(values, quantile);

        var result = new float[
            band.GetLength(0),
            band.GetLength(1)];

        for (int y = 0; y < band.GetLength(0); y++)
        {
            for (int x = 0; x < band.GetLength(1); x++)
            {
                float value = band[y, x];

                result[y, x] =
                    value < threshold
                        ? 0.0f
                        : value;
            }
        }

        return result;
    }

    public static Mask CreateBandMask(
        float[,] band,
        float low,
        float high,
        float shadowQuantile)
    {
        ArgumentNullException.ThrowIfNull(band);

        var normalizedBand = NormalizeBand(band);

        var boostedBand = BoostContrast(
            normalizedBand,
            low,
            high);

        var shadowThresholdedBand = ApplyShadowThreshold(
            boostedBand,
            shadowQuantile);

        var image = new Image(shadowThresholdedBand);

        return Threshold.Otsu(image);
    }

    public static Mask CombineMasks(
    Mask[] masks,
    int requiredVotes)
    {
        ArgumentNullException.ThrowIfNull(masks);

        if (masks.Length == 0)
        {
            throw new ArgumentException(
                "At least one mask is required.",
                nameof(masks));
        }

        if (requiredVotes <= 0 || requiredVotes > masks.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requiredVotes),
                "Required votes must be greater than zero and no greater than the number of masks.");
        }

        var height = masks[0].Height;
        var width = masks[0].Width;

        for (int i = 0; i < masks.Length; i++)
        {
            ArgumentNullException.ThrowIfNull(masks[i]);

            if (masks[i].Height != height ||
                masks[i].Width != width)
            {
                throw new ArgumentException(
                    "All masks must have the same dimensions.",
                    nameof(masks));
            }
        }

        var result = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int votes = 0;

                for (int i = 0; i < masks.Length; i++)
                {
                    if (masks[i][y, x])
                    {
                        votes++;
                    }
                }

                result[y, x] = votes >= requiredVotes;
            }
        }

        return new Mask(result);
    }

    public static Mask Apply(
        HsiCube cube,
        int topN = 5,
        float low = 0.3f,
        float high = 0.7f,
        float shadowQuantile = 0.1f)
    {
        ArgumentNullException.ThrowIfNull(cube);

        if (topN <= 0 || topN > cube.Bands)
        {
            throw new ArgumentOutOfRangeException(
                nameof(topN),
                "topN must be greater than zero and no greater than the number of bands.");
        }

        if (low >= high)
        {
            throw new ArgumentException(
                "Low limit must be less than high limit.");
        }

        if (shadowQuantile < 0.0f ||
            shadowQuantile > 1.0f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(shadowQuantile),
                "Shadow quantile must be between 0 and 1.");
        }

        int[] topBands =
            FindTopContrastBands(cube, topN);

        var masks = new Mask[topBands.Length];

        for (int i = 0; i < topBands.Length; i++)
        {
            var band = cube.GetBand(topBands[i]);

            masks[i] = CreateBandMask(
                band,
                low,
                high,
                shadowQuantile);
        }

        int requiredVotes =
            topN / 2 + 1;

        return CombineMasks(
            masks,
            requiredVotes);
    }
}
