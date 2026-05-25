using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Interfaces;

public interface ISubjectService
{
    Task<(List<SubjectModel> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<SubjectModel?> GetByIdAsync(int id);
    Task<SubjectModel> CreateAsync(SubjectModel model);
    Task<SubjectModel?> UpdateAsync(int id, SubjectModel model);
    Task<bool> DeleteAsync(int id);
}
