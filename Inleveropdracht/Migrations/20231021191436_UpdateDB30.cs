using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inleveropdracht.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Customers");
        }
    }
}
