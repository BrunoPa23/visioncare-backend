using VisionCareCore.HealthCare.Domain.Model.Commands;

namespace VisionCareCore.HealthCare.Domain.Services;

public interface IMedicineCommandService
{
    Task<Guid> Handle(CreateMedicineCommand command);
    Task Handle(DeleteMedicineCommand command);

}