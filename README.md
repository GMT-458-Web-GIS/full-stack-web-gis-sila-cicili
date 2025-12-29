# 📚 Web GIS Library Management System (Kütüphane Yönetim Sistemi)

![.NET Core](https://img.shields.io/badge/.NET%20Core-7.0-purple)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-14+-blue)
![PostGIS](https://img.shields.io/badge/PostGIS-Enabled-green)
![License](https://img.shields.io/badge/License-MIT-orange)

Bu proje, **GMT 458 – Web GIS** dersi final ödevi kapsamında geliştirilmiş; mekansal (spatial) ve mekansal olmayan verileri bir arada yöneten, farklı kullanıcı rollerine sahip web tabanlı bir **Kütüphane Bilgi Sistemidir**.

Proje **ASP.NET Core MVC**, **PostgreSQL (PostGIS)** ve **Entity Framework Core** teknolojileri kullanılarak modern mimariye uygun olarak tasarlanmıştır.

---

## 🚀 Proje Özellikleri ve Gereksinimler (Requirements Rubric)

Aşağıdaki tablo, proje gereksinimlerinin ne ölçüde karşılandığını özetlemektedir:

| Gereksinim (Requirement) | Durum | Açıklama |
| :--- | :---: | :--- |
| **Source Code Management** | ✅ Tamam | Proje versiyon kontrolü GitHub üzerinde sağlanmıştır. |
| **Managing User Types** | ✅ Tamam | **3 Farklı Rol:** <br>🎓 **Öğrenci:** 15 gün ödünç alma süresi.<br>👨‍🏫 **Akademisyen:** 30 gün ödünç alma süresi.<br>🛡️ **Yönetici (Admin):** Tam yetkili. |
| **CRUD Operations (Spatial)** | ✅ Tamam | Kütüphane şubeleri (Spatial Point) harita üzerinden **Eklenebilir, Silinebilir, Güncellenebilir ve Listelenebilir**. |
| **Authentication** | ✅ Tamam | Cookie tabanlı güvenli **Üye Kaydı (Sign-up)** ve **Giriş (Login)** mekanizması. |
| **API Development** | ✅ Tamam | **RESTful API:** Spatial (Şube) ve Non-spatial (Kitap) veriler dışarıya açılmıştır. <br>📄 **Swagger:** `/swagger` adresinde dökümantasyon mevcuttur. |
| **Database** | ✅ Tamam | İlişkisel veriler için **PostgreSQL**, coğrafi veriler için **PostGIS** kullanılmıştır. |
| **Dashboard** | ✅ Tamam | Admin panelinde anlık istatistikler ve kitap kategorilerini gösteren **Chart.js** grafikleri bulunur. |
| **Performance Testing** | ✅ Tamam | **Apache JMeter** ile Load ve Stress testleri uygulanmış, yanıt süreleri analiz edilmiştir. |
| **Performance Monitoring** | ✅ Tamam | **B-Tree** ve **R-Tree** indekslemenin sorgu performansına etkisi analiz edilmiştir. |

---

## 🛠️ Teknoloji Yığını (Tech Stack)

* **Backend:** ASP.NET Core 7.0 (MVC & Web API)
* **Veritabanı:** PostgreSQL 14+ & PostGIS Extension
* **ORM:** Entity Framework Core (NetTopologySuite ile mekansal veri desteği)
* **Frontend:** HTML5, Bootstrap 5, JavaScript
* **Görselleştirme:** Chart.js (İstatistikler), Leaflet/Google Maps (Harita Arayüzü)
* **Test & Dokümantasyon:** Apache JMeter, Swagger UI



### Sistem Tanıtım Videosu
![Tanıtım Videosu](http://img.youtube.com/vi/knZImprxyPM/maxresdefault.jpg))](https://www.youtube.com/watch?v=knZImprxyPM)


## 📸 Ekran Görüntüleri (Screenshots)

### 1. Yönetim Paneli (Dashboard)
Yöneticiler için özet istatistikler ve grafiksel raporlar.
![Panel](images/kullanıcı.png)

### 2. Swagger API Dokümantasyonu
RESTful servislerin test edilebileceği arayüz.
![Swagger](images/swagger.png)

### 3. Harita ve Şube Yönetimi
PostGIS destekli şube ekleme ve görüntüleme ekranı.
![Harita](images/harita.png)

---

## ⚙️ Kurulum (Installation)

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin;

1.  **Projeyi Klonlayın:**
    ```bash
    git clone [https://github.com/KULLANICI_ADIN/LibrarySystem.git](https://github.com/KULLANICI_ADIN/LibrarySystem.git)
    cd LibrarySystem
    ```

2.  **Veritabanı Bağlantısını Yapılandırın:**
    `appsettings.json` dosyasını açın ve `ConnectionStrings` bölümünü kendi PostgreSQL bilgilerinize göre düzenleyin:
    ```json
    "ConnectionStrings": {
      "LibraryContext": "Host=localhost;Database=LibraryDb;Username=postgres;Password=sifreniz"
    }
    ```

3.  **Veritabanını Oluşturun (Migration):**
    Terminali proje dizininde açın ve aşağıdaki komutu çalıştırın:
    ```bash
    dotnet ef database update
    ```

4. **Projeyi Başlatın:**
    ```bash
    dotnet run
    ```
---

## 🔗 API Kullanımı

Proje çalışırken API endpointlerini test etmek için:
👉 **URL:** `https://localhost:7239/swagger`

| Metot | Endpoint | Açıklama |
| :--- | :--- | :--- |
| **GET** | `/api/LibraryApi/branches` | Tüm kütüphane şubelerini (GeoJSON) getirir. |
| **POST** | `/api/LibraryApi/branches` | Yeni bir şube ekler. |
| **PUT** | `/api/LibraryApi/branches/{id}` | Şube bilgilerini günceller. |
| **DELETE** | `/api/LibraryApi/branches/{id}` | Şubeyi siler. |

---

## 🚀 Performans ve Yük Testleri (Load & Stress Testing)
Bu stres testi, uygulamanın normal kullanım sınırlarının çok ötesindeki yükler altında (Peak Traffic) kararlılığını ölçmek amacıyla gerçekleştirilmiştir. Hedefimiz, 600 eşzamanlı kullanıcının sisteme aniden yüklenmesi durumunda; veritabanı bağlantı havuzunun (connection pool) tıkanıp tıkanmadığını, API'nin çöküp çökmediğini (Crash) ve sistemin veri bütünlüğünü koruyup koruyamadığını analiz etmektir. Bu test ile sistemin sadece hızlı değil, aynı zamanda zorlu koşullarda sürdürülebilir ve dayanıklı (Resilient) olduğu doğrulanmak istenmiştir. 

---
Sistemin dayanıklılığını ölçmek için **Apache JMeter** kullanılarak testler gerçekleştirilmiştir. Veritabanına **50.000 adet Dummy (sahte) kitap verisi** eklenmiş ve testler bu set üzerinde koşulmuştur.

### 📊 Test Sonuçları

| Test Tipi | Kullanıcı (Threads) | Amaç | Ortalama Yanıt Süresi | Sonuç |
| :--- | :---: | :--- | :---: | :--- |
| **Load Test** | 100 | Normal kullanım simülasyonu | **34 ms** | ✅ Başarılı |
| **Stress Test** | 600 | Sistemi sınıra zorlama | **3400 ms** | ✅ Stabil |

#### 1. Load Test (100 Kullanıcı)
![Load Test Grafiği](images/100.png)

#### 2. Stress Test (600 Kullanıcı)
![Stress Test Grafiği](images/1000.png)
---
1000 kullanıcılı stres testi sonucunda sistem, normal çalışma süresinin üzerinde (3.4 sn) yanıt verse de kesintisiz erişilebilirlik (100% Availability) sağlamıştır. Herhangi bir HTTP 500 hatası veya sistem çökmesi yaşanmamış olması, altyapının yüksek trafik dalgalanmalarını (Traffic Spikes) tolere edebilecek sağlamlıkta olduğunu göstermektedir.

---

## ⚡ Veritabanı İndeksleme Deneyi (Performance Monitoring)

Veritabanı indekslemenin (B-Tree) sorgu performansına etkisini gözlemlemek için PostgreSQL `EXPLAIN ANALYZE` komutu kullanılarak bir deney yapılmıştır.

* **Senaryo:** `title` sütunu üzerinden belirli bir kitabın aranması.
* **Veri Seti:** 50.000 Satır.
* **Sorgu:**
    ```sql
    SELECT * FROM "books" WHERE "title" = 'Performans Test Kitabı 45000';
    ```

### 🧪 Sonuçlar ve Karşılaştırma

| Metrik | İndeks Öncesi (Sequential Scan) | İndeks Sonrası (B-Tree Index Scan) | İyileşme |
| :--- | :--- | :--- | :---: |
| **Tarama Türü** | Tüm tablo okunur (Seq Scan) | Doğrudan adrese gidilir (Index Scan) | - |
| **Sorgu Süresi** | **22.742 ms** | **0.100 ms** | **~%99** 🚀 |
| **Planlama Süresi**| 2.294 ms | 4.961 ms | - |

#### 1. İndeks Öncesi (Sequential Scan)
Index olmadığı için veritabanı 50.000 satırın tamamını tek tek kontrol etmek zorunda kalmıştır.
![Sequential Scan](images/indexsiz.png)

#### 2. İndeks Sonrası (B-Tree Optimized)
`title` sütununa B-Tree indeksi eklendikten sonra veri nokta atışı bulunmuştur.
![Index Scan](images/indexli1.png)

---
*Bu proje GMT 458 dersi için Sıla CİCİLİ tarafından hazırlanmıştır.*
