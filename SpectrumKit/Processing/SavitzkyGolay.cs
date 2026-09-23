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

        int halfWindow = windowSize / 2;
        
        double[] coefficients =
            CalculateCoefficients(
                windowSize,
                polynomialOrder,
                derivativeOrder: 0);

        double[] smoothedValues =
            new double[spectrum.Values.Length];

        // Temporarily preserve the edge values
        Array.Copy(
            spectrum.Values,
            smoothedValues,
            spectrum.Values.Length);

        for (int i = halfWindow; i < spectrum.Values.Length - halfWindow; i++)
        {
            double value = 0;

            for (int j = 0; j < windowSize; j++)
            {
                int sourceIndex =
                    i - halfWindow + j;

                value +=
                    coefficients[j] *
                    spectrum.Values[sourceIndex];
            }

            smoothedValues[i] = value;
        }

        SmoothEdges(
            spectrum,
            smoothedValues,
            windowSize,
            polynomialOrder);

        return new Spectrum(
            spectrum.Wavelengths,
            smoothedValues,
            spectrum.Name);
    }

    private static double[] CalculateCoefficients(
        int windowSize,
        int polynomialOrder,
        int derivativeOrder)
    {
        if (derivativeOrder < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(derivativeOrder),
                "derivativeOrder must be non-negative.");
        }

        if (derivativeOrder > polynomialOrder)
        {
            throw new ArgumentException(
                "derivativeOrder must not be greater than polynomialOrder.",
                nameof(derivativeOrder));
        }
        
        int halfWindow = windowSize / 2;

        var designMatrix = new Matrix(
            windowSize,
            polynomialOrder + 1);

        for (int row = 0; row < windowSize; row++)
        {
            double x = row - halfWindow;

            for (int column = 0; column <= polynomialOrder; column++)
            {
                designMatrix[row, column] =
                    Math.Pow(x, column);
            }
        }

        Matrix transpose = designMatrix.Transpose();

        Matrix normalMatrix =
            transpose.Multiply(designMatrix);

        Matrix inverse =
            normalMatrix.Inverse();

        Matrix coefficientsMatrix =
            inverse.Multiply(transpose);

        var coefficients = new double[windowSize];

        double derivativeFactor = Factorial(derivativeOrder);

        for (int i = 0; i < windowSize; i++)
        {
            coefficients[i] =
                derivativeFactor *
                coefficientsMatrix[derivativeOrder, i];
        }

        return coefficients;
    }

    private static double[] CalculateEdgeCoefficients(
        int windowSize,
        int polynomialOrder,
        double x)
    {
        var designMatrix =
            new Matrix(windowSize, polynomialOrder + 1);

        for (int row = 0; row < windowSize; row++)
        {
            for (int column = 0; column <= polynomialOrder; column++)
            {
                designMatrix[row, column] =
                    Math.Pow(row, column);
            }
        }

        Matrix transpose =
            designMatrix.Transpose();

        Matrix normalMatrix = 
            transpose.Multiply(designMatrix);

        Matrix inverse = 
            normalMatrix.Inverse();

        Matrix mapping = 
            inverse.Multiply(transpose);

        var coefficients =
            new double[windowSize];

        for (int i = 0; i < windowSize; i++)
        {
            double coefficient = 0;

            for (int j = 0; j <= polynomialOrder; j++)
            {
                coefficient +=
                    Math.Pow(x, j) *
                    mapping[j, i];
            }

            coefficients[i] = coefficient;
        }

        return coefficients;
    }

    private static void SmoothEdges(
        Spectrum spectrum,
        double[] smoothedValues,
        int windowSize,
        int polynomialOrder)
    {
        int halfWindow = windowSize / 2;

        // Left edge
        for (int i = 0; i < halfWindow; i++)
        {
            double[] coefficients =
                CalculateEdgeCoefficients(
                    windowSize,
                    polynomialOrder,
                    i);

            double value = 0;

            for (int j = 0; j < windowSize; j++)
            {
                value +=
                    coefficients[j] *
                    spectrum.Values[j];
            }

            smoothedValues[i] = value;
        }

        // Right edge
        for (int i = 0; i < halfWindow; i++)
        {
            int outputIndex =
                spectrum.Values.Length - halfWindow + i;

            double x =
                windowSize - halfWindow + i;

            double[] coefficients =
                CalculateEdgeCoefficients(
                    windowSize,
                    polynomialOrder,
                    x);

            double value = 0;

            int startIndex =
                spectrum.Values.Length - windowSize;

            for (int j = 0; j < windowSize; j++)
            {
                value +=
                    coefficients[j] *
                    spectrum.Values[startIndex + j];
            }

            smoothedValues[outputIndex] = value;
        }
    }

    private static int Factorial(int value)
    {
        int result = 1;

        for (int i = 2; i <= value; i++)
        {
            result *= i;
        }

        return result;
    }
}
