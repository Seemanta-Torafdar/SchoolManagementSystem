using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Domain.Enums;
using System.Reflection;
using Xunit;

namespace SchoolManagement.Tests.AuthorizationTests;

public class PermissionMatrixTests
{
    private readonly Assembly _webAssembly = typeof(SchoolManagement.Web.Controllers.HomeController).Assembly;

    [Fact]
    public void AdminControllers_MustAllHaveAreaAdminAndAuthorizeRoleAdmin()
    {
        var adminControllers = _webAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Controller).IsAssignableFrom(t))
            .Where(t => t.Namespace != null && t.Namespace.Contains(".Areas.Admin.Controllers"))
            .ToList();

        Assert.NotEmpty(adminControllers);

        foreach (var controller in adminControllers)
        {
            var areaAttr = controller.GetCustomAttribute<AreaAttribute>();
            Assert.True(areaAttr != null && areaAttr.RouteValue == "Admin",
                $"Controller {controller.Name} must have [Area(\"Admin\")] attribute.");

            var authAttr = controller.GetCustomAttribute<AuthorizeAttribute>();
            Assert.True(authAttr != null && (authAttr.Roles?.Contains(UserRole.Admin) == true),
                $"Controller {controller.Name} must have [Authorize(Roles = UserRole.Admin)].");
        }
    }

    [Fact]
    public void TeacherControllers_MustAllHaveAreaTeacherAndAuthorizeRoleTeacher()
    {
        var teacherControllers = _webAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Controller).IsAssignableFrom(t))
            .Where(t => t.Namespace != null && t.Namespace.Contains(".Areas.Teacher.Controllers"))
            .ToList();

        Assert.NotEmpty(teacherControllers);

        foreach (var controller in teacherControllers)
        {
            var areaAttr = controller.GetCustomAttribute<AreaAttribute>();
            Assert.True(areaAttr != null && areaAttr.RouteValue == "Teacher",
                $"Controller {controller.Name} must have [Area(\"Teacher\")] attribute.");

            var authAttr = controller.GetCustomAttribute<AuthorizeAttribute>();
            Assert.True(authAttr != null && (authAttr.Roles?.Contains(UserRole.Teacher) == true),
                $"Controller {controller.Name} must have [Authorize(Roles = UserRole.Teacher)].");
        }
    }

    [Fact]
    public void PublicControllers_MustNotBeMarkedWithAdminOrTeacherArea()
    {
        var publicControllers = _webAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(Controller).IsAssignableFrom(t))
            .Where(t => t.Namespace == "SchoolManagement.Web.Controllers")
            .ToList();

        Assert.NotEmpty(publicControllers);

        foreach (var controller in publicControllers)
        {
            var areaAttr = controller.GetCustomAttribute<AreaAttribute>();
            Assert.Null(areaAttr);
        }
    }
}

