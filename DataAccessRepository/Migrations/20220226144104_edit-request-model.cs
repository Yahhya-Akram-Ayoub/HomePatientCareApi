using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessRepository.Migrations
{
    public partial class editrequestmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Service_SeviceId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "SeviceId",
                table: "Request",
                newName: "SeviceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_SeviceId",
                table: "Request",
                newName: "IX_Request_SeviceTypeId");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Request",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_ServiceId",
                table: "Request",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Service_ServiceId",
                table: "Request",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ServiceType_SeviceTypeId",
                table: "Request",
                column: "SeviceTypeId",
                principalTable: "ServiceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_Service_ServiceId",
                table: "Request");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ServiceType_SeviceTypeId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_ServiceId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "SeviceTypeId",
                table: "Request",
                newName: "SeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_Request_SeviceTypeId",
                table: "Request",
                newName: "IX_Request_SeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Service_SeviceId",
                table: "Request",
                column: "SeviceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
