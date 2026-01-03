# 🛒 E-Commerce API (ASP.NET Core)

**E-Commerce API** adalah backend REST API berbasis **ASP.NET Core (.NET 9)** yang mensimulasikan sistem e-commerce sederhana namun realistis.
Project ini dirancang sebagai **intermediate-level backend project** dengan fokus pada **JWT Authentication, Authorization berbasis Role, Service Layer, dan complex business logic**.

---

## 🚀 Fitur Utama

* 🔐 JWT Authentication (Register, Login, Token)
* 👥 Role-based Authorization (`Customer`, `Admin`)
* 🛍️ Manajemen Produk
* 📦 Order & Order Items (relasi kompleks)
* 🧠 Service Layer (clean & scalable)
* 🔄 Stock management & order cancellation logic
* 📄 DTO separation (Entity ≠ API response)

---

## 🧩 Teknologi yang Digunakan

* **ASP.NET Core 9**
* **Entity Framework Core 9**
* **PostgreSQL**
* **JWT (Json Web Token)**
* **AutoMapper**
* **BCrypt (Password Hashing)**
* **Swagger / OpenAPI**

---

## 📁 Struktur Folder

```
ECommerceApi/
├── Controllers/
│   ├── AuthController.cs        // Register, Login, Me
│   ├── ProductsController.cs    // CRUD Products
│   ├── OrdersController.cs      // Orders & Order flow
│   └── UsersController.cs       // User management (admin)
│
├── Models/                      // Entity (Database)
│   ├── User.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
│
├── DTOs/                        // API Contract
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   ├── ProductDto.cs
│   └── OrderDto.cs
│
├── Data/
│   └── AppDbContext.cs          // EF Core DbContext
│
├── Services/                    // Business Logic Layer
│   ├── AuthService.cs
│   ├── ProductService.cs
│   └── OrderService.cs
│
├── Helpers/
│   └── JwtHelper.cs             // JWT Generate & Validate
│
├── Middleware/
│   └── JwtMiddleware.cs         // (Optional)
│
├── Program.cs                   // App configuration
└── appsettings.json
```

---

## 🔐 Authentication & Authorization Flow

### 1️⃣ Register

```
POST /api/auth/register
```

* Membuat user baru
* Password di-hash dengan BCrypt
* Role default: `Customer`
* Response berisi **JWT Token + User data**

---

### 2️⃣ Login

```
POST /api/auth/login
```

* Verifikasi username & password
* Generate JWT token
* Update `LastLoginAt`

---

### 3️⃣ Access Protected Endpoint

* Token dikirim via header:

```
Authorization: Bearer <JWT_TOKEN>
```

* JWT akan di-validate oleh middleware
* Claim digunakan untuk:

  * UserId (`NameIdentifier`)
  * Role (`Admin / Customer`)

---

## 👥 Role & Authorization Rules

| Endpoint                  | Customer         | Admin   |
| ------------------------- | ---------------- | ------- |
| Register / Login          | ✅                | ✅       |
| GET /auth/me              | ✅                | ✅       |
| GET /orders               | ✅ (own)          | ✅ (all) |
| POST /orders              | ✅                | ❌       |
| PATCH /orders/{id}/status | ❌                | ✅       |
| Cancel Order              | ✅ (own, pending) | ❌       |

---

## 📦 Order Business Flow

### 🛒 Create Order

1. User membuat order
2. Validasi stok produk
3. Hitung subtotal & total
4. Kurangi stok produk
5. Simpan order & order items

---

### ❌ Cancel Order

* Hanya bisa untuk:

  * Order milik sendiri
  * Status `Pending`
* Stock produk akan dikembalikan

---

### 🚚 Update Order Status (Admin)

* Admin dapat update status:

```
Pending → Processing → Shipped → Delivered
```

---

## 🧠 Service Layer Pattern

Controller **tidak berisi business logic**.

```
Controller
   ↓
Service (logic + validation)
   ↓
DbContext (EF Core)
```

Keuntungan:

* Mudah di-test
* Mudah dikembangkan
* Tidak “fat controller”

---

## 🗄️ Database Relationships

* **User 1 — N Order**
* **Order 1 — N OrderItem**
* **Product 1 — N OrderItem**

> Relasi many-to-many antara `Order` dan `Product` direpresentasikan melalui `OrderItem`.

---

## ⚙️ Setup Project (Initial Setup)

```bash
dotnet new gitignore

dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 9.0.0
dotnet add package EFCore.NamingConventions --version 9.0.0
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 12.0.1
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.6.0
dotnet add package BCrypt.Net-Next --version 4.0.3
dotnet add package Swashbuckle.AspNetCore --version 6.6.2
```

---

## 🔑 JWT Configuration

* **Helpers/JwtHelper.cs**

  * Generate token
  * Claims (`UserId`, `Role`)
* **AuthService.cs**

  * Business logic auth
* **Program.cs**

  * `AddAuthentication`
  * `AddJwtBearer`

---

## 📖 Swagger

Setelah running project:

```
https://localhost:{port}/swagger
```

Digunakan untuk:

* Testing endpoint
* Melihat schema request/response

---

## 🎯 Tujuan Project

Project ini dibuat untuk:

* Latihan backend architecture
* Simulasi real-world e-commerce API
* Portfolio ASP.NET Core
* Persiapan kerja backend developer

---

## ✨ Future Improvements

* Pagination & filtering
* Refresh token
* Payment integration
* Order history & invoice
* Global exception middleware

---

## 👨‍💻 Author

Dibuat sebagai **learning & portfolio project**
menggunakan **ASP.NET Core (.NET 9)**

---

> 💡 *“Build like production, even for learning projects.”*
