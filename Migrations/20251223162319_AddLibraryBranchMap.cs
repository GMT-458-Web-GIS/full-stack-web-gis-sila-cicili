using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NetTopologySuite.Geometries; // ⚠️ Bu kütüphane Point için şart!

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryBranchMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. ADIM: PostGIS Eklentisini Veritabanında Aktif Et
            // Bu satır olmadan harita verisi tutamazsın.
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            // 2. ADIM: SADECE YENİ TABLOYU OLUŞTUR (LibraryBranches)
            // Var olan books, users vb. tabloları buraya koymuyoruz!
            migrationBuilder.CreateTable(
                name: "LibraryBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    
                    // İşte GIS farkı burada: Tip "geometry" ve C# tarafında "Point"
                    Location = table.Column<Point>(type: "geometry", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryBranches", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eğer migration geri alınırsa sadece bu yeni tabloyu sil
            migrationBuilder.DropTable(
                name: "LibraryBranches");

            // PostGIS eklentisini de kapat (Tercihen)
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");
        }
    }
}