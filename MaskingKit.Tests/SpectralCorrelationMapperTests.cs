using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class SpectralCorrelationMapperTests
{
    [Fact]
    public void CalculateCorrelation_IdenticalSpectra_ReturnsOne()
    {
        var spectrum = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var result = SpectralCorrelationMapper.CalculateCorrelation(
            spectrum,
            spectrum);

        Assert.Equal(1.0f, result, 5);
    }

    [Fact]
    public void CalculateCorrelation_InverselyRelatedSpectra_ReturnsNegativeOne()
    {
        var spectrum = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
        3.0f, 2.0f, 1.0f
        };

        var result = SpectralCorrelationMapper.CalculateCorrelation(
            spectrum,
            reference);

        Assert.Equal(-1.0f, result, 5);
    }

    [Fact]
    public void CalculateCorrelation_ScalingAndOffsetDoNotChangeCorrelation()
    {
        var spectrum = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
        12.0f, 14.0f, 16.0f
        };

        var result = SpectralCorrelationMapper.CalculateCorrelation(
            spectrum,
            reference);

        Assert.Equal(1.0f, result, 5);
    }

    [Fact]
    public void CalculateCorrelationMap_CreatesCorrectValues()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        // Perfect positive correlation
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        // Perfect negative correlation
        cube[0, 1, 0] = 3.0f;
        cube[0, 1, 1] = 2.0f;
        cube[0, 1, 2] = 1.0f;

        // Same shape, different scale and offset
        cube[1, 0, 0] = 12.0f;
        cube[1, 0, 1] = 14.0f;
        cube[1, 0, 2] = 16.0f;

        // Another perfect positive correlation
        cube[1, 1, 0] = 2.0f;
        cube[1, 1, 1] = 4.0f;
        cube[1, 1, 2] = 6.0f;

        var reference = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var correlationMap =
            SpectralCorrelationMapper.CalculateCorrelationMap(
                cube,
                reference);

        Assert.Equal(1.0f, correlationMap[0, 0], 5);
        Assert.Equal(-1.0f, correlationMap[0, 1], 5);
        Assert.Equal(1.0f, correlationMap[1, 0], 5);
        Assert.Equal(1.0f, correlationMap[1, 1], 5);
    }

    [Fact]
    public void CalculateCorrelationMap_PreservesDimensions()
    {
        var cube = new HsiCube
        {
            Data = new float[3, 5, 3]
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

        var reference = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var correlationMap =
            SpectralCorrelationMapper.CalculateCorrelationMap(
                cube,
                reference);

        Assert.Equal(3, correlationMap.Height);
        Assert.Equal(5, correlationMap.Width);
    }

    [Fact]
    public void CalculateCorrelationMap_RejectsReferenceSpectrumWithWrongBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentException>(
            () => SpectralCorrelationMapper.CalculateCorrelationMap(
                cube,
                new float[] { 1.0f, 2.0f }));
    }

    [Fact]
    public void Apply_CreatesCorrectMask()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 3, 3]
        };

        // Correlation = 1
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        // Correlation = -1
        cube[0, 1, 0] = 3.0f;
        cube[0, 1, 1] = 2.0f;
        cube[0, 1, 2] = 1.0f;

        // Correlation = 1
        cube[0, 2, 0] = 2.0f;
        cube[0, 2, 1] = 4.0f;
        cube[0, 2, 2] = 6.0f;

        var reference = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var mask = SpectralCorrelationMapper.Apply(
            cube,
            reference,
            0.5f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.True(mask[0, 2]);
    }

    [Fact]
    public void Apply_IncludesCorrelationEqualToThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 3]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        var reference = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var mask = SpectralCorrelationMapper.Apply(
            cube,
            reference,
            1.0f);

        Assert.True(mask[0, 0]);
    }

    [Fact]
    public void CalculateCorrelation_RejectsZeroVarianceSpectrum()
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
            () => SpectralCorrelationMapper.CalculateCorrelation(
                spectrum,
                reference));
    }

    [Fact]
    public void Apply_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => SpectralCorrelationMapper.Apply(
                null!,
                new float[] { 1.0f },
                0.5f));
    }

    [Fact]
    public void Apply_RejectsNullReferenceSpectrum()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 2]
        };

        Assert.Throws<ArgumentNullException>(
            () => SpectralCorrelationMapper.Apply(
                cube,
                null!,
                0.5f));
    }

    [Fact]
    public void Apply_RejectsReferenceSpectrumWithWrongBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 3]
        };

        Assert.Throws<ArgumentException>(
            () => SpectralCorrelationMapper.Apply(
                cube,
                new float[] { 1.0f, 0.0f },
                0.5f));
    }

    [Fact]
    public void Apply_AcceptsNegativeThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 2, 3]
        };

        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        cube[0, 1, 0] = 3.0f;
        cube[0, 1, 1] = 2.0f;
        cube[0, 1, 2] = 1.0f;

        var reference = new float[]
        {
        1.0f, 2.0f, 3.0f
        };

        var mask = SpectralCorrelationMapper.Apply(
            cube,
            reference,
            -1.0f);

        Assert.True(mask[0, 0]);
        Assert.True(mask[0, 1]);
    }
}
