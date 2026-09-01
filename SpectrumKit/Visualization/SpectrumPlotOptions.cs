namespace SpectrumKit.Visualization
{
    public class SpectrumPlotOptions
    {
        public double Width { get; set; } = 800;
        public double Height { get; set; } = 500;

        public double? YMin { get; set; }
        public double? YMax { get; set; }

        public double YMinPadding { get; set; } = 0.1;
        public double YMaxPadding { get; set; } = 0.1;

        public double MarginLeft { get; set; } = 70;
        public double MarginRight { get; set; } = 30;
        public double MarginTop { get; set; } = 30;
        public double MarginBottom { get; set; } = 50;

        public string? Title { get; set; }
        public string? XLabel { get; set; }
        public string? YLabel { get; set; }

        public bool ShowGrid { get; set; } = true;

        public string SpectrumColor { get; set; } = "blue";
        public double SpectrumLineWidth { get; set; } = 1;
    }
}
