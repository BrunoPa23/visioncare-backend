using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Interfaces.Resources;

namespace VisionCareCore.HealthCare.Interfaces.Transform;

public static class CreateMedicineTransform
{
    public static CreateMedicineCommand ToCommand(CreateMedicineResource resource)
    {
        return new CreateMedicineCommand(
            resource.Nombre,
            resource.Description,
            resource.SideEffects,
            resource.Warnings,
            resource.UserId,
            resource.Instruccions,
            resource.ExpirationDate
            );
    }
}