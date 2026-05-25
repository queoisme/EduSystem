using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Interfaces;

public interface ICourseService
{
    Task<(List<CourseModel> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<CourseModel?> GetByIdAsync(int id);
    Task<CourseModel> CreateAsync(CourseModel model);
    Task<CourseModel?> UpdateAsync(int id, CourseModel model);
    Task<bool> DeleteAsync(int id);
}
