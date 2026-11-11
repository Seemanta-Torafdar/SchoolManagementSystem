using SchoolManagement.BLL.DTOs.Result;
using SchoolManagement.Domain.Common;

namespace SchoolManagement.BLL.Interfaces;

public interface IResultService
{
    Task<ResultResponse<StudentMarksheetDto>> GetVerifiedStudentMarksheetAsync(ResultSearchQueryDto query);
    Task<ResultResponse<StudentMarksheetDto>> GetStudentMarksheetByIdAsync(int studentId, int examinationId);
    Task<IReadOnlyList<StudentMarksheetDto>> GetClassResultsAsync(int examinationId, int classId, int? sectionId);
    Task<ResultResponse> SaveBulkMarksAsync(BulkMarksEntryDto dto, string? currentUserId = null);
    Task<ResultResponse> PublishResultsAsync(int examinationId, bool publish, string? currentUserId = null);
}

