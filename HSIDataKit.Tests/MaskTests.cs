using HSIDataKit.Models;

namespace HSIDataKit.Tests;

public class MaskTests
{
    [Fact]
    public void Constructor_StoresData()
    {
        var data = new bool[,]
        {
            { true, false },
            { false, true }
        };

        var mask = new Mask(data);

        Assert.Same(data, mask.Data);
    }

    [Fact]
    public void Constructor_SetsCorrectDimensions()
    {
        var data = new bool[3, 5];

        var mask = new Mask(data);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void Constructor_RejectsNullData()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Mask(null!));
    }

    [Fact]
    public void Constructor_RejectsZeroHeight()
    {
        var data = new bool[0, 5];

        Assert.Throws<ArgumentException>(
            () => new Mask(data));
    }

    [Fact]
    public void Constructor_RejectsZeroWidth()
    {
        var data = new bool[5, 0];

        Assert.Throws<ArgumentException>(
            () => new Mask(data));
    }

    [Fact]
    public void Indexer_ReturnsCorrectValue()
    {
        var mask = new Mask(new bool[,]
        {
        { true, false },
        { false, true }
        });

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.False(mask[1, 0]);
        Assert.True(mask[1, 1]);
    }

    [Fact]
    public void Indexer_CanSetValue()
    {
        var mask = Mask.Empty(2, 2);

        mask[1, 0] = true;

        Assert.True(mask[1, 0]);
    }

    [Fact]
    public void Count_ReturnsZeroForEmptyMask()
    {
        var mask = Mask.Empty(3, 4);

        Assert.Equal(0, mask.Count);
    }

    [Fact]
    public void Count_ReturnsTotalNumberOfSelectedPixels()
    {
        var mask = new Mask(new bool[,]
        {
        { true,  false, true },
        { false, true,  false }
        });

        Assert.Equal(3, mask.Count);
    }

    [Fact]
    public void Count_UpdatesWhenMaskChanges()
    {
        var mask = Mask.Empty(2, 2);

        Assert.Equal(0, mask.Count);

        mask[0, 0] = true;
        mask[1, 1] = true;

        Assert.Equal(2, mask.Count);

        mask[0, 0] = false;

        Assert.Equal(1, mask.Count);
    }

    [Fact]
    public void Empty_CreatesMaskWithCorrectDimensions()
    {
        var mask = Mask.Empty(3, 5);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void Empty_CreatesMaskWithAllPixelsFalse()
    {
        var mask = Mask.Empty(3, 5);

        for (int y = 0; y < mask.Height; y++)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                Assert.False(mask[y, x]);
            }
        }
    }

    [Fact]
    public void Full_CreatesMaskWithCorrectDimensions()
    {
        var mask = Mask.Full(3, 5);

        Assert.Equal(3, mask.Height);
        Assert.Equal(5, mask.Width);
    }

    [Fact]
    public void Full_CreatesMaskWithAllPixelsTrue()
    {
        var mask = Mask.Full(3, 5);

        for (int y = 0; y < mask.Height; y++)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                Assert.True(mask[y, x]);
            }
        }
    }

    [Fact]
    public void Empty_RejectsInvalidDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Mask.Empty(0, 5));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Mask.Empty(5, 0));
    }

    [Fact]
    public void Full_RejectsInvalidDimensions()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Mask.Full(0, 5));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Mask.Full(5, 0));
    }

    [Fact]
    public void Invert_FlipsEveryPixel()
    {
        var mask = new Mask(new bool[,]
        {
        { true,  false },
        { false, true }
        });

        var inverted = mask.Invert();

        Assert.False(inverted[0, 0]);
        Assert.True(inverted[0, 1]);
        Assert.True(inverted[1, 0]);
        Assert.False(inverted[1, 1]);
    }

    [Fact]
    public void Invert_PreservesDimensions()
    {
        var mask = Mask.Empty(3, 5);

        var inverted = mask.Invert();

        Assert.Equal(3, inverted.Height);
        Assert.Equal(5, inverted.Width);
    }

    [Fact]
    public void Invert_DoesNotModifyOriginal()
    {
        var mask = new Mask(new bool[,]
        {
        { true,  false },
        { false, true }
        });

        var inverted = mask.Invert();

        Assert.True(mask[0, 0]);
        Assert.False(mask[0, 1]);
        Assert.False(mask[1, 0]);
        Assert.True(mask[1, 1]);
    }

    [Fact]
    public void Invert_TwiceReturnsOriginalValues()
    {
        var mask = new Mask(new bool[,]
        {
        { true,  false, true },
        { false, false, true }
        });

        var result = mask.Invert().Invert();

        for (int y = 0; y < mask.Height; y++)
        {
            for (int x = 0; x < mask.Width; x++)
            {
                Assert.Equal(mask[y, x], result[y, x]);
            }
        }
    }

    [Fact]
    public void And_ReturnsCorrectResult()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, true }
        });

        var result = first.And(second);

        Assert.True(result[0, 0]);
        Assert.False(result[0, 1]);
        Assert.False(result[1, 0]);
        Assert.True(result[1, 1]);
    }

    [Fact]
    public void And_PreservesDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Full(3, 5);

        var result = first.And(second);

        Assert.Equal(3, result.Height);
        Assert.Equal(5, result.Width);
    }

    [Fact]
    public void And_DoesNotModifyInputs()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, true }
        });

        first.And(second);

        Assert.True(first[0, 0]);
        Assert.False(first[0, 1]);
        Assert.True(first[1, 0]);
        Assert.True(first[1, 1]);

        Assert.True(second[0, 0]);
        Assert.True(second[0, 1]);
        Assert.False(second[1, 0]);
        Assert.True(second[1, 1]);
    }

    [Fact]
    public void And_RejectsDifferentDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Empty(4, 5);

        Assert.Throws<ArgumentException>(
            () => first.And(second));
    }

    [Fact]
    public void Or_ReturnsCorrectResult()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, false }
        });

        var result = first.Or(second);

        Assert.True(result[0, 0]);
        Assert.True(result[0, 1]);
        Assert.True(result[1, 0]);
        Assert.True(result[1, 1]);
    }

    [Fact]
    public void Or_PreservesDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Full(3, 5);

        var result = first.Or(second);

        Assert.Equal(3, result.Height);
        Assert.Equal(5, result.Width);
    }

    [Fact]
    public void Or_DoesNotModifyInputs()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, false }
        });

        first.Or(second);

        Assert.True(first[0, 0]);
        Assert.False(first[0, 1]);
        Assert.True(first[1, 0]);
        Assert.True(first[1, 1]);

        Assert.True(second[0, 0]);
        Assert.True(second[0, 1]);
        Assert.False(second[1, 0]);
        Assert.False(second[1, 1]);
    }

    [Fact]
    public void Or_RejectsDifferentDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Empty(4, 5);

        Assert.Throws<ArgumentException>(
            () => first.Or(second));
    }

    [Fact]
    public void Xor_ReturnsCorrectResult()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, false }
        });

        var result = first.Xor(second);

        Assert.False(result[0, 0]);
        Assert.True(result[0, 1]);
        Assert.True(result[1, 0]);
        Assert.True(result[1, 1]);
    }

    [Fact]
    public void Xor_PreservesDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Full(3, 5);

        var result = first.Xor(second);

        Assert.Equal(3, result.Height);
        Assert.Equal(5, result.Width);
    }

    [Fact]
    public void Xor_DoesNotModifyInputs()
    {
        var first = new Mask(new bool[,]
        {
        { true,  false },
        { true,  true }
        });

        var second = new Mask(new bool[,]
        {
        { true,  true },
        { false, false }
        });

        first.Xor(second);

        Assert.True(first[0, 0]);
        Assert.False(first[0, 1]);
        Assert.True(first[1, 0]);
        Assert.True(first[1, 1]);

        Assert.True(second[0, 0]);
        Assert.True(second[0, 1]);
        Assert.False(second[1, 0]);
        Assert.False(second[1, 1]);
    }

    [Fact]
    public void Xor_RejectsDifferentDimensions()
    {
        var first = Mask.Empty(3, 5);
        var second = Mask.Empty(4, 5);

        Assert.Throws<ArgumentException>(
            () => first.Xor(second));
    }
}
