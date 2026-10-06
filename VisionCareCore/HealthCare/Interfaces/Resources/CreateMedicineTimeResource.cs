using VisionCareCore.HealthCare.Domain.Model.ValueObjects;

namespace VisionCareCore.HealthCare.Interfaces.Resources;

public class CreateMedicineTimeResource
{
    public Guid MedicineId { get; set; }
    public string Day { get; set; } = default!;
    public Type_Remember TypeRemember { get; set; }
    public Foods? Foods { get; set; }
    public TimeOnly? SpecificTime { get; set; }
    public Interval? Interval { get; set; }
}