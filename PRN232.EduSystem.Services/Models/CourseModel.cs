namespace PRN232.EduSystem.Services.Models;

public class CourseModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public SemesterModel? Semester { get; set; }
    public int EnrollmentCount { get; set; }
}
