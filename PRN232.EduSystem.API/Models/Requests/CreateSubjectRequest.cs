namespace PRN232.EduSystem.API.Models.Requests;

public class CreateSubjectRequest
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int Credit { get; set; }
}
