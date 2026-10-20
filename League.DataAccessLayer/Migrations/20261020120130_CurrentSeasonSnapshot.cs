using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace League.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class CurrentSeasonSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Stadiums_StadiumId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUpdatedAt",
                schema: "dbo",
                table: "Seasons",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataVersion",
                schema: "dbo",
                table: "Seasons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                schema: "dbo",
                table: "Seasons",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondHalfHomeGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SecondHalfAwayGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HomeShots",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HomePossession",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "HomeCorners",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FirstHalfHomeGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FirstHalfAwayGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AwayShots",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AwayCorners",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AddedMinute",
                schema: "dbo",
                table: "MatchEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "StadiumId",
                schema: "dbo",
                table: "Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SourceMatchId",
                schema: "dbo",
                table: "Matches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TimeConfirmed",
                schema: "dbo",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Event_AddedMinute",
                schema: "dbo",
                table: "MatchEvents",
                sql: "AddedMinute BETWEEN 0 AND 30 AND (AddedMinute = 0 OR Minute IN (45,90,105,120))");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SourceMatchId",
                schema: "dbo",
                table: "Matches",
                column: "SourceMatchId",
                unique: true,
                filter: "[SourceMatchId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Stadiums_StadiumId",
                schema: "dbo",
                table: "Matches",
                column: "StadiumId",
                principalSchema: "dbo",
                principalTable: "Stadiums",
                principalColumn: "StadiumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Stadiums_StadiumId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Event_AddedMinute",
                schema: "dbo",
                table: "MatchEvents");

            migrationBuilder.DropIndex(
                name: "IX_Matches_SourceMatchId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "DataUpdatedAt",
                schema: "dbo",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "DataVersion",
                schema: "dbo",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                schema: "dbo",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "AddedMinute",
                schema: "dbo",
                table: "MatchEvents");

            migrationBuilder.DropColumn(
                name: "SourceMatchId",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "TimeConfirmed",
                schema: "dbo",
                table: "Matches");

            migrationBuilder.AlterColumn<int>(
                name: "SecondHalfHomeGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondHalfAwayGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HomeShots",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HomePossession",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "HomeCorners",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FirstHalfHomeGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FirstHalfAwayGoals",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayShots",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AwayCorners",
                schema: "dbo",
                table: "MatchStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StadiumId",
                schema: "dbo",
                table: "Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Stadiums_StadiumId",
                schema: "dbo",
                table: "Matches",
                column: "StadiumId",
                principalSchema: "dbo",
                principalTable: "Stadiums",
                principalColumn: "StadiumId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
