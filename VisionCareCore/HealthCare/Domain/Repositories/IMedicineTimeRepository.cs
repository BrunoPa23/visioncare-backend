using VisionCareCore.HealthCare.Domain.Model.Entities;
using VisionCareCore.Shared.Domain.Repositories;

namespace VisionCareCore.HealthCare.Domain.Repositories;

public interface IMedicineTimeRepository : IBaseRepository<MedicineTime>
{
    Task<IEnumerable<MedicineTime>> GetByMedicineIdAsync(Guid medicineId);
    Task<MedicineTime?> GetByIdAsync(Guid id);
    Task DeleteAsync(MedicineTime entity);

}