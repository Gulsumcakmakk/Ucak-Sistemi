using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

// KOLTUK SECIM FORM SINIFI - UÇAK KOLTUK SEÇİM İŞLEMLERİNİ YAPAR

// Namespace ile kodları organize ederiz böyle geniş bir projede karmaşıklığı önlemek için gerekli
namespace Ucak
{
    /* Aşağıdaki kısım da Inheritance (Kalıtım) oop özelliği kullanıldı.
    using System.Windows.Forms; ile Form dan kalıtım aldım. */
    public partial class KoltukSecimForm : Form
    {
        // Veritabanı işlemleri için yardımcı sınıf olan DatabaseHelper private olarak tanımlanarak sadece bu class içinde erişilebilir, dışarıdan gizlidir. 
        private DatabaseHelper db;
        private int ucusID;  // ucusID: Hangi uçuş için koltuk seçimi yapıldığını tutar
        private int koltukSayisi;  // koltukSayisi: Uçaktaki toplam koltuk sayısı
        private string seciliKoltuk = "";  // seciliKoltuk: Kullanıcının seçtiği koltuk numarası  (varsayılan değer: boş string)
        // koltukButtons: Koltuk numaralarını butonlarla eşleştirir 
        // Dictionary (collections) oop yapısı 
        private Dictionary<string, Button> koltukButtons = new Dictionary<string, Button>();

        // SeciliKoltuk: Dışarıya sadece okunabilir şekilde seçili koltuk bilgisini verir
        // Sadece read-only özelliği ile property oop yapısı kullanılıyor
        public string SeciliKoltuk => seciliKoltuk;

        public KoltukSecimForm(int ucusID, int koltukSayisi) // Constructordır. Nesne oluşturulduğunda otomatik çalışan yapıcı metoddur. Parametrelidir, nesne oluşturulurken ucusID ve koltukSayisi bilgilerini alır
        {
            InitializeComponent(); // vs code tarafından otomatik oluşturulur, formdaki tüm kontrolleri hazırlar.
            /* Composition (Bileşim) oop özelliği kullanıldı. Başka bir sınıftan örnek alıyor.
               veritabanı bağlantısı için DatabaseHelper sınıfından yeni bir nesne oluşturuluyor ve db alanına atanıyor. */
            db = new DatabaseHelper();
            // Parametre değerleri sınıf değişkenlerine atanıyor
            // 'this' Sınıfın kendi üyesine işaret ederek parametre ile field'ı karışmasını engelliyor
            this.ucusID = ucusID;
            this.koltukSayisi = koltukSayisi;
            // Koltukları dinamik olarak oluşturan metod
            KoltuklariOlustur();
        }

        // Arayüz için gereken tüm form kontrollerini (buton, textbox, label) oluşturulduğu ve tasarımının yapıldığı kısım
        private void InitializeComponent()
        {
            this.lblBaslik = new System.Windows.Forms.Label();  // Başlık etiketi
            this.lblAciklama = new System.Windows.Forms.Label();   // Açıklama etiketi
            this.panelKoltuklar = new System.Windows.Forms.Panel();   // Koltuk butonlarının yerleştirileceği panel
            this.panelLegend = new System.Windows.Forms.Panel();  // Açıklama (legend) paneli
            this.btnOnayla = new System.Windows.Forms.Button();  // Onayla butonu
            this.btnIptal = new System.Windows.Forms.Button();  // İptal butonu
            this.SuspendLayout();
            
            // BAŞLIK ETİKETİ (lblBaslik) - "Koltuk Seçimi" YAZISI
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBaslik.Location = new System.Drawing.Point(200, 15);
            this.lblBaslik.Name = "lblBaslik";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.lblBaslik.Size = new System.Drawing.Size(150, 25);
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "Koltuk Seçimi"; // Etiket üzerinde görünecek metin içeriği
            
            // AÇIKLAMA ETİKETİ (lblAciklama) - KULLANICI YÖNERGESİ
            this.lblAciklama.AutoSize = true;
            this.lblAciklama.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAciklama.Location = new System.Drawing.Point(30, 50);
            this.lblAciklama.Name = "lblAciklama";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.lblAciklama.Size = new System.Drawing.Size(300, 15);
            this.lblAciklama.TabIndex = 1;
            this.lblAciklama.Text = "Lütfen oturmak istediğiniz koltuğu seçiniz:"; // Etiket üzerinde görünecek metin içeriği
            
            // KOLTUKLAR PANELİ (panelKoltuklar) - TÜM KOLTUK BUTONLARINI İÇERİR
            this.panelKoltuklar.AutoScroll = true; // Çok sayıda koltuk olduğunda kaydırma çubuğu göster
            this.panelKoltuklar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelKoltuklar.Location = new System.Drawing.Point(30, 80);
            this.panelKoltuklar.Name = "panelKoltuklar";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.panelKoltuklar.Size = new System.Drawing.Size(500, 400);
            this.panelKoltuklar.TabIndex = 2;
            
            // AÇIKLAMA PANELİ (panelLegend) - KOLTUK RENK AÇIKLAMALARI
            this.panelLegend.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLegend.Location = new System.Drawing.Point(550, 80);
            this.panelLegend.Name = "panelLegend";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.panelLegend.Size = new System.Drawing.Size(150, 290);
            this.panelLegend.TabIndex = 3;
            
            // LEGEND PANELİ - RENK KODLARININ AÇIKLAMALARI
            // Legend panelinin başlık etiketi 
            var lblLegend = new Label { 
                Text = "Sınıf & Fiyat:", 
                Font = new Font("Segoe UI", 9F, FontStyle.Bold), // Kalın yazı tipi
                Location = new Point(10, 10),  // Panel içinde konumlandırma
                AutoSize = true 
            };
            this.panelLegend.Controls.Add(lblLegend); // Legend paneline etiket ekleniyor

            // Business Class - Pencere Bölümü koltukları için açıklama
            var btnBusinessPencere = new Button { 
                Size = new Size(30, 30), // Sabit boyutlu gösterge butonu
                Location = new Point(10, 40),  // Legend paneli içinde konumu
                BackColor = Color.Gold, // Business pencere rengi: Altın sarısı
                Enabled = false 
            };
            var lblBusinessPencere = new Label { 
                Text = "Business\nPencere\n+%25", 
                Location = new Point(50, 38),  // Butonun yanına konumlandırılıyor
                AutoSize = true, 
                Font = new Font("Segoe UI", 7F) 
            };
            this.panelLegend.Controls.Add(btnBusinessPencere);  // Gösterge butonu panele ekleniyor
            this.panelLegend.Controls.Add(lblBusinessPencere);  // Açıklama etiketi panele ekleniyor

            // Business Class - Koridor Bölümü koltukları için açıklama
            var btnBusinessKoridor = new Button { 
                Size = new Size(30, 30), 
                Location = new Point(10, 90),  // Bir öncekinin altına konumlandırılıyor
                BackColor = Color.Silver,  // Business koridor rengi: Gümüş
                Enabled = false 
            };
            var lblBusinessKoridor = new Label { 
                Text = "Business\nKoridor\n+%15", 
                Location = new Point(50, 88), 
                AutoSize = true, 
                Font = new Font("Segoe UI", 7F) 
            };
            this.panelLegend.Controls.Add(btnBusinessKoridor);  // Gösterge butonu panele ekleniyor
            this.panelLegend.Controls.Add(lblBusinessKoridor);  // Açıklama etiketi panele ekleniyor

            // Economy Class Bölümü koltukları için açıklama
            var btnEconomy = new Button { 
                Size = new Size(30, 30), 
                Location = new Point(10, 140),  // Bir öncekinin altına konumlandırılıyor
                BackColor = Color.LightBlue,  // Economy sınıfı rengi: Açık mavi
                Enabled = false  
            };
            var lblEconomy = new Label { 
                Text = "Economy\nStandart", 
                Location = new Point(50, 142),  // Butonun yanına konumlandırılıyor
                AutoSize = true, 
                Font = new Font("Segoe UI", 7F) 
            };
            this.panelLegend.Controls.Add(btnEconomy);  // Gösterge butonu panele ekleniyor
            this.panelLegend.Controls.Add(lblEconomy);  // Açıklama etiketi panele ekleniyor

            // Ayırıcı çizgi, Bölümleri görsel olarak ayırmak için
            var lblSeparator = new Label { 
                Text = "─────────", 
                Location = new Point(10, 180),  // Economy göstergesinden sonra konumlandırılıyor 
                AutoSize = true,
                ForeColor = Color.Gray
            };
            this.panelLegend.Controls.Add(lblSeparator);

            // Dolu koltuk göstergesi
            var btnDolu = new Button { 
                Size = new Size(30, 30), 
                Location = new Point(10, 200), 
                BackColor = Color.LightCoral,  // Dolu koltuk rengi
                Enabled = false 
            };
            var lblDolu = new Label { 
                Text = "Dolu",  // Tek satırlık açıklama: Dolu/rezerve edilmiş koltuk
                Location = new Point(50, 205), 
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F)
            };
            this.panelLegend.Controls.Add(btnDolu); // Gösterge butonu panele ekleniyor
            this.panelLegend.Controls.Add(lblDolu);  // Açıklama etiketi panele ekleniyor

            // Seçili koltuk göstergesi
            var btnSecili = new Button { 
                Size = new Size(30, 30), 
                Location = new Point(10, 240),  // Dolu göstergesinin altına konumlandırılıyor
                BackColor = Color.DeepSkyBlue, // Seçili koltuk rengi
                Enabled = false 
            };
            var lblSecili = new Label { 
                Text = "Seçili",  
                Location = new Point(50, 245),  // Butonun yanına konumlandırılıyor
                AutoSize = true,
                Font = new Font("Segoe UI", 7.5F)
            };
            this.panelLegend.Controls.Add(btnSecili);  // Gösterge butonu panele ekleniyor
            this.panelLegend.Controls.Add(lblSecili); // Açıklama etiketi panele ekleniyor

            // ONAYLA BUTONU (btnOnayla) - KOLTUK SEÇİMİNİ ONAYLAR
            this.btnOnayla.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnOnayla.ForeColor = System.Drawing.Color.White;
            this.btnOnayla.Location = new System.Drawing.Point(30, 495);
            this.btnOnayla.Name = "btnOnayla";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.btnOnayla.Size = new System.Drawing.Size(320, 40);
            this.btnOnayla.TabIndex = 4;
            this.btnOnayla.Text = " Seçimi Onayla"; // Buton üzerinde görünecek metin içeriği
            this.btnOnayla.UseVisualStyleBackColor = false;
            // Event handling oop yapısı ile Buton tıklama olayına metod bağlama 
            this.btnOnayla.Click += new System.EventHandler(this.BtnOnayla_Click);
            
            // İPTAL BUTONU (btnIptal) - SEÇİM İŞLEMİNİ İPTAL EDER
            this.btnIptal.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnIptal.ForeColor = System.Drawing.Color.White;
            this.btnIptal.Location = new System.Drawing.Point(370, 495);
            this.btnIptal.Name = "btnIptal";  // Kontrolün programatik adı - Kod içinde bu isimle referans edilir
            this.btnIptal.Size = new System.Drawing.Size(160, 40);
            this.btnIptal.TabIndex = 5;
            this.btnIptal.Text = " İptal";  // Buton üzerinde görünecek metin içeriği
            this.btnIptal.UseVisualStyleBackColor = false;
            this.btnIptal.Click += new System.EventHandler(this.BtnIptal_Click);
            
            // ANA FORM ÖZELLİKLERİ
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 560);
            this.Controls.Add(this.btnIptal);   // İptal butonu
            this.Controls.Add(this.btnOnayla);  // Onayla butonu
            this.Controls.Add(this.panelLegend);  // Açıklama (legend) paneli
            this.Controls.Add(this.panelKoltuklar); // Koltuk butonlarının yerleştirileceği panel
            this.Controls.Add(this.lblAciklama);  // Açıklama etiketi
            this.Controls.Add(this.lblBaslik);  // Başlık etiketi
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false; // Büyütme butonu pasif
            this.MinimizeBox = false;  // Küçültme butonu pasif
            this.Name = "KoltukSecimForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Koltuk Seçimi";  // görünecek metin içeriği
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Encapsulation oop yapısı kullanılmıştır
        // Encapsulation ile kontrollere sadece bu sınıfın içindeki metodlardan erişilebilir
        // Tüm kontrol değişkenleri private olarak tanımlandı
        private Label lblBaslik, lblAciklama;
        private Panel panelKoltuklar, panelLegend;
        private Button btnOnayla, btnIptal;

        // KOLTUKLARI OLUŞTURAN METOD - DİNAMİK OLARAK KOLTUK BUTONLARINI YARATIR
        private void KoltuklariOlustur()
        {
            // Exception handling (hata yömnetimi) için try catxh kullanılmakta bu bir oop yapısıdır
            try
            {
                // DOLU KOLTUKLARI VERİTABANINDAN ALMA
                // SQL Sorgusu: Bu uçuş için aktif rezervasyonları olan koltukları bul
                string query = @"SELECT KoltukNo FROM Rezervasyonlar 
                               WHERE UcusID = @UcusID AND Durum = 'Aktif'";
                // Sql parametresinin oluşturulması güvenlik için parametre alınır
                SqlParameter[] parameters = { new SqlParameter("@UcusID", ucusID) };
                // Veritabanı sorgusunu çalıştır
                DataTable dtDoluKoltuklar = db.ExecuteQuery(query, parameters);

                // HashSet kullanımı (hızlı arama için) (collections) oop yapısı kullanıldı
                // Dolu koltuk numaralarını saklayan koleksiyon
                HashSet<string> doluKoltuklar = new HashSet<string>();
                // DataTable'daki tüm satırlar dolaşılır
                foreach (DataRow row in dtDoluKoltuklar.Rows)
                {
                    // Her dolu koltuk numarasını HashSet'e ekler
                    doluKoltuklar.Add(row["KoltukNo"].ToString());
                }

                // KOLTUK DÜZENİNİ OLUŞTURMA - UÇAK KOLTUK YERLEŞİMİ
                // Sıra sayısını hesapla: Toplam koltuk / 6 (her sırada 6 koltuk) olacak şekilde hesplama yapılıyor
                int siraSayisi = (int)Math.Ceiling(koltukSayisi / 6.0);
                // Koltuk harfleri: Standart uçak koltuk düzeni (A B C | Koridor | D E F)
                char[] harfler = { 'A', 'B', 'C', 'D', 'E', 'F' };
                
                // Y pozisyonu: Panel içinde koltukların dikey konumu
                int yPos = 10;
            
                // HER SIRA İÇİN KOLTUKLARI OLUŞTURMA
                for (int sira = 1; sira <= siraSayisi; sira++)
                {
                    // SIRA NUMARASI ETİKETİ - Sırayı gösteren sayı
                    Label lblSira = new Label
                    {
                        Text = sira.ToString(), // Sıra numarası
                        Location = new Point(10, yPos + 5),
                        Size = new Size(30, 30),
                        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    panelKoltuklar.Controls.Add(lblSira);

                    // X pozisyonu: Panel içinde koltukların yatay konumu
                    int xPos = 50;
                    
                    // HER KOLTUK İÇİN BUTON OLUŞTURMA
                    for (int i = 0; i < 6; i++)  // Her sırada 6 koltuk
                    {
                        // Koltuk indexinin hesaplanması
                        int koltukIndex = (sira - 1) * 6 + i + 1;
                        // Toplam koltuk sayısını aşmışsa döngüyü kır
                        if (koltukIndex > koltukSayisi) break;

                        // Koltuk numarasını sıra numarı ve harf birleşimi olacak şekilde adlandırdım
                        string koltukNo = $"{sira}{harfler[i]}";
                        // Bu koltuk dolu mu? HashSet'te var mı kontrol et
                        bool isDolu = doluKoltuklar.Contains(koltukNo);

                        // KOLTUK BUTONU OLUŞTURMA
                        Button btnKoltuk = new Button
                        {
                            Text = harfler[i].ToString(),   // Koltuk harfini göster
                            Size = new Size(50, 40),
                            Location = new Point(xPos, yPos), // Konumu
                            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                            Tag = koltukNo,   // Koltuk numarasını sakla
                            BackColor = isDolu ? Color.LightCoral : GetKoltukRengi(harfler[i]),
                            Enabled = !isDolu, // Dolu koltuk tıklanamaz
                            FlatStyle = FlatStyle.Flat  // Düz buton stili
                        };

                        // Buton kenarlık ayarları
                        btnKoltuk.FlatAppearance.BorderSize = 2;
                        btnKoltuk.FlatAppearance.BorderColor = Color.Gray;

                        // Eğer koltuk boşsa tıklama olayını bağla
                        if (!isDolu)
                        {
                            // Event hnadling oop yapısı. Buton tıklama olayına metod bağlaması yapılır
                            btnKoltuk.Click += KoltukButton_Click;
                            btnKoltuk.Cursor = Cursors.Hand;
                        }

                        // Dictionary'e koltuk butonunu ekle        
                        koltukButtons[koltukNo] = btnKoltuk;
                        // Butonu panel'e ekle
                        panelKoltuklar.Controls.Add(btnKoltuk);

                        // X pozisyonunu bir sonraki koltuk için güncelle
                        xPos += 60;  // Koltuk genişliği + boşluk
                        
                        // Koridor boşluğu (C ve D arasında)
                        if (i == 2) xPos += 30; // 3.koltuktan sonra (C) koridor boşluğu
                    }

                    // Y pozisyonunu bir sonraki sıra için güncelle
                    yPos += 50;  // Sıra yüksekliği + boşluk
                }

                // İSTATİSTİKLERİ GÖSTERME - KOLTUK DURUM ÖZETİ
                int toplamKoltuk = koltukSayisi;
                int doluKoltukSayisi = doluKoltuklar.Count;
                int bosKoltukSayisi = toplamKoltuk - doluKoltukSayisi;

                // İstatistik etiketi oluştur
                Label lblStats = new Label // yeni nesne oluşturulması
                {
                    Text = $"Toplam: {toplamKoltuk}\nBoş: {bosKoltukSayisi}\nDolu: {doluKoltukSayisi}",
                    Location = new Point(560, 250),
                    Size = new Size(130, 60),
                    Font = new Font("Segoe UI", 9F)
                };
                this.Controls.Add(lblStats);
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya bilgi verir
                MessageBox.Show("Koltuklar yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // KOLTUK BUTONU TIKLAMA METODU - KULLANICI BİR KOLTUĞA TIKLADIĞINDA ÇALIŞIR
        private void KoltukButton_Click(object sender, EventArgs e) // sender parametresi: Tıklanan butonun referansını verir
        {
            Button clickedButton = (Button)sender;
            // Tag property'sinden koltuk numarası alınır
            string koltukNo = clickedButton.Tag.ToString();

            // Eğer önceden seçilmiş bir koltuk varsa ve bu koltuk Dictionary'de mevcutsa önceki seçimi temizler
            if (!string.IsNullOrEmpty(seciliKoltuk) && koltukButtons.ContainsKey(seciliKoltuk))
            {
                // Önceki seçili koltuğun rengini boş koltuk rengine (yeşile) döndürür
                koltukButtons[seciliKoltuk].BackColor = Color.LightGreen;
            }

            // Yeni seçimi kaydetme, Yeni koltuk numarasını saklanması
            seciliKoltuk = koltukNo;
            // Tıklanan butonun rengini seçili koltuk rengine (mavi) ile değiştirir
            clickedButton.BackColor = Color.DeepSkyBlue;
            
            // Açıklama etiketini güncelleriz
            lblAciklama.Text = $"Seçili koltuk: {seciliKoltuk}";
        }

        // ONAYLA BUTONU TIKLAMA METODU - KOLTUK SEÇİMİNİ ONAYLAR
        private void BtnOnayla_Click(object sender, EventArgs e)
        {
            // Seçili koltuk kontrolü, kullanıcı koltuk seçmiş mi
            if (string.IsNullOrEmpty(seciliKoltuk))
            {
                // Seçmemişse kullanıcıya uyarı mesajı
                MessageBox.Show("Lütfen bir koltuk seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Formun sonucu OK olarak ayarlanır
            this.DialogResult = DialogResult.OK;
            // Formu kapat
            this.Close();
        }

        // İPTAL BUTONU TIKLAMA METODU - SEÇİM İŞLEMİNİ İPTAL EDER
        private void BtnIptal_Click(object sender, EventArgs e)
        {
            // Formun sonucunu Cancel olarak ayarlar yani iptal eder
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // KOLTUK SINIFINA GÖRE RENK BELİRLEME METODU
        private Color GetKoltukRengi(char koltukHarf)
        {
            // BUSINESS CLASS - Pencere - Altın Sarısı
            if (koltukHarf == 'A' || koltukHarf == 'F')
                return Color.Gold;
            
            // BUSINESS CLASS - Koridor - Gümüş Gri
            if (koltukHarf == 'C' || koltukHarf == 'D')
                return Color.Silver;
            
            // ECONOMY CLASS - Açık Mavi
            return Color.LightBlue;
        }
    }
}