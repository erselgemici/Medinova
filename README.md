# 🏥 Medinova - AI & ML Destekli Hastane Yönetim Sistemi

Medinova, **.NET 8.0** ile geliştirilmiş; yapay zeka destekli analiz, makine öğrenimi ile gelecek tahmini ve otomatik e-posta bildirimleri sunan kapsamlı bir hastane randevu ve yönetim sistemidir.

![Project Banner](Screenshots/dashboard-banner.png) **

## 🚀 Projenin Temel Özellikleri

### 🤖 Yapay Zeka Destekli Hasta Asistanı (Google Gemini)
- **Bağlam Enjeksiyonu (Context Injection):** Yapay zeka, rastgele cevaplar vermek yerine hastanenin veritabanındaki gerçek bölümleri tanır.
- **Semptom Analizi:** Hasta şikayetini yazar (Örn: "Gözümde yanma var"), yapay zeka bunu analiz eder.
- **Akıllı Yönlendirme:** AI, hastaya önce tıbbi tavsiye verir, sonra hastanede gitmesi gereken polikliniği önerir.

### 📧 SMTP E-Posta Entegrasyonu
- **Otomatik Bildirim:** Randevu alındığı anda backend tarafında `SmtpClient` tetiklenir.
- **HTML Şablon:** Kullanıcıya salt metin değil, hastane logosu ve randevu detaylarını içeren profesyonel bir HTML mail gönderilir.
- **Hata Yönetimi:** Mail gönderimi sırasında oluşabilecek hatalar try-catch blokları ile yönetilir ve loglanır.

### 📊 Gelecek Tahmini (ML.NET)
- **Regresyon Analizi:** `Microsoft.ML` kütüphanesi kullanılarak geçmiş randevu verileri eğitilir (Training).
- **Trend Tahmini:** Sistem, önümüzdeki 3 ayda oluşabilecek randevu yoğunluğunu tahmin eder ve Yönetici Panelinde grafik olarak sunar.

### 🔐 Özel Kimlik Doğrulama (Custom Identity)
- Hazır Identity kütüphanesi kullanılmadan, User-Role-UserRoles tabloları ile **RBAC (Role Based Access Control)** yapısı kurulmuştur.
  - **Admin:** Tüm sistemi yönetir, logları izler, istatistikleri görür.
  - **Doktor:** Kendi randevularını görüntüler.
  - **Hasta:** Randevu alır, geçmişini görür, AI asistanı kullanır.

### 📝 Gelişmiş Loglama ve İzleme
- **Veritabanı Loglama:** `Auth` (Giriş/Çıkış), `Business` (Randevu), `Audit` (AI Soruları) ve `Error` (Hatalar) kayıt altına alınır.
- **Admin Log Paneli:** Yöneticiler logları filtreleyebilir, renklendirilmiş arayüzde hataları tespit edebilir.

## 🛠 Kullanılan Teknolojiler

* **Framework:** ASP.NET Core MVC (.NET 8.0)
* **Veri Erişimi:** Entity Framework Core (Db First)
* **Yapay Zeka:** Google Gemini
* **Makine Öğrenimi:** ML.NET (SDCA Regression)
* **Mesajlaşma:** System.Net.Mail (SMTP)
* **Veritabanı:** MS SQL Server
* **Frontend:** Bootstrap 5, Tailwind CSS (Admin), jQuery, SweetAlert2, Chart.js

## 📸 Ekran Görüntüleri

### AI Asistan & Randevu Ekranı
![AI Assistant](Screenshots/ai-assistant.png)
*Yapay zeka şikayeti analiz eder ve doğru bölüme yönlendirir.*

### E-Posta Bildirimi
![Email Notification](Screenshots/email-notification.png)
*Randevu sonrası gönderilen otomatik HTML mail.*

### Admin Dashboard (ML.NET Tahminleri)
![Admin Dashboard](Screenshots/admin-dashboard.png)



## 👨‍💻 Geliştirici

**[Senin Adın]** tarafından, Murat Yücedağ & Erhan Gündüz mentörlüğünde geliştirilmiştir.
