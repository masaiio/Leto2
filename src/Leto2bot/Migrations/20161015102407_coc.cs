﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Leto2bot.Migrations
{
    public partial class coc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "ClashCallers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "ClashCallers");
        }
    }
}
