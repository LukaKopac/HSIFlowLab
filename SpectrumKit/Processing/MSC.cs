using HSIDataKit.Models;

namespace SpectrumKit.Processing;

public static class MSC
{
    public static Spectrum Apply(
        Spectrum spectrum,
        Spectrum reference)
    {
        ArgumentNullException.ThrowIfNull(spectrum);
        ArgumentNullException.ThrowIfNull(reference);

        if (spectrum.Wavelengths.Length != reference.Wavelengths.Length)
        {
            throw new ArgumentException(
                "Spectrum and reference must have the same number of wavelengths.",
                nameof(reference));
        }

        for (int i = 0; i < spectrum.Wavelengths.Length; i++)
        {
            if (spectrum.Wavelengths[i] != reference.Wavelengths[i])
            {
                throw new ArgumentException(
                    "Spectrum and reference must have matching wavelengths.",
                    nameof(reference));
            }
        }

        double referenceMean = reference.Values.Average();

        double spectrumMean = spectrum.Values.Average();

        double covariance = 0;
        double referenceVariance = 0;

        for (int i = 0; i < spectrum.Values.Length; i++)
        {
            double referenceDifference =
                reference.Values[i] - referenceMean;

            double spectrumDifference = 
                spectrum.Values[i] - spectrumMean;

            covariance += referenceDifference * spectrumDifference;

            referenceVariance += referenceDifference * referenceDifference;
        }

        if (referenceVariance == 0)
        {
            throw new ArgumentException(
                "Reference spectrum must not be constant.",
                nameof(reference));
        }

        double slope = covariance / referenceVariance;
        double intercept = spectrumMean - slope * referenceMean;

        double[] correctedValues = new double[spectrum.Values.Length];

        for (int i = 0; i < spectrum.Values.Length; i++)
        {
            correctedValues[i] =
                (spectrum.Values[i] - intercept) / slope;
        }

        return new Spectrum(
            spectrum.Wavelengths,
            correctedValues,
            spectrum.Name);
    }
}
