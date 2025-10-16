namespace Gaby.io.ViewModels;

public class CountryDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public List<string>? Authors { get; set; } = new();
}
