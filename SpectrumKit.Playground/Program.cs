using SpectrumKit.IO;
using SpectrumKit.Models;
using SpectrumKit.Visualization;

string path = @"D:\PERSONAL\Coding\test_spectrum.csv";

try
{
    SpectrumSet spectrumSet = SpectrumLoader.LoadCsv(path);

    List<Spectrum> spectra = spectrumSet.ToSpectra();

    Spectrum spectrum = spectra[1];

    string svg = SpectrumPlotter.ToSvg(spectrum);

    File.WriteAllText(
        "test.svg",
        svg,
        new System.Text.UTF8Encoding(false));

    Console.WriteLine(Path.GetFullPath("test.svg"));

}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}