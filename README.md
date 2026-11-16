# ✈️ Uçak Bileti Rezervasyon Sistemi

C# ve Windows Forms ile geliştirilmiş, SQL Server tabanlı masaüstü uçak bileti rezervasyon uygulaması.

## Özellikler

### Müşteri
- Kayıt olma ve giriş yapma
- Uçuşları listeleme ve arama
- Yolcu bilgisi girerek rezervasyon yapma
- Görsel koltuk haritasından koltuk seçimi (Business: pencere/koridor, Economy)
- Fiyatı onaylamadan önce detaylı görme
- Rezervasyonları görüntüleme ve iptal etme (iptalde boş koltuk sayısı otomatik artar)

### Admin
- Kullanıcı yönetimi (ekle / sil)
- Uçak yönetimi (ekle / sil)
- Uçuş yönetimi (ekle / sil)

### Dinamik fiyatlama
Fiyat sınıfı (`FiyatHesaplayici.cs`) şu etkenlere göre hesaplanır:
- Uçuşun doluluk oranı
- Uçuşa kalan gün
- Sezon
- Koltuk tipi
- Kupon kodu

### Veri bütünlüğü
- Parametreli SQL sorguları (SQL injection'a karşı)
- Aynı koltuğun tekrar rezerve edilmesinin engellenmesi
- Kontenjan (müsait koltuk) kontrolü

## Teknolojiler
- C#, .NET 8, Windows Forms
- SQL Server (Express) – `Microsoft.Data.SqlClient`

## Proje yapısı
| Dosya | Açıklama |
|---|---|
| `Program.cs` | Giriş noktası |
| `DatabaseHelper.cs` | Veritabanı bağlantısı ve sorgu yardımcıları |
| `GirisForm.cs`, `KayitForm.cs` | Giriş ve kayıt ekranları |
| `MusteriPanel.cs` | Müşteri paneli (uçuş arama, rezervasyonlar) |
| `AdminPanel.cs` | Admin paneli (kullanıcı, uçak, uçuş yönetimi) |
| `RezervasyonForm.cs`, `KoltukSecimForm.cs` | Rezervasyon ve koltuk seçimi |
| `FiyatHesaplayici.cs` | Dinamik fiyat hesaplama |
| `Kullanici.cs`, `Ucak.cs`, `Ucus.cs`, `Rezervasyon.cs` | Model sınıfları |

## Kurulum
1. [.NET 8 SDK](https://dotnet.microsoft.com/download) ve SQL Server Express kurun.
2. `UcakBiletDB` adlı bir veritabanı oluşturun. Uygulama şu tablolara ihtiyaç duyar: `Kullanicilar`, `Ucaklar`, `Ucuslar`, `Rezervasyonlar`.
3. `DatabaseHelper.cs` içindeki bağlantı dizesini kendi sunucu adınıza göre düzenleyin:
   ```csharp
   Data Source=SUNUCU_ADI\SQLEXPRESS;Initial Catalog=UcakBiletDB;Integrated Security=True;TrustServerCertificate=True
   ```
4. Çalıştırın:
   ```bash
   dotnet run
   ```

## Yazar
Gülsüm Çakmak – [GitHub](https://github.com/Gulsumcakmakk)
