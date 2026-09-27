using HSIDataKit.Models;
using HSIModelKit.IO;
using HSIModelKit.Models;
using System.Text.Json;

namespace HSIModelKit.Tests;

public class ModelManifestLoaderTests
{
    [Fact]
    public void Load_ValidPackage_ReturnsManifest()
    {
        using var package = new TemporaryModelPackage();

        ModelManifest manifest = ModelManifestLoader.Load(package.DirectoryPath);

        Assert.Equal("test-model", manifest.ModelId);
        Assert.Equal("Test model", manifest.Name);
        Assert.Equal("1.0", manifest.Version);
        Assert.Equal(CubeDataKind.Reflectance, manifest.RequiredDataKind);
        Assert.Equal(2, manifest.ExpectedBandCount);
        Assert.Equal(new[] { 500.0, 600.0 }, manifest.ExpectedWavelengthsNm);
    }

    [Fact]
    public void Load_UnsupportedFormatVersion_ThrowsNotSupportedException()
    {
        using var package = new TemporaryModelPackage();
        package.WriteManifest(formatVersion: 2);

        Assert.Throws<NotSupportedException>(
            () => ModelManifestLoader.Load(package.DirectoryPath));
    }

    [Fact]
    public void Load_MissingModelFile_ThrowsFileNotFoundException()
    {
        using var package = new TemporaryModelPackage();
        package.DeleteModelFile();

        Assert.Throws<FileNotFoundException>(
            () => ModelManifestLoader.Load(package.DirectoryPath));
    }

    [Fact]
    public void Load_ModelFileEscapesPackage_ThrowsInvalidDataException()
    {
        using var package = new TemporaryModelPackage(modelFile: "../outside.joblib");

        Assert.Throws<InvalidDataException>(
            () => ModelManifestLoader.Load(package.DirectoryPath));
    }

    private sealed class TemporaryModelPackage : IDisposable
    {
        public string DirectoryPath { get; } =
            Path.Combine(
                Path.GetTempPath(),
                $"HSIModelKit.Tests-{Guid.NewGuid():N}");

        private readonly string modelFile;

        public TemporaryModelPackage(string modelFile = "model.joblib")
        {
            this.modelFile = modelFile;

            Directory.CreateDirectory(DirectoryPath);
            File.WriteAllText(Path.Combine(DirectoryPath, "model.joblib"), "placeholder");
            WriteManifest();
        }

        public void WriteManifest(int formatVersion = 1)
        {
            var manifest = new
            {
                formatVersion,
                modelId = "test-model",
                name = "Test model",
                version = "1.0",
                modelFile,
                requiredDataKind = "Reflectance",
                expectedBandCount = 2,
                expectedWavelengthsNm = new[] { 500.0, 600.0 },
                wavelengthToleranceNm = 1.0
            };

            string json = JsonSerializer.Serialize(manifest);
            File.WriteAllText(Path.Combine(DirectoryPath, "manifest.json"), json);
        }

        public void DeleteModelFile()
        {
            string modelPath = Path.Combine(DirectoryPath, "model.joblib");

            if (File.Exists(modelPath))
            {
                File.Delete(modelPath);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
    }
}
