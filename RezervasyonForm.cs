using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// REZERVASYON FORM SINIFI - BU SINIF REZERVASYON İŞLEMLERİNİ YÖNETİR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class RezervasyonForm : Form
    {
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db;
        private int ucusID;                 // ucusID: Hangi uçuş için rezervasyon yapılacak
        private int kullaniciID;            // kullaniciID: Hangi kullanıcı rezervasyon yapıyor
        private int koltukSayisi;           // koltukSayisi: Uçaktaki toplam koltuk sayısı
        private string seciliKoltuk = "";   // seciliKoltuk: Kullanıcının seçtiği koltuk numarası (varsayılan değer: boş string)
        private TextBox txtKuponKodu;      // Kupon kodu girişi için metin kutusu (indirim uygulamak için)
        private Button btnFiyatGoster;     // Fiyat hesaplama butonu (kullanıcı tıklayınca fiyatı gösterir)
        private Label lblHesaplananFiyat;   // Hesaplanan fiyatın gösterileceği etiket 
        private FiyatHesaplayici fiyatHesaplayici;  // Fiyat hesaplama işlemlerini yapan sınıfın örneği

        public RezervasyonForm(int ucusID, int kullaniciID) // Constructordır. Nesne oluşturulduğunda otomatik çalışan yapıcı metoddur. Parametrelidir, nesne oluşturulurken ucusID ve kullaniciID bilgilerini alır
        {
            InitializeComponent();// vs code tarafından otomatik oluşturulur, formdaki tüm kontrolleri hazırlar.
            /* Composition (Bileşim) oop özelliği kullanıldı. Başka bir sınıftan örnek alıyor.
               veritabanı bağlantısı için DatabaseHelper sınıfından yeni bir nesne oluşturuluyor ve db alanına atanıyor. */
            db = new DatabaseHelper();

            fiyatHesaplayici = new FiyatHesaplayici(); // FiytHesaplayıcı classının yeni nesnesi oluşturulur
            // Parametre değerleri sınıf değişkenlerine atanıyor
            // 'this' Sınıfın kendi üyesine işaret ederek parametre ile field'ı karışmasını engelliyor
            this.ucusID = ucusID;
            this.kullaniciID = kullaniciID;

            // Uçuş bilgilerini veritabanından yüklenmesi için metodu çağırılır
            UcusBilgisiYukle();
        }

        // Bu değişkenler formdaki görsel bileşenleri temsil eder.
        // Encapsulation oop yapısı kullanılmıştır
        // Encapsulation ile kontrollere sadece bu sınıfın içindeki metodlardan erişilebilir
        // Tüm kontrol değişkenleri private olarak tanımlandı
        private Label lblBaslik, lblUcusBilgi, lblKoltukNo;
        private GroupBox groupYolcu;
        private TextBox txtYolcuAd, txtYolcuSoyad, txtYolcuTC;
        private Button btnKoltukSec, btnOnayla, btnIptal;

        // Arayüz için gereken tüm form kontrollerini (buton, textbox, label) oluşturulduğu ve tasarımının yapıldığı kısım
        private void InitializeComponent()
        {
            // new ile kontrollere yeni nesneleri oluşturulur
            lblBaslik = new Label();      
            lblUcusBilgi = new Label();    
            groupYolcu = new GroupBox();    
            txtYolcuAd = new TextBox();    
            txtYolcuSoyad = new TextBox();
            txtYolcuTC = new TextBox();
            lblKoltukNo = new Label();
            btnKoltukSec = new Button();    
            btnOnayla = new Button();
            btnIptal = new Button();

            groupYolcu.SuspendLayout();   
            SuspendLayout();              

            // BAŞLIK ETİKETİ 
            lblBaslik.AutoSize = true;      // Metnin boyutuna göre otomatik boyutlanır
            lblBaslik.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblBaslik.Location = new System.Drawing.Point(140, 20);  // X=140, Y=20 koordinatlarında konumlandır
            lblBaslik.Name = "lblBaslik";   // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            lblBaslik.Size = new System.Drawing.Size(200, 25);
            lblBaslik.TabIndex = 0;   
            lblBaslik.Text = "Rezervasyon Bilgileri";  // Etiket üzerinde görünecek metin içeriği

            // UÇUŞ BİLGİ ETİKETİ
            lblUcusBilgi.AutoSize = true;
            lblUcusBilgi.Font = new System.Drawing.Font("Segoe UI", 10F);  
            lblUcusBilgi.Location = new System.Drawing.Point(30, 60);
            lblUcusBilgi.Name = "lblUcusBilgi";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            lblUcusBilgi.Size = new System.Drawing.Size(200, 19);
            lblUcusBilgi.TabIndex = 1;
            lblUcusBilgi.Text = "Uçuş bilgisi yükleniyor...";  // Etiket üzerinde görünecek metin içeriği

            // YOLCU (groupbox) ÖZELLİKLERİ
            groupYolcu.Location = new System.Drawing.Point(30, 100);
            groupYolcu.Name = "groupYolcu";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            groupYolcu.Size = new System.Drawing.Size(420, 450);
            groupYolcu.TabIndex = 2;
            groupYolcu.TabStop = false;   
            groupYolcu.Text = "Yolcu Bilgileri";   // Etiket üzerinde görünecek metin içeriği

            Label lbl1 = new Label();   // nesne oluşturulması, etikettir.
            lbl1.Text = "Yolcu Adı:";        // Etiket üzerinde görünecek metin içeriği
            lbl1.Location = new System.Drawing.Point(20, 35);
            lbl1.AutoSize = true;
            groupYolcu.Controls.Add(lbl1);  // Etiketi grup kutusunun içine eklenir (Parent-Child ilişkisi)

            Label lbl2 = new Label();       // nesne oluşturulması, etikettir.
            lbl2.Text = "Yolcu Soyadı:";   // Etiket üzerinde görünecek metin içeriği
            lbl2.Location = new System.Drawing.Point(20, 75);
            lbl2.AutoSize = true;
            groupYolcu.Controls.Add(lbl2);  // Etiketi grup kutusunun içine eklenir (Parent-Child ilişkisi)

            Label lbl3 = new Label();    // nesne oluşturulması, etikettir.
            lbl3.Text = "TC Kimlik No:";    // Etiket üzerinde görünecek metin içeriği
            lbl3.Location = new System.Drawing.Point(20, 115);
            lbl3.AutoSize = true;  
            groupYolcu.Controls.Add(lbl3);   // Etiketi grup kutusunun içine eklenir (Parent-Child ilişkisi)

            Label lbl4 = new Label();   // nesne oluşturulması, etikettir.
            lbl4.Text = "Koltuk Seçimi:";   // Etiket üzerinde görünecek metin içeriği
            lbl4.Location = new System.Drawing.Point(20, 155);
            lbl4.AutoSize = true;
            groupYolcu.Controls.Add(lbl4);   // Etiketi grup kutusunun içine eklenir (Parent-Child ilişkisi)

            // KUPON KODU ALANI
            Label lbl5 = new Label();  // nesne oluşturulması, etikettir.
            lbl5.Text = "Kupon Kodu:";  // Etiket üzerinde görünecek metin içeriği
            lbl5.Location = new System.Drawing.Point(20, 230);
            lbl5.AutoSize = true;
            groupYolcu.Controls.Add(lbl5);  // Etiketi grup kutusunun içine eklenir (Parent-Child ilişkisi)

            txtKuponKodu = new TextBox();  // Kupon kodu girişi için metin kutusu 
            txtKuponKodu.Location = new System.Drawing.Point(140, 227); // Metin kutusu konumu
            txtKuponKodu.Size = new System.Drawing.Size(150, 23);
            txtKuponKodu.PlaceholderText = "Opsiyonel";   // Kutu boşken görünecek açıklama: "Opsiyonel"
            txtKuponKodu.TabIndex = 5;
            groupYolcu.Controls.Add(txtKuponKodu);  // metin kutusu grup kutusunun içine eklenir (Parent-Child ilişkisi)

            // FİYAT GÖSTER BUTONU
            btnFiyatGoster = new Button();
            btnFiyatGoster.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnFiyatGoster.ForeColor = System.Drawing.Color.White;
            btnFiyatGoster.Location = new System.Drawing.Point(300, 225);
            btnFiyatGoster.Size = new System.Drawing.Size(100, 28);
            btnFiyatGoster.Text = "Fiyat Hesapla";  // yazacak olan metin
            btnFiyatGoster.UseVisualStyleBackColor = false;
            btnFiyatGoster.TabIndex = 6;
            btnFiyatGoster.Click += new System.EventHandler(BtnFiyatGoster_Click);
            groupYolcu.Controls.Add(btnFiyatGoster); // buton grup kutusunun içine eklenir (Parent-Child ilişkisi)

            // FİYAT DETAY LABEL
            lblHesaplananFiyat = new Label();
            lblHesaplananFiyat.Location = new System.Drawing.Point(20, 260);
            lblHesaplananFiyat.Size = new System.Drawing.Size(380, 140);
            lblHesaplananFiyat.Text = "";
            lblHesaplananFiyat.Font = new System.Drawing.Font("Consolas", 8.5F);
            lblHesaplananFiyat.ForeColor = System.Drawing.Color.DarkGreen;
            lblHesaplananFiyat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblHesaplananFiyat.BackColor = System.Drawing.Color.FromArgb(255, 255, 240);
            lblHesaplananFiyat.Padding = new System.Windows.Forms.Padding(5);
            groupYolcu.Controls.Add(lblHesaplananFiyat);

            // YOLCUAD (textbox) ÖZELLİKLERİ
            txtYolcuAd.Location = new System.Drawing.Point(140, 32);
            txtYolcuAd.Size = new System.Drawing.Size(260, 23);  // Genişlik=260px, Yükseklik=23px
            txtYolcuAd.TabIndex = 1;   
            groupYolcu.Controls.Add(txtYolcuAd);  // Grup kutusuna ekleme

            // YOLCUSOYAD (textbox) ÖZELLİKLERİ
            txtYolcuSoyad.Location = new System.Drawing.Point(140, 72);
            txtYolcuSoyad.Size = new System.Drawing.Size(260, 23);
            txtYolcuSoyad.TabIndex = 2;  
            groupYolcu.Controls.Add(txtYolcuSoyad);   // Grup kutusuna ekleme

            // YOLCUTC (textbox) ÖZELLİKLERİ
            txtYolcuTC.Location = new System.Drawing.Point(140, 112);
            txtYolcuTC.Size = new System.Drawing.Size(260, 23);
            txtYolcuTC.MaxLength = 11;  // Maksimum 11 karakter girilebilir (TC kimlik no için)
            txtYolcuTC.TabIndex = 3;    
            groupYolcu.Controls.Add(txtYolcuTC);  // Grup kutusuna ekleme

            // KOLTUKNO ETİKETİ ÖZELLİKLERİ
            lblKoltukNo.Location = new System.Drawing.Point(140, 152);
            lblKoltukNo.Size = new System.Drawing.Size(260, 23);
            lblKoltukNo.Text = "Henüz koltuk seçilmedi";   // Etiket üzerinde görünecek metin içeriği
            lblKoltukNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            lblKoltukNo.ForeColor = System.Drawing.Color.Gray;  
            lblKoltukNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;  // Sola hizala
            groupYolcu.Controls.Add(lblKoltukNo);  // Grup kutusuna ekleme

            // KOLTUKSEÇ BUTON ÖZELLİKLERİ
            btnKoltukSec.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnKoltukSec.ForeColor = System.Drawing.Color.White; 
            btnKoltukSec.Location = new System.Drawing.Point(140, 185);
            btnKoltukSec.Size = new System.Drawing.Size(260, 35);
            btnKoltukSec.Text = " Koltuk Seç";   // Buton üzerinde görünecek metin içeriği
            btnKoltukSec.UseVisualStyleBackColor = false;  
            btnKoltukSec.TabIndex = 4;
            // Butona tıklandığında BtnKoltukSec_Click metodu çalışır, '+=' operatörü: Olaya (event) bir işleyici (handler) ekler
            // Polymorphism oop yapısı event hanling ile birlikte kullanıldı
            btnKoltukSec.Click += new System.EventHandler(BtnKoltukSec_Click);
            groupYolcu.Controls.Add(btnKoltukSec);

            // BUTONONAYLA BUTONU ÖZELLİKLERİ
            btnOnayla.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);  
            btnOnayla.ForeColor = System.Drawing.Color.White;
            btnOnayla.Location = new System.Drawing.Point(30, 685);
            btnOnayla.Name = "btnOnayla";   // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            btnOnayla.Size = new System.Drawing.Size(200, 40);
            btnOnayla.TabIndex = 5;
            btnOnayla.Text = " Rezervasyonu Onayla";  // Buton üzerinde görünecek metin içeriği
            btnOnayla.UseVisualStyleBackColor = false;
            // Event handling oop yapısı ile Onayla butonuna tıklandığında BtnOnayla_Click çalışır
            btnOnayla.Click += new System.EventHandler(BtnOnayla_Click);

            // İPTAL BUTONU ÖZELLİKLERİ
            btnIptal.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);  
            btnIptal.ForeColor = System.Drawing.Color.White;
            btnIptal.Location = new System.Drawing.Point(250, 685);
            btnIptal.Name = "btnIptal";   // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            btnIptal.Size = new System.Drawing.Size(200, 40);
            btnIptal.TabIndex = 6;
            btnIptal.Text = " İptal";   // Buton üzerinde görünecek metin içeriği
            btnIptal.UseVisualStyleBackColor = false;
            // Event handling oop yapısı ile Onayla butonuna tıklandığında BtnOnayla_Click çalışır
            btnIptal.Click += new System.EventHandler(BtnIptal_Click);

            // FORM ÖZELLİKLERİ 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);  // Ölçekleme boyutu
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;   // Font'a göre ölçekle
            ClientSize = new System.Drawing.Size(565, 750);            // Form pencere boyutu

            // Controls koleksiyonuna bileşenler eklenir (Parent-Child ilişkisi)
            Controls.Add(btnIptal);         // Form'a iptal butonunu ekle
            Controls.Add(btnOnayla);        // Form'a onayla butonunu ekle
            Controls.Add(groupYolcu);       // Form'a yolcu grubunu ekle
            Controls.Add(lblUcusBilgi);     // Form'a uçuş bilgi etiketini ekle
            Controls.Add(lblBaslik);        // Form'a başlık etiketini ekle

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  
            MaximizeBox = false;            // Büyütme butonu devre dışı
            MinimizeBox = false;            // Küçültme butonu devre dışı
            Name = "RezervasyonForm";       // Form adı
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;  // Ebeveyn pencerenin ortasında aç
            Text = "Rezervasyon Yap";       // Pencere başlığı

            groupYolcu.ResumeLayout(false);     
            groupYolcu.PerformLayout();
            ResumeLayout(false);               
            PerformLayout();                    
        }

        // 'private' ile sadece bu sınıf içinden çağrılabilir encapsulation oop yapısı kullanıldı
        // Veritabanından uçuş bilgilerini yükler ve ekrana gösteren metod
        private void UcusBilgisiYukle()
        {
            // Exception handling (hata yömnetimi) için try catxh kullanılmakta bu bir oop yapısıdır
            try  
            {
                // SQL SORGUSU (Query) Veritabanı ile iletişim sağlanıyor
                // CONVERT: SQL Server fonksiyonu - tarih formatını değiştirir
                // Sql Sorgusu: veritabanındaki iki tabloyu (uçuşlar ve uçaklar) birleştirir
                string query = @"SELECT u.UcusKodu, u.Nereden, u.Nereye, uc.KoltukSayisi,
                               CONVERT(VARCHAR, u.KalkisTarihi, 104) + ' ' + CONVERT(VARCHAR, u.KalkisTarihi, 108) AS KalkisTarihi,
                               CAST(u.Fiyat AS DECIMAL(10,2)) AS Fiyat
                               FROM Ucuslar u
                               INNER JOIN Ucaklar uc ON u.UcakID = uc.UcakID
                               WHERE u.UcusID = @UcusID";

                // Sql parametresinin oluşturulması güvenlik için parametre alınır
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                // Veritabanı sorgusunu çalıştır
                DataTable dt = db.ExecuteQuery(query, parameters);

                // Rows.Count: DataTable'daki satır sayısı sıfırdan büyük mü diye kontrol
                if (dt.Rows.Count > 0)  
                {
                    DataRow row = dt.Rows[0];

                    // Tür dönüşümü ile object int'e çevirilir
                    koltukSayisi = Convert.ToInt32(row["KoltukSayisi"]);

                    lblUcusBilgi.Text = $"Uçuş: {row["UcusKodu"]} | {row["Nereden"]} → {row["Nereye"]}\n" +
                                       $"Tarih: {row["KalkisTarihi"]} | Fiyat: {row["Fiyat"]} TL";
                }
            }

            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi verir
                MessageBox.Show("Uçuş bilgisi yüklenirken hata oluştu: " + ex.Message,"Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        // KOLTUK SEÇİM METODU
        // Polymorphism kullanıldı, Koltuk seç butonuna tıklandığında çalışır
        private void BtnKoltukSec_Click(object sender, EventArgs e)
        {
            // Exception handling (hata yömnetimi) için try catxh kullanılmakta bu bir oop yapısıdır
            try  
            {
                // KoltukSecimForm, ile başka bir form sınıfı (sınıf arası iletişim) kurulur
                // Constructor'a (ucusID, koltukSayisi) parametreleri gönderilerek constructor çağırılır
                KoltukSecimForm koltukForm = new KoltukSecimForm(ucusID, koltukSayisi);

                if (koltukForm.ShowDialog() == DialogResult.OK)  
                {
                    // ÖZELLİK ERİŞİMİ: koltukForm nesnesinin özelliğine eriş
                    seciliKoltuk = koltukForm.SeciliKoltuk;  // Seçilen koltuğu al

                    // $ ile string birleştirme
                    lblKoltukNo.Text = $"Seçili Koltuk: {seciliKoltuk}";

                    lblKoltukNo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);  // NESNE OLUŞTURMA: Yeni Font nesnesi
                    lblKoltukNo.ForeColor = System.Drawing.Color.Green;
                }
            }
            catch (Exception ex)  // Herhangi bir hata oluşursa
            {
                // Hata durumunda kullanıcıya mesaj verir
                MessageBox.Show("Koltuk seçimi sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // FİYAT GÖSTER BUTONU
        private void BtnFiyatGoster_Click(object sender, EventArgs e)
        {
            try
            {
                // Koltuk seçim kontrolü, Kullanıcı koltuk seçmiş mi
                if (string.IsNullOrEmpty(seciliKoltuk))
                {
                    MessageBox.Show("Önce bir koltuk seçiniz!", "Uyarı", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kupon kodu alınıyor: Büyük harfe çevrilip boşlukları temizleniyor
                string kuponKodu = txtKuponKodu.Text.Trim().ToUpper();
                // Fiyat detayı hesaplanıyor: UçuşID, seçili koltuk ve kupon kodu ile
                string detay = fiyatHesaplayici.FiyatDetayGetir(ucusID, seciliKoltuk, kuponKodu);
                // Hesaplanan fiyat detayı ekranda gösteriliyor
                lblHesaplananFiyat.Text = detay;
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcı bilgilendiriliyor
                MessageBox.Show("Fiyat hesaplanırken hata: " + ex.Message, 
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ONAYLA BUTONU TIKLAMA METODU 
        private void BtnOnayla_Click(object sender, EventArgs e)
        {
            // Exception handling (hata yönetimi) için try catch kullanılmakta bu bir oop yapısıdır
            try 
            {
                // Bilgiler boş mu diye kontrol, string.IsNullOrWhiteSpace(): String'in null, boş veya sadece boşluk karakterlerinden oluşup oluşmadığını kontrol eder
                if (string.IsNullOrWhiteSpace(txtYolcuAd.Text) ||
                    string.IsNullOrWhiteSpace(txtYolcuSoyad.Text) ||
                    string.IsNullOrWhiteSpace(txtYolcuTC.Text))
                {
                    // Uyarı mesajı göster ve metottan çık
                    MessageBox.Show("Lütfen tüm zorunlu alanları doldurun!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; 
                }

                // Koltuk seçimi kontrolü
                if (string.IsNullOrEmpty(seciliKoltuk))  // Eğer koltuk seçilmemişse
                {
                    // Kullanıcıya mesaj
                    MessageBox.Show("Lütfen bir koltuk seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;  
                }

                // TC Kimlik No uzunluk kontrolü
                if (txtYolcuTC.Text.Length != 11)
                {
                    // 11 haneli değer girilmemişse kullanıcıya mesaj
                    MessageBox.Show("TC Kimlik No 11 haneli olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // long.TryParse(): string'i long'a dönüştürür ve tür kontrolü yapılır
                if (!long.TryParse(txtYolcuTC.Text, out _))
                {
                    // eğer ki hepsi sayıdan oluşmuyorsa kullanıcıya mesaj verilir
                    MessageBox.Show("TC Kimlik No sadece rakamlardan oluşmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Veritabanı SQL sorgusu: Seçili koltuk başka biri tarafından rezerve edilmiş mi diye koltuk müsaitlik kontrolü
                string koltukKontrolQuery = @"SELECT COUNT(*) FROM Rezervasyonlar 
                                        WHERE UcusID = @UcusID AND KoltukNo = @KoltukNo AND Durum = 'Aktif'";

                // Nesne dizisi
                SqlParameter[] koltukParams = {
                new SqlParameter("@UcusID", ucusID),        // İlk parametre
                new SqlParameter("@KoltukNo", seciliKoltuk) // İkinci parametre
            };

                // Sorguyu çalıştır ve sonucu al
                DataTable dtKoltukKontrol = db.ExecuteQuery(koltukKontrolQuery, koltukParams);

                // Sonucu kontrol et
                // Dizi erişimi: [0][0] - ilk satır, ilk sütun
                // object'ten int'e dönüşüm yapılır
                if (Convert.ToInt32(dtKoltukKontrol.Rows[0][0]) > 0)
                {
                    // Koltuk doluysa uyar
                    MessageBox.Show("Seçtiğiniz koltuk başka bir kullanıcı tarafından rezerve edilmiş! Lütfen başka bir koltuk seçiniz.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Seçimi sıfırla
                    seciliKoltuk = "";  // Boş string ata
                    lblKoltukNo.Text = "Henüz koltuk seçilmedi"; // Kullanıcıya mesaj
                    lblKoltukNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
                    lblKoltukNo.ForeColor = System.Drawing.Color.Gray;
                    return;  // Metottan çık
                }

                // Müsait koltuk sayısı için Veritabanı kontrolü
                string kontrolQuery = "SELECT MusaitKoltuk FROM Ucuslar WHERE UcusID = @UcusID";
                // Sorguyu çalıştır ve sonucu al
                DataTable dtKontrol = db.ExecuteQuery(kontrolQuery, new SqlParameter[] { new SqlParameter("@UcusID", ucusID) });

                // Koltuk müsait mi kontrolü 
                if (dtKontrol.Rows.Count == 0 || Convert.ToInt32(dtKontrol.Rows[0]["MusaitKoltuk"]) <= 0)
                {
                    // boşta koltuk yok ise kullanıcıya mesaj
                    MessageBox.Show("Bu uçuşta müsait koltuk kalmamıştır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string kuponKodu = txtKuponKodu.Text.Trim().ToUpper(); // Kupon kodu alınıyor
                decimal hesaplananFiyat = fiyatHesaplayici.FiyatHesapla(ucusID, seciliKoltuk, kuponKodu); // UçuşID, seçili koltuk ve kupon kodu ile toplam fiyat hesaplatılıyor

                // VERİTABANI INSERT İŞLEMİ - Rezervasyon ekleme ve kaydetme yapar
                // INSERT: Yeni kayıt ekle
                // UPDATE: Mevcut kaydı güncelle
                                
                // INSERT SORGUSUNU DEĞİŞTİR:
                string insertQuery = @"INSERT INTO Rezervasyonlar 
                    (KullaniciID, UcusID, YolcuAd, YolcuSoyad, YolcuTC, KoltukNo, UygulananFiyat, KuponKodu) 
                    VALUES (@KullaniciID, @UcusID, @YolcuAd, @YolcuSoyad, @YolcuTC, @KoltukNo, @UygulananFiyat, @KuponKodu);
                    UPDATE Ucuslar SET MusaitKoltuk = MusaitKoltuk - 1 WHERE UcusID = @UcusID";

                SqlParameter[] parameters = {
                new SqlParameter("@KullaniciID", kullaniciID), // Kullanıcı adı parametresi 
                new SqlParameter("@UcusID", ucusID), // uçuş ID parametresi
                new SqlParameter("@YolcuAd", txtYolcuAd.Text.Trim()),  // yolcu adı parametresi 
                new SqlParameter("@YolcuSoyad", txtYolcuSoyad.Text.Trim()),  // yolcu soyadı parametresi 
                new SqlParameter("@YolcuTC", txtYolcuTC.Text.Trim()),  // yolcu TC parametresi 
                new SqlParameter("@KoltukNo", seciliKoltuk),  // Koltuk no parametresi 
                new SqlParameter("@UygulananFiyat", hesaplananFiyat),  // son fiyat parametresi 
                new SqlParameter("@KuponKodu", string.IsNullOrEmpty(kuponKodu) ? (object)DBNull.Value : kuponKodu)  // Kupon parametresi 
            };

                // ExecuteNonQuery(): INSERT/UPDATE/DELETE sorguları için kullanılır
                db.ExecuteNonQuery(insertQuery, parameters);

                // Rezervasyon başarılı ise kullanıcıya mesaj
                MessageBox.Show($"Rezervasyonunuz başarıyla oluşturuldu!\n\nÖdenecek Tutar: {hesaplananFiyat:N2} TL" +
                (string.IsNullOrEmpty(kuponKodu) ? "" : $"\nKullanılan Kupon: {kuponKodu}"), 
                "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                // Form'u kapat
                Close();
            }

            catch (SqlException sqlEx)  
            {
                // Exception handling ile hata yakalama. Veritabanı bağlantı hatası, ağ hatası gibi durumlarda çalışır
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    // Duplicate key (tekrar eden anahtar) hatası
                    MessageBox.Show("Bu koltuk zaten rezerve edilmiş! Lütfen başka bir koltuk seçiniz.",
                        "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Diğer SQL hataları için kullanıcıya mesaj
                    MessageBox.Show("Rezervasyon oluşturulurken hata oluştu: " + sqlEx.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        
            catch (Exception ex)  // SqlException dışındaki tüm hatalar
            {
                // Genel hata mesajı
                MessageBox.Show("Rezervasyon oluşturulurken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // İPTAL BUTONU TIKLAMA METODU - SEÇİM İŞLEMİNİ İPTAL EDER
        private void BtnIptal_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            // Form'u kapat
            Close();
        }
    }
}