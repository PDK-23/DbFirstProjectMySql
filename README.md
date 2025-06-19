# DbFirstProjectMySql

## Giới thiệu

DbFirstProjectMySql là một hệ thống Web API quản lý người dùng, phân quyền và sản phẩm, xây dựng bằng ASP.NET Core với kiến trúc nhiều tầng và sử dụng Entity Framework Core Database First để kết nối cơ sở dữ liệu SQL Server.

## Tính năng nổi bật

* Đăng ký, đăng nhập, phân quyền qua JWT
* Mã hóa mật khẩu với BCrypt
* Quản lý người dùng và vai trò (Admin/User)
* Quản lý sản phẩm: CRUD sản phẩm gắn với người tạo
* Đầy đủ API lấy danh sách user, role, product
* Database và bảng từ MySQL (Database First, scaffolded)
* Mapping Entity ↔ DTO với AutoMapper
* Định nghĩa Unit of Work & Repository cho thao tác dữ liệu an toàn
* Tài liệu API với Swagger

## Kiến trúc & Công nghệ

* **ASP.NET Core Web API** (.NET 9.0)
* **Entity Framework Core (EF Core) – Database First (MySQL via Pomelo.EntityFrameworkCore.MySql)**
* **AutoMapper** (mapping entity/DTO)
* **JWT Bearer Authentication**
* **BCrypt.Net** (hash password)
* **Swagger / Swashbuckle** (API document)

**Kiến trúc nhiều tầng:**

* **Domain**: Entities((scaffolded từ MySQL)), Enums
* **Infrastructure**: AppDbContext(generated), Repository, UnitOfWork
* **Application**: DTOs, Services, Interfaces, Mapping
* **API**: Controllers, Config, Middleware

## Mô hình cơ sở dữ liệu (Code First)

* **Role** (`Id`, `Name`)
* **User** (`Id`, `Username`, `PasswordHash`, `RoleId`)
* **Product** (`Id`, `Name`, `Description`, `Price`, `UserId`)

**Quan hệ:**

* Mỗi User thuộc một Role
* Mỗi Product gắn với một User (là người tạo)

Dữ liệu Role được seed tự động (`Admin` = 0, `User` = 1) từ Enum.

## Hướng dẫn cài đặt & chạy

### 1. Clone & cấu hình repo

```bash
git clone https://github.com/PDK-23/DbFirstProjectMySql.git
cd DbFirstProjectMySql
```

### 2. Tạo file cấu hình

* Copy `appsettings.json.example` → `appsettings.json`
* **Không** public file chứa key thật lên GitHub
* Chỉnh sửa:

  * Connection string MySQL (Aiven)
  * JWT key bảo mật 
    
* Chỉnh sửa connection string & JWT key cho phù hợp

### 3. Cài đặt dependencies

```bash
dotnet restore
```

### 4. Tạo database bằng migration (EF Core Code First)

```bash
dotnet ef database update \
  --project ./DbFirstProjectMySql.Infrastructure \
  --startup-project ./DbFirstProjectMySql.Api
```

### 5. Chạy ứng dụng

```bash
dotnet run --project ./DbFirstProjectMySql.Api
```

Truy cập Swagger tại: `https://localhost:7227/swagger/index.html` (hoặc cổng khác tùy cấu hình).

## Ví dụ response API

### Đăng ký user

**Endpoint:** `POST /api/auth/register`

**Request body:**

```json
{
  "username": "khoidev",
  "password": "123456",
  "roleId": 1
}
```

**Response:**

```json
"Register success"
```

### Đăng nhập

**Endpoint:** `POST /api/auth/login`

**Request body:**

```json
{
  "username": "khoidev",
  "password": "123456"
}
```

**Response:**

```json
"<JWT Token>"
```

### Lấy tất cả sản phẩm

**Endpoint:** `GET /api/product`

**Response:**

```json
[
  { "id": 1, "name": "iPhone 16", "description": "Hàng mới", "price": 500, "userId": 2 }
]
```

## Cách hoạt động Database First

* Cơ sở dữ liệu được thiết kế và tạo trong SQL Server
* Sử dụng scaffold để sinh DbContext & entities từ DB
* Khi schema thay đổi, chạy lại scaffold để cập nhật code
* Dữ liệu seed (Role) được chèn trực tiếp vào bảng hoặc thông qua script SQL

## Một số lệnh dev nhanh

* Sinh lại code từ database:

  ```bash
  dotnet ef dbcontext scaffold "YourConnectionString" Pomelo.EntityFrameworkCore.MySql \
  --project ./DbFirstProjectMySql.Infrastructure \
  --startup-project ./DbFirstProjectMySql.Api \
  --output-dir Entities --context DemoShopDbContext --data-annotations --use-database-names
  ```

* Xoá model cũ và scaffold lại:
  
  ```bash
  rm -rf ./DbFirstProjectMySql.Infrastructure/Entities
  # rồi chạy lại lệnh scaffold bên trên
  ```

## Lưu ý bảo mật

* Không push file `appsettings.json` thật lên GitHub – chỉ dùng file mẫu và biến môi trường.
* JWT key và connection string nên được quản lý qua User Secrets hoặc biến môi trường.

## Tác giả

* **Name:** Pham-Dang-Khoi
* **GitHub:** https://github.com/PDK-23/
* **Deploy URL:** https://products-api-bbdqc2h4csexbte6.eastasia-01.azurewebsites.net/index.html
