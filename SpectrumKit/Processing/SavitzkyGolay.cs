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
            smoothedValues[i] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    i - halfWindow);
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

    public static Spectrum Derivative(
        Spectrum spectrum,
        int windowSize,
        int polynomialOrder,
        int derivativeOrder)
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

        if (derivativeOrder < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(derivativeOrder),
                "derivativeOrder must be greater than zero.");
        }

        if (derivativeOrder > polynomialOrder)
        {
            throw new ArgumentException(
                "derivativeOrder must not be greater than polynomialOrder.",
                nameof(derivativeOrder));
        }

        double delta =
            spectrum.Wavelengths[1] -
            spectrum.Wavelengths[0];

        if (delta == 0)
        {
            throw new ArgumentException(
                "Wavelengths must have non-zero spacing.",
                nameof(delta));
        }

        for (int i = 2; i < spectrum.Wavelengths.Length; i++)
        {
            double currentDelta =
                spectrum.Wavelengths[i] -
                spectrum.Wavelengths[i - 1];

            if (!double.Equals(currentDelta, delta))
            {
                throw new ArgumentException(
                    "Wavelengths must be evenly spaced.",
                    nameof(spectrum));
            }
        }

        int halfWindow = windowSize / 2;

        double[] coefficients =
            CalculateCoefficients(
                windowSize,
                polynomialOrder,
                derivativeOrder);

        double wavelengthScale =
            Math.Pow(delta, derivativeOrder);

        for (int i = 0; i < coefficients.Length; i++)
        {
            coefficients[i] /= wavelengthScale;
        }

        double[] derivativeValues =
            new double[spectrum.Values.Length];

        for (int i = halfWindow; i < spectrum.Values.Length - halfWindow; i++)
        {
            derivativeValues[i] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    i - halfWindow);
        }

        DerivativeEdges(
            spectrum,
            derivativeValues,
            windowSize,
            polynomialOrder,
            derivativeOrder);

        return new Spectrum(
            spectrum.Wavelengths,
            derivativeValues,
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
        int derivativeOrder,
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

            for (int j = derivativeOrder; j <= polynomialOrder; j++)
            {
                double derivativeFactor = 
                    Factorial(j) /
                    Factorial(j - derivativeOrder);

                coefficient +=
                    derivativeFactor *
                    Math.Pow(x, j - derivativeOrder) *
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
                    derivativeOrder: 0,
                    i);

            smoothedValues[i] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    0);
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
                    derivativeOrder: 0,
                    x);

            int startIndex = spectrum.Values.Length - windowSize;

            smoothedValues[outputIndex] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    startIndex);
        }
    }

    private static void DerivativeEdges(
        Spectrum spectrum,
        double[] derivativeValues,
        int windowSize,
        int polynomialOrder,
        int derivativeOrder)
    {
        int halfWindow = windowSize / 2;

        double delta =
            spectrum.Wavelengths[1] -
            spectrum.Wavelengths[0];

        double wavelengthScale =
            Math.Pow(delta, derivativeOrder);

        // Left edge
        for (int i = 0; i < halfWindow; i++)
        {
            double[] coefficients =
                CalculateEdgeCoefficients(
                    windowSize,
                    polynomialOrder,
                    derivativeOrder,
                    i);

            for (int j = 0; j < coefficients.Length; j++)
            {
                coefficients[j] /= wavelengthScale;
            }

            derivativeValues[i] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    0);
        }

        // Right edge
        for (int i = 0; i < halfWindow; i++)
        {
            int outputIndex =
                spectrum.Values.Length - halfWindow + i;

            double x = windowSize - halfWindow + i;

            double[] coefficients =
                CalculateEdgeCoefficients(
                    windowSize,
                    polynomialOrder,
                    derivativeOrder,
                    x);

            for (int j = 0; j < coefficients.Length; j++)
            {
                coefficients[j] /= wavelengthScale;
            }

            int startIndex = spectrum.Values.Length - windowSize;

            derivativeValues[outputIndex] =
                ApplyCoefficients(
                    spectrum.Values,
                    coefficients,
                    startIndex);
        }
    }

    private static double ApplyCoefficients(
        double[] values,
        double[] coefficients,
        int startIndex)
    {
        double result = 0;

        for (int j = 0; j < coefficients.Length; j++)
        {
            result +=
                coefficients[j] *
                values[startIndex + j];
        }

        return result;
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
