using Microsoft.EntityFrameworkCore;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Infrastructure.Persistence;

public sealed class TravelPlannerDbContext : DbContext
{
    public TravelPlannerDbContext(DbContextOptions<TravelPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<Place> Places => Set<Place>();
    public DbSet<Province> Provinces => Set<Province>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Locality> Localities => Set<Locality>();
    public DbSet<Neighborhood> Neighborhoods => Set<Neighborhood>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Şehirler");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.Slug).HasColumnName("Slug").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Region).HasColumnName("Bölge").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Summary).HasColumnName("Özet").HasMaxLength(700).IsRequired();
            entity.Property(x => x.HeroTitle).HasColumnName("Başlık").HasMaxLength(250).IsRequired();
            entity.Property(x => x.HeroDescription).HasColumnName("Açıklama").HasMaxLength(700).IsRequired();
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.HasIndex(x => x.Slug).IsUnique();
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.ToTable("GezilecekYerler");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id");
            entity.Property(x => x.CityId).HasColumnName("ŞehirId").IsRequired();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(160).IsRequired();
            entity.Property(x => x.Description).HasColumnName("Açıklama").HasMaxLength(700).IsRequired();
            entity.Property(x => x.Category).HasColumnName("Kategori").HasMaxLength(80).IsRequired();
            entity.Property(x => x.EstimatedVisitDuration).HasColumnName("TahminiGeziSüresi").HasMaxLength(50).IsRequired();
            entity.Property(x => x.EstimatedVisitMinutes).HasColumnName("TahminiGeziDakikası");
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.Property(x => x.IsOnRouteCandidate).HasColumnName("RotaÜzeriMi");
            entity.HasOne(x => x.City)
                .WithMany(x => x.Places)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.CityId, x.Name });
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("İller");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(80).IsRequired();
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.HasIndex(x => x.Name);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("İlçeler");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.ProvinceId).HasColumnName("İlId").IsRequired();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(80).IsRequired();
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.HasOne(x => x.Province)
                .WithMany(x => x.Districts)
                .HasForeignKey(x => x.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.ProvinceId, x.Name });
        });

        modelBuilder.Entity<Locality>(entity =>
        {
            entity.ToTable("YerleşimYerleri");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.DistrictId).HasColumnName("İlçeId").IsRequired();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.HasOne(x => x.District)
                .WithMany(x => x.Localities)
                .HasForeignKey(x => x.DistrictId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.DistrictId, x.Name });
        });

        modelBuilder.Entity<Neighborhood>(entity =>
        {
            entity.ToTable("Mahalleler");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("Id").ValueGeneratedNever();
            entity.Property(x => x.LocalityId).HasColumnName("YerleşimYeriId").IsRequired();
            entity.Property(x => x.Name).HasColumnName("Ad").HasMaxLength(160).IsRequired();
            entity.Property(x => x.Latitude).HasColumnName("Enlem");
            entity.Property(x => x.Longitude).HasColumnName("Boylam");
            entity.HasOne(x => x.Locality)
                .WithMany(x => x.Neighborhoods)
                .HasForeignKey(x => x.LocalityId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => new { x.LocalityId, x.Name });
        });

        SeedTravelData(modelBuilder);
        SeedKnownLocationCoordinates(modelBuilder);
    }

    private static void SeedTravelData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City
            {
                Id = 100,
                Slug = "istanbul",
                Name = "İstanbul",
                Region = "Marmara",
                Summary = "Tarihi yarımada, Boğaz ve semt duraklarıyla yoğun bir şehir rotası.",
                HeroTitle = "İstanbul'da seçeceğin durakları mantıklı sıraya koy",
                HeroDescription = "Başlangıç adresinden seçilen yerlere yakınlık ve süreye göre plan oluştur.",
                Latitude = 41.0082,
                Longitude = 28.9784
            },
            new City
            {
                Id = 101,
                Slug = "edirne",
                Name = "Edirne",
                Region = "Marmara",
                Summary = "Selimiye, tarihi çarşılar ve Meriç hattıyla seçtiğin gün sayısına göre planlanabilen rota.",
                HeroTitle = "Edirne için gün gün gezi planı hazırla",
                HeroDescription = "Yol üstü molalar ve şehir içi durakları aynı rota planında topla.",
                Latitude = 41.6771,
                Longitude = 26.5557
            },
            new City
            {
                Id = 102,
                Slug = "bartin",
                Name = "Bartın",
                Region = "Karadeniz",
                Summary = "Amasra kıyısı, lav sütunları ve doğa duraklarıyla kısa kaçamak rotası.",
                HeroTitle = "Bartın'da sahil ve doğa duraklarını planla",
                HeroDescription = "Amasra merkezinden Ulukaya Şelalesi'ne uzanan durakları gün sayına göre sırala.",
                Latitude = 41.5811,
                Longitude = 32.4610
            },
            new City
            {
                Id = 103,
                Slug = "trabzon",
                Name = "Trabzon",
                Region = "Karadeniz",
                Summary = "Sümela, Uzungöl, tarihi yapılar ve yayla hissiyle yoğun Karadeniz rotası.",
                HeroTitle = "Trabzon için doğa ve tarih rotası oluştur",
                HeroDescription = "Şehir merkezi, Maçka ve Uzungöl hattındaki durakları mantıklı sıraya koy.",
                Latitude = 41.0027,
                Longitude = 39.7168
            });

        modelBuilder.Entity<Place>().HasData(
            new Place { Id = 1000, CityId = 100, Name = "Ayasofya Camii", Description = "İstanbul'un en ikonik tarihi yapılarından biri.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.0086, Longitude = 28.9802, IsOnRouteCandidate = false },
            new Place { Id = 1001, CityId = 100, Name = "Topkapı Sarayı", Description = "Osmanlı saray yaşamını keşfetmek için güçlü bir durak.", Category = "Saray", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 41.0115, Longitude = 28.9833, IsOnRouteCandidate = false },
            new Place { Id = 1002, CityId = 100, Name = "Galata Kulesi", Description = "Şehir siluetini izlemek için sevilen nokta.", Category = "Manzara", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.0256, Longitude = 28.9741, IsOnRouteCandidate = false },
            new Place { Id = 1003, CityId = 101, Name = "Selimiye Camii", Description = "Mimar Sinan'ın ustalık eseri olarak bilinen merkez durak.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.6781, Longitude = 26.5594, IsOnRouteCandidate = false },
            new Place { Id = 1004, CityId = 101, Name = "Ali Paşa Çarşısı", Description = "Tarihi çarşı ve alışveriş durağı.", Category = "Çarşı", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.6756, Longitude = 26.5566, IsOnRouteCandidate = false },
            new Place { Id = 1005, CityId = 101, Name = "Meriç Köprüsü", Description = "Nehir kenarında yürüyüş ve fotoğraf molası.", Category = "Manzara", EstimatedVisitDuration = "30 dk", EstimatedVisitMinutes = 30, Latitude = 41.6687, Longitude = 26.5434, IsOnRouteCandidate = false },
            new Place { Id = 1006, CityId = 101, Name = "Silivri Sahil", Description = "İstanbul-Edirne yolunda kahve ve sahil molası.", Category = "Yol Üstü", EstimatedVisitDuration = "40 dk", EstimatedVisitMinutes = 40, Latitude = 41.0736, Longitude = 28.2464, IsOnRouteCandidate = true },
            new Place { Id = 1007, CityId = 101, Name = "Çorlu Atalar Meydanı", Description = "Yol üstünde kısa yemek ve dinlenme durağı.", Category = "Yol Üstü", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.1591, Longitude = 27.8025, IsOnRouteCandidate = true },
            new Place { Id = 1008, CityId = 101, Name = "Lüleburgaz Sokullu Mehmet Paşa Külliyesi", Description = "Edirne yolunda tarihi bir mola noktası.", Category = "Yol Üstü", EstimatedVisitDuration = "50 dk", EstimatedVisitMinutes = 50, Latitude = 41.4065, Longitude = 27.3549, IsOnRouteCandidate = true },
            new Place { Id = 1009, CityId = 102, Name = "Amasra Kalesi", Description = "Amasra merkezinde deniz manzarasıyla öne çıkan tarihi kale.", Category = "Kale", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.7467, Longitude = 32.3864, IsOnRouteCandidate = false },
            new Place { Id = 1010, CityId = 102, Name = "Amasra Müzesi", Description = "Amasra ve çevresinden arkeolojik ve etnografik eserler sunan müze.", Category = "Müze", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.7462, Longitude = 32.3861, IsOnRouteCandidate = false },
            new Place { Id = 1011, CityId = 102, Name = "Güzelcehisar Lav Sütunları", Description = "Sahil boyunca uzanan lav kayalıkları ve gün batımı manzarası.", Category = "Doğa", EstimatedVisitDuration = "1 saat 15 dk", EstimatedVisitMinutes = 75, Latitude = 41.6433, Longitude = 32.1076, IsOnRouteCandidate = false },
            new Place { Id = 1012, CityId = 102, Name = "Kuşkayası Yol Anıtı", Description = "Roma döneminden kalan kaya oyma yol anıtı.", Category = "Anıt", EstimatedVisitDuration = "35 dk", EstimatedVisitMinutes = 35, Latitude = 41.7225, Longitude = 32.4036, IsOnRouteCandidate = false },
            new Place { Id = 1013, CityId = 102, Name = "Ulukaya Şelalesi", Description = "Küre Dağları hattında serin ve doğal bir şelale molası.", Category = "Doğa", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.5469, Longitude = 32.6190, IsOnRouteCandidate = false },
            new Place { Id = 1014, CityId = 102, Name = "Balamba Tabiat Parkı", Description = "Bartın merkezine yakın piknik ve doğa yürüyüşü alanı.", Category = "Park", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.6253, Longitude = 32.3560, IsOnRouteCandidate = false },
            new Place { Id = 1015, CityId = 103, Name = "Sümela Manastırı", Description = "Altındere Vadisi'nde kayalık yamaca kurulu ikonik manastır.", Category = "Manastır", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 40.6892, Longitude = 39.6586, IsOnRouteCandidate = false },
            new Place { Id = 1016, CityId = 103, Name = "Atatürk Köşkü", Description = "Soğuksu'da bahçesiyle birlikte gezilen tarihi köşk ve müze.", Category = "Müze", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 40.9899, Longitude = 39.6967, IsOnRouteCandidate = false },
            new Place { Id = 1017, CityId = 103, Name = "Ayasofya Camii", Description = "Bizans, Selçuklu ve Osmanlı izlerini taşıyan tarihi yapı.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.0034, Longitude = 39.6966, IsOnRouteCandidate = false },
            new Place { Id = 1018, CityId = 103, Name = "Trabzon Kalesi", Description = "Ortahisar hattında şehrin eski sur dokusunu gösteren kale.", Category = "Kale", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.0053, Longitude = 39.7182, IsOnRouteCandidate = false },
            new Place { Id = 1019, CityId = 103, Name = "Uzungöl", Description = "Göl, orman ve dağ manzarasıyla Trabzon'un en bilinen doğa durağı.", Category = "Doğa", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 40.6192, Longitude = 40.2886, IsOnRouteCandidate = false },
            new Place { Id = 1020, CityId = 103, Name = "Çal Mağarası", Description = "Düzköy'de içinden dere akan uzun yürüyüş rotalı mağara.", Category = "Mağara", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 40.8650, Longitude = 39.3790, IsOnRouteCandidate = false });
    }

    private static void SeedKnownLocationCoordinates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Province>().HasData(
            new Province { Id = 73, Name = "Şırnak", Latitude = 37.4187481, Longitude = 42.4918338 });

        modelBuilder.Entity<District>().HasData(
            new District { Id = 10909, ProvinceId = 73, Name = "İdil", Latitude = 37.33481, Longitude = 41.88944 });

        modelBuilder.Entity<Neighborhood>().HasData(
            new Neighborhood { Id = 149053, LocalityId = 23963, Name = "BEREKETLİ KÖYÜ", Latitude = 37.38419, Longitude = 41.819763 });
    }
}
