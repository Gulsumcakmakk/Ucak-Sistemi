using System;

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // Uçuş bilgilerini temsil eden sınıf
    public class Ucus
    {
        // Property'ler
        // get ve set blokları ile verilere kontrollü erişim sağlanır
        public int UcusID { get; set; }
        public int UcakID { get; set; }
        public string UcusKodu { get; set; }
        public string Nereden { get; set; }
        public string Nereye { get; set; }
        public DateTime KalkisTarihi { get; set; }
        public DateTime VarisTarihi { get; set; }
        public decimal Fiyat { get; set; }
        public int MusaitKoltuk { get; set; }
        public bool AktifMi { get; set; } 
        public string UcakModel { get; set; }
        // Sadece read-only property set blokları yok
        public string KalkisTarihiStr => KalkisTarihi.ToString("gg.aa.yyyy ss:dd"); // gün ay yıl | saat dakika olarak zamanı alır
        public string VarisTarihiStr => VarisTarihi.ToString("gg.aa.yyyy ss:dd"); // Dinamiktir, her çağrıldığında güncel tarihi kullanır
        public string FiyatStr => Fiyat.ToString("N2") + " TL"; // Fiyatı 2 ondalık basamağa dönüştürür
    }
}