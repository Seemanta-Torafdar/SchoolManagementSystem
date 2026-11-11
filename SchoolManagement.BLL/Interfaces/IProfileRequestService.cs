using SchoolManagement.BLL.DTOs.Common;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IProfileRequestService
{
    Task<IReadOnlyList<ProfileRequestDto>> GetAllRequestsAsync();
    Task<IReadOnlyList<ProfileRequestDto>> GetRequestsByTeacherIdAsync(int teacherId);
    Task<ResultResponse<ProfileRequestDto>> SubmitRequestAsync(CreateProfileRequestDto dto);
    Task<ResultResponse> ReviewRequestAsync(int id, bool approve, string? adminRemarks, string currentUserName);
}

