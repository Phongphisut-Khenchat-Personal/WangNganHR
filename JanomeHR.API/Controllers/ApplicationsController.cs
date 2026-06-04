using JanomeHR.API.DTOs.Application;
using JanomeHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JanomeHR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController(IApplicationService appService) : ControllerBase
{
    // Public — ผู้สมัครส่งใบสมัคร
    [HttpPost]
    public async Task<IActionResult> Create(CreateApplicationDto dto)
    {
        var result = await appService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Public — ผู้สมัครติดตามสถานะ
    [HttpGet("track/{referenceCode}")]
    public async Task<IActionResult> Track(string referenceCode)
    {
        var result = await appService.TrackAsync(referenceCode);
        return result is null
            ? NotFound(new { message = "ไม่พบรหัสอ้างอิงนี้" })
            : Ok(result);
    }

    // HR Only
    [Authorize(Roles = "HR,Admin,Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] Guid? jobPostingId)
        => Ok(await appService.GetAllAsync(status, jobPostingId));

    [Authorize(Roles = "HR,Admin,Manager")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await appService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Roles = "HR,Admin,Manager")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id, UpdateApplicationStatusDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ok     = await appService.UpdateStatusAsync(id, dto, userId);
        return ok
            ? Ok(new { message = "อัปเดตสถานะเรียบร้อย" })
            : NotFound();
    }
}