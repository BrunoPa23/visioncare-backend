using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VisionCareCore.HealthCare.Application.Internal.CommandServices;
using VisionCareCore.HealthCare.Application.Internal.QueryServices;
using VisionCareCore.HealthCare.Domain.Model.Commands;
using VisionCareCore.HealthCare.Domain.Queries;
using VisionCareCore.HealthCare.Domain.Services;
using VisionCareCore.HealthCare.Interfaces.Resources;
using VisionCareCore.HealthCare.Interfaces.Transform;
using VisionCareCore.Shared.Infraestructure.Interfaces.ASP.Extensions;

namespace VisionCareCore.HealthCare.Interfaces.Controllers
{
    [Authorize]
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
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var command = CreateMedicineTransform.ToCommand(resource, userId.Value);
            var id = await _medicineCommandService.Handle(command);
            return Ok(new { id });
        }

        // GET: api/medicine/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId is null) return Unauthorized();

            // Un usuario solo puede consultar sus propios medicamentos
            if (userId != currentUserId) return Forbid();

            var query = new GetAllMedicinesByUserIdQuery(userId);
            var medicines = await _medicineQueryService.Handle(query);
            return Ok(medicines);
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = User.GetUserId();
            if (currentUserId is null) return Unauthorized();

            // Se responde 404 tambien si es de otro usuario, para no revelar que existe
            var medicine = await _medicineQueryService.Handle(new GetMedicineByIdQuery(id));
            if (medicine is null || medicine.UserId != currentUserId) return NotFound();

            var command = new DeleteMedicineCommand(id);
            await _medicineCommandService.Handle(command);
            return NoContent();
        }
    }
}