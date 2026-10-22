using Microsoft.EntityFrameworkCore;
using VisionCareCore.HealthCare.Domain.Model.Entities;
using VisionCareCore.HealthCare.Domain.Repositories;
using VisionCareCore.Shared.Infrastructure.Persistence.EFC.Configuration;
using VisionCareCore.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace VisionCareCore.HealthCare.Infrastructure.Persistence.EFC.Repositories;

public class MedicineTimeRepository : BaseRepository<MedicineTime>, IMedicineTimeRepository
{
    public MedicineTimeRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<MedicineTime>> GetByMedicineIdAsync(Guid medicineId)
    {
        return await Context.Set<MedicineTime>()
            .Where(mt => mt.MedicineId == medicineId)
            .ToListAsync();
    }
}

