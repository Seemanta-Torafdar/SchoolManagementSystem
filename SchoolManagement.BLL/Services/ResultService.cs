using SchoolManagement.BLL.DTOs.Result;
using SchoolManagement.BLL.Helpers;
using SchoolManagement.BLL.Interfaces;
using SchoolManagement.DAL.Interfaces;
using SchoolManagement.Domain.Common;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Services;

public class ResultService : IResultService
{
    private readonly IResultRepository _resultRepo;
    private readonly IStudentRepository _studentRepo;
    private readonly IExaminationRepository _examRepo;
    private readonly IAccountService _accountService;

    public ResultService(
        IResultRepository resultRepo, 
        IStudentRepository studentRepo, 
        IExaminationRepository examRepo,
        IAccountService accountService)
    {
        _resultRepo = resultRepo;
        _studentRepo = studentRepo;
        _examRepo = examRepo;
        _accountService = accountService;
    }

    public async Task<ResultResponse<StudentMarksheetDto>> GetVerifiedStudentMarksheetAsync(ResultSearchQueryDto query)
    {
        if (query.ExaminationId <= 0 || query.ClassId <= 0 || query.RollNumber <= 0 || string.IsNullOrWhiteSpace(query.VerificationCode))
        {
            return ResultResponse<StudentMarksheetDto>.Failure("Please provide all required fields: Examination, Class, Roll Number, and Verification PIN.");
        }

        var exam = await _examRepo.GetByIdAsync(query.ExaminationId);
        if (exam == null)
            return ResultResponse<StudentMarksheetDto>.Failure("Selected examination was not found.");

        if (!exam.IsPublished)
            return ResultResponse<StudentMarksheetDto>.Failure("Results for this examination have not been officially published yet.");

        var student = await _studentRepo.VerifyStudentForResultAsync(
            query.ExaminationId, 
            query.AcademicSessionId, 
            query.ClassId, 
            query.RollNumber, 
            query.VerificationCode);

        if (student == null)
        {
            // Protect against enumeration by returning generic failure
            return ResultResponse<StudentMarksheetDto>.Failure("No published result found matching the provided Examination, Class, Roll Number, and Verification PIN.");
        }

        var results = await _resultRepo.GetStudentResultsAsync(student.Id, query.ExaminationId);
        var publishedResults = results.Where(r => r.IsPublished).ToList();

        if (!publishedResults.Any())
        {
            return ResultResponse<StudentMarksheetDto>.Failure("Results for this student have not been published yet.");
        }

        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(publishedResults);

        var marksheet = new StudentMarksheetDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            AdmissionNumber = student.AdmissionNumber,
            RollNumber = student.RollNumber,
            ClassName = student.SchoolClass?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            SessionName = student.AcademicSession?.SessionName ?? "",
            ExaminationName = exam.Name,
            ExaminationDate = exam.StartDate,
            PhotoUrl = student.PhotoUrl,
            TotalMarksObtained = totalMarks,
            TotalMaxMarks = publishedResults.Sum(r => r.MaxMarks),
            GPA = gpa,
            OverallGrade = overallGrade,
            ResultStatus = status,
            IsPublished = true,
            Subjects = publishedResults.Select(r => new SubjectResultDto
            {
                SubjectId = r.SubjectId,
                SubjectName = r.Subject?.Name ?? "",
                SubjectCode = r.Subject?.SubjectCode ?? "",
                MarksObtained = r.MarksObtained,
                MaxMarks = r.MaxMarks,
                Grade = r.Grade,
                GradePoint = r.GradePoint,
                Status = r.Status,
                Remarks = r.Remarks
            }).ToList()
        };

        return ResultResponse<StudentMarksheetDto>.Success(marksheet);
    }

    public async Task<ResultResponse<StudentMarksheetDto>> GetStudentMarksheetByIdAsync(int studentId, int examinationId)
    {
        var student = await _studentRepo.GetStudentWithDetailsAsync(studentId);
        if (student == null)
            return ResultResponse<StudentMarksheetDto>.Failure("Student not found.");

        var exam = await _examRepo.GetByIdAsync(examinationId);
        if (exam == null)
            return ResultResponse<StudentMarksheetDto>.Failure("Examination not found.");

        var results = await _resultRepo.GetStudentResultsAsync(studentId, examinationId);
        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(results);

        var marksheet = new StudentMarksheetDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            AdmissionNumber = student.AdmissionNumber,
            RollNumber = student.RollNumber,
            ClassName = student.SchoolClass?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            SessionName = student.AcademicSession?.SessionName ?? "",
            ExaminationName = exam.Name,
            ExaminationDate = exam.StartDate,
            PhotoUrl = student.PhotoUrl,
            TotalMarksObtained = totalMarks,
            TotalMaxMarks = results.Sum(r => r.MaxMarks),
            GPA = gpa,
            OverallGrade = overallGrade,
            ResultStatus = status,
            IsPublished = exam.IsPublished,
            Subjects = results.Select(r => new SubjectResultDto
            {
                SubjectId = r.SubjectId,
                SubjectName = r.Subject?.Name ?? "",
                SubjectCode = r.Subject?.SubjectCode ?? "",
                MarksObtained = r.MarksObtained,
                MaxMarks = r.MaxMarks,
                Grade = r.Grade,
                GradePoint = r.GradePoint,
                Status = r.Status,
                Remarks = r.Remarks
            }).ToList()
        };

        return ResultResponse<StudentMarksheetDto>.Success(marksheet);
    }

    public async Task<IReadOnlyList<StudentMarksheetDto>> GetClassResultsAsync(int examinationId, int classId, int? sectionId)
    {
        var results = await _resultRepo.GetResultsByExamAndClassAsync(examinationId, classId, sectionId);
        var grouped = results.GroupBy(r => r.StudentId);
        var marksheets = new List<StudentMarksheetDto>();

        foreach (var group in grouped)
        {
            var studentResults = group.ToList();
            var first = studentResults.First();
            var student = first.Student;
            var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(studentResults);

            marksheets.Add(new StudentMarksheetDto
            {
                StudentId = first.StudentId,
                StudentName = student?.FullName ?? "",
                AdmissionNumber = student?.AdmissionNumber ?? "",
                RollNumber = student?.RollNumber ?? 0,
                ClassName = student?.SchoolClass?.Name ?? "",
                SectionName = student?.Section?.Name ?? "",
                TotalMarksObtained = totalMarks,
                GPA = gpa,
                OverallGrade = overallGrade,
                ResultStatus = status,
                Subjects = studentResults.Select(r => new SubjectResultDto
                {
                    SubjectId = r.SubjectId,
                    SubjectName = r.Subject?.Name ?? "",
                    MarksObtained = r.MarksObtained,
                    Grade = r.Grade,
                    GradePoint = r.GradePoint,
                    Status = r.Status
                }).ToList()
            });
        }

        return marksheets.OrderBy(m => m.RollNumber).ToList();
    }

    public async Task<ResultResponse> SaveBulkMarksAsync(BulkMarksEntryDto dto, string? currentUserId = null)
    {
        if (!dto.MarksEntries.Any())
            return ResultResponse.Failure("No mark entries were provided.");

        var entities = new List<StudentResult>();

        foreach (var entry in dto.MarksEntries)
        {
            var (grade, gradePoint) = GradeCalculator.CalculateGradeAndPoint(entry.MarksObtained, dto.MaxMarks);
            var status = grade == "F" ? ResultStatus.Failed : ResultStatus.Passed;

            entities.Add(new StudentResult
            {
                StudentId = entry.StudentId,
                ExaminationId = dto.ExaminationId,
                SubjectId = dto.SubjectId,
                MarksObtained = entry.MarksObtained,
                MaxMarks = dto.MaxMarks,
                Grade = grade,
                GradePoint = gradePoint,
                Status = status,
                Remarks = entry.Remarks?.Trim(),
                IsPublished = false
            });
        }

        await _resultRepo.SaveBulkResultsAsync(entities);
        await _accountService.LogAuditAsync(currentUserId, "AuthorizedUser", "SaveMarks", "Results", $"Exam:{dto.ExaminationId},Subj:{dto.SubjectId}", $"Saved marks for {entities.Count} students");

        return ResultResponse.Success($"Successfully saved marks for {entities.Count} students.");
    }

    public async Task<ResultResponse> PublishResultsAsync(int examinationId, bool publish, string? currentUserId = null)
    {
        await _resultRepo.PublishResultsAsync(examinationId, publish);
        await _accountService.LogAuditAsync(currentUserId, "Admin", publish ? "Publish" : "Unpublish", "Results", examinationId.ToString(), $"{(publish ? "Published" : "Unpublished")} official marks for exam #{examinationId}");
        return ResultResponse.Success($"Results successfully {(publish ? "published" : "unpublished")}.");
    }
}

