using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class SpectralAngleMapperTests
{
    [Fact]
    public void CalculateAngle_IdenticalSpectra_ReturnsZero()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var angle = SpectralAngleMapper.CalculateAngle(
            spectrum,
            spectrum);

        Assert.Equal(0.0f, angle, 5);
    }

    [Fact]
    public void CalculateAngle_PerpendicularSpectra_Returns90Degrees()
    {
        var spectrum = new float[]
        {
            1.0f, 0.0f
        };

        var reference = new float[]
        {
            0.0f, 1.0f
        };

        var angle = SpectralAngleMapper.CalculateAngle(
            spectrum,
            reference);

        Assert.Equal(90.0f, angle, 5);
    }

    [Fact]
    public void CalculateAngle_OppositeSpectra_Returns180Degrees()
    {
        var spectrum = new float[]
        {
            1.0f, 0.0f
        };

        var reference = new float[]
        {
            -1.0f, 0.0f
        };

        var angle = SpectralAngleMapper.CalculateAngle(
            spectrum,
            reference);

        Assert.Equal(180.0f, angle, 5);
    }

    [Fact]
    public void CalculateAngle_ScalingSpectrumDoesNotChangeAngle()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
            2.0f, 4.0f, 6.0f
        };

        var angle = SpectralAngleMapper.CalculateAngle(
            spectrum,
            reference);

        Assert.Equal(0.0f, angle, 5);
    }

    [Fact]
    public void Apply_CreatesCorrectMask()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 2]
        };

        // Pixel [0,0] -> identical to reference
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        // Pixel [0,1] -> 45 degrees
        cube[0, 1, 0] = 1.0f;
        cube[0, 1, 1] = 1.0f;

        // Pixel [1,0] -> 90 degrees
        cube[1, 0, 0] = 0.0f;
        cube[1, 0, 1] = 1.0f;

        // Pixel [1,1] -> identical direction, different magnitude
        cube[1, 1, 0] = 5.0f;
        cube[1, 1, 1] = 0.0f;

        var reference = new float[]
        {
            1.0f, 0.0f
        };

        var mask = SpectralAngleMapper.Apply(
            cube,
            reference,
            10.0f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.False(mask[1, 0]);
        Assert.True(mask[1, 1]);
    }

    [Fact]
    public void Apply_IncludesAngleEqualToThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 1.0f;

        var reference = new float[]
        {
            1.0f, 0.0f
        };

        var mask = SpectralAngleMapper.Apply(
            cube,
            reference,
            45.0f);

        Assert.True(mask[0, 0]);
    }

    [Fact]
    public void Apply_PreservesDimensions()
    {
        var cube = new HsiCube
        {
            Data = new float[3, 5, 2]
        };

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                cube[y, x, 0] = 1.0f;
                cube[y, x, 1] = 0.0f;
            }
        }

        var reference = new float[]
        {
        1.0f, 0.0f
        };

        var mask = SpectralAngleMapper.Apply(
            cube,
            reference,
            10.0f);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void CalculateAngle_RejectsZeroMagnitudeSpectrum()
    {
        var spectrum = new float[]
        {
        0.0f, 0.0f
        };

        var reference = new float[]
        {
        1.0f, 0.0f
        };

        Assert.Throws<ArgumentException>(
            () => SpectralAngleMapper.CalculateAngle(
                spectrum,
                reference));
    }

    [Fact]
    public void Apply_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => SpectralAngleMapper.Apply(
                null!,
                new float[] { 1.0f },
                10.0f));
    }

    [Fact]
    public void Apply_RejectsNullReferenceSpectrum()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        Assert.Throws<ArgumentNullException>(
            () => SpectralAngleMapper.Apply(
                cube,
                null!,
                10.0f));
    }

    [Fact]
    public void Apply_RejectsReferenceSpectrumWithWrongBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 3]
        };

        Assert.Throws<ArgumentException>(
            () => SpectralAngleMapper.Apply(
                cube,
                new float[] { 1.0f, 0.0f },
                10.0f));
    }

    [Fact]
    public void Apply_RejectsNegativeThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpectralAngleMapper.Apply(
                cube,
                new float[] { 1.0f, 0.0f },
                -1.0f));
    }

    [Fact]
    public void Apply_RejectsThresholdAbove180()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => SpectralAngleMapper.Apply(
                cube,
                new float[] { 1.0f, 0.0f },
                181.0f));
    }

    [Fact]
    public void CalculateAngleMap_CreatesCorrectValues()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 2]
        };

        // 0 degrees
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        // 45 degrees
        cube[0, 1, 0] = 1.0f;
        cube[0, 1, 1] = 1.0f;

        // 90 degrees
        cube[1, 0, 0] = 0.0f;
        cube[1, 0, 1] = 1.0f;

        // Same direction, different magnitude -> 0 degrees
        cube[1, 1, 0] = 5.0f;
        cube[1, 1, 1] = 0.0f;

        var reference = new float[]
        {
        1.0f, 0.0f
        };

        var angleMap = SpectralAngleMapper.CalculateAngleMap(
            cube,
            reference);

        Assert.Equal(0.0f, angleMap[0, 0], 5);
        Assert.Equal(45.0f, angleMap[0, 1], 5);
        Assert.Equal(90.0f, angleMap[1, 0], 5);
        Assert.Equal(0.0f, angleMap[1, 1], 5);
    }

    [Fact]
    public void CalculateAngleMap_PreservesDimensions()
    {
        var cube = new HsiCube
        {
            Data = new float[3, 5, 2]
        };

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                cube[y, x, 0] = 1.0f;
                cube[y, x, 1] = 0.0f;
            }
        }

        var reference = new float[]
        {
        1.0f, 0.0f
        };

        var angleMap = SpectralAngleMapper.CalculateAngleMap(
            cube,
            reference);

        Assert.Equal(3, angleMap.Height);
        Assert.Equal(5, angleMap.Width);
    }

    [Fact]
    public void CalculateAngleMap_RejectsReferenceSpectrumWithWrongBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentException>(
            () => SpectralAngleMapper.CalculateAngleMap(
                cube,
                new float[] { 1.0f, 0.0f }));
    }
}
