namespace PRN232.EduSystem.API.Models.Requests;

public class UpdateCourseRequest
{
    public string CourseName { get; set; } = string.Empty;
    public int SemesterId { get; set; }
}
