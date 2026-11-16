using System;

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // Rezervasyon bilgilerini temsil eden sınıf
    public class Rezervasyon
    {
        // Property'ler
        // get ve set blokları ile verilere kontrollü erişim sağlanır
        public int RezervasyonID { get; set; }
        public int KullaniciID { get; set; }
        public int UcusID { get; set; }
        public string YolcuAd { get; set; }
        public string YolcuSoyad { get; set; }
        public string YolcuTC { get; set; }
        public string KoltukNo { get; set; }
        public DateTime RezervasyonTarihi { get; set; }
        public string Durum { get; set; }
    }
}