# School Management System (SMS)

A complete, professional, secure, and production-ready enterprise educational institution management system built with **ASP.NET Core MVC**, **Entity Framework Core**, **Microsoft SQL Server**, **ASP.NET Core Identity**, and **Bootstrap 5**.

---

## 🏛️ Architectural Overview

The solution adheres strictly to clean layered architectural principles inspired by enterprise modular design:

```text
SchoolManagementSystem/
│
├── SchoolManagementSystem.sln             # Master Solution
│
├── SchoolManagement.Domain/               # Core Domain Layer (Entities, Enums, Zero External Dependencies)
│   ├── Common/                            # BaseEntity, AuditableEntity, ResultResponse
│   ├── Enums/                             # UserRole, NoticeType, NoticeStatus, StudentStatus, ResultStatus, RequestStatus
│   └── Entities/                          # ApplicationUser, Student, Teacher, SchoolClass, Section, Subject,
│                                          # Examination, ExamSubject, StudentResult, ClassRoutine, Notice,
│                                          # Event, Gallery, ManagementMember, ProfileUpdateRequest, AuditLog
│
├── SchoolManagement.DAL/                  # Data Access Layer (EF Core, Repositories, Database Seeding)
│   ├── Data/                              # ApplicationDbContext, DatabaseSeeder
│   │   └── Configurations/                # Fluent API EntityTypeConfigurations
│   ├── Interfaces/                        # IGenericRepository, IStudentRepository, ITeacherRepository, etc.
│   ├── Repositories/                      # GenericRepository, StudentRepository, TeacherRepository, etc.
│   └── DependencyInjection.cs             # Scoped DAL registrations
│
├── SchoolManagement.BLL/                  # Business Logic Layer (Services, DTOs, Business Rules, Helpers)
│   ├── DTOs/                              # Student, Teacher, Class, Subject, Examination, Result, Notice, etc.
│   ├── Helpers/                           # GradeCalculator, ResultCalculator, FileValidationHelper, PaginationHelper
│   ├── Interfaces/                        # IStudentService, ITeacherService, IResultService, IAccountService, etc.
│   ├── Services/                          # Service implementations enforcing institutional business logic
│   └── DependencyInjection.cs             # Scoped BLL registrations
│
├── SchoolManagement.Web/                  # Presentation Layer (MVC, Areas: Admin & Teacher, Public Website)
│   ├── Areas/
│   │   ├── Admin/                         # Full Administrative Operations (16 Controllers & Responsive Views)
│   │   └── Teacher/                       # Faculty Portal (8 Controllers & Responsive Views)
│   ├── Controllers/                       # 10 Public Controllers (Home, Academics, Teachers, Results, Notices, etc.)
│   ├── Middleware/                        # GlobalExceptionMiddleware
│   ├── ViewModels/                        # Strongly-typed UI ViewModels with DataAnnotations
│   ├── Views/                             # Razor Views, Partial Views, and Custom Multi-Role Layouts
│   ├── wwwroot/                           # CSS, Custom Color Theme, JS, Chart.js integrations, Uploads
│   ├── Program.cs                         # Dependency Injection, Identity, RateLimiter, Database Auto-Seeder
│   └── appsettings.json                   # SQL Server ConnectionString & School Metadata
│
├── SchoolManagement.API/                  # Optional REST API Starter (Ready for Mobile / External integrations)
│
└── SchoolManagement.Tests/                # Automated Test Suite (Unit & Architecture Tests)
    ├── UnitTests/                         # GradeCalculatorTests, ResultCalculatorTests
    └── AuthorizationTests/                # PermissionMatrixTests (Role & Area RBAC Verification)
```

---

## 🎨 Institutional Design System & Theme

- **Primary Purple:** `#7138E8`
- **Secondary Purple:** `#9B6BFA`
- **Deep Navy:** `#151D35`
- **Background Slate:** `#F5F7FC`
- **Surface Clean White:** `#FFFFFF`
- **Body Charcoal Text:** `#17213B`
- **Muted Text:** `#718096`
- **Success Green:** `#16A36A`
- **Warning Gold:** `#F5A623`

---

## 🔑 Demo System Credentials

The database seeder automatically initializes the administrator and faculty accounts upon startup:

| Portal Role | Email Address | Password | Permissions |
| :--- | :--- | :--- | :--- |
| **System Administrator** | `admin@school.com` | `Admin@123456` | Full CRUD on students, teachers, exams, results publication, user accounts, audit trail, academic sessions |
| **Faculty / Senior Teacher** | `rahman@school.com` | `Teacher@123456` | Assigned subjects marks entry, class rosters, timetable, profile update requests, internal circulars |
| **Faculty / Teacher** | `fatema@school.com` | `Teacher@123456` | Assigned subjects marks entry, class rosters, timetable, profile update requests |
| **Faculty / Teacher** | `kamal@school.com` | `Teacher@123456` | Assigned subjects marks entry, class rosters, timetable, profile update requests |
| **Faculty / Teacher** | `nasrin@school.com` | `Teacher@123456` | Assigned subjects marks entry, class rosters, timetable, profile update requests |

---

## 🛡️ Security & Privacy Engineering

1. **Part 8 Student Result Privacy & Verification PIN:**
   - Public result lookups **never display all students in a class**.
   - Requires 5 parameters: Examination, Session, Class, Roll Number, and the student's confidential **Verification PIN** (e.g. `STU-2026-001`).
   - Rate limiting via ASP.NET Core `FixedWindowRateLimiter` (`ResultSearchLimiter`) to prevent automated credential enumeration or scraping.
   - Unpublished results remain strictly invisible on public endpoints until the Administrator explicitly clicks **Publish to Public Portal**.
2. **Role-Based Access Control (RBAC):**
   - Separate Razor layouts for Public (`_PublicLayout.cshtml`), Administrator (`_AdminLayout.cshtml`), and Teacher (`_TeacherLayout.cshtml`).
   - Strict `[Area("Admin")]` + `[Authorize(Roles = UserRole.Admin)]` and `[Area("Teacher")]` + `[Authorize(Roles = UserRole.Teacher)]` annotations.
3. **Audit Trail Logging:**
   - Automatic recording of administrative actions (marks entry, publishing exams, adding users, status changes) with IP address, user identifier, entity name, and timestamp.
4. **Data Protection & Soft Deletes:**
   - Soft-delete filters on `ApplicationDbContext` ensure student and exam history is never permanently lost.

---

## 🚀 How to Run the Application

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/)
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/) (or LocalDB / SQL Server Express)

### Configuration
1. Open `SchoolManagement.Web/appsettings.json` and ensure the connection string points to your SQL Server instance:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SchoolManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
   }
   ```
2. Build the solution:
   ```powershell
   dotnet build SchoolManagementSystem.sln -m:1
   ```
3. Run the Web Application:
   ```powershell
   dotnet run --project SchoolManagement.Web/SchoolManagement.Web.csproj
   ```
4. Access the web browser at:
   - Public School Website: `https://localhost:5001` or `http://localhost:5000`
   - Portal Login: `https://localhost:5001/Account/Login`
   - Marksheet Search: `https://localhost:5001/Results`

---

## 🧪 Testing

The solution includes automated unit and architecture verification tests in `SchoolManagement.Tests`:
- **Grade Calculator:** Exact thresholds for `A+`, `A`, `A-`, `B`, `C`, `D`, and `F`.
- **Result Calculator:** Proper GPA calculation and failing subject fail-safe rules.
- **Permission Matrix:** Reflection tests verifying area constraints and role authorizations across all controllers.

