using WangNganHR.API.DTOs.Application;
using WangNganHR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WangNganHR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController(IApplicationService appService) : ControllerBase
{
    // Public — ผู้สมัครส่งใบสมัคร
    [HttpPost]
    public async Task<IActionResult> Create(CreateApplicationDto dto)
    {
        if (!dto.PdpaConsented)
            return BadRequest(new { message = "PDPA consent is required." });

        var result = await appService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    // Public — อัปโหลดเอกสารหลังสร้างใบสมัคร
    [HttpPost("{id:guid}/documents")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> UploadDocument(
        Guid id, [FromForm] string documentType, IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "ไม่พบไฟล์" });

        var result = await appService.UploadDocumentAsync(id, documentType, file);
        return result is null
            ? BadRequest(new { message = "อัปโหลดไม่สำเร็จ — ตรวจสอบประเภทไฟล์หรือขนาด (สูงสุด 5MB)" })
            : Ok(result);
    }

    // Public — ผู้สมัครติดตามสถานะด้วยชื่อ + เบอร์โทร (หลายตำแหน่งได้)
    [HttpGet("track")]
    public async Task<IActionResult> TrackByIdentity(
        [FromQuery] string name,
        [FromQuery] string phone)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
            return BadRequest(new { message = "กรุณากรอกชื่อและเบอร์โทร" });

        var results = await appService.TrackByIdentityAsync(name.Trim(), phone.Trim());
        return results.Count == 0
            ? NotFound(new { message = "ไม่พบใบสมัครที่ตรงกับข้อมูลนี้" })
            : Ok(results);
    }

    // Public — ผู้สมัครติดตามสถานะด้วยรหัสอ้างอิง
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