using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UseSeparatedTableIdRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_GezilecekYerler_Şehirler_ŞehirId", table: "GezilecekYerler");
            migrationBuilder.DropForeignKey(name: "FK_İlçeler_İller_İlId", table: "İlçeler");
            migrationBuilder.DropForeignKey(name: "FK_YerleşimYerleri_İlçeler_İlçeId", table: "YerleşimYerleri");
            migrationBuilder.DropForeignKey(name: "FK_Mahalleler_YerleşimYerleri_YerleşimYeriId", table: "Mahalleler");

            migrationBuilder.Sql("""
UPDATE [Mahalleler]
SET [YerleşimYeriId] = [YerleşimYeriId] + 19999
WHERE [YerleşimYeriId] BETWEEN 1 AND 19999;

UPDATE [Mahalleler]
SET [Id] = [Id] + 99999
WHERE [Id] BETWEEN 1 AND 99999;

UPDATE [YerleşimYerleri]
SET [İlçeId] = [İlçeId] + 9999
WHERE [İlçeId] BETWEEN 1 AND 9999;

UPDATE [YerleşimYerleri]
SET [Id] = [Id] + 19999
WHERE [Id] BETWEEN 1 AND 19999;

UPDATE [İlçeler]
SET [Id] = [Id] + 9999
WHERE [Id] BETWEEN 1 AND 9999;
""");

            migrationBuilder.Sql("""
SET IDENTITY_INSERT [Şehirler] ON;

UPDATE [Şehirler]
SET [Slug] = CONCAT([Slug], '-range-', [Id])
WHERE [Id] IN (82, 83)
  AND [Slug] IN (N'istanbul', N'edirne');

INSERT INTO [Şehirler] ([Id], [Slug], [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam])
SELECT
    CASE [Id] WHEN 82 THEN 100 WHEN 83 THEN 101 END,
    CASE [Id] WHEN 82 THEN N'istanbul' WHEN 83 THEN N'edirne' END,
    [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam]
FROM [Şehirler]
WHERE [Id] IN (82, 83)
  AND NOT EXISTS (
      SELECT 1
      FROM [Şehirler] yeni
      WHERE yeni.[Id] = CASE [Şehirler].[Id] WHEN 82 THEN 100 WHEN 83 THEN 101 END
  );

SET IDENTITY_INSERT [Şehirler] OFF;

SET IDENTITY_INSERT [GezilecekYerler] ON;

INSERT INTO [GezilecekYerler] ([Id], [ŞehirId], [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi])
SELECT
    [Id] + 916,
    CASE [ŞehirId] WHEN 82 THEN 100 WHEN 83 THEN 101 ELSE [ŞehirId] END,
    [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi]
FROM [GezilecekYerler]
WHERE [Id] BETWEEN 84 AND 92
  AND NOT EXISTS (
      SELECT 1
      FROM [GezilecekYerler] yeni
      WHERE yeni.[Id] = [GezilecekYerler].[Id] + 916
  );

SET IDENTITY_INSERT [GezilecekYerler] OFF;

DELETE FROM [GezilecekYerler]
WHERE [Id] BETWEEN 84 AND 92;

DELETE FROM [Şehirler]
WHERE [Id] IN (82, 83);

DBCC CHECKIDENT (N'[Şehirler]', RESEED, 101);
DBCC CHECKIDENT (N'[GezilecekYerler]', RESEED, 1008);
""");

            migrationBuilder.AddForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler",
                column: "ŞehirId",
                principalTable: "Şehirler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_İlçeler_İller_İlId",
                table: "İlçeler",
                column: "İlId",
                principalTable: "İller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YerleşimYerleri_İlçeler_İlçeId",
                table: "YerleşimYerleri",
                column: "İlçeId",
                principalTable: "İlçeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mahalleler_YerleşimYerleri_YerleşimYeriId",
                table: "Mahalleler",
                column: "YerleşimYeriId",
                principalTable: "YerleşimYerleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_GezilecekYerler_Şehirler_ŞehirId", table: "GezilecekYerler");
            migrationBuilder.DropForeignKey(name: "FK_İlçeler_İller_İlId", table: "İlçeler");
            migrationBuilder.DropForeignKey(name: "FK_YerleşimYerleri_İlçeler_İlçeId", table: "YerleşimYerleri");
            migrationBuilder.DropForeignKey(name: "FK_Mahalleler_YerleşimYerleri_YerleşimYeriId", table: "Mahalleler");

            migrationBuilder.Sql("""
SET IDENTITY_INSERT [Şehirler] ON;

UPDATE [Şehirler]
SET [Slug] = CONCAT([Slug], '-range-', [Id])
WHERE [Id] IN (100, 101)
  AND [Slug] IN (N'istanbul', N'edirne');

INSERT INTO [Şehirler] ([Id], [Slug], [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam])
SELECT
    CASE [Id] WHEN 100 THEN 82 WHEN 101 THEN 83 END,
    CASE [Id] WHEN 100 THEN N'istanbul' WHEN 101 THEN N'edirne' END,
    [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam]
FROM [Şehirler]
WHERE [Id] IN (100, 101)
  AND NOT EXISTS (
      SELECT 1
      FROM [Şehirler] eski
      WHERE eski.[Id] = CASE [Şehirler].[Id] WHEN 100 THEN 82 WHEN 101 THEN 83 END
  );

SET IDENTITY_INSERT [Şehirler] OFF;

SET IDENTITY_INSERT [GezilecekYerler] ON;

INSERT INTO [GezilecekYerler] ([Id], [ŞehirId], [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi])
SELECT
    [Id] - 916,
    CASE [ŞehirId] WHEN 100 THEN 82 WHEN 101 THEN 83 ELSE [ŞehirId] END,
    [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi]
FROM [GezilecekYerler]
WHERE [Id] BETWEEN 1000 AND 1008
  AND NOT EXISTS (
      SELECT 1
      FROM [GezilecekYerler] eski
      WHERE eski.[Id] = [GezilecekYerler].[Id] - 916
  );

SET IDENTITY_INSERT [GezilecekYerler] OFF;

DELETE FROM [GezilecekYerler]
WHERE [Id] BETWEEN 1000 AND 1008;

DELETE FROM [Şehirler]
WHERE [Id] IN (100, 101);

DBCC CHECKIDENT (N'[Şehirler]', RESEED, 83);
DBCC CHECKIDENT (N'[GezilecekYerler]', RESEED, 92);
""");

            migrationBuilder.Sql("""
UPDATE [İlçeler]
SET [Id] = [Id] - 9999
WHERE [Id] BETWEEN 10000 AND 19999;

UPDATE [YerleşimYerleri]
SET [Id] = [Id] - 19999
WHERE [Id] BETWEEN 20000 AND 99999;

UPDATE [YerleşimYerleri]
SET [İlçeId] = [İlçeId] - 9999
WHERE [İlçeId] BETWEEN 10000 AND 19999;

UPDATE [Mahalleler]
SET [Id] = [Id] - 99999
WHERE [Id] BETWEEN 100000 AND 999999;

UPDATE [Mahalleler]
SET [YerleşimYeriId] = [YerleşimYeriId] - 19999
WHERE [YerleşimYeriId] BETWEEN 20000 AND 99999;
""");

            migrationBuilder.AddForeignKey(
                name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                table: "GezilecekYerler",
                column: "ŞehirId",
                principalTable: "Şehirler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_İlçeler_İller_İlId",
                table: "İlçeler",
                column: "İlId",
                principalTable: "İller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_YerleşimYerleri_İlçeler_İlçeId",
                table: "YerleşimYerleri",
                column: "İlçeId",
                principalTable: "İlçeler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mahalleler_YerleşimYerleri_YerleşimYeriId",
                table: "Mahalleler",
                column: "YerleşimYeriId",
                principalTable: "YerleşimYerleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
