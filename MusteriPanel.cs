using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// MUSTERİ PANEL SINIFI - BU SINIF MUSTERİ EKRANINI VE YAPACAĞI İŞLEMLERİ YÖNETİR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class MusteriPanel : Form
    {
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db; 
        private Kullanici aktifKullanici;  // Oturum açmış kullanıcının bilgilerini tutar

        public MusteriPanel(Kullanici kullanici) // Constructordır. Nesne oluşturulduğunda otomatik çalışan yapıcı metoddur.
        {
            InitializeComponent(); // vs code tarafından otomatik oluşturulur, formdaki tüm kontrolleri hazırlar.
            /* Composition (Bileşim) oop özelliği kullanıldı. Başka bir sınıftan örnek alıyor.
               veritabanı bağlantısı için DatabaseHelper sınıfından yeni bir nesne oluşturuluyor ve db alanına atanıyor. */
            db = new DatabaseHelper(); 
            aktifKullanici = kullanici; // Giriş yapan kullanıcıyı saklamak için
            lblHosgeldin.Text = $"Hoş geldiniz, {aktifKullanici.AdSoyad}"; // Kişiselleştirilmiş hoşgeldin mesajı
            // Mevcut uçuşları listeye yükleyen metod
            UcuslariYukle(); 
        }

        // Arayüz için gereken tüm form kontrollerini (buton, textbox, label) oluşturulduğu ve tasarımının yapıldığı kısım
        private void InitializeComponent()
        {
            // ANA SEKMELİ YAPI: Kullanıcı işlevlerini sekmeler halinde düzenler
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabUcusAra = new System.Windows.Forms.TabPage();  // Uçuş arama ve rezervasyon sekmesi
            this.tabRezervasyonlar = new System.Windows.Forms.TabPage();  // Mevcut rezervasyonları görüntüleme sekmesi
            
            // BAŞLIK VE ÇIKIŞ BİLEŞENLERİ
            this.lblHosgeldin = new System.Windows.Forms.Label();  // Kullanıcıyı karşılayan başlık
            this.btnCikis = new System.Windows.Forms.Button();  // Uygulamadan çıkış butonu
            
            // UÇUŞ ARAMA SEKMESİ BİLEŞENLERİ
            this.groupArama = new System.Windows.Forms.GroupBox(); // Arama kriterleri için gruplama kutusu
            this.txtAramaNereden = new System.Windows.Forms.TextBox(); // Kalkış şehri giriş alanı
            this.txtAramaNereye = new System.Windows.Forms.TextBox(); // Varış şehri giriş alanı
            this.dtpAramaTarih = new System.Windows.Forms.DateTimePicker(); // Tarih seçici bileşen
            this.btnAra = new System.Windows.Forms.Button(); // Filtreleme butonu
            this.btnTumunuGoster = new System.Windows.Forms.Button(); // Tüm uçuşları listeleme butonu
            this.dgvUcuslar = new System.Windows.Forms.DataGridView(); // Uçuşları tablo halinde gösterir
            this.btnRezervasyonYap = new System.Windows.Forms.Button(); // Seçili uçuş için rezervasyon butonu
            
            // REZERVASYONLARIM SEKMESİ BİLEŞENLERİ
            this.dgvRezervasyonlar = new System.Windows.Forms.DataGridView(); // Rezervasyon listesi tablosu
            this.btnRezervasyonIptal = new System.Windows.Forms.Button(); // Rezervasyon iptal butonu
            this.btnRezYenile = new System.Windows.Forms.Button(); // Listeyi yenileme butonu
            
            // BİLEŞENLERİN BAŞLATILMASI: Windows Forms bileşenlerini hazırlama süreci
            this.tabControl.SuspendLayout();
            this.tabUcusAra.SuspendLayout();
            this.tabRezervasyonlar.SuspendLayout();
            this.groupArama.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcuslar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRezervasyonlar)).BeginInit();
            this.SuspendLayout();
            
            // HOŞGELDİN LABEL AYARLARI: Kullanıcı adını gösteren metin kutusu
            this.lblHosgeldin.AutoSize = true; // Metin boyutuna göre otomatik genişlik
            this.lblHosgeldin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold); // Kalın yazı tipi
            this.lblHosgeldin.Location = new System.Drawing.Point(20, 15); // Sol üst köşede konumlandır
            this.lblHosgeldin.Name = "lblHosgeldin";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.lblHosgeldin.Size = new System.Drawing.Size(200, 21);
            this.lblHosgeldin.TabIndex = 0; // Tab sırasında ilk sırada
            this.lblHosgeldin.Text = "Hoş geldiniz"; // Varsayılan metin
            
            // ÇIKIŞ BUTONU AYARLARI: Uygulamadan çıkmak için kırmızı buton
            this.btnCikis.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan
            this.btnCikis.ForeColor = System.Drawing.Color.White; // Beyaz yazı rengi
            this.btnCikis.Location = new System.Drawing.Point(820, 10); // Sağ üst köşede
            this.btnCikis.Name = "btnCikis"; // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.btnCikis.Size = new System.Drawing.Size(80, 30); // Sabit genişlik ve yükseklik
            this.btnCikis.TabIndex = 1; // Tab sırasında ikinci
            this.btnCikis.Text = "Çıkış"; // Buton metni
            this.btnCikis.UseVisualStyleBackColor = false; // Özel arkaplan rengini kullan
            // Event handling (olay işleme) oop prensibi kullanıldı
            // Butonun Click olayına metod bağlıyoruz
            this.btnCikis.Click += new System.EventHandler(this.BtnCikis_Click); 
            
            // TAB CONTROL AYARLARI: Sekmeli arayüzün ana konteyneri
            this.tabControl.Location = new System.Drawing.Point(20, 50); // Başlığın altında konumlandır
            this.tabControl.Name = "tabControl";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.tabControl.SelectedIndex = 0; // İlk sekmeyi varsayılan olarak seç
            this.tabControl.Size = new System.Drawing.Size(880, 520); // Formun büyük kısmını kapla
            this.tabControl.TabIndex = 2; // Tab sırasında üçüncü
            
            // UÇUŞ ARA SEKMESİ AYARLARI
            this.tabUcusAra.Controls.Add(this.dgvUcuslar); // Uçuş listesi tablosunu ekle
            this.tabUcusAra.Controls.Add(this.groupArama); // Arama grubunu ekle
            this.tabUcusAra.Controls.Add(this.btnRezervasyonYap); // Rezervasyon butonunu ekle
            this.tabUcusAra.Location = new System.Drawing.Point(4, 24); // Sekme konumu
            this.tabUcusAra.Name = "tabUcusAra"; // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.tabUcusAra.Padding = new System.Windows.Forms.Padding(3); // İç kenar boşluğu
            this.tabUcusAra.Size = new System.Drawing.Size(872, 492); // Sekme boyutu
            this.tabUcusAra.TabIndex = 0; // İlk sekme
            this.tabUcusAra.Text = "Uçuş Ara"; // Sekme başlığı
            this.tabUcusAra.UseVisualStyleBackColor = true; // Varsayılan stil
            
            // ARAMA GRUBU AYARLARI: Filtreleme kriterleri için gruplu alan
            this.groupArama.Location = new System.Drawing.Point(15, 15); // Sekme içinde konum
            this.groupArama.Name = "groupArama"; // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.groupArama.Size = new System.Drawing.Size(840, 120); // Sabit yükseklik
            this.groupArama.TabIndex = 0; // İlk focus alanı
            this.groupArama.TabStop = false; // Tab ile seçilemez
            this.groupArama.Text = "Uçuş Ara"; // Grup kutusu başlığı
            
            // ETİKETLERİ DİZİ İLE OLUŞTURMA: Kalkış, varış ve tarih etiketleri
            var lbls = new Label[] {
                new Label { Text = "Nereden:", Location = new System.Drawing.Point(20, 35), AutoSize = true },
                new Label { Text = "Nereye:", Location = new System.Drawing.Point(240, 35), AutoSize = true },
                new Label { Text = "Kalkış Tarihi:", Location = new System.Drawing.Point(460, 35), AutoSize = true }
            };
            this.groupArama.Controls.AddRange(lbls); // Tüm etiketleri gruba ekle
            
            // KALKIŞ YERİ GİRİŞ ALANI
            this.txtAramaNereden.Location = new System.Drawing.Point(20, 55); // Etiketin altında konum
            this.txtAramaNereden.PlaceholderText = "Örn: İstanbul"; // Kullanıcı kılavuzu
            this.txtAramaNereden.Size = new System.Drawing.Size(200, 23); // Sabit genişlik
            this.groupArama.Controls.Add(this.txtAramaNereden); // Gruba ekle
            
            // VARIŞ YERİ GİRİŞ ALANI
            this.txtAramaNereye.Location = new System.Drawing.Point(240, 55); // Yan yana konumlandır
            this.txtAramaNereye.PlaceholderText = "Örn: Ankara"; // Örnek metin
            this.txtAramaNereye.Size = new System.Drawing.Size(200, 23); // Aynı genişlik
            this.groupArama.Controls.Add(this.txtAramaNereye); // Gruba ekle
            
            // TARİH SEÇİCİ AYARLARI
            this.dtpAramaTarih.Format = DateTimePickerFormat.Short; // Kısa tarih formatı (gg.aa.yyyy)
            this.dtpAramaTarih.Location = new System.Drawing.Point(460, 55); // Diğer alanların yanında
            this.dtpAramaTarih.Size = new System.Drawing.Size(200, 23); // Aynı genişlik
            this.groupArama.Controls.Add(this.dtpAramaTarih); // Gruba ekle
            
            // ARAMA BUTONU AYARLARI
            this.btnAra.BackColor = System.Drawing.Color.FromArgb(52, 152, 219); // Mavi arkaplan
            this.btnAra.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnAra.Location = new System.Drawing.Point(680, 50); // En sağda konumlandır
            this.btnAra.Size = new System.Drawing.Size(140, 30); // Buton boyutu
            this.btnAra.Text = " Uçuş Ara"; // buton metni
            this.btnAra.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnAra.Click += new System.EventHandler(this.BtnAra_Click); // Tıklama olayını metod bağla 
            this.groupArama.Controls.Add(this.btnAra); // Gruba ekle
            
            // TÜMÜNÜ GÖSTER BUTONU: Filtreleri temizleyip tüm uçuşları listeler
            this.btnTumunuGoster.BackColor = System.Drawing.Color.FromArgb(149, 165, 166); // Gri arkaplan
            this.btnTumunuGoster.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnTumunuGoster.Location = new System.Drawing.Point(20, 85); // Diğer kontrollerin altında
            this.btnTumunuGoster.Size = new System.Drawing.Size(800, 25); // Tam genişlik
            this.btnTumunuGoster.Text = "Tüm Uçuşları Göster"; // Buton metni
            this.btnTumunuGoster.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnTumunuGoster.Click += new System.EventHandler(this.BtnTumunuGoster_Click); // Tıklama olayını bağla
            this.groupArama.Controls.Add(this.btnTumunuGoster); // Gruba ekle
            
            // UÇUŞ LİSTESİ TABLOSU AYARLARI
            this.dgvUcuslar.AllowUserToAddRows = false; // Kullanıcı yeni satır ekleyemez
            this.dgvUcuslar.AllowUserToDeleteRows = false; // Kullanıcı satır silemez
            this.dgvUcuslar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları eşit dağıt
            this.dgvUcuslar.Location = new System.Drawing.Point(15, 150); // Arama grubunun altında
            this.dgvUcuslar.ReadOnly = true; // Sadece okunabilir mod
            this.dgvUcuslar.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            this.dgvUcuslar.Size = new System.Drawing.Size(840, 280); // Tablo boyutu
            
            // REZERVASYON YAP BUTONU: Seçili uçuş için rezervasyon formu açar
            this.btnRezervasyonYap.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Yeşil arkaplan
            this.btnRezervasyonYap.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnRezervasyonYap.Location = new System.Drawing.Point(15, 440); // Tablonun altında
            this.btnRezervasyonYap.Size = new System.Drawing.Size(840, 40); // Tam genişlik
            this.btnRezervasyonYap.Text = "Seçili Uçuş İçin Rezervasyon Yap"; // Uçak emojili metin
            this.btnRezervasyonYap.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnRezervasyonYap.Click += new System.EventHandler(this.BtnRezervasyonYap_Click); // Tıklama olayını bağla
            
            // REZERVASYONLARIM SEKMESİ AYARLARI
            this.tabRezervasyonlar.Controls.Add(this.dgvRezervasyonlar); // Rezervasyon tablosunu ekle
            this.tabRezervasyonlar.Controls.Add(this.btnRezervasyonIptal); // İptal butonunu ekle
            this.tabRezervasyonlar.Controls.Add(this.btnRezYenile); // Yenile butonunu ekle
            this.tabRezervasyonlar.Location = new System.Drawing.Point(4, 24); // Sekme konumu
            this.tabRezervasyonlar.Name = "tabRezervasyonlar";
            this.tabRezervasyonlar.Size = new System.Drawing.Size(872, 492); // Sekme boyutu
            this.tabRezervasyonlar.TabIndex = 1; // İkinci sekme
            this.tabRezervasyonlar.Text = "Rezervasyonlarım"; // Sekme başlığı
            this.tabRezervasyonlar.UseVisualStyleBackColor = true; // Varsayılan stil
            
            // REZERVASYON LİSTESİ TABLOSU AYARLARI
            this.dgvRezervasyonlar.AllowUserToAddRows = false; // Yeni satır eklenemez
            this.dgvRezervasyonlar.AllowUserToDeleteRows = false; // Satır silinemez
            this.dgvRezervasyonlar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Eşit sütun genişliği
            this.dgvRezervasyonlar.Location = new System.Drawing.Point(15, 15); // Sekme içinde üstte
            this.dgvRezervasyonlar.ReadOnly = true; // Sadece okunabilir
            this.dgvRezervasyonlar.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            this.dgvRezervasyonlar.Size = new System.Drawing.Size(840, 410); // Büyük alan kapla
            
            // YENİLE BUTONU: Rezervasyon listesini veritabanından tekrar yükler
            this.btnRezYenile.BackColor = System.Drawing.Color.FromArgb(52, 152, 219); // Mavi arkaplan
            this.btnRezYenile.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnRezYenile.Location = new System.Drawing.Point(15, 435); // Tablonun altında solda
            this.btnRezYenile.Size = new System.Drawing.Size(120, 40); // Sabit genişlik
            this.btnRezYenile.Text = "Yenile"; // Buton metni
            this.btnRezYenile.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnRezYenile.Click += new System.EventHandler(this.BtnRezYenile_Click); // Tıklama olayını bağla
            
            // REZERVASYON İPTAL BUTONU: Seçili rezervasyonu iptal eder
            this.btnRezervasyonIptal.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan
            this.btnRezervasyonIptal.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnRezervasyonIptal.Location = new System.Drawing.Point(150, 435); // Yenile butonunun yanında
            this.btnRezervasyonIptal.Size = new System.Drawing.Size(200, 40); // Sabit genişlik
            this.btnRezervasyonIptal.Text = "Seçili Rezervasyonu İptal Et"; // Buton metni
            this.btnRezervasyonIptal.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnRezervasyonIptal.Click += new System.EventHandler(this.BtnRezervasyonIptal_Click); // Tıklama olayını bağla
            
            // SEKMELERİ ANA KONTROLE EKLEME
            this.tabControl.Controls.Add(this.tabUcusAra); // Uçuş ara sekmesini ekle
            this.tabControl.Controls.Add(this.tabRezervasyonlar); // Rezervasyonlarım sekmesini ekle
            
            // ANA FORM AYARLARI
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); // Otomatik ölçekleme boyutu
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // Yazı tipine göre ölçekle
            this.ClientSize = new System.Drawing.Size(920, 590); // Form genişlik ve yükseklik
            this.Controls.Add(this.tabControl); // Sekmeli yapıyı forma ekle
            this.Controls.Add(this.btnCikis); // Çıkış butonunu forma ekle
            this.Controls.Add(this.lblHosgeldin); // Hoşgeldin yazısını forma ekle
            this.Name = "MusteriPanel"; // Form nesne adı
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Ekran ortasında aç
            this.Text = "Müşteri Paneli"; // Form başlık çubuğu metni
            this.Load += new System.EventHandler(this.MusteriPanel_Load); // Form yükleme olayı
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MusteriPanel_FormClosing); // Form kapanma olayı
            
            // BİLEŞEN BAŞLATMA İŞLEMLERİNİ TAMAMLA
            this.tabControl.ResumeLayout(false);
            this.tabUcusAra.ResumeLayout(false);
            this.tabRezervasyonlar.ResumeLayout(false);
            this.groupArama.ResumeLayout(false);
            this.groupArama.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcuslar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRezervasyonlar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // DEĞİŞKEN TANIMLARI: Form bileşenlerini sınıf seviyesinde tanımla
        private TabControl tabControl; // Sekmeli arayüz kontrolü
        private TabPage tabUcusAra, tabRezervasyonlar; // Sekme sayfaları
        private Label lblHosgeldin; // Hoşgeldin mesajı
        private Button btnCikis; // Çıkış butonu
        private GroupBox groupArama; // Arama kriterleri grubu
        private TextBox txtAramaNereden, txtAramaNereye; // Kalkış ve varış girişleri
        private DateTimePicker dtpAramaTarih; // Tarih seçici
        private Button btnAra, btnTumunuGoster, btnRezervasyonYap; // İşlev butonları
        private DataGridView dgvUcuslar, dgvRezervasyonlar; // Veri tabloları
        private Button btnRezervasyonIptal, btnRezYenile; // Rezervasyon işlem butonları

        // FORM YÜKLENME OLAYI: Panel açıldığında rezervasyonları yükler
        private void MusteriPanel_Load(object sender, EventArgs e)
        {
            RezervasyonlariYukle(); // Kullanıcının rezervasyonlarını listeler
        }

        // UÇUŞLARI YÜKLEME METODU: Veritabanından aktif uçuşları çeker ve listeler
        private void UcuslariYukle()
        {
            try
            {
                // SQL SORGUSU: Aktif, gelecekteki ve boş koltuklu uçuşları getir
                string query = @"SELECT u.UcusID, u.UcusKodu AS 'Uçuş Kodu', 
                               uc.UcakKodu AS 'Uçak Kodu', 
                               uc.Model AS 'Uçak Modeli',
                               u.Nereden, 
                               u.Nereye, 
                               CONVERT(VARCHAR, u.KalkisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.KalkisTarihi, 108) AS 'Kalkış Tarihi',
                               CONVERT(VARCHAR, u.VarisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.VarisTarihi, 108) AS 'Varış Tarihi',
                               CAST(u.Fiyat AS DECIMAL(10,2)) AS 'Fiyat (TL)',
                               u.MusaitKoltuk AS 'Müsait Koltuk'
                               FROM Ucuslar u 
                               INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID 
                               WHERE u.AktifMi = 1 AND u.KalkisTarihi > GETDATE() AND u.MusaitKoltuk > 0
                               ORDER BY u.KalkisTarihi";
                
                DataTable dt = db.ExecuteQuery(query); // Sorguyu çalıştır ve sonucu al
                dgvUcuslar.DataSource = dt; // Tabloya verileri bağla
                
                // GİZLİ SÜTUN: UcusID'yi kullanıcıdan gizle (sadece arka planda kullanılacak)
                if (dgvUcuslar.Columns.Contains("UcusID"))
                    dgvUcuslar.Columns["UcusID"].Visible = false;
                
                // BİLGİ MESAJI: Eğer uçuş yoksa kullanıcıyı bilgilendir
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Henüz aktif uçuş bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Veritabanı hatasını kullanıcıya göster
                MessageBox.Show("Uçuşlar yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ARAMA BUTONU TIKLAMA: Kriterlere göre uçuş filtreleme
        private void BtnAra_Click(object sender, EventArgs e)
        {
            try
            {
                // KRİTERLERİ AL: Kullanıcının girdiği filtre değerlerini oku
                string nereden = txtAramaNereden.Text.Trim(); // Kalkış yeri (boşlukları temizle)
                string nereye = txtAramaNereye.Text.Trim(); // Varış yeri
                DateTime tarih = dtpAramaTarih.Value.Date; // Seçilen tarih (saat bilgisini temizle)

                // TEMEL SORGÜ: Aktif uçuşları getiren temel SQL
                string query = @"SELECT u.UcusID, u.UcusKodu AS 'Uçuş Kodu', 
                               uc.UcakKodu AS 'Uçak Kodu', 
                               uc.Model AS 'Uçak Modeli',
                               u.Nereden, 
                               u.Nereye, 
                               CONVERT(VARCHAR, u.KalkisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.KalkisTarihi, 108) AS 'Kalkış Tarihi',
                               CONVERT(VARCHAR, u.VarisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.VarisTarihi, 108) AS 'Varış Tarihi',
                               CAST(u.Fiyat AS DECIMAL(10,2)) AS 'Fiyat (TL)',
                               u.MusaitKoltuk AS 'Müsait Koltuk'
                               FROM Ucuslar u 
                               INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID 
                               WHERE u.AktifMi = 1 AND u.KalkisTarihi > GETDATE() AND u.MusaitKoltuk > 0";

                // PARAMETRE LİSTESİ: SQL enjeksiyonunu önlemek için parametreli sorgu
                var parameters = new System.Collections.Generic.List<SqlParameter>();

                // KRİTER EKLEME: Kullanıcı değer girdiyse where koşuluna ekle
                if (!string.IsNullOrEmpty(nereden))
                {
                    query += " AND u.Nereden LIKE @Nereden"; // Kalkış yeri filtresi
                    parameters.Add(new SqlParameter("@Nereden", "%" + nereden + "%")); // Joker karakter ile
                }

                if (!string.IsNullOrEmpty(nereye))
                {
                    query += " AND u.Nereye LIKE @Nereye"; // Varış yeri filtresi
                    parameters.Add(new SqlParameter("@Nereye", "%" + nereye + "%")); // Joker karakter ile
                }

                // TARİH FİLTRESİ: Seçilen tarihe göre filtrele
                query += " AND CONVERT(DATE, u.KalkisTarihi) = @Tarih ORDER BY u.KalkisTarihi";
                parameters.Add(new SqlParameter("@Tarih", tarih));

                // SORGULARI ÇALIŞTIR: Filtrelenmiş verileri getir
                DataTable dt = db.ExecuteQuery(query, parameters.ToArray());
                dgvUcuslar.DataSource = dt; // Tabloya bağla
                
                // ID SÜTUNUNU GİZLE: UcusID'yi kullanıcı arayüzünden kaldır
                if (dgvUcuslar.Columns.Contains("UcusID"))
                    dgvUcuslar.Columns["UcusID"].Visible = false;

                // SONUÇ MESAJI: Kullanıcıyı arama sonucu hakkında bilgilendir
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Arama kriterlerinize uygun uçuş bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"{dt.Rows.Count} adet uçuş bulundu.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Arama sırasındaki hataları yakala ve göster
                MessageBox.Show("Arama sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TÜMÜNÜ GÖSTER BUTONU: Filtreleri temizleyip tüm uçuşları listeler
        private void BtnTumunuGoster_Click(object sender, EventArgs e)
        {
            txtAramaNereden.Clear(); // Kalkış yeri kutusunu temizle
            txtAramaNereye.Clear(); // Varış yeri kutusunu temizle
            dtpAramaTarih.Value = DateTime.Now; // Tarihi bugüne resetle
            UcuslariYukle(); // Tüm uçuşları yeniden yükle
        }

        // REZERVASYON YAP BUTONU: Seçili uçuş için rezervasyon formu açar
        private void BtnRezervasyonYap_Click(object sender, EventArgs e)
        {
            try
            {
                // SEÇİM KONTROLÜ: Kullanıcının bir uçuş seçtiğinden emin ol
                if (dgvUcuslar.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen rezervasyon yapmak istediğiniz uçuşu seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SEÇİLİ UÇUŞ BİLGİLERİNİ AL
                int ucusID = Convert.ToInt32(dgvUcuslar.SelectedRows[0].Cells["UcusID"].Value); // Gizli UcusID
                int musaitKoltuk = Convert.ToInt32(dgvUcuslar.SelectedRows[0].Cells["Müsait Koltuk"].Value); // Boş koltuk sayısı

                // KOLTUK KONTROLÜ: Uçuşta boş koltuk olup olmadığını kontrol et
                if (musaitKoltuk <= 0)
                {
                    MessageBox.Show("Bu uçuşta müsait koltuk kalmamıştır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // REZERVASYON FORMU AÇ: Yeni rezervasyon için formu başlat
                RezervasyonForm rezervasyonForm = new RezervasyonForm(ucusID, aktifKullanici.KullaniciID);
                if (rezervasyonForm.ShowDialog() == DialogResult.OK) // Formu modal aç ve sonucu kontrol et
                {
                    // BAŞARILI MESAJI: Rezervasyon tamamlandığında kullanıcıyı bilgilendir
                    MessageBox.Show("Rezervasyonunuz başarıyla oluşturuldu!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcuslariYukle(); // Uçuş listesini güncelle (koltuk sayısı değişti)
                    RezervasyonlariYukle(); // Rezervasyon listesini güncelle
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Rezervasyon işlemindeki hataları yakala
                MessageBox.Show("Rezervasyon sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // REZERVASYONLARI YÜKLEME: Kullanıcının tüm rezervasyonlarını listeler
        private void RezervasyonlariYukle()
        {
            try
            {
                // SQL SORGUSU: Kullanıcının rezervasyonlarını uçuş bilgileriyle birleştirerek getir
                string query = @"SELECT r.RezervasyonID, r.YolcuAd AS 'Yolcu Adı', r.YolcuSoyad AS 'Yolcu Soyadı',
                               u.UcusKodu AS 'Uçuş Kodu', u.Nereden, u.Nereye,
                               CONVERT(VARCHAR, u.KalkisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.KalkisTarihi, 108) AS 'Kalkış Tarihi',
                               CAST(u.Fiyat AS DECIMAL(10,2)) AS 'Temel Fiyat (TL)',
                               CAST(r.UygulananFiyat AS DECIMAL(10,2)) AS 'Ödenen Fiyat (TL)',
                               r.KoltukNo AS 'Koltuk No',
                               CONVERT(VARCHAR, r.RezervasyonTarihi, 104) AS 'Rezervasyon Tarihi',
                               r.Durum
                               FROM Rezervasyonlar r
                               INNER JOIN Ucuslar u ON r.UcusID = u.UcusID
                               WHERE r.KullaniciID = @KullaniciID
                               ORDER BY r.RezervasyonTarihi DESC";

                // PARAMETRELİ SORGU: Sadece aktif kullanıcının rezervasyonlarını getir
                SqlParameter[] parameters = { new SqlParameter("@KullaniciID", aktifKullanici.KullaniciID) };
                DataTable dt = db.ExecuteQuery(query, parameters);
                dgvRezervasyonlar.DataSource = dt; // Tabloya bağla
                
                // GİZLİ SÜTUN: RezervasyonID'yi kullanıcı arayüzünden gizle
                if (dgvRezervasyonlar.Columns.Contains("RezervasyonID"))
                    dgvRezervasyonlar.Columns["RezervasyonID"].Visible = false;

                // BİLGİ MESAJI: Rezervasyon yoksa kullanıcıyı bilgilendir
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Henüz rezervasyonunuz bulunmamaktadır.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Rezervasyon yükleme hatasını yakala
                MessageBox.Show("Rezervasyonlar yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // REZERVASYON İPTAL BUTONU: Seçili rezervasyonu iptal eder
        private void BtnRezervasyonIptal_Click(object sender, EventArgs e)
        {
            try
            {
                // SEÇİM KONTROLÜ: Kullanıcının bir rezervasyon seçtiğinden emin ol
                if (dgvRezervasyonlar.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen iptal etmek istediğiniz rezervasyonu seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SEÇİLİ REZERVASYON BİLGİLERİNİ AL
                int rezervasyonID = Convert.ToInt32(dgvRezervasyonlar.SelectedRows[0].Cells["RezervasyonID"].Value); // Gizli ID
                string durum = dgvRezervasyonlar.SelectedRows[0].Cells["Durum"].Value.ToString(); // Mevcut durum

                // DURUM KONTROLÜ: Zaten iptal edilmiş rezervasyonu tekrar iptal etmeyi önle
                if (durum == "Iptal")
                {
                    MessageBox.Show("Bu rezervasyon zaten iptal edilmiş!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // ONAY DİYALOĞU: Kullanıcıdan iptal işlemini onaylamasını iste
                var sonuc = MessageBox.Show("Rezervasyonu iptal etmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                // ONAY VERİLDİYSE İPTAL İŞLEMİNİ GERÇEKLEŞTİR
                if (sonuc == DialogResult.Yes)
                {
                    // TRANSACTION BENZERİ İŞLEM: Rezervasyonu iptal et ve koltuğu geri ekle
                    string updateQuery = @"UPDATE Rezervasyonlar SET Durum = 'Iptal' WHERE RezervasyonID = @RezervasyonID;
                                         UPDATE Ucuslar SET MusaitKoltuk = MusaitKoltuk + 1 
                                         WHERE UcusID = (SELECT UcusID FROM Rezervasyonlar WHERE RezervasyonID = @RezervasyonID)";
                    
                    SqlParameter[] parameters = { new SqlParameter("@RezervasyonID", rezervasyonID) };
                    db.ExecuteNonQuery(updateQuery, parameters); // Güncelleme sorgusunu çalıştır
                    
                    // BAŞARI MESAJI: İşlemin başarılı olduğunu bildir
                    MessageBox.Show("Rezervasyon iptal edildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RezervasyonlariYukle(); // Listeyi güncelle
                    UcuslariYukle(); // Uçuş listesini güncelle (koltuk sayısı değişti)
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: İptal işlemindeki hataları yakala
                MessageBox.Show("İptal işlemi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // YENİLE BUTONU: Rezervasyon listesini manuel olarak yeniler
        private void BtnRezYenile_Click(object sender, EventArgs e)
        {
            RezervasyonlariYukle(); // Rezervasyon listesini veritabanından tekrar yükle
        }

        // ÇIKIŞ BUTONU: Uygulamadan tamamen çıkar
        private void BtnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Uygulamayı kapat
        }

        // FORM KAPANMA OLAYI: Pencere kapatıldığında uygulamayı sonlandır
        private void MusteriPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); // Uygulamayı tamamen kapat
        }
    }
}