using Microsoft.AspNetCore.Mvc;
using PRN232.EduSystem.API.Helpers;
using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.API.Models.Responses;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;
    private readonly ICourseService   _courseService;

    public SemestersController(ISemesterService service, ICourseService courseService)
    {
        _service       = service;
        _courseService = courseService;
    }

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

        return Ok(PagedResponse<SemesterResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null) return NotFound(ApiResponse<object>.Fail($"Semester with id={id} not found"));
        return Ok(ApiResponse<SemesterResponse>.Ok(MapToResponse(model)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSemesterRequest request)
    {
        var model   = new SemesterModel { SemesterName = request.SemesterName, StartDate = request.StartDate, EndDate = request.EndDate };
        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.SemesterId },
            ApiResponse<SemesterResponse>.Ok(MapToResponse(created), "Semester created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSemesterRequest request)
    {
        var model   = new SemesterModel { SemesterName = request.SemesterName, StartDate = request.StartDate, EndDate = request.EndDate };
        var updated = await _service.UpdateAsync(id, model);
        if (updated is null) return NotFound(ApiResponse<object>.Fail($"Semester with id={id} not found"));
        return Ok(ApiResponse<SemesterResponse>.Ok(MapToResponse(updated), "Semester updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Semester with id={id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Semester deleted successfully"));
    }

    [HttpGet("{id:int}/courses")]
    public async Task<IActionResult> GetCourses(int id, [FromQuery] QueryParameters qp)
    {
        if (!await _service.ExistsAsync(id))
            return NotFound(ApiResponse<object>.Fail($"Semester with id={id} not found"));

        var filter         = QueryParameterParser.ToQueryFilter(qp);
        var (items, total) = await _courseService.GetBySemesterIdAsync(id, filter);
        var responses      = items.Select(MapCourseToResponse).ToList();

        if (!string.IsNullOrWhiteSpace(qp.Fields))
        {
            var filtered = FieldSelector.ApplyToList(responses.Cast<object>(), qp.Fields);
            return Ok(PagedResponse<object?>.Ok(filtered, filter.Page, filter.PageSize, total));
        }

        return Ok(PagedResponse<CourseResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    private static SemesterResponse MapToResponse(SemesterModel m) => new()
    {
        SemesterId   = m.SemesterId,
        SemesterName = m.SemesterName,
        StartDate    = m.StartDate,
        EndDate      = m.EndDate,
        Courses = m.Courses?.Select(c => new CourseResponse
        {
            CourseId   = c.CourseId,
            CourseName = c.CourseName,
            SemesterId = c.SemesterId,
        }).ToList(),
    };

    private static CourseResponse MapCourseToResponse(CourseModel m) => new()
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
