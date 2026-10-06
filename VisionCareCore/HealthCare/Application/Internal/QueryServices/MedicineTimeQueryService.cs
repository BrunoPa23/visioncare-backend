using VisionCareCore.HealthCare.Domain.Model.Entities;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.HealthCare.Domain.Services;

namespace VisionCareCore.HealthCare.Application.Internal.QueryServices;

public class MedicineTimeQueryService : IMedicineTimeQueryService
{
    private readonly IMedicineTimeRepository _repository;

    public MedicineTimeQueryService(IMedicineTimeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MedicineTime>> GetAllByMedicineIdAsync(Guid medicineId)
    {
        return await _repository.GetByMedicineIdAsync(medicineId);
    }

    public async Task<MedicineTime?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }
}