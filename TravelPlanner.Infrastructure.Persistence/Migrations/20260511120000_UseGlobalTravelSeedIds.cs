using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseGlobalTravelSeedIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler");

            migrationBuilder.Sql("""
SET IDENTITY_INSERT [Şehirler] ON;

UPDATE [Şehirler]
SET [Slug] = CONCAT([Slug], '-legacy-', [Id])
WHERE [Id] IN (1, 2)
  AND [Slug] IN (N'istanbul', N'edirne');

INSERT INTO [Şehirler] ([Id], [Slug], [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam])
SELECT
    CASE [Id] WHEN 1 THEN 82 WHEN 2 THEN 83 END,
    CASE [Id] WHEN 1 THEN N'istanbul' WHEN 2 THEN N'edirne' END,
    [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam]
FROM [Şehirler]
WHERE [Id] IN (1, 2)
  AND NOT EXISTS (
      SELECT 1
      FROM [Şehirler] yeni
      WHERE yeni.[Id] = CASE [Şehirler].[Id] WHEN 1 THEN 82 WHEN 2 THEN 83 END
  );

SET IDENTITY_INSERT [Şehirler] OFF;

SET IDENTITY_INSERT [GezilecekYerler] ON;

INSERT INTO [GezilecekYerler] ([Id], [ŞehirId], [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi])
SELECT
    CASE [Id]
        WHEN 1 THEN 84
        WHEN 2 THEN 85
        WHEN 3 THEN 86
        WHEN 4 THEN 87
        WHEN 5 THEN 88
        WHEN 6 THEN 89
        WHEN 7 THEN 90
        WHEN 8 THEN 91
        WHEN 9 THEN 92
    END,
    CASE [ŞehirId] WHEN 1 THEN 82 WHEN 2 THEN 83 ELSE [ŞehirId] END,
    [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi]
FROM [GezilecekYerler]
WHERE [Id] BETWEEN 1 AND 9
  AND NOT EXISTS (
      SELECT 1
      FROM [GezilecekYerler] yeni
      WHERE yeni.[Id] = CASE [GezilecekYerler].[Id]
          WHEN 1 THEN 84
          WHEN 2 THEN 85
          WHEN 3 THEN 86
          WHEN 4 THEN 87
          WHEN 5 THEN 88
          WHEN 6 THEN 89
          WHEN 7 THEN 90
          WHEN 8 THEN 91
          WHEN 9 THEN 92
      END
  );

SET IDENTITY_INSERT [GezilecekYerler] OFF;

DELETE FROM [GezilecekYerler]
WHERE [Id] BETWEEN 1 AND 9;

DELETE FROM [Şehirler]
WHERE [Id] IN (1, 2);

DBCC CHECKIDENT (N'[Şehirler]', RESEED, 83);
DBCC CHECKIDENT (N'[GezilecekYerler]', RESEED, 92);
""");

            migrationBuilder.AddForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler",
                column: "ŞehirId",
                principalTable: "Şehirler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler");

            migrationBuilder.Sql("""
SET IDENTITY_INSERT [Şehirler] ON;

UPDATE [Şehirler]
SET [Slug] = CONCAT([Slug], '-global-', [Id])
WHERE [Id] IN (82, 83)
  AND [Slug] IN (N'istanbul', N'edirne');

INSERT INTO [Şehirler] ([Id], [Slug], [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam])
SELECT
    CASE [Id] WHEN 82 THEN 1 WHEN 83 THEN 2 END,
    CASE [Id] WHEN 82 THEN N'istanbul' WHEN 83 THEN N'edirne' END,
    [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam]
FROM [Şehirler]
WHERE [Id] IN (82, 83)
  AND NOT EXISTS (
      SELECT 1
      FROM [Şehirler] eski
      WHERE eski.[Id] = CASE [Şehirler].[Id] WHEN 82 THEN 1 WHEN 83 THEN 2 END
  );

SET IDENTITY_INSERT [Şehirler] OFF;

SET IDENTITY_INSERT [GezilecekYerler] ON;

INSERT INTO [GezilecekYerler] ([Id], [ŞehirId], [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi])
SELECT
    CASE [Id]
        WHEN 84 THEN 1
        WHEN 85 THEN 2
        WHEN 86 THEN 3
        WHEN 87 THEN 4
        WHEN 88 THEN 5
        WHEN 89 THEN 6
        WHEN 90 THEN 7
        WHEN 91 THEN 8
        WHEN 92 THEN 9
    END,
    CASE [ŞehirId] WHEN 82 THEN 1 WHEN 83 THEN 2 ELSE [ŞehirId] END,
    [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi]
FROM [GezilecekYerler]
WHERE [Id] BETWEEN 84 AND 92
  AND NOT EXISTS (
      SELECT 1
      FROM [GezilecekYerler] eski
      WHERE eski.[Id] = CASE [GezilecekYerler].[Id]
          WHEN 84 THEN 1
          WHEN 85 THEN 2
          WHEN 86 THEN 3
          WHEN 87 THEN 4
          WHEN 88 THEN 5
          WHEN 89 THEN 6
          WHEN 90 THEN 7
          WHEN 91 THEN 8
          WHEN 92 THEN 9
      END
  );

SET IDENTITY_INSERT [GezilecekYerler] OFF;

DELETE FROM [GezilecekYerler]
WHERE [Id] BETWEEN 84 AND 92;

DELETE FROM [Şehirler]
WHERE [Id] IN (82, 83);

DBCC CHECKIDENT (N'[Şehirler]', RESEED, 2);
DBCC CHECKIDENT (N'[GezilecekYerler]', RESEED, 9);
""");

            migrationBuilder.AddForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler",
                column: "ŞehirId",
                principalTable: "Şehirler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
