using JanomeHR.API.DTOs.JobPosting;
using JanomeHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JanomeHR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobPostingsController(IJobPostingService jobService) : ControllerBase
{
    // Public — ผู้สมัครดูได้
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
        => Ok(await jobService.GetActiveAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await jobService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // HR Only
    [Authorize(Roles = "HR,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await jobService.GetAllAsync());

    [Authorize(Roles = "HR,Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateJobPostingDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await jobService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = "HR,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateJobPostingDto dto)
    {
        var result = await jobService.UpdateAsync(id, dto);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "HR,Admin")]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var ok = await jobService.PublishAsync(id);
        return ok ? Ok(new { message = "เผยแพร่ประกาศงานแล้ว" }) : BadRequest();
    }

    [Authorize(Roles = "HR,Admin")]
    [HttpPost("{id}/close")]
    public async Task<IActionResult> Close(Guid id)
    {
        var ok = await jobService.CloseAsync(id);
        return ok ? Ok(new { message = "ปิดรับสมัครแล้ว" }) : BadRequest();
    }

    [Authorize(Roles = "HR,Admin")]
    [HttpPost("{id}/qrcode")]
    public async Task<IActionResult> GenerateQrCode(Guid id)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result  = await jobService.GenerateQrCodeAsync(id, baseUrl);
        return result is null ? NotFound() : Ok(new { qrCode = result });
    }
}