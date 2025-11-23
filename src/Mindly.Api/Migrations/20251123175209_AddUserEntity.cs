using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindly.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Criar tabela Users primeiro
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            // Criar usuário padrão para sessões existentes (se houver)
            var defaultUserId = new Guid("11111111-1111-1111-1111-111111111111");
            var now = new DateTime(2025, 11, 23, 0, 0, 0, DateTimeKind.Utc);
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "Email", "CreatedAt", "UpdatedAt" },
                values: new object[] { defaultUserId, "Usuário Padrão", "default@mindly.com", now, now });

            // Adicionar coluna UserId permitindo NULL temporariamente
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "FocusSessions",
                type: "TEXT",
                nullable: true);

            // Atualizar todas as sessões existentes para usar o usuário padrão
            migrationBuilder.Sql($"UPDATE FocusSessions SET UserId = '{defaultUserId}' WHERE UserId IS NULL");

            // Tornar a coluna NOT NULL agora que todos os registros têm um valor válido
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "FocusSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: defaultUserId,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId",
                table: "FocusSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FocusSessions_Users_UserId",
                table: "FocusSessions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FocusSessions_Users_UserId",
                table: "FocusSessions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FocusSessions");
        }
    }
}
