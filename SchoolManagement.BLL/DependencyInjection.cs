using Microsoft.Extensions.DependencyInjection;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.BLL.Services;

namespace SchoolManagement.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IExaminationService, ExaminationService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<INoticeService, NoticeService>();
        services.AddScoped<IRoutineService, RoutineService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IGalleryService, GalleryService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IProfileRequestService, ProfileRequestService>();

        return services;
    }
}

