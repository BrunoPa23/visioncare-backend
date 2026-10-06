using VisionCareCore.HealthCare.Domain.Model.Entities;

namespace VisionCareCore.HealthCare.Domain.Services;

public interface IMedicineTimeQueryService
{
    Task<IEnumerable<MedicineTime>> GetAllByMedicineIdAsync(Guid medicineId);
    Task<MedicineTime?> GetByIdAsync(Guid id);
}