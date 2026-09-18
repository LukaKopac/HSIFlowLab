using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class MeanSpectrumAngleMapperTests
{
    [Fact]
    public void CalculateMeanSpectrum_ReturnsCorrectMean()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        cube[0, 1, 0] = 2.0f;
        cube[0, 1, 1] = 4.0f;
        cube[0, 1, 2] = 6.0f;

        cube[1, 0, 0] = 3.0f;
        cube[1, 0, 1] = 6.0f;
        cube[1, 0, 2] = 9.0f;

        cube[1, 1, 0] = 4.0f;
        cube[1, 1, 1] = 8.0f;
        cube[1, 1, 2] = 12.0f;

        var meanSpectrum =
            MeanSpectrumAngleMapper.CalculateMeanSpectrum(cube);

        Assert.Equal(2.5f, meanSpectrum[0], 5);
        Assert.Equal(5.0f, meanSpectrum[1], 5);
        Assert.Equal(7.5f, meanSpectrum[2], 5);
    }

    [Fact]
    public void CalculateMeanSpectrum_PreservesBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[3, 5, 4]
        };

        var meanSpectrum =
            MeanSpectrumAngleMapper.CalculateMeanSpectrum(cube);

        Assert.Equal(4, meanSpectrum.Length);
    }

    [Fact]
    public void CalculateMeanSpectrum_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => MeanSpectrumAngleMapper.CalculateMeanSpectrum(
                null!));
    }

    [Fact]
    public void CalculateMeanSpectrum_IdenticalPixels_ReturnsSameSpectrum()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                cube[y, x, 0] = 1.0f;
                cube[y, x, 1] = 2.0f;
                cube[y, x, 2] = 3.0f;
            }
        }

        var meanSpectrum =
            MeanSpectrumAngleMapper.CalculateMeanSpectrum(cube);

        Assert.Equal(1.0f, meanSpectrum[0], 5);
        Assert.Equal(2.0f, meanSpectrum[1], 5);
        Assert.Equal(3.0f, meanSpectrum[2], 5);
    }

    [Fact]
    public void CalculateAngleMap_IdenticalPixels_ReturnsZeroAngles()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 2]
        };

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                cube[y, x, 0] = 1.0f;
                cube[y, x, 1] = 0.0f;
            }
        }

        var angleMap =
            MeanSpectrumAngleMapper.CalculateAngleMap(cube);

        for (int y = 0; y < cube.Height; y++)
        {
            for (int x = 0; x < cube.Width; x++)
            {
                Assert.Equal(0.0f, angleMap[y, x], 5);
            }
        }
    }

    [Fact]
    public void CalculateAngleMap_CalculatesAnglesRelativeToMean()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 2, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        cube[0, 1, 0] = 0.0f;
        cube[0, 1, 1] = 1.0f;

        var angleMap =
            MeanSpectrumAngleMapper.CalculateAngleMap(cube);

        Assert.Equal(45.0f, angleMap[0, 0], 5);
        Assert.Equal(45.0f, angleMap[0, 1], 5);
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

        var angleMap =
            MeanSpectrumAngleMapper.CalculateAngleMap(cube);

        Assert.Equal(3, angleMap.Height);
        Assert.Equal(5, angleMap.Width);
    }

    [Fact]
    public void Apply_CreatesCorrectMask()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 2, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        cube[0, 1, 0] = 0.0f;
        cube[0, 1, 1] = 1.0f;

        var mask = MeanSpectrumAngleMapper.Apply(
            cube,
            45.0f);

        Assert.True(mask[0, 0]);
        Assert.True(mask[0, 1]);
    }

    [Fact]
    public void Apply_IncludesAngleEqualToThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 2, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        cube[0, 1, 0] = 0.0f;
        cube[0, 1, 1] = 1.0f;

        var mask = MeanSpectrumAngleMapper.Apply(
            cube,
            45.0f);

        Assert.True(mask[0, 0]);
        Assert.True(mask[0, 1]);
    }

    [Fact]
    public void Apply_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => MeanSpectrumAngleMapper.Apply(
                null!,
                10.0f));
    }

    [Fact]
    public void Apply_RejectsNegativeThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => MeanSpectrumAngleMapper.Apply(
                cube,
                -1.0f));
    }

    [Fact]
    public void Apply_RejectsThresholdAbove180()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 0.0f;

        Assert.Throws<ArgumentOutOfRangeException>(
            () => MeanSpectrumAngleMapper.Apply(
                cube,
                181.0f));
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

        var mask = MeanSpectrumAngleMapper.Apply(
            cube,
            10.0f);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }
}
