namespace PRN232.EduSystem.API.Models.Responses;

public class CourseResponse
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
    public int EnrollmentCount { get; set; }
    public SemesterResponse? Semester { get; set; }
}
