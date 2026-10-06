using System.Text.Json.Serialization;
using VisionCareCore.HealthCare.Domain.Model.Aggregates;
using VisionCareCore.HealthCare.Domain.Model.ValueObjects;

namespace VisionCareCore.HealthCare.Domain.Model.Entities;

public class MedicineTime
{
    public Guid Id { get; set; }
    public Guid MedicineId { get; set; }

    public Type_Remember TypeRemember { get; set; }

    public Foods? Foods { get; set; }

    public TimeOnly? SpecificTime { get; set; }

    public Interval? Interval { get; set; }

    public string Day { get; set; } = default!;
    
    [JsonIgnore]
    public Medicine Medicine { get; set; } = default!;
}
