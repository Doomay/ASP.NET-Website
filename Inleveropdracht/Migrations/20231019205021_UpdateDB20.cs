using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inleveropdracht.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RememberMe",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RememberMe",
                table: "Customers");
        }
    }
}
