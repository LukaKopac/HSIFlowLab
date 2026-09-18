using HSIDataKit.Models;

namespace SpectrumKit.Processing;

public static class Normalization
{
    public static Spectrum MinMax(Spectrum spectrum)
    {
        ArgumentNullException.ThrowIfNull(spectrum);

        double min = spectrum.Min();
        double max = spectrum.Max();

        if (min == max)
        {
            return new Spectrum(
                spectrum.Wavelengths,
                spectrum.Values,
                spectrum.Name);
        }

        double[] normalizedValues = new double[spectrum.Values.Length];

        for (int i = 0; i < spectrum.Values.Length; i++)
        {
            normalizedValues[i] =
                (spectrum.Values[i] - min) /
                (max - min);
        }

        return new Spectrum(
            spectrum.Wavelengths,
            normalizedValues,
            spectrum.Name);
    }
}
