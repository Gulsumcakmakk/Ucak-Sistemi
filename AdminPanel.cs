using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// ADMİN PANEL SINIFI - BU SINIF ADMİN EKRANINI VE YAPACAĞI İŞLEMLERİ YÖNETİR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    // KALITIM (Inheritance): AdminPanel sınıfı Form sınıfından türetilerek Windows Form özellikleri kazanıyor
    // ADMIN PANELİ: Sistem yöneticileri için kapsamlı yönetim arayüzü sağlar
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class AdminPanel : Form
    {
        // ENKAPSULASYON (Encapsulation): Private field'lar ile iç veri yapısı dışarıdan gizleniyor
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db; // Veritabanı işlemleri için yardımcı sınıf örneği - CRUD operasyonlarını merkezileştirir
        private Kullanici aktifKullanici; // Oturum açmış admin kullanıcısının bilgilerini tutar

        // YAPICI METOT (Constructor): Panel açılırken çalışarak başlangıç ayarlarını yapar
        public AdminPanel(Kullanici kullanici)
        {
            InitializeComponent(); // Form bileşenlerini oluştur ve yerleştir - Windows Forms tasarımını başlatır
            db = new DatabaseHelper(); // Veritabanı bağlantısı için nesne oluştur - SQL bağlantılarını yönetir
            aktifKullanici = kullanici; // Giriş yapan admin kullanıcısını sakla - kimlik doğrulama için
            lblHosgeldin.Text = $"Hoş geldiniz, {aktifKullanici.AdSoyad}"; // Kişiselleştirilmiş hoşgeldin mesajı - kullanıcı deneyimini iyileştirir
            KullanicilariYukle(); // Tüm kullanıcıları veritabanından yükle - kullanıcı yönetimi için
            UcaklariYukle(); // Tüm uçakları veritabanından yükle - filo yönetimi için
            UcuslariYukle(); // Tüm uçuşları veritabanından yükle - uçuş operasyonları için
        }
 
        // FORMLARIN BAŞLANGIÇ METODU: Tüm arayüz bileşenlerini oluşturur ve konumlandırır
        // BU METOT: Visual Studio designer tarafından otomatik oluşturulan kodu temsil eder
        private void InitializeComponent()
        {
            // ANA SEKMELİ YAPI: Admin işlevlerini kategorilere ayırarak düzenler
            this.tabControl = new System.Windows.Forms.TabControl(); // Ana sekme konteyneri - gezinmeyi kolaylaştırır
            this.tabKullanici = new System.Windows.Forms.TabPage(); // Kullanıcı yönetimi sekmesi - kullanıcı CRUD işlemleri
            this.tabUcak = new System.Windows.Forms.TabPage(); // Uçak yönetimi sekmesi - filo yönetimi
            this.tabUcus = new System.Windows.Forms.TabPage(); // Uçuş yönetimi sekmesi - uçuş planlama
            this.lblHosgeldin = new System.Windows.Forms.Label(); // Kullanıcıyı karşılayan başlık
            this.btnCikis = new System.Windows.Forms.Button(); // Uygulamadan çıkış butonu
            
            // KULLANICI YÖNETİMİ SEKMESİ BİLEŞENLERİ
            this.dgvKullanicilar = new System.Windows.Forms.DataGridView(); // Kullanıcı listesi tablosu - tüm kullanıcıları gösterir
            this.groupKullanici = new System.Windows.Forms.GroupBox(); // Yeni kullanıcı ekleme formu grubu
            this.txtKulAd = new System.Windows.Forms.TextBox(); // Kullanıcı adı giriş alanı
            this.txtKulSoyad = new System.Windows.Forms.TextBox(); // Kullanıcı soyadı giriş alanı
            this.txtKulEmail = new System.Windows.Forms.TextBox(); // Kullanıcı email giriş alanı - benzersiz olmalı
            this.txtKulSifre = new System.Windows.Forms.TextBox(); // Kullanıcı şifre giriş alanı - hash'lenerek saklanacak
            this.cmbKulRol = new System.Windows.Forms.ComboBox(); // Kullanıcı rolü seçici - Admin/Musteri
            this.btnKulEkle = new System.Windows.Forms.Button(); // Yeni kullanıcı ekleme butonu
            this.btnKulSil = new System.Windows.Forms.Button(); // Seçili kullanıcıyı silme butonu
            this.btnKulYenile = new System.Windows.Forms.Button(); // Kullanıcı listesini yenileme butonu
            
            // UÇAK YÖNETİMİ SEKMESİ BİLEŞENLERİ
            this.dgvUcaklar = new System.Windows.Forms.DataGridView(); // Uçak listesi tablosu - filo envanteri
            this.groupUcak = new System.Windows.Forms.GroupBox(); // Yeni uçak ekleme formu grubu
            this.txtUcakKodu = new System.Windows.Forms.TextBox(); // Uçak kodu giriş alanı - benzersiz tanımlayıcı
            this.txtUcakModel = new System.Windows.Forms.TextBox(); // Uçak modeli giriş alanı
            this.txtKoltukSayisi = new System.Windows.Forms.TextBox(); // Koltuk sayısı giriş alanı - kapasite bilgisi
            this.btnUcakEkle = new System.Windows.Forms.Button(); // Yeni uçak ekleme butonu
            this.btnUcakSil = new System.Windows.Forms.Button(); // Seçili uçağı silme butonu
            this.btnUcakYenile = new System.Windows.Forms.Button(); // Uçak listesini yenileme butonu
            
            // UÇUŞ YÖNETİMİ SEKMESİ BİLEŞENLERİ
            this.dgvUcuslar = new System.Windows.Forms.DataGridView(); // Uçuş listesi tablosu - tüm uçuş planları
            this.groupUcus = new System.Windows.Forms.GroupBox(); // Yeni uçuş ekleme formu grubu
            this.cmbUcusUcak = new System.Windows.Forms.ComboBox(); // Uçak seçici - mevcut uçaklardan seçim
            this.txtUcusKodu = new System.Windows.Forms.TextBox(); // Uçuş kodu giriş alanı - benzersiz tanımlayıcı
            this.txtNereden = new System.Windows.Forms.TextBox(); // Kalkış noktası giriş alanı
            this.txtNereye = new System.Windows.Forms.TextBox(); // Varış noktası giriş alanı
            this.dtpKalkis = new System.Windows.Forms.DateTimePicker(); // Kalkış tarih-saat seçici
            this.dtpVaris = new System.Windows.Forms.DateTimePicker(); // Varış tarih-saat seçici
            this.txtFiyat = new System.Windows.Forms.TextBox(); // Uçuş fiyatı giriş alanı
            this.btnUcusEkle = new System.Windows.Forms.Button(); // Yeni uçuş ekleme butonu
            this.btnUcusSil = new System.Windows.Forms.Button(); // Seçili uçuşu silme butonu
            this.btnUcusYenile = new System.Windows.Forms.Button(); // Uçuş listesini yenileme butonu
            
            // BİLEŞENLERİN BAŞLATILMASI: Windows Forms bileşenlerini hazırlama süreci
            // BU BLOK: Kontrollerin bellek tahsisi ve ön ayarlarını yapar
            this.tabControl.SuspendLayout();
            this.tabKullanici.SuspendLayout();
            this.tabUcak.SuspendLayout();
            this.tabUcus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvKullanicilar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcaklar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcuslar)).BeginInit();
            this.groupKullanici.SuspendLayout();
            this.groupUcak.SuspendLayout();
            this.groupUcus.SuspendLayout();
            this.SuspendLayout();
            
            // HOŞGELDİN LABEL AYARLARI: Admin adını gösteren metin kutusu
            this.lblHosgeldin.AutoSize = true; // Metin boyutuna göre otomatik genişlik
            this.lblHosgeldin.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold); // Kalın yazı tipi - önem vurgusu
            this.lblHosgeldin.Location = new System.Drawing.Point(20, 15); // Sol üst köşede konumlandır - doğal okuma akışı
            this.lblHosgeldin.Name = "lblHosgeldin";
            this.lblHosgeldin.Size = new System.Drawing.Size(200, 21);
            this.lblHosgeldin.TabIndex = 0; // Tab sırasında ilk sırada - klavye navigasyonu için
            this.lblHosgeldin.Text = "Hoş geldiniz"; // Varsayılan metin - constructor'da güncellenecek
            
            // ÇIKIŞ BUTONU AYARLARI: Uygulamadan çıkmak için kırmızı buton
            this.btnCikis.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan - tehlike/çıkış işlemi için standart renk
            this.btnCikis.ForeColor = System.Drawing.Color.White; // Beyaz yazı rengi - yüksek kontrast
            this.btnCikis.Location = new System.Drawing.Point(920, 10); // Sağ üst köşede - erişilebilir konum
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(80, 30); // Sabit genişlik ve yükseklik - tutarlı UI
            this.btnCikis.TabIndex = 1; // Tab sırasında ikinci - hızlı erişim
            this.btnCikis.Text = "Çıkış"; // Buton metni - açık talimat
            this.btnCikis.UseVisualStyleBackColor = false; // Özel arkaplan rengini koru - tema tutarlılığı
            this.btnCikis.Click += new System.EventHandler(this.BtnCikis_Click); // Tıklama olayına metot bağla - olay yönlendirme
            
            // TAB CONTROL AYARLARI: Sekmeli arayüzün ana konteyneri
            this.tabControl.Location = new System.Drawing.Point(20, 50); // Başlığın altında konumlandır - hiyerarşik düzen
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0; // İlk sekmeyi varsayılan olarak seç - kullanıcı deneyimi
            this.tabControl.Size = new System.Drawing.Size(980, 530); // Formun büyük kısmını kapla - maksimum çalışma alanı
            this.tabControl.TabIndex = 2; // Tab sırasında üçüncü - mantıksal sıralama
            
            // KULLANICI YÖNETİMİ SEKMESİ AYARLARI
            this.tabKullanici.Controls.Add(this.dgvKullanicilar); // Kullanıcı listesi tablosunu ekle
            this.tabKullanici.Controls.Add(this.groupKullanici); // Kullanıcı ekleme grubunu ekle
            this.tabKullanici.Controls.Add(this.btnKulSil); // Sil butonunu ekle
            this.tabKullanici.Controls.Add(this.btnKulYenile); // Yenile butonunu ekle
            this.tabKullanici.Location = new System.Drawing.Point(4, 24); // Sekme konumu
            this.tabKullanici.Name = "tabKullanici";
            this.tabKullanici.Padding = new System.Windows.Forms.Padding(3); // İç kenar boşluğu - görsel rahatlık
            this.tabKullanici.Size = new System.Drawing.Size(972, 502); // Sekme boyutu
            this.tabKullanici.TabIndex = 0; // İlk sekme - öncelikli işlev
            this.tabKullanici.Text = "Kullanıcı Yönetimi"; // Sekme başlığı - işlevi açıkça belirtir
            this.tabKullanici.UseVisualStyleBackColor = true; // Varsayılan stil - sistem teması
            
            // KULLANICI EKLEME GRUBU AYARLARI: Yeni kullanıcı oluşturma formu
            this.groupKullanici.Location = new System.Drawing.Point(15, 15); // Sekme içinde konum
            this.groupKullanici.Name = "groupKullanici";
            this.groupKullanici.Size = new System.Drawing.Size(940, 120); // Sabit yükseklik - tutarlı layout
            this.groupKullanici.TabIndex = 0; // İlk focus alanı - kullanıcı akışı
            this.groupKullanici.TabStop = false; // Tab ile seçilemez - gruplar seçilemez elemanlardır
            this.groupKullanici.Text = "Yeni Kullanıcı Ekle"; // Grup kutusu başlığı - işlev tanımı
            
            // ETİKETLERİ DİZİ İLE OLUŞTURMA: Form alanları için açıklayıcı etiketler
            // BU YAKLAŞIM: Kod tekrarını önler ve merkezi kontrol sağlar
            var lbls = new Label[] {
                new Label { Text = "Ad:", Location = new System.Drawing.Point(20, 35), AutoSize = true }, // Ad etiketi
                new Label { Text = "Soyad:", Location = new System.Drawing.Point(150, 35), AutoSize = true }, // Soyad etiketi
                new Label { Text = "Email:", Location = new System.Drawing.Point(280, 35), AutoSize = true }, // Email etiketi
                new Label { Text = "Şifre:", Location = new System.Drawing.Point(480, 35), AutoSize = true }, // Şifre etiketi
                new Label { Text = "Rol:", Location = new System.Drawing.Point(610, 35), AutoSize = true } // Rol etiketi
            };
            this.groupKullanici.Controls.AddRange(lbls); // Tüm etiketleri gruba ekle - toplu işlem
            
            // KULLANICI ADI GİRİŞ ALANI
            this.txtKulAd.Location = new System.Drawing.Point(20, 55); // Etiketin altında konum - ilişkisel düzen
            this.txtKulAd.Size = new System.Drawing.Size(120, 23); // Sabit genişlik - tutarlı UI
            this.groupKullanici.Controls.Add(this.txtKulAd); // Gruba ekle
            
            // KULLANICI SOYADI GİRİŞ ALANI
            this.txtKulSoyad.Location = new System.Drawing.Point(150, 55); // Yan yana konumlandır - kompakt layout
            this.txtKulSoyad.Size = new System.Drawing.Size(120, 23); // Aynı genişlik - simetri
            this.groupKullanici.Controls.Add(this.txtKulSoyad); // Gruba ekle
            
            // EMAIL GİRİŞ ALANI
            this.txtKulEmail.Location = new System.Drawing.Point(280, 55); // Diğer alanların yanında
            this.txtKulEmail.Size = new System.Drawing.Size(190, 23); // Daha geniş - email adresleri uzun olabilir
            this.groupKullanici.Controls.Add(this.txtKulEmail); // Gruba ekle
            
            // ŞİFRE GİRİŞ ALANI
            this.txtKulSifre.Location = new System.Drawing.Point(480, 55); // Yan yana konumlandır
            this.txtKulSifre.Size = new System.Drawing.Size(120, 23); // Sabit genişlik
            this.groupKullanici.Controls.Add(this.txtKulSifre); // Gruba ekle
            
            // ROL SEÇİCİ COMBOBOX: Kullanıcı rolü belirleme
            this.cmbKulRol.DropDownStyle = ComboBoxStyle.DropDownList; // Sadece listeden seçim - geçersiz girişi önler
            this.cmbKulRol.Items.AddRange(new object[] { "Admin", "Musteri" }); // Rol seçenekleri - sistem yetkileri
            this.cmbKulRol.Location = new System.Drawing.Point(610, 55); // Diğer alanların yanında
            this.cmbKulRol.Size = new System.Drawing.Size(100, 23); // Sabit genişlik
            this.cmbKulRol.SelectedIndex = 1; // Varsayılan olarak "Musteri" seçili - güvenlik önlemi
            this.groupKullanici.Controls.Add(this.cmbKulRol); // Gruba ekle
            
            // KULLANICI EKLE BUTONU: Form verilerini veritabanına kaydeder
            this.btnKulEkle.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Yeşil arkaplan - olumlu/ekleme işlemi
            this.btnKulEkle.ForeColor = System.Drawing.Color.White; // Beyaz yazı - yüksek kontrast
            this.btnKulEkle.Location = new System.Drawing.Point(720, 50); // Form alanlarının sonunda - mantıksal akış
            this.btnKulEkle.Size = new System.Drawing.Size(200, 30); // Geniş buton - kolay tıklanabilirlik
            this.btnKulEkle.Text = "Ekle"; // Buton metni - açık işlev
            this.btnKulEkle.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnKulEkle.Click += new System.EventHandler(this.BtnKulEkle_Click); // Tıklama olayını bağla
            this.groupKullanici.Controls.Add(this.btnKulEkle); // Gruba ekle
            
            // KULLANICI LİSTESİNİ YENİLE BUTONU: Veritabanından güncel verileri çeker
            this.btnKulYenile.BackColor = System.Drawing.Color.FromArgb(52, 152, 219); // Mavi arkaplan - bilgi/yenileme işlemi
            this.btnKulYenile.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnKulYenile.Location = new System.Drawing.Point(15, 150); // Tablonun üstünde solda - mantıksal gruplama
            this.btnKulYenile.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnKulYenile.Text = "Yenile"; // Buton metni
            this.btnKulYenile.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnKulYenile.Click += new System.EventHandler(this.BtnKulYenile_Click); // Tıklama olayını bağla
            
            // KULLANICI SİL BUTONU: Seçili kullanıcıyı veritabanından kaldırır
            this.btnKulSil.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan - tehlike/silme işlemi
            this.btnKulSil.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnKulSil.Location = new System.Drawing.Point(130, 150); // Yenile butonunun yanında - işlevsel gruplama
            this.btnKulSil.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnKulSil.Text = "Sil"; // Buton metni
            this.btnKulSil.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnKulSil.Click += new System.EventHandler(this.BtnKulSil_Click); // Tıklama olayını bağla
            
            // KULLANICI LİSTESİ TABLOSU AYARLARI: Tüm kullanıcıları grid'de gösterir
            this.dgvKullanicilar.AllowUserToAddRows = false; // Kullanıcı yeni satır ekleyemez - veri bütünlüğü
            this.dgvKullanicilar.AllowUserToDeleteRows = false; // Kullanıcı satır silemez - kontrollü silme
            this.dgvKullanicilar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Sütunları eşit dağıt - optimal alan kullanımı
            this.dgvKullanicilar.Location = new System.Drawing.Point(15, 200); // Butonların altında - hiyerarşik düzen
            this.dgvKullanicilar.ReadOnly = true; // Sadece okunabilir mod - doğrudan düzenlemeyi önler
            this.dgvKullanicilar.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi - kullanıcı deneyimi
            this.dgvKullanicilar.Size = new System.Drawing.Size(940, 285); // Büyük alan kapla - maksimum veri görünürlüğü
            
            // UÇAK YÖNETİMİ SEKMESİ AYARLARI
            this.tabUcak.Controls.Add(this.dgvUcaklar); // Uçak listesi tablosunu ekle
            this.tabUcak.Controls.Add(this.groupUcak); // Uçak ekleme grubunu ekle
            this.tabUcak.Controls.Add(this.btnUcakSil); // Sil butonunu ekle
            this.tabUcak.Controls.Add(this.btnUcakYenile); // Yenile butonunu ekle
            this.tabUcak.Location = new System.Drawing.Point(4, 24); // Sekme konumu
            this.tabUcak.Name = "tabUcak";
            this.tabUcak.Size = new System.Drawing.Size(972, 502); // Sekme boyutu
            this.tabUcak.TabIndex = 1; // İkinci sekme - ikincil öncelik
            this.tabUcak.Text = "Uçak Yönetimi"; // Sekme başlığı
            this.tabUcak.UseVisualStyleBackColor = true; // Varsayılan stil
            
            // UÇAK EKLEME GRUBU AYARLARI
            this.groupUcak.Location = new System.Drawing.Point(15, 15); // Sekme içinde konum
            this.groupUcak.Size = new System.Drawing.Size(940, 120); // Sabit yükseklik
            this.groupUcak.TabStop = false; // Tab ile seçilemez
            this.groupUcak.Text = "Yeni Uçak Ekle"; // Grup başlığı
            
            // UÇAK FORM ETİKETLERİ
            var lblsUcak = new Label[] {
                new Label { Text = "Uçak Kodu:", Location = new System.Drawing.Point(20, 35), AutoSize = true }, // Benzersiz tanımlayıcı
                new Label { Text = "Model:", Location = new System.Drawing.Point(200, 35), AutoSize = true }, // Uçak model bilgisi
                new Label { Text = "Koltuk Sayısı:", Location = new System.Drawing.Point(450, 35), AutoSize = true } // Kapasite bilgisi
            };
            this.groupUcak.Controls.AddRange(lblsUcak); // Etiketleri gruba ekle
            
            // UÇAK KODU GİRİŞ ALANI
            this.txtUcakKodu.Location = new System.Drawing.Point(20, 55); // Etiketin altında konum
            this.txtUcakKodu.Size = new System.Drawing.Size(170, 23); // Sabit genişlik
            this.groupUcak.Controls.Add(this.txtUcakKodu); // Gruba ekle
            
            // UÇAK MODEL GİRİŞ ALANI
            this.txtUcakModel.Location = new System.Drawing.Point(200, 55); // Yan yana konumlandır
            this.txtUcakModel.Size = new System.Drawing.Size(240, 23); // Daha geniş - model isimleri uzun olabilir
            this.groupUcak.Controls.Add(this.txtUcakModel); // Gruba ekle
            
            // KOLTUK SAYISI GİRİŞ ALANI
            this.txtKoltukSayisi.Location = new System.Drawing.Point(450, 55); // Yan yana konumlandır
            this.txtKoltukSayisi.Size = new System.Drawing.Size(120, 23); // Sabit genişlik
            this.groupUcak.Controls.Add(this.txtKoltukSayisi); // Gruba ekle
            
            // UÇAK EKLE BUTONU
            this.btnUcakEkle.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Yeşil arkaplan
            this.btnUcakEkle.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcakEkle.Location = new System.Drawing.Point(580, 50); // Form alanlarının sonunda
            this.btnUcakEkle.Size = new System.Drawing.Size(340, 30); // Geniş buton
            this.btnUcakEkle.Text = "Uçak Ekle"; // Buton metni
            this.btnUcakEkle.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcakEkle.Click += new System.EventHandler(this.BtnUcakEkle_Click); // Tıklama olayını bağla
            this.groupUcak.Controls.Add(this.btnUcakEkle); // Gruba ekle
            
            // UÇAK LİSTESİNİ YENİLE BUTONU
            this.btnUcakYenile.BackColor = System.Drawing.Color.FromArgb(52, 152, 219); // Mavi arkaplan
            this.btnUcakYenile.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcakYenile.Location = new System.Drawing.Point(15, 150); // Tablonun üstünde solda
            this.btnUcakYenile.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnUcakYenile.Text = "Yenile"; // Buton metni
            this.btnUcakYenile.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcakYenile.Click += new System.EventHandler(this.BtnUcakYenile_Click); // Tıklama olayını bağla
            
            // UÇAK SİL BUTONU
            this.btnUcakSil.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan
            this.btnUcakSil.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcakSil.Location = new System.Drawing.Point(130, 150); // Yenile butonunun yanında
            this.btnUcakSil.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnUcakSil.Text = "Sil"; // Buton metni
            this.btnUcakSil.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcakSil.Click += new System.EventHandler(this.BtnUcakSil_Click); // Tıklama olayını bağla
            
            // UÇAK LİSTESİ TABLOSU AYARLARI
            this.dgvUcaklar.AllowUserToAddRows = false; // Yeni satır eklenemez
            this.dgvUcaklar.AllowUserToDeleteRows = false; // Satır silinemez
            this.dgvUcaklar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Eşit sütun genişliği
            this.dgvUcaklar.Location = new System.Drawing.Point(15, 200); // Butonların altında
            this.dgvUcaklar.ReadOnly = true; // Sadece okunabilir
            this.dgvUcaklar.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            this.dgvUcaklar.Size = new System.Drawing.Size(940, 285); // Büyük alan kapla
            
            // UÇUŞ YÖNETİMİ SEKMESİ AYARLARI
            this.tabUcus.Controls.Add(this.dgvUcuslar); // Uçuş listesi tablosunu ekle
            this.tabUcus.Controls.Add(this.groupUcus); // Uçuş ekleme grubunu ekle
            this.tabUcus.Controls.Add(this.btnUcusSil); // Sil butonunu ekle
            this.tabUcus.Controls.Add(this.btnUcusYenile); // Yenile butonunu ekle
            this.tabUcus.Location = new System.Drawing.Point(4, 24); // Sekme konumu
            this.tabUcus.Name = "tabUcus";
            this.tabUcus.Size = new System.Drawing.Size(972, 502); // Sekme boyutu
            this.tabUcus.TabIndex = 2; // Üçüncü sekme
            this.tabUcus.Text = "Uçuş Yönetimi"; // Sekme başlığı
            this.tabUcus.UseVisualStyleBackColor = true; // Varsayılan stil
            
            // UÇUŞ EKLEME GRUBU AYARLARI - daha karmaşık form olduğu için daha yüksek
            this.groupUcus.Location = new System.Drawing.Point(15, 15); // Sekme içinde konum
            this.groupUcus.Size = new System.Drawing.Size(940, 150); // Daha yüksek - daha fazla alan gerekli
            this.groupUcus.TabStop = false; // Tab ile seçilemez
            this.groupUcus.Text = "Yeni Uçuş Ekle"; // Grup başlığı
            
            // UÇUŞ FORM ETİKETLERİ - iki satırlı düzen
            var lblsUcus = new Label[] {
                new Label { Text = "Uçak:", Location = new System.Drawing.Point(20, 30), AutoSize = true }, // Uçak seçimi
                new Label { Text = "Uçuş Kodu:", Location = new System.Drawing.Point(250, 30), AutoSize = true }, // Uçuş tanımlayıcı
                new Label { Text = "Nereden:", Location = new System.Drawing.Point(20, 75), AutoSize = true }, // Kalkış noktası
                new Label { Text = "Nereye:", Location = new System.Drawing.Point(180, 75), AutoSize = true }, // Varış noktası
                new Label { Text = "Kalkış:", Location = new System.Drawing.Point(340, 75), AutoSize = true }, // Kalkış zamanı
                new Label { Text = "Varış:", Location = new System.Drawing.Point(570, 75), AutoSize = true }, // Varış zamanı
                new Label { Text = "Fiyat (TL):", Location = new System.Drawing.Point(800, 75), AutoSize = true }, // Ücret bilgisi
                new Label { Text = "Sezon:", Location = new Point(420, 30), AutoSize = true } // Sezon tipi 
            };
            this.groupUcus.Controls.AddRange(lblsUcus); // Etiketleri gruba ekle
            
            // UÇAK SEÇİCİ COMBOBOX: Mevcut uçaklardan seçim yapar
            this.cmbUcusUcak.DropDownStyle = ComboBoxStyle.DropDownList; // Sadece listeden seçim
            this.cmbUcusUcak.Location = new System.Drawing.Point(20, 50); // İlk satırda konum
            this.cmbUcusUcak.Size = new System.Drawing.Size(220, 23); // Sabit genişlik
            this.groupUcus.Controls.Add(this.cmbUcusUcak); // Gruba ekle
            
            // UÇUŞ KODU GİRİŞ ALANI
            this.txtUcusKodu.Location = new System.Drawing.Point(250, 50); // İlk satırda yan yana
            this.txtUcusKodu.Size = new System.Drawing.Size(150, 23); // Sabit genişlik
            this.groupUcus.Controls.Add(this.txtUcusKodu); // Gruba ekle

            // SEZON TİPİ SEÇİM ALANI   
            this.cmbSezonTipi = new System.Windows.Forms.ComboBox();
            this.cmbSezonTipi.DropDownStyle = ComboBoxStyle.DropDownList;
            // ComboBox'a sezon tipi seçenekleri, fiyatlandırmada kullanılacak 3 farklı sezon tipi
            this.cmbSezonTipi.Items.AddRange(new object[] {  
                "Normal Sezon",   // Standart fiyat uygulanacak sezon
                "Yüksek Sezon (+%20)",  // Fiyat %20 artırılacak sezon (yaz tatili, bayram vb.)
                "Düşük Sezon (-%10)"   // Fiyat %10 düşürülecek sezon (düşük talep dönemi)
            });
            this.cmbSezonTipi.Location = new System.Drawing.Point(420, 50); //Konumu
            this.cmbSezonTipi.Size = new System.Drawing.Size(150, 23); // Sabit genişlik
            this.cmbSezonTipi.SelectedIndex = 0; // Default olarak Normal seçili
            this.groupUcus.Controls.Add(this.cmbSezonTipi); //Gruba ekle
            
            // KALKIŞ NOKTASI GİRİŞ ALANI
            this.txtNereden.Location = new System.Drawing.Point(20, 95); // İkinci satırda konum
            this.txtNereden.Size = new System.Drawing.Size(150, 23); // Sabit genişlik
            this.groupUcus.Controls.Add(this.txtNereden); // Gruba ekle
            
            // VARIŞ NOKTASI GİRİŞ ALANI
            this.txtNereye.Location = new System.Drawing.Point(180, 95); // İkinci satırda yan yana
            this.txtNereye.Size = new System.Drawing.Size(150, 23); // Sabit genişlik
            this.groupUcus.Controls.Add(this.txtNereye); // Gruba ekle
            
            // KALKIŞ TARİH-SAAT SEÇİCİ
            this.dtpKalkis.Format = DateTimePickerFormat.Custom; // Özel format kullan
            this.dtpKalkis.CustomFormat = "dd.MM.yyyy HH:mm"; // Türk formatı - gün.ay.yıl saat:dakika
            this.dtpKalkis.Location = new System.Drawing.Point(340, 95); // İkinci satırda konum
            this.dtpKalkis.Size = new System.Drawing.Size(220, 23); // Geniş - tarih görüntüsü için
            this.groupUcus.Controls.Add(this.dtpKalkis); // Gruba ekle
            
            // VARIŞ TARİH-SAAT SEÇİCİ
            this.dtpVaris.Format = DateTimePickerFormat.Custom; // Özel format kullan
            this.dtpVaris.CustomFormat = "dd.MM.yyyy HH:mm"; // Türk formatı
            this.dtpVaris.Location = new System.Drawing.Point(570, 95); // İkinci satırda yan yana
            this.dtpVaris.Size = new System.Drawing.Size(220, 23); // Geniş - tarih görüntüsü için
            this.groupUcus.Controls.Add(this.dtpVaris); // Gruba ekle
            
            // FİYAT GİRİŞ ALANI
            this.txtFiyat.Location = new System.Drawing.Point(800, 95); // İkinci satırda son eleman
            this.txtFiyat.Size = new System.Drawing.Size(120, 23); // Sabit genişlik
            this.groupUcus.Controls.Add(this.txtFiyat); // Gruba ekle
            
            // UÇUŞ EKLE BUTONU
            this.btnUcusEkle.BackColor = System.Drawing.Color.FromArgb(46, 204, 113); // Yeşil arkaplan
            this.btnUcusEkle.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcusEkle.Location = new System.Drawing.Point(420, 50); // İlk satırda geniş alan
            this.btnUcusEkle.Size = new System.Drawing.Size(500, 30); // Çok geniş buton
            this.btnUcusEkle.Text = "Uçuş Ekle"; // Buton metni
            this.btnUcusEkle.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcusEkle.Click += new System.EventHandler(this.BtnUcusEkle_Click); // Tıklama olayını bağla
            this.groupUcus.Controls.Add(this.btnUcusEkle); // Gruba ekle
            
            // UÇUŞ LİSTESİNİ YENİLE BUTONU
            this.btnUcusYenile.BackColor = System.Drawing.Color.FromArgb(52, 152, 219); // Mavi arkaplan
            this.btnUcusYenile.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcusYenile.Location = new System.Drawing.Point(15, 180); // Grubun altında solda
            this.btnUcusYenile.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnUcusYenile.Text = "Yenile"; // Buton metni
            this.btnUcusYenile.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcusYenile.Click += new System.EventHandler(this.BtnUcusYenile_Click); // Tıklama olayını bağla
            
            // UÇUŞ SİL BUTONU
            this.btnUcusSil.BackColor = System.Drawing.Color.FromArgb(231, 76, 60); // Kırmızı arkaplan
            this.btnUcusSil.ForeColor = System.Drawing.Color.White; // Beyaz yazı
            this.btnUcusSil.Location = new System.Drawing.Point(130, 180); // Yenile butonunun yanında
            this.btnUcusSil.Size = new System.Drawing.Size(100, 35); // Standart buton boyutu
            this.btnUcusSil.Text = "Sil"; // Buton metni
            this.btnUcusSil.UseVisualStyleBackColor = false; // Özel rengi koru
            this.btnUcusSil.Click += new System.EventHandler(this.BtnUcusSil_Click); // Tıklama olayını bağla
            
            // UÇUŞ LİSTESİ TABLOSU AYARLARI
            this.dgvUcuslar.AllowUserToAddRows = false; // Yeni satır eklenemez
            this.dgvUcuslar.AllowUserToDeleteRows = false; // Satır silinemez
            this.dgvUcuslar.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Eşit sütun genişliği
            this.dgvUcuslar.Location = new System.Drawing.Point(15, 230); // Butonların altında
            this.dgvUcuslar.ReadOnly = true; // Sadece okunabilir
            this.dgvUcuslar.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            this.dgvUcuslar.Size = new System.Drawing.Size(940, 255); // Büyük alan kapla
            
            // SEKMELERİ ANA KONTROLE EKLEME: Tüm sekmeleri tabControl'e bağla
            this.tabControl.Controls.Add(this.tabKullanici); // Kullanıcı yönetimi sekmesini ekle
            this.tabControl.Controls.Add(this.tabUcak); // Uçak yönetimi sekmesini ekle
            this.tabControl.Controls.Add(this.tabUcus); // Uçuş yönetimi sekmesini ekle
            
            // ANA FORM AYARLARI
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F); // Otomatik ölçekleme boyutu
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font; // Yazı tipine göre ölçekle - tutarlı UI
            this.ClientSize = new System.Drawing.Size(1020, 600); // Form genişlik ve yükseklik - admin paneli için geniş alan
            this.Controls.Add(this.tabControl); // Sekmeli yapıyı forma ekle
            this.Controls.Add(this.btnCikis); // Çıkış butonunu forma ekle
            this.Controls.Add(this.lblHosgeldin); // Hoşgeldin yazısını forma ekle
            this.Name = "AdminPanel"; // Form nesne adı
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Ekran ortasında aç - kullanıcı deneyimi
            this.Text = "Admin Yönetim Paneli"; // Form başlık çubuğu metni - işlev tanımı
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdminPanel_FormClosing); // Form kapanma olayı - kaynak temizleme
            
            // BİLEŞEN BAŞLATMA İŞLEMLERİNİ TAMAMLA: Windows Forms yaşam döngüsü
            this.tabControl.ResumeLayout(false);
            this.tabKullanici.ResumeLayout(false);
            this.tabUcak.ResumeLayout(false);
            this.tabUcus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvKullanicilar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcaklar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUcuslar)).EndInit();
            this.groupKullanici.ResumeLayout(false);
            this.groupKullanici.PerformLayout();
            this.groupUcak.ResumeLayout(false);
            this.groupUcak.PerformLayout();
            this.groupUcus.ResumeLayout(false);
            this.groupUcus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // DEĞİŞKEN TANIMLARI: Form bileşenlerini sınıf seviyesinde tanımla
        // BU TANIMLAR: Designer tarafından otomatik oluşturulur ve manuel düzenlenebilir
        private TabControl tabControl; // Sekmeli arayüz kontrolü
        private TabPage tabKullanici, tabUcak, tabUcus; // Sekme sayfaları
        private Label lblHosgeldin; // Hoşgeldin mesajı
        private Button btnCikis; // Çıkış butonu
        
        // Kullanıcı yönetimi kontrolleri
        private DataGridView dgvKullanicilar; // Kullanıcı veri tablosu
        private GroupBox groupKullanici; // Kullanıcı ekleme form grubu
        private TextBox txtKulAd, txtKulSoyad, txtKulEmail, txtKulSifre; // Kullanıcı bilgi girişleri
        private ComboBox cmbKulRol; // Rol seçici
        private Button btnKulEkle, btnKulSil, btnKulYenile; // Kullanıcı işlem butonları
        
        // Uçak yönetimi kontrolleri
        private DataGridView dgvUcaklar; // Uçak veri tablosu
        private GroupBox groupUcak; // Uçak ekleme form grubu
        private TextBox txtUcakKodu, txtUcakModel, txtKoltukSayisi; // Uçak bilgi girişleri
        private Button btnUcakEkle, btnUcakSil, btnUcakYenile; // Uçak işlem butonları
        
        // Uçuş yönetimi kontrolleri
        private DataGridView dgvUcuslar; // Uçuş veri tablosu
        private GroupBox groupUcus; // Uçuş ekleme form grubu
        private ComboBox cmbUcusUcak; // Uçak seçici
        private TextBox txtUcusKodu, txtNereden, txtNereye, txtFiyat; // Uçuş bilgi girişleri
        private DateTimePicker dtpKalkis, dtpVaris; // Tarih seçiciler
        private ComboBox cmbSezonTipi; // Sezon tipi seçici
        private Button btnUcusEkle, btnUcusSil, btnUcusYenile; // Uçuş işlem butonları

        //KULLANICI YÖNETİMİ METOTLARI
        
        // KULLANICILARI YÜKLEME: Veritabanından tüm kullanıcıları çeker ve listeler
        private void KullanicilariYukle()
        {
            try
            {
                // SQL SORGUSU: Tüm kullanıcı bilgilerini tarihe göre sıralı getir
                string query = "SELECT KullaniciID, Ad, Soyad, Email, Rol, OlusturmaTarihi FROM Kullanicilar ORDER BY OlusturmaTarihi DESC";
                dgvKullanicilar.DataSource = db.ExecuteQuery(query); // Sorguyu çalıştır ve tabloya bağla
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Veritabanı bağlantı veya sorgu hatalarını yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // KULLANICI EKLEME BUTONU: Form verilerini alarak yeni kullanıcı oluşturur
        private void BtnKulEkle_Click(object sender, EventArgs e)
        {
            try
            {
                // GİRDİ DOĞRULAMA: Zorunlu alanların dolu olduğunu kontrol et
                if (string.IsNullOrWhiteSpace(txtKulAd.Text) || string.IsNullOrWhiteSpace(txtKulSoyad.Text) ||
                    string.IsNullOrWhiteSpace(txtKulEmail.Text) || string.IsNullOrWhiteSpace(txtKulSifre.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur - eksik bilgi
                }

                // EMAIL KONTROLÜ: Benzersiz email adresi kontrolü yap
                string kontrolQuery = "SELECT COUNT(*) FROM Kullanicilar WHERE Email = @Email";
                SqlParameter[] kontrolParams = { new SqlParameter("@Email", txtKulEmail.Text.Trim()) };
                DataTable dtKontrol = db.ExecuteQuery(kontrolQuery, kontrolParams);
                
                // EĞER EMAIL KAYITLIYSA: Kullanıcıyı uyar ve işlemi durdur
                if (Convert.ToInt32(dtKontrol.Rows[0][0]) > 0)
                {
                    MessageBox.Show("Bu email adresi zaten kayıtlı!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur - çakışma
                }

                // VERİTABANI EKLEME SORGUSU: Yeni kullanıcıyı veritabanına ekle
                string query = "INSERT INTO Kullanicilar (Ad, Soyad, Email, Sifre, Rol) VALUES (@Ad, @Soyad, @Email, @Sifre, @Rol)";
                SqlParameter[] parameters = {
                    new SqlParameter("@Ad", txtKulAd.Text.Trim()), // Ad parametresi
                    new SqlParameter("@Soyad", txtKulSoyad.Text.Trim()), // Soyad parametresi
                    new SqlParameter("@Email", txtKulEmail.Text.Trim()), // Email parametresi
                    new SqlParameter("@Sifre", txtKulSifre.Text), // Şifre parametresi (production'da hash'lenmeli)
                    new SqlParameter("@Rol", cmbKulRol.SelectedItem.ToString()) // Rol parametresi
                };

                db.ExecuteNonQuery(query, parameters); // Sorguyu çalıştır
                MessageBox.Show("Kullanıcı başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // FORM TEMİZLEME: Giriş alanlarını sıfırla
                txtKulAd.Clear(); txtKulSoyad.Clear(); txtKulEmail.Clear(); txtKulSifre.Clear();
                cmbKulRol.SelectedIndex = 1; // Varsayılan değere dön
                
                KullanicilariYukle(); // Listeyi güncelle
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Ekleme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // KULLANICI SİLME BUTONU: Seçili kullanıcıyı veritabanından kaldırır
        private void BtnKulSil_Click(object sender, EventArgs e)
        {
            try
            {
                // SEÇİM KONTROLÜ: Kullanıcının bir satır seçtiğinden emin ol
                if (dgvKullanicilar.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek kullanıcıyı seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SEÇİLİ KULLANICI BİLGİLERİNİ AL
                int kullaniciID = Convert.ToInt32(dgvKullanicilar.SelectedRows[0].Cells["KullaniciID"].Value); // Gizli ID
                string rol = dgvKullanicilar.SelectedRows[0].Cells["Rol"].Value.ToString(); // Kullanıcı rolü

                // GÜVENLİK KONTROLÜ: Admin kullanıcılarının silinmesini önle
                if (rol == "Admin")
                {
                    MessageBox.Show("Admin kullanıcısı silinemez!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur - güvenlik önlemi
                }

                // ONAY DİYALOĞU: Kullanıcıdan silme işlemini onaylamasını iste
                var sonuc = MessageBox.Show("Kullanıcıyı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                // ONAY VERİLDİYSE SİLME İŞLEMİNİ GERÇEKLEŞTİR
                if (sonuc == DialogResult.Yes)
                {
                    string query = "DELETE FROM Kullanicilar WHERE KullaniciID = @KullaniciID";
                    db.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@KullaniciID", kullaniciID) });
                    MessageBox.Show("Kullanıcı silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    KullanicilariYukle(); // Listeyi güncelle
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Silme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // KULLANICI LİSTESİNİ YENİLEME: Veritabanından güncel verileri çeker
        private void BtnKulYenile_Click(object sender, EventArgs e)
        {
            KullanicilariYukle(); // Kullanıcı listesini yeniden yükle
        }

        // UÇAK YÖNETİMİ METOTLARI 
        
        // UÇAKLARI YÜKLEME: Veritabanından tüm uçakları çeker ve listeler
        private void UcaklariYukle()
        {
            try
            {
                // SQL SORGUSU: Tüm uçak bilgilerini ID'ye göre sıralı getir
                string query = "SELECT UcakID, UcakKodu, Model, KoltukSayisi FROM Ucaklar ORDER BY UcakID DESC";
                dgvUcaklar.DataSource = db.ExecuteQuery(query); // Sorguyu çalıştır ve tabloya bağla
                
                // COMBOBOX GÜNCELLEME: Uçuş ekleme formu için uçak listesini hazırla
                cmbUcusUcak.Items.Clear(); // Önce mevcut listeyi temizle
                DataTable dt = db.ExecuteQuery("SELECT UcakID, UcakKodu, Model FROM Ucaklar WHERE AktifMi = 1");
                
                // HER UÇAK İÇİN: Combobox'a formatlı şekilde ekle
                foreach (DataRow row in dt.Rows)
                {
                    cmbUcusUcak.Items.Add($"{row["UcakKodu"]} - {row["Model"]}"); // "Kod - Model" formatı
                }
                
                // VARSayILAN SEÇİM: İlk uçağı seçili yap
                if (cmbUcusUcak.Items.Count > 0) cmbUcusUcak.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Veritabanı hatalarını yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇAK EKLEME BUTONU: Form verilerini alarak yeni uçak oluşturur
        private void BtnUcakEkle_Click(object sender, EventArgs e)
        {
            try
            {
                // GİRDİ DOĞRULAMA: Zorunlu alanların dolu olduğunu kontrol et
                if (string.IsNullOrWhiteSpace(txtUcakKodu.Text) || string.IsNullOrWhiteSpace(txtUcakModel.Text) || 
                    string.IsNullOrWhiteSpace(txtKoltukSayisi.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SAYISAL DOĞRULAMA: Koltuk sayısının geçerli bir sayı olduğunu kontrol et
                if (!int.TryParse(txtKoltukSayisi.Text, out int koltuk) || koltuk <= 0)
                {
                    MessageBox.Show("Geçerli bir koltuk sayısı girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // VERİTABANI EKLEME SORGUSU: Yeni uçağı veritabanına ekle
                string query = "INSERT INTO Ucaklar (UcakKodu, Model, KoltukSayisi) VALUES (@UcakKodu, @Model, @KoltukSayisi)";
                SqlParameter[] parameters = {
                    new SqlParameter("@UcakKodu", txtUcakKodu.Text.Trim()), // Uçak kodu parametresi
                    new SqlParameter("@Model", txtUcakModel.Text.Trim()), // Model parametresi
                    new SqlParameter("@KoltukSayisi", koltuk) // Koltuk sayısı parametresi
                };

                db.ExecuteNonQuery(query, parameters); // Sorguyu çalıştır
                MessageBox.Show("Uçak başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // FORM TEMİZLEME: Giriş alanlarını sıfırla
                txtUcakKodu.Clear(); txtUcakModel.Clear(); txtKoltukSayisi.Clear();
                
                UcaklariYukle(); // Listeyi güncelle
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Ekleme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇAK SİLME BUTONU: Seçili uçağı veritabanından kaldırır
        private void BtnUcakSil_Click(object sender, EventArgs e)
        {
            try
            {
                // SEÇİM KONTROLÜ: Kullanıcının bir satır seçtiğinden emin ol
                if (dgvUcaklar.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek uçağı seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SEÇİLİ UÇAK BİLGİLERİNİ AL
                int ucakID = Convert.ToInt32(dgvUcaklar.SelectedRows[0].Cells["UcakID"].Value); // Gizli ID

                // ONAY DİYALOĞU: Kullanıcıdan silme işlemini onaylamasını iste
                var sonuc = MessageBox.Show("Uçağı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                // ONAY VERİLDİYSE SİLME İŞLEMİNİ GERÇEKLEŞTİR
                if (sonuc == DialogResult.Yes)
                {
                    string query = "DELETE FROM Ucaklar WHERE UcakID = @UcakID";
                    db.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@UcakID", ucakID) });
                    MessageBox.Show("Uçak silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcaklariYukle(); // Listeyi güncelle
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Silme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇAK LİSTESİNİ YENİLEME: Veritabanından güncel verileri çeker
        private void BtnUcakYenile_Click(object sender, EventArgs e)
        {
            UcaklariYukle(); // Uçak listesini yeniden yükle
        }

        //UÇUŞ YÖNETİMİ METOTLARI 
        
        // UÇUŞLARI YÜKLEME: Veritabanından tüm uçuşları çeker ve listeler
        private void UcuslariYukle()
        {
            try
            {
                // SQL SORGUSU: Uçuş ve uçak bilgilerini birleştirerek getir
                string query = @"SELECT u.UcusID, u.UcusKodu, uc.UcakKodu, uc.Model, 
                       u.Nereden, u.Nereye, u.KalkisTarihi, u.VarisTarihi, 
                       u.Fiyat AS 'Liste Fiyatı (TL)', u.MusaitKoltuk,
                       CASE u.SezonTipi 
                           WHEN 0 THEN 'Normal'
                           WHEN 1 THEN 'Yüksek Sezon '
                           WHEN 2 THEN 'Düşük Sezon '
                       END AS 'Sezon',
                       CAST((uc.KoltukSayisi - u.MusaitKoltuk) * 100.0 / uc.KoltukSayisi AS DECIMAL(5,1)) AS 'Doluluk %'
                       FROM Ucuslar u 
                       INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID 
                       ORDER BY u.UcusID DESC";
                dgvUcuslar.DataSource = db.ExecuteQuery(query); // Sorguyu çalıştır ve tabloya bağla
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Veritabanı hatalarını yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇUŞ EKLEME BUTONU: Form verilerini alarak yeni uçuş oluşturur
        private void BtnUcusEkle_Click(object sender, EventArgs e)
        {
            try
            {
                // GİRDİ DOĞRULAMA: Tüm zorunlu alanların dolu olduğunu kontrol et
                if (cmbUcusUcak.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtUcusKodu.Text) ||
                    string.IsNullOrWhiteSpace(txtNereden.Text) || string.IsNullOrWhiteSpace(txtNereye.Text) ||
                    string.IsNullOrWhiteSpace(txtFiyat.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // FİYAT DOĞRULAMA: Fiyatın geçerli bir decimal sayı olduğunu kontrol et
                if (!decimal.TryParse(txtFiyat.Text, out decimal fiyat) || fiyat <= 0)
                {
                    MessageBox.Show("Geçerli bir fiyat girin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // TARİH DOĞRULAMA: Varış tarihinin kalkış tarihinden sonra olduğunu kontrol et
                if (dtpKalkis.Value >= dtpVaris.Value)
                {
                    MessageBox.Show("Varış tarihi, kalkış tarihinden sonra olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur - mantıksal hata
                }

                // UÇAK BİLGİSİNİ AL: Seçili uçağın ID ve kapasite bilgilerini getir
                string ucakKodu = cmbUcusUcak.SelectedItem.ToString().Split('-')[0].Trim(); // Combobox'tan kodu ayıkla
                string ucakQuery = "SELECT UcakID, KoltukSayisi FROM Ucaklar WHERE UcakKodu = @UcakKodu";
                DataTable dtUcak = db.ExecuteQuery(ucakQuery, new SqlParameter[] { new SqlParameter("@UcakKodu", ucakKodu) });
                
                // UÇAK KONTROLÜ: Seçilen uçağın veritabanında var olduğunu doğrula
                if (dtUcak.Rows.Count == 0)
                {
                    MessageBox.Show("Uçak bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // İşlemi durdur
                }

                // UÇAK BİLGİLERİNİ ÇEK: ID ve koltuk sayısını al
                int ucakID = Convert.ToInt32(dtUcak.Rows[0]["UcakID"]);
                int koltukSayisi = Convert.ToInt32(dtUcak.Rows[0]["KoltukSayisi"]);
                int sezonTipi = this.cmbSezonTipi.SelectedIndex; // 0=Normal, 1=Yüksek, 2=Düşük

                // VERİTABANI EKLEME SORGUSU: Yeni uçuşu veritabanına ekle
                string query = @"INSERT INTO Ucuslar (UcakID, UcusKodu, Nereden, Nereye, KalkisTarihi, VarisTarihi, Fiyat, TemelFiyat,MusaitKoltuk, SezonTipi) 
                               VALUES (@UcakID, @UcusKodu, @Nereden, @Nereye, @KalkisTarihi, @VarisTarihi, @Fiyat, @TemelFiyat, @MusaitKoltuk, @SezonTipi)";
                
                SqlParameter[] parameters = {
                    new SqlParameter("@UcakID", ucakID), // Uçak ID parametresi
                    new SqlParameter("@UcusKodu", txtUcusKodu.Text.Trim()), // Uçuş kodu parametresi
                    new SqlParameter("@Nereden", txtNereden.Text.Trim()), // Kalkış noktası parametresi
                    new SqlParameter("@Nereye", txtNereye.Text.Trim()), // Varış noktası parametresi
                    new SqlParameter("@KalkisTarihi", dtpKalkis.Value), // Kalkış zamanı parametresi
                    new SqlParameter("@VarisTarihi", dtpVaris.Value), // Varış zamanı parametresi
                    new SqlParameter("@Fiyat", fiyat), // Fiyat parametresi
                    new SqlParameter("@TemelFiyat", fiyat), // Temel fiyat parametresi
                    new SqlParameter("@MusaitKoltuk", koltukSayisi), // Müsait koltuk parametresi (başlangıçta tam kapasite)
                    new SqlParameter("@SezonTipi", sezonTipi) //Sezon tipi parametresi
                };

                db.ExecuteNonQuery(query, parameters); // Sorguyu çalıştır
                MessageBox.Show("Uçuş başarıyla eklendi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // FORM TEMİZLEME: Giriş alanlarını sıfırla
                txtUcusKodu.Clear(); txtNereden.Clear(); txtNereye.Clear(); txtFiyat.Clear();
                dtpKalkis.Value = DateTime.Now; // Varsayılan kalkış zamanı
                dtpVaris.Value = DateTime.Now.AddHours(2); // Varsayılan varış zamanı (+2 saat)
                
                UcuslariYukle(); // Listeyi güncelle
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Ekleme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇUŞ SİLME BUTONU: Seçili uçuşu veritabanından kaldırır
        private void BtnUcusSil_Click(object sender, EventArgs e)
        {
            try
            {
                // SEÇİM KONTROLÜ: Kullanıcının bir satır seçtiğinden emin ol
                if (dgvUcuslar.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek uçuşu seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // İşlemi durdur
                }

                // SEÇİLİ UÇUŞ BİLGİLERİNİ AL
                int ucusID = Convert.ToInt32(dgvUcuslar.SelectedRows[0].Cells["UcusID"].Value); // Gizli ID

                // ONAY DİYALOĞU: Kullanıcıdan silme işlemini onaylamasını iste
                var sonuc = MessageBox.Show("Uçuşu silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                // ONAY VERİLDİYSE SİLME İŞLEMİNİ GERÇEKLEŞTİR
                if (sonuc == DialogResult.Yes)
                {
                    string query = "DELETE FROM Ucuslar WHERE UcusID = @UcusID";
                    db.ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@UcusID", ucusID) });
                    MessageBox.Show("Uçuş silindi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcuslariYukle(); // Listeyi güncelle
                }
            }
            catch (Exception ex)
            {
                // HATA YÖNETİMİ: Silme işlemindeki hataları yakala
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // UÇUŞ LİSTESİNİ YENİLEME: Veritabanından güncel verileri çeker
        private void BtnUcusYenile_Click(object sender, EventArgs e)
        {
            UcuslariYukle(); // Uçuş listesini yeniden yükle
        }

        // ÇIKIŞ BUTONU: Uygulamadan tamamen çıkar
        private void BtnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Uygulamayı kapat - tüm formları kapatır
        }

        // FORM KAPANMA OLAYI: Pencere kapatıldığında uygulamayı sonlandır
        private void AdminPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); // Uygulamayı tamamen kapat - kaynakları serbest bırak
        }
    }
}
