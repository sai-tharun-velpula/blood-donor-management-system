# Blood Donor Management System

A web-based Blood Donor Management System built with **ASP.NET Web Forms**, **C#**, **ADO.NET**, **.NET Framework 4.8**, and **Microsoft SQL Server**. Designed to streamline blood donor registration, search, and reporting through a secure, role-based interface for administrators and donors.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

---

## 📌 Overview

Blood Donor Management System is a centralized platform for managing blood donor records. It replaces manual donor tracking with a secure web application that handles donor registration, search and filtering, availability tracking, and reporting — with role-based access for administrators and donors.

## ✨ Features

- **Donor Registration** — Capture full donor details including blood group, contact, and address
- **Donor Directory** — Search and filter donors by name, mobile, email, blood group, city, state, and availability
- **Donor Profile Management** — Edit and update donor information
- **Account Status Control** — Activate or deactivate donor accounts
- **Admin Dashboard** — Overview of donor statistics and quick navigation
- **Reports** — Blood group–wise and city-wise donor reports
- **Authentication & Authorization** — Forms Authentication with role-based access for Admin and Donor users
- **Responsive UI** — Responsive interface designed for desktop, laptop, tablet, and mobile devices

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Web Forms, C# |
| Data Access | ADO.NET |
| Database | Microsoft SQL Server / LocalDB |
| Frontend | HTML5, CSS3, JavaScript |
| IDE | Visual Studio 2019 |

## 🏗️ Project Structure
```text
BloodDonorManagementSystem/
│
├── App_Data/
│   └── BloodDonorManagement.sql       # SQL Server database setup script
│
├── Content/
│   ├── site.css                       # Application styling and responsive UI
│   └── site.js                        # Client-side JavaScript
│
├── Infrastructure/
│   ├── AuthHelper.cs                  # Authentication and role handling
│   ├── Db.cs                          # Database connection and data access
│   └── PasswordHelper.cs              # Password hashing and verification
│
├── Models/
│   └── Donor.cs                       # Donor data model
│
├── Login.aspx                         # User login
├── Dashboard.aspx                     # Admin dashboard
├── DonorDashboard.aspx                # Donor dashboard
├── DonorRegistration.aspx             # Donor registration
├── DonorSearch.aspx                   # Donor search
├── Donors.aspx                        # Donor directory and management
├── Reports.aspx                       # Blood group and city reports
├── ChangePassword.aspx                # Password change
├── Logout.aspx                        # Sign out
│
├── Site.Master                        # Shared application layout
├── Global.asax                        # Application-level configuration
├── Web.config                         # Application and database configuration
│
├── BloodDonorManagementSystem.csproj  # Project file
└── BloodDonorManagementSystem.sln     # Visual Studio solution
```


## 🚀 Getting Started

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.8
- SQL Server or SQL Server Express (LocalDB)

### Installation

1. **Clone the repository**
```bash
   git clone https://github.com/sai-tharun-velpula/blood-donor-management-system.git
```

2. **Open the solution**

   Open `BloodDonorManagementSystem.sln` in Visual Studio.

3. **Restore NuGet packages** if prompted.

4. **Set up the database**

   Run the script at:BloodDonorManagementSystem/App_Data/BloodDonorManagement.sql    This creates the required tables and inserts sample donor records.

5. **Configure the connection string**

   Update `Web.config` if you're not using the default LocalDB instance:
```xml
   <connectionStrings>
  <add name="BloodDonorDb"
       connectionString="Server=.;Database=BloodDonorDB;Trusted_Connection=True;TrustServerCertificate=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

6. **Run the application**

   Press `F5` or `Ctrl+F5` in Visual Studio.

## 🔐 Default Login (Demo Only)

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin@123` |

> ⚠️ These credentials are for development/demo purposes only. Change them before any production deployment.

## 📸 Screenshots

> _Add screenshots of the dashboard, donor directory, and login page here to give visitors a quick visual tour._

## 🗺️ Roadmap

- [x] Implement Admin and Donor authentication
- [x] Implement role-based access control
- [x] Implement donor registration and management
- [x] Implement donor search and filtering
- [x] Implement donor profile management
- [x] Implement blood group and city-wise reports
- [x] Implement responsive application UI
- [x] Integrate SQL Server database

## 🔒 Security Notes

The application includes basic security and access-control mechanisms for managing donor information:

- **Forms Authentication** — Authenticated users are required to log in before accessing protected application pages
- **Role-Based Access Control** — Admin and Donor users have different access permissions
- **Session-Based Login State** — User authentication state is maintained during the session
- **Password Protection** — Passwords are handled through the application's password helper functionality
- **Authorization Checks** — Protected pages and administrative functions verify the user's authentication and role
- **SQL Server Database** — Application data is stored in Microsoft SQL Server
- **Logout Support** — Users can securely sign out of the application

> ⚠️ This project is intended for educational and demonstration purposes. Additional security hardening is recommended before production deployment, including HTTPS enforcement, secure production credentials, protected connection strings, comprehensive input validation, and stronger production-grade authentication policies.

## 🤝 Contributing

Contributions, issues, and feature requests are welcome. Feel free to check the [issues page](../../issues) or open a pull request.

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Sai Tharun Velpula**
- GitHub: [@sai-tharun-velpula](https://github.com/sai-tharun-velpula)
