using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Identity.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIdentityCodesTableByAddingActivationTineColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationTime",
                table: "IdentityCodes",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationTime",
                table: "IdentityCodes");
        }
    }
}
