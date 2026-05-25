using PRN232.EduSystem.Repositories.Models;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.Services.Interfaces;

public interface IEnrollmentService
{
    Task<(List<EnrollmentModel> Items, int Total)> GetAllAsync(QueryFilter filter);
    Task<EnrollmentModel?> GetByIdAsync(int id);
    Task<EnrollmentModel> CreateAsync(EnrollmentModel model);
    Task<EnrollmentModel?> UpdateAsync(int id, EnrollmentModel model);
    Task<bool> DeleteAsync(int id);
}
