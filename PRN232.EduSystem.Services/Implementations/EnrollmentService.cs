using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _repo;

    public EnrollmentService(IEnrollmentRepository repo) => _repo = repo;

    public async Task<(List<EnrollmentModel> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var (entities, total) = await _repo.GetAllAsync(filter);
        return (entities.Select(Map).ToList(), total);
    }

    public async Task<EnrollmentModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<EnrollmentModel> CreateAsync(EnrollmentModel model)
    {
        var entity = new Enrollment
        {
            StudentId  = model.StudentId,
            CourseId   = model.CourseId,
            EnrollDate = model.EnrollDate,
            Status     = model.Status,
        };
        var created = await _repo.CreateAsync(entity);
        return Map(created);
    }

    public async Task<EnrollmentModel?> UpdateAsync(int id, EnrollmentModel model)
    {
        var entity = new Enrollment
        {
            EnrollmentId = id,
            StudentId    = model.StudentId,
            CourseId     = model.CourseId,
            EnrollDate   = model.EnrollDate,
            Status       = model.Status,
        };
        var updated = await _repo.UpdateAsync(entity);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static EnrollmentModel Map(Enrollment e) => new()
    {
        EnrollmentId = e.EnrollmentId,
        StudentId    = e.StudentId,
        CourseId     = e.CourseId,
        EnrollDate   = e.EnrollDate,
        Status       = e.Status,
        Student = e.Student is null ? null : new StudentModel
        {
            StudentId   = e.Student.StudentId,
            FullName    = e.Student.FullName,
            Email       = e.Student.Email,
            DateOfBirth = e.Student.DateOfBirth,
        },
        Course = e.Course is null ? null : new CourseModel
        {
            CourseId   = e.Course.CourseId,
            CourseName = e.Course.CourseName,
            SemesterId = e.Course.SemesterId,
            Semester = e.Course.Semester is null ? null : new SemesterModel
            {
                SemesterId   = e.Course.Semester.SemesterId,
                SemesterName = e.Course.Semester.SemesterName,
                StartDate    = e.Course.Semester.StartDate,
                EndDate      = e.Course.Semester.EndDate,
            }
        },
    };
}
