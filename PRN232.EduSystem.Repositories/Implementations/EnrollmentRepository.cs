using Microsoft.EntityFrameworkCore;
using PRN232.EduSystem.Repositories.Data;
using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Interfaces;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context) => _context = context;

    public async Task<(List<Enrollment> Items, int Total)> GetAllAsync(QueryFilter filter)
    {
        var query = _context.Enrollments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.Status.ToLower().Contains(s));
        }

        if (filter.Expand.Contains("student", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Student);
        if (filter.Expand.Contains("course", StringComparer.OrdinalIgnoreCase))
            query = query.Include(x => x.Course).ThenInclude(c => c.Semester);

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("enrolldate",   false) => query.OrderBy(x => x.EnrollDate),
            ("enrolldate",   true)  => query.OrderByDescending(x => x.EnrollDate),
            ("status",       false) => query.OrderBy(x => x.Status),
            ("status",       true)  => query.OrderByDescending(x => x.Status),
            ("studentid",    false) => query.OrderBy(x => x.StudentId),
            ("studentid",    true)  => query.OrderByDescending(x => x.StudentId),
            ("courseid",     false) => query.OrderBy(x => x.CourseId),
            ("courseid",     true)  => query.OrderByDescending(x => x.CourseId),
            _                       => query.OrderBy(x => x.EnrollmentId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<Enrollment?> GetByIdAsync(int id, List<string>? expand = null)
    {
        var query = _context.Enrollments.AsQueryable();
        if (expand != null)
        {
            if (expand.Contains("student", StringComparer.OrdinalIgnoreCase))
                query = query.Include(x => x.Student);
            if (expand.Contains("course", StringComparer.OrdinalIgnoreCase))
                query = query.Include(x => x.Course).ThenInclude(c => c.Semester);
        }
        else
        {
            query = query.Include(x => x.Student).Include(x => x.Course).ThenInclude(c => c.Semester);
        }
        return await query.FirstOrDefaultAsync(x => x.EnrollmentId == id);
    }

    public async Task<Enrollment> CreateAsync(Enrollment entity)
    {
        _context.Enrollments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Enrollment?> UpdateAsync(Enrollment entity)
    {
        var existing = await _context.Enrollments.FindAsync(entity.EnrollmentId);
        if (existing is null) return null;
        existing.StudentId  = entity.StudentId;
        existing.CourseId   = entity.CourseId;
        existing.EnrollDate = entity.EnrollDate;
        existing.Status     = entity.Status;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Enrollments.FindAsync(id);
        if (entity is null) return false;
        _context.Enrollments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _context.Enrollments.AnyAsync(x => x.EnrollmentId == id);

    public async Task<(List<Enrollment> Items, int Total)> GetByCourseIdAsync(int courseId, QueryFilter filter)
    {
        var query = _context.Enrollments
            .Where(x => x.CourseId == courseId)
            .Include(x => x.Student)
            .Include(x => x.Course).ThenInclude(c => c.Semester)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.Status.ToLower().Contains(s));
        }

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("enrolldate", false) => query.OrderBy(x => x.EnrollDate),
            ("enrolldate", true)  => query.OrderByDescending(x => x.EnrollDate),
            ("status",     false) => query.OrderBy(x => x.Status),
            ("status",     true)  => query.OrderByDescending(x => x.Status),
            ("studentid",  false) => query.OrderBy(x => x.StudentId),
            ("studentid",  true)  => query.OrderByDescending(x => x.StudentId),
            _                     => query.OrderBy(x => x.EnrollmentId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }

    public async Task<(List<Enrollment> Items, int Total)> GetByStudentIdAsync(int studentId, QueryFilter filter)
    {
        var query = _context.Enrollments
            .Where(x => x.StudentId == studentId)
            .Include(x => x.Student)
            .Include(x => x.Course).ThenInclude(c => c.Semester)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(x => x.Status.ToLower().Contains(s));
        }

        query = (filter.SortBy?.ToLower(), filter.Descending) switch
        {
            ("enrolldate", false) => query.OrderBy(x => x.EnrollDate),
            ("enrolldate", true)  => query.OrderByDescending(x => x.EnrollDate),
            ("status",     false) => query.OrderBy(x => x.Status),
            ("status",     true)  => query.OrderByDescending(x => x.Status),
            ("courseid",   false) => query.OrderBy(x => x.CourseId),
            ("courseid",   true)  => query.OrderByDescending(x => x.CourseId),
            _                     => query.OrderBy(x => x.EnrollmentId),
        };

        var total = await query.CountAsync();
        var items = await query.Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync();
        return (items, total);
    }
}
