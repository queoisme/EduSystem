using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Interfaces;

public interface ISemesterService
{
    Task<(List<SemesterModel> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<SemesterModel?> GetByIdAsync(int id);
    Task<SemesterModel> CreateAsync(SemesterModel model);
    Task<SemesterModel?> UpdateAsync(int id, SemesterModel model);
    Task<bool> DeleteAsync(int id);
}
