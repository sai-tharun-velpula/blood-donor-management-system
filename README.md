# Blood Donor Management System

A web-based Blood Donor Management System developed using **ASP.NET Web Forms**, **C#**, **ADO.NET**, **.NET Framework 4.8**, and **SQL Server**.

The system is designed to manage blood donors, donor accounts, blood groups, availability, reports, and administrative operations through a role-based interface.

---

## Technology Stack

- ASP.NET Web Forms
- C#
- .NET Framework 4.8
- ADO.NET
- SQL Server / LocalDB
- HTML5
- CSS3
- JavaScript
- Visual Studio 2019

---

## Main Features

- Secure login and logout
- Role-based access
- Admin dashboard
- Donor dashboard
- Donor registration
- Donor profile management
- Donor directory
- Donor search and filtering
- Blood group filtering
- City and state filtering
- Donor availability filtering
- Donor account activation/deactivation
- Donor details view
- Donor edit functionality
- Donor delete functionality
- Delete confirmation
- Blood group reports
- City-wise reports
- Dashboard donor statistics
- Responsive MNC-style user interface
- Desktop, laptop, tablet and mobile layouts
- SQL Server database integration
- Sample donor records
- Forms Authentication
- Session-based login state

---

# Role-Based Functionality

## 1. Admin

The Admin has access to the complete donor management system.

### Admin Login

The Admin can:

- Login using administrator credentials
- Access the Admin Dashboard
- Logout securely
- Access administrative donor management features

### Admin Dashboard

The dashboard provides an overview of the donor system, including donor statistics and management information.

Admin can navigate to:

- Dashboard
- Donor Directory
- Donor Registration
- Reports
- Other available administrative functions

### Donor Management

Admin can:

- Register a new donor
- View registered donors
- Search donors
- Filter donors
- View complete donor details
- Edit donor information
- Delete donor records
- Activate donor accounts
- Deactivate donor accounts

### Donor Directory

The Admin Donor Directory supports searching and filtering by:

- Donor name
- Mobile number
- Email
- Blood group
- City
- State
- Availability

The directory displays donor information such as:

- Donor name
- Blood group
- Mobile
- Email
- City
- State
- Age
- Donation availability
- Account status
- Actions

Available actions include:

- Edit
- View
- Toggle account status

### Donor Details

Admin can view detailed donor information including:

- Donor ID
- Full name
- Blood group
- Gender
- Date of birth
- Age
- Mobile
- Email
- City
- State
- Pincode
- Address
- Availability

### Donor Account Status

Admin can manage donor account status.

Accounts can be:

- Active
- Inactive

The directory visually displays the current account status.

### Donor Delete

Admin can delete donor records.

A confirmation step is provided before deletion to help prevent accidental deletion.

---

# 2. Donor

The Donor role is intended for registered blood donors.

Donor functionality includes access to their donor-related information and the donor-facing features implemented in the project.

Depending on the configured account permissions, donors can:

- Login
- Access the donor dashboard
- View their donor information
- Manage their donor profile
- Check their blood group
- Check their availability information
- Logout

---

# Authentication

The application uses ASP.NET Forms Authentication for login management.

The login flow includes:

1. User enters username and password.
2. Credentials are checked against the database.
3. A successful login displays a success message.
4. The login process continues through the configured authentication flow.
5. The user is redirected to the appropriate dashboard based on their role.
6. Logged-in users cannot access pages intended for unauthenticated users.

The application also checks whether a user is already logged in before displaying the login page.

---

# Dashboard

The dashboard provides a central overview of the system.

It is designed to display donor-related statistics and provide quick navigation to important modules.

Typical dashboard functionality includes:

- Total donors
- Available donors
- Blood group information
- Donor management navigation
- Reports navigation

---

# Donor Registration

The donor registration module allows authorized users to create donor records.

Donor information includes:

- Full name
- Blood group
- Gender
- Date of birth
- Mobile number
- Email
- Address
- City
- State
- Pincode
- Availability information

Registered donor records are stored in SQL Server.

---

# Donor Search

The Donor Directory provides multiple search and filtering options.

### Search

Users can search using:

- Donor name
- Mobile number
- Email

### Filters

Available filters include:

- Blood group
- City
- State
- Availability

A Clear option is also available to reset the search filters.

---

# Reports

The project includes donor reporting functionality.

Available reports include:

## Blood Group Report

Provides donor information grouped or summarized according to blood group.

Blood groups supported include:

- A+
- A-
- B+
- B-
- AB+
- AB-
- O+
- O-

## City Report

Provides donor information based on donor city/location.

These reports help administrators understand donor distribution and availability.

---

# User Interface

The application uses a responsive, professional MNC-style design.

The UI has been designed for:

- Desktop
- Laptop
- Tablet
- Mobile

The project includes reusable styling for:

- Cards
- Forms
- Buttons
- Tables
- Search filters
- Status badges
- Dashboard sections
- Empty states
- Detail panels
- Responsive layouts

The Donor Directory table also uses consistent typography and styling across donor fields.

---

# Database

The application uses SQL Server for persistent data storage.

The database contains the main application data required for authentication and donor management.

Main database objects include:

- `Users`
- `Donors`

The database setup script is included in the project.

---

# Database Setup

1. Open SQL Server Management Studio or SQL Server Object Explorer.

2. Open:

`BloodDonorManagementSystem/BloodDonorManagementSystem/App_Data/BloodDonorManagement.sql`

3. Execute the SQL script.

4. The script creates the required database and tables.

5. Sample records are also inserted for testing.

6. Check the connection string in `Web.config`.

The default project connection uses:

`(localdb)\MSSQLLocalDB`

If a different SQL Server instance is being used, update the `BloodDonorDb` connection string in `Web.config`.

---

# Project Structure

The project follows an organized ASP.NET Web Forms structure.

Important components include:

- `Login.aspx` - Login page
- `Dashboard.aspx` - Dashboard
- `Donors.aspx` - Donor Directory
- `DonorRegistration.aspx` - Donor registration
- `Reports.aspx` - Reports
- `Site.Master` - Common application layout
- `Web.config` - Application configuration and connection string
- `App_Data/BloodDonorManagement.sql` - Database setup script
- `Infrastructure` - Authentication and supporting application utilities

---

# Development Environment

Recommended environment:

- Visual Studio 2019
- .NET Framework 4.8
- SQL Server / LocalDB
- SQL Server Management Studio

Open the solution file:

`BloodDonorManagementSystem.sln`

---

# How to Run

1. Clone or download the repository.

2. Open:

`BloodDonorManagementSystem.sln`

in **Visual Studio 2019**.

3. Restore NuGet packages if Visual Studio requests it.

4. Configure the SQL Server connection string in `Web.config`.

5. Execute the database script:

`App_Data/BloodDonorManagement.sql`

6. Build the solution.

7. Run the application using:

- `F5`
- or `Ctrl + F5`

8. Login using the configured account.

9. Use the dashboard to access the available modules.

---

# Default Admin Login

For the project/demo environment:

**Username**

`admin`

**Password**

`Admin@123`

> This credential is intended for development/demo use. The password implementation and credentials should be changed before production deployment.

---

# Security Notes

The project includes authentication and access control for the application.

For production deployment, additional security hardening should be performed, including:

- Strong password hashing
- Secure password storage
- HTTPS
- Production database credentials
- Connection string protection
- Input validation
- Proper authorization checks
- Secure session configuration
- Error handling and logging

---

# Project Status

The project currently includes the core Blood Donor Management System functionality:

- Authentication
- Role-based navigation
- Dashboard
- Donor registration
- Donor directory
- Donor search and filtering
- Donor details
- Donor editing
- Donor deletion
- Account activation/deactivation
- Reports
- SQL Server integration
- Responsive UI

---

# License

This project is developed for educational/project demonstration purposes.