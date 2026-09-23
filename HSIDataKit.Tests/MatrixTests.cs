using HSIDataKit.Models;

namespace HSIDataKit.Tests;

public class MatrixTests
{
    [Fact]
    public void Constructor_CreatesMatrixWithCorrectDimensions()
    {
        var matrix = new Matrix(2, 3);

        Assert.Equal(2, matrix.Rows);
        Assert.Equal(3, matrix.Columns);
    }

    [Fact]
    public void Constructor_InitializesValuesToZero()
    {
        var matrix = new Matrix(2, 2);

        Assert.Equal(0, matrix[0, 0]);
        Assert.Equal(0, matrix[0, 1]);
        Assert.Equal(0, matrix[1, 0]);
        Assert.Equal(0, matrix[1, 1]);
    }

    [Fact]
    public void ArrayConstructor_CopiesValues()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2 },
        { 3, 4 }
        });

        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(2, matrix[0, 1]);
        Assert.Equal(3, matrix[1, 0]);
        Assert.Equal(4, matrix[1, 1]);
    }

    [Fact]
    public void ArrayConstructor_CopiesInputArray()
    {
        var data = new double[,]
        {
        { 1, 2 },
        { 3, 4 }
        };

        var matrix = new Matrix(data);

        data[0, 0] = 999;

        Assert.Equal(1, matrix[0, 0]);
    }

    [Fact]
    public void Indexer_CanSetAndGetValues()
    {
        var matrix = new Matrix(2, 2);

        matrix[0, 1] = 42.5;

        Assert.Equal(42.5, matrix[0, 1]);
    }

    [Fact]
    public void Transpose_SwapsRowsAndColumns()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        var result = matrix.Transpose();

        Assert.Equal(3, result.Rows);
        Assert.Equal(2, result.Columns);
    }

    [Fact]
    public void Transpose_ReturnsCorrectValues()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        var result = matrix.Transpose();

        Assert.Equal(1, result[0, 0]);
        Assert.Equal(4, result[0, 1]);
        Assert.Equal(2, result[1, 0]);
        Assert.Equal(5, result[1, 1]);
        Assert.Equal(3, result[2, 0]);
        Assert.Equal(6, result[2, 1]);
    }

    [Fact]
    public void Transpose_DoesNotModifyOriginal()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        _ = matrix.Transpose();

        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(2, matrix[0, 1]);
        Assert.Equal(3, matrix[0, 2]);
        Assert.Equal(4, matrix[1, 0]);
        Assert.Equal(5, matrix[1, 1]);
        Assert.Equal(6, matrix[1, 2]);
    }

    [Fact]
    public void Multiply_ReturnsCorrectDimensionsAndValues()
    {
        var first = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        var second = new Matrix(new double[,]
        {
        { 7, 8 },
        { 9, 10 },
        { 11, 12 }
        });

        var result = first.Multiply(second);

        Assert.Equal(2, result.Rows);
        Assert.Equal(2, result.Columns);

        Assert.Equal(58, result[0, 0]);
        Assert.Equal(64, result[0, 1]);
        Assert.Equal(139, result[1, 0]);
        Assert.Equal(154, result[1, 1]);
    }

    [Fact]
    public void Multiply_RejectsIncompatibleDimensions()
    {
        var first = new Matrix(2, 3);
        var second = new Matrix(2, 2);

        Assert.Throws<ArgumentException>(
            () => first.Multiply(second));
    }

    [Fact]
    public void Identity_ReturnsIdentityMatrix()
    {
        var matrix = Matrix.Identity(3);

        Assert.Equal(3, matrix.Rows);
        Assert.Equal(3, matrix.Columns);

        Assert.Equal(1, matrix[0, 0]);
        Assert.Equal(0, matrix[0, 1]);
        Assert.Equal(0, matrix[0, 2]);

        Assert.Equal(0, matrix[1, 0]);
        Assert.Equal(1, matrix[1, 1]);
        Assert.Equal(0, matrix[1, 2]);

        Assert.Equal(0, matrix[2, 0]);
        Assert.Equal(0, matrix[2, 1]);
        Assert.Equal(1, matrix[2, 2]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Identity_RejectsInvalidSize(int size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Matrix.Identity(size));
    }

    [Fact]
    public void Multiply_ByIdentity_ReturnsOriginalMatrix()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        var identity = Matrix.Identity(3);

        var result = matrix.Multiply(identity);

        Assert.Equal(matrix.Rows, result.Rows);
        Assert.Equal(matrix.Columns, result.Columns);

        Assert.Equal(1, result[0, 0]);
        Assert.Equal(2, result[0, 1]);
        Assert.Equal(3, result[0, 2]);
        Assert.Equal(4, result[1, 0]);
        Assert.Equal(5, result[1, 1]);
        Assert.Equal(6, result[1, 2]);
    }

    [Fact]
    public void Inverse_ReturnsCorrectInverse()
    {
        var matrix = new Matrix(new double[,]
        {
        { 4, 7 },
        { 2, 6 }
        });

        var result = matrix.Inverse();

        Assert.Equal(0.6, result[0, 0], 10);
        Assert.Equal(-0.7, result[0, 1], 10);
        Assert.Equal(-0.2, result[1, 0], 10);
        Assert.Equal(0.4, result[1, 1], 10);
    }

    [Fact]
    public void Inverse_MultipliedByOriginal_ReturnsIdentity()
    {
        var matrix = new Matrix(new double[,]
        {
        { 4, 7 },
        { 2, 6 }
        });

        var inverse = matrix.Inverse();

        var result = matrix.Multiply(inverse);

        Assert.Equal(1, result[0, 0], 10);
        Assert.Equal(0, result[0, 1], 10);
        Assert.Equal(0, result[1, 0], 10);
        Assert.Equal(1, result[1, 1], 10);
    }

    [Fact]
    public void Inverse_RejectsNonSquareMatrix()
    {
        var matrix = new Matrix(2, 3);

        Assert.Throws<InvalidOperationException>(
            () => matrix.Inverse());
    }

    [Fact]
    public void Inverse_RejectsSingularMatrix()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2 },
        { 2, 4 }
        });

        Assert.Throws<InvalidOperationException>(
            () => matrix.Inverse());
    }

    [Fact]
    public void Inverse_UsesRowPivoting()
    {
        var matrix = new Matrix(new double[,]
        {
        { 0, 1 },
        { 2, 3 }
        });

        var inverse = matrix.Inverse();

        var result = matrix.Multiply(inverse);

        Assert.Equal(1, result[0, 0], 10);
        Assert.Equal(0, result[0, 1], 10);
        Assert.Equal(0, result[1, 0], 10);
        Assert.Equal(1, result[1, 1], 10);
    }

    [Fact]
    public void ToArray_ReturnsCorrectValues()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2, 3 },
        { 4, 5, 6 }
        });

        var result = matrix.ToArray();

        Assert.Equal(1, result[0, 0]);
        Assert.Equal(2, result[0, 1]);
        Assert.Equal(3, result[0, 2]);
        Assert.Equal(4, result[1, 0]);
        Assert.Equal(5, result[1, 1]);
        Assert.Equal(6, result[1, 2]);
    }

    [Fact]
    public void ToArray_ReturnsCopy()
    {
        var matrix = new Matrix(new double[,]
        {
        { 1, 2 },
        { 3, 4 }
        });

        var result = matrix.ToArray();

        result[0, 0] = 999;

        Assert.Equal(1, matrix[0, 0]);
    }
}
