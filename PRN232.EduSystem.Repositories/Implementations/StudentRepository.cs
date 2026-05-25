using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Implementations;

public class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _context;

    public StudentRepository(AppDbContext context) => _context = context;

    public async Task<(List<Student> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(s) || x.Email.ToLower().Contains(s));
        }

        if (filter.Expand.Contains("enrollments", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Enrollments);

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("fullname",    false) => query.OrderBy(x => x.FullName),
            ("fullname",    true)  => query.OrderByDescending(x => x.FullName),
            ("email",       false) => query.OrderBy(x => x.Email),
            ("email",       true)  => query.OrderByDescending(x => x.Email),
            ("dateofbirth", false) => query.OrderBy(x => x.DateOfBirth),
            ("dateofbirth", true)  => query.OrderByDescending(x => x.DateOfBirth),
            _                      => query.OrderBy(x => x.StudentId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Student?> GetByIdAsync(int id, List<string>? expand = null)
    {
        var query = _context.Students.AsQueryable();
        if (expand != null && expand.Contains("enrollments", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Semester);
        return await query.FirstOrDefaultAsync(x => x.StudentId == id);
    }

    public async Task<Student> CreateAsync(Student entity)
    {
        _context.Students.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Student?> UpdateAsync(Student entity)
    {
        var existing = await _context.Students.FindAsync(entity.StudentId);
        if (existing is null) return null;
        existing.FullName    = entity.FullName;
        existing.Email       = entity.Email;
        existing.DateOfBirth = entity.DateOfBirth;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Students.FindAsync(id);
        if (entity is null) return false;
        _context.Students.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Students.AnyAsync(x => x.StudentId == id);

    public async Task<(List<Student> Items, int Total)> GetByCourseIdAsync(int courseId, QueryFilter filter)
    {
        var query = _context.Students
            .Where(s => s.Enrollments.Any(e => e.CourseId == courseId));

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(s) || x.Email.ToLower().Contains(s));
        }

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("fullname",    false) => query.OrderBy(x => x.FullName),
            ("fullname",    true)  => query.OrderByDescending(x => x.FullName),
            ("email",       false) => query.OrderBy(x => x.Email),
            ("email",       true)  => query.OrderByDescending(x => x.Email),
            ("dateofbirth", false) => query.OrderBy(x => x.DateOfBirth),
            ("dateofbirth", true)  => query.OrderByDescending(x => x.DateOfBirth),
            _                      => query.OrderBy(x => x.StudentId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }
}
