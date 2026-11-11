using SchoolManagement.BLL.Helpers;
using Xunit;

namespace SchoolManagement.Tests.UnitTests;

public class GradeCalculatorTests
{
    [Theory]
    [InlineData(100, 100, "A+", 5.00)]
    [InlineData(85, 100, "A+", 5.00)]
    [InlineData(80, 100, "A+", 5.00)]
    [InlineData(79.5, 100, "A", 4.00)]
    [InlineData(70, 100, "A", 4.00)]
    [InlineData(65, 100, "A-", 3.50)]
    [InlineData(60, 100, "A-", 3.50)]
    [InlineData(55, 100, "B", 3.00)]
    [InlineData(50, 100, "B", 3.00)]
    [InlineData(45, 100, "C", 2.00)]
    [InlineData(40, 100, "C", 2.00)]
    [InlineData(35, 100, "D", 1.00)]
    [InlineData(33, 100, "D", 1.00)]
    [InlineData(32.5, 100, "F", 0.00)]
    [InlineData(0, 100, "F", 0.00)]
    public void CalculateGradeAndPoint_ShouldReturnExpectedGradeAndPoint_ForStandardFullMarks(
        decimal marks, decimal fullMarks, string expectedGrade, decimal expectedPoint)
    {
        // Act
        var result = GradeCalculator.CalculateGradeAndPoint(marks, fullMarks);

        // Assert
        Assert.Equal(expectedGrade, result.Grade);
        Assert.Equal(expectedPoint, result.GradePoint);
    }

    [Fact]
    public void CalculateGradeAndPoint_ShouldScaleCorrectly_ForNonStandardFullMarks()
    {
        // 50 full marks scale: 40 marks out of 50 is 80% => A+, 5.00
        var result = GradeCalculator.CalculateGradeAndPoint(40, 50);

        Assert.Equal("A+", result.Grade);
        Assert.Equal(5.00m, result.GradePoint);
    }

    [Fact]
    public void CalculateGradeAndPoint_ShouldHandleZeroOrNegativeFullMarks_Gracefully()
    {
        // Guard check: fullMarks <= 0 should default to 100
        var result = GradeCalculator.CalculateGradeAndPoint(75, 0);

        Assert.Equal("A", result.Grade);
        Assert.Equal(4.00m, result.GradePoint);
    }
}

