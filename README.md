# 🎓 Student Task Management System

### C# | .NET | WPF | Entity Framework Core

A Windows desktop application designed to help teachers manage student assignments and allow students to view their assigned tasks and update their progress.

Built using **C#, Windows Presentation Foundation (WPF), and Entity Framework Core**, this project demonstrates desktop application development, relational database integration, CRUD operations, and role-specific application workflows.

---

## 📌 Project Overview

The Student Task Management System provides separate interfaces for teachers and students to simplify academic assignment management.

Teachers can create, view, update, and manage student assignments, while students can access their tasks and update their assignment status.

The project focuses on integrating a desktop user interface with a relational database using .NET technologies.

## ✨ Key Features

### 👨‍🏫 Teacher Features
- Teacher login interface
- Create new assignments
- View existing assignments
- Update assignment details
- Manage assignments through a dedicated interface
- Access assignment management windows

### 👨‍🎓 Student Features
- Student login interface
- View assigned tasks
- Access assignment information
- Update assignment status
- Track assigned work through a dedicated student interface

### 🗄️ Database Integration
- Entity Framework Core for data access
- Relational models for students, teachers, and assignments
- Database migrations
- LINQ queries
- Database-backed CRUD operations

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| C# | Application logic |
| .NET | Application platform |
| WPF | Desktop user interface |
| XAML | UI design and layout |
| Entity Framework Core | Object-relational mapping |
| LINQ | Data querying |
| Relational Database | Persistent data storage |
| Visual Studio | Development environment |
| Git & GitHub | Version control |

## 🏗️ Application Structure

The project separates its user interface, data access, and entity models.

```text
StudentTaskSystem/
│
├── App/
│   │
│   ├── Data/
│   │   └── App_dbContext.cs
│   │
│   ├── Model/
│   │   ├── Student.cs
│   │   ├── Teacher.cs
│   │   └── Assignment.cs
│   │
│   ├── Migrations/
│   │
│   ├── Windows/
│   │   ├── AuthWindow.xaml
│   │   ├── Student_Window.xaml
│   │   ├── Student_UpdateStatus.xaml
│   │   ├── Teacher_MainWindow.xaml
│   │   ├── Teacher_CrudAssignment.xaml
│   │   ├── Teacher_UpdateAssignment.xaml
│   │   └── Teacher_AllAssignments.xaml
│   │
│   ├── App.xaml
│   └── StudentTaskSystem.csproj
│
└── README.md
```

*The structure above reflects the previously reviewed project version.*

## 🔄 Application Workflow

### Teacher Workflow

1. Open the desktop application.
2. Sign in through the teacher login interface.
3. Access the teacher dashboard.
4. Create or manage assignments.
5. View existing assignments and update their details.

### Student Workflow

1. Open the desktop application.
2. Sign in using student credentials.
3. Access the student interface.
4. View assigned tasks.
5. Update assignment progress or status.

## 💾 Data Management

The application uses Entity Framework Core to connect its data models to a relational database.

The core entities include:

- **Teacher:** Represents teacher account information.
- **Student:** Represents student account information.
- **Assignment:** Represents tasks managed within the system.

The application uses EF Core migrations to manage database schema changes.

## 🚀 Getting Started

### Prerequisites

- Windows operating system
- Visual Studio with .NET desktop development support
- Compatible .NET SDK
- Database server supported by the project's EF Core configuration

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/MMohamedHeshamm/StudentTaskSystem-Desktop-App.git
```

**2. Open the project**

Open the solution in Visual Studio.

**3. Configure the database**

Review the database connection configuration in the project's `App_dbContext.cs` file and adjust it for your local environment.

**4. Apply database migrations**

After confirming the installed EF Core tools and database configuration, apply the project's existing migrations.

**5. Run the application**

Build and start the WPF project using Visual Studio.

## 🔐 Authentication Note

The current project version uses a basic, educational login implementation.

It is intended to demonstrate desktop application workflows and database interaction, rather than production-grade identity management.

For production use, password hashing, secure account provisioning, and stronger authentication and authorization controls would be required.

## 📈 Future Improvements

- Secure password hashing and authentication
- Improved input validation
- Search and filtering for assignments
- Assignment deadlines and priorities
- Unit testing
- Improved exception handling
- Enhanced UI/UX
- More maintainable separation of presentation and data access concerns

## 🎯 What I Learned

This project gave me practical experience in:

- Developing desktop applications using C# and WPF
- Designing interfaces with XAML
- Working with Entity Framework Core
- Managing relational data
- Implementing CRUD operations
- Using LINQ for database queries
- Organizing user interfaces around different application roles

## 👨‍💻 Developed By

**Mohamed H. Eldesouky**

Backend Engineer & Technical Project Coordinator | EVA Pharma

Focused on C#, ASP.NET Core, RESTful APIs, Entity Framework Core, SQL Server, and software engineering principles.

- **GitHub:** [MMohamedHeshamm](https://github.com/MMohamedHeshamm)
- **LinkedIn:** [Mohamed H. Eldesouky](https://www.linkedin.com/in/mohamedhhh/)

---

*An educational desktop application demonstrating C#, WPF, Entity Framework Core, and database-driven assignment management.*
