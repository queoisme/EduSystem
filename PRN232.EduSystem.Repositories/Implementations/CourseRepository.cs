using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context) => _context = context;

    public async Task<(List<Course> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.CourseName.ToLower().Contains(s));
        }

        if (filter.Expand.Contains("semester", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Semester);
        if (filter.Expand.Contains("enrollments", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Enrollments);

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("coursename",  false) => query.OrderBy(x => x.CourseName),
            ("coursename",  true)  => query.OrderByDescending(x => x.CourseName),
            ("semesterid",  false) => query.OrderBy(x => x.SemesterId),
            ("semesterid",  true)  => query.OrderByDescending(x => x.SemesterId),
            _                      => query.OrderBy(x => x.CourseId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Course?> GetByIdAsync(int id, List<string>? expand = null)
    {
        var query = _context.Courses.AsQueryable();
        query = query.Include(x => x.Semester).Include(x => x.Enrollments);
        return await query.FirstOrDefaultAsync(x => x.CourseId == id);
    }

    public async Task<Course> CreateAsync(Course entity)
    {
        _context.Courses.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Course?> UpdateAsync(Course entity)
    {
        var existing = await _context.Courses.FindAsync(entity.CourseId);
        if (existing is null) return null;
        existing.CourseName  = entity.CourseName;
        existing.SemesterId  = entity.SemesterId;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Courses.FindAsync(id);
        if (entity is null) return false;
        _context.Courses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Courses.AnyAsync(x => x.CourseId == id);

    public async Task<(List<Course> Items, int Total)> GetBySemesterIdAsync(int semesterId, QueryFilter filter)
    {
        var query = _context.Courses.Where(x => x.SemesterId == semesterId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.CourseName.ToLower().Contains(s));
        }

        if (filter.Expand.Contains("semester", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Semester);
        if (filter.Expand.Contains("enrollments", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Enrollments);

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("coursename", false) => query.OrderBy(x => x.CourseName),
            ("coursename", true)  => query.OrderByDescending(x => x.CourseName),
            _                     => query.OrderBy(x => x.CourseId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }
}
