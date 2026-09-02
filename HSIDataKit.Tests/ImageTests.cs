using HSIDataKit.Models;

namespace HSIDataKit.Tests;

public class ImageTests
{
    [Fact]
    public void Constructor_StoresData()
    {
        var data = new float[,]
        {
            { 1.0f, 2.0f },
            { 3.0f, 4.0f }
        };

        var image = new Image(data);

        Assert.Same(data, image.Data);
    }

    [Fact]
    public void Constructor_SetsCorrectDimensions()
    {
        var data = new float[3, 5];

        var image = new Image(data);

        Assert.Equal(3, image.Height);
        Assert.Equal(5, image.Width);
    }

    [Fact]
    public void Constructor_RejectsNullData()
    {
        Assert.Throws<ArgumentNullException>(
            () => new Image(null!));
    }

    [Fact]
    public void Constructor_RejectsZeroHeight()
    {
        var data = new float[0, 5];

        Assert.Throws<ArgumentException>(
            () => new Image(data));
    }

    [Fact]
    public void Constructor_RejectsZeroWidth()
    {
        var data = new float[5, 0];

        Assert.Throws<ArgumentException>(
            () => new Image(data));
    }

    [Fact]
    public void Indexer_ReturnsCorrectValue()
    {
        var image = new Image(new float[,]
        {
        { 1.0f, 2.0f },
        { 3.0f, 4.0f }
        });

        Assert.Equal(1.0f, image[0, 0]);
        Assert.Equal(2.0f, image[0, 1]);
        Assert.Equal(3.0f, image[1, 0]);
        Assert.Equal(4.0f, image[1, 1]);
    }

    [Fact]
    public void Indexer_CanSetValue()
    {
        var image = new Image(new float[2, 2]);

        image[1, 0] = 42.5f;

        Assert.Equal(42.5f, image[1, 0]);
    }
}