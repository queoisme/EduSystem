namespace PRN232.EduSystem.Services.Models;

public class EnrollmentModel
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime EnrollDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public StudentModel? Student { get; set; }
    public CourseModel? Course { get; set; }
}
