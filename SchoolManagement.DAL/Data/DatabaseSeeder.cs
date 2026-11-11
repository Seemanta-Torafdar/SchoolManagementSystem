using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.DAL.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // 1. Ensure Database Created
        await context.Database.EnsureCreatedAsync();

        // 2. Seed Roles
        if (!await roleManager.RoleExistsAsync(UserRole.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole.Admin));
        }
        if (!await roleManager.RoleExistsAsync(UserRole.Teacher))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole.Teacher));
        }

        // 3. Seed Admin User
        var adminEmail = "admin@school.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, UserRole.Admin);
            }
        }

        // 4. Seed Academic Sessions
        if (!await context.AcademicSessions.AnyAsync())
        {
            var sessions = new List<AcademicSession>
            {
                new() { SessionName = "2026", Year = 2026, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2026, 12, 31), IsCurrent = true },
                new() { SessionName = "2025", Year = 2025, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), IsCurrent = false }
            };
            await context.AcademicSessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        var currentSession = await context.AcademicSessions.FirstOrDefaultAsync(s => s.IsCurrent) 
                             ?? await context.AcademicSessions.FirstAsync();

        // 5. Seed Classes & Sections
        if (!await context.SchoolClasses.AnyAsync())
        {
            var classes = new List<SchoolClass>
            {
                new()
                {
                    Name = "Class 6",
                    NumericValue = 6,
                    Description = "Lower Secondary Level - Grade 6",
                    Sections = new List<Section>
                    {
                        new() { Name = "Section A", RoomNumber = "Room 101", Capacity = 40 },
                        new() { Name = "Section B", RoomNumber = "Room 102", Capacity = 40 }
                    }
                },
                new()
                {
                    Name = "Class 7",
                    NumericValue = 7,
                    Description = "Lower Secondary Level - Grade 7",
                    Sections = new List<Section>
                    {
                        new() { Name = "Section A", RoomNumber = "Room 103", Capacity = 40 },
                        new() { Name = "Section B", RoomNumber = "Room 104", Capacity = 40 }
                    }
                },
                new()
                {
                    Name = "Class 8",
                    NumericValue = 8,
                    Description = "Junior Secondary Level - Grade 8",
                    Sections = new List<Section>
                    {
                        new() { Name = "Section A", RoomNumber = "Room 201", Capacity = 40 },
                        new() { Name = "Section B", RoomNumber = "Room 202", Capacity = 40 }
                    }
                },
                new()
                {
                    Name = "Class 9",
                    NumericValue = 9,
                    Description = "Secondary Level - Grade 9",
                    Sections = new List<Section>
                    {
                        new() { Name = "Section A", RoomNumber = "Room 301", Capacity = 40 },
                        new() { Name = "Section B", RoomNumber = "Room 302", Capacity = 40 }
                    }
                },
                new()
                {
                    Name = "Class 10",
                    NumericValue = 10,
                    Description = "Secondary SSC Candidate Level - Grade 10",
                    Sections = new List<Section>
                    {
                        new() { Name = "Section A", RoomNumber = "Room 303", Capacity = 40 },
                        new() { Name = "Section B", RoomNumber = "Room 304", Capacity = 40 }
                    }
                }
            };
            await context.SchoolClasses.AddRangeAsync(classes);
            await context.SaveChangesAsync();
        }

        // 6. Seed Teachers & Accounts
        if (!await context.Teachers.AnyAsync())
        {
            var teacherData = new[]
            {
                (Email: "rahman@school.com", First: "Dr. Anisur", Last: "Rahman", Desig: "Senior Teacher (Mathematics)", Qual: "Ph.D in Mathematics, M.Sc, B.Ed", Exp: 14, Phone: "+880 1711-223344", Gender: "Male"),
                (Email: "fatema@school.com", First: "Fatema", Last: "Khatun", Desig: "Senior Teacher (English)", Qual: "M.A in English Literature, B.Ed", Exp: 11, Phone: "+880 1819-334455", Gender: "Female"),
                (Email: "kamal@school.com", First: "Mohammad", Last: "Kamal Uddin", Desig: "Assistant Teacher (Physics)", Qual: "M.Sc in Physics, B.Sc (Hons)", Exp: 8, Phone: "+880 1912-445566", Gender: "Male"),
                (Email: "nasrin@school.com", First: "Nasrin", Last: "Akter", Desig: "Assistant Teacher (ICT)", Qual: "B.Sc in Computer Science & Engineering", Exp: 6, Phone: "+880 1613-556677", Gender: "Female")
            };

            foreach (var t in teacherData)
            {
                var user = await userManager.FindByEmailAsync(t.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = t.Email,
                        Email = t.Email,
                        EmailConfirmed = true,
                        FullName = $"{t.First} {t.Last}",
                        Role = UserRole.Teacher,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var res = await userManager.CreateAsync(user, "Teacher@123456");
                    if (res.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, UserRole.Teacher);
                    }
                }

                var teacher = new Teacher
                {
                    UserId = user.Id,
                    TeacherIdNumber = $"TCH-{new Random().Next(1000, 9999)}",
                    FirstName = t.First,
                    LastName = t.Last,
                    Designation = t.Desig,
                    Qualification = t.Qual,
                    ExperienceYears = t.Exp,
                    Phone = t.Phone,
                    Email = t.Email,
                    Gender = t.Gender,
                    JoiningDate = DateTime.UtcNow.AddYears(-t.Exp),
                    IsActive = true,
                    IsPubliclyVisible = true,
                    Bio = $"Dedicated educator with {t.Exp} years of teaching and mentoring excellence."
                };
                await context.Teachers.AddAsync(teacher);
            }
            await context.SaveChangesAsync();
        }

        // 7. Seed Subjects
        var class10 = await context.SchoolClasses.Include(c => c.Sections).FirstOrDefaultAsync(c => c.NumericValue == 10);
        var teacher1 = await context.Teachers.FirstOrDefaultAsync(t => t.FirstName.Contains("Anisur"));
        var teacher2 = await context.Teachers.FirstOrDefaultAsync(t => t.FirstName.Contains("Fatema"));
        var teacher3 = await context.Teachers.FirstOrDefaultAsync(t => t.FirstName.Contains("Kamal"));
        var teacher4 = await context.Teachers.FirstOrDefaultAsync(t => t.FirstName.Contains("Nasrin"));

        if (!await context.Subjects.AnyAsync() && class10 != null)
        {
            var subjects = new List<Subject>
            {
                new() { Name = "Mathematics", SubjectCode = "MTH-101", TotalMarks = 100, PassMarks = 33, SchoolClassId = class10.Id, TeacherId = teacher1?.Id },
                new() { Name = "English", SubjectCode = "ENG-102", TotalMarks = 100, PassMarks = 33, SchoolClassId = class10.Id, TeacherId = teacher2?.Id },
                new() { Name = "Physics", SubjectCode = "PHY-103", TotalMarks = 100, PassMarks = 33, SchoolClassId = class10.Id, TeacherId = teacher3?.Id },
                new() { Name = "ICT & Computer", SubjectCode = "ICT-104", TotalMarks = 100, PassMarks = 33, SchoolClassId = class10.Id, TeacherId = teacher4?.Id }
            };
            await context.Subjects.AddRangeAsync(subjects);
            await context.SaveChangesAsync();
        }

        // 8. Seed Sample Students
        if (!await context.Students.AnyAsync() && class10 != null)
        {
            var sectionA = class10.Sections.FirstOrDefault() ?? await context.Sections.FirstAsync();
            var students = new List<Student>
            {
                new()
                {
                    AdmissionNumber = "STD-2026-001",
                    RollNumber = 1,
                    FirstName = "Arafat",
                    LastName = "Hossain",
                    FatherName = "Mohammad Hossain",
                    MotherName = "Rokeya Begum",
                    DateOfBirth = new DateTime(2009, 5, 14),
                    Gender = "Male",
                    BloodGroup = "B+",
                    Address = "House 12, Road 4, Sector 7, Uttara, Dhaka",
                    GuardianPhone = "+880 1711-987654",
                    GuardianEmail = "arafat.guardian@gmail.com",
                    SchoolClassId = class10.Id,
                    SectionId = sectionA.Id,
                    AcademicSessionId = currentSession.Id,
                    Status = StudentStatus.Active,
                    VerificationCode = "123456" // Default verification PIN for testing
                },
                new()
                {
                    AdmissionNumber = "STD-2026-002",
                    RollNumber = 2,
                    FirstName = "Sadia",
                    LastName = "Islam",
                    FatherName = "Nurul Islam",
                    MotherName = "Jahanara Begum",
                    DateOfBirth = new DateTime(2009, 8, 22),
                    Gender = "Female",
                    BloodGroup = "A+",
                    Address = "Flat 4B, Dhanmondi R/A, Dhaka",
                    GuardianPhone = "+880 1819-876543",
                    GuardianEmail = "sadia.guardian@gmail.com",
                    SchoolClassId = class10.Id,
                    SectionId = sectionA.Id,
                    AcademicSessionId = currentSession.Id,
                    Status = StudentStatus.Active,
                    VerificationCode = "234567"
                },
                new()
                {
                    AdmissionNumber = "STD-2026-003",
                    RollNumber = 3,
                    FirstName = "Tanvir",
                    LastName = "Ahmed",
                    FatherName = "Kabir Ahmed",
                    MotherName = "Shirin Sultana",
                    DateOfBirth = new DateTime(2009, 3, 10),
                    Gender = "Male",
                    BloodGroup = "O+",
                    Address = "25 Mirpur Road, Dhaka",
                    GuardianPhone = "+880 1912-765432",
                    GuardianEmail = "tanvir.guardian@gmail.com",
                    SchoolClassId = class10.Id,
                    SectionId = sectionA.Id,
                    AcademicSessionId = currentSession.Id,
                    Status = StudentStatus.Active,
                    VerificationCode = "345678"
                }
            };
            await context.Students.AddRangeAsync(students);
            await context.SaveChangesAsync();
        }

        // 9. Seed Examination & Results
        if (!await context.Examinations.AnyAsync() && class10 != null)
        {
            var exam = new Examination
            {
                Name = "First Terminal Examination 2026",
                AcademicSessionId = currentSession.Id,
                StartDate = DateTime.UtcNow.AddDays(-15),
                EndDate = DateTime.UtcNow.AddDays(-5),
                IsPublished = true,
                Description = "First evaluation examination for academic year 2026"
            };
            await context.Examinations.AddAsync(exam);
            await context.SaveChangesAsync();

            var subjects = await context.Subjects.Where(s => s.SchoolClassId == class10.Id).ToListAsync();
            foreach (var sub in subjects)
            {
                await context.ExamSubjects.AddAsync(new ExamSubject
                {
                    ExaminationId = exam.Id,
                    SubjectId = sub.Id,
                    ExamDate = DateTime.UtcNow.AddDays(-10),
                    FullMarks = 100,
                    PassMarks = 33
                });
            }
            await context.SaveChangesAsync();

            var student1 = await context.Students.FirstOrDefaultAsync(s => s.RollNumber == 1);
            if (student1 != null)
            {
                var marks = new[] { 92m, 88m, 95m, 89m };
                for (int i = 0; i < subjects.Count && i < marks.Length; i++)
                {
                    await context.StudentResults.AddAsync(new StudentResult
                    {
                        StudentId = student1.Id,
                        ExaminationId = exam.Id,
                        SubjectId = subjects[i].Id,
                        MarksObtained = marks[i],
                        MaxMarks = 100,
                        Grade = "A+",
                        GradePoint = 5.0m,
                        Status = ResultStatus.Passed,
                        Remarks = "Outstanding performance",
                        IsPublished = true
                    });
                }
                await context.SaveChangesAsync();
            }
        }

        // 10. Seed Notices
        if (!await context.Notices.AnyAsync())
        {
            var notices = new List<Notice>
            {
                new()
                {
                    Title = "First Terminal Examination 2026 Results Published",
                    Content = "The official results for the First Terminal Examination 2026 have been published. Students and parents can check results online using their class, roll number, and private verification PIN.",
                    Category = NoticeType.Examination,
                    Status = NoticeStatus.Published,
                    PublishDate = DateTime.UtcNow.AddDays(-2),
                    IsPublic = true,
                    IsTeacherOnly = false,
                    ViewCount = 342
                },
                new()
                {
                    Title = "Annual Sports & Cultural Week 2026",
                    Content = "The Annual Sports and Cultural Week will commence from next Sunday. All students are encouraged to register their names with the respective physical education coordinators.",
                    Category = NoticeType.Event,
                    Status = NoticeStatus.Published,
                    PublishDate = DateTime.UtcNow.AddDays(-5),
                    IsPublic = true,
                    IsTeacherOnly = false,
                    ViewCount = 215
                },
                new()
                {
                    Title = "Teacher Academic Council Meeting",
                    Content = "All faculty members are requested to attend the academic council meeting scheduled for tomorrow at 2:30 PM in the Conference Hall. Discussion on midterm evaluation syllabus.",
                    Category = NoticeType.TeacherOnly,
                    Status = NoticeStatus.Published,
                    PublishDate = DateTime.UtcNow.AddDays(-1),
                    IsPublic = false,
                    IsTeacherOnly = true,
                    ViewCount = 28
                }
            };
            await context.Notices.AddRangeAsync(notices);
            await context.SaveChangesAsync();
        }

        // 11. Seed Routine
        if (!await context.ClassRoutines.AnyAsync() && class10 != null && teacher1 != null)
        {
            var sectionA = class10.Sections.FirstOrDefault() ?? await context.Sections.FirstAsync();
            var subjects = await context.Subjects.Where(s => s.SchoolClassId == class10.Id).ToListAsync();

            var routines = new List<ClassRoutine>
            {
                new() { SchoolClassId = class10.Id, SectionId = sectionA.Id, SubjectId = subjects[0].Id, TeacherId = teacher1.Id, DayOfWeek = "Sunday", PeriodNumber = 1, StartTime = "09:00 AM", EndTime = "09:45 AM", RoomNumber = "Room 303" },
                new() { SchoolClassId = class10.Id, SectionId = sectionA.Id, SubjectId = subjects[1].Id, TeacherId = teacher2?.Id ?? teacher1.Id, DayOfWeek = "Sunday", PeriodNumber = 2, StartTime = "09:50 AM", EndTime = "10:35 AM", RoomNumber = "Room 303" },
                new() { SchoolClassId = class10.Id, SectionId = sectionA.Id, SubjectId = subjects[2].Id, TeacherId = teacher3?.Id ?? teacher1.Id, DayOfWeek = "Monday", PeriodNumber = 1, StartTime = "09:00 AM", EndTime = "09:45 AM", RoomNumber = "Room 303" },
                new() { SchoolClassId = class10.Id, SectionId = sectionA.Id, SubjectId = subjects[3].Id, TeacherId = teacher4?.Id ?? teacher1.Id, DayOfWeek = "Monday", PeriodNumber = 2, StartTime = "09:50 AM", EndTime = "10:35 AM", RoomNumber = "Room 303" }
            };
            await context.ClassRoutines.AddRangeAsync(routines);
            await context.SaveChangesAsync();
        }

        // 12. Seed Events, Gallery & Management
        if (!await context.Events.AnyAsync())
        {
            var events = new List<Event>
            {
                new() { Title = "Science & Robotics Fair 2026", Description = "Interactive science exhibitions, innovative student projects, and robotics demonstrations.", EventDate = DateTime.UtcNow.AddDays(14), Location = "Main Campus Ground", IsPublic = true },
                new() { Title = "Inter-School Debate Championship", Description = "Prestigious debate contest featuring 16 top schools debating critical modern challenges.", EventDate = DateTime.UtcNow.AddDays(25), Location = "Auditorium", IsPublic = true }
            };
            await context.Events.AddRangeAsync(events);
        }

        if (!await context.Galleries.AnyAsync())
        {
            var galleries = new List<Gallery>
            {
                new() { Title = "Annual Prize Giving Ceremony", Category = "Achievements", ImageUrl = "/images/gallery/prize_ceremony.jpg", Caption = "Top achieving students receiving academic distinction awards.", IsActive = true },
                new() { Title = "Digital Computer Lab", Category = "Campus", ImageUrl = "/images/gallery/computer_lab.jpg", Caption = "State-of-the-art modern computer lab equipped with high-speed internet.", IsActive = true },
                new() { Title = "Annual Sports Meet", Category = "Sports", ImageUrl = "/images/gallery/sports_day.jpg", Caption = "Track and field athletics event finals.", IsActive = true }
            };
            await context.Galleries.AddRangeAsync(galleries);
        }

        if (!await context.ManagementMembers.AnyAsync())
        {
            var members = new List<ManagementMember>
            {
                new()
                {
                    Name = "Prof. Dr. M. A. Karim",
                    Designation = "Principal",
                    Message = "Welcome to our institution. For over three decades, we have been nurturing young minds with knowledge, integrity, and future-ready skills to shape bright futures together.",
                    DisplayOrder = 1,
                    IsActive = true,
                    Email = "principal@school.com",
                    Phone = "+880 2-9876543"
                },
                new()
                {
                    Name = "Mrs. Shamima Begum",
                    Designation = "Vice Principal",
                    Message = "We believe every child possesses unique potential. Our holistic academic environment is built to inspire curiosity, creativity, and moral excellence.",
                    DisplayOrder = 2,
                    IsActive = true,
                    Email = "viceprincipal@school.com",
                    Phone = "+880 2-9876544"
                }
            };
            await context.ManagementMembers.AddRangeAsync(members);
        }

        await context.SaveChangesAsync();
    }
}

