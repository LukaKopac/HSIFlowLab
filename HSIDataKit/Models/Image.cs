namespace HSIDataKit.Models;

public class Image
{
    public float[,] Data { get; }

    public int Height => Data.GetLength(0);
    public int Width => Data.GetLength(1);

    public Image(float[,] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.GetLength(0) == 0 || data.GetLength(1) == 0)
            throw new ArgumentException(
                "Image dimensions must be greater than zero.",
                nameof(data));

        Data = data;
    }

    public float this[int y, int x]
    {
        get => Data[y, x];
        set => Data[y, x] = value;
    }
}