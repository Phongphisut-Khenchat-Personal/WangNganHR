using JanomeHR.API.DTOs.Interview;
using JanomeHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JanomeHR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "HR,Admin,Manager")]
public class InterviewsController(IInterviewService interviewService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? date)
        => Ok(await interviewService.GetAllAsync(date));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await interviewService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInterviewDto dto)
    {
        var result = await interviewService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id}/result")]
    public async Task<IActionResult> UpdateResult(
        Guid id, UpdateInterviewResultDto dto)
    {
        var ok = await interviewService.UpdateResultAsync(id, dto);
        return ok
            ? Ok(new { message = "บันทึกผลสัมภาษณ์แล้ว" })
            : NotFound();
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var ok = await interviewService.CancelAsync(id);
        return ok
            ? Ok(new { message = "ยกเลิกนัดสัมภาษณ์แล้ว" })
            : NotFound();
    }
}