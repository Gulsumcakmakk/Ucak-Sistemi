using System;

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // Uçak bilgilerini temsil eden sınıf
    public class Ucak
    {
        // Property'ler 
        // get ve set blokları ile verilere kontrollü erişim sağlanır
        public int UcakID { get; set; }
        public string UcakKodu { get; set; }
        public string Model { get; set; }
        public int KoltukSayisi { get; set; }
        public bool AktifMi { get; set; }
    }
}