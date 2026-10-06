namespace VisionCareCore.HealthCare.Domain.Model.Commands;

public record CreateMedicineCommand(
    string Nombre,
    string? Description,
    string? SideEffects,
    string? Warnings,
    Guid UserId,
    string? Instruccions,
    string? ExpirationDate
);
