namespace VisionCareCore.HealthCare.Domain.Queries;

public class GetMedicineByIdQuery
{
    public Guid MedicineId { get; }

    public GetMedicineByIdQuery(Guid medicineId)
    {
        MedicineId = medicineId;
    }
}
