using Microsoft.AspNetCore.Mvc;
using PRN232.EduSystem.API.Helpers;
using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.API.Models.Responses;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService    _service;
    private readonly IEnrollmentService _enrollmentService;

    public StudentsController(IStudentService service, IEnrollmentService enrollmentService)
    {
        _service           = service;
        _enrollmentService = enrollmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters qp)
    {
        var filter        = QueryParameterParser.ToQueryFilter(qp);
        var (items, total) = await _service.GetAllAsync(filter);
        var responses     = items.Select(MapToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(qp.Fields))
        {
            var filtered = FieldSelector.ApplyToList(responses.Cast<object>(), qp.Fields);
            var paged1   = PagedResponse<object?>.Ok(filtered, filter.Page, filter.PageSize, total);
            return Ok(paged1);
        }

        var paged = PagedResponse<StudentResponse>.Ok(responses, filter.Page, filter.PageSize, total);
        return Ok(paged);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null) return NotFound(ApiResponse<object>.Fail($"Student with id={id} not found"));
        return Ok(ApiResponse<StudentResponse>.Ok(MapToResponse(model)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        var model   = new StudentModel { FullName = request.FullName, Email = request.Email, DateOfBirth = request.DateOfBirth };
        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.StudentId },
            ApiResponse<StudentResponse>.Ok(MapToResponse(created), "Student created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentRequest request)
    {
        var model   = new StudentModel { FullName = request.FullName, Email = request.Email, DateOfBirth = request.DateOfBirth };
        var updated = await _service.UpdateAsync(id, model);
        if (updated is null) return NotFound(ApiResponse<object>.Fail($"Student with id={id} not found"));
        return Ok(ApiResponse<StudentResponse>.Ok(MapToResponse(updated), "Student updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Student with id={id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Student deleted successfully"));
    }

    [HttpGet("{id:int}/enrollments")]
    public async Task<IActionResult> GetEnrollments(int id, [FromQuery] QueryParameters qp)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound(ApiResponse<object>.Fail($"Student with id={id} not found"));

        var filter         = QueryParameterParser.ToQueryFilter(qp);
        var (items, total) = await _enrollmentService.GetByStudentIdAsync(id, filter);
        var responses      = items.Select(MapEnrollmentToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(qp.Fields))
        {
            var filtered = FieldSelector.ApplyToList(responses.Cast<object>(), qp.Fields);
            return Ok(PagedResponse<object?>.Ok(filtered, filter.Page, filter.PageSize, total));
        }

        return Ok(PagedResponse<EnrollmentResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    private static StudentResponse MapToResponse(StudentModel m) => new()
    {
        StudentId   = m.StudentId,
        FullName    = m.FullName,
        Email       = m.Email,
        DateOfBirth = m.DateOfBirth,
        Enrollments = m.Enrollments?.Select(e => new EnrollmentResponse
        {
            EnrollmentId = e.EnrollmentId,
            StudentId    = e.StudentId,
            CourseId     = e.CourseId,
            EnrollDate   = e.EnrollDate,
            Status       = e.Status,
            Course = e.Course is null ? null : new CourseResponse
            {
                CourseId   = e.Course.CourseId,
                CourseName = e.Course.CourseName,
                SemesterId = e.Course.SemesterId,
            }
        }).ToList(),
    };

    private static EnrollmentResponse MapEnrollmentToResponse(EnrollmentModel m) => new()
    {
        EnrollmentId = m.EnrollmentId,
        StudentId    = m.StudentId,
        CourseId     = m.CourseId,
        EnrollDate   = m.EnrollDate,
        Status       = m.Status,
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
