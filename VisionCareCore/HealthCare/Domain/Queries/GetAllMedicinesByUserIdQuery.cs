namespace VisionCareCore.HealthCare.Domain.Queries;

public class GetAllMedicinesByUserIdQuery
{
    public Guid UserId { get; }

    public GetAllMedicinesByUserIdQuery(Guid userId)
    {
        UserId = userId;
    }
}