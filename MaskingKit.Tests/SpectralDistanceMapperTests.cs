using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class SpectrumDistanceMapperTests
{
    [Fact]
    public void CalculateMeanSquaredDistance_IdenticalSpectra_ReturnsZero()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var result = SpectralDistanceMapper.CalculateMeanSquaredDistance(
            spectrum,
            spectrum);

        Assert.Equal(0.0f, result, 5);
    }

    [Fact]
    public void CalculateMeanSquaredDistance_ReturnsExpectedValue()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
            2.0f, 4.0f, 2.0f
        };

        // Squared differences: 1, 4, 1
        // MSD = 6 / 3 = 2
        var result = SpectralDistanceMapper.CalculateMeanSquaredDistance(
            spectrum,
            reference);

        Assert.Equal(2.0f, result, 5);
    }

    [Fact]
    public void CalculateMeanSquaredDistance_IsSymmetric()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
            2.0f, 4.0f, 2.0f
        };

        var result1 =
            SpectralDistanceMapper.CalculateMeanSquaredDistance(
                spectrum,
                reference);

        var result2 =
            SpectralDistanceMapper.CalculateMeanSquaredDistance(
                reference,
                spectrum);

        Assert.Equal(result1, result2, 5);
    }

    [Fact]
    public void CalculateMeanSquaredDistanceMap_CreatesCorrectValues()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        // MSD = 0
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        // Differences: 1, 2, 1
        // MSD = (1 + 4 + 1) / 3 = 2
        cube[0, 1, 0] = 2.0f;
        cube[0, 1, 1] = 4.0f;
        cube[0, 1, 2] = 2.0f;

        // Differences: 2, 0, 2
        // MSD = (4 + 0 + 4) / 3 = 8/3
        cube[1, 0, 0] = 3.0f;
        cube[1, 0, 1] = 2.0f;
        cube[1, 0, 2] = 5.0f;

        // Differences: -1, -1, -1
        // MSD = 1
        cube[1, 1, 0] = 0.0f;
        cube[1, 1, 1] = 1.0f;
        cube[1, 1, 2] = 2.0f;

        var reference = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var distanceMap =
            SpectralDistanceMapper.CalculateMeanSquaredDistanceMap(
                cube,
                reference);

        Assert.Equal(0.0f, distanceMap[0, 0], 5);
        Assert.Equal(2.0f, distanceMap[0, 1], 5);
        Assert.Equal(8.0f / 3.0f, distanceMap[1, 0], 5);
        Assert.Equal(1.0f, distanceMap[1, 1], 5);
    }

    [Fact]
    public void CalculateMeanSquaredDistanceMap_PreservesDimensions()
    {
        var cube = new HsiCube
        {
            Data = new float[3, 5, 3]
        };

        var reference = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var distanceMap =
            SpectralDistanceMapper.CalculateMeanSquaredDistanceMap(
                cube,
                reference);

        Assert.Equal(3, distanceMap.Height);
        Assert.Equal(5, distanceMap.Width);
    }

    [Fact]
    public void CalculateMeanSquaredDistanceMap_RejectsReferenceSpectrumWithWrongBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentException>(
            () => SpectralDistanceMapper.CalculateMeanSquaredDistanceMap(
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

        // MSD = 0
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        // MSD = 2
        cube[0, 1, 0] = 2.0f;
        cube[0, 1, 1] = 4.0f;
        cube[0, 1, 2] = 2.0f;

        // MSD = 1
        cube[0, 2, 0] = 0.0f;
        cube[0, 2, 1] = 1.0f;
        cube[0, 2, 2] = 2.0f;

        var reference = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var mask = SpectralDistanceMapper.Apply(
            cube,
            reference,
            1.0f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.True(mask[0, 2]);
    }

    [Fact]
    public void Apply_IncludesDistanceEqualToThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 1, 3]
        };

        // MSD = 1
        cube[0, 0, 0] = 0.0f;
        cube[0, 0, 1] = 1.0f;
        cube[0, 0, 2] = 2.0f;

        var reference = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var mask = SpectralDistanceMapper.Apply(
            cube,
            reference,
            1.0f);

        Assert.True(mask[0, 0]);
    }

    [Fact]
    public void CalculateMeanSquaredDistance_RejectsSpectraWithDifferentLengths()
    {
        var spectrum = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var reference = new float[]
        {
            1.0f, 2.0f
        };

        Assert.Throws<ArgumentException>(
            () => SpectralDistanceMapper.CalculateMeanSquaredDistance(
                spectrum,
                reference));
    }

    [Fact]
    public void CalculateMeanSquaredDistance_RejectsEmptySpectrum()
    {
        Assert.Throws<ArgumentException>(
            () => SpectralDistanceMapper.CalculateMeanSquaredDistance(
                Array.Empty<float>(),
                Array.Empty<float>()));
    }

    [Fact]
    public void Apply_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => SpectralDistanceMapper.Apply(
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
            () => SpectralDistanceMapper.Apply(
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
            () => SpectralDistanceMapper.Apply(
                cube,
                new float[] { 1.0f, 2.0f },
                0.5f));
    }

    [Fact]
    public void Apply_AcceptsZeroThreshold()
    {
        var cube = new HsiCube
        {
            Data = new float[1, 2, 3]
        };

        // MSD = 0
        cube[0, 0, 0] = 1.0f;
        cube[0, 0, 1] = 2.0f;
        cube[0, 0, 2] = 3.0f;

        // MSD > 0
        cube[0, 1, 0] = 2.0f;
        cube[0, 1, 1] = 2.0f;
        cube[0, 1, 2] = 3.0f;

        var reference = new float[]
        {
            1.0f, 2.0f, 3.0f
        };

        var mask = SpectralDistanceMapper.Apply(
            cube,
            reference,
            0.0f);

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
    }
}
