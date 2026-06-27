using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigForAddIndexForAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId_IsDefault",
                table: "Addresses",
                columns: new[] { "UserId", "IsDefault" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId_IsDefault",
                table: "Addresses");
        }
    }
}
