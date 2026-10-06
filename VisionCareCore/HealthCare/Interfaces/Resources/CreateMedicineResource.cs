namespace VisionCareCore.HealthCare.Interfaces.Resources;

public class CreateMedicineResource
{
    public string Nombre { get; set; } = default!;
    public string? Description { get; set; }    
    public string? SideEffects { get; set; }    
    public string? Warnings { get; set; }       
    public Guid UserId { get; set; }
    public string? Instruccions { get; set; }
    public string? ExpirationDate { get; set; }
}