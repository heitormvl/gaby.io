using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace gaby.io.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<char>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Author_Country",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AuthorId = table.Column<int>(type: "integer", nullable: false),
                    PublisherId = table.Column<int>(type: "integer", nullable: true),
                    PageCount = table.Column<int>(type: "integer", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Book_Author",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Book_Publisher",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BookGenres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookGenres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookGenre_Book",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookGenre_Genre",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Readings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PagesRead = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Readings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reading_Book",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reading_User",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "AFG", "Afeganistão" },
                    { 2, "ZAF", "África do Sul" },
                    { 3, "ALB", "Albânia" },
                    { 4, "DEU", "Alemanha" },
                    { 5, "AND", "Andorra" },
                    { 6, "AGO", "Angola" },
                    { 7, "ATG", "Antígua e Barbuda" },
                    { 8, "SAU", "Arábia Saudita" },
                    { 9, "DZA", "Argélia" },
                    { 10, "ARG", "Argentina" },
                    { 11, "ARM", "Armênia" },
                    { 12, "AUS", "Austrália" },
                    { 13, "AUT", "Áustria" },
                    { 14, "AZE", "Azerbaijão" },
                    { 15, "BHS", "Bahamas" },
                    { 16, "BGD", "Bangladesh" },
                    { 17, "BRB", "Barbados" },
                    { 18, "BHR", "Barém" },
                    { 19, "BEL", "Bélgica" },
                    { 20, "BLZ", "Belize" },
                    { 21, "BEN", "Benin" },
                    { 22, "BLR", "Bielorrússia" },
                    { 23, "BOL", "Bolívia" },
                    { 24, "BIH", "Bósnia e Herzegovina" },
                    { 25, "BWA", "Botsuana" },
                    { 26, "BRA", "Brasil" },
                    { 27, "BRN", "Brunei" },
                    { 28, "BGR", "Bulgária" },
                    { 29, "BFA", "Burquina Faso" },
                    { 30, "BDI", "Burundi" },
                    { 31, "BTN", "Butão" },
                    { 32, "CPV", "Cabo Verde" },
                    { 33, "CMR", "Camarões" },
                    { 34, "KHM", "Camboja" },
                    { 35, "CAN", "Canadá" },
                    { 36, "QAT", "Catar" },
                    { 37, "KAZ", "Cazaquistão" },
                    { 38, "TCD", "Chade" },
                    { 39, "CHL", "Chile" },
                    { 40, "CHN", "China" },
                    { 41, "CYP", "Chipre" },
                    { 42, "COL", "Colômbia" },
                    { 43, "COM", "Comores" },
                    { 44, "COG", "Congo" },
                    { 45, "PRK", "Coreia do Norte" },
                    { 46, "KOR", "Coreia do Sul" },
                    { 47, "CIV", "Costa do Marfim" },
                    { 48, "CRI", "Costa Rica" },
                    { 49, "HRV", "Croácia" },
                    { 50, "CUB", "Cuba" },
                    { 51, "DNK", "Dinamarca" },
                    { 52, "DJI", "Djibuti" },
                    { 53, "DMA", "Dominica" },
                    { 54, "EGY", "Egito" },
                    { 55, "SLV", "El Salvador" },
                    { 56, "ARE", "Emirados Árabes Unidos" },
                    { 57, "ECU", "Equador" },
                    { 58, "ERI", "Eritreia" },
                    { 59, "SVK", "Eslováquia" },
                    { 60, "SVN", "Eslovênia" },
                    { 61, "ESP", "Espanha" },
                    { 62, "USA", "Estados Unidos" },
                    { 63, "EST", "Estônia" },
                    { 64, "SWZ", "Eswatini" },
                    { 65, "ETH", "Etiópia" },
                    { 66, "FJI", "Fiji" },
                    { 67, "PHL", "Filipinas" },
                    { 68, "FIN", "Finlândia" },
                    { 69, "FRA", "França" },
                    { 70, "GAB", "Gabão" },
                    { 71, "GMB", "Gâmbia" },
                    { 72, "GHA", "Gana" },
                    { 73, "GEO", "Geórgia" },
                    { 74, "GRD", "Granada" },
                    { 75, "GRC", "Grécia" },
                    { 76, "GTM", "Guatemala" },
                    { 77, "GUY", "Guiana" },
                    { 78, "GIN", "Guiné" },
                    { 79, "GNQ", "Guiné Equatorial" },
                    { 80, "GNB", "Guiné-Bissau" },
                    { 81, "HTI", "Haiti" },
                    { 82, "HND", "Honduras" },
                    { 83, "HUN", "Hungria" },
                    { 84, "YEM", "Iêmen" },
                    { 85, "MHL", "Ilhas Marshall" },
                    { 86, "SLB", "Ilhas Salomão" },
                    { 87, "IND", "Índia" },
                    { 88, "IDN", "Indonésia" },
                    { 89, "IRN", "Irã" },
                    { 90, "IRQ", "Iraque" },
                    { 91, "IRL", "Irlanda" },
                    { 92, "ISL", "Islândia" },
                    { 93, "ISR", "Israel" },
                    { 94, "ITA", "Itália" },
                    { 95, "JAM", "Jamaica" },
                    { 96, "JPN", "Japão" },
                    { 97, "JOR", "Jordânia" },
                    { 98, "KIR", "Kiribati" },
                    { 99, "KWT", "Kuwait" },
                    { 100, "LAO", "Laos" },
                    { 101, "LSO", "Lesoto" },
                    { 102, "LVA", "Letônia" },
                    { 103, "LBN", "Líbano" },
                    { 104, "LBR", "Libéria" },
                    { 105, "LBY", "Líbia" },
                    { 106, "LIE", "Liechtenstein" },
                    { 107, "LTU", "Lituânia" },
                    { 108, "LUX", "Luxemburgo" },
                    { 109, "MDG", "Madagascar" },
                    { 110, "MYS", "Malásia" },
                    { 111, "MWI", "Malauí" },
                    { 112, "MDV", "Maldivas" },
                    { 113, "MLI", "Mali" },
                    { 114, "MLT", "Malta" },
                    { 115, "MAR", "Marrocos" },
                    { 116, "MUS", "Maurício" },
                    { 117, "MRT", "Mauritânia" },
                    { 118, "MEX", "México" },
                    { 119, "FSM", "Micronésia" },
                    { 120, "MOZ", "Moçambique" },
                    { 121, "MDA", "Moldávia" },
                    { 122, "MCO", "Mônaco" },
                    { 123, "MNG", "Mongólia" },
                    { 124, "MNE", "Montenegro" },
                    { 125, "MMR", "Myanmar" },
                    { 126, "NAM", "Namíbia" },
                    { 127, "NRU", "Nauru" },
                    { 128, "NPL", "Nepal" },
                    { 129, "NIC", "Nicarágua" },
                    { 130, "NER", "Níger" },
                    { 131, "NGA", "Nigéria" },
                    { 132, "NOR", "Noruega" },
                    { 133, "NZL", "Nova Zelândia" },
                    { 134, "OMN", "Omã" },
                    { 135, "NLD", "Países Baixos" },
                    { 136, "PLW", "Palau" },
                    { 137, "PAN", "Panamá" },
                    { 138, "PNG", "Papua-Nova Guiné" },
                    { 139, "PAK", "Paquistão" },
                    { 140, "PRY", "Paraguai" },
                    { 141, "PER", "Peru" },
                    { 142, "POL", "Polônia" },
                    { 143, "PRT", "Portugal" },
                    { 144, "KEN", "Quênia" },
                    { 145, "KGZ", "Quirguistão" },
                    { 146, "GBR", "Reino Unido" },
                    { 147, "CAF", "República Centro-Africana" },
                    { 148, "COD", "República Democrática do Congo" },
                    { 149, "DOM", "República Dominicana" },
                    { 150, "CZE", "República Tcheca" },
                    { 151, "ROU", "Romênia" },
                    { 152, "RWA", "Ruanda" },
                    { 153, "RUS", "Rússia" },
                    { 154, "WSM", "Samoa" },
                    { 155, "SMR", "San Marino" },
                    { 156, "LCA", "Santa Lúcia" },
                    { 157, "KNA", "São Cristóvão e Névis" },
                    { 158, "STP", "São Tomé e Príncipe" },
                    { 159, "VCT", "São Vicente e Granadinas" },
                    { 160, "SEN", "Senegal" },
                    { 161, "SLE", "Serra Leoa" },
                    { 162, "SRB", "Sérvia" },
                    { 163, "SYC", "Seychelles" },
                    { 164, "SGP", "Singapura" },
                    { 165, "SYR", "Síria" },
                    { 166, "SOM", "Somália" },
                    { 167, "LKA", "Sri Lanka" },
                    { 168, "SDN", "Sudão" },
                    { 169, "SSD", "Sudão do Sul" },
                    { 170, "SWE", "Suécia" },
                    { 171, "CHE", "Suíça" },
                    { 172, "SUR", "Suriname" },
                    { 173, "THA", "Tailândia" },
                    { 174, "TWN", "Taiwan" },
                    { 175, "TJK", "Tajiquistão" },
                    { 176, "TZA", "Tanzânia" },
                    { 177, "TLS", "Timor-Leste" },
                    { 178, "TGO", "Togo" },
                    { 179, "TON", "Tonga" },
                    { 180, "TTO", "Trinidad e Tobago" },
                    { 181, "TUN", "Tunísia" },
                    { 182, "TKM", "Turcomenistão" },
                    { 183, "TUR", "Turquia" },
                    { 184, "TUV", "Tuvalu" },
                    { 185, "UKR", "Ucrânia" },
                    { 186, "UGA", "Uganda" },
                    { 187, "URY", "Uruguai" },
                    { 188, "UZB", "Uzbequistão" },
                    { 189, "VUT", "Vanuatu" },
                    { 190, "VAT", "Vaticano" },
                    { 191, "VEN", "Venezuela" },
                    { 192, "VNM", "Vietnã" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Authors_CountryId",
                table: "Authors",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_Name",
                table: "Authors",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_BookGenres_BookId_GenreId",
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookGenres_GenreId",
                table: "BookGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_AuthorId",
                table: "Books",
                columns: new[] { "Title", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Code",
                table: "Countries",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_Name",
                table: "Genres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Readings_BookId",
                table: "Readings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Readings_UserId_BookId_Year_Month",
                table: "Readings",
                columns: new[] { "UserId", "BookId", "Year", "Month" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BookGenres");

            migrationBuilder.DropTable(
                name: "Readings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Publishers");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
