using System;
using System.Collections.Generic;
using VisionCareCore.HealthCare.Interfaces.Resources;

namespace VisionCareCore.HealthCare.Interfaces.Resources
{
    public class UpdateMedicineTimesRequest
    {
        public Guid MedicineId { get; set; }
        public List<CreateMedicineTimeResource> MedicineTimes { get; set; } = new();
    }
}