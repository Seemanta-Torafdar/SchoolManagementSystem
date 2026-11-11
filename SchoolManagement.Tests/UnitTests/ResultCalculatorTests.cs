using SchoolManagement.BLL.Helpers;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using Xunit;

namespace SchoolManagement.Tests.UnitTests;

public class ResultCalculatorTests
{
    [Fact]
    public void CalculateOverallResult_ShouldReturnIncomplete_WhenNoSubjectsExist()
    {
        var results = new List<StudentResult>();

        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(results);

        Assert.Equal(0, totalMarks);
        Assert.Equal(0, gpa);
        Assert.Equal("N/A", overallGrade);
        Assert.Equal(ResultStatus.Incomplete, status);
    }

    [Fact]
    public void CalculateOverallResult_ShouldCalculateCorrectGpaAndPassedStatus_WhenAllSubjectsPassed()
    {
        var results = new List<StudentResult>
        {
            new StudentResult { SubjectId = 1, MarksObtained = 85, Grade = "A+", GradePoint = 5.00m, Status = ResultStatus.Passed },
            new StudentResult { SubjectId = 2, MarksObtained = 75, Grade = "A", GradePoint = 4.00m, Status = ResultStatus.Passed },
            new StudentResult { SubjectId = 3, MarksObtained = 65, Grade = "A-", GradePoint = 3.50m, Status = ResultStatus.Passed }
        };

        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(results);

        // Expected: Total = 225, GPA = (5 + 4 + 3.5)/3 = 4.17, Grade = A, Status = Passed
        Assert.Equal(225, totalMarks);
        Assert.Equal(4.17m, gpa);
        Assert.Equal("A", overallGrade);
        Assert.Equal(ResultStatus.Passed, status);
    }

    [Fact]
    public void CalculateOverallResult_ShouldReturnZeroGpaAndFailed_IfAnySubjectIsFailed()
    {
        var results = new List<StudentResult>
        {
            new StudentResult { SubjectId = 1, MarksObtained = 95, Grade = "A+", GradePoint = 5.00m, Status = ResultStatus.Passed },
            new StudentResult { SubjectId = 2, MarksObtained = 90, Grade = "A+", GradePoint = 5.00m, Status = ResultStatus.Passed },
            new StudentResult { SubjectId = 3, MarksObtained = 25, Grade = "F", GradePoint = 0.00m, Status = ResultStatus.Failed } // Failed in one paper
        };

        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(results);

        Assert.Equal(210, totalMarks);
        Assert.Equal(0.00m, gpa);
        Assert.Equal("F", overallGrade);
        Assert.Equal(ResultStatus.Failed, status);
    }

    [Fact]
    public void CalculateOverallResult_ShouldReturnAPlus_WhenGpaIsFivePointZero()
    {
        var results = new List<StudentResult>
        {
            new StudentResult { SubjectId = 1, MarksObtained = 88, Grade = "A+", GradePoint = 5.00m, Status = ResultStatus.Passed },
            new StudentResult { SubjectId = 2, MarksObtained = 92, Grade = "A+", GradePoint = 5.00m, Status = ResultStatus.Passed }
        };

        var (totalMarks, gpa, overallGrade, status) = ResultCalculator.CalculateOverallResult(results);

        Assert.Equal(180, totalMarks);
        Assert.Equal(5.00m, gpa);
        Assert.Equal("A+", overallGrade);
        Assert.Equal(ResultStatus.Passed, status);
    }
}

