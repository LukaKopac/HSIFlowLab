namespace HSIDataKit.Models;

public class Image
{
    public float[,] Data { get; }

    public int Height => Data.GetLength(0);
    public int Width => Data.GetLength(1);

    public Image(float[,] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Data = data;
    }

    public float this[int y, int x]
    {
        get => Data[y, x];
        set => Data[y, x] = value;
    }
}