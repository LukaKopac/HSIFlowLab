using HSIDataKit.Models;

namespace SpectrumKit.Processing;

public static class SNV
{
    public static Spectrum Apply(Spectrum spectrum)
    {
        ArgumentNullException.ThrowIfNull(spectrum);

        double mean = spectrum.Mean();

        double sumSquaredDifferences = 0;

        for (int i = 0; i < spectrum.Values.Length; i++)
        {
            double difference = spectrum.Values[i] - mean;
            sumSquaredDifferences += difference * difference;
        }

        double standardDeviation =
            Math.Sqrt(
                sumSquaredDifferences /
                spectrum.Values.Length);

        double[] normalizedValues = new double[spectrum.Values.Length];

        if (standardDeviation == 0)
        {
            return new Spectrum(
                spectrum.Wavelengths,
                normalizedValues,
                spectrum.Name);
        }

        for (int i = 0; i < spectrum.Values.Length; i++)
        {
            normalizedValues[i] =
                (spectrum.Values[i] - mean) /
                standardDeviation;
        }

        return new Spectrum(
            spectrum.Wavelengths,
            normalizedValues,
            spectrum.Name);
    }
}
