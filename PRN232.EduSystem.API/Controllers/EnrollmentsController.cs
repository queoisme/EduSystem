using Microsoft.AspNetCore.Mvc;
using PRN232.EduSystem.API.Helpers;
using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.API.Models.Responses;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;

    public EnrollmentsController(IEnrollmentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters qp)
    {
        var filter         = QueryParameterParser.ToQueryFilter(qp);
        var (items, total) = await _service.GetAllAsync(filter);
        var responses      = items.Select(MapToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(qp.Fields))
        {
            var filtered = FieldSelector.ApplyToList(responses.Cast<object>(), qp.Fields);
            return Ok(PagedResponse<object?>.Ok(filtered, filter.Page, filter.PageSize, total));
        }

        return Ok(PagedResponse<EnrollmentResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null) return NotFound(ApiResponse<object>.Fail($"Enrollment with id={id} not found"));
        return Ok(ApiResponse<EnrollmentResponse>.Ok(MapToResponse(model)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEnrollmentRequest request)
    {
        var model = new EnrollmentModel
        {
            StudentId  = request.StudentId,
            CourseId   = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status     = request.Status,
        };
        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.EnrollmentId },
            ApiResponse<EnrollmentResponse>.Ok(MapToResponse(created), "Enrollment created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEnrollmentRequest request)
    {
        var model = new EnrollmentModel
        {
            StudentId  = request.StudentId,
            CourseId   = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status     = request.Status,
        };
        var updated = await _service.UpdateAsync(id, model);
        if (updated is null) return NotFound(ApiResponse<object>.Fail($"Enrollment with id={id} not found"));
        return Ok(ApiResponse<EnrollmentResponse>.Ok(MapToResponse(updated), "Enrollment updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Enrollment with id={id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Enrollment deleted successfully"));
    }


    private static EnrollmentResponse MapToResponse(EnrollmentModel m) => new()
    {
        EnrollmentId = m.EnrollmentId,
        StudentId    = m.StudentId,
        CourseId     = m.CourseId,
        EnrollDate   = m.EnrollDate,
        Status       = m.Status,
        Student = m.Student is null ? null : new StudentResponse
        {
            StudentId   = m.Student.StudentId,
            FullName    = m.Student.FullName,
            Email       = m.Student.Email,
            DateOfBirth = m.Student.DateOfBirth,
        },
        Course = m.Course is null ? null : new CourseResponse
        {
            CourseId   = m.Course.CourseId,
            CourseName = m.Course.CourseName,
            SemesterId = m.Course.SemesterId,
            Semester = m.Course.Semester is null ? null : new SemesterResponse
            {
                SemesterId   = m.Course.Semester.SemesterId,
                SemesterName = m.Course.Semester.SemesterName,
                StartDate    = m.Course.Semester.StartDate,
                EndDate      = m.Course.Semester.EndDate,
            }
        },
    };
}
