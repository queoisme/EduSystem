using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Interfaces;

public interface IStudentRepository
{
    Task<(List<Student> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<Student?> GetByIdAsync(int id, List<string>? expand = null);
    Task<Student> CreateAsync(Student entity);
    Task<Student?> UpdateAsync(Student entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<(List<Student> Items, int Total)> GetByCourseIdAsync(int courseId, QueryFilter filter);
}
