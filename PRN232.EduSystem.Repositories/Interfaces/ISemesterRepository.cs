using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Interfaces;

public interface ISemesterRepository
{
    Task<(List<Semester> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<Semester?> GetByIdAsync(int id, List<string>? expand = null);
    Task<Semester> CreateAsync(Semester entity);
    Task<Semester?> UpdateAsync(Semester entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
