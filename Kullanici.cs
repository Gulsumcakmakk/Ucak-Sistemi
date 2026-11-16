using System;

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // Kullanıcı bilgilerini temsil eden sınıf
    public class Kullanici
    {
        // Property'ler
        // get ve set blokları ile verilere kontrollü erişim sağlanır
        public int KullaniciID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Rol { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public bool AktifMi { get; set; }

        public string AdSoyad => $"{Ad} {Soyad}"; // Ad ve Soyad bilgilerini birleştirip "Ad Soyad" olarak string döndürür
    }
}