using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessRepository.Migrations
{
    public partial class editModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VolunteerInfo_UserId",
                table: "VolunteerInfo");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VolunteerInfoId",
                table: "Service",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "distance",
                table: "RequestReceivers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerInfo_UserId",
                table: "VolunteerInfo",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_VolunteerInfoId",
                table: "Service",
                column: "VolunteerInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_VolunteerInfo_VolunteerInfoId",
                table: "Service",
                column: "VolunteerInfoId",
                principalTable: "VolunteerInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_VolunteerInfo_VolunteerInfoId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_VolunteerInfo_UserId",
                table: "VolunteerInfo");

            migrationBuilder.DropIndex(
                name: "IX_Service_VolunteerInfoId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VolunteerInfoId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "distance",
                table: "RequestReceivers");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerInfo_UserId",
                table: "VolunteerInfo",
                column: "UserId");
        }
    }
}
