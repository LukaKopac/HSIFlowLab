using HSIDataKit.Models;
using HSIModelKit.Compatibility;
using HSIModelKit.Models;

namespace HSIModelKit.Tests;

public class ModelCompatibilityValidatorTests
{
    [Fact]
    public void Validate_MatchingMetadata_IsCompatible()
    {
        ModelCompatibilityResult result =
            ModelCompatibilityValidator.Validate(
                CreateManifest(),
                CreateMetadata());

        Assert.True(result.IsCompatible);
        Assert.Empty(result.Issues);
    }

    [Fact]
    public void Validate_DataKindMismatch_IsIncompatible()
    {
        HsiMetadata metadata = CreateMetadata(
            dataKind: CubeDataKind.Raw);

        ModelCompatibilityResult result =
            ModelCompatibilityValidator.Validate(
                CreateManifest(),
                metadata);

        Assert.False(result.IsCompatible);
        Assert.Contains(result.Issues, issue => issue.Contains("data"));
    }

    [Fact]
    public void Validate_BandCountMismatch_IsIncompatible()
    {
        HsiMetadata metadata = CreateMetadata(
            bands: 3,
            wavelengths: [500.0, 600.0, 700.0]);

        ModelCompatibilityResult result =
            ModelCompatibilityValidator.Validate(
                CreateManifest(),
                metadata);

        Assert.False(result.IsCompatible);
        Assert.Contains(result.Issues, issue => issue.Contains("bands"));
    }

    [Fact]
    public void Validate_WavelengthOutsideTolerance_IsIncompatible()
    {
        HsiMetadata metadata = CreateMetadata(
            wavelengths: [500.0, 603.0]);

        ModelCompatibilityResult result =
            ModelCompatibilityValidator.Validate(
                CreateManifest(),
                metadata);

        Assert.False(result.IsCompatible);
        Assert.Contains(result.Issues, issue => issue.Contains("wavelength"));
    }

    private static ModelManifest CreateManifest()
    {
        return new ModelManifest
        {
            ModelId = "test-model",
            Name = "Test model",
            Version = "1.0",
            RequiredDataKind = CubeDataKind.Reflectance,
            ExpectedBandCount = 2,
            ExpectedWavelengthsNm = [500.0, 600.0],
            WavelengthToleranceNm = 1.0
        };
    }

    private static HsiMetadata CreateMetadata(
        CubeDataKind dataKind = CubeDataKind.Reflectance,
        int bands = 2,
        double[]? wavelengths = null)
    {
        return new HsiMetadata
        {
            DataKind = dataKind,
            Bands = bands,
            Wavelengths = wavelengths ?? [500.0, 600.0]
        };
    }
}
