using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Implementations;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context) => _context = context;

    public async Task<(List<Subject> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var query = _context.Subjects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.SubjectName.ToLower().Contains(s) || x.SubjectCode.ToLower().Contains(s));
        }

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("subjectcode", false) => query.OrderBy(x => x.SubjectCode),
            ("subjectcode", true)  => query.OrderByDescending(x => x.SubjectCode),
            ("subjectname", false) => query.OrderBy(x => x.SubjectName),
            ("subjectname", true)  => query.OrderByDescending(x => x.SubjectName),
            ("credit",      false) => query.OrderBy(x => x.Credit),
            ("credit",      true)  => query.OrderByDescending(x => x.Credit),
            _                      => query.OrderBy(x => x.SubjectId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Subject?> GetByIdAsync(int id, List<string>? expand = null) =>
        await _context.Subjects.FindAsync(id);

    public async Task<Subject> CreateAsync(Subject entity)
    {
        _context.Subjects.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Subject?> UpdateAsync(Subject entity)
    {
        var existing = await _context.Subjects.FindAsync(entity.SubjectId);
        if (existing is null) return null;
        existing.SubjectCode = entity.SubjectCode;
        existing.SubjectName = entity.SubjectName;
        existing.Credit      = entity.Credit;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Subjects.FindAsync(id);
        if (entity is null) return false;
        _context.Subjects.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Subjects.AnyAsync(x => x.SubjectId == id);
}
