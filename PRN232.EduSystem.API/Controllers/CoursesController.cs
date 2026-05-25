using Microsoft.AspNetCore.Mvc;
using PRN232.EduSystem.API.Helpers;
using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.API.Models.Responses;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service) => _service = service;

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

        return Ok(PagedResponse<CourseResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null) return NotFound(ApiResponse<object>.Fail($"Course with id={id} not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(MapToResponse(model)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var model   = new CourseModel { CourseName = request.CourseName, SemesterId = request.SemesterId };
        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.CourseId },
            ApiResponse<CourseResponse>.Ok(MapToResponse(created), "Course created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest request)
    {
        var model   = new CourseModel { CourseName = request.CourseName, SemesterId = request.SemesterId };
        var updated = await _service.UpdateAsync(id, model);
        if (updated is null) return NotFound(ApiResponse<object>.Fail($"Course with id={id} not found"));
        return Ok(ApiResponse<CourseResponse>.Ok(MapToResponse(updated), "Course updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Course with id={id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Course deleted successfully"));
    }

    private static CourseResponse MapToResponse(CourseModel m) => new()
    {
        CourseId        = m.CourseId,
        CourseName      = m.CourseName,
        SemesterId      = m.SemesterId,
        EnrollmentCount = m.EnrollmentCount,
        Semester = m.Semester is null ? null : new SemesterResponse
        {
            SemesterId   = m.Semester.SemesterId,
            SemesterName = m.Semester.SemesterName,
            StartDate    = m.Semester.StartDate,
            EndDate      = m.Semester.EndDate,
        },
    };
}
