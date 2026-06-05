using JanomeHR.API.DTOs.Application;

namespace JanomeHR.API.Services.Interfaces;

public interface IApplicationService
{
    Task<List<ApplicationResponseDto>> GetAllAsync(string? status, Guid? jobPostingId);
    Task<ApplicationDetailDto?> GetByIdAsync(Guid id);
    Task<ApplicationResponseDto> CreateAsync(CreateApplicationDto dto);
    Task<DocumentDto?> UploadDocumentAsync(Guid applicationId, string documentType, IFormFile file);
    Task<bool> UpdateStatusAsync(Guid id, UpdateApplicationStatusDto dto, Guid userId);
    Task<TrackApplicationResponseDto?> TrackAsync(string referenceCode);
    Task<List<TrackApplicationResponseDto>> TrackByIdentityAsync(string name, string phone);
}