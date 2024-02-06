using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirWatcherNew.Migrations
{
    
    public partial class InitialCreate1 : Migration
    {
      
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccurrencesOfMagicString",
                table: "TaskRunDetails",
                newName: "TotalOccurrences");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalRuntime",
                table: "TaskRunDetails",
                type: "time",
                nullable: true);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRuntime",
                table: "TaskRunDetails");

            migrationBuilder.RenameColumn(
                name: "TotalOccurrences",
                table: "TaskRunDetails",
                newName: "OccurrencesOfMagicString");
        }
    }
}
