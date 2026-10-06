using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.HealthCare.Domain.Repositories;

public interface IMedicineRepository : IBaseRepository<Medicine>
{
    Task<IEnumerable<Medicine>> GetAllByUserIdAsync(Guid userId);
    Task<Medicine?> GetByIdAsync(Guid id);
    void Delete(Medicine medicine);


}