using Microsoft.AspNetCore.Mvc;
using PRN232.EduSystem.API.Helpers;
using PRN232.EduSystem.API.Models.Requests;
using PRN232.EduSystem.API.Models.Responses;
using PRN232.EduSystem.Services.Interfaces;
using PRN232.EduSystem.Services.Models;

namespace PRN232.EduSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;

    public SubjectsController(ISubjectService service) => _service = service;

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

        return Ok(PagedResponse<SubjectResponse>.Ok(responses, filter.Page, filter.PageSize, total));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _service.GetByIdAsync(id);
        if (model is null) return NotFound(ApiResponse<object>.Fail($"Subject with id={id} not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(MapToResponse(model)));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request)
    {
        var model   = new SubjectModel { SubjectCode = request.SubjectCode, SubjectName = request.SubjectName, Credit = request.Credit };
        var created = await _service.CreateAsync(model);
        return CreatedAtAction(nameof(GetById), new { id = created.SubjectId },
            ApiResponse<SubjectResponse>.Ok(MapToResponse(created), "Subject created successfully"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectRequest request)
    {
        var model   = new SubjectModel { SubjectCode = request.SubjectCode, SubjectName = request.SubjectName, Credit = request.Credit };
        var updated = await _service.UpdateAsync(id, model);
        if (updated is null) return NotFound(ApiResponse<object>.Fail($"Subject with id={id} not found"));
        return Ok(ApiResponse<SubjectResponse>.Ok(MapToResponse(updated), "Subject updated successfully"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse<object>.Fail($"Subject with id={id} not found"));
        return Ok(ApiResponse<object>.Ok(null!, "Subject deleted successfully"));
    }

    private static SubjectResponse MapToResponse(SubjectModel m) => new()
    {
        SubjectId   = m.SubjectId,
        SubjectCode = m.SubjectCode,
        SubjectName = m.SubjectName,
        Credit      = m.Credit,
    };
}
