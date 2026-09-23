namespace HSIDataKit.Models;

public class Matrix
{
    private readonly double[,] _data;

    public int Rows { get; }

    public int Columns { get; }

    public double this[int row, int column]
    {
        get => _data[row, column];
        set => _data[row, column] = value;
    }

    public Matrix(int rows, int columns)
    {
        if (rows <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rows),
                "Rows must be greater than zero.");
        }

        if (columns <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(columns),
                "Columns must be greater than zero.");
        }

        Rows = rows;
        Columns = columns;

        _data = new double[rows, columns];
    }

    public Matrix(double[,] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Rows = data.GetLength(0);
        Columns = data.GetLength(1);

        _data = new double[Rows, Columns];

        Array.Copy(data, _data, data.Length);
    }

    public Matrix Transpose()
    {
        var result = new Matrix(Columns, Rows);

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                result[column, row] = _data[row, column];
            }
        }

        return result;
    }

    public Matrix Multiply(Matrix other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Columns != other.Rows)
        {
            throw new ArgumentException(
                "The number of columns in the first matrix must equal the number of rows in the second matrix.",
                nameof(other));
        }

        var result = new Matrix(Rows, other.Columns);

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < other.Columns; column++)
            {
                double sum = 0;

                for (int k = 0; k < Columns; k++)
                {
                    sum += this[row, k] * other[k, column];
                }

                result[row, column] = sum;
            }
        }

        return result;
    }

    public Matrix Inverse()
    {
        if (Rows != Columns)
        {
            throw new InvalidOperationException(
                "Only square matrices can be inverted.");
        }

        int size = Rows;

        var augmented = new double[size, size * 2];

        // Create [A | I]
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                augmented[row, column] = this[row, column];
            }

            augmented[row, size + row] = 1;
        }

        const double tolerance = 1e-12;

        for (int column = 0; column < size; column++)
        {
            // Find the row with the largest pivot.
            int pivotRow = column;
            double largestValue = Math.Abs(
                augmented[pivotRow, column]);

            for (int row = column + 1; row < size; row++)
            {
                double value = Math.Abs(
                    augmented[row, column]);

                if (value > largestValue)
                {
                    largestValue = value;
                    pivotRow = row;
                }
            }

            if (largestValue < tolerance)
            {
                throw new InvalidOperationException(
                    "Matrix is singular and cannot be inverted.");
            }

            // Swap the pivot row into position
            if (pivotRow != column)
            {
                for (int j = 0; j < size * 2; j++)
                {
                    (augmented[column, j], augmented[pivotRow, j]) =
                        (augmented[pivotRow, j], augmented[column, j]);
                }
            }

            // Normalize the pivot row
            double pivot = augmented[column, column];

            for (int j = 0; j < size * 2; j++)
            {
                augmented[column, j] /= pivot;
            }

            // Eliminate this column from every other row
            for (int row = 0; row < size; row++)
            {
                if (row == column)
                {
                    continue;
                }

                double factor = augmented[row, column];

                for (int j = 0; j < size * 2; j++)
                {
                    augmented[row, j] -=
                        factor * augmented[column, j];
                }
            }
        }

        // Extract the inverse from the right half
        var result = new Matrix(size, size);

        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                result[row, column] =
                    augmented[row, size + column];
            }
        }

        return result;
    }

    public double[,] ToArray()
    {
        var result = new double[Rows, Columns];

        Array.Copy(_data, result, _data.Length);

        return result;
    }

    public static Matrix Identity(int size)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(size),
                "Size must be greater than zero.");
        }

        var matrix = new Matrix(size, size);

        for (int i = 0; i < size; i++)
        {
            matrix[i, i] = 1;
        }

        return matrix;
    }
}