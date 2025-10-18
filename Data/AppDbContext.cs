using Gaby.io.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gaby.io.Data;

public class AppDbContext : IdentityDbContext<UserModel, IdentityRole<string>, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<CountryModel> Countries { get; set; }
    public DbSet<AuthorModel> Authors { get; set; }
    public DbSet<PublisherModel> Publishers { get; set; }
    public DbSet<GenreModel> Genres { get; set; }
    public DbSet<BookModel> Books { get; set; }
    public DbSet<BookGenreModel> BookGenres { get; set; }
    public DbSet<ReadingModel> Readings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de User → Reading (Restrict)
        // Se o usuário for apagado, preserve as leituras (histórico)
        modelBuilder.Entity<ReadingModel>()
            .HasOne(r => r.User)
            .WithMany(u => u.Readings)
            .HasForeignKey(r => r.UserId)
            .HasConstraintName("FK_Reading_User")
            .OnDelete(DeleteBehavior.Restrict);

        // Configuração de Book → Reading (Cascade)
        // Se um livro for apagado, apague as leituras associadas
        modelBuilder.Entity<ReadingModel>()
            .HasOne(r => r.Book)
            .WithMany(b => b.Readings)
            .HasForeignKey(r => r.BookId)
            .HasConstraintName("FK_Reading_Book")
            .OnDelete(DeleteBehavior.Cascade);

        // Configuração de Author → Book (Restrict)
        // Impede apagar um autor que tenha livros vinculados
        modelBuilder.Entity<BookModel>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .HasConstraintName("FK_Book_Author")
            .OnDelete(DeleteBehavior.Restrict);

        // Configuração de Country → Author (SetNull)
        // Se um país for removido, mantenha o autor e apenas limpe CountryId
        modelBuilder.Entity<AuthorModel>()
            .HasOne(a => a.Country)
            .WithMany(c => c.Authors)
            .HasForeignKey(a => a.CountryId)
            .HasConstraintName("FK_Author_Country")
            .OnDelete(DeleteBehavior.SetNull);

        // Configuração de Publisher → Book (SetNull)
        // Se a editora for excluída, o livro continua existindo sem referência à editora
        modelBuilder.Entity<BookModel>()
            .HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .HasConstraintName("FK_Book_Publisher")
            .OnDelete(DeleteBehavior.SetNull);

        // Configuração de BookGenre (relacionamento muitos-para-muitos)
        modelBuilder.Entity<BookGenreModel>()
            .HasOne(bg => bg.Book)
            .WithMany(b => b.BookGenres)
            .HasForeignKey(bg => bg.BookId)
            .HasConstraintName("FK_BookGenre_Book")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookGenreModel>()
            .HasOne(bg => bg.Genre)
            .WithMany(g => g.BookGenres)
            .HasForeignKey(bg => bg.GenreId)
            .HasConstraintName("FK_BookGenre_Genre")
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único para evitar duplicatas na tabela de relacionamento
        modelBuilder.Entity<BookGenreModel>()
            .HasIndex(bg => new { bg.BookId, bg.GenreId })
            .IsUnique();

        // Índices para melhorar performance
        modelBuilder.Entity<CountryModel>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<ReadingModel>()
            .HasIndex(r => new { r.UserId, r.BookId, r.Year, r.Month });

        modelBuilder.Entity<BookModel>()
            .HasIndex(b => new { b.Title, b.AuthorId })
            .IsUnique();

        modelBuilder.Entity<AuthorModel>()
            .HasIndex(a => a.Name);
    }
}
