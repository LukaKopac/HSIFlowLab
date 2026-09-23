using HSIDataKit.Models;

namespace SpectrumKit.Processing;

public static class SavitzkyGolay
{
    public static Spectrum Smooth(
        Spectrum spectrum,
        int windowSize,
        int polynomialOrder)
    {
        ArgumentNullException.ThrowIfNull(spectrum);

        if (windowSize < 3 || windowSize % 2 == 0)
        {
            throw new ArgumentException(
                "windowSize must be an odd number greater than or equal to 3.",
                nameof(windowSize));
        }

        if (polynomialOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(polynomialOrder),
                "polynomialOrder must be non-negative.");
        }

        if (polynomialOrder >= windowSize)
        {
            throw new ArgumentException(
                "polynomialOrder must be smaller than windowSize.",
                nameof(polynomialOrder));
        }


    }

    private static double[] CalculateCoefficients(
    int windowSize,
    int polynomialOrder)
    {
        int halfWindow = windowSize / 2;

        var matrix = new double[windowSize, polynomialOrder + 1];

        for (int row = 0; row < windowSize; row++)
        {
            double x = row - halfWindow;

            for (int column = 0; column <= polynomialOrder; column++)
            {
                matrix[row, column] = Math.Pow(x, column);
            }
        }

        var ata = new double[polynomialOrder + 1, polynomialOrder + 1];

        for (int row = 0; row <= polynomialOrder; row++)
        {
            for (int column = 0; column <= polynomialOrder; column++)
            {
                for (int i = 0; i < windowSize; i++)
                {
                    ata[row, column] +=
                        matrix[i, row] * matrix[i, column];
                }
            }
        }

        var ataInverse = InvertMatrix(ata);

        var coefficients = new double[windowSize];

        for (int i = 0; i < windowSize; i++)
        {
            for (int j = 0; j <= polynomialOrder; j++)
            {
                coefficients[i] +=
                    ataInverse[0, j] * matrix[i, j];
            }
        }

        return coefficients;
    }
}
