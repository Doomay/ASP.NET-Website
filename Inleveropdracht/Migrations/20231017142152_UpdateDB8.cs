using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inleveropdracht.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerPassword",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPassword",
                table: "Customers");
        }
    }
}
