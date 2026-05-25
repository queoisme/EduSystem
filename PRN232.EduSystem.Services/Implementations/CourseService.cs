using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo) => _repo = repo;

    public async Task<(List<CourseModel> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var (entities, total) = await _repo.GetAllAsync(filter);
        return (entities.Select(Map).ToList(), total);
    }

    public async Task<CourseModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<CourseModel> CreateAsync(CourseModel model)
    {
        var entity = new Course
        {
            CourseName = model.CourseName,
            SemesterId = model.SemesterId,
        };
        var created = await _repo.CreateAsync(entity);
        return Map(created);
    }

    public async Task<CourseModel?> UpdateAsync(int id, CourseModel model)
    {
        var entity = new Course
        {
            CourseId   = id,
            CourseName = model.CourseName,
            SemesterId = model.SemesterId,
        };
        var updated = await _repo.UpdateAsync(entity);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static CourseModel Map(Course e) => new()
    {
        CourseId        = e.CourseId,
        CourseName      = e.CourseName,
        SemesterId      = e.SemesterId,
        EnrollmentCount = e.Enrollments?.Count ?? 0,
        Semester = e.Semester is null ? null : new SemesterModel
        {
            SemesterId   = e.Semester.SemesterId,
            SemesterName = e.Semester.SemesterName,
            StartDate    = e.Semester.StartDate,
            EndDate      = e.Semester.EndDate,
        },
    };
}
