# SalesManagement - Satış Yönetim Sistemi

Modern ve ölçeklenebilir bir Satış Yönetim Sistemi ASP.NET Core 7.0 MVC mimarisi ile geliştirilmiştir.

## 🚀 Proje Hakkında

Bu proje, işletmelerin satış süreçlerini dijital ortamda yönetmelerini sağlayan kapsamlı bir ERP çözümüdür. Kurumsal düzeyde geliştirilen bu sistem, modern yazılım geliştirme pratiklerini ve mimari kalıpları takip etmektedir.

## 🏗️ Mimari Yapı

Proje, **Clean Architecture** prensiplerine uygun olarak katmanlı mimari ile tasarlanmıştır:

```
BaSalesManagementApp/
├── BaSalesManagementApp.Core/          # Çekirdek katman - Entity'ler, Enums, Interfaces
├── BaSalesManagementApp.Business/       # İş mantığı katmanı - Services, Validations
├── BaSalesManagementApp.DataAccess/     # Veri erişim katmanı - Repositories, Context
│   ├── BaSalesManagementApp.DataAccess.EFCore/
│   └── BaSalesManagementApp.DataAccess.Interfaces/
├── BaSalesManagementApp.Dtos/          # Data Transfer Objects
├── BaSalesManagementApp.Configurations/# Entity Configurations
├── BaSalesManagementApp.BackgroundJobs/# Arka plan işleri (Hangfire)
└── BaSalesManagementApp.MVC/           # Web UI Katmanı - Controllers, Views
```

## 🛠️ Kullanılan Teknolojiler

| Kategori | Teknoloji |
|----------|-----------|
| Framework | ASP.NET Core 7.0 MVC |
| ORM | Entity Framework Core 7.0 |
| Database | SQL Server |
| Validation | FluentValidation |
| Mapping | Mapster |
| Notifications | AspNetCoreHero.ToastNotification |
| Pagination | X.PagedList |
| Background Jobs | Hangfire |

## 📦 Proje Özellikleri

### Ana Modüller
- **Yönetici Yönetimi** - Admin kullanıcı işlemleri
- **Şirket Yönetimi** - Çoklu şirket desteği
- **Şube Yönetimi** - Şube bazlı operasyonlar
- **Çalışan Yönetimi** - Personel takibi
- **Müşteri Yönetimi** - CRM işlemleri
- **Ürün Yönetimi** - Katalog ve envanter
- **Sipariş Yönetimi** - Satış süreçleri
- **Depo Yönetimi** - Stok kontrolü
- **Promosyon Yönetimi** - Kampanya ve indirimler

### Teknik Özellikler
- ✅ Modern Clean Architecture
- ✅ Dependency Injection
- ✅ Repository Pattern
- ✅ Unit of Work
- ✅ Async/Await Programming
- ✅ Localization Desteği
- ✅ Responsive UI
- ✅ Grid Pagination
- ✅ QR Kod Üretimi
- ✅ Email Servisleri

## 📋 Gereksinimler

- .NET 7.0 SDK veya üzeri
- SQL Server 2019 veya üzeri
- Visual Studio 2022 veya VS Code

## 🚀 Kurulum

1. **Repoyu klonlayın:**
```bash
git clone https://github.com/alitumer0/SalesManagement.git
```

2. **Bağımlılıkları yükleyin:**
```bash
dotnet restore
```

3. **Veritabanı migrasyonlarını uygulayın:**
```bash
cd BaSalesManagementApp.MVC
dotnet ef database update
```

4. **Uygulamayı çalıştırın:**
```bash
dotnet run
```

## 📁 Proje Yapayıısı Det

### Core Katman
Temel entity sınıfları, arayüzler ve enum'lar bu katmanda yer alır:
- `Entities/Base/` - Temel sınıflar (AuditableEntity, BaseEntity, BaseUser)
- `Enums/` - CurrencyType, Roles, Status
- `DataAccess/Interfaces/` - Repository arayüzleri

### Business Katman
İş kuralları ve servisler bu katmanda yer alır:
- `Services/` - AccountService, OrderService, ProductService, vb.
- `Interfaces/` - Servis arayüzleri
- `Constants/` - Mesajlar ve sabitler

### DataAccess Katman
Veri erişim operasyonları bu katmanda gerçekleştirilir:
- `Repositories/` - Entity Framework implementasyonları
- `Context/` - DbContext ve configuration'lar

### MVC Katman
Kullanıcı arayüzü bileşenleri:
- `Controllers/` - MVC Controller'ları
- `Views/` - Razor Views
- `Models/` - View Models
- `Areas/` - Admin, Sales, Warehouse area'ları

## 🔧 Yapılandırma

Uygulama ayarları `BaSalesManagementApp.MVC/appsettings.json` dosyasında yapılandırılmıştır:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_SQL_Server_Connection_String"
  },
  "MailSettings": {
    "Mail": "your-email@domain.com",
    "Host": "smtp.example.com",
    "Port": 587
  }
}
```

## 📊 Veritabanı Şeması

Ana tablolar:
- Admins
- Companies
- Branches
- Employees
- Customers
- Products
- Categories
- Orders
- OrderDetails
- Stocks
- Warehouses
- Promotions
- Payments

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit yapın (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature'`)
5. Pull Request açın

## 📝 Lisans

Bu proje MIT Lisansı altında lisanslanmıştır.

## 📞 İletişim

Proje Sahibi - [GitHub Profiliniz]

Proje Linki: [https://github.com/alitumer0/SalesManagement](https://github.com/alitumer0/SalesManagement)
