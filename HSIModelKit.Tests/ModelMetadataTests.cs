using HSIModelKit;

namespace HSIModelKit.Tests;

public class ModelMetadataTests
{
    [Fact]
    public void CanCreateMetadata()
    {
        var metadata = new ModelMetadata
        {
            Name = "Wood Moisture",
            Version = "1.0",
            Description = "Test model",
            FeatureCount = 288
        };

        Assert.Equal("Wood Moisture", metadata.Name);
        Assert.Equal("1.0", metadata.Version);
        Assert.Equal("Test model", metadata.Description);
        Assert.Equal(288, metadata.FeatureCount);
    }
}
