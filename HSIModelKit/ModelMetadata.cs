namespace HSIModelKit;

public class ModelMetadata
{
    public string Name { get; init; } = string.Empty;

    public string Version { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int FeatureCount { get; init; }
}
