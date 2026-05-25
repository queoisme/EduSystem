using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<(List<Course> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<Course?> GetByIdAsync(int id, List<string>? expand = null);
    Task<Course> CreateAsync(Course entity);
    Task<Course?> UpdateAsync(Course entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<(List<Course> Items, int Total)> GetBySemesterIdAsync(int semesterId, QueryFilter filter);
}
