# 🏥 Medinova - AI & ML Destekli Hastane Yönetim Sistemi

Medinova, **.NET 8.0** ile geliştirilmiş; yapay zeka destekli analiz, makine öğrenimi ile gelecek tahmini ve otomatik e-posta bildirimleri sunan kapsamlı bir hastane randevu ve yönetim sistemidir.

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
<img width="160" height="244" alt="ss0" src="https://github.com/user-attachments/assets/c5696177-c23f-4e37-97f8-8f320a5d7132" />
<img width="953" height="473" alt="ss1" src="https://github.com/user-attachments/assets/d2be4c02-45ac-4b18-ae22-a6e4200066fa" />
<img width="955" height="423" alt="ss2" src="https://github.com/user-attachments/assets/a5f6d133-5e7b-4a18-9bb8-178109e06942" />
<img width="947" height="329" alt="ss3" src="https://github.com/user-attachments/assets/9d5085c1-2af7-481f-8197-917400d49cb6" />
<img width="948" height="406" alt="ss4" src="https://github.com/user-attachments/assets/5b5cde57-a152-46b5-bb88-17c5d473acab" />
<img width="946" height="293" alt="ss5" src="https://github.com/user-attachments/assets/22fe8e83-aef2-4d74-b38b-732b39064a83" />
<img width="946" height="329" alt="ss6" src="https://github.com/user-attachments/assets/c79fc169-eaed-4d4f-be42-69834a0fb2f9" />
<img width="946" height="335" alt="ss7" src="https://github.com/user-attachments/assets/b7165b68-bb66-4246-88b1-cbc48d92f669" />
<img width="944" height="335" alt="ss8" src="https://github.com/user-attachments/assets/645cba2b-e223-440a-b3bb-53b7beaafa2b" />
<img width="191" height="133" alt="ss9" src="https://github.com/user-attachments/assets/56e0126e-73e5-4ba8-8161-d8f7e8a319b4" />
<img width="582" height="316" alt="ss10" src="https://github.com/user-attachments/assets/3fca297d-d32c-45fb-8685-4a16f2f27336" />
<img width="953" height="406" alt="ss11" src="https://github.com/user-attachments/assets/ac7ec33a-6d3c-4968-bd08-0845c4436055" />
<img width="956" height="342" alt="ss12" src="https://github.com/user-attachments/assets/137fcad1-1108-42aa-b52e-8af3a288bfce" />
<img width="954" height="362" alt="ss13" src="https://github.com/user-attachments/assets/7314a30e-06f9-499b-833c-9e58ba51cdd6" />
<img width="954" height="337" alt="ss14" src="https://github.com/user-attachments/assets/eff674ab-a6ec-4e9f-aeb0-3513a8044c90" />
<img width="954" height="473" alt="ss15" src="https://github.com/user-attachments/assets/75fece7d-eb9e-4c8f-849c-7ff4d4101e8a" />
<img width="947" height="473" alt="ss16" src="https://github.com/user-attachments/assets/6f43ccb6-4fbb-43da-9567-f45a104551af" />
<img width="956" height="474" alt="ss17" src="https://github.com/user-attachments/assets/d8e9a4d9-3621-4042-b2ee-d925cd6f6935" />
<img width="949" height="476" alt="ss18" src="https://github.com/user-attachments/assets/ebf1fe1f-083c-4aaf-9586-7a80259fe58b" />
<img width="955" height="388" alt="ss19" src="https://github.com/user-attachments/assets/d6b33a8e-a452-4572-86fb-d5ce42aacae0" />
<img width="947" height="450" alt="ss20" src="https://github.com/user-attachments/assets/0bb2412d-2eff-471f-912a-bc24c5a1b2f3" />
<img width="955" height="415" alt="ss21" src="https://github.com/user-attachments/assets/224f2554-2819-4935-b1d4-15f09e5042e5" />
<img width="953" height="389" alt="ss22" src="https://github.com/user-attachments/assets/c12e66f4-22f4-4d67-9e17-65b6eb753f97" />



