using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProyectoColegio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Prof2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    CursoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imagen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.CursoID);
                });

            migrationBuilder.CreateTable(
                name: "Docente",
                columns: table => new
                {
                    DocenteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dni = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdLogin = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docente", x => x.DocenteID);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Clases",
                columns: table => new
                {
                    ClaseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocenteID = table.Column<int>(type: "int", nullable: false),
                    CursoID = table.Column<int>(type: "int", nullable: false),
                    DiaClase = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    TipoGrado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clases", x => x.ClaseID);
                    table.ForeignKey(
                        name: "FK_Clases_Cursos_CursoID",
                        column: x => x.CursoID,
                        principalTable: "Cursos",
                        principalColumn: "CursoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clases_Docente_DocenteID",
                        column: x => x.DocenteID,
                        principalTable: "Docente",
                        principalColumn: "DocenteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    Dni = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoGrado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grado = table.Column<int>(type: "int", nullable: false),
                    RoleID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuario_Rol_RoleID1",
                        column: x => x.RoleID1,
                        principalTable: "Rol",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "ClasesEstudiantes",
                columns: table => new
                {
                    ClasesEstudiantesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClasesID = table.Column<int>(type: "int", nullable: false),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    C1 = table.Column<float>(type: "real", nullable: false),
                    C2 = table.Column<float>(type: "real", nullable: false),
                    C3 = table.Column<float>(type: "real", nullable: false),
                    C4 = table.Column<float>(type: "real", nullable: false),
                    Final = table.Column<float>(type: "real", nullable: false),
                    TipoGrado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Grado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClasesEstudiantes", x => x.ClasesEstudiantesID);
                    table.ForeignKey(
                        name: "FK_ClasesEstudiantes_Clases_ClasesID",
                        column: x => x.ClasesID,
                        principalTable: "Clases",
                        principalColumn: "ClaseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClasesEstudiantes_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "RoleID", "Description" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Docente" },
                    { 3, "Estudiante" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clases_CursoID",
                table: "Clases",
                column: "CursoID");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_DocenteID",
                table: "Clases",
                column: "DocenteID");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesEstudiantes_ClasesID",
                table: "ClasesEstudiantes",
                column: "ClasesID");

            migrationBuilder.CreateIndex(
                name: "IX_ClasesEstudiantes_UsuarioID",
                table: "ClasesEstudiantes",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_RoleID1",
                table: "Usuario",
                column: "RoleID1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClasesEstudiantes");

            migrationBuilder.DropTable(
                name: "Clases");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "Docente");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
