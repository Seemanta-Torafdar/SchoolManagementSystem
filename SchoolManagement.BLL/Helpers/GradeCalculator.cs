namespace SchoolManagement.BLL.Helpers;

public static class GradeCalculator
{
    public static (string Grade, decimal GradePoint) CalculateGradeAndPoint(decimal marks, decimal fullMarks = 100)
    {
        if (fullMarks <= 0) fullMarks = 100;
        var percentage = (marks / fullMarks) * 100;

        return percentage switch
        {
            >= 80 => ("A+", 5.00m),
            >= 70 => ("A", 4.00m),
            >= 60 => ("A-", 3.50m),
            >= 50 => ("B", 3.00m),
            >= 40 => ("C", 2.00m),
            >= 33 => ("D", 1.00m),
            _ => ("F", 0.00m)
        };
    }
}

