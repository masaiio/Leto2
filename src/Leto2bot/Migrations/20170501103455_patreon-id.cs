﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Leto2bot.Migrations
{
    public partial class patreonid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PatreonUserId",
                table: "RewardedUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatreonUserId",
                table: "RewardedUsers");
        }
    }
}
