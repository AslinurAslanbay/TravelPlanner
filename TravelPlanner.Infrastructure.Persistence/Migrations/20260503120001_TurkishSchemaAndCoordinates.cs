using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TurkishSchemaAndCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Districts_Provinces_ProvinceId", table: "Districts");
            migrationBuilder.DropForeignKey(name: "FK_Localities_Districts_DistrictId", table: "Localities");
            migrationBuilder.DropForeignKey(name: "FK_Neighborhoods_Localities_LocalityId", table: "Neighborhoods");
            migrationBuilder.DropForeignKey(name: "FK_Places_Cities_CityId", table: "Places");

            migrationBuilder.DropTable(name: "RouteLegs");
            migrationBuilder.DropTable(name: "TripStops");
            migrationBuilder.DropTable(name: "TripDays");
            migrationBuilder.DropTable(name: "TripPlans");
            migrationBuilder.DropTable(name: "Places");
            migrationBuilder.DropTable(name: "Cities");

            migrationBuilder.DropPrimaryKey(name: "PK_Provinces", table: "Provinces");
            migrationBuilder.DropPrimaryKey(name: "PK_Districts", table: "Districts");
            migrationBuilder.DropPrimaryKey(name: "PK_Localities", table: "Localities");
            migrationBuilder.DropPrimaryKey(name: "PK_Neighborhoods", table: "Neighborhoods");

            migrationBuilder.RenameTable(name: "Provinces", newName: "İller");
            migrationBuilder.RenameTable(name: "Districts", newName: "İlçeler");
            migrationBuilder.RenameTable(name: "Localities", newName: "YerleşimYerleri");
            migrationBuilder.RenameTable(name: "Neighborhoods", newName: "Mahalleler");

            migrationBuilder.RenameColumn(name: "Name", table: "İller", newName: "Ad");
            migrationBuilder.RenameColumn(name: "ProvinceId", table: "İlçeler", newName: "İlId");
            migrationBuilder.RenameColumn(name: "Name", table: "İlçeler", newName: "Ad");
            migrationBuilder.RenameColumn(name: "DistrictId", table: "YerleşimYerleri", newName: "İlçeId");
            migrationBuilder.RenameColumn(name: "Name", table: "YerleşimYerleri", newName: "Ad");
            migrationBuilder.RenameColumn(name: "LocalityId", table: "Mahalleler", newName: "YerleşimYeriId");
            migrationBuilder.RenameColumn(name: "Name", table: "Mahalleler", newName: "Ad");

            migrationBuilder.RenameIndex(name: "IX_Provinces_Name", table: "İller", newName: "IX_İller_Ad");
            migrationBuilder.RenameIndex(name: "IX_Districts_ProvinceId_Name", table: "İlçeler", newName: "IX_İlçeler_İlId_Ad");
            migrationBuilder.RenameIndex(name: "IX_Localities_DistrictId_Name", table: "YerleşimYerleri", newName: "IX_YerleşimYerleri_İlçeId_Ad");
            migrationBuilder.RenameIndex(name: "IX_Neighborhoods_LocalityId_Name", table: "Mahalleler", newName: "IX_Mahalleler_YerleşimYeriId_Ad");

            migrationBuilder.AddColumn<double>(name: "Enlem", table: "İller", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Boylam", table: "İller", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Enlem", table: "İlçeler", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Boylam", table: "İlçeler", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Enlem", table: "YerleşimYerleri", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Boylam", table: "YerleşimYerleri", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Enlem", table: "Mahalleler", type: "float", nullable: true);
            migrationBuilder.AddColumn<double>(name: "Boylam", table: "Mahalleler", type: "float", nullable: true);

            migrationBuilder.AddPrimaryKey(name: "PK_İller", table: "İller", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_İlçeler", table: "İlçeler", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_YerleşimYerleri", table: "YerleşimYerleri", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Mahalleler", table: "Mahalleler", column: "Id");

            migrationBuilder.CreateTable(
                name: "Şehirler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Bölge = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Özet = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    Başlık = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Açıklama = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    Enlem = table.Column<double>(type: "float", nullable: false),
                    Boylam = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Şehirler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GezilecekYerler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ŞehirId = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Açıklama = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TahminiGeziSüresi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TahminiGeziDakikası = table.Column<int>(type: "int", nullable: false),
                    Enlem = table.Column<double>(type: "float", nullable: false),
                    Boylam = table.Column<double>(type: "float", nullable: false),
                    RotaÜzeriMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GezilecekYerler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GezilecekYerler_Şehirler_ŞehirId",
                        column: x => x.ŞehirId,
                        principalTable: "Şehirler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_Şehirler_Slug", table: "Şehirler", column: "Slug", unique: true);
            migrationBuilder.CreateIndex(name: "IX_GezilecekYerler_ŞehirId_Ad", table: "GezilecekYerler", columns: new[] { "ŞehirId", "Ad" });

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

            InsertTravelSeedData(migrationBuilder);

            migrationBuilder.Sql(@"
UPDATE [İller]
SET [Enlem] = 37.4187481, [Boylam] = 42.4918338
WHERE [Id] = 73 OR [Ad] = N'Şırnak';

UPDATE [İlçeler]
SET [Enlem] = 37.33481, [Boylam] = 41.88944
WHERE [Id] = 910 OR ([İlId] = 73 AND [Ad] = N'İdil');

UPDATE [Mahalleler]
SET [Enlem] = 37.38419, [Boylam] = 41.819763
WHERE [Id] = 49054 OR ([Ad] = N'BEREKETLİ KÖYÜ' AND [YerleşimYeriId] IN (
    SELECT yy.[Id]
    FROM [YerleşimYerleri] yy
    INNER JOIN [İlçeler] i ON i.[Id] = yy.[İlçeId]
    WHERE i.[Ad] = N'İdil' AND i.[İlId] = 73
));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "GezilecekYerler");
            migrationBuilder.DropTable(name: "Şehirler");

            migrationBuilder.DropForeignKey(name: "FK_Mahalleler_YerleşimYerleri_YerleşimYeriId", table: "Mahalleler");
            migrationBuilder.DropForeignKey(name: "FK_YerleşimYerleri_İlçeler_İlçeId", table: "YerleşimYerleri");
            migrationBuilder.DropForeignKey(name: "FK_İlçeler_İller_İlId", table: "İlçeler");

            migrationBuilder.DropPrimaryKey(name: "PK_Mahalleler", table: "Mahalleler");
            migrationBuilder.DropPrimaryKey(name: "PK_YerleşimYerleri", table: "YerleşimYerleri");
            migrationBuilder.DropPrimaryKey(name: "PK_İlçeler", table: "İlçeler");
            migrationBuilder.DropPrimaryKey(name: "PK_İller", table: "İller");

            migrationBuilder.DropColumn(name: "Enlem", table: "Mahalleler");
            migrationBuilder.DropColumn(name: "Boylam", table: "Mahalleler");
            migrationBuilder.DropColumn(name: "Enlem", table: "YerleşimYerleri");
            migrationBuilder.DropColumn(name: "Boylam", table: "YerleşimYerleri");
            migrationBuilder.DropColumn(name: "Enlem", table: "İlçeler");
            migrationBuilder.DropColumn(name: "Boylam", table: "İlçeler");
            migrationBuilder.DropColumn(name: "Enlem", table: "İller");
            migrationBuilder.DropColumn(name: "Boylam", table: "İller");

            migrationBuilder.RenameTable(name: "Mahalleler", newName: "Neighborhoods");
            migrationBuilder.RenameTable(name: "YerleşimYerleri", newName: "Localities");
            migrationBuilder.RenameTable(name: "İlçeler", newName: "Districts");
            migrationBuilder.RenameTable(name: "İller", newName: "Provinces");

            migrationBuilder.RenameColumn(name: "YerleşimYeriId", table: "Neighborhoods", newName: "LocalityId");
            migrationBuilder.RenameColumn(name: "Ad", table: "Neighborhoods", newName: "Name");
            migrationBuilder.RenameColumn(name: "İlçeId", table: "Localities", newName: "DistrictId");
            migrationBuilder.RenameColumn(name: "Ad", table: "Localities", newName: "Name");
            migrationBuilder.RenameColumn(name: "İlId", table: "Districts", newName: "ProvinceId");
            migrationBuilder.RenameColumn(name: "Ad", table: "Districts", newName: "Name");
            migrationBuilder.RenameColumn(name: "Ad", table: "Provinces", newName: "Name");

            migrationBuilder.RenameIndex(name: "IX_Mahalleler_YerleşimYeriId_Ad", table: "Neighborhoods", newName: "IX_Neighborhoods_LocalityId_Name");
            migrationBuilder.RenameIndex(name: "IX_YerleşimYerleri_İlçeId_Ad", table: "Localities", newName: "IX_Localities_DistrictId_Name");
            migrationBuilder.RenameIndex(name: "IX_İlçeler_İlId_Ad", table: "Districts", newName: "IX_Districts_ProvinceId_Name");
            migrationBuilder.RenameIndex(name: "IX_İller_Ad", table: "Provinces", newName: "IX_Provinces_Name");

            migrationBuilder.AddPrimaryKey(name: "PK_Neighborhoods", table: "Neighborhoods", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Localities", table: "Localities", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Districts", table: "Districts", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Provinces", table: "Provinces", column: "Id");
        }

        private static void InsertTravelSeedData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Şehirler",
                columns: new[] { "Id", "Slug", "Ad", "Bölge", "Özet", "Başlık", "Açıklama", "Enlem", "Boylam" },
                values: new object[,]
                {
                    { 82, "istanbul", "İstanbul", "Marmara", "Tarihi yarımada, Boğaz ve semt duraklarıyla yoğun bir şehir rotası.", "İstanbul'da seçeceğin durakları mantıklı sıraya koy", "Başlangıç adresinden seçilen yerlere yakınlık ve süreye göre plan oluştur.", 41.0082, 28.9784 },
                    { 83, "edirne", "Edirne", "Marmara", "Selimiye, tarihi çarşılar ve Meriç hattıyla seçtiğin gün sayısına göre planlanabilen rota.", "Edirne için gün gün gezi planı hazırla", "Yol üstü molalar ve şehir içi durakları aynı rota planında topla.", 41.6771, 26.5557 }
                });

            migrationBuilder.InsertData(
                table: "GezilecekYerler",
                columns: new[] { "Id", "ŞehirId", "Ad", "Açıklama", "Kategori", "TahminiGeziSüresi", "TahminiGeziDakikası", "Enlem", "Boylam", "RotaÜzeriMi" },
                values: new object[,]
                {
                    { 84, 82, "Ayasofya Camii", "İstanbul'un en ikonik tarihi yapılarından biri.", "Tarihi Yer", "1 saat", 60, 41.0086, 28.9802, false },
                    { 85, 82, "Topkapı Sarayı", "Osmanlı saray yaşamını keşfetmek için güçlü bir durak.", "Saray", "2 saat", 120, 41.0115, 28.9833, false },
                    { 86, 82, "Galata Kulesi", "Şehir siluetini izlemek için sevilen nokta.", "Manzara", "45 dk", 45, 41.0256, 28.9741, false },
                    { 87, 83, "Selimiye Camii", "Mimar Sinan'ın ustalık eseri olarak bilinen merkez durak.", "Tarihi Yer", "1 saat", 60, 41.6781, 26.5594, false },
                    { 88, 83, "Ali Paşa Çarşısı", "Tarihi çarşı ve alışveriş durağı.", "Çarşı", "45 dk", 45, 41.6756, 26.5566, false },
                    { 89, 83, "Meriç Köprüsü", "Nehir kenarında yürüyüş ve fotoğraf molası.", "Manzara", "30 dk", 30, 41.6687, 26.5434, false },
                    { 90, 83, "Silivri Sahil", "İstanbul-Edirne yolunda kahve ve sahil molası.", "Yol Üstü", "40 dk", 40, 41.0736, 28.2464, true },
                    { 91, 83, "Çorlu Atalar Meydanı", "Yol üstünde kısa yemek ve dinlenme durağı.", "Yol Üstü", "45 dk", 45, 41.1591, 27.8025, true },
                    { 92, 83, "Lüleburgaz Sokullu Mehmet Paşa Külliyesi", "Edirne yolunda tarihi bir mola noktası.", "Yol Üstü", "50 dk", 50, 41.4065, 27.3549, true }
                });
        }
    }
}
