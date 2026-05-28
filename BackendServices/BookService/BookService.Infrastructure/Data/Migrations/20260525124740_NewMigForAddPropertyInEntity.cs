using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigForAddPropertyInEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "RestoredBookHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "RestoredBookHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "RestoredBookHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "RestoredBookHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "RestoredBookHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "DeletedBookHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "DeletedBookHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ISBN",
                table: "DeletedBookHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "DeletedBookHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "DeletedBookHistories",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Author",
                table: "RestoredBookHistories");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "RestoredBookHistories");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "RestoredBookHistories");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "RestoredBookHistories");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "RestoredBookHistories");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "DeletedBookHistories");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "DeletedBookHistories");

            migrationBuilder.DropColumn(
                name: "ISBN",
                table: "DeletedBookHistories");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "DeletedBookHistories");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "DeletedBookHistories");
        }
    }
}
