using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_NumberOfScan_InOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfScan",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfScan",
                table: "OrderDetails");
        }
    }
}
