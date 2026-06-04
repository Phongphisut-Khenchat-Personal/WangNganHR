using JanomeHR.API.DTOs.JobPosting;

namespace JanomeHR.API.Services.Interfaces;

public interface IJobPostingService
{
    Task<List<JobPostingResponseDto>> GetAllAsync();
    Task<List<JobPostingResponseDto>> GetActiveAsync();
    Task<JobPostingResponseDto?> GetByIdAsync(Guid id);
    Task<JobPostingResponseDto> CreateAsync(CreateJobPostingDto dto, Guid userId);
    Task<JobPostingResponseDto?> UpdateAsync(Guid id, UpdateJobPostingDto dto);
    Task<bool> PublishAsync(Guid id);
    Task<bool> CloseAsync(Guid id);
    Task<string?> GenerateQrCodeAsync(Guid id, string baseUrl);
}