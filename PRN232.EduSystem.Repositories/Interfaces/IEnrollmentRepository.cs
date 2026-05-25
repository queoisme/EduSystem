using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<(List<Enrollment> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<Enrollment?> GetByIdAsync(int id, List<string>? expand = null);
    Task<Enrollment> CreateAsync(Enrollment entity);
    Task<Enrollment?> UpdateAsync(Enrollment entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
