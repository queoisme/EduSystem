using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Implementations;

public class SemesterRepository : ISemesterRepository
{
    private readonly AppDbContext _context;

    public SemesterRepository(AppDbContext context) => _context = context;

    public async Task<(List<Semester> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var query = _context.Semesters.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.SemesterName.ToLower().Contains(s));
        }

        if (filter.Expand.Contains("courses", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Courses);

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("semestername", false) => query.OrderBy(x => x.SemesterName),
            ("semestername", true)  => query.OrderByDescending(x => x.SemesterName),
            ("startdate",    false) => query.OrderBy(x => x.StartDate),
            ("startdate",    true)  => query.OrderByDescending(x => x.StartDate),
            ("enddate",      false) => query.OrderBy(x => x.EndDate),
            ("enddate",      true)  => query.OrderByDescending(x => x.EndDate),
            _                       => query.OrderBy(x => x.SemesterId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Semester?> GetByIdAsync(int id, List<string>? expand = null)
    {
        var query = _context.Semesters.Include(x => x.Courses).AsQueryable();
        return await query.FirstOrDefaultAsync(x => x.SemesterId == id);
    }

    public async Task<Semester> CreateAsync(Semester entity)
    {
        _context.Semesters.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Semester?> UpdateAsync(Semester entity)
    {
        var existing = await _context.Semesters.FindAsync(entity.SemesterId);
        if (existing is null) return null;
        existing.SemesterName = entity.SemesterName;
        existing.StartDate    = entity.StartDate;
        existing.EndDate      = entity.EndDate;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Semesters.FindAsync(id);
        if (entity is null) return false;
        _context.Semesters.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Semesters.AnyAsync(x => x.SemesterId == id);
}
