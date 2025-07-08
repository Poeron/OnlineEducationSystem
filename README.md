# Online Education System

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue.svg)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-green.svg)](https://jwt.io/)
[![BCrypt](https://img.shields.io/badge/BCrypt-Password%20Hashing-red.svg)](https://www.nuget.org/packages/BCrypt.Net-Next/)

## 📋 Proje Hakkında

Online Education System, modern web teknolojileri kullanılarak geliştirilmiş kapsamlı bir eğitim yönetim sistemidir. ASP.NET Core 8.0 Web API framework'ü ile geliştirilmiş olan bu sistem, öğrenciler, öğretmenler ve yöneticiler için tam özellikli bir online eğitim platformu sunar.

## 🚀 Özellikler

### 👥 Kullanıcı Yönetimi
- **Çok Rollü Sistemi**: Yönetici, Öğretmen, Öğrenci rolleri
- **Güvenli Kimlik Doğrulama**: JWT tabanlı authentication
- **Şifre Güvenliği**: BCrypt ile hash'lenmiş şifre koruması
- **Kullanıcı Kayıt ve Giriş**: Güvenli register/login işlemleri

### 📚 Kurs Yönetimi
- **Kurs Oluşturma ve Düzenleme**: Öğretmenler için kurs yönetimi
- **Kurs Materyalleri**: Dosya ve içerik yönetimi
- **Kurs Kayıtları**: Öğrenci-kurs ilişkileri
- **Kurs Arşivleme**: Pasif kurs yönetimi

### 📝 Sınav ve Ödev Sistemi
- **Sınav Oluşturma**: Çoktan seçmeli sınav sistemi
- **Soru Bankası**: Soru ve seçenekler yönetimi
- **Ödev Yönetimi**: Ödev verme ve teslim alma
- **Otomatik Değerlendirme**: Sınav sonuçları ve notlandırma

### 🏆 Sertifika ve Başarı Takibi
- **Dijital Sertifikalar**: Kurs tamamlama sertifikaları
- **Başarı Takibi**: Öğrenci performans analizi
- **Dashboard**: Kişiselleştirilmiş kontrol paneli

### 💬 Forum ve İletişim
- **Forum Sistemi**: Kurs bazlı tartışma forumları
- **Yorum Sistemi**: Öğrenci-öğretmen etkileşimi

## 🛠️ Teknolojiler

### Backend
- **ASP.NET Core 8.0**: Web API framework
- **PostgreSQL**: Ana veritabanı
- **Npgsql**: PostgreSQL .NET bağlayıcısı
- **JWT Bearer**: Token tabanlı kimlik doğrulama
- **BCrypt.Net**: Şifre hashleme
- **Swagger/OpenAPI**: API dokümantasyonu

### Güvenlik
- **JWT Authentication**: Güvenli token tabanlı kimlik doğrulama
- **Role-based Authorization**: Rol tabanlı yetkilendirme
- **Password Hashing**: BCrypt ile güvenli şifre koruması
- **CORS**: Cross-Origin kaynak paylaşımı

## 📁 Proje Yapısı

```
OnlineEducationSystem/
├── Controllers/                 # API Controller'ları
│   ├── AuthController.cs       # Kimlik doğrulama
│   ├── CoursesController.cs    # Kurs yönetimi
│   ├── UsersController.cs      # Kullanıcı yönetimi
│   ├── AssignmentsController.cs # Ödev yönetimi
│   ├── ExamsController.cs      # Sınav yönetimi
│   ├── CertificatesController.cs # Sertifika yönetimi
│   └── DashboardController.cs  # Dashboard
├── Models/                      # Veri modelleri
│   ├── Users.cs                # Kullanıcı modelleri
│   ├── Courses.cs              # Kurs modelleri
│   ├── Assignments.cs          # Ödev modelleri
│   ├── Exams.cs                # Sınav modelleri
│   └── Certificates.cs         # Sertifika modelleri
├── Helpers/                     # Yardımcı sınıflar
│   └── DatabaseHelper.cs       # Veritabanı yardımcısı
├── Properties/                  # Proje ayarları
├── Program.cs                   # Uygulama başlangıcı
├── appsettings.json            # Yapılandırma dosyası
└── OnlineEducationSystem.csproj # Proje dosyası
```

## 🔧 Kurulum

### Gereksinimler
- .NET 8.0 SDK
- PostgreSQL 12+
- Visual Studio 2022 veya VS Code

### Kurulum Adımları

1. **Projeyi klonlayın**
```bash
git clone https://github.com/yourusername/OnlineEducationSystem.git
cd OnlineEducationSystem
```

2. **NuGet paketlerini yükleyin**
```bash
dotnet restore
```

3. **Veritabanı bağlantısını yapılandırın**

`appsettings.json` dosyasında PostgreSQL bağlantı string'ini düzenleyin:
```json
{
  "ConnectionStrings": {
    "PostgreSqlConnection": "Host=localhost;Database=OnlineEducation;Username=postgres;Password=your_password"
  }
}
```

4. **Veritabanı şemasını oluşturun**

PostgreSQL veritabanınızda aşağıdaki tabloları oluşturun:

```sql
-- Kullanıcılar tablosu
CREATE TABLE users (
    user_id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL,
    name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Kurslar tablosu
CREATE TABLE courses (
    course_id SERIAL PRIMARY KEY,
    instructor_id INTEGER REFERENCES users(user_id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Kurs kayıtları tablosu
CREATE TABLE courseEnrollments (
    enrollment_id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES users(user_id),
    course_id INTEGER REFERENCES courses(course_id),
    enrollment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Ödevler tablosu
CREATE TABLE assignments (
    assignment_id SERIAL PRIMARY KEY,
    course_id INTEGER REFERENCES courses(course_id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    due_date TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

-- Sınavlar tablosu
CREATE TABLE exams (
    exam_id SERIAL PRIMARY KEY,
    course_id INTEGER REFERENCES courses(course_id),
    title VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);
```

5. **Uygulamayı çalıştırın**
```bash
dotnet run
```

Uygulama `https://localhost:5001` adresinde çalışacaktır.

## 📚 API Dokümantasyonu

Uygulama çalıştıktan sonra Swagger UI'a aşağıdaki adresten erişebilirsiniz:
```
https://localhost:5001/swagger
```

### Ana Endpoint'ler

#### 🔐 Kimlik Doğrulama
- `POST /api/auth/login` - Kullanıcı girişi
- `POST /api/auth/register` - Kullanıcı kaydı

#### 👥 Kullanıcı Yönetimi
- `GET /api/users` - Tüm kullanıcıları listele
- `GET /api/users/{id}` - Belirli kullanıcı detayı
- `POST /api/users` - Yeni kullanıcı oluştur
- `PUT /api/users/{id}` - Kullanıcı güncelle
- `DELETE /api/users/{id}` - Kullanıcı sil

#### 📚 Kurs Yönetimi
- `GET /api/courses` - Tüm kursları listele
- `GET /api/courses/{id}` - Belirli kurs detayı
- `POST /api/courses` - Yeni kurs oluştur
- `PUT /api/courses/{id}` - Kurs güncelle
- `DELETE /api/courses/{id}` - Kurs sil

#### 📝 Ödev Yönetimi
- `GET /api/assignments` - Tüm ödevleri listele
- `POST /api/assignments` - Yeni ödev oluştur
- `GET /api/assignments/student/{studentId}` - Öğrencinin ödevleri

#### 🏆 Sertifika Yönetimi
- `GET /api/certificates` - Sertifikaları listele
- `POST /api/certificates` - Yeni sertifika oluştur

## 🔒 Güvenlik

### JWT Authentication
- Token süresi: 30 dakika
- Symmetric key ile imzalama
- Role-based claims sistemi

### Şifre Güvenliği
- BCrypt ile hash'lenmiş şifreler
- Salt ile güçlendirilmiş güvenlik

### Rol Tabanlı Yetkilendirme
- **Admin**: Tüm işlemler
- **Instructor**: Kurs ve ödev yönetimi
- **Student**: Kurs görüntüleme ve katılım

## 🌐 CORS Yapılandırması

API tüm origin'lara açık olarak yapılandırılmıştır (geliştirme amaçlı). Prodüksiyon ortamında güvenlik için belirli domain'lere kısıtlanmalıdır.

## 🚀 Deployment

### Geliştirme Ortamı
```bash
dotnet run
```

### Prodüksiyon Ortamı
```bash
dotnet publish -c Release
```

## 🤝 Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request oluşturun

## 📞 İletişim

Proje ile ilgili sorularınız için:
- **GitHub Issues**: [Issues sayfası](https://github.com/yourusername/OnlineEducationSystem/issues)
- **Email**: your.email@example.com

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasını inceleyebilirsiniz.

## 🎯 Gelecek Planları

- [ ] Real-time chat sistemi
- [ ] Video konferans entegrasyonu
- [ ] Mobil uygulama desteği
- [ ] Gelişmiş rapor sistemi
- [ ] Email bildirimleri
- [ ] Çoklu dil desteği
- [ ] Tema özelleştirme
- [ ] Gelişmiş analitik dashboard

## 🙏 Teşekkürler

Bu projeyi geliştirirken kullanılan açık kaynak projelere ve topluluk katkılarına teşekkürler.

---

**Not**: Bu README dosyası projenin güncel durumunu yansıtmaktadır. Güncellemeler için düzenli olarak kontrol ediniz.
