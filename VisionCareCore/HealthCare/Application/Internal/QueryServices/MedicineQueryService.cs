using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Queries;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.HealthCare.Domain.Services;

namespace VisionCareCore.HealthCare.Application.Internal.QueryServices;

public class MedicineQueryService : IMedicineQueryService
{
    private readonly IMedicineRepository _medicineRepository;

    public MedicineQueryService(IMedicineRepository medicineRepository)
    {
        _medicineRepository = medicineRepository;
    }

    public async Task<IEnumerable<Medicine>> Handle(GetAllMedicinesByUserIdQuery query)
    {
        return await _medicineRepository.GetAllByUserIdAsync(query.UserId);

    }
}