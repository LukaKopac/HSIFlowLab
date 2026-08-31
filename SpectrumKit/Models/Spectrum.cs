namespace SpectrumKit.Models
{
    public class Spectrum
    {
        public string? Name { get; }
        public double[] Wavelengths { get; }
        public double[] Values { get; }

        public Spectrum(
            double[] wavelengths,
            double[] values,
            string? name = null)
        {
            ArgumentNullException.ThrowIfNull(wavelengths);
            ArgumentNullException.ThrowIfNull(values);

            if (wavelengths.Length == 0 || values.Length == 0)
                throw new ArgumentException(
                    "Wavelength and value arrays cannot be empty.");
            
            if (wavelengths.Length != values.Length)
                throw new ArgumentException(
                    "Wavelength and value arrays must have the same length");
            
            Wavelengths = wavelengths;
            Values = values;
            Name = name;
        }

        public double Max()
        {
            double currentMax = Values[0];
            
            for (int i = 1;  i < Values.Length; i++)
            {
                if (Values[i] > currentMax)
                {
                    currentMax = Values[i];
                }
            }

            return currentMax;
        }

        public double Min()
        {
            double currentMin = Values[0];

            for (int i = 1; i < Values.Length; i++)
            {
                if (Values[i] < currentMin)
                {
                    currentMin = Values[i];
                }
            }

            return currentMin;
        }

        public double MaxWavelength()
        {
            double currentMax = Wavelengths[0];

            for (int i = 1; i < Wavelengths.Length; i++)
            {
                if (Wavelengths[i] > currentMax)
                {
                    currentMax = Wavelengths[i];
                }
            }

            return currentMax;
        }

        public double MinWavelength()
        {
            double currentMin = Wavelengths[0];

            for (int i = 1; i < Wavelengths.Length; i++)
            {
                if (Wavelengths[i] < currentMin)
                {
                    currentMin = Wavelengths[i];
                }
            }

            return currentMin;
        }

        public double Mean()
        {
            double currentSum = 0;

            for (int i = 0; i < Values.Length; i++)
            {
                currentSum += Values[i];
            }

            double mean = currentSum / Values.Length;

            return mean;
        }
    }
}
