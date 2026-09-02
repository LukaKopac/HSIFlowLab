namespace HSIDataKit.Models;

public class Mask
{
    public bool[,] Data { get; }

    public int Height => Data.GetLength(0);
    public int Width => Data.GetLength(1);

    public int Count
    {
        get
        {
            int count = 0;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Data[y, x])
                        count++;
                }
            }

            return count;
        }
    }

    public Mask(bool[,] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.GetLength(0) == 0 || data.GetLength(1) == 0)
            throw new ArgumentException(
                "Mask dimensions must be greater than zero.",
                nameof(data));

        Data = data;
    }

    public bool this[int y, int x]
    {
        get => Data[y, x];
        set => Data[y, x] = value;
    }
    
    public static Mask Empty(int height, int width)
    {
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        return new Mask(new bool[height, width]);
    }

    public static Mask Full(int height, int width)
    {
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height));

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width));

        var data = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                data[y, x] = true;
            }
        }

        return new Mask(data);
    }

    public Mask Invert()
    {
        var result = new bool[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result[y, x] = !Data[y, x];
            }
        }

        return new Mask(result);
    }

    public Mask And(Mask other)
    {
        ArgumentNullException.ThrowIfNull(other);

        EnsureSameDimensions(other);

        var result = new bool[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result[y, x] = Data[y, x] && other[y, x];
            }
        }

        return new Mask(result);
    }

    public Mask Or(Mask other)
    {
        ArgumentNullException.ThrowIfNull(other);

        EnsureSameDimensions(other);

        var result = new bool[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result[y, x] = Data[y, x] || other[y, x];
            }
        }

        return new Mask(result);
    }

    public Mask Xor(Mask other)
    {
        ArgumentNullException.ThrowIfNull(other);

        EnsureSameDimensions(other);

        var result = new bool[Height, Width];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                result[y, x] = Data[y, x] ^ other[y, x];
            }
        }

        return new Mask(result);
    }

    private void EnsureSameDimensions(Mask other)
    {
        if (Height != other.Height || Width != other.Width)
        {
            throw new ArgumentException(
                "Masks must have the same dimensions.",
                nameof(other));
        }
    }
}