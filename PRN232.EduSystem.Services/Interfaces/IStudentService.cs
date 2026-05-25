using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Interfaces;

public interface IStudentService
{
    Task<(List<StudentModel> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<StudentModel?> GetByIdAsync(int id);
    Task<StudentModel> CreateAsync(StudentModel model);
    Task<StudentModel?> UpdateAsync(int id, StudentModel model);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<(List<StudentModel> Items, int Total)> GetByCourseIdAsync(int courseId, QueryFilter filter);
}
