using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class ThresholdTests
{
    [Fact]
    public void GreaterThanOrEqual_CreatesCorrectMask()
    {
        var image = new Image(new float[,]
        {
            { 0.1f, 0.8f },
            { 0.9f, 0.2f }
        });

        var mask = Threshold.GreaterThanOrEqual(image, 0.7f);

        Assert.False(mask[0, 0]);
        Assert.True(mask[0, 1]);
        Assert.True(mask[1, 0]);
        Assert.False(mask[1, 1]);
    }

    [Fact]
    public void GreaterThanOrEqual_IncludesValueEqualToThreshold()
    {
        var image = new Image(new float[,]
        {
            { 0.5f, 0.6f }
        });

        var mask = Threshold.GreaterThanOrEqual(image, 0.5f);

        Assert.True(mask[0, 0]);
        Assert.True(mask[0, 1]);
    }

    [Fact]
    public void GreaterThanOrEqual_PreservesDimensions()
    {
        var image = new Image(new float[3, 5]);

        var mask = Threshold.GreaterThanOrEqual(image, 0.5f);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void GreaterThanOrEqual_RejectsNullImage()
    {
        Assert.Throws<ArgumentNullException>(
            () => Threshold.GreaterThanOrEqual(null!, 0.5f));
    }

    [Fact]
    public void GreaterThanOrEqual_DoesNotModifyImage()
    {
        var image = new Image(new float[,]
        {
        { 0.2f, 0.8f },
        { 0.4f, 0.9f }
        });

        Threshold.GreaterThanOrEqual(image, 0.5f);

        Assert.Equal(0.2f, image[0, 0]);
        Assert.Equal(0.8f, image[0, 1]);
        Assert.Equal(0.4f, image[1, 0]);
        Assert.Equal(0.9f, image[1, 1]);
    }

    [Fact]
    public void GreaterThanOrEqual_WithThresholdAboveAllValues_ReturnsEmptyMask()
    {
        var image = new Image(new float[,]
        {
        { 0.2f, 0.4f },
        { 0.6f, 0.8f }
        });

        var mask = Threshold.GreaterThanOrEqual(image, 1.0f);

        Assert.Equal(0, mask.Count);
    }

    [Fact]
    public void GreaterThanOrEqual_WithThresholdBelowAllValues_ReturnsFullMask()
    {
        var image = new Image(new float[,]
        {
        { 0.2f, 0.4f },
        { 0.6f, 0.8f }
        });

        var mask = Threshold.GreaterThanOrEqual(image, 0.0f);

        Assert.Equal(4, mask.Count);
    }

    [Fact]
    public void LessThanOrEqual_CreatesCorrectMask()
    {
        var image = new Image(new float[,]
        {
        { 0.1f, 0.8f },
        { 0.9f, 0.2f }
        });

        var mask = Threshold.LessThanOrEqual(image, 0.7f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.False(mask[1, 0]);
        Assert.True(mask[1, 1]);
    }

    [Fact]
    public void LessThanOrEqual_IncludesValueEqualToThreshold()
    {
        var image = new Image(new float[,]
        {
        { 0.5f, 0.6f }
        });

        var mask = Threshold.LessThanOrEqual(image, 0.5f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
    }

    [Fact]
    public void NonZero_ConvertsZeroToFalseAndNonzeroToTrue()
    {
        var image = new Image(new float[,]
        {
        { 0.0f, 0.5f },
        { 1.0f, 0.0f }
        });

        var mask = Threshold.NonZero(image);

        Assert.False(mask[0, 0]);
        Assert.True(mask[0, 1]);
        Assert.True(mask[1, 0]);
        Assert.False(mask[1, 1]);
    }

    [Fact]
    public void NonZero_PreservesDimensions()
    {
        var image = new Image(new float[3, 5]);

        var mask = Threshold.NonZero(image);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void NonZero_DoesNotModifyImage()
    {
        var image = new Image(new float[,]
        {
        { 0.2f, 0.0f },
        { 1.0f, 0.5f }
        });

        Threshold.NonZero(image);

        Assert.Equal(0.2f, image[0, 0]);
        Assert.Equal(0.0f, image[0, 1]);
        Assert.Equal(1.0f, image[1, 0]);
        Assert.Equal(0.5f, image[1, 1]);
    }

    [Fact]
    public void NonZero_RejectsNullImage()
    {
        Assert.Throws<ArgumentNullException>(
            () => Threshold.NonZero(null!));
    }

    [Fact]
    public void Otsu_SeparatesTwoDistinctGroups()
    {
        var image = new Image(new float[,]
        {
        { 0.1f, 0.1f, 0.9f, 0.9f },
        { 0.1f, 0.1f, 0.9f, 0.9f }
        });

        var result = Threshold.Otsu(image);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.True(result[0, 2]);
        Assert.True(result[0, 3]);

        Assert.False(result[1, 0]);
        Assert.False(result[1, 1]);
        Assert.True(result[1, 2]);
        Assert.True(result[1, 3]);
    }

    [Fact]
    public void Otsu_SeparatesLowAndHighValues()
    {
        var image = new Image(new float[,]
        {
        { 0.0f, 0.0f, 1.0f, 1.0f }
        });

        var result = Threshold.Otsu(image);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.True(result[0, 2]);
        Assert.True(result[0, 3]);
    }

    [Fact]
    public void Otsu_RejectsNullImage()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Threshold.Otsu(null!));
    }

    [Fact]
    public void Otsu_ConstantImage_ReturnsEmptyMask()
    {
        var image = new Image(new float[,]
        {
        { 0.5f, 0.5f },
        { 0.5f, 0.5f }
        });

        var result = Threshold.Otsu(image);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.False(result[1, 0]);
        Assert.False(result[1, 1]);
    }

    [Fact]
    public void Otsu_WorksWithValuesOutsideNormalizedRange()
    {
        var image = new Image(new float[,]
        {
        { 10.0f, 10.0f, 100.0f, 100.0f }
        });

        var result = Threshold.Otsu(image);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.True(result[0, 2]);
        Assert.True(result[0, 3]);
    }
}
