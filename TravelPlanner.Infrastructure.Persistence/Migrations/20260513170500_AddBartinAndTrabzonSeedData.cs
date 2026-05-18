using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TravelPlanner.Infrastructure.Persistence;

#nullable disable

namespace TravelPlanner.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(TravelPlannerDbContext))]
    [Migration("20260513170500_AddBartinAndTrabzonSeedData")]
    public partial class AddBartinAndTrabzonSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SET IDENTITY_INSERT [Şehirler] ON;

                INSERT INTO [Şehirler] ([Id], [Slug], [Ad], [Bölge], [Özet], [Başlık], [Açıklama], [Enlem], [Boylam])
                VALUES
                    (102, N'bartin', N'Bartın', N'Karadeniz', N'Amasra kıyısı, lav sütunları ve doğa duraklarıyla kısa kaçamak rotası.', N'Bartın''da sahil ve doğa duraklarını planla', N'Amasra merkezinden Ulukaya Şelalesi''ne uzanan durakları gün sayına göre sırala.', 41.5811, 32.4610),
                    (103, N'trabzon', N'Trabzon', N'Karadeniz', N'Sümela, Uzungöl, tarihi yapılar ve yayla hissiyle yoğun Karadeniz rotası.', N'Trabzon için doğa ve tarih rotası oluştur', N'Şehir merkezi, Maçka ve Uzungöl hattındaki durakları mantıklı sıraya koy.', 41.0027, 39.7168);

                SET IDENTITY_INSERT [Şehirler] OFF;

                SET IDENTITY_INSERT [GezilecekYerler] ON;

                INSERT INTO [GezilecekYerler] ([Id], [ŞehirId], [Ad], [Açıklama], [Kategori], [TahminiGeziSüresi], [TahminiGeziDakikası], [Enlem], [Boylam], [RotaÜzeriMi])
                VALUES
                    (1009, 102, N'Amasra Kalesi', N'Amasra merkezinde deniz manzarasıyla öne çıkan tarihi kale.', N'Kale', N'1 saat', 60, 41.7467, 32.3864, 0),
                    (1010, 102, N'Amasra Müzesi', N'Amasra ve çevresinden arkeolojik ve etnografik eserler sunan müze.', N'Müze', N'1 saat', 60, 41.7462, 32.3861, 0),
                    (1011, 102, N'Güzelcehisar Lav Sütunları', N'Sahil boyunca uzanan lav kayalıkları ve gün batımı manzarası.', N'Doğa', N'1 saat 15 dk', 75, 41.6433, 32.1076, 0),
                    (1012, 102, N'Kuşkayası Yol Anıtı', N'Roma döneminden kalan kaya oyma yol anıtı.', N'Anıt', N'35 dk', 35, 41.7225, 32.4036, 0),
                    (1013, 102, N'Ulukaya Şelalesi', N'Küre Dağları hattında serin ve doğal bir şelale molası.', N'Doğa', N'1 saat', 60, 41.5469, 32.6190, 0),
                    (1014, 102, N'Balamba Tabiat Parkı', N'Bartın merkezine yakın piknik ve doğa yürüyüşü alanı.', N'Park', N'1 saat', 60, 41.6253, 32.3560, 0),
                    (1015, 103, N'Sümela Manastırı', N'Altındere Vadisi''nde kayalık yamaca kurulu ikonik manastır.', N'Manastır', N'2 saat', 120, 40.6892, 39.6586, 0),
                    (1016, 103, N'Atatürk Köşkü', N'Soğuksu''da bahçesiyle birlikte gezilen tarihi köşk ve müze.', N'Müze', N'1 saat', 60, 40.9899, 39.6967, 0),
                    (1017, 103, N'Ayasofya Camii', N'Bizans, Selçuklu ve Osmanlı izlerini taşıyan tarihi yapı.', N'Tarihi Yer', N'1 saat', 60, 41.0034, 39.6966, 0),
                    (1018, 103, N'Trabzon Kalesi', N'Ortahisar hattında şehrin eski sur dokusunu gösteren kale.', N'Kale', N'45 dk', 45, 41.0053, 39.7182, 0),
                    (1019, 103, N'Uzungöl', N'Göl, orman ve dağ manzarasıyla Trabzon''un en bilinen doğa durağı.', N'Doğa', N'2 saat', 120, 40.6192, 40.2886, 0),
                    (1020, 103, N'Çal Mağarası', N'Düzköy''de içinden dere akan uzun yürüyüş rotalı mağara.', N'Mağara', N'1 saat', 60, 40.8650, 39.3790, 0);

                SET IDENTITY_INSERT [GezilecekYerler] OFF;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM [GezilecekYerler] WHERE [Id] BETWEEN 1009 AND 1020;
                DELETE FROM [Şehirler] WHERE [Id] IN (102, 103);
                """);
        }
    }
}
