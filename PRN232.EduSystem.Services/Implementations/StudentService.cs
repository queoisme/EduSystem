using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;

    public StudentService(IStudentRepository repo) => _repo = repo;

    public async Task<(List<StudentModel> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var (entities, total) = await _repo.GetAllAsync(filter);
        return (entities.Select(Map).ToList(), total);
    }

    public async Task<StudentModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : MapFull(entity);
    }

    public async Task<StudentModel> CreateAsync(StudentModel model)
    {
        var entity = new Student
        {
            FullName    = model.FullName,
            Email       = model.Email,
            DateOfBirth = model.DateOfBirth,
        };
        var created = await _repo.CreateAsync(entity);
        return Map(created);
    }

    public async Task<StudentModel?> UpdateAsync(int id, StudentModel model)
    {
        var entity = new Student
        {
            StudentId   = id,
            FullName    = model.FullName,
            Email       = model.Email,
            DateOfBirth = model.DateOfBirth,
        };
        var updated = await _repo.UpdateAsync(entity);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<bool> ExistsAsync(int id) => await _repo.ExistsAsync(id);

    public async Task<(List<StudentModel> Items, int Total)> GetByCourseIdAsync(int courseId, QueryFilter filter)
    {
        var (entities, total) = await _repo.GetByCourseIdAsync(courseId, filter);
        return (entities.Select(Map).ToList(), total);
    }

    private static StudentModel Map(Student e) => new()
    {
        StudentId   = e.StudentId,
        FullName    = e.FullName,
        Email       = e.Email,
        DateOfBirth = e.DateOfBirth,
    };

    private static StudentModel MapFull(Student e) => new()
    {
        StudentId   = e.StudentId,
        FullName    = e.FullName,
        Email       = e.Email,
        DateOfBirth = e.DateOfBirth,
        Enrollments = e.Enrollments?.Select(en => new EnrollmentModel
        {
            EnrollmentId = en.EnrollmentId,
            StudentId    = en.StudentId,
            CourseId     = en.CourseId,
            EnrollDate   = en.EnrollDate,
            Status       = en.Status,
            Course = en.Course is null ? null : new CourseModel
            {
                CourseId   = en.Course.CourseId,
                CourseName = en.Course.CourseName,
                SemesterId = en.Course.SemesterId,
            }
        }).ToList(),
    };
}
