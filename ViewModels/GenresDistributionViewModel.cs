namespace Gaby.io.ViewModels;

public class GenresDistributionViewModel
{
    public string GenreName { get; set; } = string.Empty; // Nome do gênero
    public int BooksRead { get; set; }        // Quantidade de livros lidos no gênero
    public int TotalPages { get; set; }       // (Opcional) total de páginas, se quiser usar no tooltip
}
