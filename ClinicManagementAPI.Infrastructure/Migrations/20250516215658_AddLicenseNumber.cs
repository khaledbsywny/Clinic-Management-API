using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagementAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenseNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClinicAddress",
                table: "Doctors",
                newName: "LicenseNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "Doctors",
                newName: "ClinicAddress");
        }
    }
}
