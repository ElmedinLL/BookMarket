# Book Market

A full-stack ASP.NET Core MVC web application for managing a book marketplace. The project demonstrates clean architecture, database-driven development, and role-based user management using modern .NET practices.



## 📘 Project Overview

Book Market is a web application that allows administrators to manage books, categories, and users while providing a structured foundation for an e-commerce–style system. The project focuses on backend architecture, data modeling, and secure access control, following real-world development standards.

---

## ✨ Features

### Authentication & Authorization

- User registration and login
- Role-based access control (Admin / User)
- Secure authorization using ASP.NET Identity
- User lock and unlock functionality

### User Management

- View and manage application users
- Display assigned roles
- Administrative user controls

### Book Management

- Create, read, update, and delete books
- Category-based organization
- Server-side validation
- Clean separation of domain models

### Architecture & Code Structure

- MVC (Model–View–Controller) architecture
- Repository pattern
- Unit of Work pattern
- Dependency Injection
- Separation of concerns across layers

---

## 🧱 Tech Stack

| Layer | Technology |
|-------|------------|
| Language | C# |
| Framework | ASP.NET Core MVC |
| ORM | Entity Framework Core |
| Auth | ASP.NET Core Identity |
| Database | SQL Server |
| UI | Razor Views, Bootstrap |

---

## 📁 Project Structure

```
BookMarket/
├── Controllers/
├── Models/
├── Data/
│   └── ApplicationDbContext.cs
├── Repositories/
├── Services/
├── Views/
│   ├── Books/
│   ├── Categories/
│   └── Account/
├── wwwroot/
└── Program.cs
```

---

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (v8.0+)
- [SQL Server](https://www.microsoft.com/sql-server) (or SQL Server Express)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/BookMarket.git
cd BookMarket
```

### 2. Configure the database

Update `appsettings.json` with your connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=BookMarketDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Run migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The app runs at `https://localhost:7xxx` (port shown in terminal).

---

## Default Roles

| Role | Description |
|------|-------------|
| Admin | Full access to books, categories, and user management |
| User | Standard access to browse and interact with books |



MIT
