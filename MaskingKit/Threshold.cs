using HSIDataKit.Models;

namespace MaskingKit;

public static class Threshold
{
    public static Mask GreaterThanOrEqual(Image image, float threshold)
    {
        ArgumentNullException.ThrowIfNull(image);

        var result = new bool[image.Height, image.Width];

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                result[y, x] = image[y, x] >= threshold;
            }
        }

        return new Mask(result);
    }

    public static Mask LessThanOrEqual(Image image, float threshold)
    {
        ArgumentNullException.ThrowIfNull(image);

        var result = new bool[image.Height, image.Width];

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                result[y, x] = image[y, x] <= threshold;
            }
        }

        return new Mask(result);
    }

    public static Mask NonZero(Image image)
    {
        ArgumentNullException.ThrowIfNull(image);

        var result = new bool[image.Height, image.Width];

        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                result[y, x] = image[y, x] != 0.0f;
            }
        }

        return new Mask(result);
    }

    public static Mask Otsu(Image image)
    {
        ArgumentNullException.ThrowIfNull(image);

        const int binCount = 256;

        float min = image[0, 0];
        float max = image[0, 0];

        // Find image range.
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                float value = image[y, x];

                min = Math.Min(min, value);
                max = Math.Max(max, value);
            }
        }

        // A constant image has no meaningful threshold.
        if (min == max)
        {
            return new Mask(
                new bool[image.Height, image.Width]);
        }

        float range = max - min;

        var histogram = new int[binCount];

        // Build histogram using the actual image range.
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                float value = image[y, x];

                int bin = (int)(
                    (value - min) /
                    range *
                    (binCount - 1));

                bin = Math.Clamp(bin, 0, binCount - 1);

                histogram[bin]++;
            }
        }

        int totalPixels = image.Height * image.Width;

        double totalSum = 0.0;

        for (int i = 0; i < binCount; i++)
        {
            totalSum += i * histogram[i];
        }

        int backgroundPixels = 0;
        double backgroundSum = 0.0;

        double maximumVariance = -1.0;
        int thresholdBin = 0;

        for (int i = 0; i < binCount; i++)
        {
            backgroundPixels += histogram[i];

            if (backgroundPixels == 0)
            {
                continue;
            }

            int foregroundPixels =
                totalPixels - backgroundPixels;

            if (foregroundPixels == 0)
            {
                break;
            }

            backgroundSum += i * histogram[i];

            double backgroundMean =
                backgroundSum / backgroundPixels;

            double foregroundMean =
                (totalSum - backgroundSum) / foregroundPixels;

            double difference =
                backgroundMean - foregroundMean;

            double betweenClassVariance =
                backgroundPixels *
                (double)foregroundPixels *
                difference *
                difference;

            if (betweenClassVariance > maximumVariance)
            {
                maximumVariance = betweenClassVariance;
                thresholdBin = i;
            }
        }

        float threshold =
            min +
            (thresholdBin + 1) /
            (float)(binCount - 1) *
            range;

        return GreaterThanOrEqual(image, threshold);
    }
}