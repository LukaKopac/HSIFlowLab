namespace HSIDataKit.Models
{
    public class SpectrumSet
    {
        public double[] Wavelengths { get; }
        public List<string> Names { get; }
        public List<double[]> Values { get; }

        public SpectrumSet(
            double[] wavelengths,
            List<string> names,
            List<double[]> values)
        {
            ArgumentNullException.ThrowIfNull(wavelengths);
            ArgumentNullException.ThrowIfNull(names);
            ArgumentNullException.ThrowIfNull(values);

            if (wavelengths.Length == 0)
                throw new ArgumentException(
                    "Wavelength array cannot be empty.");

            if (names.Count != values.Count)
                throw new ArgumentException(
                    "There must be one name for every spectrum.");

            foreach (var spectrumValues in values)
            {
                if (spectrumValues.Length != wavelengths.Length)
                    throw new ArgumentException(
                        "All spectra must have the same number of values " +
                        "as the wavelength array.");
            }

            Wavelengths = wavelengths;
            Names = names;
            Values = values;
        }

        public List<Spectrum> ToSpectra()
        {
            return Values
                .Select((values, i) =>
                    new Spectrum(
                        Wavelengths,
                        values,
                        Names[i]))
                .ToList();
        }
    }
}
