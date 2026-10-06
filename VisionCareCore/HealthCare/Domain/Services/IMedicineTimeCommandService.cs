using VisionCareCore.HealthCare.Domain.Model.Commands;

namespace VisionCareCore.HealthCare.Domain.Services;

public interface IMedicineTimeCommandService
{
    Task<Guid> Handle(CreateMedicineTimeCommand command);
    Task SoftDelete(Guid id);
}