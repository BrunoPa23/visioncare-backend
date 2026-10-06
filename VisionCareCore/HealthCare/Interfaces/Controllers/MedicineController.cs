using Microsoft.AspNetCore.Mvc;
using VisionCareCore.HealthCare.Application.Internal.CommandServices;
using VisionCareCore.HealthCare.Application.Internal.QueryServices;
using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Queries;
using VisionCareCore.HealthCare.Domain.Services;
using VisionCareCore.HealthCare.Interfaces.Resources;
using VisionCareCore.HealthCare.Interfaces.Transform;

namespace VisionCareCore.HealthCare.Interfaces.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineCommandService _medicineCommandService;
        private readonly IMedicineQueryService _medicineQueryService;

        public MedicineController(
            IMedicineCommandService medicineCommandService,
            IMedicineQueryService medicineQueryService)
        {
            _medicineCommandService = medicineCommandService;
            _medicineQueryService = medicineQueryService;
        }

        // POST: api/medicine
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMedicineResource resource)
        {
            var command = CreateMedicineTransform.ToCommand(resource);
            var id = await _medicineCommandService.Handle(command);
            return Ok(new { id });
        }

        // GET: api/medicine/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var query = new GetAllMedicinesByUserIdQuery(userId);
            var medicines = await _medicineQueryService.Handle(query);
            return Ok(medicines);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteMedicineCommand(id);
            await _medicineCommandService.Handle(command);
            return NoContent();
        }
    }
}