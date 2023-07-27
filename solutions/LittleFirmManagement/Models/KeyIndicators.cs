public class KeyIndicatorsIndexViewModel
{
    public int SelectedGranularity { get; set; }
    public int SelectedDetails { get; set; }
    public decimal[,] Data { get; set; }
    public List<List<string>> Labels { get; set; }
    public int N { get; set; }
    public DateTime Begin { get; set; }
    public Dictionary<int, string> DetailsMapping { get; set; }
    public Dictionary<int, string> GranularityMapping { get; set; }
}