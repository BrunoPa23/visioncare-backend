using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Model.ValueObjects;
using VisionCareCore.HealthCare.Interfaces.Resources;

namespace VisionCareCore.HealthCare.Interfaces.Transform;

public static class CreateMedicineTimeTransform
{
    public static CreateMedicineTimeCommand ToCommand(CreateMedicineTimeResource resource)
    {
        return new CreateMedicineTimeCommand(
            resource.MedicineId,
            resource.Day,
            resource.TypeRemember,
            resource.Foods.HasValue ? (Foods?)resource.Foods : null,
            resource.SpecificTime,
            resource.Interval.HasValue ? (Interval?)resource.Interval : null
        );
    }
}