using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.BLL.Helpers;

public static class ResultCalculator
{
    public static (decimal TotalMarks, decimal GPA, string OverallGrade, ResultStatus Status) CalculateOverallResult(IEnumerable<StudentResult> results)
    {
        var resultList = results.ToList();
        if (!resultList.Any())
        {
            return (0, 0, "N/A", ResultStatus.Incomplete);
        }

        decimal totalMarks = 0;
        decimal totalGradePoints = 0;
        bool hasFailed = false;

        foreach (var r in resultList)
        {
            totalMarks += r.MarksObtained;
            totalGradePoints += r.GradePoint;
            if (r.Grade == "F" || r.Status == ResultStatus.Failed)
            {
                hasFailed = true;
            }
        }

        if (hasFailed)
        {
            return (totalMarks, 0.00m, "F", ResultStatus.Failed);
        }

        decimal gpa = Math.Round(totalGradePoints / resultList.Count, 2);
        string overallGrade = gpa switch
        {
            >= 5.00m => "A+",
            >= 4.00m => "A",
            >= 3.50m => "A-",
            >= 3.00m => "B",
            >= 2.00m => "C",
            >= 1.00m => "D",
            _ => "F"
        };

        return (totalMarks, gpa, overallGrade, ResultStatus.Passed);
    }
}

