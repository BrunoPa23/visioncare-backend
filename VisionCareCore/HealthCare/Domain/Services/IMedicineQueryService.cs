using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Queries;

namespace VisionCareCore.HealthCare.Domain.Services;

public interface IMedicineQueryService
{
    Task<IEnumerable<Medicine>> Handle(GetAllMedicinesByUserIdQuery query);
}