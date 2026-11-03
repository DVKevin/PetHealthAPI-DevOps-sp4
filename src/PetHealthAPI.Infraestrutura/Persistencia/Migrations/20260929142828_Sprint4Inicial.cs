using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetHealthAPI.Infraestrutura.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_PH_TUTOR",
                columns: table => new
                {
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_TUTOR = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    NR_TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DS_ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_TUTOR", x => x.ID_TUTOR);
                });

            migrationBuilder.CreateTable(
                name: "TB_PH_USUARIO",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_USUARIO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DS_SENHA_HASH = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    DS_PERFIL = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_USUARIO", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "TB_PH_PET",
                columns: table => new
                {
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_PET = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DS_RACA = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    NR_IDADE = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NR_PESO = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    DS_SEXO = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    FL_CASTRADO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DS_ALERGIAS = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    DT_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DT_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_PET", x => x.ID_PET);
                    table.ForeignKey(
                        name: "FK_TB_PH_PET_TB_PH_TUTOR_ID_TUTOR",
                        column: x => x.ID_TUTOR,
                        principalTable: "TB_PH_TUTOR",
                        principalColumn: "ID_TUTOR",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PH_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DT_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DS_MOTIVO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NM_VETERINARIO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    NM_CLINICA = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    DS_DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DS_TRATAMENTO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    VL_CUSTO = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: true),
                    DT_RETORNO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    DS_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_TB_PH_CONSULTA_TB_PH_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PH_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PH_MEDICAMENTO",
                columns: table => new
                {
                    ID_MEDICAMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_MEDICAMENTO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DS_DOSAGEM = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DS_FREQUENCIA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DT_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NM_VETERINARIO_PRESCREVEU = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    FL_ATIVO = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DS_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_MEDICAMENTO", x => x.ID_MEDICAMENTO);
                    table.ForeignKey(
                        name: "FK_TB_PH_MEDICAMENTO_TB_PH_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PH_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PH_VACINA",
                columns: table => new
                {
                    ID_VACINA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_VACINA = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DT_APLICACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_PROXIMA_DOSE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NM_FABRICANTE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    NR_LOTE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    NM_VETERINARIO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    DS_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PH_VACINA", x => x.ID_VACINA);
                    table.ForeignKey(
                        name: "FK_TB_PH_VACINA_TB_PH_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PH_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_CONSULTA_ID_PET",
                table: "TB_PH_CONSULTA",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_MEDICAMENTO_ID_PET",
                table: "TB_PH_MEDICAMENTO",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_PET_ID_TUTOR",
                table: "TB_PH_PET",
                column: "ID_TUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_TUTOR_DS_EMAIL",
                table: "TB_PH_TUTOR",
                column: "DS_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_USUARIO_DS_EMAIL",
                table: "TB_PH_USUARIO",
                column: "DS_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_PH_VACINA_ID_PET",
                table: "TB_PH_VACINA",
                column: "ID_PET");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PH_CONSULTA");

            migrationBuilder.DropTable(
                name: "TB_PH_MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "TB_PH_USUARIO");

            migrationBuilder.DropTable(
                name: "TB_PH_VACINA");

            migrationBuilder.DropTable(
                name: "TB_PH_PET");

            migrationBuilder.DropTable(
                name: "TB_PH_TUTOR");
        }
    }
}
