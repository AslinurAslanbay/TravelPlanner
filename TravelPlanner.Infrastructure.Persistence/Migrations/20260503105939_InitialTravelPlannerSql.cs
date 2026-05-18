using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTravelPlannerSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    HeroTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    HeroDescription = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TripPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    StartAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StartLatitude = table.Column<double>(type: "float", nullable: true),
                    StartLongitude = table.Column<double>(type: "float", nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationCityName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    DestinationLatitude = table.Column<double>(type: "float", nullable: true),
                    DestinationLongitude = table.Column<double>(type: "float", nullable: true),
                    TravelDays = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TransportationMode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TotalDistanceKm = table.Column<double>(type: "float", nullable: false),
                    TotalTravelMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CityId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EstimatedVisitDuration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EstimatedVisitMinutes = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    IsOnRouteCandidate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Places_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteLegs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    LegOrder = table.Column<int>(type: "int", nullable: false),
                    FromTripStopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToTripStopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FromAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ToAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TrafficDurationMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteLegs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteLegs_TripPlans_TripPlanId",
                        column: x => x.TripPlanId,
                        principalTable: "TripPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayNumber = table.Column<int>(type: "int", nullable: false),
                    TripDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedTravelMinutes = table.Column<int>(type: "int", nullable: false),
                    EstimatedVisitMinutes = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripDays_TripPlans_TripPlanId",
                        column: x => x.TripPlanId,
                        principalTable: "TripPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripStops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TripDayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StopOrder = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    EstimatedVisitMinutes = table.Column<int>(type: "int", nullable: false),
                    DistanceFromPreviousKm = table.Column<double>(type: "float", nullable: false),
                    TravelMinutesFromPrevious = table.Column<int>(type: "int", nullable: false),
                    PlannedArrivalTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    PlannedDepartureTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IsOnRouteStop = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TripStops_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TripStops_TripDays_TripDayId",
                        column: x => x.TripDayId,
                        principalTable: "TripDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "HeroDescription", "HeroTitle", "Name", "Region", "Slug", "Summary" },
                values: new object[,]
                {
                    { "edirne", "Yol ustu molalar ve sehir ici duraklari ayni rota planinda topla.", "Edirne icin gun gun gezi plani hazirla", "Edirne", "Marmara", "edirne", "Selimiye, tarihi carsilar ve Meric hattiyla iki gunluk geziye uygun rota." },
                    { "istanbul", "Baslangic adresinden secilen yerlere yakinlik ve sureye gore plan olustur.", "Istanbul'da sececegin duraklari mantikli siraya koy", "Istanbul", "Marmara", "istanbul", "Tarihi yarimada, bogaz ve semt duraklariyla yogun bir sehir rotasi." }
                });

            migrationBuilder.InsertData(
                table: "Places",
                columns: new[] { "Id", "Category", "CityId", "Description", "EstimatedVisitDuration", "EstimatedVisitMinutes", "IsOnRouteCandidate", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { "alic-pasa-carsisi", "Carsi", "edirne", "Tarihi carsi ve alisveris duragi.", "45 dk", 45, false, 41.675600000000003, 26.5566, "Ali Pasa Carsisi" },
                    { "ayasofya", "Tarihi Yer", "istanbul", "Istanbul'un en ikonik tarihi yapilarindan biri.", "1 saat", 60, false, 41.008600000000001, 28.9802, "Ayasofya Camii" },
                    { "corlu-atalar-meydani", "Yol Ustu", "edirne", "Yol ustunde kisa yemek ve dinlenme duragi.", "45 dk", 45, true, 41.159100000000002, 27.802499999999998, "Corlu Atalar Meydani" },
                    { "galata", "Manzara", "istanbul", "Sehir siluetini izlemek icin sevilen nokta.", "45 dk", 45, false, 41.025599999999997, 28.9741, "Galata Kulesi" },
                    { "luleburgaz-sokullu", "Yol Ustu", "edirne", "Edirne yolunda tarihi bir mola noktasi.", "50 dk", 50, true, 41.406500000000001, 27.354900000000001, "Luleburgaz Sokullu Mehmet Pasa Kulliyesi" },
                    { "meric-koprusu", "Manzara", "edirne", "Nehir kenarinda yuruyus ve fotograf molasi.", "30 dk", 30, false, 41.668700000000001, 26.543399999999998, "Meric Koprusu" },
                    { "selimiye", "Tarihi Yer", "edirne", "Mimar Sinan'in ustalik eseri olarak bilinen merkez durak.", "1 saat", 60, false, 41.678100000000001, 26.5594, "Selimiye Camii" },
                    { "silivri-sahil", "Yol Ustu", "edirne", "Istanbul-Edirne yolunda kahve ve sahil molasi.", "40 dk", 40, true, 41.073599999999999, 28.246400000000001, "Silivri Sahil" },
                    { "topkapi", "Saray", "istanbul", "Osmanli saray yasamini kesfetmek icin guclu bir durak.", "2 saat", 120, false, 41.011499999999998, 28.9833, "Topkapi Sarayi" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Slug",
                table: "Cities",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Places_CityId_Name",
                table: "Places",
                columns: new[] { "CityId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteLegs_TripPlanId_DayNumber_LegOrder",
                table: "RouteLegs",
                columns: new[] { "TripPlanId", "DayNumber", "LegOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripDays_TripPlanId_DayNumber",
                table: "TripDays",
                columns: new[] { "TripPlanId", "DayNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TripStops_PlaceId",
                table: "TripStops",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TripStops_TripDayId_StopOrder",
                table: "TripStops",
                columns: new[] { "TripDayId", "StopOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RouteLegs");

            migrationBuilder.DropTable(
                name: "TripStops");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "TripDays");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "TripPlans");
        }
    }
}
