using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _231044K_FreshFarmMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordHistoryFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PasswordHistories_CreatedAt",
                table: "PasswordHistories",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PasswordHistories_CreatedAt",
                table: "PasswordHistories");
        }
    }
}
