using SpectrumKit.IO;
using SpectrumKit.Models;
using SpectrumKit.Visualization;

string path = @"D:\PERSONAL\Coding\test_spectrum.csv";

try
{
    SpectrumSet spectrumSet = SpectrumLoader.LoadCsv(path);

    List<Spectrum> spectra = spectrumSet.ToSpectra();

    Spectrum spectrum = spectra[1];

    AxisTickResult ticks = AxisTicks.Generate(
    0,
    1.175,
    6);

    Console.WriteLine($"Minimum: {ticks.Minimum}");
    Console.WriteLine($"Maximum: {ticks.Maximum}");
    Console.WriteLine($"Step: {ticks.Step}");

    Console.WriteLine("Y ticks:");

    foreach (double tick in ticks.Ticks)
    {
        Console.WriteLine(tick);
    }

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