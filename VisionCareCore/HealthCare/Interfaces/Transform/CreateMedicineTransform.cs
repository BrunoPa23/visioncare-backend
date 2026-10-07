using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Interfaces.Resources;

namespace VisionCareCore.HealthCare.Interfaces.Transform;

public static class CreateMedicineTransform
{
    public static CreateMedicineCommand ToCommand(CreateMedicineResource resource, Guid userId)
    {
        return new CreateMedicineCommand(
            resource.Nombre,
            resource.Description,
            resource.SideEffects,
            resource.Warnings,
            userId,
            resource.Instruccions,
            resource.ExpirationDate
            );
    }
}