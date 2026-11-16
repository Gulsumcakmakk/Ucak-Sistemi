using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Ucak
{
    public class FiyatHesaplayici
    {
        private DatabaseHelper db; // Veritabanı işlemleri için yardımcı sınıf

        public FiyatHesaplayici()
        {
            db = new DatabaseHelper(); // DatabaseHelper nesnesi oluşturuluyor
        }

        // ANA FİYAT HESAPLAMA METODU - Kupon koduyla birlikte
        public decimal FiyatHesapla(int ucusID, string koltukNo, string kuponKodu = "")
        {
            try
            {
                // 1. Temel fiyatı veritabanından al
                decimal temelFiyat = TemelFiyatGetir(ucusID);
                if (temelFiyat <= 0) return 0; // Geçersiz fiyat durumu

                decimal sonucFiyat = temelFiyat; // Hesaplanacak son fiyat

                // 2. DOLULUK ORANINA GÖRE ÇARPAN uygula (uçak ne kadar dolu?)
                sonucFiyat *= DolulukHesapla(ucusID);

                // 3. KALAN GÜNE GÖRE ÇARPAN uygula (uçuşa ne kadar kaldı?)
                sonucFiyat *= KalanGunHesapla(ucusID);

                // 4. SEZON ÇARPANI uygula (yüksek/düşük/normal sezon)
                sonucFiyat *= SezonHesapla(ucusID);

                // 5. KOLTUK TİPİ ÇARPANI uygula (business/economy, pencere/koridor)
                sonucFiyat *= KoltukTipiHesapla(koltukNo);

                // 6. KUPON KODU İNDİRİMİ uygula (varsa)
                if (!string.IsNullOrEmpty(kuponKodu))
                {
                    decimal indirim = KuponKoduKontrol(kuponKodu);
                    if (indirim > 0)
                        sonucFiyat *= (1 - indirim); // % indirim uygula
                }

                return Math.Round(sonucFiyat, 2); // İki ondalık basamağa yuvarla
            }
            catch
            {
                // Hata durumunda sadece temel fiyatı döndür
                return TemelFiyatGetir(ucusID);
            }
        }

        // TEMEL FİYAT ALMA - Veritabanından uçuşun temel fiyatını getirir
        private decimal TemelFiyatGetir(int ucusID)
        {
            try
            {
                string query = "SELECT Fiyat FROM Ucuslar WHERE UcusID = @UcusID";
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                DataTable dt = db.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                    return Convert.ToDecimal(dt.Rows[0]["Fiyat"]);
            }
            catch { }
            return 0; // Hata durumunda 0 döndür
        }

        // DOLULUK HESAPLAMA - Uçağın doluluk oranına göre çarpan belirler
        private decimal DolulukHesapla(int ucusID)
        {
            try
            {
                string query = @"SELECT uc.KoltukSayisi, u.MusaitKoltuk 
                               FROM Ucuslar u 
                               INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID 
                               WHERE u.UcusID = @UcusID";
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    int toplam = Convert.ToInt32(dt.Rows[0]["KoltukSayisi"]);
                    int musait = Convert.ToInt32(dt.Rows[0]["MusaitKoltuk"]);
                    decimal dolulukYuzde = ((toplam - musait) * 100.0m) / toplam;

                    // Doluluk oranına göre fiyat artışı uygula
                    if (dolulukYuzde >= 80) return 1.30m; // %80+ dolu: %30 artış
                    if (dolulukYuzde >= 60) return 1.15m; // %60-79 dolu: %15 artış
                    if (dolulukYuzde >= 40) return 1.05m; // %40-59 dolu: %5 artış
                }
            }
            catch { }
            return 1.0m; // Varsayılan: değişiklik yok
        }

        // KALAN GÜN HESAPLAMA - Uçuş tarihine kalan güne göre çarpan belirler
        private decimal KalanGunHesapla(int ucusID)
        {
            try
            {
                string query = "SELECT KalkisTarihi FROM Ucuslar WHERE UcusID = @UcusID";
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    DateTime kalkis = Convert.ToDateTime(dt.Rows[0]["KalkisTarihi"]);
                    int kalanGun = (kalkis.Date - DateTime.Now.Date).Days;

                    // Kalan güne göre fiyatlandırma
                    if (kalanGun <= 3) return 1.40m;  // 3 günden az: %40 artış
                    if (kalanGun <= 7) return 1.20m;  // 4-7 gün: %20 artış
                    if (kalanGun <= 14) return 1.10m; // 8-14 gün: %10 artış
                    if (kalanGun >= 30) return 0.85m; // 30+ gün: %15 indirim (erken rezervasyon)
                }
            }
            catch { }
            return 1.0m; // Varsayılan: değişiklik yok
        }

        // SEZON HESAPLAMA - Uçuşun sezon tipine göre çarpan belirler
        private decimal SezonHesapla(int ucusID)
        {
            try
            {
                string query = "SELECT SezonTipi FROM Ucuslar WHERE UcusID = @UcusID";
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                DataTable dt = db.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    int sezonTipi = Convert.ToInt32(dt.Rows[0]["SezonTipi"]);
                    
                    if (sezonTipi == 1) return 1.20m; // Yüksek sezon: %20 artış
                    if (sezonTipi == 2) return 0.90m; // Düşük sezon: %10 indirim
                }
            }
            catch { }
            return 1.0m; // Normal sezon: değişiklik yok
        }

        // KOLTUK TİPİ HESAPLAMA - Koltuk numarasına göre sınıf belirler
        private decimal KoltukTipiHesapla(string koltukNo)
        {
            try
            {
                if (string.IsNullOrEmpty(koltukNo) || koltukNo.Length < 2)
                    return 1.0m;

                char koltukHarf = koltukNo[koltukNo.Length - 1]; // Son karakter harf (örn: "A1" için 'A')

                // BUSINESS CLASS (pencere A-F) %25 ekstra
                if (koltukHarf == 'A' || koltukHarf == 'F')
                    return 1.25m; // Pencere kenarı

                // BUSINESS CLASS (koridor C-D) %15 ekstra
                if (koltukHarf == 'C' || koltukHarf == 'D')
                    return 1.15m; // Koridor kenarı

                // ECONOMY CLASS (B-E) - Standart fiyat
                return 1.0m;
            }
            catch { }
            return 1.0m; // Hata durumu
        }

        // KUPON KODU KONTROL - Girilen kupon kodunun geçerliliğini kontrol eder
        private decimal KuponKoduKontrol(string kod)
        {
            try
            {
                kod = kod.Trim().ToUpper(); // Formatla

                // Sabit kupon kodları (veritabanı yerine hardcoded)
                if (kod == "YENIYIL2026") return 0.15m; // %15 indirim
                if (kod == "ILKUCUS") return 0.20m;     // %20 indirim
                if (kod == "YAZ2026") return 0.10m;      // %10 indirim
            }
            catch { }
            return 0; // Geçersiz kupon: indirim yok
        }

        // FİYAT DETAY GETİR - Fiyat hesaplama detaylarını metin olarak döndürür
        public string FiyatDetayGetir(int ucusID, string koltukNo, string kuponKodu = "")
        {
            try
            {
                decimal temelFiyat = TemelFiyatGetir(ucusID);
                decimal sonucFiyat = temelFiyat;

                string detay = "FİYAT DETAYI\n\n";  // Fiyat detayı başlığı
                detay += $"Temel Fiyat: {temelFiyat:N2} TL\n";  // Temel fiyatı göster

                decimal dolulukCarpan = DolulukHesapla(ucusID);  // Doluluk çarpanını hesapla
                if (dolulukCarpan != 1.0m)  // Doluluk farklıysa (1.0 değilse) göster
                {
                    detay += $"Doluluk Ayarı: x{dolulukCarpan:N2}\n";
                    sonucFiyat *= dolulukCarpan;  // Fiyata doluluk çarpanını uygula
                }

                decimal gunCarpan = KalanGunHesapla(ucusID);  // Kalan gün çarpanını hesapla
                if (gunCarpan != 1.0m)  // Tarih çarpanı farklıysa göster
                {
                    detay += $"Tarih Ayarı: x{gunCarpan:N2}\n";
                    sonucFiyat *= gunCarpan;  // Fiyata tarih çarpanını uygula
                }

                decimal sezonCarpan = SezonHesapla(ucusID);  // Sezon çarpanını hesapla
                if (sezonCarpan != 1.0m)  // Sezon çarpanı farklıysa göster
                {
                    detay += $"Sezon Ayarı: x{sezonCarpan:N2}\n";
                    sonucFiyat *= sezonCarpan;  // Fiyata sezon çarpanını uygula
                }

                decimal koltukCarpan = KoltukTipiHesapla(koltukNo);  // Koltuk tipi çarpanını hesapla
                if (koltukCarpan != 1.0m)  // Koltuk çarpanı farklıysa göster
                {
                    string koltukTipi = "";
                    if (koltukCarpan == 1.25m) koltukTipi = "Business Pencere";  // %25 ekstra
                    else if (koltukCarpan == 1.15m) koltukTipi = "Business Koridor";  // %15 ekstra
                    
                    detay += $"Koltuk Sınıfı ({koltukTipi}): x{koltukCarpan:N2}\n";
                    sonucFiyat *= koltukCarpan;  // Fiyata koltuk çarpanını uygula
                }

                if (!string.IsNullOrEmpty(kuponKodu))  // Kupon kodu varsa
                {
                    decimal indirim = KuponKoduKontrol(kuponKodu);  // Kupon indirimini kontrol et
                    if (indirim > 0)  // Geçerli indirim varsa
                    {
                        detay += $"Kupon İndirimi ({kuponKodu}): -%{indirim * 100:N0}\n";  // İndirim yüzdesini göster
                        sonucFiyat *= (1 - indirim);  // Fiyata indirimi uygula
                    }
                    else  // Geçersiz kupon
                    {
                        detay += $"Kupon Geçersiz: {kuponKodu}\n";  // Geçersiz kupon uyarısı
                    }
                }

                sonucFiyat = Math.Round(sonucFiyat, 2);  // Fiyatı 2 ondalık basamağa yuvarla
                detay += $"TOPLAM: {sonucFiyat:N2} TL";  // Toplam fiyatı göster

                decimal fark = sonucFiyat - temelFiyat;  // Temel fiyatla farkı hesapla
                if (fark > 0)  // Fiyat artmışsa
                    detay += $"\n(+{fark:N2} TL artış)";  // Artış miktarını göster
                else if (fark < 0)  // Fiyat azalmışsa
                    detay += $"\n({Math.Abs(fark):N2} TL tasarruf)";  // Tasarruf miktarını göster

                return detay;  // Detaylı fiyat bilgisini döndür
            }
            catch (Exception ex)
            {
                return "Fiyat hesaplanamadı: " + ex.Message;  // Hata durumunda mesaj döndür
            }
        }
    }
} 
            