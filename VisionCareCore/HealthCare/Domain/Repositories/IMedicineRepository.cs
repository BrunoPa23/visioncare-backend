using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.HealthCare.Domain.Repositories;

public interface IMedicineRepository : IBaseRepository<Medicine>
{
    Task<IEnumerable<Medicine>> GetAllByUserIdAsync(Guid userId);
    // A diferencia de FindByIdAsync, ignora los medicamentos marcados como borrados
    Task<Medicine?> GetByIdAsync(Guid id);


}