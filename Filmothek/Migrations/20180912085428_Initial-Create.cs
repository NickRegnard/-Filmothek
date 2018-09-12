using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Filmothek.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Pw = table.Column<string>(nullable: true),
                    Rights = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moderator",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Login = table.Column<string>(nullable: true),
                    Pw = table.Column<string>(nullable: true),
                    Rights = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moderator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MovieName = table.Column<string>(nullable: true),
                    Genre = table.Column<string>(nullable: true),
                    Lenght = table.Column<int>(nullable: true),
                    IsSeries = table.Column<bool>(nullable: false),
                    Rating = table.Column<float>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    LanguageDub = table.Column<string>(nullable: true),
                    LanguageSub = table.Column<string>(nullable: true),
                    Release = table.Column<DateTime>(nullable: false),
                    FSK = table.Column<int>(nullable: true),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerId = table.Column<int>(nullable: false),
                    PaypalPassword = table.Column<string>(nullable: true),
                    PaypalLogin = table.Column<string>(nullable: true),
                    CreditcardTyp = table.Column<string>(nullable: true),
                    CreditcardOwner = table.Column<string>(nullable: true),
                    CreditcardNumber = table.Column<long>(nullable: true),
                    CreditcardSecret = table.Column<int>(nullable: true),
                    CreditcardExpire = table.Column<string>(nullable: true),
                    BankaccOwner = table.Column<string>(nullable: true),
                    BankaccIBAN = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModeratorHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ModeratorId = table.Column<int>(nullable: false),
                    Activity = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratorHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModeratorHistory_Moderator_ModeratorId",
                        column: x => x.ModeratorId,
                        principalTable: "Moderator",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MovieId = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    startDate = table.Column<DateTime>(nullable: false),
                    endDate = table.Column<DateTime>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    isBorrowing = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerHistory_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerHistory_Movie_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHistory_CustomerId",
                table: "CustomerHistory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerHistory_MovieId",
                table: "CustomerHistory",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratorHistory_ModeratorId",
                table: "ModeratorHistory",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_CustomerId",
                table: "PaymentMethod",
                column: "CustomerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerHistory");

            migrationBuilder.DropTable(
                name: "ModeratorHistory");

            migrationBuilder.DropTable(
                name: "PaymentMethod");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropTable(
                name: "Moderator");

            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
