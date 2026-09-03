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
}