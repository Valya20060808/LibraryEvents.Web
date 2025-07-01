using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryEvents.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "Registrations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "Registrations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Registrations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Registrations");
        }
    }
}
