using HSIDataKit.Models;

namespace MaskingKit.Tests;

public class TopContrastMapperTests
{
    [Fact]
    public void CalculateContrast_ConstantBand_ReturnsZero()
    {
        var band = new float[,]
        {
            { 1.0f, 1.0f },
            { 1.0f, 1.0f }
        };

        var result = TopContrastMapper.CalculateContrast(band);

        Assert.Equal(0.0f, result, 5);
    }

    [Fact]
    public void CalculateContrast_ReturnsExpectedValue()
    {
        var band = new float[,]
        {
            { 0.0f, 0.0f },
            { 1.0f, 1.0f }
        };

        // Mean = 0.5
        // Standard deviation = 0.5
        // Range = 1
        // Contrast = 0.5 / 1 = 0.5
        var result = TopContrastMapper.CalculateContrast(band);

        Assert.Equal(0.5f, result, 5);
    }

    [Fact]
    public void CalculateContrast_HigherContrastBand_ReturnsHigherValue()
    {
        var lowContrastBand = new float[,]
        {
            { 0.4f, 0.5f },
            { 0.5f, 0.6f }
        };

        var highContrastBand = new float[,]
        {
            { 0.0f, 0.0f },
            { 1.0f, 1.0f }
        };

        var lowContrast =
            TopContrastMapper.CalculateContrast(lowContrastBand);

        var highContrast =
            TopContrastMapper.CalculateContrast(highContrastBand);

        Assert.True(highContrast > lowContrast);
    }

    [Fact]
    public void CalculateContrast_RejectsEmptyBand()
    {
        Assert.Throws<ArgumentException>(
            () => TopContrastMapper.CalculateContrast(
                new float[0, 0]));
    }

    [Fact]
    public void FindTopContrastBands_ReturnsHighestContrastBands()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 4]
        };

        // Band 0: low contrast
        cube[0, 0, 0] = 0.4f;
        cube[0, 1, 0] = 0.5f;
        cube[1, 0, 0] = 0.5f;
        cube[1, 1, 0] = 0.6f;

        // Band 1: moderate contrast
        cube[0, 0, 1] = 0.3f;
        cube[0, 1, 1] = 0.4f;
        cube[1, 0, 1] = 0.5f;
        cube[1, 1, 1] = 0.6f;

        // Band 2: high contrast
        cube[0, 0, 2] = 0.0f;
        cube[0, 1, 2] = 0.0f;
        cube[1, 0, 2] = 1.0f;
        cube[1, 1, 2] = 1.0f;

        // Band 3: constant
        cube[0, 0, 3] = 0.5f;
        cube[0, 1, 3] = 0.5f;
        cube[1, 0, 3] = 0.5f;
        cube[1, 1, 3] = 0.5f;

        var result =
            TopContrastMapper.FindTopContrastBands(cube, 2);

        Assert.Equal(2, result.Length);
        Assert.Equal(2, result[0]);
        Assert.Equal(1, result[1]);
    }

    [Fact]
    public void FindTopContrastBands_ReturnsBandsInDescendingContrastOrder()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        var lowContrastBand = new float[,]
        {
        { 0.45f, 0.50f },
        { 0.50f, 0.55f }
        };

        var mediumContrastBand = new float[,]
        {
        { 0.0f, 0.5f },
        { 0.5f, 0.5f }
        };

        var highContrastBand = new float[,]
        {
        { 0.0f, 0.0f },
        { 1.0f, 1.0f }
        };

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                cube[y, x, 0] = lowContrastBand[y, x];
                cube[y, x, 1] = mediumContrastBand[y, x];
                cube[y, x, 2] = highContrastBand[y, x];
            }
        }

        var result =
            TopContrastMapper.FindTopContrastBands(cube, 3);

        Assert.Equal(2, result[0]);
        Assert.Equal(1, result[1]);
        Assert.Equal(0, result[2]);
    }

    [Fact]
    public void FindTopContrastBands_RespectsTopN()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 4]
        };

        for (int b = 0; b < cube.Bands; b++)
        {
            cube[0, 0, b] = 0.0f;
            cube[0, 1, b] = 0.0f;
            cube[1, 0, b] = 1.0f;
            cube[1, 1, b] = 1.0f;
        }

        var result =
            TopContrastMapper.FindTopContrastBands(cube, 2);

        Assert.Equal(2, result.Length);
    }

    [Fact]
    public void FindTopContrastBands_RejectsZeroTopN()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => TopContrastMapper.FindTopContrastBands(cube, 0));
    }

    [Fact]
    public void FindTopContrastBands_RejectsTopNGreaterThanBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => TopContrastMapper.FindTopContrastBands(cube, 4));
    }

    [Fact]
    public void FindTopContrastBands_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(
            () => TopContrastMapper.FindTopContrastBands(null!, 2));
    }

    [Fact]
    public void CalculateContrast_SingleColumnBand_ReturnsExpectedValue()
    {
        var band = new float[,]
        {
        { 1.0f },
        { 2.0f },
        { 3.0f }
        };

        var result = TopContrastMapper.CalculateContrast(band);

        Assert.Equal(0.40824828f, result, 5);
    }

    [Fact]
    public void NormalizeBand_MinimumBecomesZero()
    {
        var band = new float[,]
        {
        { 2.0f, 4.0f },
        { 6.0f, 8.0f }
        };

        var result = TopContrastMapper.NormalizeBand(band);

        Assert.Equal(0.0f, result[0, 0]);
    }


    [Fact]
    public void NormalizeBand_MaximumBecomesOne()
    {
        var band = new float[,]
        {
        { 2.0f, 4.0f },
        { 6.0f, 8.0f }
        };

        var result = TopContrastMapper.NormalizeBand(band);

        Assert.Equal(1.0f, result[1, 1]);
    }


    [Fact]
    public void NormalizeBand_MapsValuesToZeroOneRange()
    {
        var band = new float[,]
        {
        { 2.0f, 4.0f },
        { 6.0f, 8.0f }
        };

        var result = TopContrastMapper.NormalizeBand(band);

        Assert.Equal(0.0f, result[0, 0]);
        Assert.Equal(0.33333334f, result[0, 1], 5);
        Assert.Equal(0.6666667f, result[1, 0], 5);
        Assert.Equal(1.0f, result[1, 1]);
    }


    [Fact]
    public void NormalizeBand_ConstantBandReturnsZeros()
    {
        var band = new float[,]
        {
        { 5.0f, 5.0f },
        { 5.0f, 5.0f }
        };

        var result = TopContrastMapper.NormalizeBand(band);

        Assert.Equal(0.0f, result[0, 0]);
        Assert.Equal(0.0f, result[0, 1]);
        Assert.Equal(0.0f, result[1, 0]);
        Assert.Equal(0.0f, result[1, 1]);
    }


    [Fact]
    public void NormalizeBand_RejectsNullBand()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TopContrastMapper.NormalizeBand(null!));
    }

    [Fact]
    public void BoostContrast_MapsValuesBetweenLimits()
    {
        var band = new float[,]
        {
        { 0.3f, 0.5f },
        { 0.7f, 0.9f }
        };

        var result =
            TopContrastMapper.BoostContrast(
                band,
                0.3f,
                0.7f);

        Assert.Equal(0.0f, result[0, 0]);
        Assert.Equal(0.5f, result[0, 1]);
        Assert.Equal(1.0f, result[1, 0]);
        Assert.Equal(1.0f, result[1, 1]);
    }

    [Fact]
    public void BoostContrast_ClipsValuesOutsideLimits()
    {
        var band = new float[,]
        {
        { 0.0f, 0.2f },
        { 0.8f, 1.0f }
        };

        var result =
            TopContrastMapper.BoostContrast(
                band,
                0.3f,
                0.7f);

        Assert.Equal(0.0f, result[0, 0]);
        Assert.Equal(0.0f, result[0, 1]);
        Assert.Equal(1.0f, result[1, 0]);
        Assert.Equal(1.0f, result[1, 1]);
    }

    [Fact]
    public void BoostContrast_RejectsNullBand()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TopContrastMapper.BoostContrast(
                null!,
                0.3f,
                0.7f));
    }

    [Fact]
    public void BoostContrast_RejectsInvalidLimits()
    {
        var band = new float[,]
        {
        { 0.0f, 1.0f }
        };

        Assert.Throws<ArgumentException>(() =>
            TopContrastMapper.BoostContrast(
                band,
                0.7f,
                0.3f));
    }

    [Fact]
    public void CalculateQuantile_ReturnsExpectedValue()
    {
        var values = new float[]
        {
        0.0f,
        0.25f,
        0.5f,
        0.75f,
        1.0f
        };

        var result =
            TopContrastMapper.CalculateQuantile(values, 0.5f);

        Assert.Equal(0.5f, result);
    }

    [Fact]
    public void CalculateQuantile_ReturnsLowerQuantile()
    {
        var values = new float[]
        {
        0.0f,
        0.25f,
        0.5f,
        0.75f,
        1.0f
        };

        var result =
            TopContrastMapper.CalculateQuantile(values, 0.1f);

        Assert.Equal(0.1f, result, 5);
    }

    [Fact]
    public void CalculateQuantile_RejectsEmptyValues()
    {
        Assert.Throws<ArgumentException>(() =>
            TopContrastMapper.CalculateQuantile(
                Array.Empty<float>(),
                0.5f));
    }

    [Fact]
    public void CalculateQuantile_RejectsInvalidQuantile()
    {
        var values = new float[]
        {
        0.0f,
        1.0f
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.CalculateQuantile(values, -0.1f));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.CalculateQuantile(values, 1.1f));
    }

    [Fact]
    public void ApplyShadowThreshold_SetsValuesBelowQuantileToZero()
    {
        var band = new float[,]
        {
        { 0.0f, 0.2f },
        { 0.5f, 1.0f }
        };

        var result =
            TopContrastMapper.ApplyShadowThreshold(
                band,
                0.5f);

        Assert.Equal(0.0f, result[0, 0]);
        Assert.Equal(0.0f, result[0, 1]);
        Assert.Equal(0.5f, result[1, 0]);
        Assert.Equal(1.0f, result[1, 1]);
    }

    [Fact]
    public void ApplyShadowThreshold_DoesNotModifyInput()
    {
        var band = new float[,]
        {
        { 0.0f, 0.2f },
        { 0.5f, 1.0f }
        };

        TopContrastMapper.ApplyShadowThreshold(band, 0.5f);

        Assert.Equal(0.0f, band[0, 0]);
        Assert.Equal(0.2f, band[0, 1]);
        Assert.Equal(0.5f, band[1, 0]);
        Assert.Equal(1.0f, band[1, 1]);
    }

    [Fact]
    public void CreateBandMask_SeparatesBrightAndDarkRegions()
    {
        var band = new float[,]
        {
        { 0.1f, 0.1f, 0.9f, 0.9f },
        { 0.1f, 0.1f, 0.9f, 0.9f }
        };

        var result = TopContrastMapper.CreateBandMask(
            band,
            low: 0.3f,
            high: 0.7f,
            shadowQuantile: 0.1f);

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
    public void CombineMasks_MajorityVote_ReturnsExpectedMask()
    {
        var masks = new[]
        {
        new Mask(new bool[,]
        {
            { true,  true,  false },
            { false, true,  false }
        }),
        new Mask(new bool[,]
        {
            { true,  false, false },
            { true,  true,  false }
        }),
        new Mask(new bool[,]
        {
            { false, true,  false },
            { true, false, true }
        })
    };

        var result = TopContrastMapper.CombineMasks(
            masks,
            requiredVotes: 2);

        Assert.True(result[0, 0]);
        Assert.True(result[0, 1]);
        Assert.False(result[0, 2]);

        Assert.True(result[1, 0]);
        Assert.True(result[1, 1]);
        Assert.False(result[1, 2]);
    }

    [Fact]
    public void Apply_CreatesMaskFromTopContrastBands()
    {
        var cube = new HsiCube
        {
            Data = new float[,,]
    {
        {
            { 0.1f, 0.1f, 0.1f },
            { 0.1f, 0.1f, 0.1f },
            { 0.9f, 0.9f, 0.9f },
            { 0.9f, 0.9f, 0.9f }
        }
    }
        };

        var result = TopContrastMapper.Apply(
            cube,
            topN: 2,
            low: 0.3f,
            high: 0.7f,
            shadowQuantile: 0.1f);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.True(result[0, 2]);
        Assert.True(result[0, 3]);
    }

    [Fact]
    public void Apply_RejectsNullCube()
    {
        Assert.Throws<ArgumentNullException>(() =>
            TopContrastMapper.Apply(null!));
    }

    [Fact]
    public void Apply_RejectsZeroTopN()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.Apply(cube, topN: 0));
    }

    [Fact]
    public void Apply_RejectsTopNGreaterThanBandCount()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.Apply(cube, topN: 4));
    }

    [Fact]
    public void Apply_RejectsInvalidContrastRange()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentException>(() =>
            TopContrastMapper.Apply(
                cube,
                topN: 2,
                low: 0.7f,
                high: 0.3f));
    }

    [Fact]
    public void Apply_RejectsEqualContrastLimits()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentException>(() =>
            TopContrastMapper.Apply(
                cube,
                topN: 2,
                low: 0.5f,
                high: 0.5f));
    }

    [Fact]
    public void Apply_RejectsShadowQuantileBelowZero()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.Apply(
                cube,
                shadowQuantile: -0.1f));
    }

    [Fact]
    public void Apply_RejectsShadowQuantileAboveOne()
    {
        var cube = new HsiCube
        {
            Data = new float[2, 2, 3]
        };

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TopContrastMapper.Apply(
                cube,
                shadowQuantile: 1.1f));
    }

    [Fact]
    public void Apply_WithOneBand_ReturnsThatBandMask()
    {
        var cube = new HsiCube
        {
            Data = new float[,,]
            {
            {
                { 0.1f },
                { 0.1f },
                { 0.9f },
                { 0.9f }
            }
            }
        };

        var result = TopContrastMapper.Apply(
            cube,
            topN: 1);

        Assert.False(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.True(result[0, 2]);
        Assert.True(result[0, 3]);
    }
}
