using System;
using System.Data;
using Microsoft.Data.SqlClient;

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // Tüm veritabanı işlemleri için yardımcı sınıf. Sadece veritabanı işlemlerinden sorumlu
    public class DatabaseHelper
    {
        // connectionString: SQL Server bağlantı bilgilerini içerir
        // Data Source: SQL Server localhost veya sunucu adı
        // Initial Catalog: Kullanılacak veritabanı adı
        // Integrated Security: Windows kimlik doğrulaması kullan (true)
        // TrustServerCertificate: Geliştirme ortamında SSL sertifikasını doğrulama (true)
        private string connectionString = @"Data Source=DESKTOP-2DO2TU5\SQLEXPRESS;Initial Catalog=UcakBiletDB;Integrated Security=True;TrustServerCertificate=True";

        // SORGU ÇALIŞTIRMA METODU: SELECT sorguları için - veri okuma işlemleri
        // query: Çalıştırılacak SQL sorgusu
        // parameters: SQL enjeksiyonunu önlemek için parametre listesi
        // GERİ DÖNÜŞ: DataTable - sorgu sonucundaki verileri içeren tablo
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            // Sorgu sonuçlarını tutacak DataTable nesnesi oluşturuldu
            DataTable dt = new DataTable();
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // PARAMETRE KONTROLÜ: Parametre varsa komuta ekle
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    
                    // DATA ADAPTER: Veritabanı ile DataTable arasında köprü görevi görür
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    
                    // BAĞLANTIYI AÇ: Veritabanına bağlan
                    conn.Open();
                    
                    // VERİYİ DOLDUR: Sorguyu çalıştır ve sonuçları DataTable'a doldur
                    adapter.Fill(dt);
                }
            }
            return dt; // Doldurulmuş DataTable'ı döndür
        }

        // NON-QUERY ÇALIŞTIRMA METODU: INSERT, UPDATE, DELETE sorguları için - veri değiştirme işlemleri
        // GERİ DÖNÜŞ: int - etkilenen satır sayısı
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // PARAMETRE KONTROLÜ: Parametre varsa komuta ekle
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    
                    // BAĞLANTIYI AÇ: Veritabanına bağlan
                    conn.Open();
                    
                    // SORGUVU ÇALIŞTIR: Etkilenen satır sayısını döndür
                    return cmd.ExecuteNonQuery();
                }
            } 
        }
    }
}