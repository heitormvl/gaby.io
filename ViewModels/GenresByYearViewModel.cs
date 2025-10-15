namespace Gaby.io.ViewModels;

public class GenresByYearViewModel
{
    public int Year { get; set; }         // Ex: 2023
    public string GenreName { get; set; } = string.Empty; // Nome do gênero
    public int BooksRead { get; set; }    // Quantidade de livros lidos no gênero
}
