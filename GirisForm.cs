using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// UYGULAMANIN GİRİŞ EKRANINI YÖNETİR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class GirisForm : Form
    {
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db;

        public GirisForm()  // Constructordır. Nesne oluşturulduğunda otomatik çalışan yapıcı metoddur.
        {
            InitializeComponent(); // vs code tarafından otomatik oluşturulur, formdaki tüm kontrolleri hazırlar.
            /* Composition (Bileşim) oop özelliği kullanıldı. Başka bir sınıftan örnek alıyor.
               veritabanı bağlantısı için DatabaseHelper sınıfından yeni bir nesne oluşturuluyor ve db alanına atanıyor. */
            db = new DatabaseHelper();
        }

        // Arayüz için gereken tüm form kontrollerini (buton, textbox, label) oluşturulduğu ve tasarımının yapıldığı kısım
        private void InitializeComponent()
        {
            // new ile her kontrol için yeni nesne oluşturuluyor
            // Her kontrol aslında bir sınıf örneğidir (instance) ile o sınıftan somut bir yapı oluşturulur
            this.txtEmail = new System.Windows.Forms.TextBox();   // Email giriş kutusu
            this.txtSifre = new System.Windows.Forms.TextBox();   // Şifre giriş kutusu
            this.btnGiris = new System.Windows.Forms.Button();    // Giriş butonu
            this.btnKayit = new System.Windows.Forms.Button();    // Kayıt butonu
            this.label1 = new System.Windows.Forms.Label();       // Email etiketi
            this.label2 = new System.Windows.Forms.Label();       // Şifre etiketi
            this.label3 = new System.Windows.Forms.Label();       // Başlık etiket
            // Form yerleşim ayarlarını başlat. Windows Forms ile
            this.SuspendLayout();
            
            // BAŞLIK ETİKETİ (label3) - FORMUN ÜST KISMINDA GÖRÜNEN ANA BAŞLIK
            this.label3.AutoSize = true;  // Etiketin boyutu içeriğine (yazıya) göre otomatik ayarlanır
            this.label3.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);  // Font ayarları: Yazı tipi, boyutu ve stilini belirler
            this.label3.Location = new System.Drawing.Point(150, 30);  //Etiketin form içindeki konumu (X, Y koordinatları)
            this.label3.Name = "label3";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label3.Size = new System.Drawing.Size(280, 30);
            this.label3.TabIndex = 6;   //Tab tuşu ile gezinme sırası (6. sırada)
            this.label3.Text = "Uçak Bileti Rezervasyon";   // Etiket üzerinde görünecek metin içeriği
            
            // EMAIL ETİKETİ (label1) - "Email:" YAZISINI GÖSTEREN ETİKET
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 90);
            this.label1.Name = "label1";  // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 0;  // Tab sırası: 1.
            this.label1.Text = "Email:";   // Etiket üzerinde görünecek metin içeriği
            
            // EMAIL GİRİŞ KUTUSU (txtEmail) - KULLANICININ EMAIL ADRESİNİ GİRECEĞİ ALAN
            this.txtEmail.Location = new System.Drawing.Point(150, 87);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(280, 23);
            this.txtEmail.TabIndex = 1;
        
            // ŞİFRE ETİKETİ (label2) - "Şifre:" YAZISINI GÖSTEREN ETİKET
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 130);
            this.label2.Name = "label2";  // Name: Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.label2.Size = new System.Drawing.Size(33, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Şifre:";  // Etiket üzerinde görünecek metin içeriği
            
            // ŞİFRE GİRİŞ KUTUSU (txtSifre) - KULLANICININ ŞİFRESİNİ GİRECEĞİ GİZLİ ALAN
            this.txtSifre.Location = new System.Drawing.Point(150, 127);
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.PasswordChar = '*';   // PasswordChar: Gizlilik için yazılan her karakter '*' olarak gösterilir. Bu özellik şifre girişlerinde güvenlik sağlar, başkalarının şifreyi görmesini engeller
            this.txtSifre.Size = new System.Drawing.Size(280, 23);
            this.txtSifre.TabIndex = 3;
            
            // GİRİŞ BUTONU (btnGiris) - KULLANICI GİRİŞ İŞLEMİNİ BAŞLATAN BUTON
            this.btnGiris.BackColor = System.Drawing.Color.FromArgb(0, 122, 204);
            this.btnGiris.ForeColor = System.Drawing.Color.White;
            this.btnGiris.Location = new System.Drawing.Point(150, 170);
            this.btnGiris.Name = "btnGiris";
            this.btnGiris.Size = new System.Drawing.Size(130, 35);
            this.btnGiris.TabIndex = 4;
            this.btnGiris.Text = "Giriş Yap";    // Buton üzerindeki yazı
            this.btnGiris.UseVisualStyleBackColor = false;
            
            // Event handling (olay işleme) oop prensibi kullanıldı
            // Butonun Click olayına BtnGiris_Click metodunu bağlıyoruz. += operatörü: Event'e yeni bir event handler ekler 
            this.btnGiris.Click += new System.EventHandler(this.BtnGiris_Click);
            
            // KAYIT BUTONU (btnKayit) - YENİ KULLANICI KAYDI İÇİN FORM AÇAN BUTON
            this.btnKayit.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnKayit.ForeColor = System.Drawing.Color.White;
            this.btnKayit.Location = new System.Drawing.Point(300, 170);
            this.btnKayit.Name = "btnKayit";
            this.btnKayit.Size = new System.Drawing.Size(130, 35);
            this.btnKayit.TabIndex = 5;
            this.btnKayit.Text = "Kayıt Ol";  // Buton üzerindeki yazı
            this.btnKayit.UseVisualStyleBackColor = false;

            // Event handling (olay işleme) oop prensibi kullanıldı
            // Butonun Click olayına BtnGiris_Click metodunu bağlıyoruz
            this.btnKayit.Click += new System.EventHandler(this.BtnKayit_Click);
            
            // ANA FORM ÖZELLİKLERİ (GirisForm)
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 250);

            // KONTROLLERİN FORMA EKLENMESİ - TÜM BİLEŞENLER FORM ÜZERİNDE GÖSTERİLİR
            // Controls.Add(): Her kontrolü forma ekliyoruz 
            this.Controls.Add(this.label3);  // Başlık 
            this.Controls.Add(this.btnKayit);  // Kayıt butonu
            this.Controls.Add(this.btnGiris);   // Giriş butonu
            this.Controls.Add(this.txtSifre);   // Şifre kutusu
            this.Controls.Add(this.label2);   // Şifre etiketi
            this.Controls.Add(this.txtEmail);   // Email kutusu
            this.Controls.Add(this.label1);   // Email etiketi
            this.Name = "GirisForm";  // Formun programatik adı
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Kullanıcı Girişi"; // Text: Formun başlık çubuğunda görünecek yazı
            // Form yerleşim ayarlarını tamamla ve kontrolleri görünür yap
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Encapsulation oop yapısı kullanılmıştır
        // Encapsulation ile kontrollere sadece bu sınıfın içindeki metodlardan erişilebilir
        // Tüm kontrol değişkenleri private olarak tanımlandı
        private System.Windows.Forms.TextBox txtEmail; // Email giriş kutusu referansı
        private System.Windows.Forms.TextBox txtSifre; // Şifre giriş kutusu referansı
        private System.Windows.Forms.Button btnGiris;   // Giriş butonu referansı
        private System.Windows.Forms.Button btnKayit;  // Kayıt butonu referansı
        private System.Windows.Forms.Label label1;  // Email etiketi referansı
        private System.Windows.Forms.Label label2; // Şifre etiketi referansı
        private System.Windows.Forms.Label label3;  // Başlık etiketi referansı

        // GİRİŞ BUTONU TIKLAMA OLAY METODU - KULLANICI GİRİŞ İŞLEMLERİNİ YÖNETİR
        // Event handling oop yapısı. Buton tıklandığında otomatik çağrılan metod
        private void BtnGiris_Click(object sender, EventArgs e) // sender: Olayı tetikleyen kontrol (bu durumda btnGiris)
        {
            // Exception handling oop yapısı ile hata yönetimi yapılıyor
            // try-catch bloğu, Beklenmeyen hataları yakalar ve uygulamanın çökmesini engeller
            try
            {
                // Giriş bilgileri boş mu diye kontrol
                if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))  
                // string.IsNullOrWhiteSpace(): String'in null, boş veya sadece boşluk karakterlerinden oluşup oluşmadığını kontrol eder
                {
                    // Bilgiler de eksik varsa kullanıcıya mesaj
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // SQL SORGUSU HAZIRLAMA - VERİTABANINDA KULLANICI ARAMA
                // Sql Sorgusu veritabanında kullanıcı araması @Email ve @Sifre ve aktifmi parametreleri ile yapılır
                string query = "SELECT * FROM Kullanicilar WHERE Email = @Email AND Sifre = @Sifre AND AktifMi = 1";
                // Sql parametresinin oluşturulması güvenlik için parametre ile
                SqlParameter[] parameters = {
                    new SqlParameter("@Email", txtEmail.Text.Trim()), // @Email parametresi: Kullanıcının girdiği email (baştaki ve sondaki boşluklar temizlenir trim ile)
                    new SqlParameter("@Sifre", txtSifre.Text) // @Sifre parametresi: Kullanıcının girdiği şifre 
                };

                // Veritabanı sorgusu çalıştırılıp kullanıcı doğrulaması yapılır
                // DatabaseHelper sınıfının ExecuteQuery metodu çağrılıyor. Sonuç DataTable olarak dönüyor
                DataTable dt = db.ExecuteQuery(query, parameters);

                // Sorgu sonucuna bakılması kullanıcı bulundu mu 
                // Eğer 0'dan büyükse kullanıcı bulundu demektir
                if (dt.Rows.Count > 0)
                {
                    // Kullanici sınıfından yeni bir nesne oluşturuluyor
                    Kullanici kullanici = new Kullanici
                    {
                        // DataTable'dan gelen verileri Kullanici nesnesine aktarıyoruz
                        KullaniciID = Convert.ToInt32(dt.Rows[0]["KullaniciID"]),
                        Ad = dt.Rows[0]["Ad"].ToString(), // ToString(): Nesne tipini string'e çevirir
                        Soyad = dt.Rows[0]["Soyad"].ToString(), 
                        Email = dt.Rows[0]["Email"].ToString(),
                        Rol = dt.Rows[0]["Rol"].ToString()
                    };

                    // Başarılı giriş için kullanıcıya mesaj
                    // kullanici.AdSoyad Kullanici sınıfından AdSoyad property'si 
                    MessageBox.Show($"Hoş geldiniz {kullanici.AdSoyad}!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Polymorphism (farklı davranış) oop yapısı kullanıldı
                    // Kullanıcının rolüne göre farklı formlar açılıyor
                    if (kullanici.Rol == "Admin")
                    {
                        // Admin kullanıcı için AdminPanel formu açılıyor
                        AdminPanel adminPanel = new AdminPanel(kullanici); // yeni nesne oluşturuyoruz
                        adminPanel.Show(); 
                    }
                    else
                    {
                        // Normal kullanıcı için MusteriPanel formu açılıyor
                        MusteriPanel musteriPanel = new MusteriPanel(kullanici); // yeni nesne oluşturuyoruz
                        musteriPanel.Show(); 
                    }
                    this.Hide();
                }
                else
                {
                    // Giriş başarısız kullanıcı bulunamadı ya da bilgiler yanlış ise kullanıcıya mesaj
                    MessageBox.Show("Email veya şifre hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Exception handling ile hata yakalama. Veritabanı bağlantı hatası, ağ hatası gibi durumlarda çalışır
                MessageBox.Show("Giriş sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // KAYIT BUTONU TIKLAMA OLAY METODU - YENİ KULLANICI KAYIT FORMUNU AÇAR
        private void BtnKayit_Click(object sender, EventArgs e)
        {
            KayitForm kayitForm = new KayitForm(); // KayitForm sınıfından yeni bir nesne oluşturuluyor
            kayitForm.ShowDialog(); 
        }
    }
}