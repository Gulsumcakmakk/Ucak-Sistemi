using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// KAYIT FORM SINIFI - BU SINIF YENİ KULLANICI KAYIT İŞLEMLERİNİ YÖNETİR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class KayitForm : Form
    {
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db;

        public KayitForm()  // Constructordır. Nesne oluşturulduğunda otomatik çalışan yapıcı metoddur.
        {
            InitializeComponent();  // vs code tarafından otomatik oluşturulur, formdaki tüm kontrolleri hazırlar.      
            /* Composition (Bileşim) oop özelliği kullanıldı. Başka bir sınıftan örnek alıyor.
               veritabanı bağlantısı için DatabaseHelper sınıfından yeni bir nesne oluşturuluyor ve db alanına atanıyor. */
            db = new DatabaseHelper();
        }

        // Arayüz için gereken tüm form kontrollerini (buton, textbox, label) oluşturulduğu ve tasarımının yapıldığı kısım
        private void InitializeComponent()
        {
            // new ile her kontrol için yeni nesne oluşturuluyor
            // Her kontrol aslında bir sınıf örneğidir (instance) ile o sınıftan somut bir yapı oluşturulur
            this.txtAd = new System.Windows.Forms.TextBox();   // Ad giriş kutusu
            this.txtSoyad = new System.Windows.Forms.TextBox();   // Soyad giriş kutusu
            this.txtEmail = new System.Windows.Forms.TextBox();   // Email giriş kutusu
            this.txtSifre = new System.Windows.Forms.TextBox();   // Şifre giriş kutusu
            this.txtSifreTekrar = new System.Windows.Forms.TextBox();   // Şifre tekrar kutusu
            this.btnKayit = new System.Windows.Forms.Button();    // Kayıt butonu
            this.btnIptal = new System.Windows.Forms.Button();    // İptal butonu
            this.label1 = new System.Windows.Forms.Label();   // Ad etiketi
            this.label2 = new System.Windows.Forms.Label();   // Soyad etiket
            this.label3 = new System.Windows.Forms.Label();   // Email etiket
            this.label4 = new System.Windows.Forms.Label();   // Şifre etiket
            this.label5 = new System.Windows.Forms.Label();    // Şifre tekrar  etiket
            this.label6 = new System.Windows.Forms.Label();   // Başlık etiketi
            // Form yerleşim ayarlarını başlat. Windows Forms ile
            this.SuspendLayout();
            
            // BAŞLIK ETİKETİ (label6) - FORMUN ÜSTÜNDE "KULLANICI KAYDI" YAZISI
            this.label6.AutoSize = true; // Etiketin boyutu içeriğine göre otomatik ayarlanır
            this.label6.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold); //  Yazı tipi, boyutu ve stili
            this.label6.Location = new System.Drawing.Point(200, 20); //  Form içindeki konumu - X=200, Y=20 (ortalı pozisyon)
            this.label6.Name = "label6";   // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label6.Size = new System.Drawing.Size(150, 25); //  Kontrolün genişlik ve yüksekliği
            this.label6.TabIndex = 12;// Tab tuşu ile gezinme sırası - 12. sırada
            this.label6.Text = "Kullanıcı Kaydı";  // Etiket üzerinde görünecek metin içeriği
            
            // AD ETİKETİ (label1) - "Ad:" YAZISINI GÖSTEREN ETİKET
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 70);
            this.label1.Name = "label1";   // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label1.Size = new System.Drawing.Size(22, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ad:";  // Etiket üzerinde görünecek metin içeriği
            
            // AD GİRİŞ KUTUSU (txtAd) - KULLANICININ ADINI GİRECEĞİ TEXTBOX
            this.txtAd.Location = new System.Drawing.Point(180, 67); // Etiketin yanında
            this.txtAd.Name = "txtAd";
            this.txtAd.Size = new System.Drawing.Size(280, 23);
            this.txtAd.TabIndex = 1;
            
            // SOYAD ETİKETİ (label2) - "Soyad:" YAZISINI GÖSTEREN ETİKET
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 105); // Ad'ın altında
            this.label2.Name = "label2";  // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Soyad:";  // Etiket üzerinde görünecek metin içeriği
            
            // SOYAD GİRİŞ KUTUSU (txtSoyad) - KULLANICININ SOYADINI GİRECEĞİ TEXTBOX
            this.txtSoyad.Location = new System.Drawing.Point(180, 102); 
            this.txtSoyad.Name = "txtSoyad";
            this.txtSoyad.Size = new System.Drawing.Size(280, 23);
            this.txtSoyad.TabIndex = 3;
            
            // EMAIL ETİKETİ (label3) - "Email:" YAZISINI GÖSTEREN ETİKET
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(80, 140);
            this.label3.Name = "label3";   // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label3.Size = new System.Drawing.Size(39, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Email:";  // Etiket üzerinde görünecek metin içeriği
            
            // EMAIL GİRİŞ KUTUSU (txtEmail) - KULLANICININ EMAIL ADRESİNİ GİRECEĞİ TEXTBOX
            this.txtEmail.Location = new System.Drawing.Point(180, 137);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(280, 23);
            this.txtEmail.TabIndex = 5;
            
            // ŞİFRE ETİKETİ (label4) - "Şifre:" YAZISINI GÖSTEREN ETİKET
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(80, 175);
            this.label4.Name = "label4";    // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label4.Size = new System.Drawing.Size(33, 15);
            this.label4.TabIndex = 6;
            this.label4.Text = "Şifre:";  // Etiket üzerinde görünecek metin içeriği
            
            // ŞİFRE GİRİŞ KUTUSU (txtSifre) - KULLANICININ ŞİFRESİNİ GİRECEĞİ ALAN
            this.txtSifre.Location = new System.Drawing.Point(180, 172);
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.PasswordChar = '*';   // PasswordChar: Gizlilik için yazılan her karakter '*' olarak gösterilir Bu özellik şifre girişlerinde güvenlik sağlar, başkalarının şifreyi görmesini engeller
            this.txtSifre.Size = new System.Drawing.Size(280, 23);
            this.txtSifre.TabIndex = 7;
            
            // ŞİFRE TEKRAR ETİKETİ (label5) - "Şifre Tekrar:" YAZISINI GÖSTEREN ETİKET
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(80, 210);
            this.label5.Name = "label5";   // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label5.Size = new System.Drawing.Size(73, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "Şifre Tekrar:";  // Etiket üzerinde görünecek metin içeriği
            
            // ŞİFRE TEKRAR GİRİŞ KUTUSU (txtSifreTekrar) - ŞİFRENİN DOĞRULANMASI İÇİN
            this.txtSifreTekrar.Location = new System.Drawing.Point(180, 207);
            this.txtSifreTekrar.Name = "txtSifreTekrar";
            this.txtSifreTekrar.PasswordChar = '*';   // PasswordChar: Bu kutuda da şifre gizlenir
            this.txtSifreTekrar.Size = new System.Drawing.Size(280, 23);
            this.txtSifreTekrar.TabIndex = 9;
            
            // KAYIT BUTONU (btnKayit) - KAYIT İŞLEMİNİ BAŞLATAN BUTON
            this.btnKayit.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnKayit.ForeColor = System.Drawing.Color.White;
            this.btnKayit.Location = new System.Drawing.Point(180, 250);
            this.btnKayit.Name = "btnKayit";
            this.btnKayit.Size = new System.Drawing.Size(130, 35);
            this.btnKayit.TabIndex = 10;
            this.btnKayit.Text = "Kayıt Ol"; // Buton üzerindeki yazı
            this.btnKayit.UseVisualStyleBackColor = false;
            // Event handling (olay işleme) oop prensibi kullanıldı
            // Butonun Click olayına BtnGiris_Click metodunu bağlıyoruz
            this.btnKayit.Click += new System.EventHandler(this.BtnKayit_Click);
            
            // İPTAL BUTONU (btnIptal) - KAYIT İŞLEMİNİ İPTAL EDEN BUTON
            this.btnIptal.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnIptal.ForeColor = System.Drawing.Color.White;
            this.btnIptal.Location = new System.Drawing.Point(330, 250);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(130, 35);
            this.btnIptal.TabIndex = 11;
            this.btnIptal.Text = "İptal";  // Buton üzerindeki yazı
            this.btnIptal.UseVisualStyleBackColor = false;
            // Event handling (olay işleme) oop prensibi kullanıldı
            // Click event'ine BtnIptal_Click metodunu bağla
            this.btnIptal.Click += new System.EventHandler(this.BtnIptal_Click);

            // ANA FORM ÖZELLİKLERİ - KAYIT FORMUNUN KENDİSİNE AİT AYARLAR
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 320);
            // KONTROLLERİN FORMA EKLENMESİ - TÜM BİLEŞENLER FORM ÜZERİNDE GÖSTERİLİYOR
            // Controls.Add(): Her kontrolü forma ekliyoruz 
            this.Controls.Add(this.label6);   // Başlık (en üstte)
            this.Controls.Add(this.btnIptal);   // İptal butonu
            this.Controls.Add(this.btnKayit);   // Kayıt butonu
            this.Controls.Add(this.txtSifreTekrar);  // Şifre tekrar kutusu
            this.Controls.Add(this.label5);  // Şifre tekrar etiketi
            this.Controls.Add(this.txtSifre);  // Şifre kutusu
            this.Controls.Add(this.label4);  // Şifre etiketi
            this.Controls.Add(this.txtEmail);  // Email kutusu
            this.Controls.Add(this.label3);  // Email etiketi
            this.Controls.Add(this.txtSoyad);  // Soyad kutusu
            this.Controls.Add(this.label2);  // Soyad etiketi
            this.Controls.Add(this.txtAd);  // Ad kutusu
            this.Controls.Add(this.label1);  // Ad etiketi
            this.Name = "KayitForm"; // Formun programatik adı
            // StartPosition: Formun açılış pozisyonu ekranın ortasında
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Yeni Kullanıcı Kaydı"; // Formun başlık çubuğunda görünecek yazı
            // Form yerleşim ayarlarını tamamla ve kontrolleri görünür yap
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Encapsulation oop yapısı kullanılmıştır
        // Encapsulation ile kontrollere sadece bu sınıfın içindeki metodlardan erişilebilir
        // Tüm kontrol değişkenleri private olarak tanımlandı
        private System.Windows.Forms.TextBox txtAd;  // Ad giriş kutusu referansı
        private System.Windows.Forms.TextBox txtSoyad; // Soyad giriş kutusu referansı
        private System.Windows.Forms.TextBox txtEmail;  // Email giriş kutusu referansı
        private System.Windows.Forms.TextBox txtSifre;  // Şifre giriş kutusu referansı
        private System.Windows.Forms.TextBox txtSifreTekrar;  // Şifre tekrar kutusu referansı
        private System.Windows.Forms.Button btnKayit;  // Kayıt butonu referansı
        private System.Windows.Forms.Button btnIptal;  // İptal butonu referansı
        private System.Windows.Forms.Label label1; // Ad etiketi referansı
        private System.Windows.Forms.Label label2;  // Soyad etiketi referansı
        private System.Windows.Forms.Label label3;   // Email etiketi referansı
        private System.Windows.Forms.Label label4;   // Şifre etiketi referansı
        private System.Windows.Forms.Label label5;  // Şifre tekrar etiketi referansı
        private System.Windows.Forms.Label label6;  // Başlık etiketi referansı

        // KAYIT BUTONU TIKLAMA OLAY METODU - KULLANICI KAYIT İŞLEMLERİNİ YÖNETİR
        // Event hnadling oop yapısı. Buton tıklandığında otomatik çağrılan metod
        private void BtnKayit_Click(object sender, EventArgs e)
        {
            // Exception handling oop yapısı ile hata yönetimi yapılıyor
            // try-catch bloğu, Beklenmeyen hataları yakalar ve uygulamanın çökmesini engeller
            try
            {
                // Giriş bilgileri boş mu diye kontrol, string.IsNullOrWhiteSpace(): String'in null, boş veya sadece boşluk karakterlerinden oluşup oluşmadığını kontrol eder
                if (string.IsNullOrWhiteSpace(txtAd.Text) || 
                    string.IsNullOrWhiteSpace(txtSoyad.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) || 
                    string.IsNullOrWhiteSpace(txtSifre.Text) ||
                    string.IsNullOrWhiteSpace(txtSifreTekrar.Text))
                {
                    // Bilgiler de eksik varsa kullanıcıya uyarı mesajı 
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Şifreler eşlelşiyor mu diye kontrol. İki şifre kutusundaki değerlerin aynı olup olmadığının kontrolü
                if (txtSifre.Text != txtSifreTekrar.Text)
                {
                    // Kullanıcıya uyarı mesajı 
                    MessageBox.Show("Şifreler eşleşmiyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Şifrenin minimum uzunluk kontrolünü yapar
                if (txtSifre.Text.Length < 6)
                {
                    // Kullanıcıya uyarı mesajı 
                    MessageBox.Show("Şifre en az 6 karakter olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // EMAIL KONTROLÜ - VERİTABANINDA AYNI EMAİL VAR MI
                // SQL Sorgusu, Aynı email adresiyle kayıtlı kullanıcı sayısını bul
                string kontrolQuery = "SELECT COUNT(*) FROM Kullanicilar WHERE Email = @Email";
                // Sql parametresinin oluşturulması ve Kullanıcı girdilerini güvenli şekilde sorguya eklenir
                SqlParameter[] kontrolParams = { new SqlParameter("@Email", txtEmail.Text.Trim()) };
                // Veritabanı sorgusu çalıştırılıp sonuç alınır
                DataTable dtKontrol = db.ExecuteQuery(kontrolQuery, kontrolParams);
                
                // Sorgu sonucunu kontrol ediliyor  COUNT(*) sonucu 0'dan büyükse email zaten kayıtlı
                if (Convert.ToInt32(dtKontrol.Rows[0][0]) > 0)
                {
                    // Kullanıcıya uyarı mesajı 
                    MessageBox.Show("Bu email adresi zaten kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // KAYIT İŞLEMİ - YENİ KULLANICIYI VERİTABANINA EKLEME
                // SQL INSERT sorgusu: Yeni kullanıcıyı veritabanına ekle
                // Rol alanı sabit "Musteri" olarak ayarlanıyor - Tüm yeni kayıtlar müşteri olur
                // Rolü "Musteri" olarak sabit çünkü ana giriş ekranındaki formdan sadece müşteri kaydı olur admin kaydı diğer adminler tarafından yapılabilr
                string query = @"INSERT INTO Kullanicilar (Ad, Soyad, Email, Sifre, Rol) 
                               VALUES (@Ad, @Soyad, @Email, @Sifre, 'Musteri')";

                // Sql parametrelerinin kayıt için hazırlanması 
                SqlParameter[] parameters = {
                    new SqlParameter("@Ad", txtAd.Text.Trim()),  // Ad parametresi (boşluklar temizlenir trim ile)
                    new SqlParameter("@Soyad", txtSoyad.Text.Trim()),  // Soyad parametresi (boşluklar temizlenir trim ile)
                    new SqlParameter("@Email", txtEmail.Text.Trim()),  // Email parametresi (boşluklar temizlenir trim ile)
                    new SqlParameter("@Sifre", txtSifre.Text)  // Şifre parametresi
                };

                // ExecuteNonQuery: INSERT, UPDATE, DELETE gibi veri değiştiren sorguları çalıştırır
                // Geriye etkilenen satır sayısını döndürür
                int sonuc = db.ExecuteNonQuery(query, parameters);

                // KAYIT SONUCUNUN DEĞERLENDİRİLMESİ 
                // sonuc > 0: En az bir satır etkilendiyse kayıt başarılı demektir
                if (sonuc > 0)
                {
                    // Başarılı kayıt mesajı 
                    MessageBox.Show("Kayıt başarılı! Giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                // Exception handling ile hata yakalama. Veritabanı bağlantı hatası, ağ hatası gibi durumlarda çalışır
                MessageBox.Show("Kayıt sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // İPTAL BUTONU TIKLAMA OLAY METODU - KAYIT İŞLEMİNİ İPTAL EDER
        private void BtnIptal_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}