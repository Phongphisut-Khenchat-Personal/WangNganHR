using JanomeHR.API.DTOs.Interview;

namespace JanomeHR.API.Services.Interfaces;

public interface IInterviewService
{
    Task<List<InterviewResponseDto>> GetAllAsync(DateTime? date);
    Task<InterviewResponseDto?> GetByIdAsync(Guid id);
    Task<InterviewResponseDto> CreateAsync(CreateInterviewDto dto);
    Task<bool> UpdateResultAsync(Guid id, UpdateInterviewResultDto dto);
    Task<bool> CancelAsync(Guid id);
}