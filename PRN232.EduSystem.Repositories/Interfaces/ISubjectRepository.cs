using PRN232.EduSystem.Repositories.Entities;
using PRN232.EduSystem.Repositories.Models;

namespace PRN232.EduSystem.Repositories.Interfaces;

public interface ISubjectRepository
{
    Task<(List<Subject> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<Subject?> GetByIdAsync(int id, List<string>? expand = null);
    Task<Subject> CreateAsync(Subject entity);
    Task<Subject?> UpdateAsync(Subject entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
