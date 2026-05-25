using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _repo;

    public SubjectService(ISubjectRepository repo) => _repo = repo;

    public async Task<(List<SubjectModel> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var (entities, total) = await _repo.GetAllAsync(filter);
        return (entities.Select(Map).ToList(), total);
    }

    public async Task<SubjectModel?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        return entity is null ? null : Map(entity);
    }

    public async Task<SubjectModel> CreateAsync(SubjectModel model)
    {
        var entity = new Subject
        {
            SubjectCode = model.SubjectCode,
            SubjectName = model.SubjectName,
            Credit      = model.Credit,
        };
        var created = await _repo.CreateAsync(entity);
        return Map(created);
    }

    public async Task<SubjectModel?> UpdateAsync(int id, SubjectModel model)
    {
        var entity = new Subject
        {
            SubjectId   = id,
            SubjectCode = model.SubjectCode,
            SubjectName = model.SubjectName,
            Credit      = model.Credit,
        };
        var updated = await _repo.UpdateAsync(entity);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    private static SubjectModel Map(Subject e) => new()
    {
        SubjectId   = e.SubjectId,
        SubjectCode = e.SubjectCode,
        SubjectName = e.SubjectName,
        Credit      = e.Credit,
    };
}
