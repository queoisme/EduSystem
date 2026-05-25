using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Implementations;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _repo;

    public SemesterService(ISemesterRepository repo) => _repo = repo;

    public async Task<(List<SemesterModel> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var (entities, total) = await _repo.GetAllAsync(filter);
        return (entities.Select(Map).ToList(), total);
    }

    public async Task<SemesterModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<SemesterModel> CreateAsync(SemesterModel model)
    {
        var entity = new Semester
        {
            SemesterName = model.SemesterName,
            StartDate    = model.StartDate,
            EndDate      = model.EndDate,
        };
        var created = await _repo.CreateAsync(entity);
        return Map(created);
    }

    public async Task<SemesterModel?> UpdateAsync(int id, SemesterModel model)
    {
        var entity = new Semester
        {
            SemesterId   = id,
            SemesterName = model.SemesterName,
            StartDate    = model.StartDate,
            EndDate      = model.EndDate,
        };
        var updated = await _repo.UpdateAsync(entity);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static SemesterModel Map(Semester e) => new()
    {
        SemesterId   = e.SemesterId,
        SemesterName = e.SemesterName,
        StartDate    = e.StartDate,
        EndDate      = e.EndDate,
        Courses = e.Courses?.Select(c => new CourseModel
        {
            CourseId   = c.CourseId,
            CourseName = c.CourseName,
            SemesterId = c.SemesterId,
        }).ToList(),
    };
}
