# Inventory Tool

A modern mobile-first inventory reporting system built with ASP.NET Core Razor Pages.

Employees can quickly submit inventory in/out reports from their phones, and admins can manage products, employees, review reports, and export accounting Excel files.

---

# Features

## Employee

* Login with username/password
* Mobile-first UI
* Add inventory items
* Select:

  * Product
  * Direction (In / Out)
  * Quantity
* Submit reports to admin
* View previous reports
* Automatic report locking after admin export

## Admin

* Login with username/password
* Manage products
* Manage employee accounts
* Review submitted reports
* Persian/Jalali date filtering
* Download filtered Excel reports
* Download accounting Excel reports
* Automatic locking of exported reports

---

# Tech Stack

* ASP.NET Core 8 Razor Pages
* PostgreSQL
* Entity Framework Core
* ASP.NET Identity
* ClosedXML
* Bootstrap Icons
* Persian Date Picker
* RTL Persian UI
* Dana Font

---

# Requirements

* .NET 8 SDK
* PostgreSQL
* Docker (optional)

---

# Run Locally

## Clone

```bash
git clone https://github.com/YOUR_USERNAME/inventory-tool.git
cd inventory-tool
```

## Install packages

```bash
dotnet restore
```

## Configure database

Edit:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=inventory_tool;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

---

# Database Migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

# Run

```bash
dotnet run
```

---

# Default Admin

```text
Username: admin
Password: Admin@123456
```

Change it after first login.

---

# Docker

## Build

```bash
docker build -t inventory-tool .
```

## Run

```bash
docker run -d \
--name inventory-tool \
-p 8080:8080 \
-e ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=inventory_tool;Username=postgres;Password=YOUR_PASSWORD" \
inventory-tool
```

---

# Project Structure

```text
Pages
 ├── Account
 ├── Admin
 │    ├── Products
 │    ├── Reports
 │    └── Users
 └── Employee

Models
Data
Services
wwwroot
```

---

# Excel Exports

## Accounting Excel

Exports:

* Only non-exported reports
* Locks reports after first export
* Re-download supported

Columns:

```text
بارکد
کالا
مقدار/تعداد
مقدار/تعداد واحد دوم
فی ورود
فی خروج
توضیحات کالا
```

## Filtered Excel

Exports reports by:

* Date range
* Employee

---

# Persian Features

* RTL layout
* Persian/Jalali date picker
* Persian date conversion
* Dana font
* Mobile-first Persian UI

---

# License

MIT
