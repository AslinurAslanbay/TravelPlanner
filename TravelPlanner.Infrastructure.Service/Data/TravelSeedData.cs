using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Infrastructure.Service.Data;

internal static class TravelSeedData
{
    public static readonly IReadOnlyList<City> Cities =
    [
        new City
        {
            Id = 1,
            Slug = "istanbul",
            Name = "İstanbul",
            Region = "Marmara",
            Summary = "Tarihi yarımada, Boğaz ve semt duraklarıyla yoğun bir şehir rotası.",
            HeroTitle = "İstanbul'da seçeceğin durakları mantıklı sıraya koy",
            HeroDescription = "Başlangıç adresinden seçilen yerlere yakınlık ve süreye göre plan oluştur.",
            Latitude = 41.0082,
            Longitude = 28.9784,
            Places =
            [
                new Place { Id = 1, CityId = 1, Name = "Ayasofya Camii", Description = "İstanbul'un en ikonik tarihi yapılarından biri.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.0086, Longitude = 28.9802 },
                new Place { Id = 2, CityId = 1, Name = "Topkapı Sarayı", Description = "Osmanlı saray yaşamını keşfetmek için güçlü bir durak.", Category = "Saray", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 41.0115, Longitude = 28.9833 },
                new Place { Id = 3, CityId = 1, Name = "Galata Kulesi", Description = "Şehir siluetini izlemek için sevilen nokta.", Category = "Manzara", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.0256, Longitude = 28.9741 }
            ]
        },
        new City
        {
            Id = 2,
            Slug = "edirne",
            Name = "Edirne",
            Region = "Marmara",
            Summary = "Selimiye, tarihi çarşılar ve Meriç hattıyla seçtiğin gün sayısına göre planlanabilen rota.",
            HeroTitle = "Edirne için gün gün gezi planı hazırla",
            HeroDescription = "Yol üstü molalar ve şehir içi durakları aynı rota planında topla.",
            Latitude = 41.6771,
            Longitude = 26.5557,
            Places =
            [
                new Place { Id = 4, CityId = 2, Name = "Selimiye Camii", Description = "Mimar Sinan'ın ustalık eseri olarak bilinen merkez durak.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.6781, Longitude = 26.5594 },
                new Place { Id = 5, CityId = 2, Name = "Ali Paşa Çarşısı", Description = "Tarihi çarşı ve alışveriş durağı.", Category = "Çarşı", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.6756, Longitude = 26.5566 },
                new Place { Id = 6, CityId = 2, Name = "Meriç Köprüsü", Description = "Nehir kenarında yürüyüş ve fotoğraf molası.", Category = "Manzara", EstimatedVisitDuration = "30 dk", EstimatedVisitMinutes = 30, Latitude = 41.6687, Longitude = 26.5434 },
                new Place { Id = 7, CityId = 2, Name = "Silivri Sahil", Description = "İstanbul-Edirne yolunda kahve ve sahil molası.", Category = "Yol Üstü", EstimatedVisitDuration = "40 dk", EstimatedVisitMinutes = 40, Latitude = 41.0736, Longitude = 28.2464, IsOnRouteCandidate = true },
                new Place { Id = 8, CityId = 2, Name = "Çorlu Atalar Meydanı", Description = "Yol üstünde kısa yemek ve dinlenme durağı.", Category = "Yol Üstü", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.1591, Longitude = 27.8025, IsOnRouteCandidate = true },
                new Place { Id = 9, CityId = 2, Name = "Lüleburgaz Sokullu Mehmet Paşa Külliyesi", Description = "Edirne yolunda tarihi bir mola noktası.", Category = "Yol Üstü", EstimatedVisitDuration = "50 dk", EstimatedVisitMinutes = 50, Latitude = 41.4065, Longitude = 27.3549, IsOnRouteCandidate = true }
            ]
        },
        new City
        {
            Id = 3,
            Slug = "bartin",
            Name = "Bartın",
            Region = "Karadeniz",
            Summary = "Amasra kıyısı, lav sütunları ve doğa duraklarıyla kısa kaçamak rotası.",
            HeroTitle = "Bartın'da sahil ve doğa duraklarını planla",
            HeroDescription = "Amasra merkezinden Ulukaya Şelalesi'ne uzanan durakları gün sayına göre sırala.",
            Latitude = 41.5811,
            Longitude = 32.4610,
            Places =
            [
                new Place { Id = 10, CityId = 3, Name = "Amasra Kalesi", Description = "Amasra merkezinde deniz manzarasıyla öne çıkan tarihi kale.", Category = "Kale", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.7467, Longitude = 32.3864 },
                new Place { Id = 11, CityId = 3, Name = "Amasra Müzesi", Description = "Amasra ve çevresinden arkeolojik ve etnografik eserler sunan müze.", Category = "Müze", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.7462, Longitude = 32.3861 },
                new Place { Id = 12, CityId = 3, Name = "Güzelcehisar Lav Sütunları", Description = "Sahil boyunca uzanan lav kayalıkları ve gün batımı manzarası.", Category = "Doğa", EstimatedVisitDuration = "1 saat 15 dk", EstimatedVisitMinutes = 75, Latitude = 41.6433, Longitude = 32.1076 },
                new Place { Id = 13, CityId = 3, Name = "Kuşkayası Yol Anıtı", Description = "Roma döneminden kalan kaya oyma yol anıtı.", Category = "Anıt", EstimatedVisitDuration = "35 dk", EstimatedVisitMinutes = 35, Latitude = 41.7225, Longitude = 32.4036 },
                new Place { Id = 14, CityId = 3, Name = "Ulukaya Şelalesi", Description = "Küre Dağları hattında serin ve doğal bir şelale molası.", Category = "Doğa", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.5469, Longitude = 32.6190 },
                new Place { Id = 15, CityId = 3, Name = "Balamba Tabiat Parkı", Description = "Bartın merkezine yakın piknik ve doğa yürüyüşü alanı.", Category = "Park", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.6253, Longitude = 32.3560 }
            ]
        },
        new City
        {
            Id = 4,
            Slug = "trabzon",
            Name = "Trabzon",
            Region = "Karadeniz",
            Summary = "Sümela, Uzungöl, tarihi yapılar ve yayla hissiyle yoğun Karadeniz rotası.",
            HeroTitle = "Trabzon için doğa ve tarih rotası oluştur",
            HeroDescription = "Şehir merkezi, Maçka ve Uzungöl hattındaki durakları mantıklı sıraya koy.",
            Latitude = 41.0027,
            Longitude = 39.7168,
            Places =
            [
                new Place { Id = 16, CityId = 4, Name = "Sümela Manastırı", Description = "Altındere Vadisi'nde kayalık yamaca kurulu ikonik manastır.", Category = "Manastır", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 40.6892, Longitude = 39.6586 },
                new Place { Id = 17, CityId = 4, Name = "Atatürk Köşkü", Description = "Soğuksu'da bahçesiyle birlikte gezilen tarihi köşk ve müze.", Category = "Müze", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 40.9899, Longitude = 39.6967 },
                new Place { Id = 18, CityId = 4, Name = "Ayasofya Camii", Description = "Bizans, Selçuklu ve Osmanlı izlerini taşıyan tarihi yapı.", Category = "Tarihi Yer", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 41.0034, Longitude = 39.6966 },
                new Place { Id = 19, CityId = 4, Name = "Trabzon Kalesi", Description = "Ortahisar hattında şehrin eski sur dokusunu gösteren kale.", Category = "Kale", EstimatedVisitDuration = "45 dk", EstimatedVisitMinutes = 45, Latitude = 41.0053, Longitude = 39.7182 },
                new Place { Id = 20, CityId = 4, Name = "Uzungöl", Description = "Göl, orman ve dağ manzarasıyla Trabzon'un en bilinen doğa durağı.", Category = "Doğa", EstimatedVisitDuration = "2 saat", EstimatedVisitMinutes = 120, Latitude = 40.6192, Longitude = 40.2886 },
                new Place { Id = 21, CityId = 4, Name = "Çal Mağarası", Description = "Düzköy'de içinden dere akan uzun yürüyüş rotalı mağara.", Category = "Mağara", EstimatedVisitDuration = "1 saat", EstimatedVisitMinutes = 60, Latitude = 40.8650, Longitude = 39.3790 }
            ]
        }
    ];
}
